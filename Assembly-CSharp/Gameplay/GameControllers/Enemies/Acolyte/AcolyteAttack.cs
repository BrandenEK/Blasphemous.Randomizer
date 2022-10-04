using System;
using Gameplay.GameControllers.Enemies.Acolyte.IA;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Weapon;

namespace Gameplay.GameControllers.Enemies.Acolyte
{
	public class AcolyteAttack : EnemyAttack
	{
		public bool TriggerAttack { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.CurrentEnemyWeapon = base.GetComponentInChildren<Weapon>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._acolyte = base.GetComponentInParent<Acolyte>();
			this._attackArea = this._acolyte.GetComponentInChildren<AttackArea>();
			this._behaviour = (AcolyteBehaviour)this._acolyte.EnemyBehaviour;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!this._attackArea.EnemyIsInAttackArea)
			{
				return;
			}
			if (this._behaviour.IsAttackWindowOpen && !this.TriggerAttack)
			{
				this.TriggerAttack = true;
				this.CurrentWeaponAttack(this.DamageType);
			}
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			base.CurrentWeaponAttack(damageType);
			if (!base.CurrentEnemyWeapon)
			{
				return;
			}
			Hit weapondHit = new Hit
			{
				AttackingEntity = this._acolyte.gameObject,
				DamageType = damageType,
				DamageAmount = this._acolyte.Stats.Strength.Final,
				HitSoundId = this.HitSound,
				Force = this.Force
			};
			base.CurrentEnemyWeapon.Attack(weapondHit);
		}

		private Acolyte _acolyte;

		private AttackArea _attackArea;

		private AcolyteBehaviour _behaviour;

		private bool _reverse;
	}
}
