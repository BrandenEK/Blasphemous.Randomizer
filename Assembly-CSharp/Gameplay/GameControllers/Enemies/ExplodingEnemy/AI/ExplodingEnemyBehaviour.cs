using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ExplodingEnemy.AI
{
	public class ExplodingEnemyBehaviour : EnemyBehaviour
	{
		public ExplodingEnemy ExplodingEnemy { get; set; }

		public bool IsExploding { get; set; }

		public bool IsMelting { get; set; }

		public bool IsGoingBack
		{
			get
			{
				return this._isGoingBack;
			}
		}

		public override void OnStart()
		{
			base.OnStart();
			this.ExplodingEnemy = (ExplodingEnemy)this.Entity;
			this._bottomHits = new RaycastHit2D[2];
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.ExplodingEnemy.Target == null)
			{
				return;
			}
			this.DistanceToTarget = Vector2.Distance(this.ExplodingEnemy.transform.position, this.ExplodingEnemy.Target.transform.position);
			if (this.DistanceToTarget <= this.ActivationDistance && !base.BehaviourTree.isRunning && !this.IsExploding)
			{
				base.BehaviourTree.StartBehaviour();
			}
		}

		public override void Idle()
		{
			this.StopMovement();
			if (Vector2.Distance(this.Entity.transform.position, this.ExplodingEnemy.StartPosition) <= 1f)
			{
				return;
			}
			this._currentTimeAwaitingBeforeGoBack += Time.deltaTime;
			if (this._currentTimeAwaitingBeforeGoBack >= this.MaxTimeAwaitingBeforeGoBack)
			{
				this._isGoingBack = true;
			}
		}

		public override void Wander()
		{
		}

		public void GoBack()
		{
			float num = Vector2.Distance(this.Entity.transform.position, this.ExplodingEnemy.StartPosition);
			if (num > 1f)
			{
				this.Chase(this.ExplodingEnemy.StartPosition);
			}
			else
			{
				this._isGoingBack = false;
				this.StopMovement();
			}
		}

		public void Chase(Vector3 position)
		{
			this.LookAtTarget(position);
			if (base.IsHurt || !this.Grounded() || base.TurningAround || this.IsMelting)
			{
				this.StopMovement();
				return;
			}
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.ExplodingEnemy.Input.HorizontalInput = horizontalInput;
			this._currentTimeAwaitingBeforeGoBack = 0f;
			this.ExplodingEnemy.AnimatorInyector.Walk();
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
			if (this.ExplodingEnemy.Status.Dead)
			{
				this.ExplodingEnemy.AnimatorInyector.ChargeExplosion();
			}
			else
			{
				this.ExplodingEnemy.AnimatorInyector.Damage();
			}
		}

		private bool Grounded()
		{
			bool result;
			if (this.Entity.Status.Orientation == EntityOrientation.Left)
			{
				Vector2 vector = base.transform.position - (base.transform.right * this._myWidth * 0.75f + Vector2.up * (this._myHeight * 2f));
				Debug.DrawLine(vector, vector - Vector2.up * 1f, Color.yellow);
				result = (Physics2D.LinecastNonAlloc(vector, vector - Vector2.up * 1f, this._bottomHits, this.GroundLayerMask) > 0);
			}
			else
			{
				Vector2 vector2 = base.transform.position + (base.transform.right * this._myWidth * 0.75f - Vector2.up * (this._myHeight * 2f));
				Debug.DrawLine(vector2, vector2 - Vector2.up * 1f, Color.yellow);
				result = (Physics2D.LinecastNonAlloc(vector2, vector2 - Vector2.up * 1f, this._bottomHits, this.GroundLayerMask) > 0);
			}
			return result;
		}

		public bool TargetCanBeVisible()
		{
			return this.ExplodingEnemy.VisionCone.CanSeeTarget(this.ExplodingEnemy.Target.transform, "Penitent", false);
		}

		public override void StopMovement()
		{
			this.ExplodingEnemy.Input.HorizontalInput = 0f;
			this.ExplodingEnemy.AnimatorInyector.StopWalk();
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.ExplodingEnemy.transform.position.x)
			{
				if (this.ExplodingEnemy.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.ExplodingEnemy.SetOrientation(EntityOrientation.Right, false, false);
				this.ExplodingEnemy.AnimatorInyector.TurnAround();
			}
			else
			{
				if (this.ExplodingEnemy.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.ExplodingEnemy.SetOrientation(EntityOrientation.Left, false, false);
				this.ExplodingEnemy.AnimatorInyector.TurnAround();
			}
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		public float DistanceToTarget;

		public float MaxVisibleHeight = 2f;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float MaxTimeAwaitingBeforeGoBack;

		[FoldoutGroup("Activation Settings", true, 0)]
		public bool IsChargingExplosion;

		[FoldoutGroup("Motion Settings", true, 0)]
		public LayerMask GroundLayerMask;

		private RaycastHit2D[] _bottomHits;

		[SerializeField]
		[FoldoutGroup("Motion Settings", true, 0)]
		public float _myWidth;

		[SerializeField]
		[FoldoutGroup("Motion Settings", true, 0)]
		public float _myHeight;

		private float _currentTimeAwaitingBeforeGoBack;

		private bool _isGoingBack;
	}
}
