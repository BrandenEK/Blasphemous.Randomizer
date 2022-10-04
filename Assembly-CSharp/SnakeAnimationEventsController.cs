using System;
using Gameplay.GameControllers.Bosses.Snake;
using Gameplay.GameControllers.Entities.Audio;
using UnityEngine;

public class SnakeAnimationEventsController : MonoBehaviour
{
	private bool listenEvents
	{
		get
		{
			return this.AlwaysListensEvents || this.spriteRenderer.IsVisibleFrom(Camera.main);
		}
	}

	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	public void Animation_OnMeleeAttackStarts()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.OnMeleeAttackStarts();
	}

	public void Animation_OnMeleeAttackFinished()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.OnMeleeAttackFinished();
	}

	public void DoActivateCollisionsOpenMouth(bool act)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.DoActivateCollisionsOpenMouth(act);
	}

	public void DoActivateCollisionsIdle(bool act)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.DoActivateCollisionsIdle(act);
	}

	public void Animation_SetWeapon(SnakeBehaviour.SNAKE_WEAPONS weaponToUse)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.SetWeapon(weaponToUse);
	}

	public void Animation_PlaySnakeGrunt1()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlaySnakeGrunt1();
	}

	public void Animation_PlaySnakeGrunt2()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlaySnakeGrunt2();
	}

	public void Animation_PlaySnakeGrunt3()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlaySnakeGrunt3();
	}

	public void Animation_PlaySnakeGrunt4()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlaySnakeGrunt4();
	}

	public void Animation_PlaySnakeBite()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlaySnakeBite();
	}

	public void Animation_PlaySnakeBack()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlaySnakeBack();
	}

	public void Animation_PlaySnakeTongueExplosion()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlaySnakeTongueExplosion();
	}

	public void Animation_PlayDeathStinger()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlaySnakeDeathStinger();
	}

	public void Animation_PlayOneShot(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.PlayOneShot_AUDIO(eventId, EntityAudio.FxSoundCategory.Attack);
	}

	public void Animation_PlayAudio(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.Play_AUDIO(eventId);
	}

	public void Animation_StopAudio(string eventId)
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.Stop_AUDIO(eventId);
	}

	public void Animation_StopAllAudios()
	{
		if (!this.listenEvents)
		{
			return;
		}
		this.SnakeBehaviour.Snake.Audio.StopAll();
	}

	[SerializeField]
	public SnakeBehaviour SnakeBehaviour;

	[SerializeField]
	private bool AlwaysListensEvents;

	private SpriteRenderer spriteRenderer;
}
