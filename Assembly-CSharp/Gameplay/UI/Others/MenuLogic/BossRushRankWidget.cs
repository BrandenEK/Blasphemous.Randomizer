using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using Framework.BossRush;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.BossFight;
using Gameplay.UI.Widgets;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using Tools.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class BossRushRankWidget : BasicUIBlockingWidget
	{
		public bool RetryPressed { get; private set; }

		public IEnumerator PlayBossRushRankAudio(bool complete)
		{
			BossFightManager BossFight = Object.FindObjectOfType<BossFightManager>();
			if (BossFight)
			{
				BossFight.Audio.StopBossTrack();
			}
			yield return new WaitForSeconds(0.2f);
			string audio = (!complete) ? this.looseMusic : this.winMusic;
			if (audio != string.Empty)
			{
				Core.Audio.Ambient.SetSceneParams(audio, string.Empty, new AudioParam[0], string.Empty);
			}
			yield break;
		}

		public void PlayRankLineSfx1()
		{
			Core.Audio.PlayOneShot(this.rankLineSfx1, default(Vector3));
		}

		public void PlayRankLineSfx2()
		{
			Core.Audio.PlayOneShot(this.rankLineSfx2, default(Vector3));
		}

		public void MarkShowAnimationAsDone()
		{
			this.showAnimationDone = true;
		}

		public void PlayGradeSfx()
		{
			if (this.scoreShown.Score > BossRushManager.BossRushCourseScore.A_PLUS)
			{
				Core.Audio.PlayOneShot(this.lowGradeSfx, default(Vector3));
			}
			else
			{
				Core.Audio.PlayOneShot(this.highGradeSfx, default(Vector3));
			}
		}

		public void ShowHighScore(BossRushHighScore score, bool pauseGame, bool complete, bool unlockHard)
		{
			this.scoreShown = score;
			this.FalseFade.gameObject.SetActive(false);
			this.FalseFade.color = new Color(0f, 0f, 0f, 0f);
			this.IsSelected = false;
			this.IsUnlockHard = unlockHard;
			this.RetryPressed = false;
			this.IsFailed = !complete;
			int num = (int)(score.CourseId + 1);
			BossRushManager.BossRushCourseId courseId = score.CourseId;
			switch (courseId)
			{
			case BossRushManager.BossRushCourseId.COURSE_A_1:
				this.CourseName.text = ScriptLocalization.UI_BossRush.COURSE_A_1;
				break;
			case BossRushManager.BossRushCourseId.COURSE_A_2:
				this.CourseName.text = ScriptLocalization.UI_BossRush.COURSE_A_2;
				break;
			case BossRushManager.BossRushCourseId.COURSE_A_3:
				this.CourseName.text = ScriptLocalization.UI_BossRush.COURSE_A_3;
				break;
			default:
				if (courseId != BossRushManager.BossRushCourseId.COURSE_B_1)
				{
					if (courseId != BossRushManager.BossRushCourseId.COURSE_C_1)
					{
						if (courseId == BossRushManager.BossRushCourseId.COURSE_D_1)
						{
							this.CourseName.text = ScriptLocalization.UI_BossRush.COURSE_D_1;
						}
					}
					else
					{
						this.CourseName.text = ScriptLocalization.UI_BossRush.COURSE_C_1;
					}
				}
				else
				{
					this.CourseName.text = ScriptLocalization.UI_BossRush.COURSE_B_1;
				}
				break;
			}
			if (complete)
			{
				this.CourseCompletedSuffix.gameObject.SetActive(true);
				this.CourseFailedSuffix.gameObject.SetActive(false);
			}
			else
			{
				this.CourseCompletedSuffix.gameObject.SetActive(false);
				this.CourseFailedSuffix.gameObject.SetActive(true);
			}
			if (this.NewRerecord)
			{
				this.NewRerecord.SetActive(score.IsNewHighScore);
			}
			this.ButtonRetry.SetActive(!complete);
			Sprite sprite = this.Course_A_1;
			BossRushManager.BossRushCourseId courseId2 = score.CourseId;
			if (courseId2 != BossRushManager.BossRushCourseId.COURSE_A_2)
			{
				if (courseId2 != BossRushManager.BossRushCourseId.COURSE_A_3)
				{
					if (courseId2 != BossRushManager.BossRushCourseId.COURSE_B_1)
					{
						if (courseId2 != BossRushManager.BossRushCourseId.COURSE_C_1)
						{
							if (courseId2 == BossRushManager.BossRushCourseId.COURSE_D_1)
							{
								sprite = this.Course_D_1;
							}
						}
						else
						{
							sprite = this.Course_C_1;
						}
					}
					else
					{
						sprite = this.Course_B_1;
					}
				}
				else
				{
					sprite = this.Course_A_3;
				}
			}
			else
			{
				sprite = this.Course_A_2;
			}
			this.CourseImage.sprite = sprite;
			this.DificultNormal.SetActive(score.CourseMode == BossRushManager.BossRushCourseMode.NORMAL);
			this.DificultHard.SetActive(score.CourseMode == BossRushManager.BossRushCourseMode.HARD);
			IEnumerator enumerator = this.FlaskBase.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					Object.Destroy(transform.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			IEnumerator enumerator2 = this.FlaskBase2.transform.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object obj2 = enumerator2.Current;
					Transform transform2 = (Transform)obj2;
					Object.Destroy(transform2.gameObject);
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			int num2 = 28;
			int num3 = 0;
			while (num3 < score.NumFlasksUsed && num3 < num2)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.FlaskElement, Vector3.zero, Quaternion.identity);
				if (num3 < num2 / 2)
				{
					gameObject.transform.SetParent(this.FlaskBase.transform);
				}
				else
				{
					gameObject.transform.SetParent(this.FlaskBase2.transform);
				}
				RectTransform rectTransform = (RectTransform)gameObject.transform;
				rectTransform.localRotation = Quaternion.identity;
				rectTransform.localScale = Vector3.one;
				rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
				gameObject.SetActive(true);
				num3++;
			}
			this.FlaskElement.SetActive(false);
			this.DogesText.text = score.NumDodgesAchieved.ToString();
			this.BloodText.text = score.NumBloodPenancesUsed.ToString();
			this.HitsText.text = score.NumHitsReceived.ToString();
			this.PrayerText.text = score.NumPrayersUsed.ToString();
			this.TimeText.text = score.RunDurationInString();
			this.SetGrade(score.Score);
			BossRushHighScore bossRushHighScore;
			if (score.IsNewHighScore)
			{
				bossRushHighScore = Core.BossRushManager.GetPrevHighScore(score.CourseId, score.CourseMode);
			}
			else
			{
				bossRushHighScore = Core.BossRushManager.GetHighScore(score.CourseId, score.CourseMode);
			}
			if (bossRushHighScore != null)
			{
				this.PrevTimeText.text = bossRushHighScore.RunDurationInString();
			}
			else
			{
				this.PrevTimeText.text = "-- : -- : --";
			}
			base.FadeShow(false, pauseGame, false);
		}

		private void SetGrade(BossRushManager.BossRushCourseScore score)
		{
			this.ScoreLetters.ForEach(delegate(BossRushRankWidget.ScoreLetter x)
			{
				x.letterGameObject.SetActive(false);
			});
			this.ScoreLetters.Find((BossRushRankWidget.ScoreLetter x) => x.score == score).letterGameObject.SetActive(true);
		}

		public override bool AutomaticBack()
		{
			return false;
		}

		private void Update()
		{
			if (base.IsFading || ReInput.players.playerCount <= 0)
			{
				return;
			}
			Player player = ReInput.players.GetPlayer(0);
			bool flag = this.IsFailed && player.GetButtonDown(50);
			if (!this.IsSelected && this.showAnimationDone && !FadeWidget.instance.Fading && (flag || player.GetButtonDown(51)))
			{
				this.RetryPressed = flag;
				this.IsSelected = true;
				this.scoreShown = null;
				this.showAnimationDone = false;
				if (this.IsUnlockHard)
				{
					base.StartCoroutine(this.ShowPopUp());
				}
				else
				{
					FadeWidget.instance.Fade(true, 1f, 0f, delegate
					{
						base.Hide();
					});
				}
			}
		}

		private IEnumerator ShowPopUp()
		{
			this.TextUnlocked.GetComponent<Text>().text = ScriptLocalization.UI_BossRush.TEXT_HARD_UNLOCKED;
			CanvasGroup group = this.TextUnlocked.GetComponent<CanvasGroup>();
			group.alpha = 0f;
			this.FalseFade.gameObject.SetActive(true);
			Tweener tween = ShortcutExtensions46.DOColor(this.FalseFade, Color.black, 0.5f);
			yield return TweenExtensions.WaitForCompletion(tween);
			tween = ShortcutExtensions46.DOFade(group, 1f, 0.4f);
			yield return new WaitForSecondsRealtime(4f);
			FadeWidget.instance.Fade(true, 1f, 0f, delegate
			{
				base.Hide();
			});
			yield break;
		}

		[BoxGroup("Widgets Course", true, false, 0)]
		public Text CourseName;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Text CourseCompletedSuffix;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Text CourseFailedSuffix;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Image CourseImage;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Sprite Course_A_1;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Sprite Course_A_2;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Sprite Course_A_3;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Sprite Course_B_1;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Sprite Course_C_1;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Sprite Course_D_1;

		[BoxGroup("Widgets Course", true, false, 0)]
		public GameObject ButtonRetry;

		[BoxGroup("Widgets Course", true, false, 0)]
		public Image FalseFade;

		[BoxGroup("Widgets Course", true, false, 0)]
		public GameObject NewRerecord;

		[BoxGroup("Widgets Course", true, false, 0)]
		public GameObject TextUnlocked;

		[BoxGroup("Widgets Dificult", true, false, 0)]
		public GameObject DificultNormal;

		[BoxGroup("Widgets Dificult", true, false, 0)]
		public GameObject DificultHard;

		[BoxGroup("Widgets Text", true, false, 0)]
		public GameObject FlaskBase;

		[BoxGroup("Widgets Text", true, false, 0)]
		public GameObject FlaskBase2;

		[BoxGroup("Widgets Text", true, false, 0)]
		public GameObject FlaskElement;

		[BoxGroup("Widgets Text", true, false, 0)]
		public Text DogesText;

		[BoxGroup("Widgets Text", true, false, 0)]
		public Text PrayerText;

		[BoxGroup("Widgets Text", true, false, 0)]
		public Text BloodText;

		[BoxGroup("Widgets Text", true, false, 0)]
		public Text HitsText;

		[BoxGroup("Widgets Grade", true, false, 0)]
		public Text TimeText;

		[BoxGroup("Widgets Grade", true, false, 0)]
		public Text PrevTimeText;

		[BoxGroup("Widgets Grade", true, false, 0)]
		public Image GradeImage;

		[BoxGroup("Grade Images", true, false, 0)]
		public List<BossRushRankWidget.ScoreLetter> ScoreLetters;

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string winMusic = string.Empty;

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string looseMusic = string.Empty;

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string rankLineSfx1 = string.Empty;

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string rankLineSfx2 = string.Empty;

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string lowGradeSfx = string.Empty;

		[BoxGroup("Sounds", true, false, 0)]
		[EventRef]
		public string highGradeSfx = string.Empty;

		private bool IsUnlockHard;

		private bool IsFailed;

		private bool IsSelected;

		private BossRushHighScore scoreShown;

		private bool showAnimationDone;

		[Serializable]
		public struct ScoreLetter
		{
			public BossRushManager.BossRushCourseScore score;

			public GameObject letterGameObject;
		}
	}
}
