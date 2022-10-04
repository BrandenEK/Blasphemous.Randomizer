using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Removes a flask from the player.")]
	public class FlaskRemove : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.Stats.Flask.SetPermanentBonus(Core.Logic.Penitent.Stats.Flask.PermanetBonus - 1f);
			base.Finish();
		}
	}
}
