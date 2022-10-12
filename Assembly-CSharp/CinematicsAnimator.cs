using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

public class CinematicsAnimator : MonoBehaviour
{
	public void SetTriggerList(List<float> triggerTimes)
	{
		if (triggerTimes.Count == 0)
		{
			return;
		}
		this.animator = base.GetComponent<Animator>();
		base.StartCoroutine(this.WaitForExecuteTriggers(triggerTimes));
	}

	private IEnumerator WaitForExecuteTriggers(List<float> triggerTimes)
	{
		int idx = 0;
		float oldTime = 0f;
		while (idx < triggerTimes.Count)
		{
			float waitTime = triggerTimes[idx] - oldTime;
			Debug.Log(string.Concat(new object[]
			{
				"--- Wait idx:",
				idx.ToString(),
				"  Global:",
				triggerTimes[idx],
				"  Wait:",
				waitTime
			}));
			yield return new WaitForSeconds(waitTime);
			oldTime = triggerTimes[idx];
			this.animator.SetTrigger(this.TriggerName);
			idx++;
		}
		yield break;
	}

	public void OnCinematicsEnd()
	{
		Core.Cinematics.OnCinematicsAnimationEnd();
	}

	[InfoBox("You must add an animation event called OnCinematicsEnd", InfoMessageType.Info, null)]
	public string TriggerName = "NEXT";

	private Animator animator;
}
