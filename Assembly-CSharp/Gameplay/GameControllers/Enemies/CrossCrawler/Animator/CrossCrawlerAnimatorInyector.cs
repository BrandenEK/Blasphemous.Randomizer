using System;
using Gameplay.GameControllers.Enemies.CrossCrawler.IA;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Enemies.CrossCrawler.Animator
{
	public class CrossCrawlerAnimatorInyector : EnemyAnimatorInyector
	{
		protected override void OnStart()
		{
			base.OnStart();
			this.CrossCrawler = base.GetComponentInParent<CrossCrawler>();
		}

		public void TurnAround()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TURN");
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

		public void AttackAnimationEvent()
		{
			CrossCrawler crossCrawler = (CrossCrawler)this.OwnerEntity;
			crossCrawler.Attack.CurrentWeaponAttack();
		}

		public void StartAttackAnimationEvent()
		{
			this.CrossCrawler.Audio.PlayAttack();
		}

		public void SetVulnerableTrue()
		{
			this.CrossCrawler.Behaviour.StartVulnerablePeriod();
		}

		public void WalkAnimationEvent()
		{
			this.CrossCrawler.Audio.PlayWalk();
		}

		public void TurnAnimationEvent()
		{
			this.CrossCrawler.Audio.PlayTurnAround();
		}

		public void PlayCrossCrawlerTurnMoveOne()
		{
			this.CrossCrawler.Audio.SetTurnMoveParam(1f);
		}

		public void PlayCrossCrawlerAtkMoveOne()
		{
			this.CrossCrawler.Audio.SetAttackMoveParam(1f);
		}

		public void PlayCrossCrawlerAtkMoveTwo()
		{
			this.CrossCrawler.Audio.SetAttackMoveParam(2f);
		}

		public void DeathAnimationEvent()
		{
			this.CrossCrawler.Audio.PlayDeath();
		}

		public void ResetToIdle()
		{
			base.EntityAnimator.ResetTrigger("ATTACK");
			base.EntityAnimator.Play("Idle");
		}

		public void ResetCoolDownAttack()
		{
			CrossCrawlerBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<CrossCrawlerBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}

		private CrossCrawler CrossCrawler;
	}
}
