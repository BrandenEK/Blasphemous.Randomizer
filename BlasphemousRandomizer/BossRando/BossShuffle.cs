using Framework.Managers;
using Gameplay.UI.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlasphemousRandomizer.BossRando
{
    public class BossShuffle : IShuffle
    {
        private Dictionary<string, string> mappedBosses;

        private Boss _currentBossFight;
        private string _exitScene, _exitDoor;

        public bool InBossFight => _currentBossFight != null;

        public Boss CurrentBossFight
        {
            get
            {
                if (!InBossFight)
                    throw new System.Exception("Check if in boss fight before getting fight data!");

                if (Main.Randomizer.GameSettings.BossShuffleType <= 0 || mappedBosses == null || !mappedBosses.ContainsKey(_currentBossFight.id))
                    return _currentBossFight;
                else
                    return Main.Randomizer.data.bosses[mappedBosses[_currentBossFight.id]];
            }
        }

        // Manage mapped bosses
        public Dictionary<string, string> SaveMappedBosses() => mappedBosses;
        public void LoadMappedBosses(Dictionary<string, string> mappedBosses) => this.mappedBosses = mappedBosses;
        public void ClearMappedBosses() => mappedBosses = null;


        public void EnterBossFight(string bossId, string exitScene, string exitDoor)
        {
            Boss boss = Main.Randomizer.data.bosses[bossId];
            if (!Core.Events.GetFlag(boss.defeatFlag))
            {
                _currentBossFight = boss;
                _exitScene = exitScene;
                _exitDoor = exitDoor;
            }
        }

        public void LeaveBossFight()
        {
            Main.Randomizer.LogWarning("Returning to real world");
            Core.Events.SetFlag(_currentBossFight.defeatFlag, true);
            Main.Randomizer.itemShuffler.giveItem(_currentBossFight.id, true);

            Main.Instance.StartCoroutine(LoadRealRoomAfterFade());
        }

        private IEnumerator LoadRealRoomAfterFade()
        {
            Main.Randomizer.playSoundEffect(3);
            yield return new WaitForSecondsRealtime(0.2f);
            yield return FadeWidget.instance.FadeCoroutine(new Color(0, 0, 0, 0), Color.white, 2, true, null);

            string targetScene = _exitScene, targetDoor = _exitDoor;
            _currentBossFight = null;
            _exitScene = null;
            _exitDoor = null; // Also null these if dead
            Core.SpawnManager.SpawnFromDoor(targetScene, targetDoor, true);
        }

        public void Init()
        {
            // Init filler
        }

        public void Shuffle(int seed)
        {
            mappedBosses = new()
            {
                { "BS01", "BS02" },
                { "BS02", "BS01" },
                { "BS03", "BS01" },
                { "BS04", "BS03" },
            };
        }
    }
}
