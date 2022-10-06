using System;
using System.Diagnostics;
using DG.Tweening;
using FMODUnity;
using Framework.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Others.Buttons
{
	public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IMoveHandler, IEventSystemHandler
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<MenuButton> OnMenuButtonSelected;

		public void Awake()
		{
			this.myButton = base.GetComponent<Button>();
			this.myEventsButton = base.GetComponent<EventsButton>();
			this.aloneButton = false;
			if (this.ChangeAllTexts)
			{
				this.ChangeAllTextInternal(false);
			}
			if (this.useDisplacement)
			{
				this.initPos = this.displacementRect.anchoredPosition;
			}
			if (this.myEventsButton)
			{
				this.myEventsButton.onClick.AddListener(new UnityAction(this.TaskOnClick));
				this.aloneButton = this.IsAloneButton(this.myEventsButton.navigation);
				if (this.ChangeText && this.buttonText == null)
				{
					this.buttonText = this.myEventsButton.GetComponentInChildren<Text>(true);
				}
			}
			else
			{
				this.myButton.onClick.AddListener(new UnityAction(this.TaskOnClick));
				this.aloneButton = this.IsAloneButton(this.myButton.navigation);
				if (this.ChangeText && this.buttonText == null)
				{
					this.buttonText = this.myButton.GetComponentInChildren<Text>(true);
				}
			}
			this.InheritedAwake();
		}

		protected virtual void InheritedAwake()
		{
		}

		protected virtual void InheritedStart()
		{
		}

		protected virtual void OnSelectInherited(BaseEventData eventData)
		{
		}

		protected virtual void OnDeselectedInherited(BaseEventData eventData)
		{
		}

		protected void Start()
		{
			this.InheritedStart();
		}

		protected void TaskOnClick()
		{
			if (this.OnClickAudio != string.Empty)
			{
				Core.Audio.PlayOneShot(this.OnClickAudio, default(Vector3));
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (this.buttonText)
			{
				this.buttonText.color = this.textColorHighlighted;
			}
			if (this.ChangeAllTexts)
			{
				this.ChangeAllTextInternal(true);
			}
			if (this.selectedChild)
			{
				this.selectedChild.SetActive(true);
			}
			if (this.selectedChild2)
			{
				this.selectedChild2.SetActive(true);
			}
			this.OnSelectInherited(eventData);
			if (this.useDisplacement)
			{
				TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions46.DOAnchorPos(this.displacementRect, this.initPos + this.displacement, 0.5f, false), 7);
			}
			if (this.OnMenuButtonSelected != null)
			{
				this.OnMenuButtonSelected(this);
			}
			if (this.OnSelectAction != null)
			{
				this.OnSelectAction.Invoke();
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			if (this.buttonText)
			{
				this.buttonText.color = this.textColorDefault;
			}
			if (this.ChangeAllTexts)
			{
				this.ChangeAllTextInternal(false);
			}
			if (this.selectedChild)
			{
				this.selectedChild.SetActive(false);
			}
			if (this.selectedChild2)
			{
				this.selectedChild2.SetActive(false);
			}
			this.OnDeselectedInherited(eventData);
			if (this.useDisplacement)
			{
				TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions46.DOAnchorPos(this.displacementRect, this.initPos, 0.2f, false), 7);
			}
		}

		private void ChangeAllTextInternal(bool highlighted)
		{
			bool flag = true;
			if (this.myButton)
			{
				flag = this.myButton.interactable;
			}
			else if (this.myEventsButton)
			{
				flag = this.myEventsButton.interactable;
			}
			foreach (Text text in base.GetComponentsInChildren<Text>(true))
			{
				Color color = (!highlighted) ? this.textColorDefault : this.textColorHighlighted;
				if (!flag)
				{
					color = this.textColorDisabled;
				}
				ButtonColor component = text.GetComponent<ButtonColor>();
				if (component)
				{
					color = component.GetColor(highlighted, flag);
				}
				text.color = color;
			}
		}

		public void OnMove(AxisEventData eventData)
		{
			if (string.IsNullOrEmpty(this.OnMoveAudio) || this.whenToSoundOnMove == MenuButton.BUTTON_MOVEMENT_SOUND.NONE)
			{
				return;
			}
			bool flag;
			if (this.whenToSoundOnMove == MenuButton.BUTTON_MOVEMENT_SOUND.CLASSIC)
			{
				flag = (this.OnMoveAudio != string.Empty && !this.aloneButton);
			}
			else
			{
				bool flag2 = eventData.moveDir == null || eventData.moveDir == 2;
				bool flag3 = eventData.moveDir == 1 || eventData.moveDir == 3;
				flag = (this.whenToSoundOnMove == MenuButton.BUTTON_MOVEMENT_SOUND.BOTH || (this.whenToSoundOnMove == MenuButton.BUTTON_MOVEMENT_SOUND.HORIZONTAL && flag2) || (this.whenToSoundOnMove == MenuButton.BUTTON_MOVEMENT_SOUND.VERTICAL && flag3));
			}
			if (flag)
			{
				Core.Audio.PlayOneShot(this.OnMoveAudio, default(Vector3));
			}
		}

		private bool IsAloneButton(Navigation nav)
		{
			bool result = false;
			if (nav.mode == 4)
			{
				result = (nav.selectOnDown == null && nav.selectOnLeft == null && nav.selectOnRight == null && nav.selectOnUp == null);
			}
			return result;
		}

		public Text buttonText;

		protected Button myButton;

		protected EventsButton myEventsButton;

		public Color textColorDefault;

		public Color textColorHighlighted;

		public Color textColorDisabled;

		public bool ChangeText = true;

		public bool ChangeAllTexts;

		public GameObject selectedChild;

		public GameObject selectedChild2;

		public MenuButton.BUTTON_MOVEMENT_SOUND whenToSoundOnMove;

		[EventRef]
		public string OnMoveAudio;

		[EventRef]
		public string OnClickAudio;

		public UnityEvent OnSelectAction;

		private bool aloneButton;

		public bool useDisplacement;

		public RectTransform displacementRect;

		public Vector2 displacement;

		private Vector2 initPos;

		public enum BUTTON_MOVEMENT_SOUND
		{
			CLASSIC,
			NONE,
			HORIZONTAL,
			VERTICAL,
			BOTH
		}
	}
}
