using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PontiffOldman.Animator;
using Gameplay.GameControllers.Bosses.PontiffOldman.Audio;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffOldman
{
	public class PontiffOldman : Enemy, IDamageable
	{
		public PontiffOldmanBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public NPCInputs Input { get; set; }

		public PontiffOldmanAnimatorInyector AnimatorInyector { get; private set; }

		public PontiffOldmanAudio Audio { get; private set; }

		public DamageEffectScript damageEffect { get; private set; }

		public GhostTrailGenerator GhostTrail { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Behaviour = base.GetComponent<PontiffOldmanBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<PontiffOldmanAnimatorInyector>();
			this.Audio = base.GetComponentInChildren<PontiffOldmanAudio>();
			this.damageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.Input = base.GetComponentInChildren<NPCInputs>();
			this.GhostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
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
			this.damageEffect.Blink(0f, 0.07f);
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead || Core.Logic.Penitent.Stats.Life.Current <= 0f)
			{
				return;
			}
			Debug.Log("<color=blue> DAMAGE REACHED PONTIFF");
			this.DamageFlash();
			if (!hit.Unparriable && this.GuardHit(hit))
			{
				return;
			}
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				Core.Logic.ScreenFreeze.Freeze(0.1f, 2f, 0f, this.slowTimeCurve);
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

		public Vector3 GetPosition()
		{
			throw new NotImplementedException();
		}

		public AnimationCurve slowTimeCurve;
	}
}
