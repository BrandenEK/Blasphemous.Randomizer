using System;
using CreativeSpore.SmartColliders;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	[RequireComponent(typeof(PlatformCharacterController))]
	public class NPCInputs : MonoBehaviour
	{
		public float HorizontalInput
		{
			get
			{
				return this._horizontalInput;
			}
			set
			{
				this._horizontalInput = value;
			}
		}

		public float VerticalInput
		{
			set
			{
				this._verticalInput = value;
			}
		}

		public bool IsJumpOff { get; private set; }

		public float FVerAxis { get; private set; }

		public float FHorAxis { get; private set; }

		public bool Jump { get; set; }

		private void Start()
		{
			this.m_platformCtrl = base.GetComponent<PlatformCharacterController>();
		}

		private void Update()
		{
			if (this.InputMode == NPCInputs.eInputMode.Gamepad)
			{
				this.Dash = Input.GetButtonDown("Dash");
				this.Attack = Input.GetButtonDown("Attack");
				float num = this._horizontalInput;
				this.FHorAxis = num;
				this.isJoystickDown = this.IsJoystickDown();
				this.isJoystickUp = this.IsJoystickUp();
				this.AbsHorAxis = Mathf.Abs(num);
				num *= Mathf.Abs(num);
				float num2 = this._verticalInput;
				this.FVerAxis = num2;
				num2 *= Mathf.Abs(num2);
				float num3 = Mathf.Abs(num);
				num3 = ((num3 <= this.AxisMovingThreshold) ? num3 : 1f);
				this.AbsHAxis = num3;
				float num4 = Mathf.Abs(num2);
				this.ReachAxisThreshold = (num3 > this.AxisMovingThreshold);
				if (num3 >= this.AxisMovingThreshold)
				{
					this.m_platformCtrl.HorizontalSpeedScale = ((!this.UseAxisAsSpeedFactor) ? 1f : num3);
				}
				if (num4 >= this.AxisMovingThreshold)
				{
					this.m_platformCtrl.VerticalSpeedScale = ((!this.UseAxisAsSpeedFactor) ? 1f : num4);
				}
				this.m_platformCtrl.SetActionState(eControllerActions.Left, num <= -this.AxisMovingThreshold);
				this.m_platformCtrl.SetActionState(eControllerActions.Right, num >= this.AxisMovingThreshold);
				this.m_platformCtrl.SetActionState(eControllerActions.Down, num2 <= -this.AxisMovingThreshold);
				this.m_platformCtrl.SetActionState(eControllerActions.Up, num2 >= this.AxisMovingThreshold);
				this.m_platformCtrl.SetActionState(eControllerActions.Jump, this.Jump);
			}
		}

		protected float setHorInputFactor(float horInput)
		{
			float result;
			if (Mathf.Abs(horInput) > this.AxisMovingThreshold)
			{
				this.deltaTimeInputThreshold += Time.deltaTime;
				if (this.deltaTimeInputThreshold >= this.timeInputThreshold)
				{
					result = horInput;
				}
				else
				{
					result = 0f;
				}
			}
			else
			{
				this.deltaTimeInputThreshold = 0f;
				result = 0f;
			}
			return result;
		}

		protected void setOrientation(float horRawInput)
		{
			this.deltaTimeOrientationThreshold += Time.deltaTime;
			if (horRawInput > 0f && this.deltaTimeOrientationThreshold >= this.timeOrientationThreshold)
			{
				this.deltaTimeOrientationThreshold = 0f;
				this.faceRight = true;
			}
			else if (horRawInput < 0f && this.deltaTimeOrientationThreshold >= this.timeOrientationThreshold)
			{
				this.deltaTimeOrientationThreshold = 0f;
				this.faceRight = false;
			}
		}

		public void CancelPlatformDropDown()
		{
			this.m_platformCtrl.SetActionState(eControllerActions.PlatformDropDown, false);
		}

		public void CancelJump()
		{
			this.m_platformCtrl.SetActionState(eControllerActions.Jump, false);
			this.m_platformCtrl.PlatformCharacterPhysics.VSpeed = 0f;
		}

		protected bool IsJoystickDown()
		{
			return Mathf.Approximately(Input.GetAxisRaw("Vertical"), -1f) && Mathf.Abs(Input.GetAxis("Horizontal")) < this.AxisMovingThreshold;
		}

		protected bool IsJoystickUp()
		{
			return Mathf.Approximately(Input.GetAxisRaw("Vertical"), 1f) && Mathf.Abs(Input.GetAxis("Horizontal")) < this.AxisMovingThreshold;
		}

		private readonly float timeOrientationThreshold = 0.1f;

		private float _horizontalInput;

		private float _verticalInput;

		public float AbsHAxis;

		public float AbsHorAxis;

		public bool Attack;

		public float AxisMovingThreshold = 0.2f;

		public bool canAirAttack;

		public bool Dash;

		private float deltaTimeInputThreshold;

		private float deltaTimeOrientationThreshold;

		private float deltaTimeToJumpOff;

		public bool faceRight = true;

		public NPCInputs.eInputMode InputMode = NPCInputs.eInputMode.Gamepad;

		public bool isAirAttacking;

		public bool isAttacking;

		public bool isJoystickDown;

		public bool isJoystickUp;

		private PlatformCharacterController m_platformCtrl;

		public bool ReachAxisThreshold;

		public float timeInputThreshold = 0.15f;

		[Range(0f, 1f)]
		public float timeToJumpOff = 1f;

		public bool unHang;

		public bool UseAxisAsSpeedFactor = true;

		public enum eInputMode
		{
			Keyboard,
			Gamepad
		}
	}
}
