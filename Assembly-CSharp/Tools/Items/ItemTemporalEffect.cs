using System;
using System.Collections.Generic;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Tools.Items
{
	public class ItemTemporalEffect : ObjectEffect
	{
		public List<ItemTemporalEffect.PenitentEffects> Effects
		{
			get
			{
				return this.effects;
			}
		}

		public bool ContainsEffect(ItemTemporalEffect.PenitentEffects effect)
		{
			return this.effects.Contains(effect);
		}

		protected override bool OnApplyEffect()
		{
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.StopFervourRecolection))
			{
				Core.Logic.Penitent.obtainsFervour = false;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.Invulnerable))
			{
				Core.Logic.Penitent.Status.Invulnerable = true;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.Level2Attack))
			{
				this.previousAttackLevel = Core.Logic.Penitent.PenitentAttack.CurrentLevel;
				Core.Logic.Penitent.PenitentAttack.CurrentLevel = 2;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.RedAttack))
			{
				this.previousColor = Core.Logic.Penitent.PenitentAttack.AttackColor;
				Core.Logic.Penitent.PenitentAttack.AttackColor = PenitentSword.AttackColor.Red;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.StopGuiltDrop))
			{
				Core.Logic.Penitent.GuiltDrop = false;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.DisableUnEquipSword))
			{
				Core.Logic.Penitent.AllowEquipSwords = false;
			}
			this.ShowDebug("ON");
			return true;
		}

		protected override void OnRemoveEffect()
		{
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.StopFervourRecolection))
			{
				Core.Logic.Penitent.obtainsFervour = true;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.Invulnerable))
			{
				Core.Logic.Penitent.Status.Invulnerable = false;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.Level2Attack))
			{
				Core.Logic.Penitent.PenitentAttack.CurrentLevel = this.previousAttackLevel;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.RedAttack))
			{
				Core.Logic.Penitent.PenitentAttack.AttackColor = this.previousColor;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.StopGuiltDrop))
			{
				Core.Logic.Penitent.GuiltDrop = true;
			}
			if (this.effects.Contains(ItemTemporalEffect.PenitentEffects.DisableUnEquipSword))
			{
				Core.Logic.Penitent.AllowEquipSwords = true;
			}
			this.ShowDebug("OFF");
		}

		private void ShowDebug(string status)
		{
			string text = string.Empty;
			foreach (ItemTemporalEffect.PenitentEffects penitentEffects in this.effects)
			{
				if (text != string.Empty)
				{
					text += ", ";
				}
				text += penitentEffects.ToString();
			}
			if (base.name != string.Empty)
			{
				Debug.Log("Effects " + status + "  " + text);
			}
		}

		[SerializeField]
		private List<ItemTemporalEffect.PenitentEffects> effects = new List<ItemTemporalEffect.PenitentEffects>();

		private PenitentSword.AttackColor previousColor;

		private int previousAttackLevel;

		public enum PenitentEffects
		{
			StopFervourRecolection,
			Invulnerable,
			RedAttack,
			Level2Attack,
			StopGuiltDrop,
			DisableUnEquipSword
		}
	}
}
