using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using Framework.Achievements;
using Framework.Managers;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class PopupAchievementWidget : MonoBehaviour
	{
		public void Awake()
		{
			this.pendingAchievement = new Queue<Achievement>();
			this.PopUp.localPosition = this.InitialPos.localPosition;
			this.IsShowing = false;
		}

		public void ShowPopup(Achievement achievement)
		{
			if (this.IsShowing)
			{
				this.pendingAchievement.Enqueue(achievement);
			}
			else
			{
				base.StartCoroutine(this.ShowPopupCorrutine(achievement));
			}
		}

		private IEnumerator ShowPopupCorrutine(Achievement achievement)
		{
			this.IsShowing = true;
			this.HeaderLoc.SetTerm(achievement.GetNameLocalizationTerm());
			this.SpriteImage.sprite = achievement.Image;
			Core.Audio.PlayOneShot(this.ShowSound, default(Vector3));
			this.PopUp.localPosition = this.InitialPos.localPosition;
			yield return new WaitForSecondsRealtime(this.startDelay);
			Tweener tween = TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(this.PopUp, this.EndPos.localPosition, this.animationInTime, false), this.animationInEase), true);
			yield return TweenExtensions.WaitForCompletion(tween);
			yield return new WaitForSecondsRealtime(this.popupShowTime);
			tween = TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalMove(this.PopUp, this.InitialPos.localPosition, this.animationOutTime, false), this.animationOutEase), true);
			yield return TweenExtensions.WaitForCompletion(tween);
			yield return new WaitForSecondsRealtime(this.endTime);
			if (this.pendingAchievement.Count > 0)
			{
				base.StartCoroutine(this.ShowPopupCorrutine(this.pendingAchievement.Dequeue()));
			}
			else
			{
				this.IsShowing = false;
			}
			yield break;
		}

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Localize HeaderLoc;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Image SpriteImage;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private RectTransform InitialPos;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private RectTransform EndPos;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private RectTransform PopUp;

		[SerializeField]
		[BoxGroup("Sound", true, false, 0)]
		[EventRef]
		private string ShowSound;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float startDelay = 0.2f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float animationInTime = 1f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private Ease animationInEase = 6;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float popupShowTime = 5f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float animationOutTime = 1f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private Ease animationOutEase = 6;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float endTime = 1f;

		private bool IsShowing;

		private Queue<Achievement> pendingAchievement;
	}
}
