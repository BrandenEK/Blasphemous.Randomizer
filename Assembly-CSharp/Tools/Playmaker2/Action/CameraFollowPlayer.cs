using System;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Camera follows player on certain axis.")]
	public class CameraFollowPlayer : FsmStateAction
	{
		public override void Reset()
		{
			ProCamera2D proCamera2D = Core.Logic.CameraManager.ProCamera2D;
			proCamera2D.FollowHorizontal = true;
			proCamera2D.FollowVertical = true;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			ProCamera2D proCamera2D = Core.Logic.CameraManager.ProCamera2D;
			proCamera2D.FollowHorizontal = this.FollowXAxis.Value;
			proCamera2D.FollowVertical = this.FollowYAxis.Value;
		}

		public FsmBool FollowXAxis;

		public FsmBool FollowYAxis;
	}
}
