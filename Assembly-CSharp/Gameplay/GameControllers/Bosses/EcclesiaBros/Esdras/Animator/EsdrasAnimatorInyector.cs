using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras.Animator
{
	public class EsdrasAnimatorInyector : EnemyAnimatorInyector
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<EsdrasAnimatorInyector> OnHeavyAttackLightningSummon;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<EsdrasAnimatorInyector, Vector2> OnSpinProjectilePoint;

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

		public void AnimationEvent_LightAttack()
		{
			EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
			behaviour.lightAttack.CurrentWeaponAttack();
		}

		public void AnimationEvent_HeavyAttack()
		{
			EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
			behaviour.heavyAttack.CurrentWeaponAttack();
		}

		public void AnimationEvent_SpinAttackStarts()
		{
			EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
			behaviour.singleSpinAttack.CurrentWeaponAttack();
			behaviour.singleSpinAttack.DealsDamage = true;
		}

		public void AnimationEvent_SpinAttackEnds()
		{
			EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
			behaviour.singleSpinAttack.DealsDamage = false;
		}

		public void AnimationEvent_OpenToAttacks()
		{
			EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
			behaviour.SetRecovering(true);
		}

		public void AnimationEvent_CloseAttackWindow()
		{
			EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
			UnityEngine.Debug.Log("CLOSING ATTACK WINDOW");
			behaviour.SetRecovering(false);
		}

		public void AnimationEvent_SetShieldedOn()
		{
			if (this.OwnerEntity is Enemy)
			{
				(this.OwnerEntity as Enemy).IsGuarding = true;
			}
		}

		public void AnimationEvent_SetShieldedOff()
		{
			if (this.OwnerEntity is Enemy)
			{
				(this.OwnerEntity as Enemy).IsGuarding = false;
			}
		}

		public void AnimationEvent_LightScreenShake()
		{
			Vector2 a = (this.OwnerEntity.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.15f, a * 0.5f, 10, 0.01f, 0f, default(Vector3), 0.01f, false);
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
			EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
			behaviour.AttackDisplacement(0.4f, 2f, true);
		}

		public void AnimationEvent_SpinAttackDisplacement()
		{
			EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
			behaviour.AttackDisplacement(1.5f, 10f, false);
		}

		public void AnimationEvent_TauntImpact()
		{
			if (this.OwnerEntity is Esdras)
			{
				EsdrasBehaviour behaviour = (this.OwnerEntity as Esdras).Behaviour;
				behaviour.CounterImpactShockwave();
			}
		}

		public void SummonLightning()
		{
			if (this.OnHeavyAttackLightningSummon != null)
			{
				this.OnHeavyAttackLightningSummon(this);
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

		public void SpinAttack()
		{
			UnityEngine.Debug.Log("AnimIn: SPIN ATK");
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("SPIN_ATTACK");
		}

		public void SpinLoop(bool active)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("SPIN_LOOP", active);
		}

		public void LightAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("LIGHT_ATTACK");
		}

		public void HeavyAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("HEAVY_ATTACK");
		}

		public void Taunt()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("TAUNT");
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

		public void Run(bool run)
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("RUNNING", run);
		}
	}
}
