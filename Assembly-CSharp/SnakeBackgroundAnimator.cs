using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class SnakeBackgroundAnimator : MonoBehaviour
{
	private void Awake()
	{
		this.Activate(false);
	}

	public void Activate(bool active)
	{
		this.LerpSpeed((float)((!active) ? 0 : 1), 0.5f);
	}

	public void SetAnimatorsSpeed(float spd)
	{
		this.currentSpeed = spd;
		foreach (Animator animator in this.animators)
		{
			animator.SetFloat("SPEED", this.currentSpeed);
		}
	}

	public float GetAnimatorSpeed()
	{
		return this.currentSpeed;
	}

	public void LerpSpeed(float spd, float seconds = 0.5f)
	{
		if (this.currentTween != null && TweenExtensions.IsPlaying(this.currentTween))
		{
			TweenExtensions.Kill(this.currentTween, false);
		}
		this.currentTween = TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.SetUpdate<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>(this.GetAnimatorSpeed), new DOSetter<float>(this.SetAnimatorsSpeed), spd, seconds), 0, false), 5);
	}

	public List<Animator> animators;

	private float currentSpeed = 1f;

	private Tween currentTween;
}
