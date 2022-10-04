using System;
using System.Collections.Generic;
using Framework.Inventory;
using Gameplay.UI.Others.Buttons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewInventory_GridItem : MonoBehaviour
	{
		public EventsButton Button { get; private set; }

		public BaseInventoryObject inventoryObject { get; private set; }

		private void Awake()
		{
			this.cachedImages = new List<Image>(base.GetComponentsInChildren<Image>(true));
		}

		public void SetObject(BaseInventoryObject invObj)
		{
			this.inventoryObject = invObj;
			this.Button = this.frameObj.GetComponent<EventsButton>();
			this.frameImage = this.frameObj.GetComponent<Image>();
			this.frameAnimator = this.frameObj.GetComponent<Animator>();
			if (invObj)
			{
				this.objectImage.sprite = invObj.picture;
				this.objectImage.enabled = true;
			}
			else
			{
				this.objectImage.enabled = false;
			}
		}

		public void UpdateStatus(bool p_enabled, bool p_selected, bool p_equiped)
		{
			Sprite sprite = this.backDisabled;
			if (p_enabled)
			{
				sprite = this.backNothing;
				if (this.inventoryObject)
				{
					sprite = ((!p_equiped) ? this.backUnEquipped : this.backEquipped);
				}
			}
			this.frameBack.sprite = sprite;
			this.UpdateSelect(p_selected);
		}

		public void UpdateSelect(bool selected)
		{
			Sprite sprite = (!selected) ? null : this.frameSelected;
			if (this.frameImage)
			{
				this.frameImage.sprite = sprite;
				this.frameImage.enabled = (sprite != null);
			}
			if (this.frameAnimator)
			{
				this.frameAnimator.enabled = (sprite != null);
			}
		}

		public void ActivateGrayscale()
		{
			foreach (Image image in this.cachedImages)
			{
				image.material.EnableKeyword("COLORIZE_ON");
				image.gameObject.SetActive(false);
				image.gameObject.SetActive(true);
			}
		}

		public void DeactivateGrayscale()
		{
			foreach (Image image in this.cachedImages)
			{
				image.material.DisableKeyword("COLORIZE_ON");
				image.gameObject.SetActive(false);
				image.gameObject.SetActive(true);
			}
		}

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Image frameBack;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private GameObject frameObj;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private Image objectImage;

		[SerializeField]
		[BoxGroup("Frame", true, false, 0)]
		private Sprite frameSelected;

		[SerializeField]
		[BoxGroup("Back", true, false, 0)]
		private Sprite backDisabled;

		[SerializeField]
		[BoxGroup("Back", true, false, 0)]
		private Sprite backNothing;

		[SerializeField]
		[BoxGroup("Back", true, false, 0)]
		private Sprite backUnEquipped;

		[SerializeField]
		[BoxGroup("Back", true, false, 0)]
		private Sprite backEquipped;

		private Image frameImage;

		private Animator frameAnimator;

		private List<Image> cachedImages;
	}
}
