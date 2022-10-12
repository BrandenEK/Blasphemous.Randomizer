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
		base.transform.DOMove(this.shieldTransform.position, this.timeToShieldTransform, false).SetEase(Ease.InCubic).SetDelay(delay).OnComplete(delegate
		{
			this.OnReachedShield();
		});
		base.transform.DORotate(this.shieldTransform.rotation.eulerAngles, this.timeToShieldTransform, RotateMode.Fast).SetEase(Ease.InCubic).SetDelay(delay);
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
		float num = UnityEngine.Random.Range(0.5f, 2f);
		base.transform.position = originPoint;
		this.SetAlpha(0f);
		base.transform.DOMoveY(originPoint.y + num, timeToRaise, false).SetEase(Ease.InOutCubic);
		this.FadeIn(0.2f);
	}

	public void BreakFromShield(Vector2 dir)
	{
		this.currentState = AmanecidasShieldFragment.AMA_SHIELD_FRAGMENT_STATES.BROKEN;
		base.transform.SetParent(null);
		base.transform.DOMove(base.transform.position + dir, 0.6f, false).SetEase(Ease.OutCubic);
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
		this.spr.DOFade(0f, 0.2f);
	}

	public void BlinkAlpha()
	{
		this.spr.DOFade(1f, 0.01f).OnComplete(delegate
		{
			this.spr.DOFade(0f, 0.5f);
		});
	}

	public void FadeIn(float seconds = 0.2f)
	{
		this.spr.DOFade(1f, seconds);
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
