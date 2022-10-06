using System;
using DG.Tweening;
using UnityEngine;

public class FloatInPlace : MonoBehaviour
{
	private void Start()
	{
		Vector2 vector = base.transform.localPosition;
		this.swayTween = TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMoveY(base.transform, vector.y - this.oscillationDistance, this.loopDuration, false), 7), -1, 1);
	}

	private Tween swayTween;

	public float loopDuration;

	public float oscillationDistance;
}
