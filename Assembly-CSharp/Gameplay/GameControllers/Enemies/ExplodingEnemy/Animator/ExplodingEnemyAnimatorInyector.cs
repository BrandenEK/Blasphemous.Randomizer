using System;
using Gameplay.GameControllers.Enemies.ExplodingEnemy.Attack;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.ExplodingEnemy.Animator
{
	public class ExplodingEnemyAnimatorInyector : EnemyAnimatorInyector
	{
		public void Damage()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void Walk()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (!base.EntityAnimator.GetBool("WALK"))
			{
				base.EntityAnimator.SetBool("WALK", true);
			}
		}

		public void StopWalk()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (base.EntityAnimator.GetBool("WALK"))
			{
				base.EntityAnimator.SetBool("WALK", false);
			}
		}

		public void ChargeExplosion()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("CHARGE_EXPLOSION");
		}

		public void TurnAround()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TURN");
		}

		public void Vanish()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("VANISH");
		}

		public void DisableExplosionAttack()
		{
			ExplodingEnemy explodingEnemy = (ExplodingEnemy)this.OwnerEntity;
			ExplodingEnemyAttack explodingEnemyAttack = (ExplodingEnemyAttack)explodingEnemy.EntityAttack;
			explodingEnemyAttack.HasExplode = true;
		}

		public bool IsExploding
		{
			get
			{
				return base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Explode");
			}
		}
	}
}
