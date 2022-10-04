using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine;

namespace Framework.Managers
{
	public class ScreenManager : GameSystem
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event ScreenManager.ScreenEvent OnLoad;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event ScreenManager.ScreenEvent OnSettingsChanged;

		public int CurrentWidth
		{
			get
			{
				return this.currentWidth;
			}
		}

		public int CurrentHeight
		{
			get
			{
				return this.currentHeight;
			}
		}

		public override void Initialize()
		{
			LevelManager.OnGenericsElementsLoaded += this.OnGenericsLoaded;
			Log.Trace("VScreen", "Virtual Screen has been initialized.", null);
		}

		public override void Update()
		{
			if (this.currentHeight != Screen.height || this.currentWidth != Screen.width)
			{
				this.FitScreenCamera();
			}
		}

		private void OnGenericsLoaded()
		{
			this.InitializeManager();
		}

		public void InitializeManager()
		{
			GameObject gameObject = GameObject.FindWithTag("VirtualCamera");
			GameObject gameObject2 = GameObject.FindWithTag("MainCamera");
			GameObject gameObject3 = GameObject.FindWithTag("UICamera");
			if (!gameObject || !gameObject2 || !gameObject3)
			{
				Log.Error("VScreen", "Missing components on ScreenManager.", null);
				return;
			}
			this.gameCamera = gameObject2.GetComponent<Camera>();
			this.virtualCamera = gameObject.GetComponent<Camera>();
			this.uiCamera = gameObject3.GetComponent<Camera>();
			if (!this.gameCamera || !this.virtualCamera || !this.uiCamera)
			{
				Log.Error("VScreen", "Missing components on ScreenManager.", null);
				return;
			}
			this.InitializeRenderTexture();
			this.ConfigureGameCamera();
			this.FitScreenCamera();
			if (ScreenManager.OnLoad != null)
			{
				ScreenManager.OnLoad();
			}
			Log.Trace("VScreen", "Virtual camera initialized.", null);
		}

		public bool ResolutionRequireStrategyScale(int width, int height)
		{
			bool result = false;
			float num = (float)width / 640f;
			float num2 = (float)height / 360f;
			float num3;
			if (num < num2)
			{
				num3 = num;
			}
			else
			{
				num3 = num2;
			}
			float num4 = (float)((int)Math.Floor((double)num3));
			if (Math.Abs(num4 - num3) > Mathf.Epsilon)
			{
				result = true;
			}
			return result;
		}

		public void FitScreenCamera()
		{
			float num = (float)Screen.width / 640f;
			float num2 = (float)Screen.height / 360f;
			if (num < num2)
			{
				this.fitScale = num;
				this.fitDimension = Dimensions.Horizontal;
			}
			else
			{
				this.fitScale = num2;
				this.fitDimension = Dimensions.Vertical;
			}
			this.realFitScale = (int)Math.Floor((double)this.fitScale);
			if (Math.Abs((float)this.realFitScale - this.fitScale) > Mathf.Epsilon)
			{
				this.mode = ((UIController.instance.GetScalingStrategy() != OptionsWidget.SCALING_STRATEGY.PIXEL_PERFECT) ? ScalingMode.Scale : ScalingMode.Letterbox);
			}
			float num3 = (this.mode != ScalingMode.Letterbox) ? this.fitScale : ((float)this.realFitScale);
			float orthographicSize = 1f / num3 * ((float)Screen.height / 2f);
			if (this.virtualCamera && this.uiCamera)
			{
				this.virtualCamera.orthographicSize = orthographicSize;
				this.uiCamera.orthographicSize = orthographicSize;
				this.currentHeight = Screen.height;
				this.currentWidth = Screen.width;
			}
		}

		private void InitializeRenderTexture()
		{
			this.gameRenderTexture = new RenderTexture(640, 360, 0)
			{
				name = "Game Texture",
				filterMode = FilterMode.Point,
				depth = 24
			};
			this.gameRenderTexture.Create();
			this.uiRenderTexture = new RenderTexture(640, 360, 0)
			{
				name = "UI Texture",
				filterMode = FilterMode.Point,
				depth = 24
			};
			this.uiRenderTexture.Create();
			this.gameMaterial = Core.Logic.CameraManager.gameQuadRenderer.material;
			this.gameMaterial.SetTexture(Shader.PropertyToID("_MainTex"), this.gameRenderTexture);
			this.uiMaterial = Core.Logic.CameraManager.uiQuadRenderer.material;
			this.uiMaterial.SetTexture(Shader.PropertyToID("_MainTex"), this.uiRenderTexture);
		}

		private void ConfigureGameCamera()
		{
			this.gameCamera.targetTexture = this.gameRenderTexture;
			this.uiCamera.targetTexture = this.uiRenderTexture;
		}

		public Camera VirtualCamera
		{
			get
			{
				return this.virtualCamera;
			}
		}

		public Camera GameCamera
		{
			get
			{
				return this.gameCamera;
			}
		}

		public Camera UICamera
		{
			get
			{
				return this.uiCamera;
			}
		}

		public const int VERTICAL_RESOLUTION = 360;

		public const int HORIZONTAL_RESOLUTION = 640;

		private ScalingMode mode;

		public const float PPU = 32f;

		private RenderTexture gameRenderTexture;

		private RenderTexture uiRenderTexture;

		private Material gameMaterial;

		private Material uiMaterial;

		private Camera gameCamera;

		private Camera virtualCamera;

		private Camera uiCamera;

		private int currentWidth;

		private int currentHeight;

		private float fitScale;

		private int realFitScale;

		private Dimensions fitDimension = Dimensions.Horizontal;

		public delegate void ScreenEvent();
	}
}
