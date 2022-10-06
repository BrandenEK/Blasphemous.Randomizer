using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.ShieldMaiden.Attack;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ShieldMaiden.IA
{
	public class ShieldMaidenBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public ShieldMaiden ShieldMaiden { get; private set; }

		public bool Awaken { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.ShieldMaiden = (ShieldMaiden)this.Entity;
		}

		public override void OnStart()
		{
			base.OnStart();
		}

		private void Update()
		{
			if (this._waitPeriod > 0f)
			{
				this._waitPeriod -= Time.deltaTime;
			}
			this.DistanceToTarget = Vector2.Distance(this.ShieldMaiden.transform.position, this.ShieldMaiden.Target.transform.position);
			if (!base.IsAttacking)
			{
				this._currentAttackLapse += Time.deltaTime;
			}
			this.CheckParryCounter();
			if (this.DistanceToTarget > this.ActivationDistance || base.BehaviourTree.isRunning || this.Awaken)
			{
				return;
			}
			this.Awaken = true;
			base.BehaviourTree.StartBehaviour();
		}

		private void CheckParryCounter()
		{
			if (this._parryTime > 0f)
			{
				this._parryTime -= Time.deltaTime;
			}
			else if (this._parryTime < 0f)
			{
				this._parryTime = 0f;
				this._currentAttackLapse = 0f;
				this.ShieldMaiden.AnimatorInyector.ParryRecover();
			}
		}

		public bool IsOnWaitingPeriod()
		{
			return this._waitPeriod > 0f;
		}

		private void SetRandomWait()
		{
			this._waitPeriod = Random.Range(0.3f, 0.5f);
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public bool TargetCanBeVisible()
		{
			float num = this.ShieldMaiden.Target.transform.position.y - this.ShieldMaiden.transform.position.y;
			num = ((num <= 0f) ? (-num) : num);
			return num <= this.MaxVisibleHeight;
		}

		public bool CanSeePenitent()
		{
			return this.ShieldMaiden.VisionCone.CanSeeTarget(base.GetTarget(), "Penitent", false);
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.ShieldMaiden.transform.position.x)
			{
				if (this.ShieldMaiden.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.ShieldMaiden.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.ShieldMaiden.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.ShieldMaiden.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public void OnBouncedBackByOverlapping()
		{
			if (this.ShieldMaiden.Input.HorizontalInput != 0f && this.CanSeePenitent() && this.CanSeePenitent())
			{
				this._currentAttackLapse = Random.Range(0f, this.AttackCoolDown / 2f);
			}
		}

		public void Chase(Vector3 position)
		{
			if (base.IsAttacking)
			{
				this.StopMovement();
				return;
			}
			this.LookAtTarget(position);
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			if (!this.motionChecker.HitsFloor)
			{
				this.StopMovement();
				return;
			}
			this.ShieldMaiden.Input.HorizontalInput = horizontalInput;
			this.ShieldMaiden.AnimatorInyector.Walk();
		}

		public override void Damage()
		{
		}

		public void OnShieldHit()
		{
			this._currentAttackLapse = 0f;
		}

		public bool CanAttack()
		{
			return this._currentAttackLapse >= this.AttackCoolDown;
		}

		public void ToggleShield(bool active)
		{
			this.ShieldMaiden.IsGuarding = active;
		}

		public void Death()
		{
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
			this.ShieldMaiden.AnimatorInyector.Death();
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
			if (base.IsAttacking || this.IsBeingParried())
			{
				return;
			}
			this.StopMovement();
			Transform target = base.GetTarget();
			this.ShieldMaidenAttack.target = target;
			this.LookAtTarget(target.position);
			if (this._currentAttackLapse < this.AttackCoolDown)
			{
				return;
			}
			this._currentAttackLapse = this.GetRandomAttackLapse();
			this.ShieldMaiden.AnimatorInyector.Attack();
		}

		private float GetRandomAttackLapse()
		{
			return Random.Range(-0.3f, 0f);
		}

		private bool IsBeingParried()
		{
			return this._parryTime > 0f;
		}

		public void OnParry()
		{
			this._currentAttackLapse = this.GetRandomAttackLapse();
			this.ShieldMaiden.AnimatorInyector.Parry();
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("Parry");
			this._parryTime = this.maxParryTime;
		}

		public override void StopMovement()
		{
			this.ShieldMaiden.AnimatorInyector.Stop();
			this.ShieldMaiden.Input.HorizontalInput = 0f;
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
			bool hitsPatrolBlock = this.motionChecker.HitsPatrolBlock;
			if (this.isBlocked || !hitsFloor || hitsPatrolBlock)
			{
				if (Time.time - this._lastDirectionChange > 0.5f)
				{
					this.LookAtTarget(base.transform.position - num * Vector3.right);
					this._lastDirectionChange = Time.time;
					return;
				}
				this.SetRandomWait();
			}
			this.ShieldMaiden.Input.HorizontalInput = num;
			this.ShieldMaiden.AnimatorInyector.Walk();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Execution()
		{
			base.Execution();
			this.ShieldMaiden.gameObject.layer = LayerMask.NameToLayer("Default");
			this.StopMovement();
			base.StopBehaviour();
			this.ShieldMaiden.SpriteRenderer.enabled = false;
			this.ShieldMaiden.EntExecution.InstantiateExecution();
			if (this.ShieldMaiden.EntExecution != null)
			{
				this.ShieldMaiden.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			if (this.ShieldMaiden.Status.Dead)
			{
				return;
			}
			base.StartBehaviour();
			this.ShieldMaiden.SpriteRenderer.enabled = true;
			this.ShieldMaiden.CurrentLife = this.ShieldMaiden.Stats.Life.Base / 2f;
			this.ShieldMaiden.gameObject.layer = LayerMask.NameToLayer("Enemy");
			if (this.ShieldMaiden.EntExecution != null)
			{
				this.ShieldMaiden.EntExecution.enabled = false;
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MaxVisibleHeight = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MinAttackDistance = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackCoolDown = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float maxParryTime = 2f;

		public ShieldMaidenAttack ShieldMaidenAttack;

		[FoldoutGroup("Traits", true, 0)]
		public EntityMotionChecker motionChecker;

		private float _currentAttackLapse;

		private float _parryTime;

		private float _waitPeriod;

		private float _lastDirectionChange;
	}
}
