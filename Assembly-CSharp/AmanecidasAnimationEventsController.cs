using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Bosses.Amanecidas;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

public class AmanecidasAnimationEventsController : MonoBehaviour
{
	public void DoActivateCollisions(bool act)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.amanecidasBehaviour.DoActivateCollisions(act);
	}

	public void AnimationEvent_SummonWeapon()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.amanecidasBehaviour.SummonWeapon();
	}

	public void AnimationEvent_ShowWeaponIfBowLaudes()
	{
		if (!this.listenEvents)
		{
			return;
		}
		if (this.amanecidasBehaviour.Amanecidas.IsLaudes && this.amanecidasBehaviour.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW)
		{
			this.amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
		}
	}

	public void AnimationEvent_HideWeaponIfBowLaudes()
	{
		if (!this.listenEvents)
		{
			return;
		}
		if (this.amanecidasBehaviour.Amanecidas.IsLaudes && this.amanecidasBehaviour.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW)
		{
			this.amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
		}
	}

	public void OnTurnAnimationEnded()
	{
		if (!this.listenEvents)
		{
			return;
		}
		Amanecidas amanecidas = this.amanecidasBehaviour.Amanecidas;
		bool orientation = amanecidas.Status.Orientation != EntityOrientation.Right;
		bool flag = !orientation;
		EntityOrientation orientation2 = (!flag) ? EntityOrientation.Right : EntityOrientation.Left;
		amanecidas.SetOrientation(orientation2, true, false);
	}

	public void AnimationEvent_MeleeAttackStart()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.amanecidasBehaviour.OnMeleeAttackStarts();
	}

	public void AnimationEvent_MeleeAttackFinished()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.amanecidasBehaviour.OnMeleeAttackFinished();
	}

	public void AnimationEvent_PlayAnimationOneShot(string eventId, EntityAudio.FxSoundCategory category)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.amanecidasBehaviour.Amanecidas.Audio.PlayOneShot_AUDIO(eventId, category);
	}

	public void AnimationEvent_PlayAnimationAudio(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.amanecidasBehaviour.Amanecidas.Audio.Play_AUDIO(eventId);
	}

	public void AnimationEvent_StopAnimationAudio(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.amanecidasBehaviour.Amanecidas.Audio.Stop_AUDIO(eventId);
	}

	[SerializeField]
	public AmanecidasBehaviour amanecidasBehaviour;

	[SerializeField]
	private bool listenEvents;
}
