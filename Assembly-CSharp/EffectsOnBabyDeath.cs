using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EffectsOnBabyDeath : MonoBehaviour
{
	public void ActivateEffects()
	{
		this.effectsParent.SetActive(true);
		foreach (Transform flameWall in this.flameWalls)
		{
			this.ShowFlameWall(flameWall);
		}
	}

	public void ShowFlameWall(Transform flameWall)
	{
		ShortcutExtensions43.DOFade(flameWall.GetComponent<SpriteRenderer>(), 1f, 0.5f);
		float r = Random.Range(1.5f, 3f);
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(flameWall, flameWall.localPosition.y + 2f, 4f, false), 5), delegate()
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(flameWall, flameWall.localPosition.y - 1f, r, false), 7), -1, 1);
		});
	}

	public void HideFlameWall(Transform flameWall)
	{
		ShortcutExtensions.DOKill(flameWall, false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(flameWall, flameWall.localPosition.y - 3f, 3f, false), 7);
		flameWall.GetComponentInParent<Collider2D>().enabled = false;
	}

	public List<Transform> flameWalls;

	public GameObject effectsParent;
}
