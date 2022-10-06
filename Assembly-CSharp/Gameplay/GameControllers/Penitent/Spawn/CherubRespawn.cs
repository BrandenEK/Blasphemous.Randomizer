using System;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Pooling;
using Framework.Util;
using Gameplay.GameControllers.Effects.Entity.BlobShadow;
using Gameplay.UI.Widgets;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Spawn
{
	public class CherubRespawn : PoolObject
	{
		public bool PenitentShadowVisible { get; private set; }

		private void Awake()
		{
			this._animator = base.GetComponent<Animator>();
			this._rootRenderer = base.GetComponent<SpriteRenderer>();
			this._cherubsRenderer = base.GetComponentInChildren<SpriteRenderer>();
		}

		private void Start()
		{
			FadeWidget.OnFadeHidedEnd += this.OnFadeIn;
			LogicManager.GoToMainMenu = (Core.SimpleEvent)Delegate.Combine(LogicManager.GoToMainMenu, new Core.SimpleEvent(this.GoToMainMenu));
		}

		private void OnDestroy()
		{
			FadeWidget.OnFadeHidedEnd -= this.OnFadeIn;
		}

		private void Update()
		{
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			if (this._penitentShadow == null && this._penitent)
			{
				this._penitentShadow = this._penitent.Shadow;
			}
			if (!this.PenitentShadowVisible && this._penitentShadow)
			{
				this._penitentShadow.gameObject.SetActive(false);
			}
		}

		private void OnFadeIn()
		{
			if (this._animator == null || Core.Logic.IsMenuScene())
			{
				return;
			}
			base.transform.position = Core.Logic.Penitent.transform.position;
			this._animator.SetTrigger("APPEAR");
			this._rootRenderer.flipX = (Core.Logic.Penitent.GetOrientation() != EntityOrientation.Right);
			this._cherubsRenderer.flipX = (Core.Logic.Penitent.GetOrientation() != EntityOrientation.Right);
			this.PlayRespawnSound();
		}

		private void GoToMainMenu()
		{
			LogicManager.GoToMainMenu = (Core.SimpleEvent)Delegate.Remove(LogicManager.GoToMainMenu, new Core.SimpleEvent(this.GoToMainMenu));
			FadeWidget.OnFadeHidedEnd -= this.OnFadeIn;
			this.StopRespawnSound();
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.SetPlayerVisible(false);
		}

		public void SetPlayerVisible(bool visible = true)
		{
			this._penitent = Object.FindObjectOfType<Penitent>();
			if (this._penitent == null)
			{
				return;
			}
			this._penitent.SpriteRenderer.enabled = visible;
			this.PenitentShadowVisible = false;
		}

		public void Dispose()
		{
			this._penitent = null;
			this._penitentShadow = null;
			Core.Events.SetFlag("CHERUB_RESPAWN", false, false);
			this.SetPlayerVisible(true);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
			string text = string.Empty;
			if (!Core.TutorialManager.IsTutorialUnlocked(this.TutorialFirstDead))
			{
				text = this.TutorialFirstDead;
			}
			else if (!Core.TutorialManager.IsTutorialUnlocked(this.TutorialSecondDead))
			{
				text = this.TutorialSecondDead;
			}
			if (text != string.Empty)
			{
				Singleton<Core>.Instance.StartCoroutine(Core.TutorialManager.ShowTutorial(text, true));
			}
			base.Destroy();
		}

		public void SetShadowVisible()
		{
			if (this._penitentShadow == null)
			{
				return;
			}
			this.PenitentShadowVisible = true;
			this._penitentShadow.gameObject.SetActive(true);
		}

		private void PlayRespawnSound()
		{
			this.StopRespawnSound();
			if (this._soundInstance.isValid())
			{
				return;
			}
			this._soundInstance = Core.Audio.CreateEvent(this.CherubRespawnFx, default(Vector3));
			this._soundInstance.start();
		}

		private void StopRespawnSound()
		{
			if (!this._soundInstance.isValid())
			{
				return;
			}
			this._soundInstance.stop(0);
			this._soundInstance.release();
			this._soundInstance = default(EventInstance);
		}

		private Animator _animator;

		private SpriteRenderer _rootRenderer;

		private SpriteRenderer _cherubsRenderer;

		private Penitent _penitent;

		private BlobShadow _penitentShadow;

		[SerializeField]
		[TutorialId]
		private string TutorialFirstDead;

		[SerializeField]
		[TutorialId]
		private string TutorialSecondDead;

		[EventRef]
		public string CherubRespawnFx;

		private EventInstance _soundInstance;
	}
}
