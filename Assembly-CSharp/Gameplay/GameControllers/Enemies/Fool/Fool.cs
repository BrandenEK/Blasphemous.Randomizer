using System;
using CreativeSpore.SmartColliders;
using Gameplay.GameControllers.Enemies.Fool.AI;
using Gameplay.GameControllers.Enemies.Fool.Animator;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Fool
{
	public class Fool : Enemy, IDamageable
	{
		public NPCInputs Input { get; private set; }

		public FoolAnimatorInyector AnimatorInyector { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public FoolBehaviour Behaviour { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public Vector2 StartPosition { get; private set; }

		public bool IsSummoned { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.AnimatorInyector = base.GetComponentInChildren<FoolAnimatorInyector>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Behaviour = base.GetComponentInChildren<FoolBehaviour>();
			this.enemyAttack = base.GetComponentInChildren<EnemyAttack>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.Status.CastShadow = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = this.Behaviour.GetTarget().gameObject;
			}
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (base.Landing)
			{
				return;
			}
			base.Landing = true;
			this.SetPositionAtStart();
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
			base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
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
			return base.transform.position;
		}

		public void ReuseObject()
		{
			this.IsSummoned = true;
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
