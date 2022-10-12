using System;
using System.Collections.Generic;
using FMODUnity;
using Framework.Managers;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Video;

namespace Tools.DataContainer
{
	[CreateAssetMenu(fileName = "InputIconLayout", menuName = "Blasphemous/Cutscene")]
	public class CutsceneData : SerializedScriptableObject
	{
		public string GetSubtitleBaseTermName()
		{
			return "CUTSCENE/" + base.name;
		}

		[BoxGroup("General", true, false, 0)]
		public CinematicType cinematicType;

		[BoxGroup("General", true, false, 0)]
		public bool CanBeCancelled = true;

		[BoxGroup("Video Settings", true, false, 0)]
		[InfoBox("Recommended video formats are .mp4, .m4v, and .mov", InfoMessageType.Info, null)]
		public VideoClip video;

		[BoxGroup("Video Settings", true, false, 0)]
		[Range(0.1f, 2f)]
		public float reproductionSpeed;

		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string foley;

		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string foleySpanish = string.Empty;

		[BoxGroup("Audio Settings", true, false, 0)]
		public bool OverwriteReverb;

		[ShowIf("OverwriteReverb", true)]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string ReverbId = string.Empty;

		[BoxGroup("Subtitles Settings", true, false, 0)]
		public string voiceOverPrefix = string.Empty;

		[BoxGroup("Subtitles Settings", true, false, 0)]
		public List<SubTitleBlock> subtitles;

		[BoxGroup("Subtitles Settings", true, false, 0)]
		[OdinSerialize]
		public Dictionary<string, List<TimeLocalization>> subtitlesLocalization = new Dictionary<string, List<TimeLocalization>>();

		[BoxGroup("Animation Trigger", true, false, 0)]
		[OdinSerialize]
		public Dictionary<string, List<float>> animationTrigger = new Dictionary<string, List<float>>
		{
			{
				LocalizationManager.AudioLanguagesKeys[0],
				new List<float>()
			},
			{
				LocalizationManager.AudioLanguagesKeys[1],
				new List<float>()
			}
		};

		[BoxGroup("Rumble Settings", true, false, 0)]
		public List<RummbleBlock> rumbles;

		public List<ImageList> images;

		public GameObject AnimationObject;
	}
}
