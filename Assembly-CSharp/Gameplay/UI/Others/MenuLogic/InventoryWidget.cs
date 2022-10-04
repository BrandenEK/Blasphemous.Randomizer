using System;
using Gameplay.UI.Others.UIGameLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class InventoryWidget : MonoBehaviour
	{
		private const int MAX_MEA_CULPA_LEVELS = 2;

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private GameObject gridElementBase;

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private GameObject[] tabElements = new GameObject[Enum.GetValues(typeof(InventoryWidget.GridType)).Length];

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private Text gridLabel;

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundElementChange = "event:/SFX/UI/ChangeSelection";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundEquip = "event:/SFX/UI/EquipItem";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundUnequip = "event:/SFX/UI/UnequipItem";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundEquipBead = "event:/SFX/UI/EquipBead";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundUnequipBead = "event:/SFX/UI/UnEquipBead";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundEquipPrayer = "event:/SFX/UI/EquipPrayer";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundUnequipPrayer = "event:/SFX/UI/UnequipItem";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundChangeTab = "event:/SFX/UI/ChangeTab";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundSpendFervour = "event:/SFX/UI/SpendFervour";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundLore = "event:/SFX/UI/ChangeTab";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundBack = "event:/SFX/UI/ChangeTab";

		[SerializeField]
		[BoxGroup("Description", true, false, 0)]
		private GameObject descriptionBase;

		[SerializeField]
		[BoxGroup("Description", true, false, 0)]
		private Text descriptionLabel;

		[SerializeField]
		[BoxGroup("Description", true, false, 0)]
		private Text descriptionText;

		[SerializeField]
		[BoxGroup("Description", true, false, 0)]
		private RawImage descriptionImage;

		[SerializeField]
		[BoxGroup("Equipables", true, false, 0)]
		private Transform beadElements;

		[SerializeField]
		[BoxGroup("Equipables", true, false, 0)]
		private Transform beadsLinkOn;

		[SerializeField]
		[BoxGroup("Equipables", true, false, 0)]
		private Transform relicElements;

		[SerializeField]
		[BoxGroup("Buttons", true, false, 0)]
		private GameObject buttonEquip;

		[SerializeField]
		[BoxGroup("Buttons", true, false, 0)]
		private GameObject buttonUnEquip;

		[SerializeField]
		[BoxGroup("Buttons", true, false, 0)]
		private GameObject buttonLore;

		[SerializeField]
		[BoxGroup("Lore", true, false, 0)]
		private Text loreLabel;

		[SerializeField]
		[BoxGroup("Lore", true, false, 0)]
		private Text loreText;

		[SerializeField]
		[BoxGroup("Lore", true, false, 0)]
		private RawImage loreImage;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private GameObject prayerItemTemplate;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private Text prayerDescriptionLabel;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private Text prayerDescriptionText;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private PlayerDecipher playerDecipher;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private PlayerFervour playerFervour;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private Color prayersTextNormal;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private Color prayersTextLocked;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private GameObject prayerLore;

		[SerializeField]
		[BoxGroup("Prayers", true, false, 0)]
		private GameObject PrayerUnlockButton;

		private Color prayerUnlockImgColor;

		private Color prayerUnlockTextColor;

		[SerializeField]
		[BoxGroup("Mea Culpa", true, false, 0)]
		private Text meaculpaCaption;

		[SerializeField]
		[BoxGroup("Mea Culpa", true, false, 0)]
		private GameObject[] meaculpaImages = new GameObject[2];

		private enum GridType
		{
			Beads,
			Relics,
			QuestItems,
			CollectibleItems
		}
	}
}
