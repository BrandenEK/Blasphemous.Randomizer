using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.MeltedLady.Animator
{
	public class InkLadyAnimatorInjector : FloatingLadyAnimatorInjector
	{
		protected InkLady InkLady { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.InkLady = (InkLady)this.OwnerEntity;
		}

		public override void Attack()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(InkLadyAnimatorInjector.AttackParam);
			this.InkLady.IsAttacking = true;
			this.LaunchBeam();
		}

		public override void Hurt()
		{
			if (!base.EntityAnimator || this.InkLady.IsAttacking)
			{
				return;
			}
			if (base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
			{
				return;
			}
			base.EntityAnimator.SetTrigger(InkLadyAnimatorInjector.HurtParam);
			base.EntityAnimator.ResetTrigger(InkLadyAnimatorInjector.AttackParam);
		}

		public override void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.ResetTrigger(InkLadyAnimatorInjector.AttackParam);
			base.EntityAnimator.SetTrigger(InkLadyAnimatorInjector.DeathParam);
		}

		public override void TeleportOut()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(InkLadyAnimatorInjector.Out);
		}

		public override void TeleportIn()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(InkLadyAnimatorInjector.In);
		}

		public void TriggerTeleport()
		{
			InkLady inkLady = (InkLady)this.OwnerEntity;
			if (inkLady.Status.IsHurt)
			{
				inkLady.Behaviour.TeleportToTarget();
			}
		}

		private void SetBeamPosition()
		{
			Vector3 localPosition = this.InkLady.Attack.transform.localPosition;
			localPosition.x = Mathf.Abs(localPosition.x);
			localPosition.x = ((!this.InkLady.SpriteRenderer.flipX) ? localPosition.x : (-localPosition.x));
			this.InkLady.Attack.transform.localPosition = localPosition;
		}

		private void LaunchBeam()
		{
			if (this.InkLady.Status.Dead)
			{
				return;
			}
			this.LookAtTarget();
			float num = 0.55f;
			this.InkLady.Attack.DelayedTargetedBeam(this.InkLady.Target.transform, num, this.InkLady.BeamAttackTime, this.InkLady.Status.Orientation, true);
			base.StartCoroutine(this.CheckIfDeadBeforeFiring(num * 0.9f));
		}

		private IEnumerator CheckIfDeadBeforeFiring(float warningDelay)
		{
			yield return new WaitForSeconds(warningDelay);
			if (this.InkLady.Status.Dead)
			{
				this.InkLady.Attack.Clear();
			}
			yield break;
		}

		public void LookAtTarget()
		{
			this.InkLady.Behaviour.LookAtTarget(this.InkLady.Target.transform.position);
			this.SetBeamPosition();
		}

		public void Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}

		private static readonly int In = Animator.StringToHash("TELEPORT_IN");

		private static readonly int Out = Animator.StringToHash("TELEPORT_OUT");

		private static readonly int DeathParam = Animator.StringToHash("DEATH");

		private static readonly int AttackParam = Animator.StringToHash("ATTACK");

		private static readonly int HurtParam = Animator.StringToHash("HURT");
	}
}
