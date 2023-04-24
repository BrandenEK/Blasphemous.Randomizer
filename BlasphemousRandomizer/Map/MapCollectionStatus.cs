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
            string zoneName, zoneKey;
            if (currentCell == null)
            {
                zoneKey = "Initia";
                zoneName = Main.Randomizer.Localize("varous");
            }
            else
            {
                zoneKey = currentCell.ZoneId.District + currentCell.ZoneId.Zone;
                zoneName = locationNames.ContainsKey(zoneKey) ? locationNames[zoneKey] : "???";
            }
            ZoneCollection currentZone = CollectionStatus[zoneKey];
            ZoneItemsText.text = zoneName + $": {currentZone.CurrentItems}/{currentZone.TotalItems}";
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
            foreach (ItemLocation location in Main.Randomizer.data.itemLocations.Values)
            {
                string zoneId = GetZoneId(location);
                if (!CollectionStatus.ContainsKey(zoneId))
                    CollectionStatus.Add(zoneId, new ZoneCollection());

                if (ShouldTrackLocation(location, config))
                    CollectionStatus[zoneId].TotalItems++;
            }
            CollectionStatus.Add("D08Z02", new ZoneCollection());
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

        private Dictionary<string, string> locationNames = new Dictionary<string, string>()
        {
            { "D01Z01", "THL" },
            { "D01Z02", "Albero" },
            { "D01Z03", "WotBC" },
            { "D01Z04", "MD" },
            { "D01Z05", "DC" },
            { "D01Z06", "Petrous" },
            { "D02Z01", "WOTW" },
            { "D02Z02", "GotP" },
            { "D02Z03", "CoOLotCV" },
            { "D03Z01", "MotED" },
            { "D03Z02", "Jondo" },
            { "D03Z03", "GA" },
            { "D04Z01", "PotSS" },
            { "D04Z02", "MoM" },
            { "D04Z03", "KotTW" },
            { "D04Z04", "AtTotS" },
            { "D05Z01", "LotNW" },
            { "D05Z02", "SC" },
            { "D06Z01", "AR" },
            { "D07Z01", "DoHH" },
            { "D08Z01", "THL" },
            { "D08Z02", "THL" },
            { "D08Z03", "HotD" },
            { "D09Z01", "WotHP" },
            { "D17Z01", "BotSS" },
            { "D20Z01", "EoS" },
            { "D20Z02", "MaH" },
            { "D20Z03", "TRPotS" },
        };
    }
}
