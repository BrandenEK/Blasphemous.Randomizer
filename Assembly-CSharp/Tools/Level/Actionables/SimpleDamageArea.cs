using System;
using FMODUnity;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class SimpleDamageArea : Weapon
	{
		public AttackArea attackArea { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._damageEntityDummy = new GameObject("DamageDummy");
			this._damageEntityDummy.transform.SetParent(base.transform);
			this._damageEntityDummy.AddComponent<AreaAttackDummyEntity>();
		}

		public AreaAttackDummyEntity GetDummyEntity()
		{
			return this._damageEntityDummy.GetComponent<AreaAttackDummyEntity>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.CreateHit();
			this.attackArea = base.GetComponentInChildren<AttackArea>();
			this.attackArea.OnEnter += this.AttackArea_OnEnter;
			this.attackArea.OnStay += this.AttackArea_OnStay;
		}

		private void CreateHit()
		{
			this._areaAttackHit = new Hit
			{
				DamageAmount = this.damage,
				DamageElement = this.damageElement,
				DamageType = this.damageType,
				Force = this.force,
				Unnavoidable = this.unavoidable,
				AttackingEntity = this._damageEntityDummy,
				HitSoundId = this.SoundHitFx,
				DestroysProjectiles = this.DestroyProjectile
			};
		}

		public float Damage
		{
			set
			{
				this.damage = value;
				this.CreateHit();
			}
		}

		private void AttackArea_OnEnter(object sender, Collider2DParam e)
		{
			Vector2 zero = Vector2.zero;
			if (this.horizontalDamage)
			{
				zero..ctor(base.transform.position.x, e.Collider2DArg.gameObject.transform.position.y);
			}
			else
			{
				zero..ctor(e.Collider2DArg.gameObject.transform.position.x, base.transform.position.y);
			}
			this._damageEntityDummy.transform.position = zero;
			this.ResetTickCounter();
			this.Attack(this._areaAttackHit);
		}

		private void AttackArea_OnStay(object sender, Collider2DParam e)
		{
			if (this._tickCounter > 0f)
			{
				return;
			}
			Vector2 zero = Vector2.zero;
			if (this.horizontalDamage)
			{
				zero..ctor(base.transform.position.x, e.Collider2DArg.gameObject.transform.position.y);
			}
			else
			{
				zero..ctor(e.Collider2DArg.gameObject.transform.position.x, base.transform.position.y);
			}
			this._damageEntityDummy.transform.position = zero;
			this.ResetTickCounter();
			this.Attack(this._areaAttackHit);
		}

		private void UpdateTick()
		{
			if (this.secondsBetweenTick > 0f)
			{
				this.secondsBetweenTick -= Time.deltaTime;
			}
		}

		private void ResetTickCounter()
		{
			this._tickCounter = this.secondsBetweenTick;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateTick();
		}

		private void OnDestroy()
		{
			if (this.attackArea)
			{
				this.attackArea.OnStay -= this.AttackArea_OnStay;
			}
			if (this.attackArea)
			{
				this.attackArea.OnEnter -= this.AttackArea_OnEnter;
			}
		}

		public override void Attack(Hit weapondHit)
		{
			base.GetDamageableEntities();
			base.AttackDamageableEntities(weapondHit);
		}

		public override void OnHit(Hit weaponHit)
		{
		}

		public bool horizontalDamage = true;

		[FoldoutGroup("Attack config", 0)]
		[EventRef]
		public string SoundHitFx;

		[FoldoutGroup("Attack config", 0)]
		public float damage;

		[FoldoutGroup("Attack config", 0)]
		public DamageArea.DamageType damageType;

		[FoldoutGroup("Attack config", 0)]
		public DamageArea.DamageElement damageElement;

		[FoldoutGroup("Attack config", 0)]
		public bool unavoidable;

		[FoldoutGroup("Attack config", 0)]
		public float force;

		[FoldoutGroup("Attack config", 0)]
		public bool DestroyProjectile;

		private Hit _areaAttackHit;

		private GameObject _damageEntityDummy;

		private const string PENITENT_TAG = "Penitent";

		public float secondsBetweenTick = 0.2f;

		private float _tickCounter;
	}
}
