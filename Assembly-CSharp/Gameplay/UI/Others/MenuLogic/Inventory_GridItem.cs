using System;
using Framework.Inventory;
using Gameplay.UI.Others.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class Inventory_GridItem : MonoBehaviour
	{
		public EventsButton Button { get; private set; }

		public BaseInventoryObject inventoryObject { get; private set; }

		public void SetObject(BaseInventoryObject invObj)
		{
			this.inventoryObject = invObj;
			this.Button = this.frameObj.GetComponent<EventsButton>();
			this.frameImage = this.frameObj.GetComponent<RawImage>();
			Sprite picture = this.emptyItem;
			if (invObj)
			{
				picture = invObj.picture;
			}
			this.image.sprite = picture;
		}

		public void UpdateStatus(bool selected, bool equiped)
		{
			Texture texture = this.emptyFrame;
			if (selected)
			{
				texture = this.selectedFrameEmpty;
				if (this.inventoryObject)
				{
					texture = this.selectedFrame;
				}
			}
			else if (this.inventoryObject)
			{
				texture = ((!equiped) ? this.objectFrame : this.equipedFrame);
			}
			this.frameImage.texture = texture;
		}

		[SerializeField]
		private GameObject frameObj;

		[SerializeField]
		private Image image;

		[SerializeField]
		private Texture emptyFrame;

		[SerializeField]
		private Sprite emptyItem;

		[SerializeField]
		private Texture objectFrame;

		[SerializeField]
		private Texture equipedFrame;

		[SerializeField]
		private Texture selectedFrame;

		[SerializeField]
		private Texture selectedFrameEmpty;

		private RawImage frameImage;
	}
}
