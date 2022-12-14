using UnityEngine;
using UnityEngine.UI;

namespace BlasphemousRandomizer.UI
{
    public class SettingsElement : MonoBehaviour
    {
        public virtual void onClick()
        {
            Main.Randomizer.Log(name + " has been clicked!");
            Main.Randomizer.playSoundEffect(2);
        }
    }

    // Checkbox can be toggled on or off
    public class SettingsCheckbox : SettingsElement
    {
        private Image image;
        private bool selected;
        private bool disabled;

        public void onStart()
        {
            image = GetComponent<Image>();
            selected = false;
            enable(true);
        }

        public void enable(bool value)
        {
            disabled = !value;
            image.sprite = disabled ? Main.Randomizer.getUI(2) : selected ? Main.Randomizer.getUI(1) : Main.Randomizer.getUI(0);
        }

        public void setSelected(bool value)
        {
            selected = value;
            image.sprite = selected ? Main.Randomizer.getUI(1) : Main.Randomizer.getUI(0);
        }
        public bool getSelected()
        {
            return selected;
        }

        public override void onClick()
        {
            base.onClick();
            if (!disabled)
            {
                setSelected(!selected);
            }
        }
    }

    // Cyclebox can cycle the box through a series of options
    public class SettingsCyclebox : SettingsElement
    {
        private Text text;
        private SettingsElement[] checkboxes;

        private string[] options;
        private int optionIdx;

        public void onStart(string[] options, SettingsElement[] checkboxes)
        {
            text = GetComponentInChildren<Text>();
            GetComponent<Image>().sprite = Main.Randomizer.getUI(3);
            this.options = options;
            this.checkboxes = checkboxes;
            setOption(0);
        }

        public void setOption(int option)
        {
            optionIdx = option;
            text.text = options[option];
            enableButtons(option != 0);
        }
        public int getOption()
        {
            return optionIdx;
        }

        private void enableButtons(bool value)
        {
            foreach (SettingsElement box in checkboxes)
            {
                ((SettingsCheckbox)box).enable(value);
            }
        }

        public override void onClick()
        {
            base.onClick();
            optionIdx++;
            if (optionIdx >= options.Length)
                optionIdx = 0;
            setOption(optionIdx);
        }
    }

    // Button box can be clicked to perform an action
    public class SettingsButtonbox : SettingsElement
    {
        private int id;

        public void onStart(int id)
        {
            this.id = id;
            GetComponent<Image>().sprite = Main.Randomizer.getUI(3);
        }

        public override void onClick()
        {
            base.onClick();
            if (id == 0)
            {
                // Begin game
                Main.Randomizer.getSettingsMenu().beginGame();
            }
            else if (id == 1)
            {
                // Go back to select slots screen
                Main.Randomizer.getSettingsMenu().closeMenu();
            }
        }
    }
}
