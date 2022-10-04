using System;
using Framework.Inventory;
using Gameplay.UI.Others.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class Inventory_PrayerItem : MonoBehaviour
	{
		public EventsButton Button { get; private set; }

		public Prayer Prayer { get; private set; }

		[SerializeField]
		private Image image;

		[SerializeField]
		private Image imageLocked;

		[SerializeField]
		private Text caption;

		[SerializeField]
		private GameObject buttonRoot;

		[SerializeField]
		private GameObject buttonEquip;

		[SerializeField]
		private GameObject buttonUnEquip;

		[SerializeField]
		private GameObject buttonLocked;

		[SerializeField]
		private Texture normalBackground;

		[SerializeField]
		private Texture selectedBackground;

		[SerializeField]
		private Texture normalBackgroundEquiped;

		[SerializeField]
		private Texture selectedBackgroundEquiped;

		[SerializeField]
		private GameObject frameEquipped;

		[SerializeField]
		private Color colourEquipped;

		[SerializeField]
		private Color colourNoEquipped;

		[SerializeField]
		private Color colourLocked;
	}
}
