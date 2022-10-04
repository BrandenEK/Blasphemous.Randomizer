using System;
using CreativeSpore.SmartColliders;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Isidora.Audio;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class Isidora : Enemy, IDamageable, IPaintDamageableCollider
	{
		public IsidoraBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public NPCInputs Input { get; set; }

		public MasterShaderEffects DamageEffect { get; private set; }

		public GhostTrailGenerator GhostTrail { get; private set; }

		public IsidoraAnimatorInyector AnimatorInyector { get; private set; }

		public IsidoraAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Behaviour = base.GetComponent<IsidoraBehaviour>();
			this.AnimatorInyector = base.GetComponentInChildren<IsidoraAnimatorInyector>();
			this.Audio = base.GetComponent<IsidoraAudio>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Input = base.GetComponentInChildren<NPCInputs>();
			this.GhostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this.DamageEffect = base.GetComponentInChildren<MasterShaderEffects>();
			this.AttachShowScriptIfNeeded();
		}

		public override void SetOrientation(EntityOrientation orientation, bool allowFlipRenderer = true, bool searchForRenderer = false)
		{
			this.Status.Orientation = orientation;
			if (allowFlipRenderer)
			{
				EntityOrientation orientation2 = this.Status.Orientation;
				if (orientation2 != EntityOrientation.Left)
				{
					if (orientation2 == EntityOrientation.Right)
					{
						this.spriteRenderer.flipX = false;
						this.spriteRendererVfx.flipX = false;
					}
				}
				else
				{
					this.spriteRenderer.flipX = true;
					this.spriteRendererVfx.flipX = true;
				}
			}
		}

		public void DamageFlash()
		{
			this.DamageEffect.DamageEffectBlink(0f, 0.07f, null);
		}

		public override void Parry()
		{
			base.Parry();
			this.Behaviour.Parry();
		}

		public void Damage(Hit hit)
		{
			if (this.GuardHit(hit))
			{
				this.SleepTimeByHit(hit);
				return;
			}
			if (Core.Logic.Penitent.Stats.Life.Current <= 0f)
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
				Core.Logic.ScreenFreeze.Freeze(0.05f, 4f, 0f, this.slowTimeCurve);
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeBW(0.5f);
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
			}
			else
			{
				this.DamageFlash();
				this.Behaviour.Damage(hit);
				this.SleepTimeByHit(hit);
			}
		}

		public bool IsCurrentlyDamageable()
		{
			return this.DamageArea.DamageAreaCollider.enabled;
		}

		public void AttachShowScriptIfNeeded()
		{
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

		internal float GetLifePercentage()
		{
			throw new NotImplementedException();
		}

		public AnimationCurve slowTimeCurve;

		public SpriteRenderer spriteRendererVfx;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string finalHit;
	}
}
