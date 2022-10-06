using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FullSerializer;
using Gameplay.GameControllers.Effects.Player.Recolor;
using Gameplay.UI;
using Tools;
using UnityEngine;

namespace Framework.Managers
{
	public class ColorPaletteManager : GameSystem
	{
		private void InitializeDlcPalettes()
		{
			this.palettesByDlc = new Dictionary<string, string>();
			this.palettesByDlc.Add("BACKER_DLC", "PENITENT_BACKER");
			this.palettesByDlc.Add("DIGITAL_DELUXE_DLC", "PENITENT_DELUXE");
			this.dlcPalettesIds = new List<string>
			{
				"PENITENT_BACKER",
				"PENITENT_DELUXE"
			};
			this.dlcs = new List<string>
			{
				"BACKER_DLC",
				"DIGITAL_DELUXE_DLC"
			};
			this.dlcPalettes = new Dictionary<string, bool>();
			foreach (string text in this.dlcs)
			{
				bool value = Core.DLCManager.IsDLCDownloaded(text, false);
				this.dlcPalettes.Add(this.palettesByDlc[text], value);
			}
		}

		public override void Initialize()
		{
			string pathSkinSettings = this.GetPathSkinSettings();
			this.InitializeDlcPalettes();
			this.palettes = Resources.Load<ColorPaletteDictionary>("Color Palettes/AVAILABLE_COLOR_PALETTES");
			if (!File.Exists(pathSkinSettings) || this.FileIsEmpty(pathSkinSettings))
			{
				File.CreateText(pathSkinSettings).Close();
				this.currentColorPaletteId = "PENITENT_DEFAULT";
				this.palettesStates = new Dictionary<string, bool>();
				this.palettesStates.Add("PENITENT_DEFAULT", true);
				this.SetCurrentSkinToSkinSettings("PENITENT_DEFAULT");
			}
			else
			{
				Dictionary<string, fsData> skinSettings = this.ParseCurrentSkinSettings(pathSkinSettings);
				this.palettesStates = this.GetAllPalettesStatesFromDictionary(skinSettings);
				this.currentColorPaletteId = this.GetCurrentSkinFromDictionary(skinSettings);
			}
		}

		private string GetPathSkinSettings()
		{
			return PersistentManager.GetPathAppSettings("/app_settings");
		}

		private bool FileIsEmpty(string path)
		{
			string s = File.ReadAllText(path);
			bool result;
			try
			{
				byte[] bytes = Convert.FromBase64String(s);
				string @string = Encoding.UTF8.GetString(bytes);
				fsData fsData;
				fsResult fsResult = fsJsonParser.Parse(@string, ref fsData);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("ParseCurrentSkinSettings: parsing error: " + fsResult.FormattedMessages);
					result = true;
				}
				else
				{
					if (fsData != null)
					{
						Dictionary<string, fsData> asDictionary = fsData.AsDictionary;
						if (asDictionary.Keys.Count > 0)
						{
							return false;
						}
					}
					result = true;
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Something went wrong! File is not empty but probably not base64. >> Exception: " + ex.Message);
				result = true;
			}
			return result;
		}

		private Dictionary<string, fsData> ParseCurrentSkinSettings(string path)
		{
			Dictionary<string, fsData> dictionary = new Dictionary<string, fsData>();
			if (File.Exists(path))
			{
				string s = File.ReadAllText(path);
				byte[] bytes = Convert.FromBase64String(s);
				string @string = Encoding.UTF8.GetString(bytes);
				fsData fsData;
				fsResult fsResult = fsJsonParser.Parse(@string, ref fsData);
				if (fsResult.Failed && !fsResult.FormattedMessages.Equals("No input"))
				{
					Debug.LogError("ParseCurrentSkinSettings: parsing error: " + fsResult.FormattedMessages);
				}
				else if (fsData != null)
				{
					dictionary = fsData.AsDictionary;
					dictionary = this.CleanOldSaveFileFormat(dictionary);
				}
			}
			return dictionary;
		}

		private Dictionary<string, fsData> CleanOldSaveFileFormat(Dictionary<string, fsData> skinSettings)
		{
			foreach (string text in this.dlcPalettesIds)
			{
				if (skinSettings.ContainsKey(text))
				{
					skinSettings.Remove(text);
				}
				string key = text + "_UNLOCKED";
				if (skinSettings.ContainsKey(key))
				{
					skinSettings.Remove(key);
				}
			}
			return skinSettings;
		}

		private string GetCurrentSkinFromDictionary(Dictionary<string, fsData> skinSettings)
		{
			if (skinSettings.ContainsKey("CURRENT_SKIN"))
			{
				string asString = skinSettings["CURRENT_SKIN"].AsString;
				if (this.IsColorPaletteUnlocked(asString))
				{
					return asString;
				}
			}
			return "PENITENT_DEFAULT";
		}

		private Dictionary<string, bool> GetAllPalettesStatesFromDictionary(Dictionary<string, fsData> skinSettings)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			dictionary.Add("PENITENT_DEFAULT", true);
			foreach (string text in skinSettings.Keys)
			{
				if (text.EndsWith("_UNLOCKED") && !text.Equals("PENITENT_DEFAULT_UNLOCKED"))
				{
					dictionary.Add(text.Replace("_UNLOCKED", string.Empty), skinSettings[text].AsBool);
				}
			}
			return dictionary;
		}

		public void InitializeSkinFile(string currentSkin)
		{
			string pathSkinSettings = this.GetPathSkinSettings();
			if (!File.Exists(pathSkinSettings))
			{
				File.CreateText(pathSkinSettings);
			}
			fsData fsData = fsData.CreateDictionary();
			foreach (string text in this.palettes.GetAllIds())
			{
				fsData.AsDictionary[text] = ((!(currentSkin == text)) ? fsData.False : fsData.True);
				fsData.AsDictionary[text + "_UNLOCKED"] = ((!this.IsColorPaletteUnlocked(text)) ? fsData.False : fsData.True);
			}
			fsData.AsDictionary["CURRENT_SKIN"] = new fsData(currentSkin);
			string s = fsJsonPrinter.CompressedJson(fsData);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			string encryptedData = Convert.ToBase64String(bytes);
			FileTools.SaveSecure(pathSkinSettings, encryptedData);
		}

		public void SetCurrentSkinToSkinSettings(string currentSkin)
		{
			this.InitializeSkinFile(currentSkin);
			this.SetCurrentColorPaletteId(currentSkin);
		}

		public void UnlockColorPalette(string colorPaletteId, bool showPopup = true)
		{
			if (!this.palettesStates.ContainsKey(colorPaletteId))
			{
				this.palettesStates.Add(colorPaletteId, false);
			}
			if (this.palettesStates[colorPaletteId])
			{
				return;
			}
			this.palettesStates[colorPaletteId] = true;
			if (showPopup)
			{
				UIController.instance.ShowUnlockPopup(colorPaletteId);
			}
			this.SetCurrentSkinToSkinSettings(this.currentColorPaletteId);
		}

		public void LockColorPalette(string colorPaletteId)
		{
			if (!this.palettesStates.ContainsKey(colorPaletteId))
			{
				return;
			}
			this.palettesStates[colorPaletteId] = false;
			if (this.currentColorPaletteId == colorPaletteId)
			{
				this.SetCurrentSkinToSkinSettings("PENITENT_DEFAULT");
			}
		}

		public void UnlockAlmsColorPalette()
		{
			this.UnlockColorPalette("PENITENT_ALMS", true);
		}

		public void UnlockBossRushColorPalette()
		{
			this.UnlockColorPalette("PENITENT_BOSSRUSH", true);
		}

		public void UnlockBossRushRankSColorPalette()
		{
			this.UnlockColorPalette("PENITENT_BOSSRUSH_S", true);
		}

		public void UnlockBossKonamiColorPalette()
		{
			this.UnlockColorPalette("PENITENT_KONAMI", true);
		}

		public void UnlockDemake1ColorPalette()
		{
			this.UnlockColorPalette("PENITENT_DEMAKE", true);
		}

		public void UnlockDemake2ColorPalette()
		{
			this.UnlockColorPalette("PENITENT_GAMEBOY", true);
		}

		public bool IsColorPaletteUnlocked(string colorPaletteId)
		{
			if (colorPaletteId.Equals("PENITENT_DEFAULT"))
			{
				return true;
			}
			if (this.dlcPalettesIds.Contains(colorPaletteId))
			{
				return this.dlcPalettes[colorPaletteId];
			}
			return this.palettesStates != null && this.palettesStates.ContainsKey(colorPaletteId) && this.palettesStates[colorPaletteId];
		}

		public List<string> GetAllUnlockedColorPalettesId()
		{
			List<string> list = new List<string>();
			foreach (string text in this.palettesStates.Keys)
			{
				if (this.palettesStates[text])
				{
					list.Add(text);
				}
			}
			foreach (string text2 in this.dlcPalettesIds)
			{
				if (this.dlcPalettes[text2])
				{
					list.Add(text2);
				}
			}
			return list;
		}

		public List<string> GetAllColorPalettesId()
		{
			return this.palettes.GetAllIds();
		}

		public string GetCurrentColorPaletteId()
		{
			return this.currentColorPaletteId;
		}

		public Sprite GetCurrentColorPaletteSprite()
		{
			return this.palettes.GetPalette(this.currentColorPaletteId);
		}

		public Sprite GetPaletteSpritePreview(string id)
		{
			return this.palettes.GetPreview(id);
		}

		public Sprite GetColorPaletteById(string id)
		{
			return this.palettes.GetPalette(id);
		}

		public void SetCurrentColorPaletteId(string colorPaletteId)
		{
			this.currentColorPaletteId = colorPaletteId;
		}

		private const string PALETTE_PATH = "Color Palettes/AVAILABLE_COLOR_PALETTES";

		private const string DEFAULT_SKIN = "PENITENT_DEFAULT";

		private const string BACKER_SKIN = "PENITENT_BACKER";

		private const string DELUXE_SKIN = "PENITENT_DELUXE";

		private const string ALMS_SKIN = "PENITENT_ALMS";

		private const string BOSSRUSH_SKIN = "PENITENT_BOSSRUSH";

		private const string BOSSRUSH_S_SKIN = "PENITENT_BOSSRUSH_S";

		private const string KONAMI_SKIN = "PENITENT_KONAMI";

		public const string DEMAKE1_SKIN = "PENITENT_DEMAKE";

		public const string DEMAKE2_SKIN = "PENITENT_GAMEBOY";

		private const string CURRENT_SKIN_KEY = "CURRENT_SKIN";

		private const string SKIN_SETTINGS_PATH = "/app_settings";

		private const string BACKER_DLC_NAME = "BACKER_DLC";

		private const string DELUXE_DLC_NAME = "DIGITAL_DELUXE_DLC";

		private string currentColorPaletteId = string.Empty;

		private Dictionary<string, bool> dlcPalettes;

		private List<string> dlcPalettesIds;

		private List<string> dlcs;

		private ColorPaletteDictionary palettes;

		private Dictionary<string, string> palettesByDlc;

		private Dictionary<string, bool> palettesStates;
	}
}
