using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Upgrades the Health healed by each of the player's Flasks.")]
	public class FlaskHealthUpgrade : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.Stats.FlaskHealth.Upgrade();
			base.Finish();
		}
	}
}
