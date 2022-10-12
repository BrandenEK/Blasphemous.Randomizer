using System;
using CreativeSpore.SmartColliders;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Amanecidas.Audio;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Amanecidas
{
	public class Amanecidas : Enemy, IDamageable
	{
		public AmanecidasBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public NPCInputs Input { get; set; }

		public MasterShaderEffects DamageEffect { get; private set; }

		public GhostTrailGenerator GhostTrail { get; private set; }

		public AmanecidasAnimatorInyector AnimatorInyector { get; private set; }

		public LaudesArena LaudesArena { get; private set; }

		public AmanecidasAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Behaviour = base.GetComponent<AmanecidasBehaviour>();
			this.Audio = base.GetComponent<AmanecidasAudio>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Input = base.GetComponentInChildren<NPCInputs>();
			this.GhostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this.DamageEffect = base.GetComponentInChildren<MasterShaderEffects>();
			this.AnimatorInyector = base.GetComponentInChildren<AmanecidasAnimatorInyector>();
			UnityEngine.Object.Instantiate<GameObject>(this.shieldPrefab, this.shieldParent);
			this.shield = base.GetComponentInChildren<AmanecidaShield>();
			this.shield.amanecidas = this;
			this.ActivateShield();
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
						this.AnimatorInyector.Flip(false);
					}
				}
				else
				{
					this.AnimatorInyector.Flip(true);
				}
			}
		}

		public void DamageFlash()
		{
			this.DamageEffect.DamageEffectBlink(0f, 0.07f, null);
		}

		private void TakeShieldDamage(Hit hit)
		{
			if (this.shieldCurrentHP <= 0f)
			{
				return;
			}
			Debug.Log("SHIELD takes " + hit.DamageAmount + " damage");
			if (!this.AnimatorInyector.IsOut())
			{
				this.shield.FlashShieldFromPenitent();
			}
			float num = this.shieldCurrentHP / this.shieldMaxHP;
			this.shield.SetDamagePercentage(num);
			this.shieldCurrentHP -= hit.DamageAmount;
			this.SleepTimeByHit(hit);
			if (this.shieldCurrentHP <= 0f)
			{
				this.shieldCurrentHP = 0f;
				this.BreakShield();
				if (this.IsLaudes && this.Behaviour.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW)
				{
					this.Behaviour.MoveBattleBoundsIfNeeded();
				}
				this.Audio.PlayShieldDestroy_AUDIO();
			}
			else
			{
				this.Audio.PlayShield_AUDIO(num);
			}
		}

		public void ActivateShield()
		{
			base.IsGuarding = true;
			this.shieldCurrentHP = this.shieldMaxHP;
			this.shield.FlashShieldFromDown();
		}

		public void ForceBreakShield()
		{
			Hit hit = new Hit
			{
				DamageAmount = this.shieldCurrentHP,
				DamageType = Gameplay.GameControllers.Entities.DamageArea.DamageType.Heavy
			};
			this.TakeShieldDamage(hit);
		}

		private void BreakShield()
		{
			Debug.Log("SHIELD BROKEN!!");
			this.shield.BreakShield();
			base.IsGuarding = false;
		}

		public void SetLaudesArena(LaudesArena arena, Vector2 origin, bool onlySetBoundaries)
		{
			this.LaudesArena = arena;
			this.LaudesArena.SetLaudesArena(this, origin, onlySetBoundaries);
		}

		public void SetNextLaudesArena(LaudesArena arena = null)
		{
			if (arena != null)
			{
				this.LaudesArena = arena;
			}
			if (this.LaudesArena == null)
			{
				Debug.LogError("No LaudesArena!");
			}
			else
			{
				this.LaudesArena.SetNextArena(this);
			}
		}

		public void SetupFight(AmanecidasFightSpawner.AMANECIDAS_FIGHTS fightType)
		{
			switch (fightType)
			{
			case AmanecidasFightSpawner.AMANECIDAS_FIGHTS.LANCE:
				this.Behaviour.SetWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.LANCE);
				this.AnimatorInyector.SetAmanecidaColor(AmanecidasAnimatorInyector.AMANECIDA_COLOR.SKYBLUE);
				break;
			case AmanecidasFightSpawner.AMANECIDAS_FIGHTS.AXE:
				this.Behaviour.SetWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE);
				this.AnimatorInyector.SetAmanecidaColor(AmanecidasAnimatorInyector.AMANECIDA_COLOR.RED);
				break;
			case AmanecidasFightSpawner.AMANECIDAS_FIGHTS.FALCATA:
				this.Behaviour.SetWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD);
				this.AnimatorInyector.SetAmanecidaColor(AmanecidasAnimatorInyector.AMANECIDA_COLOR.WHITE);
				break;
			case AmanecidasFightSpawner.AMANECIDAS_FIGHTS.BOW:
				this.Behaviour.SetWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW);
				this.AnimatorInyector.SetAmanecidaColor(AmanecidasAnimatorInyector.AMANECIDA_COLOR.BLUE);
				break;
			case AmanecidasFightSpawner.AMANECIDAS_FIGHTS.LAUDES:
				this.AnimatorInyector.SetAmanecidaColor(AmanecidasAnimatorInyector.AMANECIDA_COLOR.LAUDES);
				this.IsLaudes = true;
				this.Behaviour.SetWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD);
				break;
			}
		}

		public override void Parry()
		{
			base.Parry();
			this.Behaviour.Parry();
		}

		public void Damage(Hit hit)
		{
			if (this.Behaviour.DodgeHit(hit) || Core.Logic.Penitent.Stats.Life.Current <= 0f || this.Status.Dead)
			{
				return;
			}
			if (this.GuardHit(hit))
			{
				this.SleepTimeByHit(hit);
				this.Behaviour.ShieldDamage(hit);
				this.TakeShieldDamage(hit);
				return;
			}
			this.DamageFlash();
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
				this.shield.Death();
				this.Behaviour.Death();
			}
			else
			{
				this.DamageFlash();
				this.Behaviour.Damage(hit);
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
		private string finalHit;

		public GameObject shieldPrefab;

		public Transform shieldParent;

		public float shieldMaxHP;

		public float shieldCurrentHP;

		public AmanecidaShield shield;

		public bool IsLaudes;
	}
}
