using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras.Animator;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras.Audio;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras
{
	public class Esdras : Enemy, IDamageable
	{
		public EsdrasBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public NPCInputs Input { get; set; }

		public EsdrasAnimatorInyector AnimatorInyector { get; private set; }

		public EsdrasAudio Audio { get; private set; }

		public DamageEffectScript damageEffect { get; private set; }

		public GhostTrailGenerator GhostTrail { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Behaviour = base.GetComponent<EsdrasBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<EsdrasAnimatorInyector>();
			this.Audio = base.GetComponentInChildren<EsdrasAudio>();
			this.damageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.Input = base.GetComponentInChildren<NPCInputs>();
			this.GhostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.SetOrientation((!this.lookLeftOnStart) ? EntityOrientation.Right : EntityOrientation.Left, true, false);
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			throw new NotImplementedException();
		}

		public override EnemyBumper EnemyBumper()
		{
			throw new NotImplementedException();
		}

		protected override void EnablePhysics(bool enable = true)
		{
			throw new NotImplementedException();
		}

		public void DamageFlash()
		{
			this.damageEffect.Blink(0f, 0.07f);
		}

		public void CounterFlash()
		{
			this.damageEffect.Blink(0f, 0.15f);
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead || Core.Logic.Penitent.Stats.Life.Current <= 0f)
			{
				return;
			}
			this.DamageFlash();
			if (this.GuardHit(hit))
			{
				this.SleepTimeByHit(hit);
				return;
			}
			if (this.WillDieByHit(hit))
			{
				hit.HitSoundId = this.finalHit;
			}
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
				this.Behaviour.Damage();
				this.SleepTimeByHit(hit);
			}
		}

		public void PerpetuaSummonSlowmo()
		{
			Core.Logic.ScreenFreeze.Freeze(0.1f, 1f, 0f, this.slowTimeCurve);
		}

		public override void Parry()
		{
			base.Parry();
			this.Behaviour.Parry();
		}

		public Vector3 GetPosition()
		{
			throw new NotImplementedException();
		}

		public AnimationCurve slowTimeCurve;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit;

		public bool lookLeftOnStart = true;
	}
}
