using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.RangedBoomerang.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.RangedBoomerang.IA
{
	public class RangedBoomerangBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public RangedBoomerang RangedBoomerang { get; private set; }

		public bool Awaken { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.RangedBoomerang = (RangedBoomerang)this.Entity;
		}

		private void Update()
		{
			this.DistanceToTarget = Vector2.Distance(this.RangedBoomerang.transform.position, this.RangedBoomerang.Target.transform.position);
			if (!base.IsAttacking && this.CanSeeTarget())
			{
				this._currentAttackLapse += Time.deltaTime;
			}
			if (this.DistanceToTarget > this.ActivationDistance || base.BehaviourTree.isRunning || this.Awaken)
			{
				return;
			}
			this.Awaken = true;
			base.BehaviourTree.StartBehaviour();
		}

		public void OnBoomerangRecovered()
		{
			this.RangedBoomerang.Audio.StopThrow();
			this.RangedBoomerang.AnimatorInyector.Recover();
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public bool ShouldPatrol()
		{
			return this.doPatrol;
		}

		public bool CanSeeTarget()
		{
			return this.RangedBoomerang.VisionCone.CanSeeTarget(this.RangedBoomerang.Target.transform, "Penitent", false);
		}

		public bool CanAttack()
		{
			return this._currentAttackLapse >= this.AttackCoolDown;
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.RangedBoomerang.transform.position.x)
			{
				if (this.RangedBoomerang.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.RangedBoomerang.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.RangedBoomerang.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.RangedBoomerang.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public void Chase(Vector3 position)
		{
		}

		public override void Damage()
		{
		}

		public void Death()
		{
			this.RangedBoomerang.Audio.StopThrow();
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
			this.RangedBoomerang.AnimatorInyector.Death();
		}

		public void ResetCoolDown()
		{
			if (this._currentAttackLapse > 0f)
			{
				this._currentAttackLapse = 0f;
			}
		}

		public override void Attack()
		{
			if (base.IsAttacking)
			{
				return;
			}
			this.StopMovement();
			Transform target = base.GetTarget();
			this.rangedBoomerangAttack.target = target;
			this.LookAtTarget(target.position);
			if (this._currentAttackLapse < this.AttackCoolDown)
			{
				return;
			}
			this._currentAttackLapse = 0f;
			this.RangedBoomerang.AnimatorInyector.Attack();
		}

		public override void StopMovement()
		{
			this.RangedBoomerang.AnimatorInyector.Stop();
			this.RangedBoomerang.Input.HorizontalInput = 0f;
		}

		public override void Wander()
		{
			if (base.IsAttacking || base.IsHurt)
			{
				this.StopMovement();
				return;
			}
			float num = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.isBlocked = this.motionChecker.HitsBlock;
			bool hitsFloor = this.motionChecker.HitsFloor;
			if (this.isBlocked || !hitsFloor)
			{
				this.LookAtTarget(base.transform.position - num * Vector3.right);
				return;
			}
			this.RangedBoomerang.Input.HorizontalInput = num;
			this.RangedBoomerang.AnimatorInyector.Walk();
		}

		public override void Execution()
		{
			base.Execution();
			this.isExecuted = true;
			this.RangedBoomerang.gameObject.layer = LayerMask.NameToLayer("Default");
			this.RangedBoomerang.Audio.StopAll();
			this.RangedBoomerang.Animator.Play("Idle");
			this.StopMovement();
			base.StopBehaviour();
			this.RangedBoomerang.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			if (this.RangedBoomerang.EntExecution != null)
			{
				this.RangedBoomerang.EntExecution.InstantiateExecution();
				this.RangedBoomerang.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.isExecuted = false;
			this.RangedBoomerang.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.RangedBoomerang.SpriteRenderer.enabled = true;
			this.RangedBoomerang.Animator.Play("Idle");
			this.RangedBoomerang.CurrentLife = this.RangedBoomerang.Stats.Life.Base / 2f;
			this.RangedBoomerang.Attack.enabled = true;
			base.StartBehaviour();
			if (this.RangedBoomerang.EntExecution != null)
			{
				this.RangedBoomerang.EntExecution.enabled = false;
			}
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void ReadSpawnerConfig(SpawnBehaviourConfig config)
		{
			base.ReadSpawnerConfig(config);
			this.doPatrol = !config.dontWalk;
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MaxVisibleHeight = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MinAttackDistance = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackCoolDown = 1.75f;

		public bool doPatrol = true;

		private float _currentAttackLapse = 1.25f;

		public RangedBoomerangAttack rangedBoomerangAttack;

		public EntityMotionChecker motionChecker;

		public LayerMask sightCollisionMask;

		public Vector2 sightOffset;

		public float visionAngle;

		public float debugLastAngle;

		private bool isExecuted;
	}
}
