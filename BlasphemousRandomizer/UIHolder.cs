using UnityEngine;
using UnityEngine.UI;
using Gameplay.UI.Others.MenuLogic;
using Gameplay.UI.Others.Buttons;

namespace BlasphemousRandomizer
{
    public static class UIHolder
    {
        private static Text modsText;

        private static GameObject settingsMenu;
        private static GameObject slotsMenu;

        private static EventsButton button;

        public static Text getModsText(Transform parent, Font font)
        {
            if (modsText == null)
                modsText = createText("Mods Text", parent, new Vector2(200, 100), new Vector2(-115, -60), font, 18, TextAnchor.UpperRight);
            return modsText;
        }

        private static Text createText(string name, Transform parent, Vector2 size, Vector2 position, Font font, int fontSize, TextAnchor alignment)
        {
            // Create object
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));

            // Set transform values
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            // Set text values
            Text text = obj.GetComponent<Text>();
            text.alignment = alignment;
            text.font = font;
            text.fontSize = fontSize;

            return text;
        }

        public static void setSettingsButton(EventsButton obj)
        {
            button = obj;
        }

        public static void toggleSettingsMenu()
        {
            if (settingsMenu != null)
            {
                settingsMenu.SetActive(!settingsMenu.activeSelf);
                slotsMenu.SetActive(!settingsMenu.activeSelf);
                Main.Randomizer.Log("Toggling settings menu");
            }
        }

        public static void createSettingsMenu()
        {
            if (settingsMenu != null)
                return;

            //Font font = null;
            //VersionNumber version = Object.FindObjectOfType<VersionNumber>();
            //if (version != null)
            //    font = version.GetComponent<Text>().font;

            //settingsMenu = new GameObject("Randomizer Settings Menu", typeof(RectTransform), typeof(Image));
            Transform menu = Object.FindObjectOfType<NewMainMenu>().transform;
            slotsMenu = menu.GetChild(2).gameObject;

            // Duplicate slot menu
            settingsMenu = Object.Instantiate(slotsMenu, menu);
            Object.Destroy(settingsMenu.GetComponent<SelectSaveSlots>());
            Object.Destroy(settingsMenu.GetComponent<CanvasGroup>());
            for (int i = 2; i < settingsMenu.transform.childCount; i++)
                Object.Destroy(settingsMenu.transform.GetChild(i).gameObject);

            // Set rect of settings menu
            RectTransform rect = settingsMenu.GetComponent<RectTransform>(); // Is this necessary ??
            rect.SetParent(menu, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            Text headerText = settingsMenu.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            headerText.text = "CHOOSE SETTINGS";
            Font font = headerText.font;

            // Create main section
            int width = 630, height = 260;
            RectTransform mainSection = getNewPanel("Main Section", settingsMenu.transform, Color.white);
            mainSection.pivot = new Vector2(0.5f, 0);
            mainSection.anchorMin = new Vector2(0.5f, 0);
            mainSection.anchorMax = new Vector2(0.5f, 0);
            mainSection.sizeDelta = new Vector2(width, height);
            mainSection.anchoredPosition = new Vector2(0, 185);
            //Main.Randomizer.Log("Size: " + mainSection.sizeDelta);
            //Main.Randomizer.Log("Offset min: " + mainSection.offsetMin);
            //Main.Randomizer.Log("Offset max: " + mainSection.offsetMax);

            // General section

            RectTransform section1 = getNewPanel("Section1", mainSection, Color.red);
            section1.sizeDelta = new Vector2(width / 4, height);
            section1.anchoredPosition = new Vector2(-0.375f * width, 0);

            RectTransform title1 = getNewText("General Title", section1, "General:", font, 20);
            title1.anchoredPosition = new Vector2(0, height / 2 - 10);

            //RectTransform title1 = getNewText("General Title", section1, "General:", font, 20);
            //title1.anchoredPosition = new Vector2(0, height / 2 - 10);
            //RectTransform title1 = getNewText("General Title", section1, "General:", font, 20);
            //title1.anchoredPosition = new Vector2(0, height / 2 - 10);

            

            // Items section

            RectTransform section2 = getNewPanel("Section2", mainSection, Color.yellow);
            section2.sizeDelta = new Vector2(width / 4, height);
            section2.anchoredPosition = new Vector2(-0.125f * width, 0);

            RectTransform section3 = getNewPanel("Section3", mainSection, Color.green);
            section3.sizeDelta = new Vector2(width / 4, height);
            section3.anchoredPosition = new Vector2(0.125f * width, 0);

            RectTransform section4 = getNewPanel("Section4", mainSection, Color.blue);
            section4.sizeDelta = new Vector2(width / 4, height);
            section4.anchoredPosition = new Vector2(0.375f * width, 0);

            Main.Randomizer.Log("Settings menu has been created");
            settingsMenu.SetActive(false);
            // Set parent as canvas
            // Give background color and stretch across screen
            // Add buttons

            RectTransform getNewPanel(string name, Transform parent, Color color)
            {
                GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.SetParent(parent, false);
                obj.GetComponent<Image>().color = color;
                return rect;
            }

            RectTransform getNewText(string name, Transform parent, string text, Font font, int size)
            {
                GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Text));
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.SetParent(parent, false);
                Text display = obj.GetComponent<Text>();
                display.text = text;
                display.font = font;
                display.fontSize = size;
                display.alignment = TextAnchor.MiddleCenter;
                return rect;
            }
        }

        public static string displayHierarchy(Transform transform, string output, int level, bool components)
        {
            // Indent
            for (int i = 0; i < level; i++)
                output += "\t";

            // Add this object
            output += transform.name;

            // Add components
            if (components)
            {
                output += " (";
                foreach (Component c in transform.GetComponents<Component>())
                    output += c.ToString() + ", ";
                output = output.Substring(0, output.Length - 2) + ")";
            }
            output += "\n";

            // Add children
            for (int i = 0; i < transform.childCount; i++)
                output = displayHierarchy(transform.GetChild(i), output, level + 1, components);

            // Return output
            return output;
        }
    }
}
