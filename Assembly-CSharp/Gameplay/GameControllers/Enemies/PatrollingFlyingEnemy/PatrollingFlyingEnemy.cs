using System;
using BezierSplines;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy.AI;
using Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy.Animator;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PatrollingFlyingEnemy
{
	public class PatrollingFlyingEnemy : Enemy, IDamageable
	{
		public PatrollingFlyingEnemyBehaviour Behaviour { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public PatrollingFlyingEnemyAnimatorInyector AnimatorInyector { get; private set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Behaviour = base.GetComponent<PatrollingFlyingEnemyBehaviour>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.AnimatorInyector = base.GetComponentInChildren<PatrollingFlyingEnemyAnimatorInyector>();
		}

		public void SetConfig(BezierSpline spline, AnimationCurve curve, float secondsToFoolLoop)
		{
			this.Behaviour = base.GetComponent<PatrollingFlyingEnemyBehaviour>();
			this.Behaviour.SetPath(spline);
			this.Behaviour.currentCurve = curve;
			this.Behaviour.secondsToFullLoop = secondsToFoolLoop;
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
			if (this.Status.Dead)
			{
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
			throw new NotImplementedException();
		}
	}
}
