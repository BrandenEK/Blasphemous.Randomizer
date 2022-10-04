using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Sets the camera in free mode and sets a position.")]
	public class CameraModeNormal : FsmStateAction
	{
		public override void OnEnter()
		{
			base.OnEnter();
			Core.Cinematics.SetFreeCamera(false);
			base.Finish();
		}
	}
}
