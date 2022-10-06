using System;
using Framework.EditorScripts.BossesBalance;
using Gameplay.GameControllers.Bosses.PietyMonster;
using Gameplay.GameControllers.Bosses.PietyMonster.Attack;
using UnityEngine;

public class PietyBalanceImporter : BossBalanceImporter
{
	protected override void ApplyLoadedStats()
	{
		if (!this.bossEnemy)
		{
			return;
		}
		this.SetStompAttackDamageAmount();
		this.SetClawAttackDamageAmount();
		this.SetSmashAttackDamageAmount();
		this.SetRootsDamage();
		this.SetSpitAttack();
		this.SetBushAttack();
	}

	private void SetStompAttackDamageAmount()
	{
		PietyStompAttack stompAttack = this.PietyBoss.PietyBehaviour.StompAttack;
		if (!stompAttack)
		{
			return;
		}
		stompAttack.DamageAmount = float.Parse(this.bossLoadedStats["Medium Attack"].ToString());
	}

	private void SetClawAttackDamageAmount()
	{
		PietyClawAttack clawAttack = this.PietyBoss.PietyBehaviour.ClawAttack;
		if (!clawAttack)
		{
			return;
		}
		clawAttack.DamageAmount = float.Parse(this.bossLoadedStats["Medium Attack"].ToString());
	}

	private void SetSmashAttackDamageAmount()
	{
		PietySmashAttack smashAttack = this.PietyBoss.PietyBehaviour.SmashAttack;
		if (!smashAttack)
		{
			return;
		}
		smashAttack.DamageAmount = float.Parse(this.bossLoadedStats["Heavy Attack"].ToString());
	}

	private void SetRootsDamage()
	{
		PietyRootsManager pietyRootsManager = this.PietyBoss.PietyRootsManager;
		if (!pietyRootsManager)
		{
			return;
		}
		pietyRootsManager.RootDamage = float.Parse(this.bossLoadedStats["Light Attack"].ToString());
	}

	private void SetSpitAttack()
	{
		PietySpitAttack componentInChildren = this.PietyBoss.GetComponentInChildren<PietySpitAttack>();
		if (!componentInChildren)
		{
			return;
		}
		componentInChildren.SpitDamage = float.Parse(this.bossLoadedStats["Light Attack"].ToString());
	}

	private void SetBushAttack()
	{
		PietyBushManager pietyBushManager = Object.FindObjectOfType<PietyBushManager>();
		if (!pietyBushManager)
		{
			return;
		}
		pietyBushManager.BushDamage = (int)float.Parse(this.bossLoadedStats["Light Attack"].ToString());
	}

	private PietyMonster PietyBoss
	{
		get
		{
			return (PietyMonster)this.bossEnemy;
		}
	}
}
