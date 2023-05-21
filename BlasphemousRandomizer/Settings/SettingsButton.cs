using UnityEngine;
using UnityEngine.UI;

namespace BlasphemousRandomizer.Settings
{
    // Any settings button can be clicked, or enabled/disabled
    public abstract class SettingsElement : MonoBehaviour
    {
        public abstract bool Enabled { get; set; }

        public void onClick()
        {
            Main.Randomizer.Log(name + " has been clicked!");
            Main.Randomizer.playSoundEffect(2);
            if (Enabled)
                Click();
        }

        protected abstract void Click();

        public abstract string Description { get; }
    }

    // Checkbox can be toggled on or off
    public class SettingsCheckbox : SettingsElement
    {
        private Image image;
        private string description;

        public void onStart(string desc)
        {
            image = GetComponent<Image>();
            description = desc;
            Selected = false;
            Enabled = true;
        }

        private bool m_Enabled;
        public override bool Enabled
        {
            get => m_Enabled;
            set
            {
                m_Enabled = value;
                if (value)
                {
                    Selected = m_Selected;
                }
                else
                {
                    image.sprite = Main.Randomizer.data.uiImages[2];
                }
            }
        }

        private bool m_Selected;
        public bool Selected
        {
            get => m_Selected && m_Enabled;
            set
            {
                m_Selected = value;
                image.sprite = value ? Main.Randomizer.data.uiImages[1] : Main.Randomizer.data.uiImages[0];
            }
        }

        // When clicked, toggle checkbox
        protected override void Click()
        {
            Selected = !m_Selected;
        }

        public override string Description => description;
    }

    // Cyclebox can cycle the box through a series of options
    public class SettingsCyclebox : SettingsElement
    {
        private Text text;
        private Image image;
        
        private string[] options;
        private string[] descriptions;
        private bool right;

        public void onStart(string[] options, string[] descriptions, bool right)
        {
            this.right = right;
            this.options = options;
            this.descriptions = descriptions;

            text = GetComponentInParent<Text>();
            image = GetComponent<Image>();

            CurrentOption = 0;
        }

        private bool m_Enabled;
        public override bool Enabled
        {
            get => m_Enabled;
            set
            {
                m_Enabled = value;
                if (value)
                    image.sprite = Main.Randomizer.data.uiImages[right ? 5 : 4];
                else
                    image.sprite = Main.Randomizer.data.uiImages[right ? 7 : 6];
            }
        }

        private int m_CurrentOption;
        public int CurrentOption
        {
            get => m_CurrentOption;
            set
            {
                m_CurrentOption = value;
                text.text = options[value];

                if (right)
                    Enabled = value < options.Length - 1;
                else
                    Enabled = value > 0;
            }
        }

        // When clicked, change option on both cycleboxes & disable checkboxes
        protected override void Click()
        {
            CurrentOption = m_CurrentOption + (right ? 1 : -1);
            transform.parent.GetChild(right ? 0 : 1).GetComponent<SettingsCyclebox>().CurrentOption = m_CurrentOption;
        }

        public override string Description => descriptions[m_CurrentOption];
    }

    // Textbox can be used to input string fields
    public class SettingsTextbox : SettingsElement
    {
        private Text text;
        private Image line;

        public void OnStart()
        {
            text = GetComponent<Text>();
            line = GetComponentInChildren<Image>();
            Selected = false;
        }

        public override bool Enabled
        {
            get => true;
            set { }
        }

        private bool m_Selected;
        public bool Selected
        {
            get => m_Selected;
            set
            {
                m_Selected = value;
                text.color = value ? Color.yellow : Color.white;
                line.color = value ? Color.yellow : Color.white;
            }
        }

        public string TextContent
        {
            get => text.text;
            set => text.text = value;
        }

        protected override void Click() { }

        public override string Description => string.Empty;
    }
}
