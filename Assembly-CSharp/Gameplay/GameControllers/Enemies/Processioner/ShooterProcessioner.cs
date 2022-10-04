using System;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Enemies.Processioner.Animator;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Processioner
{
	public class ShooterProcessioner : Processioner, IDamageable
	{
		public BossStraightProjectileAttack ProjectileAttack { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.ProjectileAttack = base.GetComponentInChildren<BossStraightProjectileAttack>();
			base.Behaviour.enabled = false;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			else
			{
				base.Behaviour.enabled = true;
			}
		}

		public void Damage(Hit hit)
		{
			base.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				ShooterProcessionerAnimator shooterProcessionerAnimator = (ShooterProcessionerAnimator)base.ProcessionerAnimator;
				shooterProcessionerAnimator.Death();
				base.DamageArea.DamageAreaCollider.enabled = false;
				return;
			}
			base.ColorFlash.TriggerColorFlash();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}
	}
}
