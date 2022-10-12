using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Crisanta.Animator
{
	public class CrisantaAnimatorInyector : EnemyAnimatorInyector
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<CrisantaAnimatorInyector, Vector2> OnSpinProjectilePoint;

		public void AnimationEvent_SlashProjectile()
		{
			if (this.OnSpinProjectilePoint != null)
			{
				this.OnSpinProjectilePoint(this, Vector2.right);
			}
		}

		public void AnimationEvent_UpwardsSlashStarts()
		{
			UnityEngine.Debug.Log("-----UPWARDS SLASH ATTACK STARTS-----");
			CrisantaBehaviour behaviour = (this.OwnerEntity as Crisanta).Behaviour;
			behaviour.lightAttack.damageOnEnterArea = true;
			behaviour.lightAttack.CurrentWeaponAttack();
		}

		public void AnimationEvent_UpwardsSlashEnds()
		{
			UnityEngine.Debug.Log("-----UPWARDS SLASH ATTACK ENDS-----");
			CrisantaBehaviour behaviour = (this.OwnerEntity as Crisanta).Behaviour;
			behaviour.lightAttack.damageOnEnterArea = false;
		}

		public void AnimationEvent_DownwardsSlashStarts()
		{
			UnityEngine.Debug.Log("-----DOWNWARDS SLASH ATTACK STARTS-----");
			CrisantaBehaviour behaviour = (this.OwnerEntity as Crisanta).Behaviour;
			behaviour.heavyAttack.damageOnEnterArea = true;
			behaviour.heavyAttack.CurrentWeaponAttack();
		}

		public void AnimationEvent_DownwardsSlashEnds()
		{
			UnityEngine.Debug.Log("-----DOWNWARDS SLASH ATTACK ENDS-----");
			CrisantaBehaviour behaviour = (this.OwnerEntity as Crisanta).Behaviour;
			behaviour.heavyAttack.damageOnEnterArea = false;
		}

		public void AnimationEvent_OpenToAttacks()
		{
			CrisantaBehaviour behaviour = (this.OwnerEntity as Crisanta).Behaviour;
			behaviour.SetRecovering(true);
		}

		public void AnimationEvent_CloseAttackWindow()
		{
			CrisantaBehaviour behaviour = (this.OwnerEntity as Crisanta).Behaviour;
			behaviour.SetRecovering(false);
		}

		public void AnimationEvent_SetShieldedOn()
		{
			Crisanta crisanta = this.OwnerEntity as Crisanta;
			if (!crisanta.IsCrisantaRedux)
			{
				(this.OwnerEntity as Enemy).IsGuarding = true;
			}
		}

		public void AnimationEvent_SetShieldedOff()
		{
			Crisanta crisanta = this.OwnerEntity as Crisanta;
			if (!crisanta.IsCrisantaRedux)
			{
				(this.OwnerEntity as Enemy).IsGuarding = false;
			}
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
			CrisantaBehaviour behaviour = (this.OwnerEntity as Crisanta).Behaviour;
			if (!behaviour.ignoreAnimDispl)
			{
				behaviour.AttackDisplacement(0.4f, 2.5f, true);
			}
		}

		public void AnimationEvent_MediumAttackDisplacement()
		{
			CrisantaBehaviour behaviour = (this.OwnerEntity as Crisanta).Behaviour;
			if (!behaviour.ignoreAnimDispl)
			{
				behaviour.AttackDisplacementToPoint(Core.Logic.Penitent.transform.position, 1.2f, 30f, true);
			}
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void CancelAll()
		{
			this.Guard(false);
			this.Blinkslash(false);
			this.ComboMode(false);
			this.AirAttack(false);
			base.EntityAnimator.ResetTrigger("DEATH");
			base.EntityAnimator.ResetTrigger("UPWARDS_SLASH");
			base.EntityAnimator.ResetTrigger("DOWNWARDS_SLASH");
			base.EntityAnimator.ResetTrigger("BLINKIN");
			base.EntityAnimator.ResetTrigger("BLINKOUT");
			base.EntityAnimator.ResetTrigger("PARRY");
			base.EntityAnimator.ResetTrigger("HURT");
			base.EntityAnimator.ResetTrigger("BACKFLIP");
			base.EntityAnimator.ResetTrigger("CHANGE_STANCE");
		}

		public void Backflip()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("BACKFLIP");
		}

		public void DeathBackflip()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			this.Backflip();
			this.Death();
		}

		public void BackflipLand()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("LAND");
		}

		public void AirAttack(bool active)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("AIR_ATTACK", active);
		}

		public void ChangeStance()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("CHANGE_STANCE");
		}

		public void Guard(bool active)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("GUARD", active);
		}

		public void Unseal()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("UNSEAL");
		}

		public void UpwardsSlash()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			UnityEngine.Debug.Log("Setting trigger Upward Slash");
			base.EntityAnimator.SetTrigger("UPWARDS_SLASH");
		}

		public void DownwardsSlash()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			UnityEngine.Debug.Log("Setting trigger Downward Slash");
			base.EntityAnimator.SetTrigger("DOWNWARDS_SLASH");
		}

		public void BlinkIn()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("BLINKIN");
		}

		public void BlinkOut()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("BLINKOUT");
		}

		public void Blinkslash(bool blinkIn)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("BLINKSLASH", blinkIn);
		}

		public void ComboMode(bool active)
		{
			UnityEngine.Debug.Log("Setting combo mode " + active);
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("COMBO_MODE", active);
		}

		public void Parry()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("PARRY");
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

		public void SetStayKneeling(bool active)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("STAY_KNEELING", active);
		}

		public void PlayHurtRecovery()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.Play("HURT_RECOVERY", 0, 0f);
		}
	}
}
