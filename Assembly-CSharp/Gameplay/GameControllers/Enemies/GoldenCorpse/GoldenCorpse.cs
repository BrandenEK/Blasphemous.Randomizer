using System;
using CreativeSpore.SmartColliders;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.GoldenCorpse.AI;
using Gameplay.GameControllers.Enemies.GoldenCorpse.Animator;
using Gameplay.GameControllers.Enemies.GoldenCorpse.Audio;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.GoldenCorpse
{
	public class GoldenCorpse : Enemy, IDamageable
	{
		public NPCInputs Input { get; private set; }

		public GoldenCorpseAnimatorInyector AnimatorInyector { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public GoldenCorpseBehaviour Behaviour { get; private set; }

		public EntityMotionChecker MotionChecker { get; private set; }

		public Vector2 StartPosition { get; private set; }

		public GoldenCorpseAudio Audio { get; private set; }

		public bool IsSummoned { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.AnimatorInyector = base.GetComponentInChildren<GoldenCorpseAnimatorInyector>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			this.Behaviour = base.GetComponentInChildren<GoldenCorpseBehaviour>();
			this.enemyAttack = base.GetComponentInChildren<EnemyAttack>();
			this.MotionChecker = base.GetComponentInChildren<EntityMotionChecker>();
			this.Audio = base.GetComponentInChildren<GoldenCorpseAudio>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.Status.CastShadow = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
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
			Vector3 position = base.transform.position;
			float groundDist = base.Controller.GroundDist;
			Vector3 position2 = new Vector3(position.x, position.y - groundDist, position.z);
			base.transform.position = position2;
			base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
		}

		public void Damage(Hit hit)
		{
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
