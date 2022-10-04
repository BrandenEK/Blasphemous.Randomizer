using System;
using Gameplay.GameControllers.Bosses.Isidora;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

public class IsidoraAnimationEventsController : MonoBehaviour
{
	public void Animation_OnMeleeAttackStarts()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.OnMeleeAttackStarts();
	}

	public void Animation_OnMeleeAttackFinished()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.OnMeleeAttackFinished();
	}

	public void DoActivateCollisions(bool act)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.DoActivateCollisions(act);
	}

	public void Animation_SetWeapon(IsidoraBehaviour.ISIDORA_WEAPONS weaponToUse)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.SetWeapon(weaponToUse);
	}

	public void Animation_CheckFlagAndResetSpeed()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.Isidora.AnimatorInyector.CheckFlagAnimationSpeed();
	}

	public void Animation_PlayOneShot(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.Isidora.Audio.PlayOneShot_AUDIO(eventId, EntityAudio.FxSoundCategory.Attack);
	}

	public void Animation_PlayAudio(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.Isidora.Audio.Play_AUDIO(eventId);
	}

	public void Animation_PlayRisingScytheSlashAudio()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.Isidora.Audio.PlayRisingScytheSlashAudio();
	}

	public void Animation_PlayRisingScytheAnticipationLoopAudio()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.Isidora.Audio.PlayRisingScytheAnticipationLoopAudio();
	}

	public void Animation_StopAudio(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.Isidora.Audio.Stop_AUDIO(eventId);
	}

	public void Animation_StopAllAudios()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.Isidora.Audio.StopAll();
	}

	public void Animation_FlipCollider()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.FlipCurrentWeaponCollider();
	}

	public void Animation_SpawnOrb()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.isidoraBehaviour.SpawnOrb();
	}

	[SerializeField]
	public IsidoraBehaviour isidoraBehaviour;

	[SerializeField]
	private bool listenEvents;
}
