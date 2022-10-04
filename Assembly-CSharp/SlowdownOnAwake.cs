using System;
using Framework.Managers;
using UnityEngine;

public class SlowdownOnAwake : MonoBehaviour
{
	private void Awake()
	{
		Core.Logic.ScreenFreeze.Freeze(0.1f, this.duration, 0f, this.slowTimeCurve);
	}

	public AnimationCurve slowTimeCurve;

	public float duration = 2f;
}
