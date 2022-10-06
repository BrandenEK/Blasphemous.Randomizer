using System;
using CreativeSpore.SmartColliders;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.ExplodingEnemy.Animator;
using Gameplay.GameControllers.Enemies.ExplodingEnemy.Attack;
using Gameplay.GameControllers.Enemies.ExplodingEnemy.Audio;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Framework.Physics;
using Gameplay.GameControllers.Enemies.ReekLeader;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ExplodingEnemy
{
	public class ExplodingEnemy : Enemy, IDamageable
	{
		public NPCInputs Input { get; private set; }

		public ExplodingEnemyAnimatorInyector AnimatorInyector { get; private set; }

		public EnemyDamageArea DamageArea { get; private set; }

		public ExplodingEnemyAudio Audio { get; private set; }

		public ReekLeader ReekLeader { get; set; }

		public VisionCone VisionCone { get; private set; }

		public Vector2 StartPosition { get; private set; }

		public bool IsSummoned { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Input = base.GetComponent<NPCInputs>();
			base.Controller = base.GetComponent<PlatformCharacterController>();
			this.AnimatorInyector = base.GetComponentInChildren<ExplodingEnemyAnimatorInyector>();
			this.DamageArea = base.GetComponentInChildren<EnemyDamageArea>();
			base.EnemyBehaviour = base.GetComponentInChildren<EnemyBehaviour>();
			this.enemyAttack = base.GetComponentInChildren<ExplodingEnemyAttack>();
			this.Audio = base.GetComponentInChildren<ExplodingEnemyAudio>();
			this.VisionCone = base.GetComponentInChildren<VisionCone>();
			base.Target = Core.Logic.Penitent.gameObject;
			if (this.ReekLeader != null)
			{
				this.ReekLeader.OnDeath += this.OnDeathReekLeader;
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Target == null)
			{
				base.Target = Core.Logic.Penitent.gameObject;
			}
			this.Status.IsGrounded = base.Controller.IsGrounded;
			if (!base.Landing)
			{
				base.Landing = true;
				this.SetPositionAtStart();
				this.StartPosition = new Vector2(base.transform.position.x, base.transform.position.y);
				base.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			}
			if (!base.SpriteRenderer.isVisible)
			{
				this._currentInvisibleTime += Time.deltaTime;
				if (this._currentInvisibleTime >= this.InvisibleTimeBeforeRecycle && !base.IsFalling)
				{
					base.IsFalling = true;
					base.EnemyBehaviour.StopBehaviour();
					this.AnimatorInyector.Vanish();
				}
			}
			else
			{
				this._currentInvisibleTime = 0f;
			}
		}

		public override void SetPositionAtStart()
		{
			base.SetPositionAtStart();
			float groundDist = base.Controller.GroundDist;
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y - groundDist, base.transform.position.z);
			base.transform.position = position;
			base.Controller.PlatformCharacterPhysics.GravityScale = 0f;
		}

		public void Damage(Hit hit)
		{
			this.DamageArea.TakeDamage(hit, false);
			base.EnemyBehaviour.Damage();
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

		public void Destroy()
		{
			if (!this.IsSummoned)
			{
				return;
			}
			int instanceID = base.gameObject.GetInstanceID();
			this.Status.CastShadow = false;
			this.ReekLeader.Behaviour.ReekSpawner.ResetSpawnPoint(instanceID);
		}

		private void OnDeathReekLeader()
		{
			base.EnemyBehaviour.StopBehaviour();
			this.AnimatorInyector.Vanish();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.ReekLeader != null)
			{
				this.ReekLeader.OnDeath -= this.OnDeathReekLeader;
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

		public float InvisibleTimeBeforeRecycle = 2f;

		private float _currentInvisibleTime;
	}
}
