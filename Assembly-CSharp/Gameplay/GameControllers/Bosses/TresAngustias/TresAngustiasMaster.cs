using System;
using DamageEffect;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.TresAngustias.Animator;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.MasterAnguish.Audio;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.TresAngustias
{
	public class TresAngustiasMaster : Enemy, IDamageable
	{
		public TresAngustiasMasterBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public TresAngustiasMasterAnimatorInyector AnimatorInyector { get; private set; }

		public DamageEffectScript damageEffect { get; private set; }

		public MasterAnguishAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Audio = base.GetComponentInChildren<MasterAnguishAudio>();
			this.Behaviour = base.GetComponent<TresAngustiasMasterBehaviour>();
			base.EnemyBehaviour = this.Behaviour;
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<TresAngustiasMasterAnimatorInyector>();
			this.damageEffect = base.GetComponentInChildren<DamageEffectScript>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Behaviour.singleAnguishLance.OnSingleAnguishDamaged += this.OnSingleAnguishDamaged;
			this.Behaviour.singleAnguishMace.OnSingleAnguishDamaged += this.OnSingleAnguishDamaged;
			this.Behaviour.singleAnguishShield.OnSingleAnguishDamaged += this.OnSingleAnguishDamaged;
		}

		private void OnSingleAnguishDamaged(SingleAnguish arg1, Hit arg2)
		{
			this.Damage(arg2);
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
			if (this.damageEffect)
			{
				this.damageEffect.Blink(0f, 0.07f);
			}
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead || this.Invencible || Core.Logic.Penitent.Stats.Life.Current <= 0f)
			{
				return;
			}
			if (this.WillDieByHit(hit))
			{
				hit.HitSoundId = this.finalHit;
			}
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				Core.Logic.ScreenFreeze.Freeze(0.01f, 2f, 0f, this.slowTimeCurve);
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeBW(0.5f);
				this.DamageArea.DamageAreaCollider.enabled = false;
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

		public AnguishBossfightConfig bossfightPoints;

		public AnimationCurve slowTimeCurve;

		public bool Invencible = true;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit;
	}
}
