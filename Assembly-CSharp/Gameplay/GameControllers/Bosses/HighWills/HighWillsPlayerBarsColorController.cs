using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.GameControllers.Bosses.HighWills
{
	[RequireComponent(typeof(Image))]
	public class HighWillsPlayerBarsColorController : MonoBehaviour
	{
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		private void Start()
		{
			this.RecursiveColorTweening();
		}

		[BoxGroup("Setup Button", true, false, 0)]
		[InfoBox("Use this button to add more color segments,as it automatically sets the Starting Color of the new segment and the same Ease as the previous one.", 1, null)]
		[Button(0)]
		private void AddNewColorSegment()
		{
			HighWillsPlayerBarsColorController.ColorSegment item = default(HighWillsPlayerBarsColorController.ColorSegment);
			if (this.ColorSegments.Count > 0)
			{
				item.StartColor = this.ColorSegments[this.ColorSegments.Count - 1].EndColor;
				item.TweeningTime = this.ColorSegments[this.ColorSegments.Count - 1].TweeningTime;
				item.Ease = this.ColorSegments[this.ColorSegments.Count - 1].Ease;
			}
			this.ColorSegments.Add(item);
		}

		[BoxGroup("Debugging Buttons", true, false, 0)]
		[InfoBox("This button serves debugging purpouses, as the Gameobject should be deactivated when not in use.", 1, null)]
		[Button(0)]
		public void StopColorTweening()
		{
			this.stoppingColorTweening = true;
		}

		[BoxGroup("Debugging Buttons", true, false, 0)]
		[InfoBox("This button serves debugging purpouses, as the color tweening should start with the Gameobject's Start.", 1, null)]
		[Button(0)]
		public void StartColorTweening()
		{
			this.RecursiveColorTweening();
		}

		private void RecursiveColorTweening()
		{
			if (this.stoppingColorTweening)
			{
				this.stoppingColorTweening = false;
				return;
			}
			this.colorSegmentIndex++;
			if (this.colorSegmentIndex == this.ColorSegments.Count)
			{
				this.colorSegmentIndex = 0;
			}
			HighWillsPlayerBarsColorController.ColorSegment colorSegment = this.ColorSegments[this.colorSegmentIndex];
			this.image.color = colorSegment.StartColor;
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions46.DOColor(this.image, colorSegment.EndColor, colorSegment.TweeningTime), colorSegment.Ease), new TweenCallback(this.RecursiveColorTweening));
		}

		[SerializeField]
		private List<HighWillsPlayerBarsColorController.ColorSegment> ColorSegments = new List<HighWillsPlayerBarsColorController.ColorSegment>();

		private Image image;

		private int colorSegmentIndex = -1;

		private bool stoppingColorTweening;

		[Serializable]
		private struct ColorSegment
		{
			public Color StartColor;

			public Color EndColor;

			public Ease Ease;

			public float TweeningTime;
		}
	}
}
