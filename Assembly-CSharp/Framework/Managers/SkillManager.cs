using System;
using System.Collections.Generic;
using System.Linq;
using Framework.FrameworkCore;
using I2.Loc;
using UnityEngine;

namespace Framework.Managers
{
	public class SkillManager : GameSystem, PersistentInterface
	{
		public override void Start()
		{
			LocalizationManager.OnLocalizeEvent += new LocalizationManager.OnLocalizeCallback(this.OnLocalizationChange);
			this.LoadAllSkills(false);
		}

		private void LoadAllSkills(bool resetSkillUnlock = false)
		{
			this.TierConfiguration = Resources.Load<UnlockableSkillConfiguration>("Skill/TIER_CONFIG");
			if (!this.TierConfiguration)
			{
				Debug.LogError("Skill Manager: Config file NOT found: Skill/TIER_CONFIG");
			}
			UnlockableSkill[] array = Resources.LoadAll<UnlockableSkill>("Skill/");
			this.allSkills.Clear();
			this.tierSkills.Clear();
			foreach (UnlockableSkill unlockableSkill in array)
			{
				this.allSkills[unlockableSkill.id] = unlockableSkill;
				if (resetSkillUnlock)
				{
					this.allSkills[unlockableSkill.id].unlocked = false;
				}
				if (!this.tierSkills.ContainsKey(unlockableSkill.tier))
				{
					this.tierSkills[unlockableSkill.tier] = new List<UnlockableSkill>();
				}
				this.tierSkills[unlockableSkill.tier].Add(unlockableSkill);
			}
			Log.Debug("Skills", this.allSkills.Count.ToString() + " skills loaded succesfully.", null);
			SkillManager.currentLanguage = string.Empty;
			this.OnLocalizationChange();
		}

		public override void AllInitialized()
		{
			Core.Persistence.AddPersistentManager(this);
		}

		public int GetMaxSkillsTier()
		{
			return this.tierSkills.Keys.Max((int x) => x);
		}

		public UnlockableSkill GetSkill(string skill)
		{
			if (this.allSkills.ContainsKey(skill))
			{
				return this.allSkills[skill];
			}
			return null;
		}

		public List<UnlockableSkill> GetSkillsByTier(int tier)
		{
			if (this.tierSkills.ContainsKey(tier))
			{
				return this.tierSkills[tier];
			}
			return new List<UnlockableSkill>();
		}

		public int GetLockedSkillsNumber()
		{
			return (from p in this.allSkills.Values
			where !p.unlocked
			select p).Count<UnlockableSkill>();
		}

		public int GetUnLockedSkillsNumber()
		{
			return (from p in this.allSkills.Values
			where p.unlocked
			select p).Count<UnlockableSkill>();
		}

		public bool IsSkillUnlocked(string skill)
		{
			bool result = false;
			if (this.allSkills.ContainsKey(skill))
			{
				result = this.allSkills[skill].unlocked;
			}
			return result;
		}

		public bool CanUnlockSkill(string skill, bool ignoreChecks = false)
		{
			bool flag = false;
			if (this.allSkills.ContainsKey(skill))
			{
				flag = !this.allSkills[skill].unlocked;
				if (flag && !ignoreChecks)
				{
					flag = this.CanUnlockSkillNoCheckPoints(skill);
					flag = (flag && Core.Logic.Penitent.Stats.Purge.Current >= (float)this.allSkills[skill].cost);
				}
			}
			return flag;
		}

		public bool CanUnlockSkillNoCheckPoints(string skill)
		{
			bool flag = false;
			if (this.allSkills.ContainsKey(skill))
			{
				flag = (!this.allSkills[skill].unlocked && this.GetCurrentMeaCulpa() >= (float)this.allSkills[skill].tier);
				string parentSkill = this.allSkills[skill].GetParentSkill();
				if (flag && parentSkill != string.Empty)
				{
					if (this.allSkills.ContainsKey(parentSkill))
					{
						flag = this.allSkills[parentSkill].unlocked;
					}
					else
					{
						Debug.Log(string.Concat(new string[]
						{
							"SkillManager: ",
							skill,
							"  Parent is ",
							parentSkill,
							" that can be found"
						}));
					}
				}
			}
			return flag;
		}

		public bool UnlockSkill(string skill, bool ignoreChecks = false)
		{
			bool flag = this.CanUnlockSkill(skill, ignoreChecks);
			if (flag)
			{
				this.allSkills[skill].unlocked = true;
				Core.Logic.Penitent.Stats.Purge.Current -= (float)this.allSkills[skill].cost;
			}
			return flag;
		}

		public bool LockSkill(string skill)
		{
			bool result = false;
			if (this.allSkills.ContainsKey(skill))
			{
				result = this.allSkills[skill].unlocked;
				this.allSkills[skill].unlocked = false;
			}
			return result;
		}

		public float GetPurgePoints()
		{
			return Core.Logic.Penitent.Stats.Purge.Current;
		}

		public void AddPurgePoints(float points)
		{
			Core.Logic.Penitent.Stats.Purge.Current += points;
		}

		public float GetCurrentMeaCulpa()
		{
			return Core.Logic.Penitent.Stats.MeaCulpa.Final;
		}

		private void OnLocalizationChange()
		{
			if (SkillManager.currentLanguage != LocalizationManager.CurrentLanguage)
			{
				if (SkillManager.currentLanguage != string.Empty)
				{
					Log.Debug("Skills", "Language change, localize items to: " + LocalizationManager.CurrentLanguage, null);
				}
				SkillManager.currentLanguage = LocalizationManager.CurrentLanguage;
				LanguageSource mainLanguageSource = LocalizationManager.GetMainLanguageSource();
				int languageIndexFromCode = mainLanguageSource.GetLanguageIndexFromCode(LocalizationManager.CurrentLanguageCode, true);
				foreach (UnlockableSkill unlockableSkill in this.allSkills.Values)
				{
					foreach (string text in SkillManager.LANGUAGE_ELEMENT_LIST)
					{
						string text2 = unlockableSkill.GetBaseTranslationID() + "_" + text.ToUpper();
						TermData termData = mainLanguageSource.GetTermData(text2, false);
						if (termData == null)
						{
							Debug.LogError("Term " + text2 + " not found in Inventory Localization");
						}
						else
						{
							string text3 = termData.Languages[languageIndexFromCode];
							if (text != null)
							{
								if (!(text == "caption"))
								{
									if (!(text == "description"))
									{
										if (text == "instructions")
										{
											unlockableSkill.instructions = text3;
										}
									}
									else
									{
										unlockableSkill.description = text3;
									}
								}
								else
								{
									unlockableSkill.caption = text3;
								}
							}
						}
					}
				}
			}
		}

		public int GetOrder()
		{
			return 1;
		}

		public string GetPersistenID()
		{
			return "ID_SKILLS";
		}

		public void ResetPersistence()
		{
			this.LoadAllSkills(true);
		}

		public PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			SkillManager.SkillsPersistenceData skillsPersistenceData = new SkillManager.SkillsPersistenceData();
			foreach (UnlockableSkill unlockableSkill in this.allSkills.Values)
			{
				skillsPersistenceData.Skills[unlockableSkill.id] = unlockableSkill.unlocked;
			}
			return skillsPersistenceData;
		}

		public void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			SkillManager.SkillsPersistenceData skillsPersistenceData = (SkillManager.SkillsPersistenceData)data;
			foreach (KeyValuePair<string, bool> keyValuePair in skillsPersistenceData.Skills)
			{
				this.allSkills[keyValuePair.Key].unlocked = keyValuePair.Value;
			}
		}

		public static readonly string[] LANGUAGE_ELEMENT_LIST = new string[]
		{
			"caption",
			"description",
			"instructions"
		};

		private const string SKILL_RESOUCE_DIR = "Skill/";

		private const string SKILL_RESOUCE_CONFIG = "Skill/TIER_CONFIG";

		private Dictionary<string, UnlockableSkill> allSkills = new Dictionary<string, UnlockableSkill>();

		private Dictionary<int, List<UnlockableSkill>> tierSkills = new Dictionary<int, List<UnlockableSkill>>();

		private static string currentLanguage = string.Empty;

		private UnlockableSkillConfiguration TierConfiguration;

		private const string PERSITENT_ID = "ID_SKILLS";

		[Serializable]
		public class SkillsPersistenceData : PersistentManager.PersistentData
		{
			public SkillsPersistenceData() : base("ID_SKILLS")
			{
			}

			public Dictionary<string, bool> Skills = new Dictionary<string, bool>();
		}
	}
}
