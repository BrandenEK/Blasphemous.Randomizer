using System;
using DamageEffect;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.BurntFace;
using Gameplay.GameControllers.Bosses.WickerWurm;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BlindBaby
{
	public class WickerWurm : Enemy, IDamageable
	{
		public WickerWurmBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public WickerWurmAnimatorInyector AnimatorInyector { get; private set; }

		public DamageEffectScript damageEffect { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<WickerWurmBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<WickerWurmAnimatorInyector>();
			this.damageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.Audio = base.GetComponentInChildren<WickerWurmAudio>();
		}

		protected override void OnStart()
		{
			base.OnStart();
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
			this.damageEffect.Blink(0f, 0.1f);
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead || Core.Logic.Penitent.Stats.Life.Current <= 0f || this.Behaviour.HasGrabbedPenitent())
			{
				return;
			}
			if (this.WillDieByHit(hit))
			{
				hit.HitSoundId = this.finalHit;
			}
			this.DamageArea.TakeDamage(hit, false);
			this.DamageFlash();
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				Core.Logic.ScreenFreeze.Freeze(0.05f, 4f, 0f, this.slowTimeCurve);
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeBW(0.5f);
				this.Behaviour.Death();
			}
			else
			{
				this.Behaviour.Damage();
				this.SleepTimeByHit(hit);
			}
		}

		public Vector3 GetPosition()
		{
			throw new NotImplementedException();
		}

		public AnimationCurve slowTimeCurve;

		public WickerWurmAudio Audio;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit;
	}
}
