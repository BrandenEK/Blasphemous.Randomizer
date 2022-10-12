using System;
using System.Collections;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using Tools.Audio;
using UnityEngine;

namespace Gameplay.UI.Others.Buttons
{
	public class DeadScreenWidget : MonoBehaviour
	{
		private void Awake()
		{
			this.HideAll();
			SpawnManager.OnPlayerSpawn += this.OnPenitentReady;
			this.reverbInstance = default(EventInstance);
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPenitentReady;
		}

		private IEnumerator OnDeadAction()
		{
			DialogWidget dialogWidget = UIController.instance.GetDialog();
			if (dialogWidget != null)
			{
				while (dialogWidget.IsShowingDialog())
				{
					yield return new WaitForSecondsRealtime(0.2f);
				}
			}
			this.StartReverb();
			Core.Input.SetBlocker("DEAD_SCREEN", true);
			UIController.instance.HideBossHealth();
			Core.Audio.PlayOneShot(this.eventSound, default(Vector3));
			yield return new WaitForSecondsRealtime(this.DelayToAnimImage);
			Tweener tween = this.NormalGroup.DOFade(1f, this.FadeImageTime);
			yield return tween.WaitForCompletion();
			yield return new WaitForSecondsRealtime(this.DelayToAnimText);
			tween = this.ContinueGroup.DOFade(1f, this.FadeImageTime);
			yield return tween.WaitForCompletion();
			while (!Input.anyKey)
			{
				yield return null;
			}
			Core.UI.Fade.Fade(true, 1.5f, 0f, delegate
			{
				this.StopReverb();
				this.HideAll();
				Core.Logic.Penitent.Respawn();
			});
			yield break;
		}

		private void OnPenitentReady(Penitent penitent)
		{
			Core.Input.SetBlocker("DEAD_SCREEN", false);
			if (Core.Logic != null && Core.Logic.Penitent != null)
			{
				Penitent penitent2 = Core.Logic.Penitent;
				penitent2.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent2.OnDead, new Core.SimpleEvent(this.OnDead));
			}
		}

		private IEnumerator OnDeadMiriam()
		{
			UIController.instance.HideBossHealth();
			this.StartReverb();
			Core.Audio.PlayOneShot(this.eventSound, default(Vector3));
			yield return new WaitForSeconds(0.5f);
			yield return Core.UI.Fade.FadeCoroutine(new Color(0f, 0f, 0f, 0f), Color.white, 1.5f, true, null);
			this.StopReverb();
			this.HideAll();
			Core.Persistence.RestoreStored();
			Core.SpawnManager.RespawnMiriamSameLevel(true, new Color?(Color.white));
			yield break;
		}

		private void OnDead()
		{
			bool ignoreNextAutomaticRespawn = Core.SpawnManager.IgnoreNextAutomaticRespawn;
			Core.SpawnManager.IgnoreNextAutomaticRespawn = false;
			if (Core.SpawnManager.AutomaticRespawn && !ignoreNextAutomaticRespawn)
			{
				Core.Persistence.RestoreStored();
				Core.SpawnManager.RespawnSafePosition();
			}
			else if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH))
			{
				Singleton<Core>.Instance.StartCoroutine(this.OnDeadActionBossRush());
			}
			else if (Core.Events.AreInMiriamLevel())
			{
				Singleton<Core>.Instance.StartCoroutine(this.OnDeadMiriam());
			}
			else if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
			{
				Singleton<Core>.Instance.StartCoroutine(this.OnDeadDemake());
			}
			else
			{
				Singleton<Core>.Instance.StartCoroutine(this.OnDeadAction());
			}
		}

		private IEnumerator OnDeadActionBossRush()
		{
			this.StartReverb();
			Core.Input.SetBlocker("DEAD_SCREEN", true);
			UIController.instance.HideBossHealth();
			Core.Audio.PlayOneShot(this.eventSound, default(Vector3));
			yield return new WaitForSeconds(1f);
			this.StopReverb();
			UIController.instance.PlayBossRushRankAudio(false);
			yield return Core.UI.Fade.FadeCoroutine(true, 2.5f, 0.5f);
			yield return new WaitForSeconds(2.5f);
			this.HideAll();
			Core.Input.SetBlocker("DEAD_SCREEN", false);
			Core.BossRushManager.EndCourse(false);
			yield break;
		}

		private IEnumerator OnDeadDemake()
		{
			Core.Input.SetBlocker("DEAD_SCREEN", true);
			UIController.instance.HideBossHealth();
			Core.Audio.Ambient.StopCurrent();
			yield return new WaitForSecondsRealtime(this.DelayToAnimImage);
			Core.Audio.Ambient.SetSceneParams(this.eventSoundDemake, string.Empty, new AudioParam[0], string.Empty);
			Tweener tween = this.DemakeGroup.DOFade(1f, this.FadeImageTime);
			yield return tween.WaitForCompletion();
			yield return new WaitForSecondsRealtime(this.DelayToAnimText);
			tween = this.ContinueGroup.DOFade(1f, this.FadeImageTime);
			yield return tween.WaitForCompletion();
			while (!Input.anyKey)
			{
				yield return null;
			}
			Core.UI.Fade.Fade(true, 1f, 0f, delegate
			{
				this.HideAll();
				Core.DemakeManager.EndDemakeRun(false, 0);
			});
			yield break;
		}

		private void StopReverb()
		{
			if (!this.reverbInstance.isValid())
			{
				return;
			}
			this.reverbInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.reverbInstance.release();
			this.reverbInstance = default(EventInstance);
		}

		private void StartReverb()
		{
			if (this.reverbInstance.isValid())
			{
				this.StopReverb();
			}
			this.reverbInstance = Core.Audio.CreateEvent(this.reverbSound, default(Vector3));
			this.reverbInstance.start();
		}

		private void HideAll()
		{
			this.NormalGroup.alpha = 0f;
			this.BossRushGroup.alpha = 0f;
			this.ContinueGroup.alpha = 0f;
			this.DemakeGroup.alpha = 0f;
		}

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string eventSound = "event:/Key Event/Player Death";

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string eventSoundDemake = "event:/Background Layer/DemakeDeathFanfarria";

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string reverbSound = "snapshot:/PlayerDeath";

		[BoxGroup("Controls", true, false, 0)]
		public CanvasGroup NormalGroup;

		[BoxGroup("Controls", true, false, 0)]
		public CanvasGroup BossRushGroup;

		[BoxGroup("Controls", true, false, 0)]
		public CanvasGroup DemakeGroup;

		[BoxGroup("Controls", true, false, 0)]
		public CanvasGroup ContinueGroup;

		[BoxGroup("Anim", true, false, 0)]
		public float DelayToAnimImage = 1f;

		[BoxGroup("Anim", true, false, 0)]
		public float FadeImageTime = 2.15f;

		[BoxGroup("Anim", true, false, 0)]
		public float DelayToAnimText = 1f;

		[BoxGroup("Anim", true, false, 0)]
		public float FadeTextTime = 0.5f;

		private EventInstance reverbInstance;
	}
}
