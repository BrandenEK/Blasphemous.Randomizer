using System;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Surrogates;
using DG.Tweening.Plugins.Options;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Gizmos;
using Sirenix.OdinInspector;
using Tools.Level.Layout;
using UnityEngine;

namespace Tools.Level.Interactables
{
	public class FakeExecution : Interactable
	{
		public string ActivationSound
		{
			get
			{
				return this.activationSound;
			}
		}

		public Penitent Penitent { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._rootMotion = base.GetComponentInChildren<RootMotionDriver>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._cameraZoom = Core.Logic.CameraManager.PanAndZoom;
			this._currentLevel = Core.Logic.CurrentLevelConfig;
			this._cameraPlayerOffset = Core.Logic.CameraManager.CameraPlayerOffset;
			this.Penitent = Core.Logic.Penitent;
			base.Invoke("TriggerAwareness", 0.2f);
		}

		public override bool AllwaysShowIcon()
		{
			return false;
		}

		protected override IEnumerator OnUse()
		{
			WaitForEndOfFrame lapse = new WaitForEndOfFrame();
			this.Penitent.PlatformCharacterController.PlatformCharacterPhysics.HSpeed = 0f;
			this.Penitent.PlatformCharacterController.InstantVelocity = Vector3.zero;
			Core.Input.SetBlocker("EXECUTION", true);
			Penitent interactor = this.Penitent;
			this._interactorStartPosition = new Vector2(interactor.transform.position.x, interactor.transform.position.y);
			this.FlipAnimation(base.PlayerDirection);
			this.interactableAnimator.gameObject.SetActive(false);
			this.ShowPlayer(false);
			if (this.PlayOnUse)
			{
				Core.Audio.PlaySfx(this.activationSound, this.soundDelay);
			}
			this.interactorAnimator.SetTrigger("USED");
			if (this.IsCrisanta && Core.InventoryManager.IsTrueSwordHeartEquiped())
			{
				PlayMakerFSM.BroadcastEvent("TRUE ENDING EXECUTION START");
			}
			while (!base.Consumed)
			{
				yield return lapse;
			}
			this.Penitent.transform.position = this.GetRootMotionPosition(base.PlayerDirection);
			UnityEngine.Object.Destroy(base.gameObject);
			this.ShowPlayer(true);
			Core.Input.SetBlocker("EXECUTION", false);
			yield break;
		}

		protected override void OnUpdate()
		{
			if (this.Penitent == null)
			{
				this.Penitent = Core.Logic.Penitent;
				return;
			}
			this._currentTimeThreshold += Time.deltaTime;
			if (base.PlayerInRange && this.Penitent.Status.IsGrounded && !this.Penitent.Status.IsHurt && !base.Consumed && base.InteractionTriggered)
			{
				this.Penitent.IsOnExecution = true;
				GhostTrailGenerator.AreGhostTrailsAllowed = false;
				base.Use();
			}
			if (base.BeingUsed)
			{
				this.PlayerReposition();
			}
		}

		protected override void PlayerReposition()
		{
			this.Penitent.transform.position = this.GetRootMotionPosition(base.PlayerDirection);
		}

		protected override void ShowPlayer(bool show)
		{
			base.ShowPlayer(show);
			this.Penitent.Physics.EnablePhysics(show);
		}

		protected override void InteractionEnd()
		{
			base.Consumed = true;
			Core.Logic.Penitent.IsOnExecution = false;
			GhostTrailGenerator.AreGhostTrailsAllowed = true;
			Hit hit = new Hit
			{
				DamageType = DamageArea.DamageType.Critical
			};
			this.Penitent.IncrementFervour(hit);
			if (this.IsCrisanta && Core.InventoryManager.IsTrueSwordHeartEquiped())
			{
				PlayMakerFSM.BroadcastEvent("TRUE ENDING EXECUTION DONE");
			}
			if (this.InstantiatesSomethingAfterExecution && this.PrefabToInstantiateAfterExecution != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.PrefabToInstantiateAfterExecution, base.transform.position, base.transform.rotation);
				gameObject.transform.localScale = ((this.InstanceOrientation != EntityOrientation.Left) ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f));
			}
		}

		public Vector2 GetRootMotionPosition(EntityOrientation playerDirection)
		{
			Vector3 vector = base.transform.TransformPoint(this._rootMotion.transform.localPosition);
			if (playerDirection == EntityOrientation.Left)
			{
				return new Vector2(vector.x, this._interactorStartPosition.y);
			}
			float num = Math.Abs(base.transform.position.x - vector.x);
			return new Vector2(new Vector2(base.transform.position.x + num, this._interactorStartPosition.y).x, this._interactorStartPosition.y);
		}

		private void FlipAnimation(EntityOrientation direction)
		{
			if (direction == EntityOrientation.Left)
			{
				this.interactorAnimator.transform.position = this.sensors[0].transform.position;
				SpriteRenderer[] componentsInChildren = this.interactorAnimator.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer in componentsInChildren)
				{
					spriteRenderer.flipX = false;
				}
			}
			else
			{
				this.interactorAnimator.transform.position = this.sensors[1].transform.position;
				SpriteRenderer[] componentsInChildren2 = this.interactorAnimator.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer2 in componentsInChildren2)
				{
					spriteRenderer2.flipX = true;
				}
			}
		}

		private void TriggerAwareness()
		{
			if (this.ExecutionAwareness != null)
			{
			}
		}

		public void DoSlowmotion()
		{
			DOTween.To(() => this._currentLevel.TimeScale, delegate(float x)
			{
				this._currentLevel.TimeScale = x;
			}, 0.25f, this._slowMotionTimeLapse).SetUpdate(true).OnComplete(new TweenCallback(this.OnCameraZoomIn)).SetEase(Ease.OutSine).OnStart(new TweenCallback(this.SlowMotionStart)).OnComplete(new TweenCallback(this.SlowMotionCallBack));
		}

		public void StopSlowMotion()
		{
			DOTween.To(() => this._currentLevel.TimeScale, delegate(float x)
			{
				this._currentLevel.TimeScale = x;
			}, 1f, this._slowMotionTimeLapse).SetUpdate(true).OnComplete(new TweenCallback(this.OnCameraZoomIn)).SetEase(Ease.OutSine).OnComplete(new TweenCallback(this.NormalTimeCallback));
		}

		private void SlowMotionStart()
		{
			if (this.OnSlowMotionStart != null)
			{
				this.OnSlowMotionStart();
			}
		}

		private void SlowMotionCallBack()
		{
			if (this.OnSlowMotion != null)
			{
				this.OnSlowMotion();
			}
		}

		private void NormalTimeCallback()
		{
			if (this.OnNormalTime != null)
			{
				this.OnNormalTime();
			}
		}

		public void CameraZoomIn()
		{
			if (this._cameraZoom == null)
			{
				return;
			}
			Core.Logic.CameraManager.ZoomActive = true;
			if (!this._cameraZoom.AllowZoom)
			{
				this._cameraZoom.AllowZoom = true;
			}
			this.CenterCamera();
			DOTween.To(() => this._cameraZoom.ZoomInput, delegate(float x)
			{
				this._cameraZoom.ZoomInput = x;
			}, -this._maxZoomInput, this._zoomTimeLapse).OnStart(new TweenCallback(this.PlayZoomIn)).SetUpdate(true).OnComplete(new TweenCallback(this.OnCameraZoomIn)).SetEase(Ease.OutSine);
		}

		private void OnCameraZoomIn()
		{
			Core.Logic.CameraManager.ProCamera2D.OverallOffset.x = 0f;
			this._cameraPlayerOffset.RestoredXOffset = false;
		}

		public void CameraZoomOut()
		{
			if (this._cameraZoom == null)
			{
				return;
			}
			DOTween.To(() => this._cameraZoom.ZoomInput, delegate(float x)
			{
				this._cameraZoom.ZoomInput = x;
			}, this._maxZoomInput, this._zoomTimeLapse).SetUpdate(true).OnComplete(new TweenCallback(this.OnCameraZoomOut)).SetEase(Ease.OutSine);
		}

		private void OnCameraZoomOut()
		{
			Core.Logic.CameraManager.ZoomActive = false;
			this._cameraZoom.CancelZoom();
			this.CameraFollowTarget(true);
		}

		public void CenterCamera()
		{
			if (this._cameraPlayerOffset == null)
			{
				return;
			}
			ProCamera2D proCamera = Core.Logic.CameraManager.ProCamera2D;
			this._currentCameraPosition = new Vector3(proCamera.transform.position.x, proCamera.transform.position.y, proCamera.transform.position.z);
			Vector3 endValue = new Vector3(Core.Logic.Penitent.transform.position.x, this._currentCameraPosition.y, this._currentCameraPosition.z);
			DOTween.To(() => proCamera.transform.position, delegate(Vector3Wrapper x)
			{
				proCamera.transform.position = x;
			}, endValue, this._zoomTimeLapse).SetUpdate(true).OnStart(delegate
			{
				this.CameraFollowTarget(false);
			}).SetEase(Ease.InSine);
		}

		private void CameraFollowTarget(bool follow)
		{
			Core.Logic.CameraManager.ProCamera2D.FollowHorizontal = follow;
			Core.Logic.CameraManager.ProCamera2D.FollowVertical = follow;
			if (!follow)
			{
				return;
			}
			Core.Logic.CameraManager.ProCamera2D.HorizontalFollowSmoothness = 0.1f;
			Core.Logic.CameraManager.ProCamera2D.VerticalFollowSmoothness = 0.1f;
		}

		public void RestoreCameraOffset()
		{
			if (this._cameraPlayerOffset == null)
			{
				return;
			}
			ProCamera2D proCamera = Core.Logic.CameraManager.ProCamera2D;
			DOTween.To(() => proCamera.transform.position, delegate(Vector3Wrapper x)
			{
				proCamera.transform.position = x;
			}, this._currentCameraPosition, 1f).SetUpdate(true).SetEase(Ease.InSine);
		}

		public void PlayZoomIn()
		{
			Core.Audio.PlaySfx(this.ZoomInFx, 0f);
		}

		public void PlayZoomOut()
		{
			Core.Audio.PlaySfx(this.ZoomOutFx, 0f);
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return false;
		}

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string activationSound;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float soundDelay;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		protected string ZoomInFx;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		protected string ZoomOutFx;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		public bool PlayOnUse;

		[SerializeField]
		[BoxGroup("Visual Effect", true, false, 0)]
		public GameObject ExecutionAwareness;

		[SerializeField]
		[BoxGroup("Special Case", true, false, 0)]
		public bool IsCrisanta = true;

		[SerializeField]
		[BoxGroup("Special Case", true, false, 0)]
		public bool InstantiatesSomethingAfterExecution = true;

		[SerializeField]
		[BoxGroup("Special Case", true, false, 0)]
		[ShowIf("InstantiatesSomethingAfterExecution", true)]
		public GameObject PrefabToInstantiateAfterExecution;

		[HideInInspector]
		public EntityOrientation InstanceOrientation;

		private Vector2 _interactorStartPosition;

		private RootMotionDriver _rootMotion;

		private CameraPlayerOffset _cameraPlayerOffset;

		private Vector3 _currentCameraPosition;

		public float SafeTimeThreshold = 0.5f;

		private float _currentTimeThreshold;

		private ProCamera2DPanAndZoom _cameraZoom;

		private float _maxZoomInput = 1f;

		private float _zoomTimeLapse = 0.35f;

		private float _slowMotionTimeLapse = 0.5f;

		public Core.SimpleEvent OnSlowMotion;

		public Core.SimpleEvent OnSlowMotionStart;

		public Core.SimpleEvent OnNormalTime;

		private LevelInitializer _currentLevel;
	}
}
