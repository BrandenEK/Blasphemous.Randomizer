using System;
using FMOD.Studio;
using FMODUnity;
using Gameplay.GameControllers.Entities.Audio;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.MiriamPortal.Audio
{
	public class MiriamPortalPrayerAudio : EntityAudio
	{
		public void PlayAttack()
		{
			if (StringExtensions.IsNullOrWhitespace(this.AttackFx))
			{
				return;
			}
			base.AudioManager.PlaySfx(this.AttackFx, 0f);
		}

		public void PlayAppear()
		{
			if (StringExtensions.IsNullOrWhitespace(this.AppearFx))
			{
				return;
			}
			base.AudioManager.PlaySfx(this.AppearFx, 0f);
		}

		public void PlayFollow()
		{
			if (StringExtensions.IsNullOrWhitespace(this.FollowFx))
			{
				return;
			}
			this.StopFollow();
			base.AudioManager.PlayEventNoCatalog(ref this.followEvent, this.FollowFx, default(Vector3));
		}

		public void StopFollow()
		{
			if (!this.followEvent.isValid())
			{
				return;
			}
			this.followEvent.stop(0);
			this.followEvent.release();
		}

		public void PlayVanish()
		{
			if (StringExtensions.IsNullOrWhitespace(this.VanishFx))
			{
				return;
			}
			base.AudioManager.PlaySfx(this.VanishFx, 0f);
		}

		public void PlayTurn()
		{
			if (StringExtensions.IsNullOrWhitespace(this.VanishFx))
			{
				return;
			}
			base.AudioManager.PlaySfx(this.TurnFx, 0f);
		}

		[FoldoutGroup("Audio Events", 0)]
		[SerializeField]
		[EventRef]
		private string AttackFx;

		[FoldoutGroup("Audio Events", 0)]
		[SerializeField]
		[EventRef]
		private string AppearFx;

		[FoldoutGroup("Audio Events", 0)]
		[SerializeField]
		[EventRef]
		private string FollowFx;

		[FoldoutGroup("Audio Events", 0)]
		[SerializeField]
		[EventRef]
		private string VanishFx;

		[FoldoutGroup("Audio Events", 0)]
		[SerializeField]
		[EventRef]
		private string TurnFx;

		private EventInstance followEvent;
	}
}
