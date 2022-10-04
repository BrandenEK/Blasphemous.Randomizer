using System;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.FireTrap
{
	public class ElmFireTrapAttack : MonoBehaviour
	{
		private void Awake()
		{
			this.AttackArea = base.GetComponentInChildren<CircleAttackArea>();
			this.AttackArea.OnEnter += this.OnEnterAttackArea;
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			IDamageable componentInParent = e.Collider2DArg.GetComponentInParent<IDamageable>();
			this.ProximityAttack(componentInParent);
		}

		private void ProximityAttack(IDamageable damageable)
		{
			if (damageable != null)
			{
				damageable.Damage(this.ProximityHitAttack);
			}
		}

		private void OnDestroy()
		{
			if (this.AttackArea)
			{
				this.AttackArea.OnEnter -= this.OnEnterAttackArea;
			}
		}

		public Hit ProximityHitAttack;

		private CircleAttackArea AttackArea;
	}
}
