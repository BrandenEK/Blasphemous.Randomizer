using System;
using Framework.Inventory;
using Gameplay.GameControllers.Environment.AreaEffects;
using UnityEngine;

namespace Tools.Items
{
	public class SilverLungRelicEffect : RelicEffect
	{
		public override void OnEquipEffect()
		{
			base.OnEquipEffect();
			this.EnablePoisonAreas(false);
		}

		public override void OnUnEquipEffect()
		{
			base.OnUnEquipEffect();
			this.EnablePoisonAreas(true);
		}

		private void EnablePoisonAreas(bool enablePoisonAreas = true)
		{
			PoisonAreaEffect[] array = Object.FindObjectsOfType<PoisonAreaEffect>();
			foreach (PoisonAreaEffect poisonAreaEffect in array)
			{
				poisonAreaEffect.EnableEffect(enablePoisonAreas);
			}
		}
	}
}
