using System;
using System.Collections.Generic;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.UI.Others
{
	public class KeepFocus : MonoBehaviour
	{
		private void Awake()
		{
			this.isActive = true;
			this.firstActivation = false;
			if (this.firstSelected)
			{
				if (!this.allowedObjects.Contains(this.firstSelected))
				{
					this.allowedObjects.Add(this.firstSelected);
				}
				if (EventSystem.current)
				{
					this.SelectFirstSelected();
				}
			}
		}

		private void SelectFirstSelected()
		{
			if (this.checkGroup && !this.checkGroup.interactable)
			{
				return;
			}
			if (this.checkGroup && this.checkGroup.alpha < 0.9f)
			{
				return;
			}
			if (this.checkGroup && !this.checkGroup.gameObject.activeInHierarchy)
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(this.firstSelected);
			this.firstActivation = true;
		}

		private void Update()
		{
			bool flag = this.checkGroup && (!this.checkGroup.interactable || this.checkGroup.alpha < 0.9f || !this.checkGroup.gameObject.activeInHierarchy || (this.parentBlockingWidget != null && !this.parentBlockingWidget.IsActive()));
			if (flag)
			{
				if (EventSystem.current && EventSystem.current.currentSelectedGameObject != null && this.allowedObjects.Contains(EventSystem.current.currentSelectedGameObject))
				{
					EventSystem.current.SetSelectedGameObject(null);
				}
				return;
			}
			if (!this.firstActivation && this.firstSelected && EventSystem.current)
			{
				this.SelectFirstSelected();
			}
			bool flag2 = false;
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject)
			{
				if (this.allowedObjects.Contains(currentSelectedGameObject) && currentSelectedGameObject.activeInHierarchy)
				{
					this.lastFocusObject = currentSelectedGameObject;
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag2 = true;
			}
			if (flag2)
			{
				if (this.lastFocusObject && this.lastFocusObject.activeInHierarchy)
				{
					EventSystem.current.SetSelectedGameObject(this.lastFocusObject);
				}
				else if (this.firstSelected && this.firstSelected.activeInHierarchy)
				{
					EventSystem.current.SetSelectedGameObject(this.firstSelected);
				}
				else
				{
					foreach (GameObject gameObject in this.allowedObjects)
					{
						if (gameObject && gameObject.activeInHierarchy)
						{
							EventSystem.current.SetSelectedGameObject(gameObject);
							break;
						}
					}
				}
			}
		}

		[SerializeField]
		private CanvasGroup checkGroup;

		[SerializeField]
		private List<GameObject> allowedObjects = new List<GameObject>();

		[SerializeField]
		private GameObject firstSelected;

		[SerializeField]
		private BasicUIBlockingWidget parentBlockingWidget;

		private GameObject lastFocusObject;

		private bool isActive;

		private bool firstActivation;

		private const float ALPHA_EPSYLON = 0.9f;
	}
}
