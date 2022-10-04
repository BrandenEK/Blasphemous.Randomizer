using System;
using Com.LuisPedroFonseca.ProCamera2D;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Interactables
{
	public class MiriamPortal : Interactable
	{
		public override bool IsIgnoringPersistence()
		{
			return true;
		}

		public EntityOrientation Orientation()
		{
			return this.orientation;
		}

		protected override void OnAwake()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		protected override void OnDispose()
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			if (!this._eventrefportalAmbienceSound.isValid())
			{
				return;
			}
			Core.Audio.StopEvent(ref this._eventrefportalAmbienceSound);
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.CurrentLevel = newLevel.LevelName;
			this.IsUsing = false;
			this.IsPortalEnabled = Core.Events.IsMiriamPortalEnabled(this.CurrentLevel);
			if (this.IsPortalEnabled)
			{
				Core.Audio.PlayEventNoCatalog(ref this._eventrefportalAmbienceSound, this.portalAmbienceSound, default(Vector3));
			}
			this.UpdatePortal();
		}

		private void UpdatePortal()
		{
			if (this.IsPortalEnabled)
			{
				this.PortalPreRoot.SetActive(false);
				this.PortalOnRoot.SetActive(true);
				this.PortalOffRoot.SetActive(false);
			}
			else if (Core.Events.IsMiriamQuestStarted)
			{
				this.PortalPreRoot.SetActive(false);
				this.PortalOnRoot.SetActive(false);
				this.PortalOffRoot.SetActive(true);
			}
			else
			{
				this.PortalPreRoot.SetActive(true);
				this.PortalOnRoot.SetActive(false);
				this.PortalOffRoot.SetActive(false);
			}
		}

		protected override void OnUpdate()
		{
			if (Core.Logic != null && Core.Logic.Penitent == null)
			{
				return;
			}
			if (!this.IsUsing && this.IsPortalEnabled && base.PlayerInRange && base.InteractionTriggered && !base.Locked && !Core.Logic.Penitent.IsClimbingLadder)
			{
				this.UsePortal();
			}
			if (base.BeingUsed)
			{
				this.PlayerReposition();
			}
		}

		protected override void InteractionStart()
		{
			this.ShowPlayer(false);
		}

		private void UsePortal()
		{
			this.IsUsing = true;
			this.interactorAnimator.SetTrigger(this.InteractorAnimation);
			Core.Audio.PlayOneShot(this.PortalSound, default(Vector3));
			Core.Input.SetBlocker("DOOR", true);
			FadeWidget.OnFadeShowEnd += this.OnFadeShowEnd;
			FadeWidget.instance.Fade(true, 0.2f, this.EnterDelay, null);
			Core.Logic.Penitent.Status.CastShadow = false;
			Core.Logic.Penitent.DamageArea.DamageAreaCollider.enabled = false;
			Core.Audio.StopEvent(ref this._eventrefportalAmbienceSound);
		}

		private void OnFadeShowEnd()
		{
			ProCamera2D.Instance.HorizontalFollowSmoothness = 0f;
			ProCamera2D.Instance.VerticalFollowSmoothness = 0f;
			FadeWidget.OnFadeShowEnd -= this.OnFadeShowEnd;
			Core.Input.SetBlocker("DOOR", false);
			Core.Events.ActivateMiriamPortalAndTeleport(this.UseFade);
		}

		[BoxGroup("Design Settings", true, false, 0)]
		public float EnterDelay = 2f;

		[BoxGroup("Design Settings", true, false, 0)]
		public bool UseFade = true;

		[BoxGroup("Design Settings", true, false, 0)]
		public string InteractorAnimation = "OPEN_ENTER";

		[BoxGroup("Portal", true, false, 0)]
		public GameObject PortalPreRoot;

		[BoxGroup("Portal", true, false, 0)]
		public GameObject PortalOnRoot;

		[BoxGroup("Portal", true, false, 0)]
		public GameObject PortalOffRoot;

		[BoxGroup("Design Settings", true, false, 0)]
		[EventRef]
		public string PortalSound = string.Empty;

		[BoxGroup("Design Settings", true, false, 0)]
		[EventRef]
		public string portalAmbienceSound = "event:/SFX/Level/MiriamRoomPortal";

		private EventInstance _eventrefportalAmbienceSound;

		private string CurrentLevel = string.Empty;

		private bool IsPortalEnabled;

		private bool IsUsing;
	}
}
