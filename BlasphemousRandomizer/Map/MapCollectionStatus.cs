using System.Collections.Generic;
using Gameplay.UI.Others.MenuLogic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Managers;
using Framework.Map;
using BlasphemousRandomizer.ItemRando;

namespace BlasphemousRandomizer.Map
{
    public class MapCollectionStatus
    {
        public Dictionary<string, ZoneCollection> CollectionStatus { get; set; }

        // Displays the total items collected and zone items collected on the map screen
        public void UpdateMap(NewMapMenuWidget widget, CellData currentCell)
        {
            mapWidget = widget;
            if (CollectionStatus == null)
            {
                Main.Randomizer.LogError("Collection status dictionary was null - Not updating map");
                return;
            }

            // Get and display total items
            int currentAll = 0, totalAll = 0;
            foreach (ZoneCollection zone in CollectionStatus.Values)
            {
                currentAll += zone.CurrentItems;
                totalAll += zone.TotalItems;
            }
            TotalItemsText.text = $"{Main.Randomizer.Localize("items")}: {currentAll}/{totalAll}";
            
            // Get and display zone items
            string currentZoneName;
            ZoneCollection currentZoneCollection;
            if (currentCell == null)
            {
                currentZoneCollection = CollectionStatus["Initia"];
                currentZoneName = Main.Randomizer.Localize("varous");
            }
            else
            {
                currentZoneCollection = CollectionStatus[currentCell.ZoneId.District + currentCell.ZoneId.Zone];
                currentZoneName = currentZoneCollection.ZoneInitials;
            }
            ZoneItemsText.text = $"{currentZoneName}: {currentZoneCollection.CurrentItems}/{currentZoneCollection.TotalItems}";
        }

        // When a new item location is collected, increase the counter in the specified zone
        public void CollectLocation(string locationId, Config config)
        {
            ItemLocation location = Main.Randomizer.data.itemLocations[locationId];
            if (!ShouldTrackLocation(location, config))
                return;

            string zoneId = GetZoneId(location);
            CollectionStatus[zoneId].CurrentItems++;
        }

        // When a new game is started, create a new collection status based on which items are shuffled
        public void ResetCollectionStatus(Config config)
        {
            CollectionStatus = new Dictionary<string, ZoneCollection>();
            foreach (KeyValuePair<string, string> zoneEntry in Main.Randomizer.data.LocationNames)
            {
                // Create new zone collection and set its initials
                ZoneCollection currentZone = new ZoneCollection();
                string initials = string.Empty;
                foreach (string word in zoneEntry.Value.Split(' '))
                    initials += word[0];
                currentZone.ZoneInitials = initials.Length == 1 ? zoneEntry.Value : initials;
                CollectionStatus.Add(zoneEntry.Key, currentZone);
            }

            foreach (ItemLocation location in Main.Randomizer.data.itemLocations.Values)
            {
                // Increment total items for this zone for each item location that should be tracked
                if (ShouldTrackLocation(location, config))
                    CollectionStatus[GetZoneId(location)].TotalItems++;
            }
        }

        private string GetZoneId(ItemLocation location)
        {
            if (location.LocationFlag == null)
                return location.Room.Substring(0, 6);
            else
                return location.LocationFlag.Split('~')[1];
        }

        private bool ShouldTrackLocation(ItemLocation location, Config config)
        {
            if (!config.ShuffleSwordSkills && location.Type == 1)
                return false;
            if (!config.ShuffleThorns && location.Type == 2)
                return false;
            if (!config.ShuffleBootsOfPleading && location.Id == "RE401")
                return false;
            if (!config.ShufflePurifiedHand && location.Id == "RE402")
                return false;
            return true;
        }

        private NewMapMenuWidget mapWidget;

        private Text m_TotalItemsText;
        private Text TotalItemsText
        {
            get
            {
                if (m_TotalItemsText == null)
                {
                    RectTransform rect = Object.Instantiate(mapWidget.CherubsText.gameObject, mapWidget.transform).transform as RectTransform;
                    rect.name = "TotalItemsText";
                    rect.anchoredPosition = new Vector2(45f, -60f);
                    m_TotalItemsText = rect.GetComponentInChildren<Text>();
                    m_TotalItemsText.alignment = TextAnchor.MiddleLeft;
                }
                return m_TotalItemsText;
            }
        }

        private Text m_ZoneItemsText;
        private Text ZoneItemsText
        {
            get
            {
                if (m_ZoneItemsText == null)
                {
                    RectTransform rect = Object.Instantiate(mapWidget.CherubsText.gameObject, mapWidget.transform).transform as RectTransform;
                    rect.name = "ZoneItemsText";
                    rect.anchoredPosition = new Vector2(45f, -80f);
                    m_ZoneItemsText = rect.GetComponentInChildren<Text>();
                    m_ZoneItemsText.alignment = TextAnchor.MiddleLeft;
                }
                return m_ZoneItemsText;
            }
        }
    }
}
