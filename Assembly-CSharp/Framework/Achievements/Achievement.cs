using System;
using Framework.Managers;
using Gameplay.UI;
using UnityEngine;

namespace Framework.Achievements
{
	[Serializable]
	public class Achievement
	{
		public Achievement(Achievement other)
		{
			this.Id = other.Id;
			this.Progress = other.Progress;
			this.Name = other.Name;
			this.Description = other.Description;
			this.PreserveProgressInNewGamePlus = other.PreserveProgressInNewGamePlus;
			this.Image = other.Image;
			this.CurrentStatus = other.CurrentStatus;
			this.CanBeHidden = other.CanBeHidden;
		}

		public Achievement(string id)
		{
			this.Id = id;
		}

		public string GetNameLocalizationTerm()
		{
			return this.GetLocalizationBase() + "_NAME";
		}

		public string GetDescLocalizationTerm()
		{
			return this.GetLocalizationBase() + "_DESC";
		}

		public void AddProgress(float progress)
		{
			if (this.IsGranted() || !Core.GameModeManager.ShouldProgressAchievements())
			{
				return;
			}
			this.Progress += progress;
			if (this.Progress >= 99.99f)
			{
				this.Progress = 100f;
			}
			this.SetAchievementProgress(this.Progress);
		}

		public void AddProgressSafeTo99(float progress)
		{
			if (this.IsGranted() || !Core.GameModeManager.ShouldProgressAchievements())
			{
				return;
			}
			this.Progress += progress;
			if (this.Progress >= 99.9f)
			{
				this.Progress = 100f;
			}
			this.SetAchievementProgress(this.Progress);
		}

		public void Grant()
		{
			if (this.IsGranted() || !Core.GameModeManager.ShouldProgressAchievements())
			{
				return;
			}
			this.Progress = 100f;
			this.SetAchievementProgress(this.Progress);
		}

		private void SetAchievementProgress(float progress)
		{
			Core.AchievementsManager.SetAchievementProgress(this.Id, progress);
			if (this.IsGranted())
			{
				UIController.instance.ShowPopupAchievement(this);
			}
		}

		public bool IsGranted()
		{
			return this.Progress == 100f;
		}

		public void Reset()
		{
			this.Progress = 0f;
		}

		private string GetLocalizationBase()
		{
			return "Achievements/" + this.Id;
		}

		public string Id;

		public float Progress;

		public string Name;

		public string Description;

		public Sprite Image;

		[HideInInspector]
		public Achievement.Status CurrentStatus;

		public bool CanBeHidden;

		public bool PreserveProgressInNewGamePlus;

		public enum Status
		{
			LOCKED,
			UNLOCKED,
			HIDDEN
		}
	}
}
