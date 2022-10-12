using System;
using Gameplay.GameControllers.Camera;
using UnityEngine;

[ExecuteInEditMode]
public class PIDI_2DReflection : MonoBehaviour
{
	private void OnEnable()
	{
		if (!this.parallaxInternal)
		{
		}
		if (!this.blitMat)
		{
			this.blitMat = new Material(this.parallaxInternal);
			this.blitMat.hideFlags = HideFlags.DontSave;
		}
		SpriteRenderer component = base.GetComponent<SpriteRenderer>();
		if (component != null)
		{
			component.enabled = true;
		}
	}

	private void Start()
	{
		this.targetPos = base.transform.localPosition;
	}

	private void OnDrawGizmosSelected()
	{
		if (this.isLocalReflection)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(new Vector3(base.transform.position.x - 100f, this.waterSurfaceLine, base.transform.position.z), new Vector3(base.transform.position.x + 100f, this.waterSurfaceLine, base.transform.position.z));
			Gizmos.color = Color.red;
			Gizmos.DrawLine(new Vector3(this.horizontalLimits.x, base.transform.position.y + 100f, base.transform.position.z), new Vector3(this.horizontalLimits.x, base.transform.position.y - 100f, base.transform.position.z));
			Gizmos.DrawLine(new Vector3(this.horizontalLimits.y, base.transform.position.y + 100f, base.transform.position.z), new Vector3(this.horizontalLimits.y, base.transform.position.y - 100f, base.transform.position.z));
			Gizmos.color = Color.white;
		}
	}

	private void LateUpdate()
	{
		if (Application.isPlaying && this.isLocalReflection && this.reflectAsLocal)
		{
			base.transform.localPosition = this.targetPos;
			base.transform.position = new Vector3(Mathf.Clamp(this.reflectAsLocal.position.x, this.horizontalLimits.x, this.horizontalLimits.y), Mathf.Clamp(base.transform.position.y, float.NegativeInfinity, this.waterSurfaceLine), this.reflectAsLocal.position.z);
		}
	}

	private void OnWillRenderObject()
	{
		if (this.srpMode)
		{
			return;
		}
		if (!this.tempCamera)
		{
			this.tempCamera = new GameObject("TempPIDI2D_Camera", new Type[]
			{
				typeof(Camera)
			}).GetComponent<Camera>();
			this.tempCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
			this.tempCamera.enabled = false;
		}
		float num = 1f / (float)this.downScaleValue;
		Camera current = Camera.current;
		if (base.GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection"))
		{
			current = this.foregroundCam;
		}
		if (current.name == "TempPIDI2D_Camera")
		{
			return;
		}
		if (current.GetComponent<CameraManager>() == null)
		{
			return;
		}
		if (!this.rt)
		{
			this.rt = new RenderTexture((int)((float)current.pixelWidth * num), (int)((float)current.pixelHeight * num), 0);
		}
		else if ((int)((float)current.pixelWidth * num) != this.rt.width || (int)((float)current.pixelHeight * num) != this.rt.height)
		{
			UnityEngine.Object.DestroyImmediate(this.rt);
			this.rt = new RenderTexture((int)((float)current.pixelWidth * num), (int)((float)current.pixelHeight * num), 0);
		}
		if (!this.mask)
		{
			this.mask = new RenderTexture((int)((float)current.pixelWidth * num), (int)((float)current.pixelHeight * num), 0);
		}
		else if ((int)((float)current.pixelWidth * num) != this.rt.width || (int)((float)current.pixelHeight * num) != this.rt.height)
		{
			UnityEngine.Object.DestroyImmediate(this.rt);
			this.mask = new RenderTexture((int)((float)current.pixelWidth * num), (int)((float)current.pixelHeight * num), 0);
		}
		if (current)
		{
			this.tempCamera.transform.position = current.transform.position;
			this.tempCamera.transform.rotation = current.transform.rotation;
			if (this.improvedReflection)
			{
				float y = (this.tempCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 10f)) - this.tempCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f, 0f, 10f))).y;
				this.tempCamera.transform.position = new Vector3(this.tempCamera.transform.position.x, base.transform.position.y + y, this.tempCamera.transform.position.z);
				base.GetComponent<Renderer>().sharedMaterial.SetFloat("_BetaReflections", 1f);
			}
			else
			{
				base.GetComponent<Renderer>().sharedMaterial.SetFloat("_BetaReflections", 0f);
			}
			this.tempCamera.orthographic = current.orthographic;
			this.tempCamera.orthographicSize = current.orthographicSize;
			this.tempCamera.fieldOfView = current.fieldOfView;
			this.tempCamera.aspect = current.aspect;
			this.tempCamera.cullingMask = (this.renderLayers & -17);
			this.tempCamera.targetTexture = this.rt;
			this.tempCamera.clearFlags = ((current.clearFlags != CameraClearFlags.Nothing && current.clearFlags != CameraClearFlags.Depth) ? current.clearFlags : CameraClearFlags.Color);
			this.tempCamera.backgroundColor = current.backgroundColor;
			this.tempCamera.backgroundColor = ((!this.alphaBackground && !base.GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection")) ? current.backgroundColor : Color.clear);
			this.tempCamera.allowHDR = current.allowHDR;
			this.tempCamera.allowMSAA = current.allowMSAA;
			if (base.GetComponent<Renderer>().sharedMaterial.HasProperty("_Reflection2D"))
			{
				if (!this.advancedParallax)
				{
					this.tempCamera.Render();
				}
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				base.GetComponent<Renderer>().GetPropertyBlock(materialPropertyBlock);
				if (this.surfaceLevel == -99f)
				{
					this.surfaceLevel = base.GetComponent<Renderer>().sharedMaterial.GetFloat("_SurfaceLevel");
				}
				if (this.refColor == Color.clear)
				{
					this.refColor = base.GetComponent<Renderer>().sharedMaterial.GetColor("_Color");
				}
				if (this.backgroundColor == Color.clear && base.GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection"))
				{
					this.backgroundColor = base.GetComponent<Renderer>().sharedMaterial.GetColor("_ColorB");
				}
				if (!this.advancedParallax)
				{
					materialPropertyBlock.SetTexture("_Reflection2D", this.rt);
				}
				if (!this.advancedParallax && base.GetComponent<Renderer>().sharedMaterial.HasProperty("_ReflectionMask"))
				{
					this.tempCamera.clearFlags = CameraClearFlags.Color;
					this.tempCamera.backgroundColor = Color.clear;
					this.tempCamera.cullingMask = this.drawOverLayers;
					this.tempCamera.targetTexture = this.mask;
					this.tempCamera.transform.position = current.transform.position;
					this.tempCamera.Render();
					materialPropertyBlock.SetTexture("_ReflectionMask", this.mask);
				}
				materialPropertyBlock.SetColor("_Color", this.refColor);
				if (!this.advancedParallax && base.GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection"))
				{
					materialPropertyBlock.SetColor("_ColorB", this.backgroundColor);
					current = this.backgroundCam;
					if (current)
					{
						this.tempCamera.transform.position = current.transform.position;
						this.tempCamera.transform.rotation = current.transform.rotation;
						this.tempCamera.orthographic = current.orthographic;
						this.tempCamera.orthographicSize = current.orthographicSize;
						this.tempCamera.fieldOfView = current.fieldOfView;
						this.tempCamera.aspect = current.aspect;
						this.tempCamera.cullingMask = (this.drawOverLayers & -17);
						this.tempCamera.targetTexture = this.rt;
						this.tempCamera.clearFlags = ((current.clearFlags != CameraClearFlags.Nothing && current.clearFlags != CameraClearFlags.Depth) ? current.clearFlags : CameraClearFlags.Color);
						this.tempCamera.backgroundColor = current.backgroundColor;
						this.tempCamera.backgroundColor = ((!this.alphaBackground) ? current.backgroundColor : Color.clear);
						this.tempCamera.allowHDR = current.allowHDR;
						this.tempCamera.allowMSAA = current.allowMSAA;
						this.tempCamera.targetTexture = this.mask;
						this.tempCamera.transform.position = current.transform.position;
						this.tempCamera.Render();
						materialPropertyBlock.SetTexture("_BackgroundReflection", this.mask);
					}
				}
				if (this.advancedParallax)
				{
					RenderTexture temporary = RenderTexture.GetTemporary(Screen.width, Screen.height);
					RenderTexture temporary2 = RenderTexture.GetTemporary(Screen.width, Screen.height);
					for (int i = 0; i < this.cameras.Length; i++)
					{
						if (this.cameras[i])
						{
							Camera camera = this.cameras[i];
							this.tempCamera.transform.position = camera.transform.position;
							this.tempCamera.transform.rotation = camera.transform.rotation;
							this.tempCamera.orthographic = camera.orthographic;
							this.tempCamera.orthographicSize = camera.orthographicSize;
							this.tempCamera.fieldOfView = camera.fieldOfView;
							this.tempCamera.aspect = camera.aspect;
							if (this.improvedReflection)
							{
								float y2 = (this.tempCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 10f)) - this.tempCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f, 0f, 10f))).y;
								this.tempCamera.transform.position = new Vector3(this.tempCamera.transform.position.x, base.transform.position.y + y2, this.tempCamera.transform.position.z);
								this.blitMat.SetFloat("_BetaReflections", 1f);
							}
							else
							{
								this.blitMat.SetFloat("_BetaReflections", 0f);
							}
							this.tempCamera.cullingMask = (camera.cullingMask & -17);
							this.tempCamera.clearFlags = CameraClearFlags.Skybox;
							this.tempCamera.backgroundColor = ((i <= 0) ? camera.backgroundColor : Color.clear);
							this.tempCamera.depth = camera.depth;
							this.tempCamera.allowHDR = camera.allowHDR;
							this.tempCamera.allowMSAA = camera.allowMSAA;
							this.tempCamera.targetTexture = ((i <= 0) ? temporary : temporary2);
							this.tempCamera.Render();
							this.blitMat.SetTexture("_SecondReflection", (i <= 0) ? temporary : temporary2);
							Graphics.Blit(temporary, temporary, this.blitMat);
						}
					}
					Graphics.Blit(temporary, this.rt);
					RenderTexture.ReleaseTemporary(temporary);
					RenderTexture.ReleaseTemporary(temporary2);
					materialPropertyBlock.SetTexture("_Reflection2D", this.rt);
				}
				materialPropertyBlock.SetFloat("_AlphaBackground", (float)((!this.alphaBackground) ? 1 : 0));
				materialPropertyBlock.SetFloat("_SurfaceLevel", this.surfaceLevel);
				base.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
			}
			this.tempCamera.targetTexture = null;
		}
	}

	public void SetLocalReflectionConfig(LocalReflectionConfig config)
	{
		this.waterSurfaceLine = config.groundLevel;
		this.horizontalLimits.x = config.LeftSideLimit;
		this.horizontalLimits.y = config.RightSideLimit;
	}

	public void SRPUpdate(Camera cam)
	{
		if (!this.tempCamera)
		{
			this.tempCamera = new GameObject("TempPIDI2D_Camera", new Type[]
			{
				typeof(Camera)
			}).GetComponent<Camera>();
			this.tempCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
			this.tempCamera.enabled = false;
		}
		float num = 1f / (float)this.downScaleValue;
		Camera camera = cam;
		if (base.GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection"))
		{
			camera = this.foregroundCam;
		}
		if (!camera || camera.name == "TempPIDI2D_Camera")
		{
			return;
		}
		if (!this.rt)
		{
			this.rt = new RenderTexture((int)((float)camera.pixelWidth * num), (int)((float)camera.pixelHeight * num), 0);
		}
		else if ((int)((float)camera.pixelWidth * num) != this.rt.width || (int)((float)camera.pixelHeight * num) != this.rt.height)
		{
			UnityEngine.Object.DestroyImmediate(this.rt);
			this.rt = new RenderTexture((int)((float)camera.pixelWidth * num), (int)((float)camera.pixelHeight * num), 0);
		}
		if (!this.mask)
		{
			this.mask = new RenderTexture((int)((float)camera.pixelWidth * num), (int)((float)camera.pixelHeight * num), 0);
		}
		else if ((int)((float)camera.pixelWidth * num) != this.rt.width || (int)((float)camera.pixelHeight * num) != this.rt.height)
		{
			UnityEngine.Object.DestroyImmediate(this.rt);
			this.mask = new RenderTexture((int)((float)camera.pixelWidth * num), (int)((float)camera.pixelHeight * num), 0);
		}
		if (camera)
		{
			this.tempCamera.transform.position = camera.transform.position;
			this.tempCamera.transform.rotation = camera.transform.rotation;
			this.tempCamera.orthographic = camera.orthographic;
			this.tempCamera.orthographicSize = camera.orthographicSize;
			this.tempCamera.fieldOfView = camera.fieldOfView;
			this.tempCamera.aspect = camera.aspect;
			this.tempCamera.cullingMask = (this.renderLayers & -17);
			this.tempCamera.targetTexture = this.rt;
			this.tempCamera.clearFlags = ((camera.clearFlags != CameraClearFlags.Nothing && camera.clearFlags != CameraClearFlags.Depth) ? camera.clearFlags : CameraClearFlags.Color);
			this.tempCamera.backgroundColor = camera.backgroundColor;
			this.tempCamera.backgroundColor = ((!this.alphaBackground && !base.GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection")) ? camera.backgroundColor : Color.clear);
			this.tempCamera.allowHDR = camera.allowHDR;
			this.tempCamera.allowMSAA = camera.allowMSAA;
			if (base.GetComponent<Renderer>().sharedMaterial.HasProperty("_Reflection2D"))
			{
				this.tempCamera.Render();
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				base.GetComponent<Renderer>().GetPropertyBlock(materialPropertyBlock);
				if (this.surfaceLevel == -99f)
				{
					this.surfaceLevel = base.GetComponent<Renderer>().sharedMaterial.GetFloat("_SurfaceLevel");
				}
				if (this.refColor == Color.clear)
				{
					this.refColor = base.GetComponent<Renderer>().sharedMaterial.GetColor("_Color");
				}
				if (this.backgroundColor == Color.clear && base.GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection"))
				{
					this.backgroundColor = base.GetComponent<Renderer>().sharedMaterial.GetColor("_ColorB");
				}
				materialPropertyBlock.SetTexture("_Reflection2D", this.rt);
				if (base.GetComponent<Renderer>().sharedMaterial.HasProperty("_ReflectionMask"))
				{
					this.tempCamera.clearFlags = CameraClearFlags.Color;
					this.tempCamera.backgroundColor = Color.clear;
					this.tempCamera.cullingMask = this.drawOverLayers;
					this.tempCamera.targetTexture = this.mask;
					this.tempCamera.transform.position = camera.transform.position;
					this.tempCamera.Render();
					materialPropertyBlock.SetTexture("_ReflectionMask", this.mask);
				}
				materialPropertyBlock.SetColor("_Color", this.refColor);
				if (base.GetComponent<Renderer>().sharedMaterial.HasProperty("_BackgroundReflection"))
				{
					materialPropertyBlock.SetColor("_ColorB", this.backgroundColor);
					camera = this.backgroundCam;
					if (camera)
					{
						this.tempCamera.transform.position = camera.transform.position;
						this.tempCamera.transform.rotation = camera.transform.rotation;
						this.tempCamera.orthographic = camera.orthographic;
						this.tempCamera.orthographicSize = camera.orthographicSize;
						this.tempCamera.fieldOfView = camera.fieldOfView;
						this.tempCamera.aspect = camera.aspect;
						this.tempCamera.cullingMask = (this.drawOverLayers & -17);
						this.tempCamera.targetTexture = this.rt;
						this.tempCamera.clearFlags = ((camera.clearFlags != CameraClearFlags.Nothing && camera.clearFlags != CameraClearFlags.Depth) ? camera.clearFlags : CameraClearFlags.Color);
						this.tempCamera.backgroundColor = camera.backgroundColor;
						this.tempCamera.backgroundColor = ((!this.alphaBackground) ? camera.backgroundColor : Color.clear);
						this.tempCamera.allowHDR = camera.allowHDR;
						this.tempCamera.allowMSAA = camera.allowMSAA;
						this.tempCamera.targetTexture = this.mask;
						this.tempCamera.transform.position = camera.transform.position;
						this.tempCamera.Render();
						materialPropertyBlock.SetTexture("_BackgroundReflection", this.mask);
					}
				}
				materialPropertyBlock.SetFloat("_AlphaBackground", (float)((!this.alphaBackground) ? 1 : 0));
				materialPropertyBlock.SetFloat("_SurfaceLevel", this.surfaceLevel);
				base.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
			}
			this.tempCamera.targetTexture = null;
		}
	}

	private void OnDestroy()
	{
		if (this.rt)
		{
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(this.rt);
			}
			else
			{
				UnityEngine.Object.Destroy(this.rt);
			}
		}
		if (this.tempCamera)
		{
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(this.tempCamera.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(this.tempCamera.gameObject);
			}
		}
	}

	private void OnDisable()
	{
		if (this.rt)
		{
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(this.rt);
			}
			else
			{
				UnityEngine.Object.Destroy(this.rt);
			}
		}
		if (this.tempCamera)
		{
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(this.tempCamera.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(this.tempCamera.gameObject);
			}
		}
		SpriteRenderer component = base.GetComponent<SpriteRenderer>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	[HideInInspector]
	public Camera tempCamera;

	[HideInInspector]
	private Camera secondCam;

	[HideInInspector]
	public RenderTexture rt;

	[HideInInspector]
	public RenderTexture mask;

	public Shader parallaxInternal;

	public bool advancedParallax;

	public Camera[] cameras;

	[Range(1f, 5f)]
	public int downScaleValue = 1;

	public float surfaceLevel = -99f;

	public bool improvedReflection;

	public Color refColor = Color.white;

	public Color backgroundColor = Color.white;

	public LayerMask renderLayers;

	public LayerMask drawOverLayers;

	public bool alphaBackground;

	public bool srpMode;

	public Camera backgroundCam;

	public Camera foregroundCam;

	public bool isLocalReflection;

	public float waterSurfaceLine;

	public Vector2 horizontalLimits;

	public Transform reflectAsLocal;

	private Vector3 targetPos;

	private Material blitMat;

	private Camera cam;
}
