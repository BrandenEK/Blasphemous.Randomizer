using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using Framework.Achievements;
using Framework.Managers;
using Framework.Randomizer;
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
			if (achievement.GetType() != typeof(RewardAchievement))
			{
				return;
			}
			if (this.IsShowing)
			{
				this.pendingAchievement.Enqueue(achievement);
				return;
			}
			base.StartCoroutine(this.ShowPopupCorrutine(achievement));
		}

		private IEnumerator ShowPopupCorrutine(Achievement achievement)
		{
			this.IsShowing = true;
			PopUpWidget.closeDialog();
			this.SpriteImage.sprite = achievement.Image;
			this.PopUp.GetChild(1).GetComponent<Text>().text = achievement.Name;
			this.PopUp.GetChild(2).GetComponent<Text>().text = achievement.Description;
			Core.Audio.PlayOneShot(this.ShowSound, default(Vector3));
			this.PopUp.localPosition = this.InitialPos.localPosition;
			Tweener t = this.PopUp.DOLocalMove(this.EndPos.localPosition, this.animationInTime, false).SetEase(this.animationInEase).SetUpdate(true);
			yield return t.WaitForCompletion();
			yield return new WaitForSecondsRealtime(2.5f);
			t = this.PopUp.DOLocalMove(this.InitialPos.localPosition, this.animationOutTime, false).SetEase(this.animationOutEase).SetUpdate(true);
			yield return t.WaitForCompletion();
			yield return new WaitForSecondsRealtime(0.2f);
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
		private Ease animationInEase = Ease.OutQuad;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float popupShowTime = 5f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float animationOutTime = 1f;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private Ease animationOutEase = Ease.OutQuad;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float endTime = 1f;

		private bool IsShowing;

		private Queue<Achievement> pendingAchievement;
	}
}
