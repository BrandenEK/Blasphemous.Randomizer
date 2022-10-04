using System;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	[RequireComponent(typeof(ProCamera2D))]
	public class CameraPlayerOffset : MonoBehaviour
	{
		public CameraTarget PlayerTarget { get; private set; }

		public Vector2 DefaultTargetOffset { get; private set; }

		public bool ReadyOffset { get; set; }

		public bool RestoredXOffset { get; set; }

		private void Awake()
		{
			this._proCamera2D = base.GetComponent<ProCamera2D>();
			this._geometryBoundaries = base.GetComponent<ProCamera2DGeometryBoundaries>();
			this._penitent = null;
		}

		public void UpdateNewParams()
		{
			this._penitent = Core.Logic.Penitent;
			this._proCamera2D = base.GetComponent<ProCamera2D>();
			this._geometryBoundaries = base.GetComponent<ProCamera2DGeometryBoundaries>();
			this._playerCurrentOrientation = this._penitent.Status.Orientation;
			this._playerLastOrientation = this._playerCurrentOrientation;
			this.SetCameraXOffset(this._playerCurrentOrientation, 0f);
			this.RestoredXOffset = true;
			this.SetCameraTarget(this._proCamera2D.CameraTargets);
			this.DefaultTargetOffset = this._proCamera2D.OverallOffset;
		}

		private void Start()
		{
			this.ReadyOffset = false;
		}

		private void Update()
		{
			if (this.RestoredXOffset)
			{
				return;
			}
			if (this._penitent && this._penitent.Animator.GetCurrentAnimatorStateInfo(0).IsName("Run Start"))
			{
				this.RestoredXOffset = true;
				this.SetCameraXOffset(this._playerCurrentOrientation, this.ElapsedTime);
			}
		}

		private void LateUpdate()
		{
			if (!this._penitent)
			{
				return;
			}
			this._playerCurrentOrientation = this._penitent.Status.Orientation;
			if (this._playerLastOrientation != this._playerCurrentOrientation)
			{
				if (DOTween.IsTweening("ForwardFocus", false))
				{
					DOTween.Kill("ForwardFocus", false);
				}
				this._deltaXOffsetRecoverTime += Time.deltaTime;
				if (this._deltaXOffsetRecoverTime >= this.XOffsetRecoverTime)
				{
					this._playerLastOrientation = this._playerCurrentOrientation;
					this.SetCameraXOffset(this._playerCurrentOrientation, this.ElapsedTime);
					this._deltaXOffsetRecoverTime = 0f;
					this.RestoredXOffset = true;
				}
			}
			else
			{
				this._deltaXOffsetRecoverTime = 0f;
			}
			if (this.IsCameraForwardBlocked())
			{
				CameraPlayerOffset.StopCameraTween();
			}
		}

		private void SetCameraXOffset(EntityOrientation orientation, float elapsedTime)
		{
			if (this.PlayerTarget == null)
			{
				return;
			}
			Vector2 vector = new Vector2(this.XOffset, this.YOffset);
			if (orientation == EntityOrientation.Left)
			{
				vector.x = -this.XOffset;
			}
			DOTween.To(delegate(float x)
			{
				this._proCamera2D.OverallOffset.x = x;
			}, this._proCamera2D.OverallOffset.x, vector.x, elapsedTime).SetEase(Ease.OutSine).SetId("ForwardFocus");
		}

		public static void StopCameraTween()
		{
			if (DOTween.IsTweening("ForwardFocus", false))
			{
				DOTween.Kill("ForwardFocus", false);
			}
		}

		public void SetCameraTarget(IList<CameraTarget> targets)
		{
			byte b = 0;
			while ((int)b < targets.Count)
			{
				if (targets[(int)b].TargetTransform.tag.Equals("Penitent"))
				{
					this.PlayerTarget = targets[(int)b];
					break;
				}
				b += 1;
			}
			if (this.PlayerTarget != null)
			{
				this.PlayerTarget.TargetOffset = Vector2.zero;
			}
			else
			{
				Debug.LogError("NO Player Target Found!");
			}
		}

		private bool IsCameraForwardBlocked()
		{
			bool result = false;
			EntityOrientation orientation = this._penitent.Status.Orientation;
			if (this._proCamera2D.IsCameraPositionLeftBounded && orientation == EntityOrientation.Left)
			{
				result = true;
			}
			else if (this._proCamera2D.IsCameraPositionRightBounded && orientation == EntityOrientation.Right)
			{
				result = true;
			}
			else if (this._geometryBoundaries._cameraMoveInColliderBoundaries.CameraCollisionState.HTopLeft && this._playerCurrentOrientation == EntityOrientation.Left)
			{
				result = true;
			}
			else if (this._geometryBoundaries._cameraMoveInColliderBoundaries.CameraCollisionState.HTopRight && this._playerCurrentOrientation == EntityOrientation.Right)
			{
				result = true;
			}
			return result;
		}

		public const string ForwardFocusTween = "ForwardFocus";

		private Penitent _penitent;

		private float _currentOverallYOffset;

		private float _deltaXOffsetRecoverTime;

		private EntityOrientation _playerLastOrientation;

		private EntityOrientation _playerCurrentOrientation;

		private ProCamera2D _proCamera2D;

		private ProCamera2DGeometryBoundaries _geometryBoundaries;

		[Range(0f, 10f)]
		public float ElapsedTime = 1f;

		[Range(-5f, 5f)]
		public float XOffset = 1.5f;

		public float XOffsetRecoverTime = 0.5f;

		[Range(-10f, 10f)]
		public float YOffset;
	}
}
