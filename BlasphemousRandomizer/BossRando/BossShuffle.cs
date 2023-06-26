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

        //public BossRoom CurrentBossFight { get; set; }
        private BossRoom _currentBossFight;

        public bool InBossFight => _currentBossFight != null;

        public string EnterBossRoom
        {
            get
            {
                if (/*Main.Randomizer.GameSettings.BossShuffleType <= 0 ||*/ mappedBosses == null || !mappedBosses.ContainsKey(_currentBossFight.RealRoom))
                    return _currentBossFight.FakeRoom;
                else
                    return mappedBosses[_currentBossFight.RealRoom];
            }
        }
        public string LeaveBossRoom => _currentBossFight.RealRoom;

        // Manage mapped bosses
        public Dictionary<string, string> SaveMappedBosses() => mappedBosses;
        public void LoadMappedBosses(Dictionary<string, string> mappedBosses) => this.mappedBosses = mappedBosses;
        public void ClearMappedBosses() => mappedBosses = null;

        //public string GetRandomBossRoom(string bossRoom)
        //{
        //    // Make sure boss rando is on and this is a boss room
        //    if (/*Main.Randomizer.GameSettings.BossShuffleType <= 0 ||*/ mappedBosses == null || !mappedBosses.ContainsKey(bossRoom))
        //        return null;

        //    BossRoom roomData = bossRooms["WS"];

        //    // Make sure this boss hasn't already been defeated
        //    if (Core.Events.GetFlag(roomData.DefeatFlag))
        //        return null;

        //    return mappedBosses[bossRoom];
        //}

        public void ReturnToGameWorld() => Main.Instance.StartCoroutine(ReturnToGameWorldCoroutine());

        private IEnumerator ReturnToGameWorldCoroutine()
        {
            Main.Randomizer.LogWarning("Returning to real world");
            Core.Events.SetFlag(bossRooms["WS"].DefeatFlag, true);
            Main.Randomizer.itemShuffler.giveItem("BS13", true);
            yield return FadeToWhite();

            Core.SpawnManager.SpawnFromDoor("D17Z01S11", "W", true);
        }

        private IEnumerator FadeToWhite()
        {
            Main.Randomizer.playSoundEffect(3);
            yield return new WaitForSecondsRealtime(0.2f);
            yield return FadeWidget.instance.FadeCoroutine(new Color(0, 0, 0, 0), Color.white, 2, true, null);
        }

        public void EnterBossFight(string bossId)
        {
            BossRoom room = bossRooms[bossId];
            if (!Core.Events.GetFlag(room.DefeatFlag))
                _currentBossFight = room;
        }

        public void Init()
        {
            // Init filler
        }

        public void Shuffle(int seed)
        {
            mappedBosses = new()
            {
                { "D17Z01S11", "D22Z01S02" },
                { "D01Z04S18", "D22Z01S01" },
            };
        }

        private readonly Dictionary<string, BossRoom> bossRooms = new Dictionary<string, BossRoom>()
        {
            { "WS",
            new BossRoom()
            {
                RealRoom = "D17Z01S11",
                FakeRoom = "D22Z01S01",
                DefeatFlag = "D17Z01_BOSSDEAD"
            }
            },
            { "TP",
            new BossRoom()
            {
                RealRoom = "D01Z04S18",
                FakeRoom = "D22Z01S02",
                DefeatFlag = "D01Z06S01_BOSSDEAD"
            }
            },
        };
    }

    public class BossRoom
    {
        public string RealRoom { get; set; }
        public string FakeRoom { get; set; }

        public string DefeatFlag { get; set; }
    }
}
