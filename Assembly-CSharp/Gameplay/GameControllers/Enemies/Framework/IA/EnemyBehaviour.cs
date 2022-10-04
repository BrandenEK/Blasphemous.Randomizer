using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public abstract class EnemyBehaviour : MonoBehaviour
	{
		public bool PlayerHeard { get; protected set; }

		public bool PlayerSeen { get; protected set; }

		public bool TurningAround { get; set; }

		public bool SensorHitsFloor { get; protected set; }

		public bool GotParry { get; set; }

		protected BehaviourTreeOwner BehaviourTree { get; set; }

		public bool IsGrounded
		{
			get
			{
				return this.SensorHitsFloor;
			}
		}

		public bool IsHurt
		{
			get
			{
				return this.Entity.Status.IsHurt;
			}
			set
			{
				this.Entity.Status.IsHurt = value;
			}
		}

		public bool IsBlocked
		{
			get
			{
				return this.isBlocked;
			}
		}

		public bool IsTrapDetected
		{
			get
			{
				return this.TrapDetected;
			}
		}

		public bool IsChasing
		{
			get
			{
				return this.Entity.IsChasing;
			}
			set
			{
				this.Entity.IsChasing = value;
			}
		}

		public bool IsAttacking
		{
			get
			{
				return this.Entity.IsAttacking;
			}
		}

		public Collider2D GetVisualSensor()
		{
			return this.VisualSensor.SensorCollider2D;
		}

		public Collider2D GetHearingSensor()
		{
			return this.HearingSensor.SensorCollider2D;
		}

		public bool IsPlayerHeard()
		{
			return this.PlayerHeard;
		}

		public bool IsPlayerSeen()
		{
			return this.PlayerSeen;
		}

		public bool IsTurningAround()
		{
			return this.TurningAround;
		}

		public bool IsDead()
		{
			return this.Entity.Status.Dead;
		}

		private void Awake()
		{
			this.Entity = base.GetComponent<Enemy>();
			this.BehaviourTree = base.GetComponent<BehaviourTreeOwner>();
			this.OnAwake();
		}

		public virtual void OnAwake()
		{
		}

		private void Start()
		{
			if (this.HearingSensor != null)
			{
				this.HearingSensor.SensorTriggerStay += this.HearingSensorOnTriggerStay;
				this.HearingSensor.OnEntityExit += this.HearingSensor_OnEntityExit;
			}
			if (this.VisualSensor != null)
			{
				this.VisualSensor.SensorTriggerStay += this.VisualSensorOnTriggerStay;
				this.VisualSensor.OnEntityExit += this.VisualSensor_OnEntityExit;
			}
			this.OnStart();
		}

		public virtual void OnStart()
		{
		}

		private void Update()
		{
			this.DeltaTargetTime += Time.deltaTime;
			if (!this.EnableBehaviourOnLoad)
			{
				this.StopBehaviour();
				return;
			}
			this.OnUpdate();
		}

		public virtual void OnUpdate()
		{
		}

		private void FixedUpdate()
		{
			this.OnFixedUpdate();
		}

		public virtual void OnFixedUpdate()
		{
		}

		public virtual void ReverseOrientation()
		{
			EntityOrientation orientation = this.Entity.Status.Orientation;
			EntityOrientation orientation2 = (orientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right;
			this.Entity.SetOrientation(orientation2, true, false);
		}

		public virtual void LookAtTarget(Vector3 targetPos)
		{
			if (this.Entity.Status.Dead)
			{
				return;
			}
			this.DeltaTargetTime += Time.deltaTime;
			if (this.DeltaTargetTime >= this.targetTime)
			{
				this.DeltaTargetTime = 0f;
				if (this.Entity.transform.position.x >= targetPos.x + 1f)
				{
					if (this.Entity.Status.Orientation != EntityOrientation.Left)
					{
						if (this.OnTurning != null)
						{
							this.OnTurning();
						}
						this.Entity.SetOrientation(EntityOrientation.Left, true, false);
					}
				}
				else if (this.Entity.transform.position.x < targetPos.x - 1f && this.Entity.Status.Orientation != EntityOrientation.Right)
				{
					if (this.OnTurning != null)
					{
						this.OnTurning();
					}
					this.Entity.SetOrientation(EntityOrientation.Right, true, false);
				}
			}
		}

		protected void EnableColliders(bool enableCollider = true)
		{
			Collider2D[] componentsInChildren = this.Entity.GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider2D in componentsInChildren)
			{
				collider2D.enabled = enableCollider;
			}
		}

		public bool DetectTrap(RaycastHit2D[] hits)
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

		public Transform GetTarget()
		{
			if (!this.Entity)
			{
				this.Entity = base.GetComponentInParent<Enemy>();
			}
			if (this.Entity.Target)
			{
				return this.Entity.Target.transform;
			}
			if (Core.Logic.Penitent)
			{
				this.Entity.Target = Core.Logic.Penitent.gameObject;
				return this.Entity.Target.transform;
			}
			Debug.LogError("ERROR: Penitent reference cant be accesed yet");
			Penitent penitent = Core.Logic.Penitent;
			if (!penitent)
			{
				return null;
			}
			Transform transform = penitent.transform;
			this.Entity.Target = transform.gameObject;
			return transform;
		}

		private void VisualSensorOnTriggerStay(Collider2D objectCollider)
		{
			if (!this.PlayerSeen)
			{
				this.PlayerSeen = true;
			}
		}

		private void HearingSensorOnTriggerStay(Collider2D objectCollider)
		{
			if (!this.PlayerHeard)
			{
				this.PlayerHeard = true;
			}
		}

		private void VisualSensor_OnEntityExit(Entity entity)
		{
			if (this.PlayerSeen)
			{
				this.PlayerSeen = !this.PlayerSeen;
			}
		}

		private void HearingSensor_OnEntityExit(Entity entity)
		{
			if (this.PlayerHeard)
			{
				this.PlayerHeard = !this.PlayerHeard;
			}
		}

		public void StartBehaviour()
		{
			if (this.BehaviourTree == null)
			{
				return;
			}
			if (!this.BehaviourTree.isRunning)
			{
				this.BehaviourTree.StartBehaviour();
			}
		}

		public void PauseBehaviour()
		{
			if (this.BehaviourTree == null)
			{
				return;
			}
			if (this.BehaviourTree.isRunning)
			{
				this.BehaviourTree.PauseBehaviour();
			}
		}

		public void StopBehaviour()
		{
			if (this.BehaviourTree == null)
			{
				return;
			}
			if (this.BehaviourTree.isRunning)
			{
				this.BehaviourTree.StopBehaviour();
			}
		}

		public abstract void Idle();

		public abstract void Wander();

		public abstract void Chase(Transform targetPosition);

		public abstract void Attack();

		public abstract void Damage();

		public abstract void StopMovement();

		public virtual void ReadSpawnerConfig(SpawnBehaviourConfig config)
		{
		}

		public virtual void Parry()
		{
		}

		public virtual void Alive()
		{
			if (this.Entity.IsStunt)
			{
				this.Entity.IsStunt = false;
			}
		}

		public virtual void Execution()
		{
			if (!this.Entity.IsStunt)
			{
				this.Entity.IsStunt = true;
			}
		}

		public LayerMask BlockLayerMask;

		public bool EnableBehaviourOnLoad = true;

		protected float DeltaTargetTime;

		protected Enemy Entity;

		[SerializeField]
		protected CollisionSensor HearingSensor;

		protected bool isBlocked;

		public Core.SimpleEvent OnTurning;

		private readonly float targetTime;

		protected bool TrapDetected;

		[SerializeField]
		protected CollisionSensor VisualSensor;
	}
}
