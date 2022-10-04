using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Add a custom amount of life.")]
	public class LifeAdd : FsmStateAction
	{
		public override void OnEnter()
		{
			if (Core.Logic.Penitent.Stats.Life.Current <= 0f)
			{
				return;
			}
			float num = this.value.Value;
			if (Core.PenitenceManager.UseStocksOfHealth)
			{
				num = 30f;
			}
			float num2 = Core.Logic.Penitent.Stats.Life.Current + num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			Core.Logic.Penitent.Stats.Life.Current = num2;
			base.Finish();
		}

		public FsmFloat value;
	}
}
