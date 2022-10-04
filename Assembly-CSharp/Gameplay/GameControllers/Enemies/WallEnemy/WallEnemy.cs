using System;
using DamageEffect;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.WallEnemy.Audio;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WallEnemy
{
	public class WallEnemy : Enemy, IDamageable
	{
		public EnemyDamageArea DamageArea { get; private set; }

		public DamageEffectScript DamageEffect { get; set; }

		public WallEnemyAudio Audio { get; set; }

		public GameObject ClimbableWall { get; private set; }

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			base.EnemyBehaviour = base.GetComponentInChildren<EnemyBehaviour>();
			this.DamageEffect = base.GetComponentInChildren<DamageEffectScript>();
			this.Audio = base.GetComponentInChildren<WallEnemyAudio>();
		}

		public void Damage(Hit hit)
		{
			if (this.Status.Dead)
			{
				return;
			}
			this.Audio.PlayDamage();
			this.DamageArea.TakeDamage(hit, false);
			base.EnemyBehaviour.Damage();
			this.DamageEffect.Blink(0f, 0.1f);
			this.SleepTimeByHit(hit);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (this.Status.Dead)
			{
				return;
			}
			if ((this.WallLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.ClimbableWall = other.gameObject;
			this.ClimbableWall.SetActive(false);
		}

		private new void OnDestroy()
		{
			if (this.ClimbableWall)
			{
				this.ClimbableWall.SetActive(true);
			}
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

		public LayerMask WallLayer;
	}
}
