using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity.BlobShadow;
using Gameplay.GameControllers.Entities.Animations;
using I2.Loc;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[SelectionBase]
	public class Entity : MonoBehaviour
	{
		[Button(ButtonSizes.Small)]
		public void KillEntity()
		{
			Debug.Log("TRYING TO KILL ENTITY");
			this.Kill();
		}

		public event Core.SimpleEvent OnDamaged;

		public event Action<float> OnDamageTaken;

		public event Core.SimpleEvent OnDeath;

		public event Core.SimpleEvent OnDestroyEntity;

		public event Core.EntityEvent OnEntityDeath;

		public event Entity.EntityFlagEvent FlagChanged;

		public static event Core.EntityEvent Started;

		public static event Core.EntityEvent Death;

		public static event Core.EntityEvent Damaged;

		public Animator Animator { get; private set; }

		public SpriteRenderer SpriteRenderer { get; private set; }

		public bool Dead
		{
			get
			{
				return this.Stats.Life.Current <= 0f;
			}
		}

		public bool IsImpaled { get; set; }

		public bool Mute { get; set; }

		public static Entity LastSpawned { get; private set; }

		public static Entity[] LivingEntities
		{
			get
			{
				return Entity.livingEntities.ToArray();
			}
		}

		public float CurrentLife
		{
			get
			{
				return this.Stats.Life.Current;
			}
			set
			{
				this.Stats.Life.Current = value;
			}
		}

		public float CurrentFervour
		{
			get
			{
				return this.Stats.Fervour.Current;
			}
			set
			{
				this.Stats.Fervour.Current = value;
			}
		}

		public float CurrentCriticalChance
		{
			get
			{
				return this.Stats.CriticalChance.Final;
			}
			set
			{
				this.Stats.CriticalChance.Bonus = value;
			}
		}

		private void Awake()
		{
			this.Animator = base.GetComponentInChildren<Animator>();
			if (this.Animator != null)
			{
				this.SpriteRenderer = this.Animator.GetComponent<SpriteRenderer>();
			}
			this.entityAttack = base.GetComponentInChildren<Attack>();
			this.entityDamageArea = base.GetComponentInChildren<DamageArea>();
			this.EntityAnimationEvents = base.GetComponentInChildren<EntityAnimationEvents>();
			this.OnAwake();
			this.Stats.Initialize();
			Entity.livingEntities.Add(this);
			Entity.LastSpawned = this;
			if (Entity.OnCreated != null)
			{
				Entity.OnCreated(base.gameObject);
			}
		}

		protected virtual void OnDestroy()
		{
			Entity.livingEntities.Remove(this);
			if (this.OnDestroyEntity != null)
			{
				this.OnDestroyEntity();
			}
		}

		public EntityAnimationEvents EntityAnimationEvents { get; set; }

		public BlobShadow Shadow { get; set; }

		public int CurrentOutputDamage { get; set; }

		public float SlopeAngle { get; set; }

		public Attack EntityAttack
		{
			get
			{
				return this.entityAttack;
			}
		}

		public DamageArea EntityDamageArea
		{
			get
			{
				return this.entityDamageArea;
			}
		}

		private void Start()
		{
			this.OnStart();
			if (Entity.Started != null)
			{
				Entity.Started(this);
			}
		}

		private void Update()
		{
			this.OnUpdate();
		}

		private void FixedUpdate()
		{
			this.OnFixedUpdated();
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnFixedUpdated()
		{
		}

		public virtual void HitDisplacement(Vector3 enemyPos, DamageArea.DamageType damageType)
		{
		}

		public virtual void SleepTimeByHit(Hit hit)
		{
			float hitSleepTime = Core.Logic.Penitent.LevelSleepTime.GetHitSleepTime(hit);
			LevelInitializer currentLevelConfig = Core.Logic.CurrentLevelConfig;
			currentLevelConfig.sleepTime = hitSleepTime;
			if (!currentLevelConfig.IsSleeping)
			{
				currentLevelConfig.SleepTime();
			}
		}

		public virtual bool BleedOnImpact()
		{
			return false;
		}

		public virtual bool SparkOnImpact()
		{
			return false;
		}

		public float GetReducedDamage(Hit hit)
		{
			float num = hit.DamageAmount;
			switch (hit.DamageElement)
			{
			case DamageArea.DamageElement.Normal:
				num -= this.Stats.NormalDmgReduction.CalculateValue() * num;
				break;
			case DamageArea.DamageElement.Fire:
				num -= this.Stats.FireDmgReduction.CalculateValue() * num;
				break;
			case DamageArea.DamageElement.Toxic:
				num -= this.Stats.ToxicDmgReduction.CalculateValue() * num;
				break;
			case DamageArea.DamageElement.Magic:
				num -= this.Stats.MagicDmgReduction.CalculateValue() * num;
				break;
			case DamageArea.DamageElement.Lightning:
				num -= this.Stats.LightningDmgReduction.CalculateValue() * num;
				break;
			case DamageArea.DamageElement.Contact:
				num -= this.Stats.ContactDmgReduction.CalculateValue() * num;
				break;
			}
			return num;
		}

		public void Damage(float amount, string impactAudioId = "")
		{
			if (!impactAudioId.IsNullOrWhitespace() && !this.Mute)
			{
				Core.Audio.EventOneShotPanned(impactAudioId, base.transform.position);
				Log.Trace("Audio", "Playing audio hit. " + impactAudioId, null);
			}
			float num = Mathf.Max(amount - this.Stats.Defense.Final, 0f);
			this.Stats.Life.Current -= num;
			if (this.OnDamaged != null)
			{
				this.OnDamaged();
			}
			if (this.OnDamageTaken != null)
			{
				this.OnDamageTaken(num);
			}
			if (this.Stats.Life.Current <= 0f)
			{
				this.KillInstanteneously();
			}
			if (Entity.Damaged != null)
			{
				Entity.Damaged(this);
			}
		}

		public void SetHealth(float health)
		{
			this.Stats.Life.Current = health;
			if (this.Stats.Life.Current <= 0f)
			{
				this.KillInstanteneously();
			}
		}

		public virtual void Revive()
		{
			this.SetHealth(this.Stats.Life.MaxValue);
			this.Status.Dead = false;
		}

		public virtual void Kill()
		{
			this.Damage(this.Stats.Life.Current + this.Stats.Defense.Final, string.Empty);
		}

		public void KillInstanteneously()
		{
			this.Status.Dead = true;
			this.Stats.Life.Current = 0f;
			if (Entity.Death != null)
			{
				Entity.Death(this);
			}
			if (this.OnDeath != null)
			{
				this.OnDeath();
			}
			if (this.OnEntityDeath != null)
			{
				this.OnEntityDeath(this);
			}
			Core.Events.LaunchEvent(this.GenericEntityDead, string.Empty);
			Core.Events.LaunchEvent(this.OnDead, string.Empty);
		}

		public virtual void SetOrientation(EntityOrientation orientation, bool allowFlipRenderer = true, bool searchForRenderer = false)
		{
			this.Status.Orientation = orientation;
			if (allowFlipRenderer)
			{
				if (!this.spriteRenderer && searchForRenderer)
				{
					this.spriteRenderer = base.GetComponentInChildren<SpriteRenderer>();
				}
				if (!this.spriteRenderer)
				{
					Debug.LogWarning("No sprite renderer available. The entity cannot flip.");
					return;
				}
				EntityOrientation orientation2 = this.Status.Orientation;
				if (orientation2 != EntityOrientation.Left)
				{
					if (orientation2 != EntityOrientation.Right)
					{
						throw new ArgumentOutOfRangeException();
					}
					this.spriteRenderer.flipX = false;
					return;
				}
				else
				{
					this.spriteRenderer.flipX = true;
				}
			}
		}

		public IEnumerator FreezeAnimator(float freezeTime)
		{
			float deltaFreezeTime = 0f;
			this.Animator.speed = 0f;
			while (deltaFreezeTime <= freezeTime)
			{
				deltaFreezeTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			this.Animator.speed = 1f;
			yield break;
		}

		public EntityOrientation SetOrientationbyHit(Vector3 enemyPos)
		{
			float x = base.transform.position.x;
			EntityOrientation entityOrientation = this.Status.Orientation;
			this.OrientationBeforeHit = this.Status.Orientation;
			if (x > enemyPos.x)
			{
				entityOrientation = EntityOrientation.Left;
			}
			else if (x <= enemyPos.x)
			{
				entityOrientation = EntityOrientation.Right;
			}
			this.SetOrientation(entityOrientation, true, false);
			return entityOrientation;
		}

		public Vector3 GetForwardTangent(Vector3 dir, Vector3 up)
		{
			return Vector3.Cross(Vector3.Cross(up, dir), up);
		}

		protected static bool IsVisibleFrom(Renderer entityRenderer, Camera mainCamera)
		{
			return entityRenderer.isVisible;
		}

		public void SetFlag(string flag, bool active)
		{
			if (active && !this.HasFlag(flag))
			{
				this.flags.Add(flag);
				if (this.FlagChanged != null)
				{
					this.FlagChanged(flag, true);
					return;
				}
			}
			else if (!active && this.HasFlag(flag))
			{
				this.flags.Remove(flag);
				if (this.FlagChanged != null)
				{
					this.FlagChanged(flag, false);
				}
			}
		}

		public bool HasFlag(string key)
		{
			return this.flags.Contains(key);
		}

		public static Core.ObjectEvent OnCreated;

		private static readonly List<Entity> livingEntities = new List<Entity>();

		private readonly List<string> flags = new List<string>();

		private bool invulnerable;

		[TitleGroup("Entity Id", "A code which uniquely identifies an entity the documentation.", TitleAlignments.Left, true, true, false, 0)]
		public string Id;

		[TitleGroup("Entity Name", "The name of the entity in the documentation.", TitleAlignments.Left, true, true, false, 0)]
		public string EntityName;

		[SerializeField]
		public LocalizedString displayName;

		[SerializeField]
		[BoxGroup("Events", true, false, 0)]
		[ReadOnly]
		private string GenericEntityDead = "ENTITY_DEAD";

		[SerializeField]
		[BoxGroup("Events", true, false, 0)]
		private string OnDead;

		public EntityStats Stats = new EntityStats();

		public EntityStatus Status = new EntityStatus();

		public bool IsExecutable;

		public EntityOrientation OrientationBeforeHit;

		private float paralysisTimeCount;

		private Attack entityAttack;

		private DamageArea entityDamageArea;

		[SerializeField]
		protected SpriteRenderer spriteRenderer;

		[HideInInspector]
		public EntityStates entityCurrentState;

		public delegate void EntityFlagEvent(string key, bool active);
	}
}
