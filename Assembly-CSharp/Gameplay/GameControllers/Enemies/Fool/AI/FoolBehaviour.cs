using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Fool.AI
{
	public class FoolBehaviour : EnemyBehaviour
	{
		public Fool Fool { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.Fool = (Fool)this.Entity;
			this.Fool.Target = base.GetTarget().gameObject;
			this._bottomHits = new RaycastHit2D[2];
		}

		public bool CanWalk()
		{
			return !this._isSpawning;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.DistanceToTarget = Vector2.Distance(this.Fool.transform.position, this.Fool.Target.transform.position);
			if (this.DistanceToTarget <= this.ActivationDistance && !base.BehaviourTree.isRunning)
			{
				base.BehaviourTree.StartBehaviour();
			}
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public override void Wander()
		{
		}

		public void Chase(Vector3 position)
		{
			if (this.DistanceToTarget > 1f)
			{
				this.LookAtTarget(position);
			}
			if (!this.Fool.MotionChecker.HitsFloor || this.Fool.MotionChecker.HitsBlock || this.Fool.Status.Dead || Core.Logic.Penitent.Dead)
			{
				this.StopMovement();
				return;
			}
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.Fool.Input.HorizontalInput = horizontalInput;
			this.Fool.AnimatorInyector.Walk();
		}

		public void ChargeExplosion()
		{
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Attack()
		{
		}

		public override void Damage()
		{
		}

		public void Death()
		{
			this.StopMovement();
			this.Fool.AnimatorInyector.Death();
		}

		public bool TargetCanBeVisible()
		{
			float num = this.Fool.Target.transform.position.y - this.Fool.transform.position.y;
			num = ((num <= 0f) ? (-num) : num);
			return num <= this.MaxVisibleHeight;
		}

		public override void StopMovement()
		{
			this.Fool.Input.HorizontalInput = 0f;
			this.Fool.AnimatorInyector.StopWalk();
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.Fool.transform.position.x)
			{
				if (this.Fool.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.Fool.SetOrientation(EntityOrientation.Right, false, false);
				this.Fool.AnimatorInyector.TurnAround();
			}
			else
			{
				if (this.Fool.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.Fool.SetOrientation(EntityOrientation.Left, false, false);
				this.Fool.AnimatorInyector.TurnAround();
			}
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		public float DistanceToTarget;

		public float MaxVisibleHeight = 2f;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float MaxTimeAwaitingBeforeGoBack;

		[FoldoutGroup("Motion Settings", true, 0)]
		public LayerMask GroundLayerMask;

		private RaycastHit2D[] _bottomHits;

		[SerializeField]
		[FoldoutGroup("Motion Settings", true, 0)]
		public float _myWidth;

		[SerializeField]
		[FoldoutGroup("Motion Settings", true, 0)]
		public float _myHeight;

		private bool _isSpawning;
	}
}
