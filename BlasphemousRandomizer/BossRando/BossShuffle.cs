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

        private BossRoom _currentBossFight;
        private string _exitDoor;

        public bool InBossFight => _currentBossFight != null;

        public BossRoom CurrentBossFight
        {
            get
            {
                if (!InBossFight)
                    throw new System.Exception("Check if in boss fight before getting fight data!");

                if (/*Main.Randomizer.GameSettings.BossShuffleType <= 0 ||*/ mappedBosses == null || !mappedBosses.ContainsKey(_currentBossFight.id))
                    return _currentBossFight;
                else
                    return bossRooms[mappedBosses[_currentBossFight.id]];
            }
        }

        // Manage mapped bosses
        public Dictionary<string, string> SaveMappedBosses() => mappedBosses;
        public void LoadMappedBosses(Dictionary<string, string> mappedBosses) => this.mappedBosses = mappedBosses;
        public void ClearMappedBosses() => mappedBosses = null;


        public void EnterBossFight(string bossId, string exitDoor)
        {
            BossRoom room = bossRooms[bossId];
            if (!Core.Events.GetFlag(room.DefeatFlag))
            {
                _currentBossFight = room;
                _exitDoor = exitDoor;
            }
        }

        public void LeaveBossFight()
        {
            Main.Randomizer.LogWarning("Returning to real world");
            Core.Events.SetFlag(_currentBossFight.DefeatFlag, true);
            Main.Randomizer.itemShuffler.giveItem(_currentBossFight.locationId, true);

            Main.Instance.StartCoroutine(LoadRealRoomAfterFade());
        }

        private IEnumerator LoadRealRoomAfterFade()
        {
            Main.Randomizer.playSoundEffect(3);
            yield return new WaitForSecondsRealtime(0.2f);
            yield return FadeWidget.instance.FadeCoroutine(new Color(0, 0, 0, 0), Color.white, 2, true, null);

            string targetScene = _currentBossFight.RealRoom, targetDoor = _exitDoor;
            _currentBossFight = null;
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
                { "WS", "TP" },
                { "TP", "WS" },
                { "CL", "WS" },
            };
        }

        private readonly Dictionary<string, BossRoom> bossRooms = new()
        {
            { "WS",
            new BossRoom("WS")
            {
                RealRoom = "D17Z01S11",
                fakeRoomId = 1,
                locationId = "BS13",
                DefeatFlag = "D17Z01_BOSSDEAD"
            }
            },
            { "TP",
            new BossRoom("TP")
            {
                RealRoom = "D01Z04S18",
                fakeRoomId = 2,
                locationId = "BS01",
                DefeatFlag = "D01Z06S01_BOSSDEAD"
            }
            },
            {
                "CL",
                new BossRoom("CL")
                {
                    RealRoom = "D02Z03S20",
                    fakeRoomId = 3,
                    locationId = "BS03",
                    DefeatFlag = "D02Z05S01_BOSSDEAD"
                }
            }
        };
    }

    public class BossRoom
    {
        public readonly string id;

        // Change to readonly fields
        public string RealRoom { get; set; }
        //public string FakeRoom { get; set; }
        //public string fakeTeleport;

        public int fakeRoomId;

        public string DefeatFlag { get; set; }

        public string locationId; // temp

        public BossRoom(string id) => this.id = id;

        public string FakeRoom => "D22Z01S" + fakeRoomId.ToString("00");

        public string FakeTeleport => "BOSSRUSH" + fakeRoomId.ToString("00");
    }
}
