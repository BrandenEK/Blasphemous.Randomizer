using System;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.MiriamPortal.Animation
{
	public class MiriamPortalPrayerAnimationHandler : MonoBehaviour
	{
		public MiriamPortalPrayer MiriamPortalPrayer { get; set; }

		public void SetAnimatorTrigger(int animatorTrigger)
		{
			if (this.MiriamPortalPrayer == null)
			{
				return;
			}
			this.MiriamPortalPrayer.Animator.SetTrigger(animatorTrigger);
		}

		public void Appearing()
		{
			if (this.MiriamPortalPrayer == null)
			{
				return;
			}
			this.SetInvisible(1);
			this.MiriamPortalPrayer.Animator.Play("Appearing");
		}

		public void StopAnimator()
		{
			if (this.MiriamPortalPrayer == null)
			{
				return;
			}
			this.MiriamPortalPrayer.Animator.speed = 0.1f;
		}

		public void RestartAnimator()
		{
			if (this.MiriamPortalPrayer == null)
			{
				return;
			}
			this.MiriamPortalPrayer.Animator.speed = 1f;
		}

		public void WeaponAttack()
		{
			if (this.MiriamPortalPrayer == null)
			{
				return;
			}
			this.MiriamPortalPrayer.Attack.DealsDamage = true;
		}

		public void WeaponAttackFinished()
		{
			if (this.MiriamPortalPrayer == null)
			{
				return;
			}
			this.MiriamPortalPrayer.Attack.DealsDamage = false;
		}

		public void Vanish()
		{
			if (this.MiriamPortalPrayer.Behaviour.ReappearFlag)
			{
				return;
			}
			this.MiriamPortalPrayer.gameObject.SetActive(false);
		}

		public void SetInvisible(int visible = 1)
		{
			float num = Mathf.Clamp01((float)visible);
			this.MiriamPortalPrayer.SpriteRenderer.enabled = (num > 0f);
		}

		public static readonly int AttackTrigger = Animator.StringToHash("ATTACK");

		public static readonly int VanishTrigger = Animator.StringToHash("VANISH");

		public static readonly int TurnTrigger = Animator.StringToHash("TURN");
	}
}
