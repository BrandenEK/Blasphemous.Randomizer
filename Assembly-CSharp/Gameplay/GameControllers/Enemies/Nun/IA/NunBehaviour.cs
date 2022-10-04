using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Nun.IA
{
	public class NunBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public Nun Nun { get; private set; }

		public bool Awaken { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.Nun = (Nun)this.Entity;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Nun.Dead)
			{
				return;
			}
			this.DistanceToTarget = Vector2.Distance(this.Nun.transform.position, base.GetTarget().position);
			if (!base.IsAttacking)
			{
				this._currentAttackLapse += Time.deltaTime;
			}
			if (this.DistanceToTarget > this.ActivationDistance || base.BehaviourTree.isRunning || this.Awaken || base.GotParry)
			{
				return;
			}
			this.Awaken = true;
			base.BehaviourTree.StartBehaviour();
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public bool TargetCanBeVisible()
		{
			float num = this.Nun.Target.transform.position.y - this.Nun.transform.position.y;
			num = ((num <= 0f) ? (-num) : num);
			return num <= this.MaxVisibleHeight;
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.Nun.transform.position.x)
			{
				if (this.Nun.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.Nun.SetOrientation(EntityOrientation.Right, false, false);
				this.Nun.AnimatorInyector.TurnAround();
			}
			else
			{
				if (this.Nun.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.Nun.SetOrientation(EntityOrientation.Left, false, false);
				this.Nun.AnimatorInyector.TurnAround();
			}
		}

		public void Chase(Vector3 position)
		{
			this.LookAtTarget(position);
			if (base.IsHurt || base.TurningAround || base.IsAttacking || this.Nun.Status.Dead)
			{
				this.StopMovement();
				return;
			}
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.Nun.Input.HorizontalInput = horizontalInput;
			this.Nun.AnimatorInyector.Walk();
		}

		public override void Damage()
		{
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Execution()
		{
			base.Execution();
			base.GotParry = true;
			base.BehaviourTree.StopBehaviour();
			this.Nun.IsAttacking = false;
			this.StopMovement();
			this.Nun.gameObject.layer = LayerMask.NameToLayer("Default");
			this.Nun.SpriteRenderer.enabled = false;
			this.Nun.Attack.AttackArea.WeaponCollider.enabled = false;
			this.Nun.Animator.Play("Idle");
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.Nun.EntExecution.InstantiateExecution();
			if (this.Nun.EntExecution != null)
			{
				this.Nun.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			base.GotParry = false;
			base.BehaviourTree.StartBehaviour();
			this.Nun.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.Nun.SpriteRenderer.enabled = true;
			this.Nun.Attack.AttackArea.WeaponCollider.enabled = true;
			this.Nun.Animator.Play("Idle");
		}

		public void Death()
		{
			base.BehaviourTree.StopBehaviour();
			this.Nun.AnimatorInyector.Death();
			this.StopMovement();
			this.Nun.Attack.AttackArea.gameObject.SetActive(false);
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
			this.Nun.AnimatorInyector.Attack();
		}

		public override void StopMovement()
		{
			this.Nun.AnimatorInyector.Stop();
			this.Nun.Input.HorizontalInput = 0f;
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
	}
}
