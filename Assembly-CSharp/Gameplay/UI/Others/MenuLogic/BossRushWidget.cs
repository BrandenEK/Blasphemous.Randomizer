using System;
using System.Collections.Generic;
using Framework.BossRush;
using Framework.Managers;
using Gameplay.UI.Others.Buttons;
using Gameplay.UI.Widgets;
using I2.Loc;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class BossRushWidget : BasicUIBlockingWidget
	{
		public bool IsAllSelected { get; private set; }

		public int CurrentSlot { get; private set; }

		public BossRushManager.BossRushCourseId SelectedCourse { get; private set; }

		public BossRushManager.BossRushCourseMode SelectedMode { get; private set; }

		protected override void OnWidgetInitialize()
		{
			this.CurrentSlot = 0;
			this.stateWidgets = new List<BasicUIBlockingWidget>
			{
				this.SlotSelector,
				this.CourseSelector,
				this.DifficultySelector
			};
			this.stateWidgets.ForEach(delegate(BasicUIBlockingWidget x)
			{
				x.InitializeWidget();
			});
			this.IsAllSelected = false;
			this.HowToUnlockText.text = string.Empty;
		}

		protected override void OnWidgetShow()
		{
			this.selectSaveSlotsWidget.Clear();
			this.selectSaveSlotsWidget.SetAllData(null, SelectSaveSlots.SlotsModes.BossRush);
			this.SetState(BossRushWidget.States.Slot);
			this.IsAllSelected = false;
			this.HowToUnlockText.text = string.Empty;
			this.OptionSelected(0);
		}

		private void Update()
		{
			if (ReInput.players.playerCount <= 0)
			{
				return;
			}
			Player player = ReInput.players.GetPlayer(0);
			if (player.GetButtonDown(51) && this.CheckFading())
			{
				if (this.CurrentState > BossRushWidget.States.Slot)
				{
					this.SetState(this.CurrentState - 1);
				}
				else
				{
					base.FadeHide();
				}
			}
		}

		public void OptionSelected(int option)
		{
			bool flag = true;
			BossRushWidget.States currentState = this.CurrentState;
			if (currentState != BossRushWidget.States.Slot)
			{
				if (currentState != BossRushWidget.States.Course)
				{
					if (currentState == BossRushWidget.States.Difficulty)
					{
						if (option != 0)
						{
							if (option == 1)
							{
								this.SelectedMode = BossRushManager.BossRushCourseMode.HARD;
								flag = Core.BossRushManager.IsModeUnlocked(this.SelectedCourse, BossRushManager.BossRushCourseMode.HARD);
							}
						}
						else
						{
							this.SelectedMode = BossRushManager.BossRushCourseMode.NORMAL;
							flag = true;
						}
					}
				}
				else
				{
					switch (option)
					{
					case 0:
						this.LeftArrowGO.SetActive(false);
						this.RightArrowGO.SetActive(true);
						this.SelectedCourse = BossRushManager.BossRushCourseId.COURSE_A_1;
						break;
					case 1:
						this.LeftArrowGO.SetActive(true);
						this.RightArrowGO.SetActive(true);
						this.SelectedCourse = BossRushManager.BossRushCourseId.COURSE_A_2;
						this.HowToUnlockText.text = ScriptLocalization.UI_BossRush.LABEL_UNLOCK_COURSE_A_2.Replace("%", Environment.NewLine);
						break;
					case 2:
						this.LeftArrowGO.SetActive(true);
						this.RightArrowGO.SetActive(true);
						this.SelectedCourse = BossRushManager.BossRushCourseId.COURSE_A_3;
						this.HowToUnlockText.text = ScriptLocalization.UI_BossRush.LABEL_UNLOCK_COURSE_A_3.Replace("%", Environment.NewLine);
						break;
					case 3:
						this.LeftArrowGO.SetActive(true);
						this.RightArrowGO.SetActive(true);
						this.SelectedCourse = BossRushManager.BossRushCourseId.COURSE_B_1;
						this.HowToUnlockText.text = ScriptLocalization.UI_BossRush.LABEL_UNLOCK_COURSE_B_1.Replace("%", Environment.NewLine);
						break;
					case 4:
						this.LeftArrowGO.SetActive(true);
						this.RightArrowGO.SetActive(true);
						this.SelectedCourse = BossRushManager.BossRushCourseId.COURSE_C_1;
						this.HowToUnlockText.text = ScriptLocalization.UI_BossRush.LABEL_UNLOCK_COURSE_C_1.Replace("%", Environment.NewLine);
						break;
					case 5:
						this.LeftArrowGO.SetActive(true);
						this.RightArrowGO.SetActive(false);
						this.SelectedCourse = BossRushManager.BossRushCourseId.COURSE_D_1;
						this.HowToUnlockText.text = ScriptLocalization.UI_BossRush.LABEL_UNLOCK_COURSE_D_1.Replace("%", Environment.NewLine);
						break;
					}
					bool flag2 = Core.BossRushManager.IsModeUnlocked(this.SelectedCourse, BossRushManager.BossRushCourseMode.NORMAL);
					if (flag2)
					{
						this.HowToUnlockText.text = string.Empty;
					}
					flag = this.UnlockedCourses.Contains(this.SelectedCourse);
					this.CoursesRoot.localPosition = new Vector3(this.CoursesRoot.localPosition.x, (float)(32 + option * 68), this.CoursesRoot.localPosition.z);
				}
			}
			else
			{
				this.CurrentSlot = option;
				this.UnlockedCourses = Core.BossRushManager.GetUnlockedCourses();
				flag = this.selectSaveSlotsWidget.CanLoadSelectedSlot;
			}
			this.AcceptButton.SetActive(flag);
			this.AcceptButtonDisabled.SetActive(!flag);
		}

		public void OptionPressed(int option)
		{
			bool flag = true;
			if (!this.CheckFading())
			{
				return;
			}
			BossRushWidget.States currentState = this.CurrentState;
			if (currentState != BossRushWidget.States.Slot)
			{
				if (currentState != BossRushWidget.States.Course)
				{
					if (currentState == BossRushWidget.States.Difficulty)
					{
						flag = false;
						if (this.SelectedMode != BossRushManager.BossRushCourseMode.HARD || this.IsHardModeEnabled)
						{
							FadeWidget.instance.Fade(true, 1f, 0f, delegate
							{
								this.IsAllSelected = true;
								base.Hide();
							});
						}
					}
				}
				else if (!this.UnlockedCourses.Contains(this.SelectedCourse))
				{
					flag = false;
				}
				else
				{
					this.DeactivateResumeButtons();
					BossRushManager.BossRushCourseId selectedCourse = this.SelectedCourse;
					switch (selectedCourse)
					{
					case BossRushManager.BossRushCourseId.COURSE_A_1:
						this.ResumeButtonA1.gameObject.SetActive(true);
						this.ResumeButtonA1.SetData(1, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_A_1));
						break;
					case BossRushManager.BossRushCourseId.COURSE_A_2:
						this.ResumeButtonA2.gameObject.SetActive(true);
						this.ResumeButtonA2.SetData(2, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_A_2));
						break;
					case BossRushManager.BossRushCourseId.COURSE_A_3:
						this.ResumeButtonA3.gameObject.SetActive(true);
						this.ResumeButtonA3.SetData(3, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_A_3));
						break;
					default:
						if (selectedCourse != BossRushManager.BossRushCourseId.COURSE_B_1)
						{
							if (selectedCourse != BossRushManager.BossRushCourseId.COURSE_C_1)
							{
								if (selectedCourse == BossRushManager.BossRushCourseId.COURSE_D_1)
								{
									this.ResumeButtonD1.gameObject.SetActive(true);
									this.ResumeButtonD1.SetData(6, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_D_1));
								}
							}
							else
							{
								this.ResumeButtonC1.gameObject.SetActive(true);
								this.ResumeButtonC1.SetData(5, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_C_1));
							}
						}
						else
						{
							this.ResumeButtonB1.gameObject.SetActive(true);
							this.ResumeButtonB1.SetData(4, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_B_1));
						}
						break;
					}
					this.IsHardModeEnabled = Core.BossRushManager.IsModeUnlocked(this.SelectedCourse, BossRushManager.BossRushCourseMode.HARD);
					this.HardModeButton.interactable = this.IsHardModeEnabled;
					this.SetHighScore(this.NormalModeText, this.NormalRankMedal, BossRushManager.BossRushCourseMode.NORMAL);
					this.SetHighScore(this.HardModeText, this.HardRankMedal, BossRushManager.BossRushCourseMode.HARD);
				}
			}
			else if (!this.selectSaveSlotsWidget.CanLoadSelectedSlot)
			{
				flag = false;
			}
			else
			{
				this.CourseA1Button.SetData(1, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_A_1));
				this.CourseA2Button.SetData(2, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_A_2));
				this.CourseA3Button.SetData(3, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_A_3));
				this.CourseB1Button.SetData(4, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_B_1));
				this.CourseC1Button.SetData(5, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_C_1));
				this.CourseD1Button.SetData(6, this.UnlockedCourses.Contains(BossRushManager.BossRushCourseId.COURSE_D_1));
			}
			if (flag)
			{
				this.SetState(this.CurrentState + 1);
			}
		}

		private void DeactivateResumeButtons()
		{
			this.ResumeButtonA1.gameObject.SetActive(false);
			this.ResumeButtonA2.gameObject.SetActive(false);
			this.ResumeButtonA3.gameObject.SetActive(false);
			this.ResumeButtonB1.gameObject.SetActive(false);
			this.ResumeButtonC1.gameObject.SetActive(false);
			this.ResumeButtonD1.gameObject.SetActive(false);
		}

		public void SetSaveSlot(int slot)
		{
			this.CurrentSlot = slot;
		}

		private void SetState(BossRushWidget.States state)
		{
			this.CurrentState = state;
			for (int i = 0; i < Enum.GetValues(typeof(BossRushWidget.States)).Length; i++)
			{
				if (i == (int)state)
				{
					this.stateWidgets[i].FadeShow(false, false, true);
				}
				else
				{
					this.stateWidgets[i].FadeHide();
				}
			}
		}

		private bool CheckFading()
		{
			bool flag = true;
			foreach (BasicUIBlockingWidget basicUIBlockingWidget in this.stateWidgets)
			{
				flag = (flag && !basicUIBlockingWidget.IsFading);
			}
			return flag;
		}

		private void SetHighScore(Text control, Image rank, BossRushManager.BossRushCourseMode mode)
		{
			string newValue = "-- : -- : --";
			BossRushHighScore score = Core.BossRushManager.GetHighScore(this.SelectedCourse, mode);
			if (score != null)
			{
				newValue = score.RunDurationInString();
				rank.gameObject.SetActive(true);
				rank.sprite = this.AllRanksMedals.Find((RankMedal x) => x.score == score.Score).sprite;
			}
			else
			{
				rank.gameObject.SetActive(false);
			}
			control.text = ScriptLocalization.UI_BossRush.TEXT_BESTTIME.Replace("%", newValue);
		}

		[BoxGroup("Widgets", true, false, 0)]
		public BasicUIBlockingWidget SlotSelector;

		[BoxGroup("Widgets", true, false, 0)]
		public BasicUIBlockingWidget CourseSelector;

		[BoxGroup("Widgets", true, false, 0)]
		public BasicUIBlockingWidget DifficultySelector;

		[BoxGroup("Slots", true, false, 0)]
		public SelectSaveSlots selectSaveSlotsWidget;

		[BoxGroup("Course", true, false, 0)]
		public RectTransform CoursesRoot;

		[BoxGroup("Course", true, false, 0)]
		public BossRushButton CourseA1Button;

		[BoxGroup("Course", true, false, 0)]
		public BossRushButton CourseA2Button;

		[BoxGroup("Course", true, false, 0)]
		public BossRushButton CourseA3Button;

		[BoxGroup("Course", true, false, 0)]
		public BossRushButton CourseB1Button;

		[BoxGroup("Course", true, false, 0)]
		public BossRushButton CourseC1Button;

		[BoxGroup("Course", true, false, 0)]
		public BossRushButton CourseD1Button;

		[BoxGroup("Course", true, false, 0)]
		public GameObject LeftArrowGO;

		[BoxGroup("Course", true, false, 0)]
		public GameObject RightArrowGO;

		[BoxGroup("Course", true, false, 0)]
		public Text HowToUnlockText;

		[BoxGroup("Difficult", true, false, 0)]
		public Text NormalModeText;

		[BoxGroup("Difficult", true, false, 0)]
		public Text HardModeText;

		[BoxGroup("Difficult", true, false, 0)]
		public Button HardModeButton;

		[BoxGroup("Difficult", true, false, 0)]
		public BossRushButton ResumeButtonA1;

		[BoxGroup("Difficult", true, false, 0)]
		public BossRushButton ResumeButtonA2;

		[BoxGroup("Difficult", true, false, 0)]
		public BossRushButton ResumeButtonA3;

		[BoxGroup("Difficult", true, false, 0)]
		public BossRushButton ResumeButtonB1;

		[BoxGroup("Difficult", true, false, 0)]
		public BossRushButton ResumeButtonC1;

		[BoxGroup("Difficult", true, false, 0)]
		public BossRushButton ResumeButtonD1;

		[BoxGroup("Difficult", true, false, 0)]
		public Image NormalRankMedal;

		[BoxGroup("Difficult", true, false, 0)]
		public Image HardRankMedal;

		[BoxGroup("Difficult", true, false, 0)]
		public List<RankMedal> AllRanksMedals;

		[BoxGroup("Controls", true, false, 0)]
		public GameObject AcceptButton;

		[BoxGroup("Controls", true, false, 0)]
		public GameObject AcceptButtonDisabled;

		private BossRushWidget.States CurrentState;

		private List<BasicUIBlockingWidget> stateWidgets;

		private List<BossRushManager.BossRushCourseId> UnlockedCourses = new List<BossRushManager.BossRushCourseId>();

		private bool IsHardModeEnabled;

		public enum States
		{
			Slot,
			Course,
			Difficulty
		}
	}
}
