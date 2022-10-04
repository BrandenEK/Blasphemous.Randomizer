using System;
using FMODUnity;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Audio
{
	public class DemakeEnemyAudio : MonoBehaviour
	{
		private void Start()
		{
			this.MuteDefaultAudio(true);
		}

		public void PlayAttackDemake()
		{
			this.PlaySfx(this.attackEvent);
		}

		public void PLayJumpDemake()
		{
			this.PlaySfx(this.jumpEvent);
		}

		public void PlayWarningDemake()
		{
			this.PlaySfx(this.warningEvent);
		}

		public void PlayDeadDemake()
		{
			this.PlaySfx(this.deadEvent);
		}

		public void PlayGruntDemake()
		{
			this.PlaySfx(this.gruntEvent);
		}

		public void PlayEffectsDemake()
		{
			this.PlaySfx(this.effectsEvent);
		}

		public void PlayLeftLeg()
		{
			this.PlaySfx(this.stepEvent);
		}

		private void MuteDefaultAudio(bool mute = true)
		{
			Entity componentInParent = base.GetComponentInParent<Entity>();
			if (componentInParent == null)
			{
				return;
			}
			EntityAudio componentInChildren = base.GetComponentInParent<Entity>().GetComponentInChildren<EntityAudio>();
			if (componentInChildren)
			{
				componentInChildren.Mute = mute;
			}
		}

		private void PlaySfx(string eventId)
		{
			if (!string.IsNullOrEmpty(eventId))
			{
				if (this.muteAudioOutsideScreen && this.ownerSprite != null && !this.ownerSprite.isVisible)
				{
					return;
				}
				Core.Audio.PlaySfx(eventId, 0f);
			}
		}

		[SerializeField]
		[EventRef]
		private string attackEvent;

		[SerializeField]
		[EventRef]
		private string jumpEvent;

		[SerializeField]
		[EventRef]
		private string deadEvent;

		[SerializeField]
		[EventRef]
		private string warningEvent;

		[SerializeField]
		[EventRef]
		private string gruntEvent;

		[SerializeField]
		[EventRef]
		private string effectsEvent;

		[SerializeField]
		[EventRef]
		private string stepEvent;

		[SerializeField]
		private SpriteRenderer ownerSprite;

		[SerializeField]
		private bool muteAudioOutsideScreen;
	}
}
