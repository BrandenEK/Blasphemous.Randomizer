using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class EnemyAI : MonoBehaviour
	{
		public float RangeGroundDetection
		{
			get
			{
				return this.rangeGroundDetection;
			}
			set
			{
				this.rangeGroundDetection = value;
			}
		}

		public bool GroundSensorHitsFloor { get; private set; }

		private void Awake()
		{
			this.entity = base.GetComponent<Enemy>();
		}

		private void Start()
		{
			BoxCollider2D componentInChildren = base.GetComponentInChildren<BoxCollider2D>();
			this.myWidth = componentInChildren.bounds.extents.x;
			this.myHeight = componentInChildren.bounds.extents.y;
			this.stateTimeTreshold = this.walkTimeLimit;
			if (this.hearingSensor != null)
			{
				this.hearingSensor.OnEntityEnter += this.HearingSensor_OnEntityEnter;
				this.hearingSensor.OnEntityExit += this.HearingSensor_OnEntityExit;
			}
			if (this.visualSensor != null)
			{
				this.visualSensor.OnEntityEnter += this.VisualSensor_OnEntityEnter;
				this.visualSensor.OnEntityExit += this.VisualSensor_OnEntityExit;
			}
			this._penitent = Core.Logic.Penitent;
			this.currentGroundDetection = this.rangeGroundDetection;
			this.maxHitsAllocated = 2;
			this.bottomHits = new RaycastHit2D[this.maxHitsAllocated];
			this.forwardsHits = new RaycastHit2D[this.maxHitsAllocated];
		}

		private void Update()
		{
			this.deltaTurnAroundTime += Time.deltaTime;
			if (this.trapDetected)
			{
				this.deltaTurnAroundTime = 0f;
			}
			if (this.deltaTurnAroundTime <= this.turnAroundTime)
			{
				this.playerSeen = (this.playerHeard = false);
			}
			switch (this.entity.entityCurrentState)
			{
			case EntityStates.Wander:
				this.evaluateWalk();
				break;
			case EntityStates.Attack:
				this.evaluateAttack();
				break;
			case EntityStates.Hurt:
				this.evaluateHurt();
				break;
			case EntityStates.Idle:
				this.evaluateIdle();
				break;
			case EntityStates.Chasing:
				this.evaluateChasing();
				break;
			}
		}

		private void FixedUpdate()
		{
			if (this.entity.Status.Orientation == EntityOrientation.Left)
			{
				Vector2 vector = base.transform.position - (base.transform.right * this.myWidth * 1.5f + Vector2.up * (this.myHeight * 2f));
				Debug.DrawLine(vector, vector - Vector2.up * this.currentGroundDetection, Color.yellow);
				this.GroundSensorHitsFloor = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * this.currentGroundDetection, this.bottomHits, this.enemyLayerMask) > 0);
				Debug.DrawLine(vector, vector - base.transform.right * this.rangeBlockDectection, Color.yellow);
				this.isBlocked = (Physics2D.LinecastNonAlloc(vector, vector - base.transform.right * this.rangeBlockDectection, this.forwardsHits, this.enemyLayerMask) > 0);
			}
			else
			{
				Vector2 vector = base.transform.position + (base.transform.right * this.myWidth * 1.5f - Vector2.up * (this.myHeight * 2f));
				Debug.DrawLine(vector, vector - Vector2.up * this.currentGroundDetection, Color.yellow);
				this.GroundSensorHitsFloor = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * this.currentGroundDetection, this.bottomHits, this.enemyLayerMask) > 0);
				Debug.DrawLine(vector, vector + base.transform.right * this.rangeBlockDectection, Color.yellow);
				this.isBlocked = (Physics2D.LinecastNonAlloc(vector, vector + base.transform.right * this.rangeBlockDectection, this.forwardsHits, this.enemyLayerMask) > 0);
			}
			this.trapDetected = this.detectTrap(this.bottomHits);
			if (this.trapDetected)
			{
				this.stopChasing();
				this.entity.entityCurrentState = EntityStates.Idle;
				this.reverseOrientation();
			}
			if ((!this.GroundSensorHitsFloor || this.isBlocked) && !this.entity.Status.IsHurt)
			{
				if (this.allowEntityOrientation)
				{
					this.reverseOrientation();
				}
				this.stopChasing();
			}
		}

		private void reverseOrientation()
		{
			EntityOrientation orientation = this.entity.Status.Orientation;
			EntityOrientation orientation2 = (orientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right;
			this.entity.SetOrientation(orientation2, true, false);
		}

		public void ResetStateTime()
		{
			if (this.deltaStateCounter > 0f)
			{
				this.deltaStateCounter = 0f;
			}
		}

		private void lookAtTarget(Vector3 targetPos)
		{
			if (this.entity.Status.Dead)
			{
				return;
			}
			this.deltaTargetTime += Time.deltaTime;
			if (this.deltaTargetTime >= this.targetTime)
			{
				this.deltaTargetTime = 0f;
				if (this.entity.transform.position.x >= targetPos.x + 1f)
				{
					if (this.entity.Status.Orientation != EntityOrientation.Left)
					{
						this.entity.SetOrientation(EntityOrientation.Left, true, false);
					}
				}
				else if (this.entity.transform.position.x < targetPos.x - 1f && this.entity.Status.Orientation != EntityOrientation.Right)
				{
					this.entity.SetOrientation(EntityOrientation.Right, true, false);
				}
			}
		}

		private bool detectTrap(RaycastHit2D[] hits)
		{
			bool result = false;
			foreach (RaycastHit2D raycastHit2D in hits)
			{
				if (!(raycastHit2D.collider == null))
				{
					if (raycastHit2D.collider.gameObject.layer == LayerMask.NameToLayer("Trap"))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private void evaluateWalk()
		{
			this.deltaStateCounter += Time.deltaTime;
			if (this.deltaStateCounter >= this.stateTimeTreshold)
			{
				this.deltaStateCounter = 0f;
				this.stateTimeTreshold = this.idleTimeLimit;
				this.entity.entityCurrentState = EntityStates.Idle;
			}
			if ((this.playerSeen || this.playerHeard) && this.entity.entityCurrentState != EntityStates.Chasing && !this._penitent.Status.Dead)
			{
				this.stateTimeTreshold = this.timeChasing;
				this.entity.entityCurrentState = EntityStates.Chasing;
			}
		}

		private void evaluateIdle()
		{
			this.deltaStateCounter += Time.deltaTime;
			if (this.deltaStateCounter >= this.stateTimeTreshold)
			{
				this.deltaStateCounter = 0f;
				this.stateTimeTreshold = this.walkTimeLimit;
				this.entity.entityCurrentState = EntityStates.Wander;
			}
			if ((this.playerSeen || this.playerHeard) && this.entity.entityCurrentState != EntityStates.Chasing && !this._penitent.Status.Dead)
			{
				this.entity.entityCurrentState = EntityStates.Chasing;
			}
		}

		private void evaluateChasing()
		{
			if (this._penitent.Status.Dead)
			{
				this.stateTimeTreshold = this.idleTimeLimit;
				this.entity.entityCurrentState = EntityStates.Idle;
				return;
			}
			this.currentGroundDetection = this.maxRangeGroundDetection;
			if (this._penitent != null)
			{
				this.lookAtTarget(this._penitent.transform.position);
			}
			if (this.entity.EntityAttack.IsEnemyHit)
			{
				this.deltaStateCounter = 0f;
				this.stateTimeTreshold = this.idleTimeLimit;
				this.entity.entityCurrentState = EntityStates.Attack;
			}
			if (!this.playerSeen && !this.playerHeard)
			{
				this.deltaStateCounter += Time.deltaTime;
				if (this.deltaStateCounter >= this.timeChasing)
				{
					this.currentGroundDetection = this.rangeGroundDetection;
					this.deltaStateCounter = 0f;
					this.stateTimeTreshold = this.walkTimeLimit;
					this.entity.entityCurrentState = EntityStates.Wander;
				}
			}
		}

		private void evaluateAttack()
		{
			if (this._penitent != null && this._penitent.Status.Dead)
			{
				this.stateTimeTreshold = this.idleTimeLimit;
				this.entity.entityCurrentState = EntityStates.Idle;
				return;
			}
			if (!this.entity.EntityAttack.IsEnemyHit)
			{
				this.deltaAttackTime += Time.deltaTime;
				if (this.deltaAttackTime < this.attackTime)
				{
					return;
				}
				this.deltaAttackTime = 0f;
				this.stateTimeTreshold = this.timeChasing;
				this.entity.entityCurrentState = EntityStates.Chasing;
			}
			else
			{
				this.deltaAttackTime = 0f;
			}
		}

		private void evaluateHurt()
		{
			this.deltaStateCounter += Time.deltaTime;
			if (this.deltaStateCounter < this.hurtTime)
			{
				return;
			}
			this.deltaStateCounter = 0f;
			this.stateTimeTreshold = this.timeChasing;
			this.entity.entityCurrentState = EntityStates.Chasing;
		}

		private void stopChasing()
		{
			if (this.entity.entityCurrentState != EntityStates.Chasing)
			{
				return;
			}
			this.playerHeard = false;
			this.playerSeen = false;
			this.deltaStateCounter = this.timeChasing;
			this.currentGroundDetection = this.rangeGroundDetection;
		}

		private void VisualSensor_OnEntityExit(Entity entity)
		{
			this.playerSeen = false;
		}

		private void VisualSensor_OnEntityEnter(Entity entity)
		{
			this.playerSeen = true;
		}

		private void HearingSensor_OnEntityExit(Entity entity)
		{
			this.playerHeard = false;
		}

		private void HearingSensor_OnEntityEnter(Entity entity)
		{
			this.playerHeard = true;
		}

		private Penitent _penitent;

		public bool allowEntityOrientation;

		public float attackTime = 1f;

		private RaycastHit2D[] bottomHits;

		private RaycastHit2D[] forwardsHits;

		private float currentGroundDetection;

		private float deltaAttackTime;

		private float deltaStateCounter;

		private float deltaTargetTime;

		private float deltaTurnAroundTime;

		public LayerMask enemyLayerMask;

		private Enemy entity;

		[SerializeField]
		private CollisionSensor hearingSensor;

		public float hurtTime = 2f;

		public float idleTimeLimit = 5f;

		private bool isBlocked;

		private int maxHitsAllocated;

		public float maxRangeGroundDetection = 5f;

		[Header("Sensors variables")]
		private float myWidth;

		[Header("Sensors variables")]
		private float myHeight;

		private bool playerHeard;

		private Vector3 playerPosition;

		private bool playerSeen;

		[Tooltip("The length of the block detection raycast")]
		[Range(0f, 1f)]
		public float rangeBlockDectection = 0.5f;

		[Tooltip("The length og the ground detection raycast")]
		[Range(0f, 10f)]
		public float rangeGroundDetection = 2f;

		public float speed = 1f;

		private float stateTimeTreshold;

		private readonly float targetTime;

		[Header("Behaviour time variables")]
		public float timeChasing = 1f;

		private bool trapDetected;

		private readonly float turnAroundTime = 1.5f;

		[SerializeField]
		private CollisionSensor visualSensor;

		public float walkTimeLimit = 10f;
	}
}
