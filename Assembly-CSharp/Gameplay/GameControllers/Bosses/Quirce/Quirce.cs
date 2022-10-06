using System;
using CreativeSpore.SmartColliders;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Animation;
using Gameplay.GameControllers.Bosses.Quirce.Audio;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce
{
	public class Quirce : Enemy, IDamageable
	{
		public QuirceBehaviour Behaviour { get; set; }

		public NPCInputs Input { get; set; }

		public SmartPlatformCollider Collider { get; set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public QuirceAnimatorInyector AnimatorInyector { get; set; }

		public EnemyAttack Attack { get; set; }

		public ColorFlash ColorFlash { get; set; }

		public QuirceAudio Audio { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponent<QuirceBehaviour>();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Collider = base.GetComponent<SmartPlatformCollider>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Attack = base.GetComponentInChildren<EnemyAttack>();
			this.AnimatorInyector = base.GetComponentInChildren<QuirceAnimatorInyector>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.Audio = base.GetComponentInChildren<QuirceAudio>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.CastShadow = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsGrounded = this.Behaviour.motionChecker.HitsFloor;
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y, base.transform.position.z);
			base.transform.position = position;
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead || Core.Logic.Penitent.Stats.Life.Current <= 0f)
			{
				return;
			}
			if (this.GuardHit(hit))
			{
				Debug.Log("GUARDED HIT");
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
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeBW(0.5f);
				Core.Logic.ScreenFreeze.Freeze(0.1f, 3f, 0f, this.slowTimeCurve);
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
				this.Audio.PlayDeath();
			}
			else
			{
				this.SleepTimeByHit(hit);
				this.Behaviour.Damage();
				this.ColorFlash.TriggerColorFlash();
			}
		}

		public void ActivateColliders(bool activate)
		{
			this.DamageArea.DamageAreaCollider.enabled = activate;
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

		public QuirceBossFightPoints BossFightPoints;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit;
	}
}
