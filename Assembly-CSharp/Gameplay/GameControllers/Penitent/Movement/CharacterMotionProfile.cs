using System;
using System.Diagnostics;
using CreativeSpore.SmartColliders;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Movement
{
	[CreateAssetMenu(fileName = "Character Motion Profile", menuName = "Blasphemous/Character/Motion Profile")]
	public class CharacterMotionProfile : ScriptableObject
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action OnMotionProfileLoaded;

		public void Init(PlatformCharacterController controller)
		{
			controller.PlatformCharacterPhysics.GravityScale = this.gravityScale;
			controller.PlatformCharacterPhysics.TerminalVelocity = this.terminalVelocity;
			controller.WalkingAcc = this.walkingAcc;
			controller.WalkingDrag = this.walkingDrag;
			this.maxWalkingSpeed = controller.PlatformCharacterPhysics.SolveMaxSpeedWithAccAndDrag(controller.WalkingAcc, controller.WalkingDrag);
			controller.MaxWalkingSpeed = this.maxWalkingSpeed;
			controller.AirborneAcc = this.airborneAcc;
			controller.JumpingSpeed = this.jumpingSpeed;
			controller.JumpingAcc = this.jumpingAcc;
			controller.JumpingAccTime = this.jumpingAccTime;
			if (CharacterMotionProfile.OnMotionProfileLoaded != null)
			{
				CharacterMotionProfile.OnMotionProfileLoaded();
			}
		}

		[Header("Physics Parameters")]
		public float gravityScale;

		public float terminalVelocity;

		[Space(10f)]
		[Header("Moving Parameters")]
		public float walkingAcc;

		public float walkingDrag;

		[Range(0f, 10f)]
		public float maxWalkingSpeed;

		[Space(10f)]
		[Header("Jumping Parameters")]
		public float airborneAcc;

		public float jumpingSpeed;

		public float jumpingAcc;

		public float jumpingAccTime;
	}
}
