using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Guardian.Animation;
using Gameplay.GameControllers.Entities.StateMachine;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Animator;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Guardian.AI
{
	public class GuardianPrayerBehaviour : MonoBehaviour
	{
		public Entity Master { get; set; }

		public bool VanishFlag { get; set; }

		public bool IdleFlag { get; set; }

		private bool LeftOrientation { get; set; }

		public bool IsTurning { get; set; }

		public EntityOrientation GuessedOrientation { get; private set; }

		private void Awake()
		{
			this.StateMachine = base.GetComponentInChildren<StateMachine>();
		}

		private void OnEnable()
		{
			this.currentState = GuardianPrayerBehaviour.GuardianState.Follow;
			this.Guardian.SpriteRenderer.enabled = true;
			this.SetInitialOrientation();
		}

		private void Start()
		{
			this.Master = Core.Logic.Penitent;
			AnimatorInyector animatorInyector = Core.Logic.Penitent.AnimatorInyector;
			animatorInyector.OnAttack = (Core.SimpleEvent)Delegate.Combine(animatorInyector.OnAttack, new Core.SimpleEvent(this.OnAttack));
			Parry parry = Core.Logic.Penitent.Parry;
			parry.OnParryCast = (Core.SimpleEvent)Delegate.Combine(parry.OnParryCast, new Core.SimpleEvent(this.OnShieldCast));
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnMasterDead));
			this.SetInitialOrientation();
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			this._currentActionCooldown += deltaTime;
			this._currentReverseCoolDown += deltaTime;
			switch (this.currentState)
			{
			case GuardianPrayerBehaviour.GuardianState.Idle:
				if (this.VanishFlag)
				{
					this.SetState(GuardianPrayerBehaviour.GuardianState.Vanish);
				}
				else
				{
					this.StateMachine.SwitchState<GuardianPrayerIdleState>();
				}
				break;
			case GuardianPrayerBehaviour.GuardianState.Follow:
				if (this.VanishFlag)
				{
					this.SetState(GuardianPrayerBehaviour.GuardianState.Vanish);
				}
				else if (this.IdleFlag)
				{
					this.SetState(GuardianPrayerBehaviour.GuardianState.Idle);
				}
				else
				{
					this.StateMachine.SwitchState<GuardianPrayerFollowState>();
				}
				break;
			case GuardianPrayerBehaviour.GuardianState.Attack:
				this._currentActionCooldown = 0f;
				this.StateMachine.SwitchState<GuardianPrayerAttackState>();
				break;
			case GuardianPrayerBehaviour.GuardianState.Guard:
				this._currentActionCooldown = 0f;
				this.StateMachine.SwitchState<GuardianPrayerGuardState>();
				break;
			case GuardianPrayerBehaviour.GuardianState.Vanish:
				this.StateMachine.SwitchState<GuardianPrayerVanishState>();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public void SetState(GuardianPrayerBehaviour.GuardianState state)
		{
			this.currentState = state;
		}

		public Vector3 GetMasterOffSetPosition
		{
			get
			{
				float hspeed = Core.Logic.Penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed;
				if (Mathf.Abs(hspeed) > this.ThresholdSpeed)
				{
					return this.Master.transform.position;
				}
				Vector3 result;
				if (this.Master.Status.Orientation == EntityOrientation.Right)
				{
					result = this.Master.transform.position - this.MasterOffsetPosition;
				}
				else
				{
					result = this.Master.transform.position + this.MasterOffsetPosition;
				}
				return result;
			}
		}

		public float GetActionDirection(float distance)
		{
			Vector3 position = base.transform.position;
			int num = (this.Master.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
			float num2 = Mathf.Clamp(distance, 0f, this.GetMaxDisplacement);
			return position.x + num2 * (float)num;
		}

		private float GetMaxDisplacement
		{
			get
			{
				return Mathf.Abs(this.Master.transform.position.x - base.transform.position.x);
			}
		}

		public void SetInitialOrientation()
		{
			if (!this.Master)
			{
				return;
			}
			this.GuessedOrientation = this.Master.Status.Orientation;
			this.Guardian.SetOrientation(this.GuessedOrientation, true, false);
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
			if (this.currentState != GuardianPrayerBehaviour.GuardianState.Follow || !this.CanPerformAction)
			{
				return;
			}
			if (this.ConsumeActionCooldown)
			{
				this.SetState(GuardianPrayerBehaviour.GuardianState.Attack);
			}
		}

		private void OnShieldCast()
		{
			if (this.currentState != GuardianPrayerBehaviour.GuardianState.Follow || !this.CanPerformAction)
			{
				return;
			}
			if (this.ConsumeActionCooldown)
			{
				this.SetState(GuardianPrayerBehaviour.GuardianState.Guard);
			}
		}

		private void OnMasterDead()
		{
			this.VanishFlag = true;
			this.SetState(GuardianPrayerBehaviour.GuardianState.Vanish);
		}

		private bool ConsumeActionCooldown
		{
			get
			{
				return this._currentActionCooldown >= this.ActionCooldown;
			}
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
				this.FlipGuardian();
			}
			else if (!flip && this.GuessedOrientation != EntityOrientation.Right)
			{
				this.GuessedOrientation = EntityOrientation.Right;
				this.FlipGuardian();
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
				return this.Guardian.Status.Orientation == this.Master.Status.Orientation;
			}
		}

		private void FlipGuardian()
		{
			this.Guardian.AnimationHandler.SetAnimatorTrigger(GuardianPrayerAnimationHandler.TurnTrigger);
			this.Guardian.Audio.PlayTurn();
		}

		[FoldoutGroup("Guardian Skill Settings", 0)]
		[Range(0f, 2f)]
		public float ActionCooldown = 1f;

		[FoldoutGroup("Guardian Motion Settings", 0)]
		[MinMaxSlider(0f, 500f, false)]
		public Vector2 FollowSpeed;

		[FoldoutGroup("Guardian Motion Settings", 0)]
		[MinMaxSlider(0f, 10f, false)]
		public Vector2 FollowDistance;

		[FoldoutGroup("Guardian Motion Settings", 0)]
		[Range(0f, 10f)]
		public float ThresholdSpeed = 3f;

		[FoldoutGroup("Guardian Motion Settings", 0)]
		[Range(0f, 10f)]
		public float FollowingCooldown = 2f;

		[FoldoutGroup("Guardian Motion Settings", 0)]
		public Vector2 MasterOffsetPosition;

		[FoldoutGroup("Guardian Motion Settings", 0)]
		public float SmoothDampElongation = 0.3f;

		[FoldoutGroup("Guardian Motion Settings", 0)]
		[Range(0f, 0.1f)]
		public float FloatingVerticalElongation = 0.25f;

		[FoldoutGroup("Guardian Motion Settings", 0)]
		[Range(1f, 5f)]
		public float FloatingSpeed = 1.5f;

		[FoldoutGroup("Guardian Attack Settings", 0)]
		public float AttackDistance = 3f;

		[FoldoutGroup("Guardian Attack Settings", 0)]
		public float ShieldDistance = 1.5f;

		public GuardianPrayer Guardian;

		private StateMachine StateMachine;

		private GuardianPrayerBehaviour.GuardianState currentState;

		private float _currentActionCooldown;

		private float _reverseCoolDown = 0.25f;

		private float _currentReverseCoolDown;

		public enum GuardianState
		{
			Idle,
			Follow,
			Attack,
			Guard,
			Vanish
		}
	}
}
