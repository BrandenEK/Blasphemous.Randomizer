using System;
using System.Collections.Generic;
using System.Linq;
using Framework.EditorScripts.BossesBalance;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.TresAngustias
{
	public class TresAngustiasBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.maceAttackDamage = (float)int.Parse(this.bossLoadedStats["Medium Attack"].ToString());
			this.spearAttackDamage = int.Parse(this.bossLoadedStats["Light Attack"].ToString());
			this.SetBeamAttack();
			this.SetLanceAttack();
			this.SetMaceAttack();
			this.SetShieldAttack();
			this.SetScrollableSpeed();
		}

		private void SetLanceAttack()
		{
			this.BossBehaviour.singleAnguishLance.Behaviour.spearAttack.SetDamage(this.spearAttackDamage);
			this.BossBehaviour.singleAnguishLance.Behaviour.maceAttack.PathFollowingProjectileDamage = this.maceAttackDamage;
		}

		private void SetMaceAttack()
		{
			this.BossBehaviour.singleAnguishMace.Behaviour.spearAttack.SetDamage(this.spearAttackDamage);
			this.BossBehaviour.singleAnguishMace.Behaviour.maceAttack.PathFollowingProjectileDamage = this.maceAttackDamage;
		}

		private void SetShieldAttack()
		{
			this.BossBehaviour.singleAnguishShield.Behaviour.spearAttack.SetDamage(this.spearAttackDamage);
			this.BossBehaviour.singleAnguishShield.Behaviour.maceAttack.PathFollowingProjectileDamage = this.maceAttackDamage;
		}

		private void SetBeamAttack()
		{
			int spawnedAreaAttackDamage = int.Parse(this.bossLoadedStats["Heavy Attack"].ToString());
			BossAreaSummonAttack componentInChildren = this.bossEnemy.GetComponentInChildren<BossAreaSummonAttack>();
			if (componentInChildren)
			{
				componentInChildren.SpawnedAreaAttackDamage = spawnedAreaAttackDamage;
			}
		}

		private void SetScrollableSpeed()
		{
			ScrollableModulesManager scrollableModulesManager = Object.FindObjectOfType<ScrollableModulesManager>();
			if (!scrollableModulesManager)
			{
				return;
			}
			foreach (TresAngustiasBalanceImporter.LevelScrollSpeed levelScrollSpeed in from scrollSpeed in this.ScrollSpeeds
			where Core.GameModeManager.IsCurrentMode(scrollSpeed.GameMode)
			select scrollSpeed)
			{
				scrollableModulesManager.speed = levelScrollSpeed.Speed;
			}
		}

		private TresAngustiasMasterBehaviour BossBehaviour
		{
			get
			{
				return this.bossEnemy.EnemyBehaviour as TresAngustiasMasterBehaviour;
			}
		}

		private float maceAttackDamage;

		private int spearAttackDamage;

		[SerializeField]
		private List<TresAngustiasBalanceImporter.LevelScrollSpeed> ScrollSpeeds;

		[Serializable]
		public struct LevelScrollSpeed
		{
			public GameModeManager.GAME_MODES GameMode;

			public float Speed;
		}
	}
}
