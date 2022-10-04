using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.FrameworkCore
{
	[CreateAssetMenu(fileName = "skill", menuName = "Blasphemous/Unlockable Skill")]
	public class UnlockableSkill : ScriptableObject, ILocalizable
	{
		public void OnIdChanged(string value)
		{
			this.id = value.Replace(' ', '_').ToUpper();
		}

		public string GetParentSkill()
		{
			if (this.parentSkill == "NO DEPENDENCY")
			{
				return string.Empty;
			}
			return this.parentSkill;
		}

		public string GetBaseTranslationID()
		{
			return "UnlockableSkill/" + this.id;
		}

		private IList<string> SkillsValues()
		{
			return null;
		}

		[OnValueChanged("OnIdChanged", false)]
		public string id = string.Empty;

		public string caption = string.Empty;

		[TextArea(3, 10)]
		public string description = string.Empty;

		public string instructions = string.Empty;

		public int tier;

		public int cost = 500;

		public bool unlocked;

		[ValueDropdown("SkillsValues")]
		public string parentSkill = string.Empty;

		public Sprite bigImage;

		public Sprite smallImage;

		public Sprite smallImageBuyed;

		private const string NO_DEPENDENCY = "NO DEPENDENCY";
	}
}
