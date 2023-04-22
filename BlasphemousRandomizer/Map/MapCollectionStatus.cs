﻿using UnityEngine;
using UnityEngine.UI;
using Gameplay.UI.Others.MenuLogic;
using Framework.Managers;
using Framework.Map;

namespace BlasphemousRandomizer.Map
{
    public class MapCollectionStatus
    {
        public void UpdateMap(NewMapMenuWidget widget, CellData currentCell)
        {
            mapWidget = widget;

            TotalItemsText.text = $"{Main.Randomizer.Localize("items")}: {Main.Randomizer.itemsCollected}/{Main.Randomizer.totalItems}";
            
            string zoneName = string.Empty;
            if (currentCell == null)
            {
                zoneName = "Various: ";
            }
            else
            {
                string fullName = Core.NewMapManager.GetZoneName(currentCell.ZoneId);
                foreach (string word in fullName.Split(' '))
                    zoneName += word[0];
                if (zoneName.Length == 1)
                    zoneName = fullName;
                zoneName += ": ";

            }
            ZoneItemsText.text = zoneName + "0/0";

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