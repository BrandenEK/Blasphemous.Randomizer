using System;
using System.Collections.Generic;
using Framework.Achievements;
using Framework.Managers;
using FullSerializer;
using Gameplay.UI.Others.Buttons;
using Gameplay.UI.Widgets;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class ExtrasMenuWidget : SerializedMonoBehaviour
	{
		private ExtrasMenuWidget.STATE currentState
		{
			get
			{
				return this._currentState;
			}
			set
			{
				this._currentState = value;
				if (this.animator)
				{
					this.animator.SetInteger("STATUS", (int)this.currentState);
				}
			}
		}

		private void Awake()
		{
			this.extrasRoot.Clear();
			this.extrasRoot[ExtrasMenuWidget.MENU.ACHIEVEMENT] = this.AchievementRoot;
			this.extrasRoot[ExtrasMenuWidget.MENU.EXTRAS] = this.ExtrasRoot;
			this.extrasRoot[ExtrasMenuWidget.MENU.SKINSELECTOR] = this.SelectorRoot;
			this.serializer = new fsSerializer();
			this.currentState = ExtrasMenuWidget.STATE.STATE_OFF;
			this.animator = base.GetComponent<Animator>();
			this.currentMenu = ExtrasMenuWidget.MENU.EXTRAS;
			this.colorPaletteShader = Shader.Find("Maikel/MaikelSpriteMasterShader");
			if (this.colorPaletteShader == null)
			{
				Debug.LogError("Couldn't find shader: Maikel/MaikelSpriteMasterShader");
			}
			this.allSkins = Core.ColorPaletteManager.GetAllColorPalettesId();
			this.ResetMenus();
			this.CreateAchievements();
		}

		private void OnDestroy()
		{
			Core.Logic.ResumeGame();
		}

		private void Update()
		{
			if (this.rewired == null)
			{
				Player player = ReInput.players.GetPlayer(0);
				if (player == null)
				{
					return;
				}
				this.rewired = player;
			}
			float axisRaw = this.rewired.GetAxisRaw(48);
			float axisRaw2 = this.rewired.GetAxisRaw(49);
			bool flag = axisRaw < 0.3f && (double)axisRaw >= -0.3 && axisRaw2 < 0.3f && (double)axisRaw2 >= -0.3;
			if (this.currentState == ExtrasMenuWidget.STATE.STATE_EXTRAS)
			{
				ExtrasMenuWidget.MENU menu = this.currentMenu;
				if (menu == ExtrasMenuWidget.MENU.SKINSELECTOR)
				{
					if ((flag && this.lastHorizontalInOptions != 0f) || (flag && this.lastVerticalInOptions != 0f))
					{
						this.UpdateInputSkinSelectorOptions();
						this.lastHorizontalInOptions = 0f;
						this.lastVerticalInOptions = 0f;
					}
					else
					{
						if (!flag && axisRaw < 0f)
						{
							this.lastHorizontalInOptions = -1f;
						}
						else if (!flag && axisRaw > 0f)
						{
							this.lastHorizontalInOptions = 1f;
						}
						if (!flag && axisRaw2 < 0f)
						{
							this.lastVerticalInOptions = -1f;
						}
						else if (!flag && axisRaw2 > 0f)
						{
							this.lastVerticalInOptions = 1f;
						}
					}
				}
			}
		}

		public bool currentlyActive
		{
			get
			{
				return this.currentState != ExtrasMenuWidget.STATE.STATE_OFF;
			}
		}

		public void ShowExtras()
		{
			Core.Input.SetBlocker("INGAME_MENU", true);
			Core.Logic.SetState(LogicStates.Unresponsive);
			this.currentState = ExtrasMenuWidget.STATE.STATE_EXTRAS;
			this.ShowMenu(ExtrasMenuWidget.MENU.EXTRAS);
		}

		private void LinkAllButtons()
		{
			List<string> allUnlockedColorPalettesId = Core.ColorPaletteManager.GetAllUnlockedColorPalettesId();
			string text = this.allSkins[0];
			EventsButton first = this.skinSelectorSelectionElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[0]).element.GetComponentInChildren<EventsButton>();
			int i;
			for (i = 1; i <= this.allSkins.Count; i++)
			{
				if (i == this.allSkins.Count)
				{
					EventsButton componentInChildren = this.skinSelectorSelectionElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[0]).element.GetComponentInChildren<EventsButton>();
					this.LinkButtonHorizontal(first, componentInChildren);
				}
				else if (allUnlockedColorPalettesId.Contains(this.allSkins[i]))
				{
					EventsButton componentInChildren2 = this.skinSelectorSelectionElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[i]).element.GetComponentInChildren<EventsButton>();
					this.LinkButtonHorizontal(first, componentInChildren2);
					first = componentInChildren2;
					text = this.allSkins[i];
				}
			}
		}

		public void Hide()
		{
			if (FadeWidget.instance.Fading)
			{
				return;
			}
			this.ResetMenus();
			Core.Logic.SetState(LogicStates.Playing);
			this.currentState = ExtrasMenuWidget.STATE.STATE_OFF;
			Core.Input.SetBlocker("INGAME_MENU", false);
			UIController.instance.ShowMainMenu(string.Empty);
		}

		public void GoBack()
		{
			ExtrasMenuWidget.STATE currentState = this.currentState;
			if (currentState == ExtrasMenuWidget.STATE.STATE_EXTRAS)
			{
				if (this.currentMenu != ExtrasMenuWidget.MENU.EXTRAS)
				{
					EventSystem.current.SetSelectedGameObject(null);
					this.ShowMenu(ExtrasMenuWidget.MENU.EXTRAS);
				}
				else
				{
					this.Hide();
				}
			}
			if (this.soundBack != string.Empty)
			{
				Core.Audio.PlayOneShot(this.soundBack, default(Vector3));
			}
		}

		public void Option_OnSelect(GameObject item)
		{
			if (this.optionLastSelected)
			{
				this.SetOptionSelected(this.optionLastSelected, false);
			}
			this.optionLastSelected = item;
			this.SetOptionSelected(item, true);
		}

		public void Option_OnSelectSkin(int idx)
		{
			string item = this.allSkins[idx];
			this.SetOptionSkinSelected(this.optionLastSkinSelected, false);
			this.skinSelectorSelectionElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.optionLastSkinSelected).element.GetComponentInChildren<Text>().enabled = false;
			this.optionLastSkinSelected = item;
			this.SetOptionSkinSelected(item, true);
			this.skinSelectorSelectionElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == item).element.GetComponentInChildren<Text>().enabled = true;
		}

		public void Option_MenuCredits()
		{
			if (this.currentState == ExtrasMenuWidget.STATE.STATE_EXTRAS)
			{
				this.ResetMenus();
				this.currentState = ExtrasMenuWidget.STATE.STATE_OFF;
				Core.Logic.SetState(LogicStates.Playing);
				Core.Input.SetBlocker("INGAME_MENU", false);
				EventSystem.current.SetSelectedGameObject(null);
				Core.Audio.Ambient.StopCurrent();
				Core.Logic.LoadCreditsScene(true);
			}
		}

		public void Option_Achievement()
		{
			if (this.currentState == ExtrasMenuWidget.STATE.STATE_EXTRAS)
			{
				this.ShowMenu(ExtrasMenuWidget.MENU.ACHIEVEMENT);
			}
		}

		public void Option_MenuSkinSelector()
		{
			if (this.currentState == ExtrasMenuWidget.STATE.STATE_EXTRAS)
			{
				this.ShowMenu(ExtrasMenuWidget.MENU.SKINSELECTOR);
			}
		}

		public void Option_PatchNotes()
		{
			if (this.currentState == ExtrasMenuWidget.STATE.STATE_EXTRAS)
			{
				this.GoBack();
				UIController.instance.ShowPatchNotes();
			}
		}

		public void Option_ChooseBackground()
		{
			if (this.currentState == ExtrasMenuWidget.STATE.STATE_EXTRAS)
			{
				this.GoBack();
				base.StartCoroutine(UIController.instance.ShowMainMenuToChooseBackground());
			}
		}

		public void Option_MainMenu()
		{
			if (this.currentState == ExtrasMenuWidget.STATE.STATE_EXTRAS)
			{
				this.GoBack();
			}
		}

		private void ShowMenu(ExtrasMenuWidget.MENU menu)
		{
			this.currentMenu = menu;
			foreach (KeyValuePair<ExtrasMenuWidget.MENU, Transform> keyValuePair in this.extrasRoot)
			{
				if (keyValuePair.Value != null)
				{
					CanvasGroup component = keyValuePair.Value.gameObject.GetComponent<CanvasGroup>();
					component.alpha = ((keyValuePair.Key != this.currentMenu) ? 0f : 1f);
					component.interactable = (keyValuePair.Key == this.currentMenu);
				}
			}
			Transform transform = this.extrasRoot[this.currentMenu];
			if (transform == null)
			{
				return;
			}
			transform = transform.Find("Selection");
			if (transform == null && this.currentMenu != ExtrasMenuWidget.MENU.ACHIEVEMENT)
			{
				return;
			}
			this.lastHorizontalInOptions = 0f;
			ExtrasMenuWidget.MENU menu2 = this.currentMenu;
			if (menu2 != ExtrasMenuWidget.MENU.SKINSELECTOR)
			{
				if (menu2 != ExtrasMenuWidget.MENU.EXTRAS)
				{
					if (menu2 == ExtrasMenuWidget.MENU.ACHIEVEMENT)
					{
						this.CreateAchievements();
					}
				}
				else
				{
					for (int k = 0; k < transform.childCount; k++)
					{
						this.SetOptionSelected(transform.GetChild(k).gameObject, k == 0);
					}
					if (Core.ColorPaletteManager.GetAllUnlockedColorPalettesId().Count > 1)
					{
						this.skinSelectorOption.transform.parent.gameObject.SetActive(true);
						this.LinkButtonVertical(this.skinSelectorOptionPrevious, this.skinSelectorOption);
						this.LinkButtonVertical(this.skinSelectorOption, this.skinSelectorOptionNext);
					}
					else
					{
						this.skinSelectorOption.transform.parent.gameObject.SetActive(false);
						this.LinkButtonVertical(this.skinSelectorOptionPrevious, this.skinSelectorOptionNext);
					}
					this.optionLastSelected = transform.GetChild(0).gameObject;
				}
			}
			else
			{
				foreach (ExtrasMenuWidget.SkinSelectorElement skinSelectorElement in this.skinSelectorDataElements)
				{
					this.SetOptionSkinSelected(skinSelectorElement.skinKey, false);
				}
				this.currentSkin = Core.ColorPaletteManager.GetCurrentColorPaletteId();
				List<string> allUnlockedColorPalettesId = Core.ColorPaletteManager.GetAllUnlockedColorPalettesId();
				int i;
				for (i = 0; i < this.allSkins.Count; i++)
				{
					if (Core.ColorPaletteManager.GetColorPaletteById(this.allSkins[i]) == null)
					{
						Debug.LogError("Color palette " + this.allSkins[i] + " has no Sprite attached to it in the asset: Color Palettes/AVAILABLE_COLOR_PALETTES");
					}
					else if (allUnlockedColorPalettesId.Contains(this.allSkins[i]))
					{
						this.skinSelectorDataElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[i]).element.SetActive(true);
						this.skinSelectorSelectionElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[i]).element.SetActive(true);
						Material material = new Material(this.colorPaletteShader);
						material.SetTexture("_PaletteTex", Core.ColorPaletteManager.GetColorPaletteById(this.allSkins[i]).texture);
						material.EnableKeyword("PALETTE_ON");
						this.skinSelectorDataElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[i]).element.GetComponentInChildren<Image>().material = material;
					}
					else
					{
						this.skinSelectorDataElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[i]).element.SetActive(false);
						this.skinSelectorSelectionElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[i]).element.SetActive(false);
					}
				}
				int num = this.allSkins.IndexOf(this.currentSkin);
				this.optionLastSelected = transform.GetChild(num).gameObject;
				for (int j = 0; j < transform.childCount; j++)
				{
					this.SetOptionSelected(transform.GetChild(j).gameObject, j == num);
				}
				this.optionLastSkinSelected = this.currentSkin;
				this.LinkAllButtons();
				this.ShowSkinSelectorValues();
			}
			EventSystem.current.SetSelectedGameObject(this.optionLastSelected.GetComponentInChildren<Text>(true).gameObject);
		}

		private void SetOptionSelected(GameObject option, bool selected)
		{
			option.GetComponentInChildren<Text>(true).color = ((!selected) ? this.extrasNormalColor : this.extrasHighligterColor);
			option.GetComponentInChildren<Image>(true).gameObject.SetActive(selected);
		}

		private void SetOptionSkinSelected(string option, bool selected)
		{
			this.skinSelectorDataElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == option).element.GetComponentInChildren<Text>(true).color = ((!selected) ? this.extrasNormalColor : this.extrasHighligterColor);
		}

		public void Option_AcceptSkinSelectorOptions()
		{
			Core.ColorPaletteManager.SetCurrentSkinToSkinSettings(this.currentSkin);
			this.GoBack();
		}

		private void UpdateInputSkinSelectorOptions()
		{
			this.currentSkin = this.optionLastSkinSelected;
			this.ShowSkinSelectorValues();
		}

		private void ShowSkinSelectorValues()
		{
			int i;
			for (i = 0; i < this.allSkins.Count; i++)
			{
				Image componentInChildren = this.skinSelectorDataElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[i]).element.GetComponentInChildren<Image>();
				componentInChildren.enabled = (this.currentSkin == this.allSkins[i]);
				Text componentInChildren2 = this.skinSelectorSelectionElements.Find((ExtrasMenuWidget.SkinSelectorElement x) => x.skinKey == this.allSkins[i]).element.GetComponentInChildren<Text>();
				componentInChildren2.enabled = (this.currentSkin == this.allSkins[i]);
			}
		}

		private void ResetMenus()
		{
			foreach (KeyValuePair<ExtrasMenuWidget.MENU, Transform> keyValuePair in this.extrasRoot)
			{
				if (keyValuePair.Value != null)
				{
					keyValuePair.Value.gameObject.SetActive(true);
					CanvasGroup component = keyValuePair.Value.gameObject.GetComponent<CanvasGroup>();
					component.alpha = 0f;
					component.interactable = false;
				}
			}
		}

		private void LinkButtonHorizontal(EventsButton first, EventsButton second)
		{
			Navigation navigation = first.navigation;
			Navigation navigation2 = second.navigation;
			navigation.selectOnRight = second;
			navigation2.selectOnLeft = first;
			first.navigation = navigation;
			second.navigation = navigation2;
		}

		private void LinkButtonVertical(EventsButton first, EventsButton second)
		{
			Navigation navigation = first.navigation;
			Navigation navigation2 = second.navigation;
			navigation.selectOnDown = second;
			navigation2.selectOnUp = first;
			first.navigation = navigation;
			second.navigation = navigation2;
		}

		private void CreateAchievements()
		{
			foreach (Achievement achievement in Core.AchievementsManager.GetAllAchievements())
			{
				if (!this.AchivementsCache.ContainsKey(achievement.Id))
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.AchievementElement, Vector3.zero, Quaternion.identity);
					gameObject.transform.SetParent(this.AchievementScrollView.scrollRect.content);
					RectTransform rectTransform = (RectTransform)gameObject.transform;
					rectTransform.localRotation = Quaternion.identity;
					rectTransform.localScale = Vector3.one;
					rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0f);
					gameObject.SetActive(true);
					this.AchivementsCache[achievement.Id] = gameObject;
				}
				this.AchivementsCache[achievement.Id].GetComponent<AchievementElementWidget>().SetData(achievement);
			}
			this.AchievementElement.SetActive(false);
			this.AchievementScrollView.NewContentSetted();
		}

		[SerializeField]
		[BoxGroup("Roots", true, false, 0)]
		private Transform ExtrasRoot;

		[SerializeField]
		[BoxGroup("Roots", true, false, 0)]
		private Transform SelectorRoot;

		[SerializeField]
		[BoxGroup("Roots", true, false, 0)]
		private Transform AchievementRoot;

		private Dictionary<ExtrasMenuWidget.MENU, Transform> extrasRoot = new Dictionary<ExtrasMenuWidget.MENU, Transform>();

		[SerializeField]
		[BoxGroup("Extras", true, false, 0)]
		private Color extrasNormalColor = new Color(0.972549f, 0.89411765f, 0.78039217f);

		[SerializeField]
		[BoxGroup("Extras", true, false, 0)]
		private Color extrasHighligterColor = new Color(0.80784315f, 0.84705883f, 0.49803922f);

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundBack = "event:/SFX/UI/ChangeTab";

		[SerializeField]
		[BoxGroup("Options Extras", true, false, 0)]
		private EventsButton skinSelectorOption;

		[SerializeField]
		[BoxGroup("Options Extras", true, false, 0)]
		private EventsButton skinSelectorOptionPrevious;

		[SerializeField]
		[BoxGroup("Options Extras", true, false, 0)]
		private EventsButton skinSelectorOptionNext;

		[SerializeField]
		[BoxGroup("Skin Selector Data Elements", true, false, 0)]
		private List<ExtrasMenuWidget.SkinSelectorElement> skinSelectorDataElements;

		[SerializeField]
		[BoxGroup("Skin Selector Selection Elements", true, false, 0)]
		private List<ExtrasMenuWidget.SkinSelectorElement> skinSelectorSelectionElements;

		[SerializeField]
		[BoxGroup("Achievements", true, false, 0)]
		private CustomScrollView AchievementScrollView;

		[SerializeField]
		[BoxGroup("Achievements", true, false, 0)]
		private GameObject AchievementElement;

		private Animator animator;

		private GameObject optionLastSelected;

		private const string SHADER_NAME = "Maikel/MaikelSpriteMasterShader";

		private ExtrasMenuWidget.STATE _currentState;

		private fsSerializer serializer;

		private ExtrasMenuWidget.MENU currentMenu;

		private List<string> allSkins;

		private const string PALETTE_PATH = "Color Palettes/AVAILABLE_COLOR_PALETTES";

		private Player rewired;

		private float lastHorizontalInOptions;

		private float lastVerticalInOptions;

		private string optionLastSkinSelected = "PENITENT_DEFAULT";

		private string currentSkin;

		private Shader colorPaletteShader;

		private Dictionary<string, GameObject> AchivementsCache = new Dictionary<string, GameObject>();

		[Serializable]
		public struct SkinSelectorElement
		{
			public string skinKey;

			public GameObject element;
		}

		private enum STATE
		{
			STATE_OFF,
			STATE_EXTRAS
		}

		private enum MENU
		{
			EXTRAS,
			SKINSELECTOR,
			ACHIEVEMENT
		}
	}
}
