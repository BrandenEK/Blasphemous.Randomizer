using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class PontiffHuskBossAnimationAudioBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.played = false;
		this.stopped = false;
		if (this.eventsController == null)
		{
			this.eventsController = animator.GetComponent<PontiffHuskBossAnimationEventsController>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		int num = Mathf.FloorToInt(stateInfo.normalizedTime);
		if (num > this.lastLoop)
		{
			this.lastLoop = num;
			this.played = false;
			this.stopped = false;
		}
		else
		{
			float num2 = stateInfo.normalizedTime - (float)num;
			if (this.isOneShot)
			{
				if (!this.played && num2 >= this.percentageToPlayEvent)
				{
					this.eventsController.Animation_PlayOneShot(this.eventId);
					this.played = true;
				}
			}
			else
			{
				if (this.playsAudioEvent && !this.played && num2 >= this.percentageToPlayEvent)
				{
					this.eventsController.Animation_PlayAudio(this.eventId);
					this.played = true;
				}
				if (this.stopsAudioEvent && !this.stopped && num2 >= this.percentageToStopEvent)
				{
					this.eventsController.Animation_StopAudio(this.eventId);
					this.stopped = true;
				}
			}
		}
	}

	public string eventId;

	public bool isOneShot = true;

	[ShowIf("isOneShot", true)]
	[Range(0f, 1f)]
	public float percentageToThrowEvent;

	[HideIf("isOneShot", true)]
	public bool playsAudioEvent;

	[ShowIf("playsAudioEvent", true)]
	[Range(0f, 1f)]
	public float percentageToPlayEvent;

	[HideIf("isOneShot", true)]
	public bool stopsAudioEvent;

	[ShowIf("stopsAudioEvent", true)]
	[Range(0f, 1f)]
	public float percentageToStopEvent;

	private PontiffHuskBossAnimationEventsController eventsController;

	private bool played;

	private bool stopped;

	private int lastLoop = -1;
}
