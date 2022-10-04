using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Entities.MiriamPortal.Animation;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Animator;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.MiriamPortal.AI
{
	public class MiriamPortalPrayerBehaviour : MonoBehaviour
	{
		public Entity Master { get; set; }

		public bool VanishFlag { get; set; }

		public bool IdleFlag { get; set; }

		public bool ReappearFlag { get; set; }

		public bool IsTurning { get; set; }

		public EntityOrientation GuessedOrientation { get; private set; }

		private bool LeftOrientation { get; set; }

		private void Awake()
		{
			this.StateMachine = base.GetComponentInChildren<StateMachine>();
		}

		private void OnEnable()
		{
			this.VanishFlag = false;
			this.MiriamPortal.SpriteRenderer.enabled = true;
			this.SetInitialOrientation();
		}

		private void Start()
		{
			this.Master = Core.Logic.Penitent;
			this.SetInitialOrientation();
			this.results = new RaycastHit2D[1];
			PoolManager.Instance.CreatePool(this.PortalShatteringVfx, 1);
			PoolManager.Instance.CreatePool(this.LandingVfx, 1);
			AnimatorInyector animatorInyector = Core.Logic.Penitent.AnimatorInyector;
			animatorInyector.OnAttack = (Core.SimpleEvent)Delegate.Combine(animatorInyector.OnAttack, new Core.SimpleEvent(this.OnAttack));
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnMasterDead));
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			this._currentReverseCoolDown += deltaTime;
			this._currentAttackCoolDown += deltaTime;
			switch (this.currentState)
			{
			case MiriamPortalPrayerBehaviour.MiriamPortalState.Idle:
				if (this.VanishFlag)
				{
					this.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Vanish);
				}
				else
				{
					this.StateMachine.SwitchState<MiriamPortalPrayerIdleState>();
				}
				break;
			case MiriamPortalPrayerBehaviour.MiriamPortalState.Follow:
				if (this.VanishFlag)
				{
					this.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Vanish);
				}
				else if (this.IdleFlag)
				{
					this.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Idle);
				}
				else if (this.ReappearFlag)
				{
					this.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Idle);
				}
				else
				{
					this.StateMachine.SwitchState<MiriamPortalPrayerFollowState>();
				}
				break;
			case MiriamPortalPrayerBehaviour.MiriamPortalState.Attack:
				if (this._currentAttackCoolDown < this._attackCoolDown)
				{
					return;
				}
				this.StateMachine.SwitchState<MiriamPortalPrayerAttackState>();
				break;
			case MiriamPortalPrayerBehaviour.MiriamPortalState.Vanish:
				if (this.ReappearFlag)
				{
					this.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Idle);
				}
				this.StateMachine.SwitchState<MiriamPortalPrayerVanishState>();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public void SetState(MiriamPortalPrayerBehaviour.MiriamPortalState state)
		{
			this.currentState = state;
		}

		public Vector3 GetMasterOffSetPosition
		{
			get
			{
				Vector3 position = this.Master.transform.position;
				position.y += this.MasterOffsetPosition.y;
				float hspeed = Core.Logic.Penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed;
				if (Mathf.Abs(hspeed) < this.ThresholdSpeed)
				{
					if (this.Master.Status.Orientation == EntityOrientation.Right)
					{
						position.x -= this.MasterOffsetPosition.x;
					}
					else
					{
						position.x += this.MasterOffsetPosition.x;
					}
				}
				return position;
			}
		}

		public Vector2 GetActionDirection(bool checkToHitGorund)
		{
			int num = (this.MiriamPortal.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
			float x = base.transform.position.x + this.HorizontalAttackDistance * (float)num;
			float y = base.transform.position.y - this.VerticalAttackDistance;
			if (checkToHitGorund && this.CanHitGround())
			{
				this.reachedGround = true;
				y = this.results[0].point.y + 0.3f;
			}
			return new Vector2(x, y);
		}

		private bool CanHitGround()
		{
			Vector2 actionDirection = this.GetActionDirection(false);
			if (Physics2D.RaycastNonAlloc(actionDirection, Vector2.down, this.results, this.MaxDistanceToHitGround, this.FloorMask) > 0)
			{
				Debug.DrawLine(actionDirection, this.results[0].point, Color.cyan, 3f);
				return true;
			}
			return false;
		}

		public void CheckAndSpawnLandingVfx()
		{
			if (!this.reachedGround)
			{
				return;
			}
			Vector3 position = base.transform.position;
			position.x += ((this.MiriamPortal.Status.Orientation != EntityOrientation.Right) ? -0.7f : 0.7f);
			position.y -= 0.6f;
			PoolManager.Instance.ReuseObject(this.LandingVfx, position, Quaternion.identity, false, 1);
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("HardFall");
			position.y += 5f;
			this.Pillar.SetDamageStrength(this.MiriamPortal.Stats.Strength.Final * Core.Logic.Penitent.Stats.DamageMultiplier.Final * 0.2f);
			Vector2 v = (this.MiriamPortal.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this.Pillar.SummonAreas(position, v, this.MiriamPortal.Status.Orientation);
		}

		public void SetInitialOrientation()
		{
			if (!this.Master)
			{
				return;
			}
			this.GuessedOrientation = this.Master.Status.Orientation;
			this.MiriamPortal.SetOrientation(this.GuessedOrientation, true, false);
		}

		public void LookAtMaster()
		{
			if (this.Master == null || this._currentReverseCoolDown < this._reverseCoolDown)
			{
				return;
			}
			this._currentReverseCoolDown = 0f;
			float num = (this.Master.Status.Orientation != EntityOrientation.Left) ? 0.5f : -0.5f;
			this.LeftOrientation = (base.transform.position.x > this.Master.transform.position.x + num);
			this.SetOrientation(this.LeftOrientation);
		}

		private void OnAttack()
		{
			if (Core.Logic.Penitent.Animator.GetCurrentAnimatorStateInfo(0).IsName("HardLanding"))
			{
				return;
			}
			if (this.currentState != MiriamPortalPrayerBehaviour.MiriamPortalState.Follow || !this.CanPerformAction)
			{
				return;
			}
			this._currentAttackCoolDown = 0f;
			this.reachedGround = false;
			this.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Attack);
		}

		private void OnMasterDead()
		{
			this.VanishFlag = true;
			this.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Vanish);
		}

		private bool CanPerformAction
		{
			get
			{
				return !this.IsTurning && this.IsAlignedWithMaster;
			}
		}

		private void SetOrientation(bool flip)
		{
			if (flip && this.GuessedOrientation != EntityOrientation.Left)
			{
				this.GuessedOrientation = EntityOrientation.Left;
				this.FlipMiriamPortal();
			}
			else if (!flip && this.GuessedOrientation != EntityOrientation.Right)
			{
				this.GuessedOrientation = EntityOrientation.Right;
				this.FlipMiriamPortal();
			}
		}

		public float GetMasterDistance
		{
			get
			{
				return Vector2.Distance(this.Master.transform.position, base.transform.position);
			}
		}

		public bool IsAlignedWithMaster
		{
			get
			{
				return this.MiriamPortal.Status.Orientation == this.Master.Status.Orientation;
			}
		}

		private void FlipMiriamPortal()
		{
			this.MiriamPortal.AnimationHandler.SetAnimatorTrigger(MiriamPortalPrayerAnimationHandler.TurnTrigger);
			this.MiriamPortal.Audio.PlayTurn();
		}

		[FoldoutGroup("Motion Settings", 0)]
		[MinMaxSlider(0f, 500f, false)]
		public Vector2 FollowSpeed;

		[FoldoutGroup("Motion Settings", 0)]
		[MinMaxSlider(0f, 10f, false)]
		public Vector2 FollowDistance;

		[FoldoutGroup("Motion Settings", 0)]
		[Range(0f, 10f)]
		public float ThresholdSpeed = 3f;

		[FoldoutGroup("Motion Settings", 0)]
		[Range(0f, 10f)]
		public float FollowingCooldown = 2f;

		[FoldoutGroup("Motion Settings", 0)]
		public Vector2 MasterOffsetPosition;

		[FoldoutGroup("Motion Settings", 0)]
		public float SmoothDampElongation = 0.3f;

		[FoldoutGroup("Motion Settings", 0)]
		[Range(0f, 0.1f)]
		public float FloatingVerticalElongation = 0.25f;

		[FoldoutGroup("Motion Settings", 0)]
		[Range(1f, 5f)]
		public float FloatingSpeed = 1.5f;

		[FoldoutGroup("Attack Settings", 0)]
		public float HorizontalAttackDistance = 4f;

		[FoldoutGroup("Attack Settings", 0)]
		public float VerticalAttackDistance = 1f;

		[FoldoutGroup("Attack Settings", 0)]
		public LayerMask FloorMask;

		[FoldoutGroup("Attack Settings", 0)]
		public BossAreaSummonAttack Pillar;

		[FoldoutGroup("Attack Settings", 0)]
		public float MaxDistanceToHitGround = 9f;

		public MiriamPortalPrayer MiriamPortal;

		public GameObject PortalShatteringVfx;

		public GameObject LandingVfx;

		private MiriamPortalPrayerBehaviour.MiriamPortalState currentState;

		private float _reverseCoolDown = 0.25f;

		private float _attackCoolDown = 0.2f;

		private float _currentReverseCoolDown;

		private float _currentAttackCoolDown;

		private StateMachine StateMachine;

		private RaycastHit2D[] results;

		private bool reachedGround;

		public enum MiriamPortalState
		{
			Idle,
			Follow,
			Attack,
			Vanish
		}
	}
}
