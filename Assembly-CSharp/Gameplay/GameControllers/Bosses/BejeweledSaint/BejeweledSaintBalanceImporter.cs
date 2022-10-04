using System;
using System.Linq;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Bosses.BejeweledSaint.IA;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint
{
	public class BejeweledSaintBalanceImporter : BossBalanceImporter
	{
		protected override void ApplyLoadedStats()
		{
			this.SetHandLineAttackCooldown();
		}

		private void SetHandLineAttackCooldown()
		{
			BejeweledSaintBehaviour component = this.bossEnemy.GetComponent<BejeweledSaintBehaviour>();
			float cooldown = float.Parse(this.bossLoadedStats["M.A. Cooldown"].ToString());
			foreach (BejeweledSaintBehaviour.BejewelledAttackConfig bejewelledAttackConfig in from attack in component.attacksConfig
			where attack.atk.ToString() == "HANDS_LINE"
			select attack)
			{
				bejewelledAttackConfig.cooldown = cooldown;
			}
		}

		private const string HANDS_LINE = "HANDS_LINE";
	}
}
