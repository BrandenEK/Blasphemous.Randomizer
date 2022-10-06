using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake
{
	public class SnakeSegmentsMovementController : MonoBehaviour
	{
		private void Start()
		{
			this.boundaries = Camera.main.GetComponent<ProCamera2DNumericBoundaries>();
			if (this.CurrentStage != SnakeSegmentsMovementController.STAGES.OUT)
			{
				this.UpdateSnakeSegmentsPosition(false, SnakeSegmentsMovementController.STAGES.OUT);
			}
		}

		private void Update()
		{
			if (this.goingUp)
			{
				float x = Core.Logic.Penitent.GetPosition().x;
				float num = Mathf.InverseLerp(this.SnakeBehaviour.SnakeLeftCorner.position.x, this.SnakeBehaviour.SnakeRightCorner.position.x, x);
				float num2 = Mathf.Lerp(0f, 3f, num);
				ProCamera2D.Instance.ApplyInfluence(Vector2.up * num2);
				this.boundaries.UseTopBoundary = false;
			}
			else if (this.goingDown && this.CurrentStage != SnakeSegmentsMovementController.STAGES.FLOOR)
			{
				this.boundaries.UseBottomBoundary = false;
			}
		}

		[Button(0)]
		public void MoveToNextStage()
		{
			switch (this.CurrentStage)
			{
			case SnakeSegmentsMovementController.STAGES.FLOOR:
				this.UpdateSnakeSegmentsPosition(true, SnakeSegmentsMovementController.STAGES.STAGE_ONE);
				break;
			case SnakeSegmentsMovementController.STAGES.STAGE_ONE:
				this.UpdateSnakeSegmentsPosition(true, SnakeSegmentsMovementController.STAGES.STAGE_TWO);
				break;
			case SnakeSegmentsMovementController.STAGES.OUT:
				this.UpdateSnakeSegmentsPosition(true, SnakeSegmentsMovementController.STAGES.FLOOR);
				break;
			}
		}

		[Button(0)]
		public void MoveToPrevStage()
		{
			switch (this.CurrentStage)
			{
			case SnakeSegmentsMovementController.STAGES.FLOOR:
				this.UpdateSnakeSegmentsPosition(false, SnakeSegmentsMovementController.STAGES.OUT);
				break;
			case SnakeSegmentsMovementController.STAGES.STAGE_ONE:
				this.UpdateSnakeSegmentsPosition(false, SnakeSegmentsMovementController.STAGES.FLOOR);
				break;
			case SnakeSegmentsMovementController.STAGES.STAGE_TWO:
				this.UpdateSnakeSegmentsPosition(false, SnakeSegmentsMovementController.STAGES.STAGE_ONE);
				break;
			}
		}

		public void ChangeSegmentsAnim(bool movingUpwards)
		{
			for (int i = 0; i < this.SnakeSegments.Count; i++)
			{
				SnakeSegmentVisualController snakeSegmentVisualController = this.SnakeSegments[i];
				if (movingUpwards)
				{
					snakeSegmentVisualController.MoveUp();
				}
				else
				{
					snakeSegmentVisualController.MoveDown();
				}
			}
		}

		private IEnumerator WaitToSetCamBoundsGoingUp(SnakeSegmentsMovementController.SnakeSegmentsStageInfo targetStageInfo)
		{
			float centerY = Mathf.Lerp(targetStageInfo.CamBounds.TopBoundary, targetStageInfo.CamBounds.BottomBoundary, 0.5f);
			yield return new WaitUntil(() => Camera.main.transform.position.y > centerY);
			targetStageInfo.CamBounds.SetBoundaries();
			this.goingUp = false;
			yield break;
		}

		private IEnumerator WaitToSetCamBoundsGoingDown(SnakeSegmentsMovementController.SnakeSegmentsStageInfo targetStageInfo)
		{
			float centerY = Mathf.Lerp(targetStageInfo.CamBounds.TopBoundary, targetStageInfo.CamBounds.BottomBoundary, 0.5f);
			yield return new WaitUntil(() => Camera.main.transform.position.y < centerY);
			targetStageInfo.CamBounds.SetBoundaries();
			this.goingDown = false;
			yield break;
		}

		private void UpdateSnakeSegmentsPosition(bool movingUpwards, SnakeSegmentsMovementController.STAGES targetStage)
		{
			SnakeSegmentsMovementController.SnakeSegmentsStageInfo targetStageInfo = this.StagesInfo.Find((SnakeSegmentsMovementController.SnakeSegmentsStageInfo x) => x.Stage == targetStage);
			if (Application.isPlaying)
			{
				this.SetCamCoroutine(targetStageInfo, movingUpwards);
			}
			this.UpdateRain(targetStageInfo, movingUpwards);
			for (int i = 0; i < this.SnakeSegments.Count; i++)
			{
				if (Application.isPlaying)
				{
					Tween tween = this.MoveSnakeSegment(targetStageInfo, movingUpwards, i);
					if (i == 0)
					{
						if (targetStage != SnakeSegmentsMovementController.STAGES.FLOOR)
						{
							this.SnakeBehaviour.Snake.SnakeAnimatorInyector.BackgroundAnimationSetSpeed(2f, 1f);
						}
						TweenSettingsExtensions.OnComplete<Tween>(tween, delegate()
						{
							if (this.curCoroutine != null)
							{
								this.StopCoroutine(this.curCoroutine);
							}
							if (targetStage != SnakeSegmentsMovementController.STAGES.FLOOR)
							{
								this.SnakeBehaviour.Snake.SnakeAnimatorInyector.BackgroundAnimationSetSpeed(1f, 1f);
							}
							this.goingUp = false;
							this.goingDown = false;
							targetStageInfo.CamBounds.SetBoundaries();
							this.CurrentStage = targetStage;
							this.SnakeBehaviour.BattleBounds.position = targetStageInfo.BattleBoundsCenterMarker.transform.position;
						});
					}
				}
				else
				{
					float num = targetStageInfo.PositionMarkers[i].transform.position.y - this.SnakeSegments[i].transform.position.y;
					this.SnakeSegments[i].transform.position += Vector3.up * num;
					this.CurrentStage = targetStage;
					this.SnakeBehaviour.BattleBounds.position = targetStageInfo.BattleBoundsCenterMarker.transform.position;
				}
			}
		}

		private Tween MoveSnakeSegment(SnakeSegmentsMovementController.SnakeSegmentsStageInfo targetStageInfo, bool movingUpwards, int i)
		{
			SnakeSegmentVisualController segment = this.SnakeSegments[i];
			Tween tween = TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(segment.transform, targetStageInfo.PositionMarkers[i].transform.position.y, this.TweeningTime, false), this.TweeningEase), targetStageInfo.DelaysToStartMoving[i]);
			if (movingUpwards)
			{
				TweenSettingsExtensions.OnPlay<Tween>(tween, delegate()
				{
					segment.MoveUp();
				});
			}
			else
			{
				TweenSettingsExtensions.OnPlay<Tween>(tween, delegate()
				{
					segment.MoveDown();
				});
			}
			return tween;
		}

		public void InstantSetCamAsStart()
		{
			this.StagesInfo.Find((SnakeSegmentsMovementController.SnakeSegmentsStageInfo x) => x.Stage == SnakeSegmentsMovementController.STAGES.OUT).CamBounds.SetBoundaries();
		}

		private void SetCamCoroutine(SnakeSegmentsMovementController.SnakeSegmentsStageInfo targetStageInfo, bool movingUpwards)
		{
			if (this.curCoroutine != null)
			{
				base.StopCoroutine(this.curCoroutine);
			}
			if (movingUpwards)
			{
				this.goingUp = true;
				this.curCoroutine = base.StartCoroutine(this.WaitToSetCamBoundsGoingUp(targetStageInfo));
			}
			else
			{
				this.goingDown = true;
				this.curCoroutine = base.StartCoroutine(this.WaitToSetCamBoundsGoingDown(targetStageInfo));
			}
		}

		private void UpdateRain(SnakeSegmentsMovementController.SnakeSegmentsStageInfo targetStageInfo, bool movingUpwards)
		{
			Tween tween = TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(this.RainParticlesRoot, targetStageInfo.CamBounds.TopBoundary, this.TweeningTime, false), this.TweeningEase), targetStageInfo.DelaysToStartMoving[0]);
			foreach (SnakeSegmentsMovementController.RainInfo rainInfo in this.RainsInfo)
			{
				ParticleSystem.EmissionModule emission = rainInfo.RainSystem.emission;
				float rateOverTimeMultiplier = emission.rateOverTimeMultiplier;
				Tween tween2 = DOTween.To(() => emission.rateOverTimeMultiplier, delegate(float x)
				{
					emission.rateOverTimeMultiplier = x;
				}, (!movingUpwards) ? (rateOverTimeMultiplier * 0.5f) : (rateOverTimeMultiplier * 2f), this.TweeningTime);
				TweenSettingsExtensions.SetDelay<Tween>(TweenSettingsExtensions.SetEase<Tween>(tween2, this.TweeningEase), targetStageInfo.DelaysToStartMoving[0]);
			}
		}

		public void ResetRain()
		{
			this.RainParticlesRoot.transform.position = new Vector3(this.RainParticlesRoot.transform.position.x, this.StagesInfo[0].CamBounds.TopBoundary, this.RainParticlesRoot.transform.position.z);
			foreach (SnakeSegmentsMovementController.RainInfo rainInfo in this.RainsInfo)
			{
				rainInfo.RainSystem.emission.rateOverTimeMultiplier = rainInfo.BaseRate;
			}
		}

		public SnakeBehaviour SnakeBehaviour;

		public List<SnakeSegmentVisualController> SnakeSegments = new List<SnakeSegmentVisualController>();

		public List<SnakeSegmentsMovementController.SnakeSegmentsStageInfo> StagesInfo = new List<SnakeSegmentsMovementController.SnakeSegmentsStageInfo>();

		public float TweeningTime = 1f;

		public Ease TweeningEase = 5;

		public SnakeSegmentsMovementController.STAGES CurrentStage = SnakeSegmentsMovementController.STAGES.OUT;

		public List<SnakeSegmentsMovementController.RainInfo> RainsInfo = new List<SnakeSegmentsMovementController.RainInfo>();

		public Transform RainParticlesRoot;

		private ProCamera2DNumericBoundaries boundaries;

		private bool goingUp;

		private bool goingDown;

		private Coroutine curCoroutine;

		public enum STAGES
		{
			OUT = 4,
			FLOOR = 0,
			STAGE_ONE,
			STAGE_TWO
		}

		[Serializable]
		public struct SnakeSegmentsStageInfo
		{
			public SnakeSegmentsMovementController.STAGES Stage;

			public CameraNumericBoundaries CamBounds;

			public List<GameObject> PositionMarkers;

			public GameObject BattleBoundsCenterMarker;

			public List<float> DelaysToStartMoving;
		}

		[Serializable]
		public struct RainInfo
		{
			public ParticleSystem RainSystem;

			public float BaseRate;
		}
	}
}
