using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Legionary.Animator
{
	public class LegionaryAnimator : EnemyAnimatorInyector
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<LegionaryAnimator> OnHeavyAttackLightningSummon;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<LegionaryAnimator, Vector2> OnSpinProjectilePoint;

		private protected Legionary Legionary { protected get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Legionary = (Legionary)this.OwnerEntity;
		}

		public void Walk(bool walk = true)
		{
			base.EntityAnimator.SetBool("WALK", walk);
			if (walk)
			{
				base.EntityAnimator.SetBool("RUN", false);
			}
		}

		public void Run(bool run = true)
		{
			base.EntityAnimator.SetBool("RUN", run);
			if (run)
			{
				base.EntityAnimator.SetBool("WALK", false);
			}
		}

		public void Parry()
		{
			base.EntityAnimator.SetTrigger("PARRY");
		}

		public void SpinAttack()
		{
			base.EntityAnimator.SetTrigger("SPIN_ATTACK");
		}

		public void Hurt()
		{
			if (base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
			{
				this.PlayDamageAnim();
			}
			else
			{
				base.EntityAnimator.SetTrigger("HURT");
			}
		}

		public void Death()
		{
			this.Walk(false);
			this.Run(false);
			base.EntityAnimator.SetTrigger("DEATH");
		}

		private void PlayDamageAnim()
		{
			base.EntityAnimator.Play("Hurt", 0, 0f);
		}

		public void LightningSummon()
		{
			base.EntityAnimator.SetTrigger("SUMMON_ATTACK");
		}

		public void LightAttack()
		{
			base.EntityAnimator.SetTrigger("LIGHT_ATTACK");
		}

		public void AnimationEvent_SpinProjectilePointRight()
		{
			if (this.OnSpinProjectilePoint != null)
			{
				this.OnSpinProjectilePoint(this, Vector2.right);
			}
		}

		public void AnimationEvent_SpinProjectilePointLeft()
		{
			if (this.OnSpinProjectilePoint != null)
			{
				this.OnSpinProjectilePoint(this, Vector2.left);
			}
		}

		public void AnimationEvent_LightAttackStarts()
		{
			this.Legionary.LightAttack.DealsDamage = true;
		}

		public void AnimationEvent_LightAttackEnds()
		{
			this.Legionary.LightAttack.DealsDamage = false;
		}

		public void AnimationEvent_HeavyAttack()
		{
			this.Legionary.SpinAttack.CurrentWeaponAttack();
		}

		public void AnimationEvent_TauntImpact()
		{
			this.Legionary.Behaviour.LightningSummonAttack();
		}

		public void AnimationEvent_SpinAttackStarts()
		{
			this.Legionary.SpinAttack.DealsDamage = true;
		}

		public void AnimationEvent_SpinAttackEnds()
		{
			this.Legionary.SpinAttack.DealsDamage = false;
		}

		public void AnimationEvent_StopLerping()
		{
			this.Legionary.MotionLerper.StopLerping();
		}

		public void AnimationEvent_OpenToAttacks()
		{
			this.Legionary.CanTakeDamage = true;
		}

		public void AnimationEvent_CloseAttackWindow()
		{
			this.Legionary.CanTakeDamage = false;
		}

		public void AnimationEvent_SetShieldedOn()
		{
			this.Legionary.IsGuarding = true;
		}

		public void AnimationEvent_SetShieldedOff()
		{
			this.Legionary.IsGuarding = false;
		}

		public void AnimationEvent_DisableEntity()
		{
			this.Legionary.gameObject.SetActive(false);
		}

		public void AnimationEvent_LightScreenShake()
		{
			Vector2 vector = (this.OwnerEntity.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.15f, vector * 0.5f, 10, 0.01f, 0f, default(Vector3), 0.01f, false);
		}

		public void AnimationEvent_HeavySecondScreenShake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.3f, Vector3.up * 3f, 60, 0.01f, 0f, default(Vector3), 0.01f, false);
		}

		public void AnimationEvent_HeavyFirstScreenShake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.7f, Vector3.down * 2f, 30, 0.01f, 0f, default(Vector3), 0.001f, false);
		}

		public void AnimationEvent_AttackDisplacement()
		{
			Vector3 dir = (this.Legionary.Status.Orientation != EntityOrientation.Right) ? (-base.transform.right) : base.transform.right;
			this.Legionary.MotionLerper.distanceToMove = 1.5f;
			this.Legionary.MotionLerper.TimeTakenDuringLerp = 0.2f;
			this.Legionary.MotionLerper.StartLerping(dir);
		}

		public void AnimationEvent_SpinAttackDisplacement()
		{
			Vector3 dir = (this.Legionary.Status.Orientation != EntityOrientation.Right) ? (-base.transform.right) : base.transform.right;
			this.Legionary.MotionLerper.distanceToMove = 4.5f;
			this.Legionary.MotionLerper.TimeTakenDuringLerp = 1.75f;
			this.Legionary.MotionLerper.StartLerping(dir);
		}
	}
}
