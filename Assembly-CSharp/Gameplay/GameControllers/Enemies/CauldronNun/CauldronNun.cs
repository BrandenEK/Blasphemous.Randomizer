using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.CauldronNun.AI;
using Gameplay.GameControllers.Enemies.CauldronNun.Animator;
using Gameplay.GameControllers.Enemies.CauldronNun.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CauldronNun
{
	public class CauldronNun : Enemy, IDamageable
	{
		public CauldronNunBehaviour Behaviour { get; private set; }

		public CauldronNunAnimatorInyector AnimatorInyector { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public ColorFlash colorFlash { get; private set; }

		public CauldronNunAudio Audio { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Behaviour = base.GetComponentInChildren<CauldronNunBehaviour>();
			this.AnimatorInyector = base.GetComponentInChildren<CauldronNunAnimatorInyector>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.colorFlash = base.GetComponentInChildren<ColorFlash>();
			this.Audio = base.GetComponentInChildren<CauldronNunAudio>();
		}

		private void SetDefaultOrientation(Vector3 targetPos)
		{
			this.SetOrientation((targetPos.x < base.transform.position.x) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
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
			this.colorFlash.TriggerColorFlash();
			if (this.Status.Dead)
			{
				this.DamageArea.DamageAreaCollider.enabled = false;
				this.Behaviour.Death();
			}
			else
			{
				this.Behaviour.Damage();
			}
			this.SleepTimeByHit(hit);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}
	}
}
