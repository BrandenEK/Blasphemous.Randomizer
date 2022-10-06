using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.MeltedLady.IA;
using Gameplay.GameControllers.Penitent.Gizmos;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.Animator
{
	public class MeltedLadyAnimatorInyector : FloatingLadyAnimatorInjector
	{
		public override void Attack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("ATTACK");
		}

		private Quaternion GetShotRotation()
		{
			MeltedLady meltedLady = (MeltedLady)this.OwnerEntity;
			Vector3 position = meltedLady.transform.position;
			Vector2 vector;
			if (meltedLady.Target.gameObject.transform.position.x < position.x)
			{
				vector = this.OwnerEntity.transform.position - meltedLady.Target.transform.position;
			}
			else
			{
				vector = meltedLady.Target.transform.position - this.OwnerEntity.transform.position;
			}
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			num = Mathf.Clamp(num, -20f, 20f);
			return Quaternion.AngleAxis(num, Vector3.forward);
		}

		public override void Hurt()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			if (base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
			{
				return;
			}
			base.EntityAnimator.SetTrigger("HURT");
			base.EntityAnimator.ResetTrigger("ATTACK");
			this.SetDefaultRotation(0.1f);
		}

		public override void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public override void TeleportOut()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TELEPORT_OUT");
		}

		public override void TeleportIn()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TELEPORT_IN");
		}

		public void AttackAnimationEvent()
		{
		}

		public void SetTargetRotation(float timeLapse)
		{
			MeltedLady meltedLady = (MeltedLady)this.OwnerEntity;
			TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotateQuaternion(meltedLady.SpriteRenderer.transform, this.GetShotRotation(), timeLapse), this.AttackAnimationCurve);
		}

		public void SetDefaultRotation(float timeLapse)
		{
			MeltedLady meltedLady = (MeltedLady)this.OwnerEntity;
			DOTween.Kill(meltedLady.SpriteRenderer.transform, false);
			ShortcutExtensions.DORotateQuaternion(meltedLady.SpriteRenderer.transform, Quaternion.identity, timeLapse);
		}

		public void GetTarget()
		{
			MeltedLady meltedLady = (MeltedLady)this.OwnerEntity;
			this._target = meltedLady.Target.transform.position;
		}

		public void LaunchProjectile()
		{
			MeltedLady meltedLady = (MeltedLady)this.OwnerEntity;
			RootMotionDriver projectileLaunchRoot = meltedLady.ProjectileLaunchRoot;
			Vector3 vector = (meltedLady.Status.Orientation != EntityOrientation.Right) ? projectileLaunchRoot.FlipedPosition : projectileLaunchRoot.transform.position;
			Vector3 target = this._target;
			target.y += 1f;
			Vector3 normalized = (target - vector).normalized;
			meltedLady.Attack.Shoot(vector, normalized);
			this.Recoil(this.OwnerEntity.transform.position + normalized.normalized * -this.RecoilDistance);
		}

		private void Recoil(Vector2 dir)
		{
			ShortcutExtensions.DOMove(this.OwnerEntity.transform, dir, this.RecoilLapse, false);
		}

		public void ResetCoolDownAttack()
		{
			MeltedLadyBehaviour componentInChildren = this.OwnerEntity.GetComponentInChildren<MeltedLadyBehaviour>();
			if (componentInChildren != null)
			{
				componentInChildren.ResetCoolDown();
			}
		}

		public void TriggerTeleport()
		{
			MeltedLady meltedLady = (MeltedLady)this.OwnerEntity;
			if (meltedLady.Status.IsHurt)
			{
				meltedLady.Behaviour.TeleportToTarget();
			}
		}

		public void Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}

		private Vector3 _target;

		public AnimationCurve AttackAnimationCurve;

		public float RecoilLapse = 0.15f;

		public float RecoilDistance = 0.5f;
	}
}
