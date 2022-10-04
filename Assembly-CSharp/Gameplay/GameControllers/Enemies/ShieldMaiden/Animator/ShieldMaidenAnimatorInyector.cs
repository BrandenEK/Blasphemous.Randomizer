using System;
using Gameplay.GameControllers.Enemies.ShieldMaiden.Attack;
using Gameplay.GameControllers.Enemies.ShieldMaiden.IA;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.ShieldMaiden.Animator
{
	public class ShieldMaidenAnimatorInyector : EnemyAnimatorInyector
	{
		public void Walk()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", true);
		}

		public void Parry()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("PARRY", true);
		}

		public void ParryRecover()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("PARRY", false);
		}

		public void Stop()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
		}

		public void Recover()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("RECOVER");
		}

		public void Attack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void AttackAnimationEvent()
		{
			ShieldMaiden shieldMaiden = (ShieldMaiden)this.OwnerEntity;
			shieldMaiden.Attack.CurrentWeaponAttack();
		}

		public void ActivateShieldAnimationEvent()
		{
			ShieldMaiden shieldMaiden = (ShieldMaiden)this.OwnerEntity;
			shieldMaiden.Behaviour.ToggleShield(true);
		}

		public void StopAll()
		{
			base.EntityAnimator.Play("Idle");
		}

		public void ResetCoolDownAttack()
		{
			ShieldMaidenBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<ShieldMaidenBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}

		public ShieldMaidenAttack attack;
	}
}
