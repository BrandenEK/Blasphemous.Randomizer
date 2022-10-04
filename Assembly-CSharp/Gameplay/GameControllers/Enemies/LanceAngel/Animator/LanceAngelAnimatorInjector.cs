using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.LanceAngel.Animator
{
	public class LanceAngelAnimatorInjector : EnemyAnimatorInyector
	{
		public void AttackReady()
		{
			base.EntityAnimator.SetTrigger("ATTACK_READY");
		}

		public void AttackStart()
		{
			base.EntityAnimator.SetTrigger("ATTACK_START");
			base.EntityAnimator.SetBool("ATTACKING", true);
		}

		public void StopAttack()
		{
			base.EntityAnimator.SetBool("ATTACKING", false);
		}

		public void Death()
		{
			this.StopAttack();
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void AnimationEvent_DisposeEntity()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}

		public void AnimationEvent_OnStopReposition()
		{
			LanceAngel lanceAngel = (LanceAngel)this.OwnerEntity;
			lanceAngel.Behaviour.Dash();
		}
	}
}
