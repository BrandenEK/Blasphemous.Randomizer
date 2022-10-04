using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Firethrower.IA
{
	public class FirethrowerBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public Firethrower Firethrower { get; private set; }

		public bool Awaken { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.Firethrower = (Firethrower)this.Entity;
		}

		public override void OnStart()
		{
			base.OnStart();
			this.Firethrower.OnDeath += this.OnDeath;
		}

		private void OnDeath()
		{
			this.Firethrower.OnDeath -= this.OnDeath;
			BoxCollider2D[] componentsInChildren = this.Firethrower.Attack.GetComponentsInChildren<BoxCollider2D>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}

		private void Update()
		{
			if (this.Firethrower.Status.Dead)
			{
				return;
			}
			this.Firethrower.Target = base.GetTarget().gameObject;
			if (this.Firethrower.Target != null)
			{
				this.DistanceToTarget = Vector2.Distance(this.Firethrower.transform.position, this.Firethrower.Target.transform.position);
			}
			if (!base.IsAttacking)
			{
				this._currentAttackLapse += Time.deltaTime;
			}
			this.CheckCounters();
			if (this.DistanceToTarget > this.ActivationDistance || base.BehaviourTree.isRunning || this.Awaken)
			{
				return;
			}
			this.Awaken = true;
			base.BehaviourTree.StartBehaviour();
		}

		private void CheckCounters()
		{
			if (this.currentChargeTime >= 0f && this.currentChargeTime < this.chargeTime)
			{
				this.currentChargeTime += Time.deltaTime;
			}
			else if (this.currentChargeTime >= this.chargeTime)
			{
				this.currentChargeTime = -1f;
				this.Firethrower.AnimatorInyector.Attack();
				this.currentAttackState = FirethrowerBehaviour.ATTACK_STATE.THROWING_FIRE;
				this.currentAttackTime = 0f;
			}
			if (this.currentAttackTime >= 0f && this.currentAttackTime < this.attackDuration)
			{
				this.currentAttackTime += Time.deltaTime;
			}
			else if (this.currentAttackTime >= this.attackDuration)
			{
				this.currentAttackTime = -1f;
				this.Firethrower.AnimatorInyector.StopAttack();
			}
		}

		public bool IsFreeToMove()
		{
			return this.currentAttackState == FirethrowerBehaviour.ATTACK_STATE.NOT_ATTACKING;
		}

		public override void Idle()
		{
			Debug.Log("Firethrower: IDLE");
			this.StopMovement();
		}

		public bool TargetCanBeVisible()
		{
			float num = this.Firethrower.Target.transform.position.y - this.Firethrower.transform.position.y;
			num = ((num <= 0f) ? (-num) : num);
			return num <= this.MaxVisibleHeight;
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.Firethrower.transform.position.x)
			{
				if (this.Firethrower.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.Firethrower.SetOrientation(EntityOrientation.Right, false, false);
				this.Firethrower.AnimatorInyector.TurnAround();
			}
			else
			{
				if (this.Firethrower.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.Firethrower.SetOrientation(EntityOrientation.Left, false, false);
				this.Firethrower.AnimatorInyector.TurnAround();
			}
		}

		public void Chase(Vector3 position)
		{
			if (base.IsAttacking || base.IsHurt)
			{
				this.StopMovement();
				return;
			}
			this.LookAtTarget(position);
			if (base.TurningAround)
			{
				this.StopMovement();
				return;
			}
			float num = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.ChangePlaceholderDirection(num);
			this.Firethrower.Input.HorizontalInput = num;
			this.Firethrower.AnimatorInyector.Walk();
		}

		public override void Damage()
		{
		}

		public void Death()
		{
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
			this.Firethrower.AnimatorInyector.Death();
			this.currentAttackState = FirethrowerBehaviour.ATTACK_STATE.NOT_ATTACKING;
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
			this.CheckAttackState();
		}

		private void CheckAttackState()
		{
			FirethrowerBehaviour.ATTACK_STATE attack_STATE = this.currentAttackState;
			if (attack_STATE != FirethrowerBehaviour.ATTACK_STATE.NOT_ATTACKING)
			{
				if (attack_STATE != FirethrowerBehaviour.ATTACK_STATE.CHARGING)
				{
					if (attack_STATE != FirethrowerBehaviour.ATTACK_STATE.THROWING_FIRE)
					{
					}
				}
			}
			else
			{
				this.currentAttackState = FirethrowerBehaviour.ATTACK_STATE.CHARGING;
				this.Firethrower.AnimatorInyector.Charge();
				this.currentChargeTime = 0f;
			}
		}

		public override void StopMovement()
		{
			this.Firethrower.AnimatorInyector.Stop();
			this.Firethrower.Input.HorizontalInput = 0f;
		}

		public override void Wander()
		{
			Debug.Log("Firethrower: WANDER");
			if (base.IsAttacking || base.IsHurt)
			{
				this.StopMovement();
				return;
			}
			if (base.TurningAround)
			{
				this.StopMovement();
				return;
			}
			float num = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.ChangePlaceholderDirection(num);
			this.isBlocked = this.motionChecker.HitsBlock;
			bool hitsFloor = this.motionChecker.HitsFloor;
			if (this.isBlocked || !hitsFloor)
			{
				this.LookAtTarget(base.transform.position - num * Vector3.right);
				return;
			}
			this.Firethrower.Input.HorizontalInput = num;
			this.Firethrower.AnimatorInyector.Walk();
		}

		public void OnAttackAnimationFinished()
		{
			this.currentAttackState = FirethrowerBehaviour.ATTACK_STATE.NOT_ATTACKING;
			this.ResetCoolDown();
		}

		private void ChangePlaceholderDirection(float dir)
		{
			this.attackPlaceholderParent.localScale = new Vector3(dir, 1f, 1f);
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
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

		private float _currentAttackLapse;

		private FirethrowerBehaviour.ATTACK_STATE currentAttackState;

		public float chargeTime = 1f;

		public float attackDuration = 1f;

		private float currentChargeTime = -1f;

		private float currentAttackTime = -1f;

		public EntityMotionChecker motionChecker;

		[Header("TEMP ONLY")]
		public Transform attackPlaceholderParent;

		private enum ATTACK_STATE
		{
			NOT_ATTACKING,
			CHARGING,
			THROWING_FIRE
		}
	}
}
