using System;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	public class CameraManager : MonoBehaviour
	{
		public ProCamera2D ProCamera2D { get; private set; }

		public ProCamera2DParallax ProCamera2DParallax { get; private set; }

		public ProCamera2DPixelPerfect ProCamera2DPixelPerfect { get; private set; }

		public CameraPlayerOffset CameraPlayerOffset { get; private set; }

		public ProCamera2DNumericBoundaries ProCamera2DNumericBoundaries { get; private set; }

		public ProCamera2DGeometryBoundaries ProCamera2DGeometryBoundaries { get; private set; }

		public ProCamera2DShake ProCamera2DShake { get; private set; }

		public ProCamera2DRails CameraRails { get; private set; }

		public ProCamera2DPanAndZoom PanAndZoom { get; private set; }

		public CameraPan CustomCameraPan { get; set; }

		public ShockwaveManager ShockwaveManager { get; set; }

		public ScreenMaterialEffectsManager ScreenEffectsManager { get; set; }

		public CameraRumbler CameraRumbler { get; set; }

		public CameraTextureHolder TextureHolder { get; set; }

		public bool ZoomActive { get; set; }

		public Player Rewired { get; private set; }

		public CRTEffect CrtEffect { get; private set; }

		private void Awake()
		{
			CameraManager.Instance = this;
			this.ProCamera2D = base.GetComponent<ProCamera2D>();
			this.TextureHolder = base.GetComponent<CameraTextureHolder>();
			this.ProCamera2DParallax = base.GetComponent<ProCamera2DParallax>();
			this.ProCamera2DPixelPerfect = base.GetComponent<ProCamera2DPixelPerfect>();
			this.CameraPlayerOffset = base.GetComponent<CameraPlayerOffset>();
			this.ProCamera2DNumericBoundaries = base.GetComponent<ProCamera2DNumericBoundaries>();
			this.ProCamera2DGeometryBoundaries = base.GetComponent<ProCamera2DGeometryBoundaries>();
			this.ProCamera2DShake = base.GetComponent<ProCamera2DShake>();
			this.CameraRails = base.GetComponent<ProCamera2DRails>();
			this.PanAndZoom = base.GetComponent<ProCamera2DPanAndZoom>();
			this.CustomCameraPan = base.GetComponent<CameraPan>();
			this.ShockwaveManager = base.GetComponent<ShockwaveManager>();
			this.ScreenEffectsManager = base.GetComponent<ScreenMaterialEffectsManager>();
			this.CameraRumbler = base.GetComponent<CameraRumbler>();
			this.CrtEffect = base.GetComponent<CRTEffect>();
		}

		private void OnDestroy()
		{
			base.StopCoroutine("EnableSmoothness");
		}

		public void UpdateNewCameraParams()
		{
			ProCamera2D.Instance.RemoveAllCameraTargets(0f);
			ProCamera2D.Instance.AddCameraTarget(Core.Logic.Penitent.transform, 1f, 1f, 0f, new Vector2(0f, 6f));
			this.ProCamera2D.HorizontalFollowSmoothness = 0f;
			this.ProCamera2D.VerticalFollowSmoothness = 0f;
			Vector3 position = this.ProCamera2D.transform.position;
			Vector3 position2 = Core.Logic.Penitent.transform.position;
			Vector3 position3 = new Vector3(position2.x, position2.y, position.z);
			this.ProCamera2D.transform.position = position3;
			this.Rewired = ReInput.players.GetPlayer(0);
			this.ProCamera2DGeometryBoundaries.enabled = this.LevelHasGeometryBoundaries;
			base.StartCoroutine(this.EnableSmoothness());
		}

		private IEnumerator EnableSmoothness()
		{
			yield return new WaitForSeconds(1f);
			this.ProCamera2D.HorizontalFollowSmoothness = 0.1f;
			this.ProCamera2D.VerticalFollowSmoothness = 0.1f;
			yield break;
		}

		private void Update()
		{
			if (FadeWidget.instance && FadeWidget.instance.FadingIn)
			{
				ProCamera2D.Instance.MoveCameraInstantlyToPosition(ProCamera2D.Instance.CameraTargetPosition);
			}
			if (Core.Logic.CurrentState == LogicStates.Playing)
			{
				this.EnableCameraPan(this.PanningRequired && this.CustomCameraPan.EnableCameraPan);
			}
		}

		public void EnableCameraPan(bool enable)
		{
			if (!this.CustomCameraPan)
			{
				return;
			}
			if (enable)
			{
				if (this.CustomCameraPan.enabled)
				{
					return;
				}
				this.CustomCameraPan.enabled = true;
				this.CustomCameraPan.AddCameraPanToTargets();
			}
			else
			{
				if (!this.CustomCameraPan.enabled)
				{
					return;
				}
				this.CustomCameraPan.ProCamera2DPan.PanTarget.transform.position = Core.Logic.Penitent.transform.position;
				this.CustomCameraPan.RemoveCameraPanFromTargets();
				this.CustomCameraPan.enabled = false;
			}
		}

		public bool PanningRequired
		{
			get
			{
				bool result = false;
				if (!Core.Input.InputBlocked)
				{
					float axisRaw = this.Rewired.GetAxisRaw(20);
					float axisRaw2 = this.Rewired.GetAxisRaw(21);
					result = (Math.Abs(axisRaw) > 0f || Mathf.Abs(axisRaw2) > 0f);
				}
				return result;
			}
		}

		public bool LevelHasGeometryBoundaries
		{
			get
			{
				return Resources.FindObjectsOfTypeAll(typeof(BoxCollider)).Length > 0;
			}
		}

		public const float HorizontalFollowSmoothness = 0.1f;

		public const float VerticalFollowSmoothness = 0.1f;

		public static CameraManager Instance;

		[FoldoutGroup("ScreenManager references", 0)]
		public MeshRenderer uiQuadRenderer;

		[FoldoutGroup("ScreenManager references", 0)]
		public MeshRenderer gameQuadRenderer;
	}
}
