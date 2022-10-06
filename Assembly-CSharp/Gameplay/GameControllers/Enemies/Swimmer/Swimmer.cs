using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.Swimmer.AI;
using Gameplay.GameControllers.Enemies.Swimmer.Animator;
using Gameplay.GameControllers.Enemies.Swimmer.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Swimmer
{
	public class Swimmer : Enemy, IDamageable
	{
		public NPCInputs Input { get; private set; }

		public SwimmerBehaviour Behaviour { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public BoxCollider2D DamageAreaCollider { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public SwimmerAnimatorInyector AnimatorInjector { get; private set; }

		public ColorFlash ColorFlash { get; private set; }

		public SmartPlatformCollider Collider { get; private set; }

		public Vector3 StartPoint { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.Behaviour = base.GetComponent<SwimmerBehaviour>();
			this.Input = base.GetComponent<NPCInputs>();
			this.MotionChecker = base.GetComponent<EntityMotionChecker>();
			this.DamageAreaCollider = base.GetComponent<BoxCollider2D>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.enemyAttack = base.GetComponentInChildren<SwimmerAttack>();
			this.AnimatorInjector = base.GetComponentInChildren<SwimmerAnimatorInyector>();
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
			this.Collider = base.GetComponentInChildren<SmartPlatformCollider>();
			this.StartPoint = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			else
			{
				base.DistanceToTarget = Vector2.Distance(base.transform.position, base.Target.transform.position);
			}
			if (base.Landing)
			{
				return;
			}
			base.Landing = true;
			this.SetPositionAtStart();
			base.Animator.Play("Underground");
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
		}

		public override EnemyFloorChecker EnemyFloorChecker()
		{
			throw new NotImplementedException();
		}

		public override EnemyAttack EnemyAttack()
		{
			return this.enemyAttack;
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
			this.GetSparks();
			this.SleepTimeByHit(hit);
			if (this.Status.Dead)
			{
				base.Controller.enabled = false;
				this.AnimatorInjector.Death();
			}
			else
			{
				this.ColorFlash.TriggerColorFlash();
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private void GetSparks()
		{
			if (this.DamageArea == null)
			{
				return;
			}
			PenitentSword penitentSword = (PenitentSword)Core.Logic.Penitent.PenitentAttack.CurrentPenitentWeapon;
			if (penitentSword == null)
			{
				return;
			}
			Bounds bounds = this.DamageArea.DamageAreaCollider.bounds;
			Vector2 vector = default(Vector2);
			vector.x = ((penitentSword.transform.position.x > base.transform.position.x) ? bounds.max.x : bounds.min.x);
			vector.y = bounds.min.y;
			Vector2 position = vector;
			penitentSword.GetSwordSparks(position);
		}
	}
}
