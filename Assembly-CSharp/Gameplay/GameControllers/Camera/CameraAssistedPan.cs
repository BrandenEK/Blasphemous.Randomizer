using System;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	[RequireComponent(typeof(CircleCollider2D))]
	public class CameraAssistedPan : MonoBehaviour
	{
		public Transform Target { get; private set; }

		private float DistanceToInternalCircle
		{
			get
			{
				float value = Vector2.Distance(base.transform.position, this.Target.position) - this.InternalRadius;
				return Mathf.Clamp(value, 0f, float.PositiveInfinity);
			}
		}

		private float DistanceToExternalCircle
		{
			get
			{
				float value = Vector2.Distance(base.transform.position, this.GetTarget.position) - this.ExternalRadius;
				return Mathf.Clamp(value, 0f, float.PositiveInfinity);
			}
		}

		private void Awake()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		private void OnLevelLoaded(Level oldlevel, Level newlevel)
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			this._cameraPlayerOffset = this._cameraManager.CameraPlayerOffset;
			this.SetCameraPanValue();
		}

		private void Start()
		{
			this._cameraManager = Core.Logic.CameraManager;
			this._proCamera2D = this._cameraManager.ProCamera2D;
		}

		private void LateUpdate()
		{
			if (!this.EnabledAssistedPan || this.GetTarget == null)
			{
				return;
			}
			if (this.DistanceToExternalCircle > 0f)
			{
				return;
			}
			if (this._cameraManager.ZoomActive)
			{
				return;
			}
			float scale = Mathf.Clamp01(this.DistanceToInternalCircle / this.Collider.radius);
			this.CameraPanTransition(scale);
		}

		private void OnDestroy()
		{
			this.Target = null;
			this.SetDefaultCameraTargetOffsetInstant();
		}

		private Transform GetTarget
		{
			get
			{
				if (this.Target == null && Core.Logic.Penitent)
				{
					this.Target = Core.Logic.Penitent.transform;
				}
				return this.Target;
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if ((this.TargetLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.SetCameraPanValue();
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if ((this.TargetLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this.SetDefaultCameraTargetOffset(1f);
		}

		private Vector2 AssistedPanValue(Vector2 defaultOffsetValue)
		{
			Vector2 result = new Vector2(defaultOffsetValue.x, defaultOffsetValue.y);
			switch (this.PanDirection)
			{
			case CameraAssistedPan.InfluenceDirection.Up:
				result.y = this.CameraPanValue;
				break;
			case CameraAssistedPan.InfluenceDirection.Down:
				result.y = -this.CameraPanValue;
				break;
			case CameraAssistedPan.InfluenceDirection.Left:
				result.x = -this.CameraPanValue;
				break;
			case CameraAssistedPan.InfluenceDirection.Right:
				result.x = this.CameraPanValue;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		public void CameraPanTransition(float scale)
		{
			scale = (float)Math.Round((double)scale, 2);
			if (this._cameraPlayerOffset == null)
			{
				return;
			}
			if (this.PanDirection == CameraAssistedPan.InfluenceDirection.Up || this.PanDirection == CameraAssistedPan.InfluenceDirection.Down)
			{
				this._proCamera2D.OverallOffset.y = Mathf.Lerp(this._assistedPanValue.y, this._cameraPlayerOffset.DefaultTargetOffset.y, this.InfluenceCurve.Evaluate(scale));
			}
			else
			{
				this._proCamera2D.OverallOffset.x = Mathf.Lerp(this._assistedPanValue.x, this._cameraPlayerOffset.DefaultTargetOffset.x, this.InfluenceCurve.Evaluate(scale));
			}
		}

		private void SetCameraPanValue()
		{
			if (this._cameraPlayerOffset == null)
			{
				return;
			}
			this._assistedPanValue = this.AssistedPanValue(this._cameraPlayerOffset.DefaultTargetOffset);
		}

		private void SetDefaultCameraTargetOffsetInstant()
		{
			this.KillTweens();
			this._proCamera2D.OverallOffset.x = this._cameraPlayerOffset.DefaultTargetOffset.x;
			this._proCamera2D.OverallOffset.y = this._cameraPlayerOffset.DefaultTargetOffset.y;
		}

		private void SetDefaultCameraTargetOffset(float duration = 1f)
		{
			this.KillTweens();
			if (this.PanDirection == CameraAssistedPan.InfluenceDirection.Up || this.PanDirection == CameraAssistedPan.InfluenceDirection.Down)
			{
				this.currentTweenY = DOTween.To(delegate(float y)
				{
					this._proCamera2D.OverallOffset.y = y;
				}, this._proCamera2D.OverallOffset.y, this._cameraPlayerOffset.DefaultTargetOffset.y, duration).SetEase(Ease.InSine);
			}
			else
			{
				this.currentTweenX = DOTween.To(delegate(float x)
				{
					this._proCamera2D.OverallOffset.x = x;
				}, this._proCamera2D.OverallOffset.x, this._cameraPlayerOffset.DefaultTargetOffset.x, duration).SetEase(Ease.InSine);
			}
		}

		private void KillTweens()
		{
			if (this.currentTweenX != null)
			{
				this.currentTweenX.Kill(false);
			}
			if (this.currentTweenY != null)
			{
				this.currentTweenY.Kill(false);
			}
		}

		private void OnDrawGizmos()
		{
			if (this.InternalRadius >= this.ExternalRadius)
			{
				this.ExternalRadius += 2f;
			}
			this.Collider.radius = this.ExternalRadius;
			this.DrawCircle(this.ExternalRadius, Color.yellow);
			this.DrawCircle(this.InternalRadius, Color.red);
		}

		private void DrawCircle(float radius, Color color)
		{
			Gizmos.color = color;
			float num = 0f;
			float x = radius * Mathf.Cos(num);
			float y = radius * Mathf.Sin(num);
			Vector3 vector = base.transform.position + new Vector3(x, y);
			Vector3 to = vector;
			for (num = 0.1f; num < 6.2831855f; num += 0.1f)
			{
				x = radius * Mathf.Cos(num);
				y = radius * Mathf.Sin(num);
				Vector3 vector2 = base.transform.position + new Vector3(x, y);
				Gizmos.DrawLine(vector, vector2);
				vector = vector2;
			}
			Gizmos.DrawLine(vector, to);
		}

		public CircleCollider2D Collider;

		public LayerMask TargetLayer;

		public AnimationCurve InfluenceCurve;

		public bool EnabledAssistedPan;

		[Range(0f, 10f)]
		public float CameraPanValue = 5f;

		public CameraAssistedPan.InfluenceDirection PanDirection;

		private CameraTarget _playerCameraTarget;

		private ProCamera2D _proCamera2D;

		private CameraPlayerOffset _cameraPlayerOffset;

		private Vector2 _assistedPanValue;

		[Range(0f, 50f)]
		public float ExternalRadius = 4f;

		[Range(1f, 50f)]
		public float InternalRadius = 2f;

		private const float MinInternalOffset = 2f;

		private CameraManager _cameraManager;

		private Tween currentTweenX;

		private Tween currentTweenY;

		public enum InfluenceDirection
		{
			Up,
			Down,
			Left,
			Right
		}
	}
}
