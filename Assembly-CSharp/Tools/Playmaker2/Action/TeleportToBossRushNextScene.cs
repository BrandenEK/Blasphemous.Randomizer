using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Teleports to the Next Scene of the current Boss Rush course.")]
	public class TeleportToBossRushNextScene : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.BossRushManager.LoadCourseNextScene();
		}
	}
}
