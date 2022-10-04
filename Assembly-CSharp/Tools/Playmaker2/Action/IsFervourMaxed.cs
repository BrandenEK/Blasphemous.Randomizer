using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Get")]
	public class IsFervourMaxed : FsmStateAction
	{
		public override void Reset()
		{
			this.output = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			this.output.Value = (Core.Logic.Penitent.Stats.Fervour.Current == Core.Logic.Penitent.Stats.Fervour.CurrentMax);
			Debug.Log(string.Format("Fervour:  {0} Max: {1}", Core.Logic.Penitent.Stats.Fervour.Current, Core.Logic.Penitent.Stats.Fervour.CurrentMax));
			base.Finish();
		}

		public FsmBool output;
	}
}
