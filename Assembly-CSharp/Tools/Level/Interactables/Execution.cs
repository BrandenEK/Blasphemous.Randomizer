using System;
using System.Collections;
using System.Collections.Generic;
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
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Gizmos;
using Sirenix.OdinInspector;
using Tools.Level.Layout;
using UnityEngine;

namespace Tools.Level.Interactables
{
	public class Execution : Interactable, IDamageable
	{
		public string ActivationSound
		{
			get
			{
				return this.activationSound;
			}
		}

		public Enemy ExecutedEntity { get; set; }

		public Penitent Penitent { get; private set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this._rootMotion = base.GetComponentInChildren<RootMotionDriver>();
			this.DamageArea = base.GetComponent<EnemyDamageArea>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._cameraZoom = Core.Logic.CameraManager.PanAndZoom;
			this._currentLevel = Core.Logic.CurrentLevelConfig;
			this._cameraPlayerOffset = Core.Logic.CameraManager.CameraPlayerOffset;
			this.Penitent = Core.Logic.Penitent;
			this.DamageArea.SetOwner(this.ExecutedEntity);
			this.SetOverlappedInteractables();
			base.Invoke("TriggerAwareness", 0.2f);
			base.Invoke("ShowTutorial", 0.6f);
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			this.ReleaseOverlappedInteractables();
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
			this.ExecutedEntity.KillEntity();
			if (this.PlayOnUse)
			{
				Core.Audio.PlaySfx(this.activationSound, this.soundDelay);
			}
			this.interactorAnimator.SetTrigger("USED");
			while (!base.Consumed)
			{
				yield return lapse;
			}
			this.AddProgressToAC27();
			this.Penitent.transform.position = this.GetRootMotionPosition(base.PlayerDirection);
			UnityEngine.Object.Destroy(this.ExecutedEntity.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
			this.ShowPlayer(true);
			this.ReleaseOverlappedInteractables();
			Core.Input.SetBlocker("EXECUTION", false);
			yield break;
		}

		private bool PlayerIsOnSameFloor
		{
			get
			{
				RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, -Vector2.up, 3f, this.AllowedFloor);
				RaycastHit2D raycastHit2D2 = Physics2D.Raycast(this.Penitent.GetPosition(), -Vector2.up, 1f, this.AllowedFloor);
				return !(raycastHit2D.collider == null) && !(raycastHit2D2.collider == null) && Mathf.Abs(raycastHit2D.collider.bounds.max.y - raycastHit2D2.collider.bounds.max.y) < 0.1f;
			}
		}

		private void AddProgressToAC27()
		{
			string text = this.ExecutedEntity.GetComponent<EnemyBehaviour>().GetType().ToString();
			string str = text.Split(new char[]
			{
				'.'
			})[text.Split(new char[]
			{
				'.'
			}).Length - 1].Replace("Behaviour", string.Empty).Trim().ToUpper();
			string text2 = str + "_EXECUTED_FOR_AC27";
			Debug.Log("AddProgressToAC27: flagName: " + text2);
			Debug.Log("AddProgressToAC27: Core.Events.GetFlag(flagName): " + Core.Events.GetFlag(text2));
			if (!Core.Events.GetFlag(text2))
			{
				Core.Events.SetFlag(text2, true, true);
				Core.AchievementsManager.Achievements["AC27"].AddProgress(20f);
			}
		}

		protected override void OnUpdate()
		{
			this._currentTimeThreshold += Time.deltaTime;
			this.attackCoolDown -= Time.deltaTime;
			if (this.Penitent.PlatformCharacterInput.Attack)
			{
				this.attackCoolDown = 0.5f;
			}
			if (base.PlayerInRange && this.Penitent.Status.IsGrounded && !this.Penitent.Status.IsHurt && !base.Consumed && base.InteractionTriggered && this.attackCoolDown <= 0f && this.PlayerIsOnSameFloor)
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

		private void ShowTutorial()
		{
			Core.Logic.Penitent.ShowTutorial(this.TutorialID);
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
			this.AddFervourBonus();
			Core.Logic.Penitent.IsOnExecution = false;
			GhostTrailGenerator.AreGhostTrailsAllowed = true;
		}

		private void AddFervourBonus()
		{
			float num = 13.3f * Core.Logic.Penitent.Stats.FervourStrength.Final;
			Core.Logic.Penitent.Stats.Fervour.Current += num;
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

		public void Damage(Hit hit)
		{
			if (this._currentTimeThreshold < this.SafeTimeThreshold || this.Penitent.IsOnExecution)
			{
				return;
			}
			this._currentTimeThreshold = 0f;
			this.ExecutedEntity.Animator.gameObject.SetActive(true);
			this.ExecutedEntity.Animator.enabled = true;
			this.ExecutedEntity.SpriteRenderer.enabled = true;
			this.ExecutedEntity.Animator.Play("Death");
			Core.Audio.EventOneShotPanned(hit.HitSoundId, base.transform.position);
			this.ExecutedEntity.KillEntity();
			this.ReleaseOverlappedInteractables();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private void ReleaseOverlappedInteractables()
		{
			foreach (Interactable interactable in this.overlappedInteractables)
			{
				interactable.OverlappedInteractor = false;
			}
			this.overlappedInteractables.Clear();
		}

		private void SetOverlappedInteractables()
		{
			float num = Vector2.Distance(base.transform.position, Core.Logic.Penitent.GetPosition());
			foreach (Interactable interactable in UnityEngine.Object.FindObjectsOfType<Interactable>())
			{
				if (!interactable.Equals(this))
				{
					if (Vector2.Distance(interactable.transform.position, base.transform.position) < 2.5f)
					{
						bool flag = interactable as Execution != null;
						if (flag)
						{
							float num2 = Vector2.Distance(interactable.transform.position, Core.Logic.Penitent.GetPosition());
							bool flag2 = num2 < num;
							if (!flag2)
							{
								interactable.OverlappedInteractor = true;
								this.overlappedInteractables.Add(interactable);
							}
						}
						else
						{
							interactable.OverlappedInteractor = true;
							this.overlappedInteractables.Add(interactable);
						}
					}
				}
			}
		}

		private void TriggerAwareness()
		{
			if (this.ExecutionAwareness != null)
			{
				Vector2 v = new Vector2(this.ExecutedEntity.transform.position.x, Core.Logic.Penitent.transform.position.y);
				UnityEngine.Object.Instantiate<GameObject>(this.ExecutionAwareness, v, Quaternion.identity);
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
		[BoxGroup("Allowed floor", true, false, 0)]
		public LayerMask AllowedFloor;

		public const float OverlappedInteractorDistanceOffset = 2.5f;

		private Vector2 _interactorStartPosition;

		private RootMotionDriver _rootMotion;

		public EnemyDamageArea DamageArea;

		private CameraPlayerOffset _cameraPlayerOffset;

		private Vector3 _currentCameraPosition;

		public float SafeTimeThreshold = 0.5f;

		private float _currentTimeThreshold;

		private ProCamera2DPanAndZoom _cameraZoom;

		private float _maxZoomInput = 1f;

		private float _zoomTimeLapse = 0.35f;

		private float _slowMotionTimeLapse = 0.5f;

		private float attackCoolDown;

		private const float AttackCooldown = 0.5f;

		public Core.SimpleEvent OnSlowMotion;

		public Core.SimpleEvent OnSlowMotionStart;

		public Core.SimpleEvent OnNormalTime;

		private LevelInitializer _currentLevel;

		[TutorialId]
		public string TutorialID;

		private const int TOTAL_NUMBER_OF_DIFFERENT_EXECUTIONS_FOR_AC27 = 5;

		private List<Interactable> overlappedInteractables = new List<Interactable>();
	}
}
