using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Teleports to the Hub of the Boss Rush (or end the course if last scene).")]
	public class TeleportToBossRushHub : FsmStateAction
	{
		public override void OnEnter()
		{
			if (Core.BossRushManager.IsLastScene())
			{
				Core.BossRushManager.EndCourse(true);
			}
			else
			{
				Core.BossRushManager.LoadHub(true);
			}
		}
	}
}
