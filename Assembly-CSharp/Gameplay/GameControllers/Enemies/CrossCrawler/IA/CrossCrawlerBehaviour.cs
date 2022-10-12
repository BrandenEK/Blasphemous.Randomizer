using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CrossCrawler.IA
{
	public class CrossCrawlerBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public CrossCrawler CrossCrawler { get; private set; }

		public bool Awaken { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.CrossCrawler = (CrossCrawler)this.Entity;
		}

		public override void OnStart()
		{
			base.OnStart();
			if (Core.Logic.Penitent != null)
			{
				Core.Logic.Penitent.OnDeath += this.PlayerOnDeath;
			}
		}

		private void Update()
		{
			this.DistanceToTarget = Vector2.Distance(this.CrossCrawler.transform.position, this.CrossCrawler.Target.transform.position);
			if (!base.IsAttacking)
			{
				this._currentAttackLapse += Time.deltaTime;
			}
			if (this.DistanceToTarget > this.ActivationDistance || base.BehaviourTree.isRunning || this.Awaken || this.isExecuted)
			{
				return;
			}
			this.Awaken = true;
			base.BehaviourTree.StartBehaviour();
		}

		public override void Idle()
		{
			this.CrossCrawler.Audio.StopWalk();
			this.StopMovement();
		}

		public bool CanSeeTarget()
		{
			Transform target = base.GetTarget();
			Vector2 vector = base.transform.position + this.sightOffset;
			Vector2 vector2 = target.position + this.sightOffset * 0.5f;
			float num = Vector2.Distance(vector, target.position);
			if (num > this.sightDistance)
			{
				return false;
			}
			Vector2 vector3 = vector2 - vector;
			float num2 = Mathf.Atan2(Mathf.Abs(vector3.y), Mathf.Abs(vector3.x)) * 57.29578f;
			float num3 = this.visionAngle;
			if (num2 > num3)
			{
				Debug.DrawLine(vector, vector2, Color.magenta, 1f);
				return false;
			}
			RaycastHit2D[] array = new RaycastHit2D[1];
			bool flag = Physics2D.LinecastNonAlloc(vector, vector2, array, this.sightCollisionMask) > 0;
			if (flag)
			{
				if (array[0].collider.gameObject.layer == LayerMask.NameToLayer("Penitent"))
				{
					Debug.DrawLine(vector, array[0].point, Color.green, 1f);
					return true;
				}
				Debug.DrawLine(vector, array[0].point, Color.red, 1f);
			}
			else
			{
				Debug.DrawLine(vector, vector2, Color.red, 1f);
			}
			return false;
		}

		public void StartVulnerablePeriod()
		{
			this.CrossCrawler.VulnerablePeriod.StartVulnerablePeriod(this.CrossCrawler);
		}

		private void ToggleCollidersOrientation()
		{
			foreach (BoxCollider2D boxCollider2D in this.orientationAffectedColliders)
			{
				Vector2 offset = boxCollider2D.offset;
				offset.x *= -1f;
				boxCollider2D.offset = offset;
			}
		}

		public override void ReadSpawnerConfig(SpawnBehaviourConfig config)
		{
			base.ReadSpawnerConfig(config);
			if (this.CrossCrawler.Status.Orientation == EntityOrientation.Left)
			{
				this.ToggleCollidersOrientation();
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.CrossCrawler.transform.position.x)
			{
				if (this.CrossCrawler.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.CrossCrawler.SetOrientation(EntityOrientation.Right, false, false);
				this.ToggleCollidersOrientation();
				this.CrossCrawler.Audio.StopWalk();
				this.CrossCrawler.AnimatorInyector.TurnAround();
			}
			else
			{
				if (this.CrossCrawler.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.CrossCrawler.SetOrientation(EntityOrientation.Left, false, false);
				this.ToggleCollidersOrientation();
				this.CrossCrawler.Audio.StopWalk();
				this.CrossCrawler.AnimatorInyector.TurnAround();
			}
		}

		private bool CantChase()
		{
			return base.IsAttacking || base.IsDead() || base.IsHurt || base.TurningAround || !this.CrossCrawler.MotionChecker.HitsFloor;
		}

		public void Chase(Vector3 position)
		{
			if (this.CantChase())
			{
				this.StopMovement();
				return;
			}
			this.LookAtTarget(position);
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.CrossCrawler.Input.HorizontalInput = horizontalInput;
			this.CrossCrawler.AnimatorInyector.Walk();
		}

		public override void Damage()
		{
		}

		public void Death()
		{
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
			this.CrossCrawler.Audio.StopWalk();
			this.CrossCrawler.AnimatorInyector.Death();
		}

		private void PlayerOnDeath()
		{
			base.BehaviourTree.StopBehaviour();
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
			this.CrossCrawler.Audio.StopWalk();
			if (base.TurningAround)
			{
				return;
			}
			this.StopMovement();
			if (this._currentAttackLapse < this.AttackCoolDown)
			{
				return;
			}
			this._currentAttackLapse = 0f;
			this.CrossCrawler.AnimatorInyector.Attack();
		}

		public override void Execution()
		{
			base.Execution();
			this.isExecuted = true;
			this.CrossCrawler.gameObject.layer = LayerMask.NameToLayer("Default");
			this.CrossCrawler.Audio.StopAll();
			this.CrossCrawler.AnimatorInyector.ResetToIdle();
			this.StopMovement();
			base.StopBehaviour();
			this.CrossCrawler.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.CrossCrawler.EntExecution.InstantiateExecution();
			if (this.CrossCrawler.EntExecution != null)
			{
				this.CrossCrawler.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.CrossCrawler.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.CrossCrawler.SpriteRenderer.enabled = true;
			this.CrossCrawler.Animator.Play("Idle");
			this.CrossCrawler.CurrentLife = this.CrossCrawler.Stats.Life.Base / 2f;
			base.StartBehaviour();
			if (this.CrossCrawler.EntExecution != null)
			{
				this.CrossCrawler.EntExecution.enabled = false;
			}
		}

		public override void StopMovement()
		{
			this.CrossCrawler.AnimatorInyector.Stop();
			this.CrossCrawler.Input.HorizontalInput = 0f;
		}

		private void OnDestroy()
		{
			Core.Logic.Penitent.OnDeath -= this.PlayerOnDeath;
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
			Vector2 vector = base.transform.position + this.sightOffset;
			Vector2 v = vector + Quaternion.Euler(0f, 0f, this.visionAngle) * Vector2.right * this.sightDistance;
			Vector2 v2 = vector + Quaternion.Euler(0f, 0f, -this.visionAngle) * Vector2.right * this.sightDistance;
			Gizmos.DrawLine(vector, v);
			Gizmos.DrawLine(vector, v2);
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MinAttackDistance = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackCoolDown = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		private float _currentAttackLapse;

		[FoldoutGroup("Vision Settings", true, 0)]
		public LayerMask sightCollisionMask;

		[FoldoutGroup("Vision Settings", true, 0)]
		public Vector2 sightOffset;

		[FoldoutGroup("Vision Settings", true, 0)]
		public float visionAngle;

		[FoldoutGroup("Vision Settings", true, 0)]
		public float sightDistance;

		public List<BoxCollider2D> orientationAffectedColliders;

		private bool isExecuted;
	}
}
