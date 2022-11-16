using UnityEngine;
using Gameplay.UI;
using BlasphemousRandomizer.Shufflers;

namespace BlasphemousRandomizer
{
    public class Randomizer
    {
        public ItemShuffle itemShuffler;
        public EnemyShuffle enemyShuffler;
        public DoorShuffle doorShuffler;
        public HintShuffle hintShuffler;

        // Initialize main class
        public Randomizer()
        {
            itemShuffler = new ItemShuffle();
            enemyShuffler = new EnemyShuffle();
            doorShuffler = new DoorShuffle();
            hintShuffler = new HintShuffle();
        }

        // Keyboard input
        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                UIController.instance.ShowPopUp("Test", "", 0, false);
            }
        }
    }
}
