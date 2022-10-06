using System;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI.Widgets;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	public class CameraPan : MonoBehaviour
	{
		public bool CameraPanReady { get; private set; }

		public Player Rewired { get; private set; }

		public ProCamera2D ProCamera { get; private set; }

		public ProCamera2DNumericBoundaries CameraNumericBoundaries { get; private set; }

		public bool IsRightBounded
		{
			get
			{
				if (!this.CameraNumericBoundaries.UseNumericBoundaries || !this.CameraNumericBoundaries.UseRightBoundary)
				{
					return false;
				}
				float num = Camera.main.ScreenToWorldPoint(Vector3.zero).x + this.ProCamera.ScreenSizeInWorldCoordinates.x;
				return num > this.CameraNumericBoundaries.RightBoundary - 1f;
			}
		}

		public bool IsLeftBounded
		{
			get
			{
				return this.CameraNumericBoundaries.UseNumericBoundaries && this.CameraNumericBoundaries.UseLeftBoundary && Camera.main.ScreenToWorldPoint(Vector3.zero).x < this.CameraNumericBoundaries.LeftBoundary + 1f;
			}
		}

		public bool IsTopBounded
		{
			get
			{
				if (!this.CameraNumericBoundaries.UseNumericBoundaries || !this.CameraNumericBoundaries.UseTopBoundary)
				{
					return false;
				}
				float num = Camera.main.ScreenToWorldPoint(Vector3.zero).y + this.ProCamera.ScreenSizeInWorldCoordinates.y;
				return num > this.CameraNumericBoundaries.TopBoundary - 1f;
			}
		}

		public bool IsBottomBounded
		{
			get
			{
				return this.CameraNumericBoundaries.UseNumericBoundaries && this.CameraNumericBoundaries.UseBottomBoundary && Camera.main.ScreenToWorldPoint(Vector3.zero).y < this.CameraNumericBoundaries.BottomBoundary + 1f;
			}
		}

		private void Start()
		{
			this.Rewired = ReInput.players.GetPlayer(0);
			this._penitent = null;
			this.ProCamera = Core.Logic.CameraManager.ProCamera2D;
			this.CameraNumericBoundaries = Core.Logic.CameraManager.ProCamera2DNumericBoundaries;
			if (this.ProCamera2DPan != null)
			{
				this._defaultCameraSmooth = new Vector2(this.ProCamera2DPan.ProCamera2D.HorizontalFollowSmoothness, this.ProCamera2DPan.ProCamera2D.VerticalFollowSmoothness);
				this.LimitDistance = this.ProCamera2DPan.ProCamera2D.GetComponent<ProCamera2DLimitDistance>();
			}
			else
			{
				Debug.LogError("A Camera Pan Target is Required");
			}
			if (this.CameraPlayerOffset == null)
			{
				Debug.LogError("A Camera Player Offset Component is Required");
			}
			FadeWidget.OnFadeHidedEnd += this.FadeWidgetOnFadeHidedEnd;
			FadeWidget.OnFadeShowEnd += this.DoorOnDoorEnter;
			FadeWidget.OnFadeHidedStart += this.DoorOnDoorExit;
			SpawnManager.OnPlayerSpawn += this.OnPenitentReady;
		}

		private void OnDestroy()
		{
			FadeWidget.OnFadeHidedEnd -= this.FadeWidgetOnFadeHidedEnd;
			FadeWidget.OnFadeShowEnd -= this.DoorOnDoorEnter;
			FadeWidget.OnFadeHidedStart -= this.DoorOnDoorExit;
			SpawnManager.OnPlayerSpawn -= this.OnPenitentReady;
		}

		private void OnPenitentReady(Penitent penitent)
		{
			this._penitent = penitent;
			GameObject gameObject = GameObject.Find("PC2DPanTarget");
			CameraTarget panTarget = new CameraTarget
			{
				TargetTransform = gameObject.transform,
				TargetInfluenceH = 1f,
				TargetInfluenceV = 1f,
				TargetOffset = Vector2.zero
			};
			this._panTarget = panTarget;
		}

		private void FadeWidgetOnFadeHidedEnd()
		{
			if (!this.CameraPanReady)
			{
				this.CameraPanReady = true;
			}
		}

		private void LateUpdate()
		{
			if (!this._penitent)
			{
				return;
			}
			if (!this.CameraPanReady)
			{
				this.ResetCameraPan();
				return;
			}
			if ((Core.Input.InputBlocked && !Core.Input.HasBlocker("PLAYER_LOGIC")) || this._penitent.Status.Dead || !this.EnableCameraPan)
			{
				return;
			}
			this.ProcessCameraPanningInput();
		}

		private void DoorOnDoorEnter()
		{
			this.RemoveCameraPanFromTargets();
		}

		private void DoorOnDoorExit()
		{
		}

		private void ProcessCameraPanningInput()
		{
			if (this._penitent == null)
			{
				return;
			}
			Vector2 zero = Vector2.zero;
			float axis = this.Rewired.GetAxis(20);
			float axis2 = this.Rewired.GetAxis(21);
			zero.x = axis2;
			zero.y = axis;
			float offset = (axis < 0f) ? this.DownWardCameraPanOffset : this.UpWardCameraPanOffset;
			this.SetCameraPan(zero, offset);
		}

		public void RemoveCameraPanFromTargets()
		{
			List<CameraTarget> cameraTargets = this.ProCamera.CameraTargets;
			foreach (CameraTarget cameraTarget in cameraTargets)
			{
				if (cameraTarget.TargetTransform == this.ProCamera2DPan.PanTarget)
				{
					this._panTarget = cameraTarget;
					if (cameraTargets.Contains(cameraTarget))
					{
						cameraTargets.Remove(cameraTarget);
					}
					break;
				}
			}
		}

		public void AddCameraPanToTargets()
		{
			if (this._panTarget == null)
			{
				return;
			}
			this._panTarget.TargetTransform.position = Core.Logic.Penitent.transform.position;
			if (!this.ProCamera.CameraTargets.Contains(this._panTarget))
			{
				this.ProCamera.CameraTargets.Add(this._panTarget);
			}
		}

		private void SetCameraPan(Vector3 joystickPan, float offset)
		{
			if (this.ProCamera2DPan == null || this._penitent == null)
			{
				return;
			}
			Vector3 vector = this.ResolvePaningValueConstraints(new Vector2(joystickPan.x, joystickPan.y));
			if (joystickPan.normalized.sqrMagnitude > 0f && this.EnableCameraPan)
			{
				this.SetPanCameraSmoothness();
				this.EnableLimitDistance(false);
				this._currentVeticalLimitRestoreLapse = 0f;
				Vector3 targetPosition = this.CameraPlayerOffset.PlayerTarget.TargetPosition;
				Vector3 vector2 = targetPosition + vector.normalized * offset;
				this.ProCamera2DPan.PanTarget.position = this.GetPanningDirectionConstraints(vector2);
				this.ProCamera2DPan.CheckTargetisOutsideBounds();
			}
			else
			{
				this.ResetCameraPan();
			}
		}

		private Vector2 GetPanningDirectionConstraints(Vector2 rawPosition)
		{
			Vector2 result = rawPosition;
			if (!this.AllowXPanning)
			{
				result.x = this._penitent.transform.position.x;
			}
			if (!this.AllowYPanning)
			{
				result.y = this._penitent.transform.position.y;
			}
			return result;
		}

		private void ResetCameraPan()
		{
			bool flag = this.IsLeftBounded || this.IsRightBounded;
			bool flag2 = this.IsTopBounded || this.IsBottomBounded;
			if (flag)
			{
				this.ProCamera2DPan.PanTarget.position = new Vector2(this.ProCamera2DPan.PanTarget.position.x, this._penitent.transform.position.y);
			}
			if (flag2)
			{
				this.ProCamera2DPan.PanTarget.position = new Vector2(this._penitent.transform.position.x, this.ProCamera2DPan.PanTarget.position.y);
			}
			if (!flag && !flag2)
			{
				this.ProCamera2DPan.PanTarget.position = this._penitent.transform.position;
			}
			this.SetDefaultProCameraFollowSmoothness();
			this.RestoreVerticalLimit();
		}

		public Vector3 ResolvePaningValueConstraints(Vector2 panValue)
		{
			Vector2 vector;
			vector..ctor(panValue.x, panValue.y);
			if (!this.CameraNumericBoundaries.UseNumericBoundaries)
			{
				return vector;
			}
			if (this.CameraNumericBoundaries.UseLeftBoundary)
			{
				float num = Mathf.Abs(this.CameraNumericBoundaries.LeftBoundary - this._penitent.transform.position.x);
				vector.x = ((num >= 15f) ? vector.x : 0f);
			}
			if (this.CameraNumericBoundaries.UseRightBoundary)
			{
				float num2 = Mathf.Abs(this.CameraNumericBoundaries.RightBoundary - this._penitent.transform.position.x);
				vector.x = ((num2 >= 15f) ? vector.x : 0f);
			}
			if (this.CameraNumericBoundaries.UseBottomBoundary)
			{
				float num3 = Mathf.Abs(this.CameraNumericBoundaries.BottomBoundary - this._penitent.transform.position.y);
				if (num3 < 7.5f)
				{
					vector.y = ((vector.y > 0f) ? vector.y : 0f);
				}
			}
			if (this.CameraNumericBoundaries.UseTopBoundary)
			{
				float num4 = Mathf.Abs(this.CameraNumericBoundaries.TopBoundary - this._penitent.transform.position.y);
				if (num4 < 7.5f)
				{
					vector.y = ((vector.y <= 0f) ? vector.y : 0f);
				}
			}
			return vector;
		}

		private void SetDefaultProCameraFollowSmoothness()
		{
			if (this.ProCamera2DPan == null)
			{
				return;
			}
			this.ProCamera2DPan.ProCamera2D.HorizontalFollowSmoothness = this._defaultCameraSmooth.x;
			this.ProCamera2DPan.ProCamera2D.VerticalFollowSmoothness = this._defaultCameraSmooth.y;
		}

		private void SetPanCameraSmoothness()
		{
			if (this.ProCamera2DPan == null)
			{
				return;
			}
			this.ProCamera2DPan.ProCamera2D.HorizontalFollowSmoothness = this.PanSmooth.x;
			this.ProCamera2DPan.ProCamera2D.VerticalFollowSmoothness = this.PanSmooth.y;
		}

		private void EnableLimitDistance(bool enable)
		{
			this.LimitDistance.LimitVerticalCameraDistance = enable;
		}

		private void RestoreVerticalLimit()
		{
			this._currentVeticalLimitRestoreLapse += Time.deltaTime;
			this.EnableLimitDistance(this._currentVeticalLimitRestoreLapse >= 1f);
		}

		public bool EnableCameraPan;

		public bool AllowXPanning;

		public bool AllowYPanning;

		private float _accumulatedPanningTime;

		public float TimeToKeyoboardPanning = 0.5f;

		private bool _panningCamera;

		private CameraTarget _panTarget;

		private const float HorizontalOffset = 1f;

		private const float VerticalOffset = 1f;

		private const float MinDistanceToHorizontalBoundaries = 15f;

		private const float MinDistanceToVerticalBoundaries = 7.5f;

		private Penitent _penitent;

		public Vector2 PanSmooth;

		private Vector2 _defaultCameraSmooth;

		private float _defaultLeftFocus;

		private float _defaultRightFocus;

		[FoldoutGroup("Component Dependencies", 0)]
		public ProCamera2DPanAndZoom ProCamera2DPan;

		[FoldoutGroup("Component Dependencies", 0)]
		public CameraPlayerOffset CameraPlayerOffset;

		protected ProCamera2DLimitDistance LimitDistance;

		private const float VerticalLimitRestoreLapse = 1f;

		private float _currentVeticalLimitRestoreLapse;

		[FoldoutGroup("OffSet Panning", 0)]
		public float UpWardCameraPanOffset = 3f;

		[FoldoutGroup("OffSet Panning", 0)]
		public float DownWardCameraPanOffset = 3f;
	}
}
