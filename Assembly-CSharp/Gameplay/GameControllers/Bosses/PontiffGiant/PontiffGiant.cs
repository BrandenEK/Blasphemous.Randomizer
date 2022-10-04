using System;
using CreativeSpore.SmartColliders;
using DamageEffect;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PontiffGiant.Animator;
using Gameplay.GameControllers.Bosses.PontiffGiant.Audio;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffGiant
{
	public class PontiffGiant : Enemy, IDamageable
	{
		public PontiffGiantBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public NPCInputs Input { get; set; }

		public PontiffGiantAnimatorInyector AnimatorInyector { get; private set; }

		public PontiffGiantAudio Audio { get; private set; }

		public GhostTrailGenerator GhostTrail { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Behaviour = base.GetComponent<PontiffGiantBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<PontiffGiantAnimatorInyector>();
			this.Audio = base.GetComponentInChildren<PontiffGiantAudio>();
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
			this.damageEffectFace.Blink(0f, 0.07f);
		}

		public void Damage(Hit hit)
		{
			if (Core.Logic.Penitent.Stats.Life.Current <= 0f || this.Status.Dead)
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
				Core.Logic.ScreenFreeze.Freeze(0.01f, 2f, 0f, this.slowTimeCurve);
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

		public DamageEffectScript damageEffectFace;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit;
	}
}
