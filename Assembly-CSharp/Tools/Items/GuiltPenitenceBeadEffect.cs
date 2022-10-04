using System;
using Framework.Inventory;
using Framework.Managers;
using Framework.Penitences;

namespace Tools.Items
{
	public class GuiltPenitenceBeadEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Core.PenitenceManager.UseFervourFlasks = true;
			return true;
		}

		protected override void OnRemoveEffect()
		{
			Core.PenitenceManager.UseFervourFlasks = (Core.PenitenceManager.GetCurrentPenitence() is PenitencePE03);
		}
	}
}
