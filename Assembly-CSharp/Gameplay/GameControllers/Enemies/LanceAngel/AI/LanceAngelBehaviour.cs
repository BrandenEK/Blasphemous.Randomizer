using System;
using System.Collections;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.LanceAngel.AI
{
	public class LanceAngelBehaviour : EnemyBehaviour
	{
		protected LanceAngel LanceAngel { get; set; }

		public bool IsRepositioning { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.results = new RaycastHit2D[1];
			this.LanceAngel = (LanceAngel)this.Entity;
			this.TargetOffset += new Vector2(Random.Range(-this.RandomTargetOffset.x, this.RandomTargetOffset.x), Random.Range(-this.RandomTargetOffset.x, this.RandomTargetOffset.x));
			if (this.LanceAngel.DashAttack)
			{
				this.LanceAngel.DashAttack.SetDamage((int)this.LanceAngel.Stats.Strength.Final);
			}
			DOTween.To(delegate(float x)
			{
				this._currentAmplitudeX = x;
			}, this._currentAmplitudeX, this.AmplitudeX, 1f);
			DOTween.To(delegate(float x)
			{
				this._currentAmplitudeY = x;
			}, this._currentAmplitudeY, this.AmplitudeY, 1f);
			this.LanceAngel.StateMachine.SwitchState<LanceAngelIdleState>();
			MotionLerper motionLerper = this.LanceAngel.MotionLerper;
			motionLerper.OnLerpStart = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			MotionLerper motionLerper2 = this.LanceAngel.MotionLerper;
			motionLerper2.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper2.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			this.LanceAngel.DashAttack.OnDashAdvancedEvent += this.OnDashUpdatedEvent;
			this.LanceAngel.DashAttack.OnDashFinishedEvent += this.OnDashFinishedEvent;
			this.Entity.OnDeath += this.OnDeath;
		}

		public bool CanSeeTarget
		{
			get
			{
				return this.LanceAngel.VisionCone.CanSeeTarget(this.LanceAngel.Target.transform, "Penitent", false);
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (base.IsAttacking)
			{
				return;
			}
			if (targetPos.x > this.LanceAngel.transform.position.x)
			{
				if (this.LanceAngel.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.LanceAngel.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.LanceAngel.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.LanceAngel.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public void Reposition(Action callBack = null)
		{
			base.StartCoroutine(this.TargetReposition(callBack, 0.9f));
		}

		private IEnumerator TargetReposition(Action callBack, float callbackPercentage = 1f)
		{
			float currentRepositionTime = 0f;
			float repositionTime = this.LanceAngel.Behaviour.RepositionTime;
			this.IsRepositioning = true;
			this.PathOrigin = this.LanceAngel.Spline.transform.position;
			this.SetRepositionDirection();
			bool callbackLaunched = false;
			while (currentRepositionTime <= repositionTime && this.GetHeight > this._minHeight)
			{
				float percentage = currentRepositionTime / repositionTime;
				float value = this.LanceAngel.Behaviour.RepositionCurve.Evaluate(percentage);
				Vector3 nextPoint = this.LanceAngel.Spline.GetPoint(value);
				this.LanceAngel.transform.position = nextPoint;
				currentRepositionTime += Time.deltaTime;
				if (callbackPercentage > percentage && !callbackLaunched)
				{
					if (callBack != null)
					{
						callBack();
					}
					callbackLaunched = true;
				}
				yield return null;
			}
			if (callBack != null)
			{
				callBack();
			}
			yield break;
		}

		public void Dash()
		{
			Vector3 position = this.LanceAngel.Target.transform.position;
			this.LanceAngel.DashAttack.dashDuration = 7f / this.AttackSpeed;
			this.LanceAngel.DashAttack.Dash(this.LanceAngel.transform, this.GetAttackDirection(position), 7f, 0f, false);
		}

		public void Chasing(Vector2 targetPosition)
		{
			Vector3 chasingTargetPosition = this.GetChasingTargetPosition(targetPosition);
			chasingTargetPosition.y += this.LanceAngel.Behaviour.StartHeightPosition;
			this.LanceAngel.transform.position = Vector3.SmoothDamp(this.LanceAngel.transform.position, chasingTargetPosition, ref this._velocity, this.LanceAngel.Behaviour.ChasingElongation, this.LanceAngel.Behaviour.ChasingSpeed * 0.5f);
		}

		private Vector2 GetPointBelow(Vector2 p, float maxDistance = 2f)
		{
			LayerMask layerMask = this.targetFloorMask;
			if (Physics2D.RaycastNonAlloc(p, Vector2.down, this.results, maxDistance, layerMask) > 0)
			{
				return this.results[0].point;
			}
			GizmoExtensions.DrawDebugCross(p, Color.red, 10f);
			return p;
		}

		private Vector3 GetChasingTargetPosition(Vector3 targetPosition)
		{
			targetPosition = this.GetPointBelow(targetPosition, 3f);
			if (this.LanceAngel.Status.Orientation == EntityOrientation.Left)
			{
				targetPosition.x += this.LanceAngel.Behaviour.TargetOffset.x;
			}
			else
			{
				targetPosition.x -= this.LanceAngel.Behaviour.TargetOffset.x;
			}
			return targetPosition;
		}

		public void Floating()
		{
			this._index += Time.deltaTime;
			float num = this._currentAmplitudeX * Mathf.Sin(this.SpeedX * this._index);
			float num2 = Mathf.Cos(this.SpeedY * this._index) * this._currentAmplitudeY;
			this.LanceAngel.SpriteRenderer.transform.localPosition = new Vector3(num, num2, 0f);
		}

		private void SetRepositionDirection()
		{
			Vector3 one = Vector3.one;
			Vector3 one2 = Vector3.one;
			one2.x = -1f;
			this.LanceAngel.Spline.transform.localScale = ((this.LanceAngel.Status.Orientation != EntityOrientation.Left) ? one : one2);
		}

		private float GetHeight
		{
			get
			{
				return Physics2D.Raycast(base.transform.position, -Vector2.up, 10f, this.BlockLayerMask).distance;
			}
		}

		private Vector3 GetTargetPosition(Vector2 targetOffset)
		{
			Vector3 position = this.LanceAngel.Target.transform.position;
			position.y += targetOffset.y;
			if (this.LanceAngel.Status.Orientation == EntityOrientation.Right)
			{
				position.x += targetOffset.x;
			}
			else
			{
				position.x -= targetOffset.x;
			}
			return new Vector2(position.x, position.y);
		}

		private Vector2 GetAttackDirection(Vector2 target)
		{
			Vector2 vector = Vector2.right;
			if (this.LanceAngel.Target.transform.position.x <= this.LanceAngel.transform.position.x)
			{
				vector *= -1f;
			}
			return vector;
		}

		public void HurtDisplacement()
		{
			Vector3 dir = (this.LanceAngel.Status.Orientation != EntityOrientation.Left) ? (-Vector3.right) : Vector3.right;
			this.LanceAngel.MotionLerper.StartLerping(dir);
		}

		public override void Damage()
		{
			this.HurtDisplacement();
		}

		public override void Parry()
		{
			base.Parry();
			this.LanceAngel.DashAttack.StopDash(this.LanceAngel.transform, true);
			if (this.OnParry != null)
			{
				this.OnParry();
			}
		}

		private void OnLerpStop()
		{
			this.LanceAngel.GhostSprites.EnableGhostTrail = false;
		}

		private void OnLerpStart()
		{
			this.LanceAngel.GhostSprites.EnableGhostTrail = true;
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		private void OnDeath()
		{
			this.Entity.OnDeath -= this.OnDeath;
			this.LanceAngel.StateMachine.enabled = false;
		}

		private void OnDashFinishedEvent()
		{
			this.LanceAngel.IsAttacking = false;
		}

		private void OnDashUpdatedEvent(float value)
		{
			this.LanceAngel.IsAttacking = true;
		}

		private void OnDestroy()
		{
			if (this.LanceAngel == null)
			{
				return;
			}
			MotionLerper motionLerper = this.LanceAngel.MotionLerper;
			motionLerper.OnLerpStart = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStart, new Core.SimpleEvent(this.OnLerpStart));
			MotionLerper motionLerper2 = this.LanceAngel.MotionLerper;
			motionLerper2.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper2.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			this.LanceAngel.DashAttack.OnDashAdvancedEvent -= this.OnDashUpdatedEvent;
			this.LanceAngel.DashAttack.OnDashFinishedEvent -= this.OnDashFinishedEvent;
		}

		public Core.SimpleEvent OnParry;

		private float _index;

		public float StartHeightPosition = 4f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float AmplitudeX = 10f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float AmplitudeY = 5f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float SpeedX = 1f;

		[FoldoutGroup("Floating Motion", true, 0)]
		public float SpeedY = 2f;

		[FoldoutGroup("Attack Motion", true, 0)]
		public float AttackSpeed = 7f;

		private float _minHeight = 1f;

		[FoldoutGroup("Chasing", true, 0)]
		public float ChasingElongation = 0.5f;

		[FoldoutGroup("Chasing", true, 0)]
		public float ChasingSpeed = 5f;

		[FoldoutGroup("Attack", true, 0)]
		public float DistanceAttack = 5f;

		[FoldoutGroup("Attack", true, 0)]
		public AnimationCurve RepositionCurve;

		[FoldoutGroup("Attack", true, 0)]
		public float RepositionTime = 1.5f;

		[FoldoutGroup("Attack", true, 0)]
		public float AttackCooldown = 2f;

		[FoldoutGroup("Attack", true, 0)]
		public Vector2 TargetOffset;

		[FoldoutGroup("Attack", true, 0)]
		public Vector2 RandomTargetOffset;

		[FoldoutGroup("Attack", true, 0)]
		public LayerMask targetFloorMask;

		private float _currentAmplitudeY;

		private float _currentAmplitudeX;

		public Vector3 PathOrigin;

		private Vector3 _velocity = Vector3.zero;

		private RaycastHit2D[] results;
	}
}
