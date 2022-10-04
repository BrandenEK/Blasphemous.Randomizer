using System;
using Gameplay.GameControllers.Bosses.PietyMonster.IA;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Attack
{
	public class PietyStompAttack : EnemyAttack
	{
		public Vector2 DefaultDamageAreaSize { get; private set; }

		public Vector2 DefaultDamageAreaOffset { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._pietyMonster = (PietyMonster)base.EntityOwner;
			base.CurrentEnemyWeapon = this.PietyHoof;
		}

		protected override void OnStart()
		{
			base.OnStart();
			BoxCollider2D boxCollider2D = (BoxCollider2D)this._pietyMonster.DamageArea.DamageAreaCollider;
			this.DefaultDamageAreaSize = new Vector2(boxCollider2D.size.x, boxCollider2D.size.y);
			this.DefaultDamageAreaOffset = new Vector2(boxCollider2D.offset.x, boxCollider2D.offset.y);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.CheckTargetOnRange();
		}

		private void CheckTargetOnRange()
		{
			if (this.PietyMonsterBehaviour.TargetOnRange)
			{
				this._remainSafeTimeOnStompAttackArea -= Time.deltaTime;
				if (this._remainSafeTimeOnStompAttackArea > 0f)
				{
					return;
				}
				this._remainSafeTimeOnStompAttackArea = this.SafeTimeOnStompAttackArea;
				if (!this.PietyMonsterBehaviour.ReadyToAttack)
				{
					this.PietyMonsterBehaviour.ReadyToAttack = true;
				}
			}
			else
			{
				this.PietyMonsterBehaviour.ReadyToAttack = false;
				if (this.PietyMonsterBehaviour.Attacking)
				{
					this.PietyMonsterBehaviour.Attacking = false;
				}
				this._remainSafeTimeOnStompAttackArea = this.SafeTimeOnStompAttackArea;
			}
		}

		public override void CurrentWeaponAttack()
		{
			base.CurrentWeaponAttack();
			if (base.CurrentEnemyWeapon == null || this._pietyMonster.Status.Dead)
			{
				return;
			}
			Hit weapondHit = new Hit
			{
				AttackingEntity = this._pietyMonster.gameObject,
				DamageType = this.DamageType,
				DamageAmount = this.DamageAmount,
				Force = this.Force,
				HitSoundId = this.HitSound
			};
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		public void SetDamageAreaOffset(Vector2 newDamageAreaOffset)
		{
			this._pietyMonster.DamageArea.DamageAreaCollider.offset = newDamageAreaOffset;
		}

		public void SetDamageAreaSize(Vector2 newDamageAreaSize)
		{
			BoxCollider2D boxCollider2D = (BoxCollider2D)this._pietyMonster.DamageArea.DamageAreaCollider;
			boxCollider2D.size = newDamageAreaSize;
		}

		public PietyMonsterBehaviour PietyMonsterBehaviour;

		public float SafeTimeOnStompAttackArea = 0.5f;

		private float _remainSafeTimeOnStompAttackArea;

		public Weapon PietyHoof;

		public float DamageAmount = 25f;

		private PietyMonster _pietyMonster;

		public Vector2 AttackDamageAreaOffset;

		public Vector2 AttackDamageAreaSize;

		[Range(0f, 2f)]
		public float DamageFactor = 1f;
	}
}
