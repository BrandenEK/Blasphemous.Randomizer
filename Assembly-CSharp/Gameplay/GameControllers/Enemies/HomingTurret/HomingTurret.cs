using System;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.HomingTurret.AI;
using Gameplay.GameControllers.Enemies.HomingTurret.Animation;
using Gameplay.GameControllers.Enemies.HomingTurret.Audio;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.HomingTurret
{
	public class HomingTurret : Enemy, IDamageable
	{
		public VisionCone Vision { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public HomingTurretAnimationInyector AnimationInyector { get; private set; }

		public HomingTurretAudio Audio { get; private set; }

		private MasterShaderEffects ColorBlink { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Vision = base.GetComponentInChildren<VisionCone>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			base.EnemyBehaviour = base.GetComponentInChildren<EnemyBehaviour>();
			this.AnimationInyector = base.GetComponentInChildren<HomingTurretAnimationInyector>();
			this.ColorBlink = base.GetComponentInChildren<MasterShaderEffects>();
			this.Audio = base.GetComponentInChildren<HomingTurretAudio>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.SetPositionAtStart();
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			if (base.Landing)
			{
				return;
			}
			float distance = Physics2D.Raycast(base.transform.position, -Vector2.up, 5f, base.EnemyBehaviour.BlockLayerMask).distance;
			Vector3 position = base.transform.position;
			position.y -= distance;
			base.transform.position = position;
			base.Landing = true;
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

		public void Damage(Hit hit)
		{
			this.DamageArea.TakeDamage(hit, false);
			this.SleepTimeByHit(hit);
			this.ColorBlink.DamageEffectBlink(0f, this.ColorBlink.FlashTimeAmount, this.ColorBlink.damageEffectTestMaterial);
			HomingTurretBehaviour homingTurretBehaviour = (HomingTurretBehaviour)base.EnemyBehaviour;
			if (this.Status.Dead)
			{
				homingTurretBehaviour.Dead();
				this.AnimationInyector.Death();
				this.DisableEnemyBarrier();
			}
			else
			{
				homingTurretBehaviour.Damage();
			}
		}

		private void DisableEnemyBarrier()
		{
			EnemyBarrier componentInChildren = base.GetComponentInChildren<EnemyBarrier>();
			if (componentInChildren)
			{
				componentInChildren.gameObject.SetActive(false);
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}
	}
}
