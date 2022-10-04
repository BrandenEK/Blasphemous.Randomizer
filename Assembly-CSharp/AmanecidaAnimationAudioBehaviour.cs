using System;
using Gameplay.GameControllers.Entities.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

public class AmanecidaAnimationAudioBehaviour : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.played = false;
		this.stopped = false;
		if (this.amanecidasAnimationEventsController == null)
		{
			this.amanecidasAnimationEventsController = animator.GetComponent<AmanecidasAnimationEventsController>();
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
					bool flag = true;
					if (this.playSfxOnlyInBattlebounds)
					{
						SpriteRenderer spriteRenderer = animator.GetComponent<SpriteRenderer>();
						if (spriteRenderer == null)
						{
							spriteRenderer = animator.GetComponentInChildren<SpriteRenderer>();
						}
						flag = spriteRenderer.isVisible;
					}
					if (flag)
					{
						this.amanecidasAnimationEventsController.AnimationEvent_PlayAnimationOneShot(this.eventId, this.soundCategory);
						this.played = true;
					}
				}
			}
			else
			{
				if (this.playsAudioEvent && !this.played && num2 >= this.percentageToPlayEvent)
				{
					this.amanecidasAnimationEventsController.AnimationEvent_PlayAnimationAudio(this.eventId);
					this.played = true;
				}
				if (this.stopsAudioEvent && !this.stopped && num2 >= this.percentageToStopEvent)
				{
					this.amanecidasAnimationEventsController.AnimationEvent_StopAnimationAudio(this.eventId);
					this.stopped = true;
				}
			}
		}
	}

	public string eventId;

	public bool isOneShot = true;

	[ShowIf("isOneShot", true)]
	public bool playSfxOnlyInBattlebounds = true;

	[ShowIf("isOneShot", true)]
	[Range(0f, 1f)]
	public float percentageToThrowEvent;

	[ShowIf("isOneShot", true)]
	[EnumToggleButtons]
	public EntityAudio.FxSoundCategory soundCategory;

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

	private AmanecidasAnimationEventsController amanecidasAnimationEventsController;

	private bool played;

	private bool stopped;

	private const float maxPercentageThreshold = 0.96f;

	private int lastLoop = -1;
}
