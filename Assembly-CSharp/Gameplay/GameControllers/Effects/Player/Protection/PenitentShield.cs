using System;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent;
using Plugins.GhostSprites2D.Scripts.GhostSprites;
using Sirenix.OdinInspector;

namespace Gameplay.GameControllers.Effects.Player.Protection
{
	public class PenitentShield : Weapon
	{
		public AttackArea AttackArea { get; private set; }

		public GhostSprites GhostSprites { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponent<AttackArea>();
			this.GhostSprites = base.GetComponent<GhostSprites>();
			this.shieldEffects = base.GetComponent<MasterShaderEffects>();
			this._shieldHit = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageElement = this.DamageElement,
				DamageType = this.DamageType,
				HitSoundId = this.HitSoundFx,
				DestroysProjectiles = true
			};
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.OnEnter += this.OnEnterAttackArea;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.shieldEffects)
			{
				this.shieldEffects.ColorizeWave(1.5f);
			}
		}

		private void OnEnterAttackArea(object sender, Collider2DParam e)
		{
			if (this.shieldEffects)
			{
				this.shieldEffects.TriggerColorFlash();
			}
			this.Attack(this._shieldHit);
		}

		public void SmallDistortion()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.2f, 0.1f, 0.5f);
		}

		private void OnEnable()
		{
			this.GhostSprites.EnableGhostTrail = true;
		}

		private void OnDisable()
		{
			this.GhostSprites.EnableGhostTrail = false;
		}

		private void OnDestroy()
		{
			this.AttackArea.OnEnter -= this.OnEnterAttackArea;
		}

		public override void Attack(Hit weapondHit)
		{
			weapondHit.DamageAmount = this.Strength * Core.Logic.Penitent.Stats.PrayerStrengthMultiplier.Final;
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		private Hit _shieldHit;

		[FoldoutGroup("Hit Settings", 0)]
		public float Strength;

		[FoldoutGroup("Hit Settings", 0)]
		public DamageArea.DamageElement DamageElement;

		[FoldoutGroup("Hit Settings", 0)]
		public DamageArea.DamageType DamageType;

		[FoldoutGroup("Audio", 0)]
		[EventRef]
		public string HitSoundFx;

		private MasterShaderEffects shieldEffects;
	}
}
