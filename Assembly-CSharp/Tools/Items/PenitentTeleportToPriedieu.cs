using System;
using Framework.Inventory;
using Framework.Managers;

namespace Tools.Items
{
	public class PenitentTeleportToPriedieu : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			if (Core.Input.HasBlocker("INTERACTABLE"))
			{
				return false;
			}
			Core.Logic.Penitent.Animator.Play(this.TPOAnimatorState, 0, 0f);
			return true;
		}

		public string TPOAnimatorState = "RegresoAPuerto";
	}
}
