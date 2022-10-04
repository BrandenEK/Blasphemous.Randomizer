using System;
using Framework.Achievements;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class AchievementElementWidget : MonoBehaviour
	{
		public void SetData(Achievement achievement)
		{
			this.HiddenRoot.SetActive(achievement.CurrentStatus == Achievement.Status.HIDDEN);
			this.NoHiddenRoot.SetActive(achievement.CurrentStatus != Achievement.Status.HIDDEN);
			this.ElementImage.sprite = achievement.Image;
			this.HeaderLoc.SetTerm(achievement.GetNameLocalizationTerm());
			this.ContentLoc.SetTerm(achievement.GetDescLocalizationTerm());
			this.Header.color = ((achievement.CurrentStatus != Achievement.Status.LOCKED) ? this.HeaderUnlocked : this.HeaderLocked);
			this.Content.color = ((achievement.CurrentStatus != Achievement.Status.LOCKED) ? this.ContentUnlocked : this.ContentLocked);
			this.ElementImage.color = ((achievement.CurrentStatus != Achievement.Status.LOCKED) ? Color.white : this.IconLocked);
			base.GetComponent<Image>().sprite = ((achievement.CurrentStatus != Achievement.Status.UNLOCKED) ? this.BgLocked : this.BgUnlocked);
		}

		[SerializeField]
		[BoxGroup("Widgets", true, false, 0)]
		private Image ElementImage;

		[SerializeField]
		[BoxGroup("Widgets", true, false, 0)]
		private Text Header;

		[SerializeField]
		[BoxGroup("Widgets", true, false, 0)]
		private Localize HeaderLoc;

		[SerializeField]
		[BoxGroup("Widgets", true, false, 0)]
		private Text Content;

		[SerializeField]
		[BoxGroup("Widgets", true, false, 0)]
		private Localize ContentLoc;

		[SerializeField]
		[BoxGroup("Widgets", true, false, 0)]
		private GameObject NoHiddenRoot;

		[SerializeField]
		[BoxGroup("Widgets", true, false, 0)]
		private GameObject HiddenRoot;

		[SerializeField]
		[BoxGroup("Colors", true, false, 0)]
		private Color HeaderLocked;

		[SerializeField]
		[BoxGroup("Colors", true, false, 0)]
		private Color ContentLocked;

		[SerializeField]
		[BoxGroup("Colors", true, false, 0)]
		private Color IconLocked;

		[SerializeField]
		[BoxGroup("Colors", true, false, 0)]
		private Color HeaderUnlocked;

		[SerializeField]
		[BoxGroup("Colors", true, false, 0)]
		private Color ContentUnlocked;

		[SerializeField]
		[BoxGroup("Background", true, false, 0)]
		private Sprite BgLocked;

		[SerializeField]
		[BoxGroup("Background", true, false, 0)]
		private Sprite BgUnlocked;
	}
}
