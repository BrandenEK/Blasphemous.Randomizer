using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Rewired;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameControllers.Penitent.InputSystem
{
	[RequireComponent(typeof(PlatformCharacterController))]
	public class PlatformCharacterInput : MonoBehaviour
	{
		public bool BlockJump { get; private set; }

		public bool IsJumpOff { get; private set; }

		public bool Blocked
		{
			get
			{
				return Core.Input.InputBlocked;
			}
		}

		public Player Rewired { get; private set; }

		public float FVerAxis { get; private set; }

		public float ClampledVerticalAxis
		{
			get
			{
				return this.verticalAxis;
			}
		}

		public float FHorAxis { get; private set; }

		public bool Jump { get; private set; }

		public float RVerticalAxis { get; private set; }

		public bool IsAttackButtonHold { get; private set; }

		public bool IsDashButtonHold { get; private set; }

		public bool IsJumpButtonHold { get; private set; }

		private void Awake()
		{
			this._penitent = (base.GetComponent<Entity>() as Penitent);
		}

		private void Start()
		{
			this.m_platformCtrl = base.GetComponent<PlatformCharacterController>();
			this.IsJumpButtonHold = false;
		}

		private void Update()
		{
			if (ReInput.players.playerCount <= 0)
			{
				return;
			}
			if (this.Rewired == null)
			{
				Player player = ReInput.players.GetPlayer(0);
				if (player == null)
				{
					return;
				}
				this.Rewired = player;
			}
			bool flag = !this.Blocked;
			if (!flag)
			{
				this.jumpBlocked = true;
			}
			if (flag && this.Rewired.GetButton(6))
			{
				this.jumpBlocked = false;
			}
			if (!this.simulatingMove)
			{
				if (flag)
				{
					this.horizontalAxis = this.Rewired.GetAxisRaw(0);
					this.verticalAxis = this.Rewired.GetAxisRaw(4);
					this.RVerticalAxis = this.Rewired.GetAxisRaw(20);
					if (Math.Abs(this.forceHorizontalMovement) > Mathf.Epsilon)
					{
						this.horizontalAxis = this.forceHorizontalMovement;
					}
					this.bKey = this.Rewired.GetButtonDown(5);
					this.xKey = this.Rewired.GetButtonDown(7);
					this.IsDashButtonHold = this.Rewired.GetButton(7);
					if (!this.jumpBlocked)
					{
						this.aKey = this.Rewired.GetButton(6);
					}
				}
				else
				{
					this.ResetInputs();
				}
			}
			this.verticalAxis = PlatformCharacterInput.ResolveVerticalInputs(this.verticalAxis);
			if (this.InputMode == PlatformCharacterInput.EInputMode.Gamepad)
			{
				this.Dash = this.xKey;
				this.Attack = this.bKey;
				float num = this.horizontalAxis;
				this.FHorAxis = num;
				this.isJoystickDown = this.IsJoystickDown();
				this.isJoystickUp = this.IsJoystickUp();
				this.Jump = this.aKey;
				this.AbsHorAxis = Mathf.Abs(num);
				num = this.SetHorInputFactor(num);
				num *= Mathf.Abs(num);
				if (this._penitent.Status.Paralyzed)
				{
					num *= 0f;
				}
				float num2 = this.verticalAxis;
				this.FVerAxis = num2;
				num2 *= Mathf.Abs(num2);
				float num3 = Mathf.Abs(num);
				num3 = ((num3 <= this.AxisMovingThreshold) ? num3 : 1f);
				num3 = ((!this._penitent.AnimatorInyector.IsDashAnimRunning) ? num3 : 0f);
				this.AbsHAxis = num3;
				float num4 = Mathf.Abs(num2);
				this.AttackButtonHold();
				this.ResetAttackButtonHold();
				if (this.Jump)
				{
					this._penitent.IsStickedOnWall = false;
				}
				if (this.isAirAttacking)
				{
					return;
				}
				if (this.IsHorizontalClamped())
				{
					this.ReachAxisThreshold = false;
					if (this.m_platformCtrl.PlatformCharacterPhysics.VSpeed >= 1f)
					{
						this.m_platformCtrl.PlatformCharacterPhysics.VSpeed = 0f;
					}
					this.m_platformCtrl.PlatformCharacterPhysics.HSpeed = 0f;
					this.m_platformCtrl.SetActionState(16, false);
					this.m_platformCtrl.SetActionState(2, false);
					this.m_platformCtrl.SetActionState(1, false);
					return;
				}
				this.ReachAxisThreshold = (num3 > this.AxisMovingThreshold);
				if (this._penitent.IsDashing)
				{
					num = 0f;
				}
				if (this._penitent.Status.IsHurt || this._penitent.Status.IsIdle)
				{
					this.IsJumpOff = false;
				}
				if (this._penitent.IsClimbingCliffLede)
				{
					this.unHang = this._penitent.CanLowerCliff;
					return;
				}
				this.SetOrientation(this.horizontalAxis);
				if (num3 >= this.AxisMovingThreshold)
				{
					this.m_platformCtrl.HorizontalSpeedScale = ((!this.UseAxisAsSpeedFactor) ? 1f : num3);
				}
				if (num4 >= this.AxisMovingThreshold)
				{
					this.m_platformCtrl.VerticalSpeedScale = ((!this.UseAxisAsSpeedFactor) ? 1f : num4);
				}
				if (this._penitent.IsGrabbingLadder || this._penitent.IsCrouched || this._penitent.BeginCrouch || this._penitent.IsCrouchAttacking || this.IsFrontBlocked)
				{
					this.m_platformCtrl.SetActionState(2, false);
					this.m_platformCtrl.SetActionState(1, false);
				}
				else
				{
					this.m_platformCtrl.SetActionState(2, num <= -this.AxisMovingThreshold && flag);
					this.m_platformCtrl.SetActionState(1, num >= this.AxisMovingThreshold && flag);
				}
				this.m_platformCtrl.SetActionState(8, num2 <= -this.AxisMovingThreshold && flag);
				this.m_platformCtrl.SetActionState(4, num2 >= this.AxisMovingThreshold && flag);
				if (this.Jump && !this.IsJumpOff && this.m_platformCtrl.IsGrounded && this._penitent.isJumpOffReady && !this.pressedJumpButton && !this._penitent.FloorChecker.IsOnFloorPlatform && !this._penitent.StepOnLadder && this.isJoystickDown && !Core.LevelManager.currentLevel.LevelName.Equals("D24Z01S01"))
				{
					this.pressedJumpButton = true;
					this.IsJumpOff = true;
					if (this.onJumpOff != null)
					{
						this.onJumpOff.Invoke(base.transform.position);
					}
					base.StartCoroutine(this.JumpOff());
				}
				else if (!this.Jump)
				{
					this.pressedJumpButton = false;
				}
				else
				{
					this.IsJumpOff = false;
				}
				bool isGrounded = this.m_platformCtrl.IsGrounded;
				if ((!this.IsAttacking || !isGrounded) && !this._penitent.IsDashing && !this._penitent.BeginCrouch && !this._penitent.GrabLadder.IsBottomLadderRepositioning && !this._penitent.GrabLadder.StartGoingDown)
				{
					if (!this._penitent.IsClimbingLadder)
					{
						bool flag2 = this.aKey && !this.BlockJump && !this.Blocked && (!this.Rewired.GetButton(5) || !isGrounded) && !this.IsJoystickDown() && !this.Rewired.GetButtonDown(23) && !this._penitent.IsFallingStunt;
						this.m_platformCtrl.SetActionState(16, flag2);
					}
					else if (this._penitent.CanJumpFromLadder && this.Rewired.GetAxis(4) > -1f)
					{
						if (this.aKey)
						{
							this.m_platformCtrl.ResetLadderJumpTimeThreshold();
						}
						this.m_platformCtrl.SetActionState(16, this.aKey && !this.BlockJump && !this.Rewired.GetButton(5));
					}
				}
				this.JumpButtonHold();
				this.ResetJumpButtonHold();
			}
		}

		private float SetHorInputFactor(float horInput)
		{
			float result = 0f;
			if (horInput > this.AxisMovingThreshold)
			{
				result = 1f;
			}
			else if (horInput < -this.AxisMovingThreshold)
			{
				result = -1f;
			}
			return result;
		}

		private void SetOrientation(float horRawInput)
		{
			if (horRawInput > this.AxisMovingThreshold)
			{
				this.faceRight = true;
			}
			else if (horRawInput < -this.AxisMovingThreshold)
			{
				this.faceRight = false;
			}
		}

		private static float ResolveVerticalInputs(float currentVerticalInput)
		{
			float result = 0f;
			if (currentVerticalInput > 0.65f)
			{
				result = 1f;
			}
			else if (currentVerticalInput < -0.65f)
			{
				result = -1f;
			}
			return result;
		}

		private IEnumerator JumpOff()
		{
			this.m_platformCtrl.SetActionState(32, false);
			this.deltaTimeToJumpOff = 0f;
			while (this.deltaTimeToJumpOff <= this.timeToJumpOff)
			{
				this.deltaTimeToJumpOff += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			this.m_platformCtrl.SetActionState(32, true);
			this.IsJumpOff = false;
			yield break;
		}

		public void CancelPlatformDropDown()
		{
			this.m_platformCtrl.SetActionState(32, false);
		}

		public void CancelJump()
		{
			this.m_platformCtrl.SetActionState(16, false);
			this.m_platformCtrl.PlatformCharacterPhysics.VSpeed = 0f;
		}

		private bool IsJoystickDown()
		{
			return !this.Blocked && this.Rewired.GetAxis(4) <= -0.75f && this.Rewired.GetAxis(4) >= -1f;
		}

		private bool IsJoystickUp()
		{
			return !this.Blocked && this.Rewired.GetAxis(4) >= 0.75f && this.Rewired.GetAxis(4) <= 1f;
		}

		private void AttackButtonHold()
		{
			if (Core.Input.HasBlocker("DIALOG") || Core.Input.HasBlocker("PLAYER_LOGIC"))
			{
				return;
			}
			if (this._penitent.Status.Dead || !this._penitent.Status.IsGrounded)
			{
				this.deltaTimeButtonHeld = 0f;
				this.IsAttackButtonHold = false;
			}
			if (this.Rewired.GetButton(5))
			{
				this.deltaTimeButtonHeld += Time.deltaTime;
				if (this.deltaTimeButtonHeld < this.timeInputAttackHold || this.IsAttackButtonHold)
				{
					return;
				}
				this.deltaTimeButtonHeld = 0f;
				this.IsAttackButtonHold = true;
			}
		}

		public void ResetInputs()
		{
			this.horizontalAxis = 0f;
			this.verticalAxis = 0f;
			this.bKey = false;
			this.xKey = false;
			this.aKey = false;
		}

		public void ResetActions()
		{
			if (!this.m_platformCtrl)
			{
				return;
			}
			this.m_platformCtrl.SetActionState(16, false);
			this.m_platformCtrl.SetActionState(4, false);
			this.m_platformCtrl.SetActionState(8, false);
			this.m_platformCtrl.SetActionState(2, false);
			this.m_platformCtrl.SetActionState(1, false);
		}

		public void ResetHorizontalBlockers()
		{
			if (!this._penitent)
			{
				return;
			}
			this._penitent.IsGrabbingLadder = false;
			this._penitent.IsCrouched = false;
			this._penitent.BeginCrouch = false;
			this._penitent.IsCrouchAttacking = false;
		}

		private void ResetAttackButtonHold()
		{
			if (!this.Rewired.GetButtonUp(5))
			{
				return;
			}
			this.deltaTimeButtonHeld = 0f;
			this.IsAttackButtonHold = false;
		}

		private void JumpButtonHold()
		{
			if (!this.Rewired.GetButton(6))
			{
				return;
			}
			this.deltaTimeJumpButtonHeld += Time.deltaTime;
			if (this.deltaTimeJumpButtonHeld >= this.timeInputJumpHold && !this.IsJumpButtonHold)
			{
				this.deltaTimeJumpButtonHeld = 0f;
				if (!this.IsJumpButtonHold)
				{
					this.IsJumpButtonHold = true;
				}
				if (!this.BlockJump)
				{
					this.BlockJump = true;
				}
			}
		}

		private void ResetJumpButtonHold()
		{
			if (this.Rewired.GetButton(6))
			{
				return;
			}
			this.deltaTimeJumpButtonHeld = 0f;
			if (this.IsJumpButtonHold)
			{
				this.IsJumpButtonHold = !this.IsJumpButtonHold;
			}
			if (this.BlockJump)
			{
				this.BlockJump = !this.BlockJump;
			}
		}

		private bool IsHorizontalClamped()
		{
			return this._penitent.Status.IsHurt || this._penitent.IsJumpingOff || this._penitent.Status.Dead || this._penitent.IsChargingAttack || this.IsAttacking;
		}

		private bool IsFrontBlocked
		{
			get
			{
				return this._penitent.HasFlag("FRONT_BLOCKED");
			}
		}

		public void Move(float x, float time)
		{
			base.StartCoroutine(this.MoveAction(x, time));
		}

		public IEnumerator MoveAction(float x, float time)
		{
			this.simulatingMove = true;
			this.horizontalAxis = x;
			yield return new WaitForSeconds(time);
			this.simulatingMove = false;
			yield break;
		}

		public PlatformCharacterInput.JumpEvent onJumpOff;

		public const string PlayerInputBlocker = "PLAYER_LOGIC";

		public const float VerticalJoystickOffset = 0.75f;

		private Penitent _penitent;

		public float AbsHAxis;

		public float AbsHorAxis;

		private bool aKey;

		public bool Attack;

		public float AxisMovingThreshold = 0.2f;

		private bool bKey;

		public bool canAirAttack;

		public bool Dash;

		private float deltaTimeButtonHeld;

		private float deltaTimeInputThreshold;

		private float deltaTimeJumpButtonHeld;

		private float deltaTimeOrientationThreshold;

		private float deltaTimeToJumpOff;

		public bool faceRight;

		public PlatformCharacterInput.EInputMode InputMode = PlatformCharacterInput.EInputMode.Gamepad;

		public bool isAirAttacking;

		public bool IsAttacking;

		public bool isJoystickDown;

		public bool isJoystickUp;

		public float forceHorizontalMovement;

		private bool jumpBlocked;

		private PlatformCharacterController m_platformCtrl;

		private bool pressedJumpButton;

		public bool ReachAxisThreshold;

		public bool simulatingMove;

		[Tooltip("Time pressing button to fire hold attack")]
		public float timeInputAttackHold = 0.5f;

		public float timeInputJumpHold = 0.5f;

		public float timeInputThreshold = 0.15f;

		private readonly float timeOrientationThreshold = 0.1f;

		[Range(0f, 1f)]
		public float timeToJumpOff = 1f;

		public bool unHang;

		public bool UseAxisAsSpeedFactor = true;

		private float verticalAxis;

		private float horizontalAxis;

		private bool xKey;

		[Serializable]
		public class JumpEvent : UnityEvent<Vector3>
		{
		}

		public enum EInputMode
		{
			Keyboard,
			Gamepad
		}
	}
}
