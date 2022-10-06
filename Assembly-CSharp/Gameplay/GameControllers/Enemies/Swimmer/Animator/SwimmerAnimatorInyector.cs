using System;
using DG.Tweening;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Swimmer.Animator
{
	public class SwimmerAnimatorInyector : EnemyAnimatorInyector
	{
		private protected Vector2 GroundPosition { protected get; private set; }

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.SetJumpParam();
			if (this.OwnerEntity.Status.IsGrounded)
			{
				this.GroundPosition = this.OwnerEntity.transform.position;
			}
		}

		public void SetJumpParam()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("GROUNDED", this.OwnerEntity.Status.IsGrounded);
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}

		public void SpriteVisible(bool visible, float timeElapsed, Action callback = null)
		{
			TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions43.DOFade(this.OwnerEntity.SpriteRenderer, (!visible) ? 0f : 1f, timeElapsed), delegate()
			{
				if (callback != null)
				{
					callback();
				}
			});
		}

		public void TerrainEffectRising(int isRising)
		{
			Swimmer swimmer = (Swimmer)this.OwnerEntity;
			Vector3 position = swimmer.transform.position;
			position.y = this.GroundPosition.y;
			swimmer.Behaviour.RisingTerrainEffect(isRising > 0, position);
		}

		public void Attack()
		{
			Swimmer swimmer = (Swimmer)this.OwnerEntity;
			swimmer.Behaviour.Attack();
		}
	}
}
