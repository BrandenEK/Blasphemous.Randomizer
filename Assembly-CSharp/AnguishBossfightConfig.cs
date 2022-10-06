using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.GameControllers.Bosses.Quirce;
using UnityEngine;

public class AnguishBossfightConfig : MonoBehaviour
{
	public void ShowFlameWall()
	{
		TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(this.flameWall, this.flameWall.localPosition.y + 2.5f, 4f, false), 5), delegate()
		{
			TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(this.flameWall, this.flameWall.localPosition.y - 1f, 2f, false), 7), -1, 1);
		});
	}

	public void HideFlameWall()
	{
		ShortcutExtensions.DOKill(this.flameWall, false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(this.flameWall, this.flameWall.localPosition.y - 3f, 3f, false), 7);
		this.flameWall.GetComponentInParent<Collider2D>().enabled = false;
	}

	public Transform GetRandomBeamPoint()
	{
		return this.beamPoints[Random.Range(0, this.beamPoints.Count)];
	}

	public Transform GetDifferentBeamTransform(Transform currentTransform)
	{
		Transform transform = currentTransform;
		while (transform == currentTransform)
		{
			transform = this.GetRandomBeamPoint();
		}
		return transform;
	}

	public SplinePointInfo GetMaceSplineInfo()
	{
		return this.maceSpline;
	}

	public List<Transform> GetSpearPoints()
	{
		if (Random.Range(0f, 1f) > 0.5f)
		{
			return this.spearComboPointsL;
		}
		return this.spearComboPointsR;
	}

	public SplinePointInfo maceSpline;

	public List<Transform> beamPoints;

	public List<Transform> spearComboPointsL;

	public List<Transform> spearComboPointsR;

	public Transform flameWall;
}
