using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PontiffHusk.Audio;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffHusk
{
	public class PontiffHuskBoss : Enemy, IDamageable
	{
		public PontiffHuskBossBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public MasterShaderEffects DamageEffect { get; private set; }

		public PontiffHuskBossAudio Audio { get; private set; }

		public PontiffHuskBossAnimatorInyector AnimatorInyector { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Audio = base.GetComponentInChildren<PontiffHuskBossAudio>();
			this.Behaviour = base.GetComponent<PontiffHuskBossBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.DamageEffect = base.GetComponentInChildren<MasterShaderEffects>();
			this.AnimatorInyector = base.GetComponentInChildren<PontiffHuskBossAnimatorInyector>();
		}

		public void DamageFlash()
		{
			this.DamageEffect.DamageEffectBlink(0f, 0.07f, null);
		}

		public void Damage(Hit hit)
		{
			if (this.GuardHit(hit))
			{
				return;
			}
			if (this.WillDieByHit(hit))
			{
				hit.HitSoundId = this.finalHit;
			}
			if (!this.DamageArea.enabled)
			{
				this.DamageArea.enabled = true;
				this.DamageArea.ClearAccumDamage();
			}
			this.Behaviour.Damage(hit);
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				Core.Logic.ScreenFreeze.Freeze(0.05f, 4f, 0f, this.slowTimeCurve);
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeBW(0.5f);
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
			}
			else
			{
				this.DamageFlash();
				this.SleepTimeByHit(hit);
			}
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

		public AnimationCurve slowTimeCurve;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit = "event:/SFX/Penitent/Damage/PenitentBossDeathHit";
	}
}
