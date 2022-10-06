using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Stoners.Animator
{
	public class StonerAnimatorInyector : EnemyAnimatorInyector
	{
		public void Hurt()
		{
			base.EntityAnimator.SetTrigger("HURT");
		}

		public void Raise(Vector3 targetPos)
		{
			if (base.EntityAnimator.speed < 1f)
			{
				base.EntityAnimator.speed = 1f;
			}
			this.SetRaiseOrientation(targetPos);
			Stoners stoners = (Stoners)this.OwnerEntity;
			if (stoners)
			{
				stoners.StonersDamageArea.DamageAreaCollider.enabled = true;
			}
		}

		private void SetRaiseOrientation(Vector3 targetPos)
		{
			string text = (targetPos.x < base.transform.position.x) ? "RisingLeft" : "RisingRight";
			base.EntityAnimator.Play(text);
		}

		public void Attack()
		{
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		public void AllowOrientation(bool allow)
		{
			base.EntityAnimator.SetBool("FLIP", allow);
		}
	}
}
