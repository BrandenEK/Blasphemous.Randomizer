using UnityEngine;
using Gameplay.UI.Widgets;

namespace BlasphemousRandomizer
{
    class ConsoleOverride
    {
        private bool console = false;

        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                console = !console;
                ConsoleWidget.Instance.SetEnabled(console);
            }
        }
    }
}
