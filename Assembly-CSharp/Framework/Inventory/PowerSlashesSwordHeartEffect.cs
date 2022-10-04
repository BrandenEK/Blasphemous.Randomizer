using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Framework.Inventory
{
	public class PowerSlashesSwordHeartEffect : ObjectEffect
	{
		protected override void OnAwake()
		{
			base.OnAwake();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		protected override bool OnApplyEffect()
		{
			this.SetPowerSlashes();
			return true;
		}

		protected override void OnRemoveEffect()
		{
			this.UnsetPowerSlashes();
			base.OnRemoveEffect();
		}

		private void SetPowerSlashes()
		{
			this.prevAttackLevel = Core.Logic.Penitent.PenitentAttack.CurrentLevel;
			Core.Logic.Penitent.PenitentAttack.CurrentLevel = 2;
			this.SetSlashColor(this.slashColor);
		}

		private void UnsetPowerSlashes()
		{
			Core.Logic.Penitent.PenitentAttack.CurrentLevel = this.prevAttackLevel;
			PenitentSword penitentSword = Core.Logic.Penitent.PenitentAttack.CurrentPenitentWeapon as PenitentSword;
			if (penitentSword)
			{
				penitentSword.SlashAnimator.ResetParameters();
			}
		}

		private void SetSlashColor(PowerSlashesSwordHeartEffect.SlashColors slashColor)
		{
			PenitentSword.AttackColor attackColor = PenitentSword.AttackColor.Default;
			if (slashColor != PowerSlashesSwordHeartEffect.SlashColors.Blue)
			{
				if (slashColor == PowerSlashesSwordHeartEffect.SlashColors.Red)
				{
					attackColor = PenitentSword.AttackColor.Red;
				}
			}
			else
			{
				attackColor = PenitentSword.AttackColor.Default;
			}
			Core.Logic.Penitent.PenitentAttack.AttackColor = attackColor;
		}

		[SerializeField]
		private PowerSlashesSwordHeartEffect.SlashColors slashColor;

		private int prevAttackLevel;

		private enum SlashColors
		{
			Blue,
			Red
		}
	}
}
