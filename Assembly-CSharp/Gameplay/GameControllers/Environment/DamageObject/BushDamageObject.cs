using System;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities;

namespace Gameplay.GameControllers.Environment.DamageObject
{
	public class BushDamageObject : DamageObject
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
		}

		public override void Damage(Hit hit)
		{
			base.Damage(hit);
			this.ColorFlash.TriggerColorFlash();
		}

		public void Destroy()
		{
		}

		protected ColorFlash ColorFlash;
	}
}
