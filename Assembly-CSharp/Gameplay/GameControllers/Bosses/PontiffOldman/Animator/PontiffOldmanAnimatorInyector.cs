using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffOldman.Animator
{
	public class PontiffOldmanAnimatorInyector : EnemyAnimatorInyector
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PontiffOldmanAnimatorInyector, Vector2> OnSpinProjectilePoint;

		public void AnimationEvent_OpenToAttacks()
		{
			PontiffOldmanBehaviour behaviour = (this.OwnerEntity as PontiffOldman).Behaviour;
			behaviour.SetRecovering(true);
		}

		public void AnimationEvent_CloseAttackWindow()
		{
			PontiffOldmanBehaviour behaviour = (this.OwnerEntity as PontiffOldman).Behaviour;
			behaviour.SetRecovering(false);
		}

		public void AnimationEvent_LightScreenShake()
		{
			Vector2 a = (this.OwnerEntity.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.15f, a * 0.5f, 10, 0.01f, 0f, default(Vector3), 0.01f, false);
		}

		public void AnimationEvent_HeavyScreenShake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.3f, Vector3.up * 3f, 60, 0.01f, 0f, default(Vector3), 0.01f, false);
		}

		public void AnimationEvent_ShortAttackDisplacement()
		{
			PontiffOldmanBehaviour behaviour = (this.OwnerEntity as PontiffOldman).Behaviour;
			behaviour.AttackDisplacement(0.4f, 2.5f, true);
		}

		public void AnimationEvent_MediumAttackDisplacement()
		{
			PontiffOldmanBehaviour behaviour = (this.OwnerEntity as PontiffOldman).Behaviour;
			behaviour.AttackDisplacement(0.5f, 5.5f, true);
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
			this.castFXAnimator.SetBool("CASTING", false);
		}

		public void CancelAll()
		{
			this.Cast(false);
			this.Vanish(false);
			this.ComboMode(false);
			base.EntityAnimator.ResetTrigger("DEATH");
			base.EntityAnimator.ResetTrigger("HURT");
		}

		public void Cast(bool cast)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("CASTING", cast);
			this.castFXAnimator.SetBool("CASTING", cast);
		}

		public void Vanish(bool dissapear)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger((!dissapear) ? "APPEAR" : "VANISH");
		}

		public void ComboMode(bool active)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("COMBO_MODE", active);
		}

		public void Hurt()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HURT"))
			{
				UnityEngine.Debug.Log("PLAY TRICK HURT");
				base.EntityAnimator.Play("HURT", 0, 0f);
			}
			else
			{
				base.EntityAnimator.SetTrigger("HURT");
			}
		}

		public Animator castFXAnimator;
	}
}
