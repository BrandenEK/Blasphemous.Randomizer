using System;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class HomingBonfire : Enemy, IDamageable
	{
		public EnemyDamageArea DamageArea { get; private set; }

		public HomingBonfireAnimationInyector AnimationInyector { get; private set; }

		private MasterShaderEffects ColorBlink { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			base.EnemyBehaviour = base.GetComponentInChildren<EnemyBehaviour>();
			this.AnimationInyector = base.GetComponentInChildren<HomingBonfireAnimationInyector>();
			this.ColorBlink = base.GetComponentInChildren<MasterShaderEffects>();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public void Damage(Hit hit)
		{
			throw new NotImplementedException();
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
	}
}
