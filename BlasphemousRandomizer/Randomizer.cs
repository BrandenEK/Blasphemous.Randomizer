using UnityEngine;
using Gameplay.UI;

namespace BlasphemousRandomizer
{
    class Randomizer
    {
        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                UIController.instance.ShowPopUp("Test", "", 0, false);
            }
        }
    }
}
