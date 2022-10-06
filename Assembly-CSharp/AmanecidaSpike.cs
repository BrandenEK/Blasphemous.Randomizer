using System;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class AmanecidaSpike : MonoBehaviour
{
	private void ToPlatformLayer()
	{
		base.gameObject.layer = LayerMask.NameToLayer("OneWayDown");
	}

	private void ToFloorLayer()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Floor");
	}

	private void Awake()
	{
		this.renderers = new List<SpriteRenderer>(base.GetComponentsInChildren<SpriteRenderer>());
	}

	[Button("Show spike", 0)]
	public void Show(float timeToShow = 0.2f, float delay = 0f, float heightPercentage = 1f)
	{
		this.initialLocalHeight = base.transform.localPosition.y;
		ShortcutExtensions.DOKill(base.transform, false);
		this.ShowRenderers(true);
		this.currentHeightTarget = Mathf.Lerp(this.minHeight, this.maxHeight, heightPercentage);
		this.currentHeightPercentage = heightPercentage;
		this.ToPlatformLayer();
		Tweener tweener = TweenSettingsExtensions.SetDelay<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.transform, this.initialLocalHeight + this.currentHeightTarget, timeToShow, false), 9), 0.1f + delay);
		tweener.onComplete = new TweenCallback(this.ToFloorLayer);
		tweener.onPlay = new TweenCallback(this.OnShow);
	}

	private void OnShow()
	{
		Core.Audio.PlaySfx(this.appearSound, 0f);
		Vector2 vector = base.transform.position + this.effectOffset;
		vector.y = this.initialLocalHeight - 1f;
		PoolManager.Instance.ReuseObject(this.dustPrefab, vector, Quaternion.identity, false, 1);
	}

	[Button("Hide spike", 0)]
	public void Hide()
	{
		ShortcutExtensions.DOKill(base.transform, false);
		Tweener tweener = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.transform, this.initialLocalHeight, 0.4f, false), 8);
		tweener.onComplete = new TweenCallback(this.OnHide);
	}

	private void ShowRenderers(bool show)
	{
		foreach (SpriteRenderer spriteRenderer in this.renderers)
		{
			spriteRenderer.enabled = show;
		}
	}

	private void OnHide()
	{
		this.ShowRenderers(false);
	}

	private const string PLATFORM_LAYER = "OneWayDown";

	private const string FLOOR_LAYER = "Floor";

	[EventRef]
	public string appearSound;

	public float minHeight = 1f;

	public float maxHeight = 3f;

	private float initialLocalHeight;

	private float currentHeightTarget;

	private float currentHeightPercentage;

	public GameObject dustPrefab;

	public Vector2 effectOffset;

	private List<SpriteRenderer> renderers;
}
