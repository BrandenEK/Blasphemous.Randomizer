using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PontiffSword.Animator;
using Gameplay.GameControllers.Bosses.PontiffSword.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffSword
{
	public class PontiffSword : Enemy, IDamageable
	{
		public PontiffSwordBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public NPCInputs Input { get; set; }

		public PontiffSwordAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.animatorInyector = base.GetComponentInChildren<PontiffSwordAnimatorInyector>();
			this.Behaviour = base.GetComponent<PontiffSwordBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Audio = base.GetComponentInChildren<PontiffSwordAudio>();
			this.Input = base.GetComponentInChildren<NPCInputs>();
			this.DamageArea.DamageAreaCollider.enabled = false;
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
			this.damageEffectScript.Blink(0f, 0.07f);
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
				return;
			}
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
			}
			else
			{
				this.Behaviour.Damage();
				this.SleepTimeByHit(hit);
			}
		}

		public override void Parry()
		{
			base.Parry();
			this.Behaviour.Parry();
		}

		public override void Revive()
		{
			base.Revive();
			this.Behaviour.Revive();
			this.DamageArea.DamageAreaCollider.enabled = true;
		}

		public Vector3 GetPosition()
		{
			throw new NotImplementedException();
		}

		public AnimationCurve slowTimeCurve;

		public DamageEffectScript damageEffectScript;

		public PontiffSwordAnimatorInyector animatorInyector;
	}
}
