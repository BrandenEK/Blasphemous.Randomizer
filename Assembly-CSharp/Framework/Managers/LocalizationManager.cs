using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Framework.FrameworkCore;
using Framework.Util;
using Gameplay.UI;
using I2.Loc;
using Rewired;
using Sirenix.Utilities;
using Steamworks;
using TMPro;
using Tools.DataContainer;
using Tools.UI;
using UnityEngine;

namespace Framework.Managers
{
	public class LocalizationManager : GameSystem
	{
		public override void Initialize()
		{
			Singleton<Core>.Instance.StartCoroutine(this.WaitForCoreAnContinue());
		}

		private IEnumerator WaitForCoreAnContinue()
		{
			yield return new WaitUntil(() => Core.ready);
			if (!UIController.instance.GetOptionsWidget().ReadOptionsFromFile())
			{
				this.SteamLanguageChange();
			}
			yield break;
		}

		private void SteamLanguageChange()
		{
			if (SteamManager.Initialized)
			{
				Debug.Log("LocalizationManager::Initialize: SteamManager is Initialized!");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("spanish", "es");
				dictionary.Add("english", "en");
				dictionary.Add("french", "fr");
				dictionary.Add("german", "de");
				dictionary.Add("italian", "it");
				dictionary.Add("schinese", "zh");
				dictionary.Add("tchinese", "zh");
				dictionary.Add("russian", "ru");
				dictionary.Add("japanese", "ja");
				dictionary.Add("brazilian", "pt-BR");
				string currentGameLanguage = SteamApps.GetCurrentGameLanguage();
				Debug.Log("LocalizationManager::Initialize: SteamApps.GetCurrentGameLanguage() returns the following value: " + currentGameLanguage);
				try
				{
					string text = dictionary[currentGameLanguage];
					Debug.Log("LocalizationManager::Initialize: gameLanCode has the following value: " + text);
					int indexByLanguageCode = Core.Localization.GetIndexByLanguageCode(text);
					Debug.Log("LocalizationManager::Initialize: Core.Localization.GetIndexByLanguageCode returns the following value: " + indexByLanguageCode);
					Core.Localization.SetLanguageByIdx(indexByLanguageCode);
					Debug.Log("LocalizationManager::Initialize: Language setted!");
				}
				catch (KeyNotFoundException)
				{
					Debug.Log("LocalizationManager::Initialize: Language not found!");
				}
			}
			else
			{
				Debug.Log("LocalizationManager::Initialize: SteamManager is not Initialized!");
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event LocalizationManager.OnLocalizeCallback OnLocalizeAudioEvent;

		public static LanguageSource GetMainLanguageSource()
		{
			string currentLanguage = LocalizationManager.CurrentLanguage;
			return LocalizationManager.Sources[0];
		}

		public void SetNextLanguage()
		{
			string text = LocalizationManager.CurrentLanguageCode;
			LanguageSource lang = LocalizationManager.Sources[0];
			int idx = lang.GetLanguageIndexFromCode(text, true);
			idx++;
			if (idx >= lang.mLanguages.Count)
			{
				idx = 0;
			}
			while (!this.GetAllEnabledLanguages().Exists((LanguageData x) => lang.mLanguages[idx].Code == x.Code))
			{
				idx++;
				if (idx >= lang.mLanguages.Count)
				{
					idx = 0;
				}
			}
			text = lang.mLanguages[idx].Code;
			LocalizationManager.CurrentLanguageCode = text;
		}

		public void SetLanguageByIdx(int idx)
		{
			if (idx >= this.GetAllEnabledLanguages().Count)
			{
				idx = 0;
			}
			LocalizationManager.CurrentLanguageCode = this.GetAllEnabledLanguages()[idx].Code;
		}

		public int GetCurrentLanguageIndex()
		{
			string currentLanguageCode = LocalizationManager.CurrentLanguageCode;
			return this.GetIndexByLanguageCode(currentLanguageCode);
		}

		public string GetCurrentLanguageCode()
		{
			return LocalizationManager.CurrentLanguageCode;
		}

		public string GetLanguageCodeByIndex(int index)
		{
			return this.GetAllEnabledLanguages()[index].Code;
		}

		public string GetLanguageNameByIndex(int index)
		{
			return this.GetAllEnabledLanguages()[index].Name;
		}

		public int GetIndexByLanguageCode(string languageCode)
		{
			return this.GetAllEnabledLanguages().FindIndex((LanguageData x) => x.Code == languageCode);
		}

		public Font GetFontByLanguageName(string languageName)
		{
			Font result = null;
			LanguageSource languageSource = LocalizationManager.Sources[0];
			string value = GameConstants.DefaultFont;
			if (GameConstants.FontByLanguages.ContainsKey(languageName))
			{
				value = GameConstants.FontByLanguages[languageName];
			}
			foreach (Font font in LinqExtensions.FilterCast<Font>(languageSource.Assets))
			{
				if (font.name.StartsWith(value))
				{
					result = font;
					break;
				}
			}
			return result;
		}

		public List<string> GetAllEnabledLanguagesNames()
		{
			List<string> list = new List<string>();
			foreach (LanguageData languageData in this.GetAllEnabledLanguages())
			{
				list.Add(languageData.Name);
			}
			return list;
		}

		public List<LanguageData> GetAllEnabledLanguages()
		{
			List<LanguageData> list = new List<LanguageData>();
			LanguageSource languageSource = LocalizationManager.Sources[0];
			foreach (LanguageData languageData in languageSource.mLanguages)
			{
				if (languageData.IsEnabled())
				{
					list.Add(languageData);
				}
			}
			return list;
		}

		public void AddLanguageSource(string SourceName)
		{
			GameObject asset = ResourceManager.pInstance.GetAsset<GameObject>(SourceName);
			LanguageSource languageSource = (!asset) ? null : asset.GetComponent<LanguageSource>();
			if (languageSource && !LocalizationManager.Sources.Contains(languageSource))
			{
				LocalizationManager.AddSource(languageSource);
			}
		}

		public string Get(string key)
		{
			string translation = LocalizationManager.GetTranslation(key, true, 0, true, false, null, null);
			if (StringExtensions.IsNullOrWhitespace(translation))
			{
				return "[!LOC_" + key.ToUpper() + "]";
			}
			return translation;
		}

		public static string ParseMeshPro(string localizedText, string idString, TextMeshProUGUI textMesh = null)
		{
			Regex regex = new Regex("\\[(.*?)\\]");
			LocalizationManager.currentId = idString;
			LocalizationManager.currentTextMeshProFont = textMesh;
			Regex regex2 = regex;
			if (LocalizationManager.<>f__mg$cache0 == null)
			{
				LocalizationManager.<>f__mg$cache0 = new MatchEvaluator(LocalizationManager.ProcessTag);
			}
			return regex2.Replace(localizedText, LocalizationManager.<>f__mg$cache0);
		}

		public int CurrentAudioLanguageIndex
		{
			get
			{
				return this.currentAudioLanguageIndex;
			}
			set
			{
				if (value >= 0 && value <= this.AudioLanguages.Count)
				{
					this.currentAudioLanguageIndex = value;
					if (LocalizationManager.OnLocalizeAudioEvent != null)
					{
						LocalizationManager.OnLocalizeAudioEvent(this.GetCurrentAudioLanguageCode());
					}
				}
			}
		}

		public List<string> GetAllAudioLanguagesNames()
		{
			return this.AudioLanguages;
		}

		public string GetCurrentAudioLanguageCode()
		{
			return LocalizationManager.AudioLanguagesKeys[this.currentAudioLanguageIndex];
		}

		public string GetCurrentAudioLanguage()
		{
			return this.AudioLanguages[this.currentAudioLanguageIndex];
		}

		public string GetCurrentAudioLanguageByIndex(int index)
		{
			return this.AudioLanguages[index];
		}

		private static string ProcessTag(Match m)
		{
			string value = m.Groups[1].Value;
			string result = value;
			string[] array = value.Split(new char[]
			{
				':'
			});
			if (array.Length != 2)
			{
				Debug.LogWarning("Localization PARSE error ID : " + LocalizationManager.currentId + " Tag element different 2");
			}
			else
			{
				string text = array[0].ToUpper();
				string text2 = array[1];
				string str = (!(LocalizationManager.CurrentLanguageCode == "zh")) ? string.Empty : " ";
				if (text != null)
				{
					if (text == "ICON")
					{
						string spriteName = "ICON_" + text2;
						TMP_Sprite spriteData = LocalizationManager.GetSpriteData(spriteName);
						return string.Format("<size={0}><sprite name=\"{1}\"></size>", spriteData.height, spriteData.name);
					}
					if (text == "ACT")
					{
						return str + LocalizationManager.ParseAction(text2);
					}
					if (text == "VAR")
					{
						Debug.LogWarning("Localization PARSE error ID : " + LocalizationManager.currentId + " VAR NOT IMPLEMENTED");
						return result;
					}
				}
				Debug.LogWarning("Localization PARSE error ID : " + LocalizationManager.currentId + " Unknow prefix " + text);
			}
			return result;
		}

		private static string ParseAction(string action)
		{
			string result = "[" + action + "]";
			JoystickType activeJoystickModel = Core.Input.ActiveJoystickModel;
			ControllerType activeControllerType = Core.Input.ActiveControllerType;
			string str = "KB_";
			if (activeControllerType != null)
			{
				switch (activeJoystickModel)
				{
				case JoystickType.PlayStation:
					str = "PS_";
					break;
				case JoystickType.XBOX:
				case JoystickType.Generic:
					str = "XBOX_";
					break;
				case JoystickType.Switch:
					str = "SWITCH_";
					break;
				}
			}
			InputIcon.AxisCheck axisCheck = InputIcon.AxisCheck.None;
			if (action.EndsWith("+"))
			{
				axisCheck = InputIcon.AxisCheck.Positive;
				action = action.Remove(action.Length - 1);
			}
			else if (action.EndsWith("-"))
			{
				axisCheck = InputIcon.AxisCheck.Negative;
				action = action.Remove(action.Length - 1);
			}
			Player player = ReInput.players.GetPlayer(0);
			InputAction action2 = ReInput.mapping.GetAction(action);
			ActionElementMap actionElementMap = null;
			if (action2 != null)
			{
				AxisRange axisRange = (axisCheck != InputIcon.AxisCheck.Positive) ? 2 : 1;
				actionElementMap = Core.ControlRemapManager.FindLastElementMapByInputAction(action2, axisRange, Core.Input.ActiveController);
			}
			if (actionElementMap == null)
			{
				Debug.LogWarning("Localization PARSE error ID : " + LocalizationManager.currentId + " Action not found: " + action);
			}
			else if (activeControllerType == null)
			{
				string name = InputIcon.GetButtonDescriptionByButtonKey(actionElementMap.elementIdentifierName).icon.name;
				if (name.Equals("KB_BLANK"))
				{
					result = LocalizationManager.GetKeyboardIconRtfByLanguage(actionElementMap.keyCode.ToString());
				}
				else
				{
					TMP_Sprite spriteData = LocalizationManager.GetSpriteData(name);
					result = string.Format("<size={0}><sprite name=\"{1}\"></size>", spriteData.height, name);
				}
			}
			else
			{
				string text = actionElementMap.elementIdentifierName.ToUpper();
				if (text.StartsWith("BUTTON "))
				{
					result = LocalizationManager.GetJoystickIconRtfByLanguage(text[text.Length - 1] + string.Empty);
				}
				else
				{
					if (activeJoystickModel == JoystickType.Generic)
					{
						if (actionElementMap.elementIdentifierName.ToUpper().Equals("RIGHT TRIGGER"))
						{
							text = "START";
						}
						else if (actionElementMap.elementIdentifierName.ToUpper().Equals("LEFT TRIGGER"))
						{
							text = "BACK";
						}
					}
					string text2 = str + text;
					TMP_Sprite spriteData2 = LocalizationManager.GetSpriteData(text2);
					string arg = (spriteData2 != null) ? string.Format("<size={0}>", spriteData2.height) : string.Empty;
					string arg2 = (spriteData2 != null) ? "</size>" : string.Empty;
					result = string.Format("{0}<sprite name=\"{1}\">{2}", arg, text2, arg2);
				}
			}
			return result;
		}

		private static string GetKeyboardIconRtfByLanguage(string keycode)
		{
			LocalizationSpacingData[] array = Resources.LoadAll<LocalizationSpacingData>("UI");
			int num = -22;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			float num5 = 0f;
			int num6 = 22;
			int num7 = 26;
			float num8 = 0f;
			bool flag = true;
			TMP_Sprite spriteData = LocalizationManager.GetSpriteData("KB_BLANK");
			if (spriteData != null)
			{
				num6 = (int)spriteData.width;
				num7 = (int)spriteData.xAdvance;
			}
			foreach (LocalizationSpacingData localizationSpacingData in array)
			{
				if (!(localizationSpacingData.Language != LocalizationManager.CurrentLanguage))
				{
					num2 = localizationSpacingData.extraSpacing;
					num4 = localizationSpacingData.extraAfterSpacing;
					num8 = ((!localizationSpacingData.addCharacterWidth) ? 0f : 1f);
					num5 = localizationSpacingData.verticalSpacing;
					break;
				}
			}
			if (LocalizationManager.currentTextMeshProFont)
			{
				int num9 = (int)LocalizationManager.currentTextMeshProFont.GetPreferredValues(keycode).x;
				num = -num6 + num2;
				num3 = (int)((float)(num7 - num6 + num4) + num8 * (float)num9 / 2f);
			}
			if (keycode.Length > 1)
			{
				flag = false;
				num = 0;
				num3 = 0;
			}
			return string.Format("<size={5}>{6}</size><space={0}><voffset={1}px><color={4}>{2}</color><space={3}></voffset>", new object[]
			{
				num,
				num5,
				keycode,
				num3,
				"#F8E4C7FF",
				spriteData.height,
				(!flag) ? string.Empty : "<sprite name=\"KB_BLANK\">"
			});
		}

		private static string GetJoystickIconRtfByLanguage(string keycode)
		{
			string text = "<sprite name=\"CONSOLE_BLANK\">";
			string text2 = "<space=-12>";
			string text3 = " ";
			string text4 = string.Empty;
			string text5 = "</voffset>";
			string text6 = "<voffset=4px>";
			string currentLanguageCode = LocalizationManager.CurrentLanguageCode;
			if (currentLanguageCode != null)
			{
				if (!(currentLanguageCode == "ru"))
				{
					if (currentLanguageCode == "zh" || currentLanguageCode == "ja")
					{
						text4 = "<voffset=-2px>";
						text2 = "<space=-13>";
						text3 = "<space=10></voffset>";
					}
				}
				else
				{
					text2 = "<space=-11>";
				}
			}
			return string.Concat(new string[]
			{
				text6,
				text,
				text5,
				text2,
				text4,
				keycode,
				text3
			});
		}

		private static TMP_Sprite GetSpriteData(string spriteName)
		{
			int num = -1;
			if (!LocalizationManager._cachedIconData)
			{
				LocalizationManager._cachedIconData = Resources.Load<TMP_SpriteAsset>("Input/all_platform_buttons");
			}
			if (LocalizationManager._cachedIconData)
			{
				num = LocalizationManager._cachedIconData.GetSpriteIndexFromName(spriteName);
			}
			return (num < 0) ? null : LocalizationManager._cachedIconData.spriteInfoList[num];
		}

		public string GetValueWithParam(string scriptKey, string key, string value)
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{
					key,
					value
				}
			};
			return this.GetValueWithParams(scriptKey, parameters);
		}

		public string GetValueWithParams(string scriptKey, Dictionary<string, string> parameters)
		{
			string text = scriptKey;
			foreach (KeyValuePair<string, string> keyValuePair in parameters)
			{
				text = text.Replace("{[" + keyValuePair.Key + "]}", keyValuePair.Value);
			}
			return text;
		}

		private static string currentId = string.Empty;

		private static TextMeshProUGUI currentTextMeshProFont;

		private List<string> AudioLanguages = new List<string>
		{
			"English",
			"Spanish"
		};

		public static List<string> AudioLanguagesKeys = new List<string>
		{
			"EN",
			"ES"
		};

		private int currentAudioLanguageIndex;

		private static TMP_SpriteAsset _cachedIconData;

		[CompilerGenerated]
		private static MatchEvaluator <>f__mg$cache0;

		public delegate void OnLocalizeCallback(string languageKey);
	}
}
