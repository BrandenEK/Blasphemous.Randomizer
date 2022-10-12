using System;
using System.Diagnostics;
using DG.Tweening;
using Framework.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Others.Buttons
{
	public class ControlRemapButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IMoveHandler, IEventSystemHandler
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ControlRemapButton> OnControlRemapButtonSelected;

		public void Awake()
		{
			this.myButton = base.GetComponent<Button>();
			this.myEventsButton = base.GetComponent<EventsButton>();
			this.buttonText = null;
			this.aloneButton = false;
			if (this.useDisplacement)
			{
				this.initPos = this.displacementRect.anchoredPosition;
			}
			if (this.myEventsButton)
			{
				this.myEventsButton.onClick.AddListener(new UnityAction(this.TaskOnClick));
				this.aloneButton = this.IsAloneButton(this.myEventsButton.navigation);
				this.buttonText = this.myEventsButton.GetComponent<Text>();
			}
			else
			{
				this.myButton.onClick.AddListener(new UnityAction(this.TaskOnClick));
				this.aloneButton = this.IsAloneButton(this.myButton.navigation);
				this.buttonText = this.myButton.GetComponent<Text>();
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
				this.buttonText.color = ((!this.conflict) ? this.textColorHighlighted : this.textColorConflictHighlighted);
			}
			if (this.selectedChild)
			{
				this.selectedChild.SetActive(true);
			}
			this.OnSelectInherited(eventData);
			if (this.useDisplacement)
			{
				this.displacementRect.DOAnchorPos(this.initPos + this.displacement, 0.5f, false).SetEase(Ease.InOutQuad);
			}
			if (this.OnControlRemapButtonSelected != null)
			{
				this.OnControlRemapButtonSelected(this);
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			if (this.buttonText)
			{
				this.buttonText.color = ((!this.conflict) ? this.textColorDefault : this.textColorConflictDefault);
			}
			if (this.selectedChild)
			{
				this.selectedChild.SetActive(false);
			}
			this.OnDeselectedInherited(eventData);
			if (this.useDisplacement)
			{
				this.displacementRect.DOAnchorPos(this.initPos, 0.2f, false).SetEase(Ease.InOutQuad);
			}
		}

		public void OnMove(AxisEventData eventData)
		{
			if (this.OnMoveAudio != string.Empty && !this.aloneButton)
			{
				Core.Audio.PlayOneShot(this.OnMoveAudio, default(Vector3));
			}
		}

		private bool IsAloneButton(Navigation nav)
		{
			bool result = false;
			if (nav.mode == Navigation.Mode.Explicit)
			{
				result = (nav.selectOnDown == null && nav.selectOnLeft == null && nav.selectOnRight == null && nav.selectOnUp == null);
			}
			return result;
		}

		public void SetConflict(bool b)
		{
			if (this.conflict != b)
			{
				this.buttonText.color = ((!b) ? this.textColorDefault : this.textColorConflictDefault);
			}
			this.conflict = b;
		}

		public bool GetConflict()
		{
			return this.conflict;
		}

		protected Text buttonText;

		protected Button myButton;

		protected EventsButton myEventsButton;

		public Color textColorDefault;

		public Color textColorHighlighted;

		public Color textColorConflictDefault;

		public Color textColorConflictHighlighted;

		public GameObject selectedChild;

		public string OnMoveAudio;

		public string OnClickAudio;

		private bool aloneButton;

		public bool useDisplacement;

		public RectTransform displacementRect;

		public Vector2 displacement;

		private Vector2 initPos;

		private bool conflict;
	}
}
