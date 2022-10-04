using System;
using System.Collections;
using FMOD.Studio;
using Framework.Managers;
using Sirenix.OdinInspector;
using Tools.DataContainer;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleWidget : MonoBehaviour
{
	private void Awake()
	{
		this.inPlay = false;
		this.videoRaw.enabled = false;
		Core.Cinematics.VideoStarted += this.VideoStarted;
		Core.Cinematics.VideoEnded += this.VideoEnded;
	}

	private void OnDestroy()
	{
		Core.Cinematics.VideoStarted -= this.VideoStarted;
		Core.Cinematics.VideoEnded -= this.VideoEnded;
	}

	public void EnableSubtitleBackground(bool enable)
	{
		this.textBackground.SetActive(enable);
	}

	private void VideoStarted(CutsceneData cutscene)
	{
		if (this.inPlay || cutscene.subtitles.Count == 0)
		{
			return;
		}
		this.inPlay = true;
		base.StartCoroutine(this.ShowSubtitles(cutscene));
		base.StartCoroutine(this.PerformRumble(cutscene));
	}

	private void VideoEnded()
	{
		if (!this.inPlay)
		{
			return;
		}
		base.StopAllCoroutines();
		this.inPlay = false;
		this.canvasText.alpha = 0f;
		this.StopCurrentSound();
	}

	private void StopCurrentSound()
	{
		if (this.currentSound.isValid())
		{
			this.currentSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.currentSound.release();
			this.currentSound.clearHandle();
			this.currentSound = default(EventInstance);
		}
	}

	private IEnumerator ShowSubtitles(CutsceneData cutscene)
	{
		float currentTime = 0f;
		int idx = 0;
		foreach (SubTitleBlock subtitle in cutscene.subtitles)
		{
			float timetowait = subtitle.from - currentTime;
			yield return new WaitForSeconds(timetowait);
			if (!string.IsNullOrEmpty(cutscene.voiceOverPrefix))
			{
				string key = cutscene.voiceOverPrefix + "_" + idx.ToString();
				this.StopCurrentSound();
				this.currentSound = Core.Dialog.PlayProgrammerSound(key, null);
			}
			idx++;
			currentTime += timetowait;
			this.canvasText.alpha = 1f;
			this.dialogLine.text = subtitle.text;
			float length = subtitle.to - subtitle.from;
			yield return new WaitForSeconds(length);
			currentTime += length;
			this.canvasText.alpha = 0f;
			this.dialogLine.text = string.Empty;
		}
		yield return 0;
		yield break;
	}

	private IEnumerator PerformRumble(CutsceneData cutscene)
	{
		float currentTime = 0f;
		foreach (RummbleBlock rumble in cutscene.rumbles)
		{
			float timetowait = rumble.from - currentTime;
			if (timetowait < 0f)
			{
				timetowait = 0f;
			}
			yield return new WaitForSeconds(timetowait);
			Core.Input.ApplyRumble(rumble.rumble);
			currentTime += timetowait;
		}
		yield return 0;
		yield break;
	}

	[BoxGroup("Text", true, false, 0)]
	public Text dialogLine;

	[BoxGroup("Text", true, false, 0)]
	public CanvasGroup canvasText;

	[BoxGroup("Text", true, false, 0)]
	public GameObject textBackground;

	[BoxGroup("Video", true, false, 0)]
	public RawImage videoRaw;

	private bool inPlay;

	private EventInstance currentSound;
}
