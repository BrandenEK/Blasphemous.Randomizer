using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using Framework.FrameworkCore;
using Framework.Util;
using Gameplay.UI;
using I2.Loc;
using Rewired;
using Tools.Audio;
using Tools.DataContainer;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Framework.Managers
{
	public class Cinematics : GameSystem
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent CinematicStarted;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Cinematics.CinematicEndEvent CinematicEnded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Cinematics.CinematicEvent VideoStarted;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent VideoEnded;

		public bool InSantosCutscene { get; set; }

		public float CinematicVolume
		{
			get
			{
				float num = -1f;
				float result = -1f;
				if (this.audio.isValid())
				{
					this.audio.getVolume(ref num, ref result);
				}
				return result;
			}
			set
			{
				if (this.audio.isValid())
				{
					this.audio.setVolume(value);
				}
			}
		}

		public override void Awake()
		{
			this.rewired = null;
			this.lastCameraValid = false;
			this.ImageCoroutine = null;
		}

		public override void Update()
		{
			if (this.cinematicRunning && this.currentData != null && this.currentData.CanBeCancelled && !this.cinematicCancelled)
			{
				this.rewired = ReInput.players.GetPlayer(0);
				if (this.rewired.GetButtonDown(51))
				{
					this.cinematicCancelled = true;
					this.EndCutscene(true);
				}
			}
		}

		public void StartCutscene(CutsceneData cutscene, bool muteAudio, float fadeStart, float fadeEnd, bool useBackground)
		{
			if (this.cinematicRunning)
			{
				return;
			}
			this.cinematicCancelled = false;
			this.ImageCoroutine = null;
			Log.Trace("Cutscene", "Starting cinematic video.", null);
			if (!this.SearchReferences())
			{
				Log.Error("Cutscene", "References were not found.", null);
				return;
			}
			this.LocalizeCutscene(cutscene);
			this.audioMuted = muteAudio;
			if (this.audioMuted)
			{
				this.oldAudio = Core.Audio.Ambient.Volume;
				Core.Audio.FadeLevelAudio(0f, 1.5f);
			}
			Core.Input.SetBlocker("CINEMATIC", true);
			this.StartFade = fadeStart;
			this.EndFade = fadeEnd;
			if (this.StartFade > 0f)
			{
				Core.UI.Fade.Fade(true, this.StartFade, 0f, delegate
				{
					this.PlayVideo(cutscene, useBackground);
				});
			}
			else
			{
				this.PlayVideo(cutscene, useBackground);
			}
			if (this.CinematicStarted != null)
			{
				this.CinematicStarted();
			}
		}

		public void EndCutscene(bool cancelled)
		{
			if (!this.cinematicRunning)
			{
				return;
			}
			if (this.currentData.cinematicType == CinematicType.Video)
			{
				this.player.loopPointReached -= new VideoPlayer.EventHandler(this.OnVideoEnd);
			}
			Log.Trace("Cutscene", "Ending cinematic video.", null);
			if (this.VideoEnded != null)
			{
				this.VideoEnded();
			}
			if (this.EndFade > 0f)
			{
				Core.UI.Fade.FadeInOut(this.EndFade, 0f, delegate
				{
					this.EndCinematic();
				}, delegate
				{
					if (this.CinematicEnded != null)
					{
						this.CinematicEnded(cancelled);
					}
				});
			}
			else
			{
				this.EndCinematic();
				if (this.CinematicEnded != null)
				{
					this.CinematicEnded(cancelled);
				}
			}
		}

		public void SetFreeCamera(bool freeCamera)
		{
			if (freeCamera)
			{
				if (!this.lastCameraValid)
				{
					Core.Logic.CameraManager.ProCamera2D.Reset(true);
					this.lastCamera = Core.Logic.CameraManager.ProCamera2D.LocalPosition;
					this.lastCameraValid = true;
					this.lastCameraBoundaries = Core.Logic.CameraManager.ProCamera2DNumericBoundaries.UseNumericBoundaries;
				}
				Core.Logic.CameraManager.ProCamera2D.FollowHorizontal = false;
				Core.Logic.CameraManager.ProCamera2D.FollowVertical = false;
				Core.Logic.CameraManager.ProCamera2DNumericBoundaries.UseNumericBoundaries = false;
			}
			else
			{
				if (this.lastCameraValid)
				{
					Core.Logic.CameraManager.ProCamera2D.MoveCameraInstantlyToPosition(this.lastCamera);
					this.lastCameraValid = false;
					Core.Logic.CameraManager.ProCamera2DNumericBoundaries.UseNumericBoundaries = this.lastCameraBoundaries;
				}
				Core.Logic.CameraManager.ProCamera2D.FollowHorizontal = true;
				Core.Logic.CameraManager.ProCamera2D.FollowVertical = true;
				Core.Logic.CameraManager.ProCamera2D.Reset(true);
			}
			Core.Input.SetBlocker("FREECAMERA", freeCamera);
		}

		public void SetFreeCameraPosition(Vector3 newPosition)
		{
			Core.Logic.CameraManager.ProCamera2D.MoveCameraInstantlyToPosition(newPosition);
		}

		private void EndCinematic()
		{
			if (this.ImageCoroutine != null)
			{
				Singleton<Core>.Instance.StopCoroutine(this.ImageCoroutine);
				this.ImageCoroutine = null;
			}
			if (this.audio.isValid())
			{
				Debug.Log("*** EndCinematic, stop audio");
				this.audio.stop(0);
				this.audio.release();
				this.audio.clearHandle();
				this.audio = default(EventInstance);
			}
			if (this.currentData.cinematicType == CinematicType.Video)
			{
				this.player.Stop();
			}
			if (this.CurrentPrefabInstance != null)
			{
				Object.Destroy(this.CurrentPrefabInstance);
				this.CurrentPrefabInstance = null;
			}
			this.cinematicRunning = false;
			this.CamCinematicMode(false);
			Core.Input.SetBlocker("CINEMATIC", false);
			if (this.audioMuted)
			{
				Core.Audio.FadeLevelAudio(this.oldAudio, 1.5f);
			}
			this.videoImage.enabled = false;
			if (this.currentData.OverwriteReverb)
			{
				Core.Audio.Ambient.StopModifierParams("CINEMATICS");
			}
		}

		private void CamCinematicMode(bool cinematicMode)
		{
			Log.Trace("Cutscene", "Cam cinematic mode is " + cinematicMode, null);
			float x = this.cam.transform.position.x;
			float y = this.cam.transform.position.y;
			if (cinematicMode)
			{
				Core.UI.GameplayUI.gameObject.SetActive(false);
			}
			else
			{
				this.cam.transform.position = new Vector3(x, y, -10f);
				Core.UI.GameplayUI.gameObject.SetActive(true);
			}
		}

		private bool SearchReferences()
		{
			this.cam = Camera.main;
			if (this.cam == null)
			{
				return false;
			}
			this.player = this.cam.gameObject.GetComponent<VideoPlayer>();
			if (this.player == null)
			{
				this.player = this.cam.gameObject.AddComponent<VideoPlayer>();
			}
			return this.cam != null && this.player != null;
		}

		private void PlayVideo(CutsceneData data, bool useSubtitlesBackgound)
		{
			this.currentData = data;
			this.audio = default(EventInstance);
			string text = data.foley;
			if (data.foleySpanish.Length > 0 && Core.Localization.GetCurrentAudioLanguageCode().ToUpper() == "ES")
			{
				text = data.foleySpanish;
			}
			if (text.Length > 0)
			{
				Debug.Log("Cutscene: Create audio " + text);
				this.audio = Core.Audio.CreateEvent(text, default(Vector3));
			}
			this.videoImage = UIController.instance.GetSubtitleWidget().videoRaw;
			if (this.renderTexture == null)
			{
				this.renderTexture = new RenderTexture(1920, 1080, 24);
			}
			UIController.instance.GetSubtitleWidget().EnableSubtitleBackground(useSubtitlesBackgound);
			if (data.OverwriteReverb)
			{
				Core.Audio.Ambient.StartModifierParams("CINEMATICS", data.ReverbId, new AudioParamInitialized[0]);
			}
			CinematicType cinematicType = this.currentData.cinematicType;
			if (cinematicType != CinematicType.Images)
			{
				if (cinematicType != CinematicType.Video)
				{
					if (cinematicType == CinematicType.Animation)
					{
						this.ShowAnimationCutscene();
					}
				}
				else
				{
					try
					{
						this.player.clip = data.video;
						this.player.started += new VideoPlayer.EventHandler(this.OnVideoStart);
						this.player.loopPointReached += new VideoPlayer.EventHandler(this.OnVideoEnd);
						this.player.playbackSpeed = data.reproductionSpeed;
						this.player.prepareCompleted += new VideoPlayer.EventHandler(this.OnVideoPrepared);
						this.player.playOnAwake = false;
						this.player.renderMode = 2;
						this.videoImage.texture = this.renderTexture;
						this.player.targetTexture = this.renderTexture;
						this.player.Prepare();
					}
					catch (NullReferenceException ex)
					{
						Log.Error("Cutscene", "Invalid cutscene data when preparing. Forcing ending. Error: " + ex.ToString(), null);
					}
				}
			}
			else
			{
				if (this.currentData.images.Count > 0)
				{
					this.videoImage.texture = this.currentData.images[0].image.texture;
				}
				Core.UI.Fade.ClearFade();
				this.ImageCoroutine = Singleton<Core>.Instance.StartCoroutine(this.ShowImagesCutscene());
			}
		}

		private void OnVideoPrepared(VideoPlayer source)
		{
			this.player.prepareCompleted -= new VideoPlayer.EventHandler(this.OnVideoPrepared);
			try
			{
				if (this.StartFade > 0f)
				{
					Core.UI.Fade.Fade(false, this.StartFade, 1f, delegate
					{
					});
				}
				this.player.Play();
				if (this.audio.isValid())
				{
					this.audio.start();
				}
				this.cinematicRunning = true;
				Log.Trace("Cutscene", "Starting video with camera " + this.cam.name + " and clip " + source.clip.name, null);
				Singleton<Core>.Instance.StartCoroutine(this.OnVideoPreparedSecured());
			}
			catch (NullReferenceException ex)
			{
				Log.Error("Cutscene", "Invalid cutscene data when playing. Forcing ending. Error: " + ex.ToString(), null);
			}
		}

		private IEnumerator OnVideoPreparedSecured()
		{
			yield return new WaitForEndOfFrame();
			this.videoImage.enabled = true;
			if (this.StartFade == 0f)
			{
				Core.UI.Fade.ClearFade();
			}
			yield break;
		}

		private void OnVideoEnd(VideoPlayer source)
		{
			Log.Trace("Cutscene", "Video End", null);
			this.EndCutscene(false);
		}

		private void OnVideoStart(VideoPlayer source)
		{
			this.player.started -= new VideoPlayer.EventHandler(this.OnVideoStart);
			Log.Trace("Cutscene", "Video Start", null);
			if (this.VideoStarted != null)
			{
				this.VideoStarted(this.currentData);
			}
			this.CamCinematicMode(true);
		}

		private IEnumerator ShowImagesCutscene()
		{
			this.videoImage.enabled = true;
			if (this.StartFade > 0f)
			{
				Core.UI.Fade.Fade(false, this.StartFade, 1f, delegate
				{
				});
			}
			if (this.audio.isValid())
			{
				this.audio.start();
			}
			this.cinematicRunning = true;
			Log.Trace("Cutscene", "Starting image list " + this.currentData.name, null);
			this.OnVideoStart(null);
			foreach (ImageList img in this.currentData.images)
			{
				this.videoImage.texture = img.image.texture;
				yield return new WaitForSeconds(img.duration);
			}
			this.EndCutscene(false);
			yield break;
		}

		private void ShowAnimationCutscene()
		{
			this.videoImage.enabled = false;
			this.CurrentPrefabInstance = Object.Instantiate<GameObject>(this.currentData.AnimationObject);
			Vector3 position = Core.Logic.CameraManager.ProCamera2D.transform.position;
			position.z = 0f;
			this.CurrentPrefabInstance.transform.position = position;
			string currentAudioLanguageCode = Core.Localization.GetCurrentAudioLanguageCode();
			if (this.currentData.animationTrigger != null && this.currentData.animationTrigger.ContainsKey(currentAudioLanguageCode) && this.currentData.animationTrigger[currentAudioLanguageCode].Count > 0)
			{
				CinematicsAnimator component = this.CurrentPrefabInstance.GetComponent<CinematicsAnimator>();
				component.SetTriggerList(this.currentData.animationTrigger[currentAudioLanguageCode]);
			}
			if (this.StartFade > 0f)
			{
				Core.UI.Fade.Fade(false, this.StartFade, 1f, delegate
				{
				});
			}
			if (this.audio.isValid())
			{
				this.audio.start();
			}
			this.cinematicRunning = true;
			Log.Trace("Cutscene", "Starting animation list " + this.currentData.name, null);
			this.OnVideoStart(null);
		}

		public void OnCinematicsAnimationEnd()
		{
			this.EndCutscene(false);
		}

		public void LocalizeCutscene(CutsceneData data)
		{
			int languageIndexFromCode = Core.Dialog.Language.GetLanguageIndexFromCode(LocalizationManager.CurrentLanguageCode, true);
			string subtitleBaseTermName = data.GetSubtitleBaseTermName();
			List<TimeLocalization> list = new List<TimeLocalization>();
			string key = Core.Localization.GetCurrentAudioLanguageCode().ToUpper();
			if (data.subtitlesLocalization.ContainsKey(key))
			{
				list = data.subtitlesLocalization[key];
			}
			int num = 0;
			foreach (SubTitleBlock subTitleBlock in data.subtitles)
			{
				string text = subtitleBaseTermName + "_" + num.ToString();
				TermData termData = Core.Dialog.Language.GetTermData(text, false);
				if (termData == null)
				{
					Debug.LogWarning("Term " + text + " not found in Cutscene Localization");
				}
				else
				{
					subTitleBlock.text = termData.Languages[languageIndexFromCode];
					if (num < list.Count)
					{
						subTitleBlock.from = list[num].from;
						subTitleBlock.to = list[num].to;
					}
					num++;
				}
			}
		}

		private bool cinematicRunning;

		private Camera cam;

		private VideoPlayer player;

		private EventInstance audio;

		private CutsceneData currentData;

		private Player rewired;

		private bool audioMuted;

		private float StartFade;

		private float EndFade;

		private RawImage videoImage;

		private RenderTexture renderTexture;

		private float oldAudio;

		private const string CINEMATICS_PARAMS_ID = "CINEMATICS";

		private Vector3 lastCamera = Vector3.zero;

		private bool lastCameraValid;

		private bool lastCameraBoundaries;

		private GameObject CurrentPrefabInstance;

		private Coroutine ImageCoroutine;

		private bool cinematicCancelled;

		public delegate void CinematicEvent(CutsceneData cutscene);

		public delegate void CinematicEndEvent(bool cancelled);
	}
}
