using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.ElderBrother
{
	public class ElderBrother : Enemy, IDamageable
	{
		public ElderBrotherBehaviour Behaviour { get; set; }

		public NPCInputs Input { get; set; }

		public SmartPlatformCollider Collider { get; set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public ElderBrotherAnimatorInyector AnimatorInyector { get; set; }

		public EnemyAttack Attack { get; set; }

		public DamageEffectScript DamageEffect { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<ElderBrotherBehaviour>();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Collider = base.GetComponent<SmartPlatformCollider>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Attack = base.GetComponentInChildren<EnemyAttack>();
			this.AnimatorInyector = base.GetComponentInChildren<ElderBrotherAnimatorInyector>();
			this.DamageEffect = base.GetComponentInChildren<DamageEffectScript>();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
				base.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
			base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead || Core.Logic.Penitent.Stats.Life.Current <= 0f)
			{
				return;
			}
			if (this.WillDieByHit(hit))
			{
				hit.HitSoundId = this.finalHit;
			}
			this.Behaviour.Damage();
			this.DamageArea.TakeDamage(hit, false);
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				Core.Logic.ScreenFreeze.Freeze(0.01f, 2f, 0f, this.slowTimeCurve);
				if (this.showBossDeathEffect)
				{
					Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeBW(1.5f);
				}
				this.Behaviour.Death();
			}
			else
			{
				this.DamageEffect.Blink(0f, 0.15f);
				this.SleepTimeByHit(hit);
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
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

		public AnimationCurve slowTimeCurve;

		public bool showBossDeathEffect = true;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit;
	}
}
