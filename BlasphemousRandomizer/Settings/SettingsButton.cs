using UnityEngine;
using UnityEngine.UI;

namespace BlasphemousRandomizer.Settings
{
    // Any settings button can be clicked, or enabled/disabled
    public abstract class SettingsElement : MonoBehaviour
    {
        protected bool disabled;

        public void setEnabled(bool value)
        {
            disabled = !value;
            if (value) enable();
            else disable();
        }

        public void onClick()
        {
            Main.Randomizer.Log(name + " has been clicked!");
            Main.Randomizer.playSoundEffect(2);
            if (!disabled)
                click();
        }

        protected abstract void click();
        protected abstract void enable();
        protected abstract void disable();
        public abstract string getDescription();
    }

    // Checkbox can be toggled on or off
    public class SettingsCheckbox : SettingsElement
    {
        private Image image;
        private bool selected;
        private string description;

        public void onStart(string desc)
        {
            image = GetComponent<Image>();
            description = desc;
            setSelected(false);
            enable();
        }

        // When changing enabled status, change sprite
        protected override void enable()
        {
            setSelected(selected);
        }
        protected override void disable()
        {
            image.sprite = Main.Randomizer.data.uiImages[2];
        }

        // When clicked, toggle checkbox
        protected override void click()
        {
            setSelected(!selected);
        }

        // Get and set selected variable
        public void setSelected(bool value)
        {
            selected = value;
            image.sprite = selected ? Main.Randomizer.data.uiImages[1] : Main.Randomizer.data.uiImages[0];
        }
        public bool getSelected()
        {
            return selected;
        }

        // Return desc
        public override string getDescription()
        {
            return description;
        }
    }

    // Cyclebox can cycle the box through a series of options
    public class SettingsCyclebox : SettingsElement
    {
        private Text text;
        private Image image;
        
        private SettingsElement[] checkboxes;
        private string[] options;
        private string[] descriptions;
        private int optionIdx;

        private bool right;

        public void onStart(string[] options, string[] descs, SettingsElement[] checkboxes, bool right)
        {
            this.right = right;
            this.options = options;
            this.descriptions = descs;
            this.checkboxes = checkboxes;

            text = GetComponentInParent<Text>();
            image = GetComponent<Image>();

            setOption(0);
        }

        // When changing enabled status, change sprite
        protected override void enable()
        {
            image.sprite = Main.Randomizer.data.uiImages[right ? 5 : 4];
        }
        protected override void disable()
        {
            image.sprite = Main.Randomizer.data.uiImages[right ? 7 : 6];
        }

        // When clicked, change option on both cycleboxes & disable checkboxes
        protected override void click()
        {
            setOption(optionIdx + (right ? 1 : -1));
            transform.parent.GetChild(right ? 0 : 1).GetComponent<SettingsCyclebox>().setOption(optionIdx);
        }

        // Get and set option index variable
        public void setOption(int option)
        {
            optionIdx = option;
            text.text = options[option];

            foreach (SettingsElement box in checkboxes)
            {
                ((SettingsCheckbox)box).setEnabled(option != 0);
            }

            if (right)
                setEnabled(optionIdx < options.Length - 1);
            else
                setEnabled(optionIdx > 0);
        }
        public int getOption()
        {
            return optionIdx;
        }

        // Return description for this option
        public override string getDescription()
        {
            return descriptions[optionIdx];
        }
    }
}
