using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gameplay.UI.Others.MenuLogic;

namespace BlasphemousRandomizer
{
    class Randomizer
    {
        public void closeDialog()
        {
            if (PopUpWidget.OnDialogClose != null)
            {
                PopUpWidget.OnDialogClose();
            }
        }
    }
}
