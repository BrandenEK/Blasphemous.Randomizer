using System;
using System.Diagnostics;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class BoomerangProjectile : StraightProjectile
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event BoomerangProjectile.BoomerangProjectileDelegate OnBackToOrigin;

		public override void Init(Vector3 origin, Vector3 target, float speed)
		{
			base.Init(origin, target, speed);
			this.targetPosition = target;
			this.originPosition = origin;
			this._maxSpeed = speed;
			this._currentPhase = BoomerangProjectile.BOOMERANG_PHASES.TO_TARGET;
			this.reactionChecker.OnProjectileHit += this.OnProjectileHitsBlade;
		}

		private void OnProjectileHitsBlade(ProjectileReaction obj)
		{
			if (this._currentPhase == BoomerangProjectile.BOOMERANG_PHASES.TO_TARGET || this._currentPhase == BoomerangProjectile.BOOMERANG_PHASES.BRAKE)
			{
				this._currentPhase = BoomerangProjectile.BOOMERANG_PHASES.ACCELERATE;
				this._counter = 0f;
				this.velocity = (this.originPosition - base.transform.position).normalized;
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			switch (this._currentPhase)
			{
			case BoomerangProjectile.BOOMERANG_PHASES.TO_TARGET:
				UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + Vector3.up * 0.1f, Color.green, this.debugMarkersDuration);
				this.UpdatePhaseToTarget();
				break;
			case BoomerangProjectile.BOOMERANG_PHASES.BRAKE:
				UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + Vector3.up * 0.1f, Color.yellow, this.debugMarkersDuration);
				this.UpdatePhaseBrake();
				break;
			case BoomerangProjectile.BOOMERANG_PHASES.ACCELERATE:
				UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position - Vector3.up * 0.1f, Color.red, this.debugMarkersDuration);
				this.UpdatePhaseAccelerate();
				break;
			case BoomerangProjectile.BOOMERANG_PHASES.BACK_TO_ORIGIN:
				UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position - Vector3.up * 0.1f, Color.cyan, this.debugMarkersDuration);
				this.UpdatePhaseBackToOrigin();
				break;
			}
		}

		private void UpdatePhaseToTarget()
		{
			this._distanceToTarget = Vector2.Distance(base.transform.position, this.targetPosition);
			this.lastDistanceToTarget = this._distanceToTarget;
			if (this._distanceToTarget < this.brakeDistance)
			{
				this._counter = 0f;
				this._currentPhase = BoomerangProjectile.BOOMERANG_PHASES.BRAKE;
			}
		}

		private void UpdatePhaseBrake()
		{
			float t = this.brakeCurve.Evaluate(this._counter / this.brakeSeconds);
			Vector2 velocity = Vector2.Lerp(this.velocity, Vector2.zero, t);
			this._counter += Time.deltaTime;
			this.velocity = velocity;
			if (this._counter >= this.brakeSeconds)
			{
				this._currentPhase = BoomerangProjectile.BOOMERANG_PHASES.ACCELERATE;
				this._counter = 0f;
				this.velocity = (this.originPosition - base.transform.position).normalized;
			}
		}

		private void UpdatePhaseAccelerate()
		{
			float t = this.accelCurve.Evaluate(this._counter / this.accelerationSeconds);
			Vector2 velocity = Vector2.Lerp(this.velocity, this.velocity.normalized * this._maxSpeed, t);
			this._counter += Time.deltaTime;
			this.velocity = velocity;
			this._distanceToTarget = (base.transform.position - this.originPosition).magnitude;
			if (this._distanceToTarget < this.pickupDistance)
			{
				this.OnReachedOrigin();
			}
			if (this._counter >= this.accelerationSeconds)
			{
				this._currentPhase = BoomerangProjectile.BOOMERANG_PHASES.BACK_TO_ORIGIN;
			}
		}

		private void UpdatePhaseBackToOrigin()
		{
			this._distanceToTarget = (base.transform.position - this.originPosition).magnitude;
			if (this._distanceToTarget < this.pickupDistance)
			{
				this.OnReachedOrigin();
			}
		}

		private void OnReachedOrigin()
		{
			UnityEngine.Debug.Log("BOOMERANG: BACK TO ORIGIN");
			UnityEngine.Debug.DrawLine(base.transform.position - Vector3.up * 0.5f, base.transform.position + Vector3.up * 0.5f, Color.red, this.debugMarkersDuration);
			UnityEngine.Debug.DrawLine(base.transform.position - Vector3.right * 0.5f, base.transform.position + Vector3.right * 0.5f, Color.red, this.debugMarkersDuration);
			if (this.OnBackToOrigin != null)
			{
				this.OnBackToOrigin(this);
			}
		}

		public float brakeDistance = 5f;

		public float brakeSeconds = 0.5f;

		public float accelerationSeconds = 0.5f;

		public float accelerationFactor = 1.2f;

		public float lastDistanceToTarget;

		public AnimationCurve brakeCurve;

		public AnimationCurve accelCurve;

		public ProjectileReaction reactionChecker;

		public BoomerangProjectile.BOOMERANG_PHASES _currentPhase;

		private Vector3 originPosition;

		private Vector3 targetPosition;

		private float _distanceToTarget;

		private float _maxSpeed;

		private float _counter;

		[Header("DEBUG")]
		private float debugMarkersDuration = 3f;

		private float pickupDistance = 0.5f;

		public enum BOOMERANG_PHASES
		{
			TO_TARGET,
			BRAKE,
			ACCELERATE,
			BACK_TO_ORIGIN
		}

		public delegate void BoomerangProjectileDelegate(BoomerangProjectile b);
	}
}
