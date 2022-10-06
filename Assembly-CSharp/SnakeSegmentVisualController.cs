using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class SnakeSegmentVisualController : MonoBehaviour
{
	public bool IsIdle
	{
		get
		{
			return this.currentState == SnakeSegmentVisualController.STATES.IDLE;
		}
	}

	public bool IsMovingUp
	{
		get
		{
			return this.currentState == SnakeSegmentVisualController.STATES.MOVING_UP;
		}
	}

	public bool IsMovingDown
	{
		get
		{
			return this.currentState == SnakeSegmentVisualController.STATES.MOVING_DOWN;
		}
	}

	public void Start()
	{
		this.GoToIdle();
	}

	[HideIf("IsIdle", true)]
	[Button(0)]
	public void GoToIdle()
	{
		this.currentState = SnakeSegmentVisualController.STATES.IDLE;
		this.UpdateSnakeSegmentVisually(false);
	}

	[ShowIf("IsIdle", true)]
	[Button(0)]
	public void MoveUp()
	{
		this.currentState = SnakeSegmentVisualController.STATES.MOVING_UP;
		this.UpdateSnakeSegmentVisually(true);
	}

	[ShowIf("IsIdle", true)]
	[Button(0)]
	public void MoveDown()
	{
		this.currentState = SnakeSegmentVisualController.STATES.MOVING_DOWN;
		this.UpdateSnakeSegmentVisually(true);
	}

	private void UpdateSnakeSegmentVisually(bool goesToIdle)
	{
		SnakeSegmentVisualController.SnakeSegmentVisualStateInfo snakeSegmentVisualStateInfo = this.StatesInfo.Find((SnakeSegmentVisualController.SnakeSegmentVisualStateInfo x) => x.State == this.currentState);
		if (Application.isPlaying)
		{
			TweenExtensions.Kill(this.activeTween, false);
			this.activeTween = TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(DOTween.To(delegate(float x)
			{
				this.SpriteRenderer.size = new Vector2(x, this.SpriteRenderer.size.y);
			}, snakeSegmentVisualStateInfo.SpriteWidthAtStart, snakeSegmentVisualStateInfo.SpriteWidthAtEnd, snakeSegmentVisualStateInfo.TweeningTime), snakeSegmentVisualStateInfo.TweeningEase), snakeSegmentVisualStateInfo.TweeningDelay);
			if (goesToIdle)
			{
				TweenSettingsExtensions.OnComplete<Tween>(this.activeTween, delegate()
				{
					this.GoToIdle();
				});
			}
			else if (snakeSegmentVisualStateInfo.ShouldLoopTween)
			{
				TweenSettingsExtensions.SetLoops<Tween>(this.activeTween, snakeSegmentVisualStateInfo.NumLoops, snakeSegmentVisualStateInfo.LoopType);
			}
		}
	}

	public SpriteRenderer SpriteRenderer;

	public List<SnakeSegmentVisualController.SnakeSegmentVisualStateInfo> StatesInfo = new List<SnakeSegmentVisualController.SnakeSegmentVisualStateInfo>();

	private SnakeSegmentVisualController.STATES currentState;

	private Tween activeTween;

	public enum STATES
	{
		IDLE,
		MOVING_UP,
		MOVING_DOWN
	}

	[Serializable]
	public struct SnakeSegmentVisualStateInfo
	{
		public SnakeSegmentVisualController.STATES State;

		public float TweeningDelay;

		public float TweeningTime;

		public Ease TweeningEase;

		public float SpriteWidthAtStart;

		public float SpriteWidthAtEnd;

		public bool ShouldLoopTween;

		[ShowIf("ShouldLoopTween", true)]
		public int NumLoops;

		[ShowIf("ShouldLoopTween", true)]
		public LoopType LoopType;
	}
}
