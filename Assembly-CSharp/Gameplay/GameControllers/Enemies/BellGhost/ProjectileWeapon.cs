using System;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Damage;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.GameControllers.Enemies.BellGhost
{
	public class ProjectileWeapon : Weapon, IDamageable
	{
		public SpriteRenderer SpriteRenderer { get; private set; }

		public AttackArea AttackArea { get; private set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ProjectileWeapon> OnProjectileDeath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ProjectileWeapon> OnProjectileHitsSomething;

		protected override void OnAwake()
		{
			base.OnAwake();
			this.AttackArea = base.GetComponentInChildren<AttackArea>();
			this.SpriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
			this.ProjectileReaction = base.GetComponentInChildren<ProjectileReaction>();
			AttackArea attackArea = this.AttackArea;
			attackArea.enemyLayerMask |= 1 << LayerMask.NameToLayer("ProjectileBarrier");
			if (this.explosion != null)
			{
				PoolManager.Instance.CreatePool(this.explosion, 1);
			}
			if (this.hasAdditionalExplosions)
			{
				this.additionalExplosions.ForEach(delegate(GameObject x)
				{
					PoolManager.Instance.CreatePool(x, 1);
				});
			}
			if (this.onHitEffect != null)
			{
				PoolManager.Instance.CreatePool(this.onHitEffect, this.numberOfHits);
			}
			if (this.onDamageEffect != null)
			{
				PoolManager.Instance.CreatePool(this.onDamageEffect, 1);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.AttackArea.OnEnter += this.AttackAreaOnEnter;
			if (this.multiHit)
			{
				this.AttackArea.OnStay += this.AttackArea_OnStay;
			}
			if (this.ProjectileReaction)
			{
				this.ProjectileReaction.OnProjectileHit += this.OnProjectileHit;
			}
		}

		private bool IsInLayermask(int layer, LayerMask layermask)
		{
			return layermask == (layermask | 1 << layer);
		}

		public void ForceDestroy()
		{
			this.BulletDestruction();
		}

		private void OnProjectileHit(ProjectileReaction obj)
		{
			if ((!obj.IsPhysical && obj.DestructorHit) || obj.DestroyedByNormalHits)
			{
				this.BulletDestruction();
			}
		}

		private void OnLifeEnded(Projectile obj)
		{
			this.BulletDestruction();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.SpriteRenderer.isVisible)
			{
				this.PlayFlightFx();
			}
			else
			{
				this.DisposeFxFlightEvent();
			}
			if (this._cdCounter > 0f)
			{
				this._cdCounter -= Time.deltaTime;
			}
			this.UpdateEvent(ref this._ghostBulletFlightAudioInstance);
		}

		private void CreateOnHitEffect()
		{
			if (this.onHitEffect != null)
			{
				PoolManager.Instance.ReuseObject(this.onHitEffect, base.transform.position, base.transform.rotation, false, 1);
			}
		}

		private void CreateOnDamageEffect()
		{
			if (this.onDamageEffect != null)
			{
				PoolManager.Instance.ReuseObject(this.onDamageEffect, base.transform.position, base.transform.rotation, false, 1);
			}
		}

		private void AttackArea_OnStay(object sender, Collider2DParam e)
		{
			if (this._cdCounter > 0f)
			{
				return;
			}
			GameObject gameObject = e.Collider2DArg.gameObject;
			if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				this._cdCounter = this._hitCooldown;
				this.Attack(this.weaponHit);
				this.CreateOnHitEffect();
				if (!this.IsInLayermask(gameObject.layer, this.pierceLayers))
				{
					this._hitsRemaining--;
					if (this.destroyOnHit && this._hitsRemaining == 0)
					{
						this.BulletDestruction();
					}
				}
			}
		}

		private void AttackAreaOnEnter(object sender, Collider2DParam collider2DParam)
		{
			if (this._destroyed)
			{
				return;
			}
			GameObject gameObject = collider2DParam.Collider2DArg.gameObject;
			this.Attack(this.weaponHit);
			this._cdCounter = this._hitCooldown;
			if (!this.IsInLayermask(gameObject.layer, this.pierceLayers))
			{
				this._hitsRemaining--;
				if (this.destroyOnHit && (!this.multiHit || this._hitsRemaining == 0))
				{
					this.BulletDestruction();
				}
			}
			if (this.OnProjectileHitsSomething != null)
			{
				this.OnProjectileHitsSomething(this);
			}
			this.CreateOnHitEffect();
		}

		private void CreateHit()
		{
			if (!this.AttackingEntity)
			{
				this.AttackingEntity = base.gameObject;
			}
			this.weaponHit = new Hit
			{
				AttackingEntity = this.AttackingEntity.gameObject,
				DamageType = this.damageType,
				DamageAmount = (float)this.damage * this._hitStrength,
				DamageElement = this.damageElement,
				Unblockable = this.unblockable,
				Unnavoidable = this.unavoidable,
				Unparriable = this.unparryable,
				HitSoundId = this.hitSound,
				forceGuardslide = this.forceGuardslide,
				Force = this.force
			};
		}

		public void SetDamage(int dmg)
		{
			this.damage = dmg;
			this.CreateHit();
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		private void BulletDestruction()
		{
			if (this.OnProjectileDeath != null)
			{
				this.OnProjectileDeath(this);
			}
			if (this.OnDeath != null)
			{
				this.OnDeath.Invoke();
			}
			this._destroyed = true;
			this.projectile.OnLifeEndedEvent -= this.OnLifeEnded;
			if (this.explosion)
			{
				PoolManager.Instance.ReuseObject(this.explosion, base.transform.position, Quaternion.identity, false, 1);
				this.PlayExplosionSound();
				if (this.hasAdditionalExplosions)
				{
					this.additionalExplosions.ForEach(delegate(GameObject x)
					{
						PoolManager.Instance.ReuseObject(x, base.transform.position, Quaternion.identity, false, 1);
					});
				}
			}
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Remove(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
			this.StopFlightFx();
			base.Destroy();
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			if (!this.projectile)
			{
				this.projectile = base.GetComponent<Projectile>();
			}
			this.DisposeFxFlightEvent();
			this._hitsRemaining = this.numberOfHits;
			this._destroyed = false;
			this.projectile.OnLifeEndedEvent += this.OnLifeEnded;
			this.projectile.ResetTTL();
			this.CreateHit();
			if (this.SpriteRenderer.IsVisibleFrom(Camera.main) && this.shootSound != string.Empty)
			{
				Core.Audio.PlayOneShot(this.shootSound, default(Vector3));
			}
			if (this.SpriteRenderer.IsVisibleFrom(Camera.main) && this.flightSound != string.Empty)
			{
				this.PlayFlightFx();
			}
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
			if (this.OnSpawn != null)
			{
				this.OnSpawn.Invoke();
			}
		}

		private void OnDamagedGlobal(Penitent damaged, Hit hit)
		{
			if (this.weaponHit.Equals(hit))
			{
				this.CreateOnDamageEffect();
			}
		}

		public void Damage(Hit hit)
		{
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public void SetDamageStrength(float strength)
		{
			this._hitStrength = strength;
			this.CreateHit();
		}

		private void OnDestroy()
		{
			this.DisposeFxFlightEvent();
			if (this.multiHit)
			{
				this.AttackArea.OnStay -= this.AttackArea_OnStay;
			}
			if (this.ProjectileReaction)
			{
				this.ProjectileReaction.OnProjectileHit -= this.OnProjectileHit;
			}
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Remove(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public void SetOwner(GameObject enemy)
		{
			this.AttackingEntity = enemy;
		}

		private void PlayFlightFx()
		{
			if (!this.SpriteRenderer.isVisible)
			{
				return;
			}
			if (this._ghostBulletFlightAudioInstance.isValid() || this.flightSound == string.Empty)
			{
				return;
			}
			this._ghostBulletFlightAudioInstance = Core.Audio.CreateEvent(this.flightSound, default(Vector3));
			this._ghostBulletFlightAudioInstance.start();
		}

		private void StopFlightFx()
		{
			if (!this._ghostBulletFlightAudioInstance.isValid())
			{
				return;
			}
			ParameterInstance parameterInstance;
			if (this._ghostBulletFlightAudioInstance.getParameter("End", ref parameterInstance) == null)
			{
				parameterInstance.setValue(1f);
			}
			else
			{
				this.DisposeFxFlightEvent();
			}
		}

		private void UpdateEvent(ref EventInstance eventInstance)
		{
			if (!this.SpriteRenderer.isVisible)
			{
				return;
			}
			if (eventInstance.isValid())
			{
				this.SetPanning(eventInstance);
			}
		}

		private void DisposeFxFlightEvent()
		{
			if (!this._ghostBulletFlightAudioInstance.isValid())
			{
				return;
			}
			this._ghostBulletFlightAudioInstance.stop(0);
			this._ghostBulletFlightAudioInstance.release();
			this._ghostBulletFlightAudioInstance = default(EventInstance);
		}

		private void PlayExplosionSound()
		{
			if (string.IsNullOrEmpty(this.explosionSound))
			{
				return;
			}
			if (this.SpriteRenderer.IsVisibleFrom(Camera.main))
			{
				Core.Audio.EventOneShotPanned(this.explosionSound, base.transform.position);
			}
		}

		private EVENT_CALLBACK SetPanning(EventInstance e)
		{
			ParameterInstance parameterInstance;
			e.getParameter("Panning", ref parameterInstance);
			if (parameterInstance.isValid())
			{
				float panningValueByPosition = FMODAudioManager.GetPanningValueByPosition(base.transform.position);
				parameterInstance.setValue(panningValueByPosition);
			}
			return null;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		private float _cdCounter;

		private bool _destroyed;

		private readonly float _hitCooldown = 0.1f;

		private int _hitsRemaining;

		private float _hitStrength = 1f;

		private bool poolsCreated;

		[FoldoutGroup("References", 0)]
		public GameObject AttackingEntity;

		[FoldoutGroup("Design settings", 0)]
		public int damage = 10;

		[FoldoutGroup("Design settings", 0)]
		public DamageArea.DamageElement damageElement;

		[FoldoutGroup("Design settings", 0)]
		public DamageArea.DamageType damageType;

		[FoldoutGroup("Design settings", 0)]
		public bool destroyOnHit = true;

		[FoldoutGroup("Design settings", 0)]
		public bool unparryable;

		[FoldoutGroup("Design settings", 0)]
		public bool forceGuardslide;

		[FoldoutGroup("Design settings", 0)]
		public float force;

		[FoldoutGroup("Design settings", 0)]
		public ProjectileReaction ProjectileReaction;

		[FoldoutGroup("Design settings", 0)]
		public GameObject explosion;

		[FoldoutGroup("Design settings", 0)]
		public bool hasAdditionalExplosions;

		[FoldoutGroup("Design settings", 0)]
		[ShowIf("hasAdditionalExplosions", true)]
		public List<GameObject> additionalExplosions = new List<GameObject>();

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string explosionSound;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string flightSound;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string hitSound;

		[FoldoutGroup("Design settings", 0)]
		public bool multiHit;

		[FoldoutGroup("Design settings", 0)]
		[ShowIf("destroyOnHit", true)]
		public int numberOfHits = 1;

		[FoldoutGroup("Design settings", 0)]
		[ShowIf("destroyOnHit", true)]
		public LayerMask pierceLayers;

		[FoldoutGroup("Design settings", 0)]
		public GameObject onHitEffect;

		[FoldoutGroup("Design settings", 0)]
		public GameObject onDamageEffect;

		[FoldoutGroup("References", 0)]
		public Projectile projectile;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string shootSound;

		[FoldoutGroup("Design settings", 0)]
		public bool unavoidable = true;

		[FoldoutGroup("Design settings", 0)]
		public bool unblockable;

		private Hit weaponHit;

		public UnityEvent OnSpawn;

		public UnityEvent OnDeath;

		private const string PROJECTILE_BARRIER_LAYER = "ProjectileBarrier";

		private EventInstance _ghostBulletFlightAudioInstance;

		private const string LabelPanning = "Panning";
	}
}
