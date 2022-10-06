using System;
using System.Diagnostics;
using DG.Tweening;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class HomingProjectile : Projectile
	{
		private Rigidbody2D Rigidbody { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Projectile> OnDisableEvent;

		private void OnEnable()
		{
			base.ResetTTL();
			this.Randomize();
			this.Accelerate();
			this.crossColorHue = Random.Range(0f, 1f);
		}

		private void OnDisable()
		{
			if (this.OnDisableEvent != null)
			{
				this.OnDisableEvent(this);
			}
		}

		public void ResetSpeed()
		{
			this.instanceSpeed = this.Speed;
		}

		public void ResetRotateSpeed()
		{
			this.instanceRotationSpeed = this.RotateSpeed;
		}

		private void Randomize()
		{
			this.instanceRotationSpeed = this.RotateSpeed + Random.Range(-this.rotateSpeedRandomRange, this.rotateSpeedRandomRange);
			this.instanceSpeed = this.Speed + Random.Range(-this.speedRandomRange, this.speedRandomRange);
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Rigidbody = base.GetComponent<Rigidbody2D>();
			this.currentDirection = base.transform.right;
		}

		protected override void OnUpdate()
		{
			this._currentTTL -= Time.deltaTime;
			if (this._currentTTL < 0f)
			{
				base.OnLifeEnded();
			}
			if (this.DestroyedOnReachingTarget && !this.TargetsPenitent && Vector2.Distance(base.transform.position, this.CalculateTargetPosition()) < 0.5f)
			{
				this._currentTTL = 0f;
			}
			if (this.ChangesRotatesSpeedInFlight)
			{
				this.UpdateRotatesSpeedInFlight();
			}
			if (this.ChangesSortingOrderInFlight)
			{
				this.UpdateSortingOrderInFlight();
			}
		}

		public void SetTTL(float ttl)
		{
			this._currentTTL = ttl;
		}

		private void UpdateSortingOrderInFlight()
		{
			bool flag = (this.ChangeOrderWhenHorMoving && this.currentDirection.x > 0f) || (this.ChangeOrderWhenVerMoving && this.currentDirection.y > 0f);
			this.spriteRenderer.sortingOrder = ((!flag) ? this.NegMoveSortingOrder : this.PosMoveSortingOrder);
		}

		private void UpdateRotatesSpeedInFlight()
		{
			Vector2 normalized = this.currentDirection.normalized;
			this.instanceRotationSpeed = this.RotateSpeedWhenHorMoving * Mathf.Abs(normalized.x) + this.RotateSpeedWhenVerMoving * Mathf.Abs(normalized.y);
		}

		protected override void OnFixedUpdated()
		{
			this.HomingDisplacement();
			GizmoExtensions.DrawDebugCross(base.transform.position, Color.HSVToRGB(this.crossColorHue, 1f, 1f), 5f);
		}

		private void HomingDisplacement()
		{
			Vector2 vector = this.CalculateTargetPosition();
			Vector2 normalized = (base.transform.position - vector).normalized;
			float num = Mathf.Sign(Vector3.Cross(normalized, this.velocity).z);
			float num2 = this.instanceRotationSpeed * num * Time.deltaTime;
			Quaternion quaternion = Quaternion.Euler(0f, 0f, num2);
			this.currentDirection = quaternion * this.currentDirection;
			this.velocity = this.currentDirection * this.currentSpeed;
			base.transform.position += this.velocity * Time.deltaTime;
		}

		public Vector2 CalculateTargetPosition()
		{
			Vector2 vector = this.TargetOffset * (1f + this.TargetOffsetFactor * Mathf.Pow(this._currentTTL / this.timeToLive, 5f));
			Vector2 vector2 = (!this.TargetsPenitent) ? this.AlternativeTarget.position : Core.Logic.Penitent.GetPosition();
			return vector2 + vector;
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Vector2 vector = this.CalculateTargetPosition();
				Gizmos.DrawSphere(vector, 0.5f);
				Gizmos.DrawLine(vector, base.transform.position);
				Gizmos.DrawLine(base.transform.position + this.velocity, base.transform.position);
			}
		}

		public bool ChangeTargetToAlternative(Transform transform, float speedFactor, float accelerationTimeFactor, float rotateSpeedFactor)
		{
			if (!this.TargetsPenitent)
			{
				return false;
			}
			this.TargetsPenitent = false;
			this.AlternativeTarget = transform;
			this.AccelerateAlternative(speedFactor, accelerationTimeFactor, rotateSpeedFactor);
			base.ResetTTL();
			return true;
		}

		public bool ChangeTargetToPenitent(bool changeTargetOnlyIfInactive)
		{
			if (this.TargetsPenitent || (changeTargetOnlyIfInactive && base.gameObject.activeInHierarchy))
			{
				return false;
			}
			this.TargetsPenitent = true;
			this.Accelerate();
			base.ResetTTL();
			return true;
		}

		private void Accelerate()
		{
			ShortcutExtensions.DOKill(base.transform, false);
			if (this.currentSpeed == 0f)
			{
				this.currentSpeed = this.instanceSpeed * this.InitialSpeedFactor;
			}
			float num = this.currentSpeed;
			TweenSettingsExtensions.SetEase<Tweener>(DOTween.To(delegate(float x)
			{
				this.currentSpeed = x;
			}, num, this.instanceSpeed, this.Acceleration), this.AccelerationEase);
		}

		private void AccelerateAlternative(float speedFactor, float accelerationTimeFactor, float rorateSpeedFactor)
		{
			ShortcutExtensions.DOKill(base.transform, false);
			this.instanceRotationSpeed = this.RotateSpeed * rorateSpeedFactor;
			if (this.currentSpeed == 0f)
			{
				this.currentSpeed = this.instanceSpeed * this.InitialSpeedFactor;
			}
			float num = this.currentSpeed;
			TweenSettingsExtensions.SetEase<Tweener>(DOTween.To(delegate(float x)
			{
				this.currentSpeed = x;
			}, num, this.instanceSpeed * speedFactor, this.Acceleration * accelerationTimeFactor), this.AccelerationEase);
		}

		[Header("Speed")]
		[FoldoutGroup("Motion Settings", 0)]
		public float Speed;

		[FoldoutGroup("Motion Settings", 0)]
		[Range(0f, 10f)]
		public float speedRandomRange;

		[FoldoutGroup("Motion Settings", 0)]
		[Range(0f, 1f)]
		public float InitialSpeedFactor = 0.5f;

		[Header("Acceleration")]
		[FoldoutGroup("Motion Settings", 0)]
		[Tooltip("Time to reach max speed")]
		public float Acceleration;

		[FoldoutGroup("Motion Settings", 0)]
		public AnimationCurve AccelerationEase;

		[Header("Rotation")]
		[FoldoutGroup("Motion Settings", 0)]
		public float RotateSpeed;

		[FoldoutGroup("Motion Settings", 0)]
		public Vector2 TargetOffset = Vector2.up;

		[FoldoutGroup("Motion Settings", 0)]
		public float TargetOffsetFactor;

		[FoldoutGroup("Motion Settings", 0)]
		[Range(0f, 90f)]
		public float rotateSpeedRandomRange;

		[FoldoutGroup("Motion Settings", 0)]
		public bool TargetsPenitent = true;

		[FoldoutGroup("Motion Settings", 0)]
		public Transform AlternativeTarget;

		[FoldoutGroup("Motion Settings", 0)]
		public bool DestroyedOnReachingTarget = true;

		[FoldoutGroup("Motion Settings", 0)]
		public bool ChangesRotatesSpeedInFlight;

		[FoldoutGroup("Motion Settings", 0)]
		[ShowIf("ChangesRotatesSpeedInFlight", true)]
		public float RotateSpeedWhenHorMoving;

		[FoldoutGroup("Motion Settings", 0)]
		[ShowIf("ChangesRotatesSpeedInFlight", true)]
		public float RotateSpeedWhenVerMoving;

		[FoldoutGroup("Motion Settings", 0)]
		public bool ChangesSortingOrderInFlight;

		[FoldoutGroup("Motion Settings", 0)]
		[ShowIf("ChangesSortingOrderInFlight", true)]
		[HideIf("ChangeOrderWhenVerMoving", true)]
		public bool ChangeOrderWhenHorMoving;

		[FoldoutGroup("Motion Settings", 0)]
		[ShowIf("ChangesSortingOrderInFlight", true)]
		[HideIf("ChangeOrderWhenHorMoving", true)]
		public bool ChangeOrderWhenVerMoving;

		[FoldoutGroup("Motion Settings", 0)]
		[ShowIf("ChangesSortingOrderInFlight", true)]
		public int PosMoveSortingOrder;

		[FoldoutGroup("Motion Settings", 0)]
		[ShowIf("ChangesSortingOrderInFlight", true)]
		public int NegMoveSortingOrder;

		[FoldoutGroup("Debug", 0)]
		public Vector2 currentDirection;

		private float currentSpeed;

		private float instanceSpeed;

		private float instanceRotationSpeed;

		private float crossColorHue;
	}
}
