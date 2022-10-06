using System;
using DG.Tweening;
using Gameplay.GameControllers.Effects.Entity;
using UnityEngine;

public class AmanecidasShieldFragment : MonoBehaviour
{
	private void Awake()
	{
		this.shieldTransform = base.transform.parent;
		this.spr = base.GetComponentInChildren<SpriteRenderer>();
		this.currentState = AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.SHIELD;
		this.FadeOut(0.2f);
	}

	public void GoToShieldTransform(float delay)
	{
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, this.shieldTransform.position, this.timeToShieldTransform, false), 8), delay), delegate()
		{
			this.OnReachedShield();
		});
		TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DORotate(base.transform, this.shieldTransform.rotation.eulerAngles, this.timeToShieldTransform, 0), 8), delay);
	}

	private void OnReachedShield()
	{
		base.transform.SetParent(this.shieldTransform);
		base.transform.localRotation = Quaternion.identity;
		base.transform.localPosition = Vector3.zero;
		this.currentState = AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.SHIELD;
	}

	public void Flash()
	{
		MasterShaderEffects componentInChildren = base.GetComponentInChildren<MasterShaderEffects>();
		componentInChildren.TriggerColorizeLerpInOut(0.01f, 0.2f);
	}

	public void ColorizeOut(float seconds = 0.2f)
	{
		MasterShaderEffects componentInChildren = base.GetComponentInChildren<MasterShaderEffects>();
		componentInChildren.TriggerColorizeLerp(1f, 0f, seconds, null);
	}

	public void RaiseFromGround(Vector2 originPoint, float timeToRaise)
	{
		this.currentState = AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.RISING;
		base.transform.SetParent(null);
		float num = Random.Range(0.5f, 2f);
		base.transform.position = originPoint;
		this.SetAlpha(0f);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, originPoint.y + num, timeToRaise, false), 10);
		this.FadeIn(0.2f);
	}

	public void BreakFromShield(Vector2 dir)
	{
		this.currentState = AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.BROKEN;
		base.transform.SetParent(null);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, base.transform.position + dir, 0.6f, false), 9);
		this.FadeOut(0.2f);
	}

	public void OnChargeInterrupted()
	{
		this.currentState = AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.BROKEN;
	}

	public void SetAlpha(float a)
	{
		this.spr.color = new Color(this.spr.color.r, this.spr.color.g, this.spr.color.b, a);
	}

	public void FadeOut(float seconds = 0.2f)
	{
		ShortcutExtensions43.DOFade(this.spr, 0f, 0.2f);
	}

	public void BlinkAlpha()
	{
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions43.DOFade(this.spr, 1f, 0.01f), delegate()
		{
			ShortcutExtensions43.DOFade(this.spr, 0f, 0.5f);
		});
	}

	public void FadeIn(float seconds = 0.2f)
	{
		ShortcutExtensions43.DOFade(this.spr, 1f, seconds);
	}

	public Transform shieldTransform;

	public float timeToShieldTransform = 0.5f;

	private SpriteRenderer spr;

	public AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES currentState;

	public enum AMA_SHIELD_FRAGMENT_STATES
	{
		SHIELD,
		BROKEN,
		RISING
	}
}
