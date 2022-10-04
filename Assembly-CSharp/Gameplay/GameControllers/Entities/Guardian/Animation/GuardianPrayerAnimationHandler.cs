using System;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Guardian.Animation
{
	public class GuardianPrayerAnimationHandler : MonoBehaviour
	{
		public GuardianPrayer GuardianPrayer { get; set; }

		public void SetAnimatorTrigger(int animatorTrigger)
		{
			if (this.GuardianPrayer == null)
			{
				return;
			}
			this.GuardianPrayer.Animator.SetTrigger(animatorTrigger);
		}

		public void Appearing()
		{
			this.SetInvisible(1);
			this.GuardianPrayer.Animator.Play("Appearing");
		}

		public void WeaponAttack()
		{
			if (this.GuardianPrayer == null)
			{
				return;
			}
			this.GuardianPrayer.Attack.CurrentWeaponAttack();
		}

		public void Vanish()
		{
			this.GuardianPrayer.gameObject.SetActive(false);
		}

		public void SetInvisible(int visible = 1)
		{
			float num = Mathf.Clamp01((float)visible);
			this.GuardianPrayer.SpriteRenderer.enabled = (num > 0f);
		}

		public static readonly int AttackTrigger = Animator.StringToHash("ATTACK");

		public static readonly int GuardTrigger = Animator.StringToHash("GUARD");

		public static readonly int VanishTrigger = Animator.StringToHash("VANISH");

		public static readonly int TurnTrigger = Animator.StringToHash("TURN");

		public static readonly int AwaitingTrigger = Animator.StringToHash("AWAITING");
	}
}
