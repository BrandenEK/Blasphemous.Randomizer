using System;
using DG.Tweening;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.HighWills
{
	public class HighWills : Enemy, IDamageable
	{
		public HighWillsBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public VisionCone VisionCone { get; private set; }

		public MasterShaderEffects DamageEffect { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<HighWillsBehaviour>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.DamageEffect = base.GetComponentInChildren<MasterShaderEffects>();
		}

		public void ActivateLeftHWEyes(float activationTime, float activatedTime, float deactivationTime)
		{
			this.StartEyesActivationSequence(this.LeftHWEyes, activationTime, activatedTime, deactivationTime);
		}

		public void ActivateMiddleHWEyes(float activationTime, float activatedTime, float deactivationTime)
		{
			this.StartEyesActivationSequence(this.MiddleHWEyes, activationTime, activatedTime, deactivationTime);
		}

		public void ActivateRightHWEyes(float activationTime, float activatedTime, float deactivationTime)
		{
			this.StartEyesActivationSequence(this.RightHWEyes, activationTime, activatedTime, deactivationTime);
		}

		private void StartEyesActivationSequence(SpriteRenderer eyes, float activationTime, float activatedTime, float deactivationTime)
		{
			Sequence s = DOTween.Sequence();
			s.Append(eyes.DOFade(1f, activationTime)).AppendInterval(activatedTime).Append(eyes.DOFade(0f, deactivationTime));
		}

		public void DamageFlash()
		{
			this.DamageEffect.DamageEffectBlink(0f, 0.07f, null);
		}

		public void Damage(Hit hit)
		{
			if (this.WillDieByHit(hit))
			{
			}
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				this.Behaviour.LaunchDeathAction();
			}
			else
			{
				this.DamageFlash();
				this.SleepTimeByHit(hit);
			}
		}

		public void DamageByCrisanta()
		{
			Hit hit = new Hit
			{
				DamageAmount = 20f
			};
			this.Damage(hit);
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public Vector3 GetPosition()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}

		public float GetHpPercentage()
		{
			return this.Stats.Life.Current / this.Stats.Life.CurrentMax;
		}

		public SpriteRenderer LeftHWEyes;

		public SpriteRenderer MiddleHWEyes;

		public SpriteRenderer RightHWEyes;
	}
}
