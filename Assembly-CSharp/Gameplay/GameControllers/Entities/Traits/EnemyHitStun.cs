using System;
using System.Collections;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Traits
{
	public class EnemyHitStun : Trait
	{
		protected override void OnTraitEnable()
		{
			base.OnTraitEnable();
			if (!base.EntityOwner)
			{
				return;
			}
			this.entityAnimator = base.EntityOwner.Animator;
			base.EntityOwner.OnDamaged -= this.EntityOwner_OnDamaged;
			base.EntityOwner.OnDamaged += this.EntityOwner_OnDamaged;
		}

		private void EntityOwner_OnDamaged()
		{
			this.OnDamage();
		}

		public void OnDamage()
		{
			if (this.hitStunCoroutine != null)
			{
				base.StopCoroutine(this.hitStunCoroutine);
			}
			this.hitStunCoroutine = base.StartCoroutine(this.ActivateHitStun(this.hitStunAmount));
		}

		private IEnumerator ActivateHitStun(float seconds)
		{
			this.onHitStun = true;
			this.SetAnimatorSpeed(0f);
			yield return new WaitForSeconds(seconds);
			this.onHitStun = false;
			this.SetAnimatorSpeed(1f);
			yield break;
		}

		private void SetAnimatorSpeed(float spd)
		{
			this.entityAnimator.speed = spd;
		}

		private Animator entityAnimator;

		private Coroutine hitStunCoroutine;

		public bool onHitStun;

		public float hitStunAmount = 0.25f;
	}
}
