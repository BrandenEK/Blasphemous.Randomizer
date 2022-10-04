using System;
using Framework.Inventory;
using Gameplay.GameControllers.Environment.AreaEffects;
using UnityEngine;

namespace Tools.Items
{
	public class DirtyNailRelicEffect : RelicEffect
	{
		public override void OnEquipEffect()
		{
			base.OnEquipEffect();
			this.EnableMudAreas(false);
		}

		public override void OnUnEquipEffect()
		{
			base.OnUnEquipEffect();
			this.EnableMudAreas(true);
		}

		private void EnableMudAreas(bool enableMudAreas = true)
		{
			MudAreaEffect[] array = UnityEngine.Object.FindObjectsOfType<MudAreaEffect>();
			foreach (MudAreaEffect mudAreaEffect in array)
			{
				if (!mudAreaEffect.unafectedByRelic)
				{
					mudAreaEffect.EnableEffect(enableMudAreas);
				}
			}
		}
	}
}
