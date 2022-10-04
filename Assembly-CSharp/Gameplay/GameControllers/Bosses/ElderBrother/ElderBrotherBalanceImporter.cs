using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.ElderBrother
{
	public class ElderBrotherBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetJumpAttackDamage();
			this.SetCorpseAreaAttackDamage();
			this.SetMaceAttackDamage();
			this.SetContactDamage();
		}

		private void SetJumpAttackDamage()
		{
			BossJumpAttack componentInChildren = this.bossEnemy.GetComponentInChildren<BossJumpAttack>();
			if (componentInChildren)
			{
				componentInChildren.SetDamage(base.GetLightAttackDamage);
			}
		}

		private void SetCorpseAreaAttackDamage()
		{
			if (this.CorpsesSummonAttack == null)
			{
				return;
			}
			this.CorpsesSummonAttack.SpawnedAreaAttackDamage = base.GetMediumAttackDamage;
		}

		private void SetMaceAttackDamage()
		{
			if (this.MaceSummonAttack == null)
			{
				return;
			}
			this.MaceSummonAttack.SpawnedAreaAttackDamage = base.GetMediumAttackDamage;
		}

		private void SetContactDamage()
		{
			foreach (EnemyAttack enemyAttack in this.bossEnemy.GetComponentsInChildren<EnemyAttack>())
			{
				enemyAttack.ContactDamageAmount = (float)base.GetContactDamage;
			}
		}

		[SerializeField]
		protected BossAreaSummonAttack CorpsesSummonAttack;

		[SerializeField]
		protected BossAreaSummonAttack MaceSummonAttack;
	}
}
