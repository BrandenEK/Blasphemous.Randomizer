using System;
using Gameplay.GameControllers.Bosses.PontiffHusk;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

public class PontiffHuskBossAnimationEventsController : MonoBehaviour
{
	public void Animation_DoActivateCollisions(bool act)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.pontiffHuskBossBehaviour.DoActivateCollisions(act);
	}

	public void Animation_PlayOneShot(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.pontiffHuskBossBehaviour.PontiffHuskBoss.Audio.PlayOneShot_AUDIO(eventId, EntityAudio.FxSoundCategory.Attack);
	}

	public void Animation_PlayAudio(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.pontiffHuskBossBehaviour.PontiffHuskBoss.Audio.Play_AUDIO(eventId);
	}

	public void Animation_StopAudio(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.pontiffHuskBossBehaviour.PontiffHuskBoss.Audio.Stop_AUDIO(eventId);
	}

	public void Animation_FlipOrientation()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.pontiffHuskBossBehaviour.FlipOrientation();
	}

	public void Animation_DoActivateGuard(bool act)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.pontiffHuskBossBehaviour.DoActivateGuard(act);
	}

	[SerializeField]
	public PontiffHuskBossBehaviour pontiffHuskBossBehaviour;

	[SerializeField]
	private bool listenEvents;
}
