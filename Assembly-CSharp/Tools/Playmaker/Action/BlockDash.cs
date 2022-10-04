using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using HutongGames.PlayMaker;
using Tools.Playmaker2.Action;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	public class BlockDash : FsmStateAction
	{
		public override void OnEnter()
		{
			this.penitent = Core.Logic.Penitent;
		}

		public override void OnExit()
		{
			this.penitent.Dash.StopCast();
			base.Finish();
		}

		private Penitent penitent;

		private DialogStart dialogStart;

		private bool startdialog;
	}
}
