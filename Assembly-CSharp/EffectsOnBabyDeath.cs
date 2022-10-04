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
		flameWall.GetComponent<SpriteRenderer>().DOFade(1f, 0.5f);
		float r = UnityEngine.Random.Range(1.5f, 3f);
		flameWall.DOLocalMoveY(flameWall.localPosition.y + 2f, 4f, false).SetEase(Ease.InQuad).OnComplete(delegate
		{
			flameWall.DOLocalMoveY(flameWall.localPosition.y - 1f, r, false).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
		});
	}

	public void HideFlameWall(Transform flameWall)
	{
		flameWall.DOKill(false);
		flameWall.DOLocalMoveY(flameWall.localPosition.y - 3f, 3f, false).SetEase(Ease.InOutQuad);
		flameWall.GetComponentInParent<Collider2D>().enabled = false;
	}

	public List<Transform> flameWalls;

	public GameObject effectsParent;
}
