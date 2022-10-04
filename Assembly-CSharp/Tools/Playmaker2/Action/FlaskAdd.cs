using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Add a flask.")]
	public class FlaskAdd : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.Stats.Flask.Upgrade();
			base.Finish();
		}
	}
}
