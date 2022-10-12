using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gameplay.UI.Others.Buttons
{
	public class EventsButton : Selectable, IPointerClickHandler, ISubmitHandler, IEventSystemHandler
	{
		protected EventsButton()
		{
		}

		public EventsButton.ButtonSelectedEvent onSelected
		{
			get
			{
				return this.m_onSelected;
			}
			set
			{
				this.m_onSelected = value;
			}
		}

		public EventsButton.ButtonClickedEvent onClick
		{
			get
			{
				return this.m_OnClick;
			}
			set
			{
				this.m_OnClick = value;
			}
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			this.m_onSelected.Invoke();
		}

		private void Press()
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			this.m_OnClick.Invoke();
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			this.Press();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			this.Press();
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			this.DoStateTransition(Selectable.SelectionState.Pressed, false);
			base.StartCoroutine(this.OnFinishSubmit());
		}

		private IEnumerator OnFinishSubmit()
		{
			float fadeTime = base.colors.fadeDuration;
			float elapsedTime = 0f;
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			this.DoStateTransition(base.currentSelectionState, false);
			yield break;
		}

		[FormerlySerializedAs("onSelected")]
		[SerializeField]
		private EventsButton.ButtonSelectedEvent m_onSelected = new EventsButton.ButtonSelectedEvent();

		[FormerlySerializedAs("onClick")]
		[SerializeField]
		private EventsButton.ButtonClickedEvent m_OnClick = new EventsButton.ButtonClickedEvent();

		[Serializable]
		public class ButtonSelectedEvent : UnityEvent
		{
		}

		[Serializable]
		public class ButtonClickedEvent : UnityEvent
		{
		}
	}
}
