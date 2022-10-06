using System;
using CreativeSpore.SmartColliders;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.InputSystem;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Animator
{
	[RequireComponent(typeof(PlatformCharacterController))]
	public class AnimatorInyector : MonoBehaviour
	{
		public bool FireJumpOffTrigger { get; set; }

		public bool ForwardJump { get; set; }

		public bool IsJumpingForward { get; set; }

		public bool IsAirAttacking { get; set; }

		public bool IsDashAnimRunning { get; private set; }

		public bool IsJumpWhileDashing { get; set; }

		public bool IsFalling { get; private set; }

		public float TimeGrounded { get; private set; }

		private bool IsDemakeMode
		{
			get
			{
				return Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE);
			}
		}

		private void Awake()
		{
			this._platformCharacterController = base.GetComponent<PlatformCharacterController>();
			this._playerInput = base.GetComponent<PlatformCharacterInput>();
			this._penitent = base.GetComponent<Penitent>();
			this._playerDash = this._penitent.GetComponentInChildren<Dash>();
		}

		private void Start()
		{
			this._setDeadAnimation = false;
			this._setImpaledAnimation = false;
		}

		private void Update()
		{
			this._deltaAttackTime += Time.deltaTime;
			this._penitent.Status.IsGrounded = this._platformCharacterController.IsGrounded;
			this._isGrounded = this._penitent.Status.IsGrounded;
			this._penitent.SlopeAngle = this._platformCharacterController.SmartPlatformCollider.CalculateSlopeAngle();
			this.CheckStuntFall();
			this._penitent.WatchBelow = (this._playerInput.RVerticalAxis <= -0.2f && this._penitent.Status.IsGrounded);
			this.JoystickAxisInput();
			this.UpdateActions();
		}

		private void JoystickAxisInput()
		{
			this.SpriteAnimator.SetBool(AnimatorInyector.AxisThreshold, this._playerInput.ReachAxisThreshold);
			this.SpriteAnimator.SetBool(AnimatorInyector.JoystickUp, this._playerInput.FVerAxis > 0f && this._playerInput.FVerAxis > this._playerInput.AxisMovingThreshold);
			this.SpriteAnimator.SetBool(AnimatorInyector.JoystickDown, this._playerInput.FVerAxis < 0f && Mathf.Abs(this._playerInput.FVerAxis) > this._playerInput.AxisMovingThreshold);
		}

		private void EnableAnimator(bool enable = true)
		{
			this.SpriteAnimator.enabled = enable;
		}

		private void UpdateActions()
		{
			if (!this.SpriteAnimator)
			{
				return;
			}
			this.SpriteAnimator.SetBool(AnimatorInyector.Grounded, this._isGrounded);
			this.SpriteAnimator.SetBool(AnimatorInyector.CanJumpOff, this._penitent.FloorChecker.OneWayDownCollision && this._penitent.isJumpOffReady);
			this.SpriteAnimator.SetBool(AnimatorInyector.IsJumpingOff, this._penitent.IsJumpingOff || this._playerInput.IsJumpOff);
			this.SpriteAnimator.SetBool(AnimatorInyector.CanAirAttack, !this._isGrounded);
			this.SpriteAnimator.SetBool(AnimatorInyector.Ladder, this._penitent.IsClimbingLadder);
			this.SpriteAnimator.SetBool(AnimatorInyector.IsGrabbingLadder, this._penitent.IsGrabbingLadder);
			this.SpriteAnimator.SetBool(AnimatorInyector.IsHurt, this._penitent.Status.IsHurt);
			this.SpriteAnimator.SetBool(AnimatorInyector.Dead, this._penitent.Status.Dead);
			this.SpriteAnimator.SetBool(AnimatorInyector.Demake, this.IsDemakeMode);
			if (this._isGrounded)
			{
				this.TimeGrounded += Time.deltaTime;
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.AirAttackParam);
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.ClimbCliffLedge);
				this.SpriteAnimator.SetBool(AnimatorInyector.IsGrabbingCliffLede, false);
				this.SpriteAnimator.SetBool(AnimatorInyector.AirAttacking, false);
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.Unhang);
				this.IsDashAnimRunning = this.SpriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dash");
				this._penitent.IsGrabbingCliffLede = false;
				this._penitent.IsStickedOnWall = false;
				this._penitent.CanJumpFromLadder = false;
				this._penitent.JumpFromLadder = false;
				this.IsJumpingForward = false;
				this.IsAirAttacking = false;
				this._isJumpReady = true;
				this.ForwardJump = false;
				this.IsJumpWhileDashing = false;
				this.Dashing();
				this.GroundAttack();
				this.ChargedAttack();
				this.Crouch();
			}
			else
			{
				this.TimeGrounded = 0f;
				this.FireJumpOffTrigger = false;
				if (!this._penitent.IsClimbingLadder)
				{
					this.SpriteAnimator.ResetTrigger(AnimatorInyector.Jump);
				}
				if (this._penitent.IsDashing)
				{
					this._penitent.Dash.StopCast();
				}
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.JumpOff);
				this.SpriteAnimator.SetBool(AnimatorInyector.CanJumpOff, this._isGrounded);
				this.SpriteAnimator.SetBool(AnimatorInyector.IsCrouch, this._isGrounded);
				this.SpriteAnimator.SetBool(AnimatorInyector.IsGrabbingCliffLede, this._penitent.IsGrabbingCliffLede);
				this.IsJumpingForward = (this.SpriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump Forward") || this.SpriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Falling Forward"));
				this.IsAirAttacking = this.SpriteAnimator.GetBool(AnimatorInyector.AirAttacking);
				this.IsDashAnimRunning = false;
				this.AirAttack();
				this.ClimbingLadder();
				this.LadderSliding();
				this.ClimbingCliffLede();
			}
			this.Running();
			this.Jumping();
			this.IsClimbingLadder();
			this.IsDead();
		}

		public void UpdateAirAttackingAction()
		{
			this.IsAirAttacking = this.SpriteAnimator.GetBool(AnimatorInyector.AirAttacking);
		}

		private void SetOrientation(EntityOrientation playerOrientation)
		{
			if (this._penitent)
			{
				this._penitent.SetOrientation(playerOrientation, true, false);
			}
		}

		private EntityOrientation GetPlayerInputOrientation
		{
			get
			{
				return (!this._playerInput.faceRight) ? EntityOrientation.Left : EntityOrientation.Right;
			}
		}

		private void Running()
		{
			if (!this.SpriteAnimator)
			{
				return;
			}
			if (!this.IsDashAnimRunning && !this._penitent.Status.IsHurt && !this._penitent.IsClimbingLadder)
			{
				EntityOrientation getPlayerInputOrientation = this.GetPlayerInputOrientation;
				float absHorAxis = this._playerInput.AbsHorAxis;
				if (this._penitent.Status.Orientation != getPlayerInputOrientation && absHorAxis > 0f)
				{
					this.SetOrientation(getPlayerInputOrientation);
				}
			}
			this._startRun = (this._playerInput.ReachAxisThreshold && this._isGrounded);
			this.SpriteAnimator.SetBool(AnimatorInyector.RunStep, this._startRun);
			this.SpriteAnimator.SetBool(AnimatorInyector.RunningParam, this._startRun);
		}

		public void RaiseAttackEvent()
		{
			if (this.OnAttack != null)
			{
				this.OnAttack();
			}
		}

		private void GroundAttack()
		{
			if (this._playerInput.Attack && !this._playerInput.Jump)
			{
				this._deltaAttackTime = 0f;
				this.SpriteAnimator.SetTrigger(AnimatorInyector.Attack);
			}
			else if (this._deltaAttackTime >= 0.05f)
			{
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.Attack);
			}
		}

		private void AirAttack()
		{
			if (this.IsDemakeMode && this._playerInput.isJoystickUp)
			{
				return;
			}
			if (this._playerInput.Attack)
			{
				this.SpriteAnimator.SetTrigger(AnimatorInyector.AirAttackParam);
			}
		}

		private void ChargedAttack()
		{
			if (this._playerInput.Blocked)
			{
				return;
			}
			this.SpriteAnimator.SetBool(AnimatorInyector.IsAttackCharged, this._penitent.IsAttackCharged);
			this.SpriteAnimator.SetBool(AnimatorInyector.IsAttackHold, this._playerInput.IsAttackButtonHold);
			this.ChargeAttackTriggered();
		}

		private void ChargeAttackTriggered()
		{
			if (this._playerInput.IsAttackButtonHold && !this._penitent.ReleaseChargedAttack && this._penitent.ChargedAttack.IsAvailableSkilledAbility)
			{
				if (!this._isChargeAttackTriggered)
				{
					this._isChargeAttackTriggered = true;
					this.SpriteAnimator.SetTrigger(AnimatorInyector.ChargeAttack);
				}
			}
			else
			{
				this._isChargeAttackTriggered = false;
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.ChargeAttack);
			}
		}

		public bool IsHoldingChargeAttack
		{
			get
			{
				return this._penitent.ChargedAttack.IsChargingAttack || this._penitent.ReleaseChargedAttack;
			}
		}

		private void Jumping()
		{
			if (!this.SpriteAnimator)
			{
				return;
			}
			if (!this._isGrounded && this._isJumpReady && !this.IsFalling && !this._penitent.IsClimbingCliffLede && !this._playerInput.isJoystickDown && !this._penitent.StepOnLadder && !this._penitent.JumpFromLadder)
			{
				if (this.SpriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("ComboFinisherUp"))
				{
					return;
				}
				this._isJumpReady = !this._isJumpReady;
				string trigger = (!this._playerInput.ReachAxisThreshold) ? "JUMP" : "FORWARD_JUMP";
				if (this.IsJumpWhileDashing)
				{
					trigger = "FORWARD_JUMP";
				}
				this.SpriteAnimator.SetTrigger(trigger);
				this._penitent.OnJumpTrigger(this);
			}
			else if (!this._isGrounded && this.IsFalling)
			{
				this.SpriteAnimator.SetBool(AnimatorInyector.RunningParam, this._playerInput.ReachAxisThreshold);
			}
			this.IsJumping();
		}

		public void CancelJumpOff()
		{
			this._penitent.IsJumpingOff = false;
		}

		private void IsJumping()
		{
			int shortNameHash = this.SpriteAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
			bool isJumping = shortNameHash == Animator.StringToHash("Jump") || shortNameHash == Animator.StringToHash("Falling") || shortNameHash == Animator.StringToHash("Jump Forward") || shortNameHash == Animator.StringToHash("Falling Forward");
			this._penitent.IsJumping = isJumping;
		}

		private void CheckStuntFall()
		{
			if (!this._isGrounded)
			{
				float vspeed = this._platformCharacterController.PlatformCharacterPhysics.VSpeed;
				this.IsFalling = (vspeed <= -0.1f);
				if (vspeed <= -this.MaxVSpeedFallStunt && !this.IsDemakeMode)
				{
					this._penitent.IsFallingStunt = true;
				}
			}
			else
			{
				this.IsFalling = false;
			}
			this.SpriteAnimator.SetBool(AnimatorInyector.Falling, this.IsFalling);
		}

		public void ResetStuntByFall()
		{
			if (this._penitent.IsFallingStunt)
			{
				this._penitent.IsFallingStunt = false;
			}
		}

		private void Dashing()
		{
			if (this._playerInput.Dash && !this._playerInput.Jump && this._penitent.Dash.enabled && this._playerDash.ReadyToUse && !this._penitent.IsGrabbingCliffLede && !this._penitent.Status.IsHurt && !this._penitent.Status.Dead && !this._penitent.Dash.StandUpAfterDash && !this._penitent.IsChargingAttack && !this._penitent.PlatformCharacterInput.Blocked && !this._penitent.IsFallingStunt)
			{
				this.SpriteAnimator.SetTrigger(AnimatorInyector.Dash);
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.Jump);
				this.SpriteAnimator.SetBool(AnimatorInyector.DashingParam, true);
				this._playerDash.Cast();
			}
			this.SpriteAnimator.SetBool(AnimatorInyector.DashingParam, this.SpriteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
		}

		private void ClimbingLadder()
		{
			if (this._penitent.IsClimbingLadder)
			{
				this._penitent.JumpFromLadder = false;
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.Jump);
				this._penitent.CanJumpFromLadder = true;
				if (this._playerInput.Jump && !this.ForwardJump)
				{
					this.SpriteAnimator.ResetTrigger(AnimatorInyector.AirAttackParam);
					this.ForwardJump = true;
					this._penitent.IsGrabbingLadder = false;
				}
			}
			else
			{
				this._penitent.CanJumpFromLadder = false;
			}
		}

		private void LadderSliding()
		{
			bool flag = false;
			if (this._penitent.IsOnLadder)
			{
				flag = (this._playerInput.isJoystickDown && this._playerInput.IsJumpButtonHold);
			}
			this._penitent.IsLadderSliding = flag;
			this.SpriteAnimator.SetBool(AnimatorInyector.Sliding, flag);
		}

		private void ClimbingCliffLede()
		{
			if (this._playerInput.isJoystickUp && this._penitent.canClimbCliffLede && this._penitent.cliffLedeClimbingStarted)
			{
				this.ClimbCliffLede();
			}
			if (this._penitent.IsClimbingCliffLede && this._playerInput.unHang)
			{
				this.ReleaseCliffLede();
			}
		}

		public void ClimbCliffLede()
		{
			this._penitent.canClimbCliffLede = false;
			this._penitent.transform.position = this._penitent.RootMotionDrive;
			this.SpriteAnimator.Play(this._climbCliffAnim);
		}

		public void ReleaseCliffLede()
		{
			this._penitent.GrabCliffLede.ReleaseCliffLede();
			this.SpriteAnimator.SetTrigger(AnimatorInyector.Unhang);
			if (this._penitent.CanLowerCliff)
			{
				this._penitent.CanLowerCliff = false;
			}
		}

		public void ManualHangOffCliff()
		{
			this._penitent.Physics.EnablePhysics(true);
			this._penitent.CanLowerCliff = true;
			float num = (this._penitent.Status.Orientation != EntityOrientation.Left) ? -0.5f : 0.5f;
			float num2 = this._penitent.transform.position.x + num;
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(this._penitent.transform, num2, 0.1f, false), 3);
		}

		private void IsClimbingLadder()
		{
			this._penitent.IsClimbingLadder = this._penitent.PlatformCharacterController.IsClimbing;
		}

		private void Crouch()
		{
			this._penitent.IsCrouched = (this._playerInput.isJoystickDown && !this._playerInput.IsAttackButtonHold && !this.IsDemakeMode && !this._penitent.IsChargingAttack && !this._penitent.IsDashing && !this._penitent.PenitentAttack.IsRunningCombo);
			this.SpriteAnimator.SetBool(AnimatorInyector.IsCrouch, this._penitent.IsCrouched && !this._playerInput.IsJumpOff);
			if (!this._playerInput.IsJumpOff || this.FireJumpOffTrigger)
			{
				return;
			}
			this.SpriteAnimator.SetTrigger(AnimatorInyector.JumpOff);
			this.FireJumpOffTrigger = true;
		}

		public void PlayerGetDamage(DamageArea.DamageType damageType)
		{
			switch (damageType)
			{
			case DamageArea.DamageType.Normal:
				if (this._penitent.Status.IsGrounded || this._penitent.IsClimbingLadder)
				{
					this.SpriteAnimator.SetTrigger(AnimatorInyector.Hurt);
				}
				break;
			case DamageArea.DamageType.Heavy:
				this.SpriteAnimator.SetBool(AnimatorInyector.Throw, true);
				break;
			}
		}

		private void IsDead()
		{
			if (this._penitent.Status.Dead)
			{
				if (!this._setDeadAnimation && !this._penitent.IsDeadInAir)
				{
					this._setDeadAnimation = true;
					this.SpriteAnimator.SetTrigger(AnimatorInyector.Death);
				}
			}
			else if (this._setDeadAnimation)
			{
				this._setDeadAnimation = false;
				this.EnableAnimator(true);
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.Death);
			}
			if (this._penitent.IsImpaled && !this._setImpaledAnimation)
			{
				this._setImpaledAnimation = true;
				this.SpriteAnimator.SetTrigger(AnimatorInyector.DeathSpike);
			}
			else if (!this._penitent.IsImpaled && this._setImpaledAnimation)
			{
				this._setImpaledAnimation = !this._setImpaledAnimation;
				this.SpriteAnimator.ResetTrigger(AnimatorInyector.DeathSpike);
			}
		}

		public Core.SimpleEvent OnAttack;

		private readonly int _climbCliffAnim = Animator.StringToHash("Player_Climb_Edge");

		private PlatformCharacterController _platformCharacterController;

		private PlatformCharacterInput _playerInput;

		public Animator SpriteAnimator;

		private Penitent _penitent;

		private Dash _playerDash;

		private bool _isGrounded;

		private bool _isJumpReady;

		private bool _startRun;

		private bool _readyToRun;

		private const float AttackTime = 0.05f;

		private float _deltaAttackTime;

		private bool _setDeadAnimation;

		private bool _isChargeAttackTriggered;

		private bool _setImpaledAnimation;

		[Tooltip("Max VSPeed without stunting fall")]
		[Range(0f, 20f)]
		public float MaxVSpeedFallStunt;

		private static readonly int IsAttackCharged = Animator.StringToHash("IS_ATTACK_CHARGED");

		private static readonly int IsAttackHold = Animator.StringToHash("IS_ATTACK_HOLD");

		private static readonly int Attack = Animator.StringToHash("ATTACK");

		private static readonly int AirAttackParam = Animator.StringToHash("AIR_ATTACK");

		private static readonly int ChargeAttack = Animator.StringToHash("CHARGE_ATTACK");

		private static readonly int RunStep = Animator.StringToHash("RUN_STEP");

		private static readonly int RunningParam = Animator.StringToHash("RUNNING");

		private static readonly int Grounded = Animator.StringToHash("GROUNDED");

		private static readonly int CanJumpOff = Animator.StringToHash("CAN_JUMP_OFF");

		private static readonly int IsJumpingOff = Animator.StringToHash("IS_JUMPING_OFF");

		private static readonly int CanAirAttack = Animator.StringToHash("CAN_AIR_ATTACK");

		private static readonly int Ladder = Animator.StringToHash("IS_CLIMBING_LADDER");

		private static readonly int IsGrabbingLadder = Animator.StringToHash("IS_GRABBING_LADDER");

		private static readonly int IsHurt = Animator.StringToHash("IS_HURT");

		private static readonly int Dead = Animator.StringToHash("IS_DEAD");

		private static readonly int ClimbCliffLedge = Animator.StringToHash("CLIMB_CLIFF_LEDGE");

		private static readonly int IsGrabbingCliffLede = Animator.StringToHash("IS_GRABBING_CLIFF_LEDE");

		private static readonly int AirAttacking = Animator.StringToHash("AIR_ATTACKING");

		private static readonly int Unhang = Animator.StringToHash("UNHANG");

		private static readonly int AxisThreshold = Animator.StringToHash("AXIS_THRESHOLD");

		private static readonly int JoystickUp = Animator.StringToHash("JOYSTICK_UP");

		private static readonly int JoystickDown = Animator.StringToHash("JOYSTICK_DOWN");

		private static readonly int Jump = Animator.StringToHash("JUMP");

		private static readonly int JumpOff = Animator.StringToHash("JUMP_OFF");

		private static readonly int IsCrouch = Animator.StringToHash("IS_CROUCH");

		private static readonly int Falling = Animator.StringToHash("FALLING");

		private static readonly int Dash = Animator.StringToHash("DASH");

		private static readonly int DashingParam = Animator.StringToHash("DASHING");

		private static readonly int Sliding = Animator.StringToHash("LADDER_SLIDING");

		private static readonly int Hurt = Animator.StringToHash("HURT");

		private static readonly int Throw = Animator.StringToHash("THROW");

		private static readonly int Death = Animator.StringToHash("DEATH");

		private static readonly int DeathSpike = Animator.StringToHash("DEATH_SPIKE");

		private static readonly int Demake = Animator.StringToHash("DEMAKE");
	}
}
