using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Surrogates;
using Framework.Managers;
using Rewired;
using RewiredConsts;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.UI
{
	[SelectionBase]
	public class InputIcon : MonoBehaviour
	{
		private bool InsideCanvas
		{
			get
			{
				RectTransform component = base.GetComponent<RectTransform>();
				return component != null;
			}
		}

		private void Awake()
		{
			if (this.gpIcon != null)
			{
				this.gpIcon.sprite = null;
			}
			if (this.uiIcon != null)
			{
				this.uiIcon.sprite = null;
			}
		}

		private void Start()
		{
			Core.Input.JoystickPressed += this.ActiveInputChanged;
			Core.Input.KeyboardPressed += this.ActiveInputChanged;
			if (this.gpText != null)
			{
				this.gpText.GetComponent<Renderer>().sortingLayerName = "In-Game UI";
			}
			if (!this.isControlsRemappingInputIcon)
			{
				this.RefreshIcon();
			}
		}

		private void OnDestroy()
		{
			Core.Input.JoystickPressed -= this.ActiveInputChanged;
			Core.Input.KeyboardPressed -= this.ActiveInputChanged;
		}

		public void RefreshIcon()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (this.action == -1)
			{
				this.RefreshBlankIcon();
				return;
			}
			Player player = ReInput.players.GetPlayer(0);
			InputAction inputAction = ReInput.mapping.GetAction(this.action);
			ActionElementMap actionElementMap = null;
			if (inputAction != null)
			{
				AxisRange axisRange = (this.axisCheck != InputIcon.AxisCheck.Positive) ? 2 : 1;
				actionElementMap = Core.ControlRemapManager.FindLastElementMapByInputAction(inputAction, axisRange, Core.Input.ActiveController);
			}
			if (actionElementMap == null)
			{
				return;
			}
			this.SetIconByButtonKey(actionElementMap.elementIdentifierName);
		}

		private void RefreshBlankIcon()
		{
			InputIconLayout inputIconLayout = this.FindIconLayout();
			ButtonDescription buttonDescription = default(ButtonDescription);
			buttonDescription.text = string.Empty;
			buttonDescription.icon = inputIconLayout.defaultIcon;
			Sprite icon = buttonDescription.icon;
			string text = buttonDescription.text;
			if (!this.InsideCanvas)
			{
				if (this.gpText)
				{
					this.gpText.text = text;
				}
				if (this.gpIcon)
				{
					this.gpIcon.sprite = icon;
				}
			}
			else
			{
				if (this.uiIcon)
				{
					this.uiIcon.sprite = icon;
					this.uiIcon.SetNativeSize();
				}
				if (this.uiText)
				{
					this.uiText.text = text;
				}
			}
		}

		public static ButtonDescription GetButtonDescriptionByButtonKey(string buttonName)
		{
			bool flag;
			InputIconLayout inputIconLayout = InputIcon.FindIconLayout(out flag);
			ButtonDescription result = default(ButtonDescription);
			ButtonDescription[] array = Array.FindAll<ButtonDescription>(inputIconLayout.buttons, (ButtonDescription x) => string.Equals(x.button, buttonName, StringComparison.CurrentCultureIgnoreCase));
			if (array != null && array.Length > 0)
			{
				result = array[array.Length - 1];
				result.text = ((!(result.text == "-")) ? result.text : string.Empty);
			}
			else if (flag)
			{
				result.text = buttonName;
				result.icon = inputIconLayout.defaultIcon;
			}
			else
			{
				result.text = ((buttonName.Length <= 1) ? buttonName : (buttonName[buttonName.Length - 1] + string.Empty));
				result.icon = inputIconLayout.defaultIcon;
			}
			return result;
		}

		public void SetIconByButtonKey(string buttonName)
		{
			bool flag;
			InputIconLayout inputIconLayout = InputIcon.FindIconLayout(out flag);
			ButtonDescription buttonDescriptionByButtonKey = InputIcon.GetButtonDescriptionByButtonKey(buttonName);
			Sprite icon = buttonDescriptionByButtonKey.icon;
			string text = buttonDescriptionByButtonKey.text;
			if (!this.InsideCanvas)
			{
				if (this.gpText)
				{
					this.gpText.text = text;
				}
				if (this.gpIcon)
				{
					this.gpIcon.sprite = icon;
				}
			}
			else
			{
				if (this.uiIcon)
				{
					this.uiIcon.sprite = icon;
					this.uiIcon.SetNativeSize();
				}
				if (this.uiText)
				{
					this.uiText.text = text;
					this.uiText.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 2f, this.uiText.transform.localPosition.z);
				}
			}
		}

		public void Fade(float value, float time)
		{
			if (this.InsideCanvas)
			{
				Image[] componentsInChildren = base.GetComponentsInChildren<Image>();
				Text[] componentsInChildren2 = base.GetComponentsInChildren<Text>();
				LinqExtensions.ForEach<Image>(componentsInChildren, delegate(Image render)
				{
					ShortcutExtensions46.DOFade(render, value, time);
				});
				LinqExtensions.ForEach<Text>(componentsInChildren2, delegate(Text text)
				{
					ShortcutExtensions46.DOFade(text, value, time);
				});
			}
			else
			{
				SpriteRenderer[] componentsInChildren3 = base.GetComponentsInChildren<SpriteRenderer>();
				TextMesh[] componentsInChildren4 = base.GetComponentsInChildren<TextMesh>();
				LinqExtensions.ForEach<SpriteRenderer>(componentsInChildren3, delegate(SpriteRenderer render)
				{
					ShortcutExtensions43.DOFade(render, value, time);
				});
				LinqExtensions.ForEach<TextMesh>(componentsInChildren4, delegate(TextMesh text)
				{
					DOTween.To(() => text.color, delegate(ColorWrapper x)
					{
						text.color = x;
					}, new Color(1f, 1f, 1f, value), time);
				});
			}
		}

		private static InputIconLayout FindIconLayout(out bool buttonsWithText)
		{
			buttonsWithText = false;
			JoystickType activeJoystickModel = Core.Input.ActiveJoystickModel;
			ControllerType activeControllerType = Core.Input.ActiveControllerType;
			string text = "Input/GenericIconLayout";
			if (activeControllerType == null)
			{
				buttonsWithText = true;
				text = "Input/KeyboardIconLayout";
			}
			else if (activeControllerType == 2 && activeJoystickModel == JoystickType.PlayStation)
			{
				text = "Input/PSIconLayout";
			}
			else if (activeControllerType == 2 && activeJoystickModel == JoystickType.XBOX)
			{
				text = "Input/XBOXIconLayout";
			}
			else if (activeControllerType == 2 && activeJoystickModel == JoystickType.Generic)
			{
				text = "Input/GenericIconLayout";
			}
			if (!InputIcon.CachedLayouts.ContainsKey(text))
			{
				InputIcon.CachedLayouts[text] = Resources.Load<InputIconLayout>(text);
			}
			return InputIcon.CachedLayouts[text];
		}

		private InputIconLayout FindIconLayout()
		{
			JoystickType activeJoystickModel = Core.Input.ActiveJoystickModel;
			ControllerType activeControllerType = Core.Input.ActiveControllerType;
			string text = "Input/XBOXIconLayout";
			if (activeControllerType == null)
			{
				text = "Input/KeyboardIconLayout";
			}
			else if (activeControllerType == 2 && activeJoystickModel == JoystickType.PlayStation)
			{
				text = "Input/PSIconLayout";
			}
			else if (activeControllerType == 2 && activeJoystickModel == JoystickType.XBOX)
			{
				text = "Input/XBOXIconLayout";
			}
			else if (activeControllerType == 2 && activeJoystickModel == JoystickType.Generic)
			{
				text = "Input/GenericIconLayout";
			}
			if (!InputIcon.CachedLayouts.ContainsKey(text))
			{
				InputIcon.CachedLayouts[text] = Resources.Load<InputIconLayout>(text);
			}
			return InputIcon.CachedLayouts[text];
		}

		private void ActiveInputChanged()
		{
			if (!this.isControlsRemappingInputIcon)
			{
				this.RefreshIcon();
			}
		}

		private const string GENERIC_ICON_LAYOUT_PATH = "Input/GenericIconLayout";

		private const string XBOX_ICON_LAYOUT_PATH = "Input/XBOXIconLayout";

		private const string PLAYSTATION_ICON_LAYOUT_PATH = "Input/PSIconLayout";

		private const string KEYBOARD_ICON_LAYOUT_PATH = "Input/KeyboardIconLayout";

		private static readonly Dictionary<string, InputIconLayout> CachedLayouts = new Dictionary<string, InputIconLayout>();

		[BoxGroup("Design Settings", true, false, 0)]
		[ActionIdProperty(typeof(RewiredConsts.Action))]
		[InfoBox("Define the icon layout in the config files placed at Assets/Design/Resources/Input", 1, null)]
		public int action;

		[BoxGroup("Design Settings", true, false, 0)]
		public InputIcon.AxisCheck axisCheck;

		[SerializeField]
		[FoldoutGroup("Attached References", false, 0)]
		[HideIf("InsideCanvas", true)]
		private SpriteRenderer gpIcon;

		[SerializeField]
		[FoldoutGroup("Attached References", false, 0)]
		[HideIf("InsideCanvas", true)]
		private TextMesh gpText;

		public bool isControlsRemappingInputIcon;

		[SerializeField]
		[FoldoutGroup("Attached References", false, 0)]
		[ShowIf("InsideCanvas", true)]
		private Image uiIcon;

		[SerializeField]
		[FoldoutGroup("Attached References", false, 0)]
		[ShowIf("InsideCanvas", true)]
		private Text uiText;

		public enum AxisCheck
		{
			None,
			Positive,
			Negative
		}
	}
}
