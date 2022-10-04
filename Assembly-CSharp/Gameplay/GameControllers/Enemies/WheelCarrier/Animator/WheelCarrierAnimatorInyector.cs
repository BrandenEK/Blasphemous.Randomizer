using System;
using Gameplay.GameControllers.Enemies.WheelCarrier.IA;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.WheelCarrier.Animator
{
	public class WheelCarrierAnimatorInyector : EnemyAnimatorInyector
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._wheelCarrier = base.GetComponentInParent<WheelCarrier>();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			base.EntityAnimator.SetBool("IS_PARRIED", this._wheelCarrier.Behaviour.GotParry);
		}

		public void Walk()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", true);
		}

		public void Stop()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
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

		public void ParryReaction()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.Play("Parry");
		}

		public void ResetCoolDownAttack()
		{
			WheelCarrierBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<WheelCarrierBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}

		public void DisposeEnemy()
		{
			this._wheelCarrier.gameObject.SetActive(false);
		}

		public void SetVulnerableTrue()
		{
			this._wheelCarrier.Behaviour.StartVulnerablePeriod();
		}

		public void AttackAnimationEvent()
		{
			WheelCarrier wheelCarrier = (WheelCarrier)this.OwnerEntity;
			wheelCarrier.Attack.CurrentWeaponAttack();
		}

		public void PlayAttack()
		{
			this._wheelCarrier.Audio.PlayAttack();
		}

		public void PlayDeathAnimation()
		{
			this._wheelCarrier.Audio.PlayDeath();
		}

		private WheelCarrier _wheelCarrier;
	}
}
