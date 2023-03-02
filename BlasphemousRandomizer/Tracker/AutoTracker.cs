using System;
using WebSocketSharp.Server;
using Framework.Managers;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Tracker
{
    public class AutoTracker
    {
        private WebSocketServer server;
        private UATService service;

        public bool TrackerActive => server != null && server.IsListening && service != null;
        public string[] SpecialLocations { get; private set; }
        private bool waitingToSendItems = false;

        public bool Connect()
        {
            if (TrackerActive) return true;

            try
            {
                server = new WebSocketServer("ws://localhost:65399");
                server.AddWebSocketService("/", (UATService uat) => { service = uat; });

                server.Start();
                Main.Randomizer.Log("Autotracker server started");
                return true;
            }
            catch (Exception e)
            {
                Main.Randomizer.LogError("Autotracker server failed - " + e.Message);
                return false;
            }
        }

        public void Disconnect()
        {
            if (TrackerActive)
            {
                server.Stop();
            }
            server = null;
            service = null;
        }

        public void TrackerConnected()
        {
            waitingToSendItems = true;
            SendAllVariables(Core.LevelManager.currentLevel.LevelName);
        }

        public void LevelLoaded(string scene)
        {
            SendAllVariables(scene);
        }

        private void SendAllVariables(string scene)
        {
            if (!waitingToSendItems || scene == "MainMenu") return;

            Main.Randomizer.LogWarning("Sending all items!");
            waitingToSendItems = false;

            // Send all items
            foreach (string itemId in Main.Randomizer.data.items.Keys)
            {
                if (Main.Randomizer.data.items[itemId] is ProgressiveItem progressiveItem)
                {
                    foreach (string subItem in progressiveItem.items)
                    {
                        if (Core.Events.GetFlag("ITEM_" + subItem))
                            NewItem(subItem);
                    }
                }
                else
                {
                    if (Core.Events.GetFlag("ITEM_" + itemId))
                        NewItem(itemId);
                }
            }

            // Send all locations
            foreach (string locationId in Main.Randomizer.data.itemLocations.Keys)
            {
                if (Core.Events.GetFlag("LOCATION_" + locationId))
                    NewLocation(locationId);
            }

            // Send special locations
            foreach (string locationFlag in SpecialLocations)
            {
                if (Core.Events.GetFlag(locationFlag))
                    NewItem(locationFlag);
            }
        }

        public void NewItem(string item)
        {
            if (!TrackerActive) return;

            service.VariableChanged(item, 1);
        }

        public void NewLocation(string locationId)
        {
            if (!TrackerActive) return;

            service.VariableChanged("@" + locationId, 1);
        }

        public AutoTracker()
        {
            SpecialLocations = new string[]
            {
                // Boss killed flags
                "D17Z01_BOSSDEAD",
                "D01Z06S01_BOSSDEAD",
                "D02Z05S01_BOSSDEAD",
                "D03Z04S01_BOSSDEAD",
                "D08Z01S01_BOSSDEAD",
                "D04Z04S01_BOSSDEAD",
                "D05Z01S13_BOSSDEAD",
                "D09Z01S03_BOSSDEAD",
                "D06Z01S25_BOSSDEAD",
                "D07Z01S02_BOSSDEAD",
                "D20Z02S08_BOSSDEAD",
                "D01BZ08S01_BOSSDEAD",
                "SANTOS_LAUDES_DEFEATED",
                // Confessor dungeons
                "CONFESSOR_1_ARENACOMPLETED",
                "CONFESSOR_2_ARENACOMPLETED",
                "CONFESSOR_3_ARENACOMPLETED",
                "CONFESSOR_4_ARENACOMPLETED",
                "CONFESSOR_5_ARENACOMPLETED",
                "CONFESSOR_6_ARENACOMPLETED",
                "CONFESSOR_7_ARENACOMPLETED",
                // Jondo bells
                "BELL_PUZZLE1_ACTIVATED",
                "BELL_PUZZLE2_ACTIVATED",
                "BELL_ACTIVATED",
                // Candles
                "CANDLE_RED_1_USED",
                "CANDLE_RED_2_USED",
                "CANDLE_BLUE_1_USED",
                "CANDLE_BLUE_2_USED",
                // Jibrael quest
                "SANTOS_FIRSTCONVERSATION_DONE", // ??
                "SANTOS_AMANECIDA_LOCATION1_ACTIVATED",
                "SANTOS_AMANECIDA_LOCATION2_ACTIVATED",
                "SANTOS_AMANECIDA_LOCATION3_ACTIVATED",
                "SANTOS_AMANECIDA_LOCATION4_ACTIVATED",
                // Redento quest
                "REDENTO_0203_DONE", // ??
                "REDENTO_0204_DONE",
                "REDENTO_0205_DONE",
                "REDENTO_0206_DONE",
                "REDENTO_0207_DONE",
                "REDENTO_0209_DONE",
                "REDENTO_0210_DONE",
                "REDENTO_0211_DONE",
                "REDENTO_0214_DONE",
                // Miriam quest
                "MIRIAM_FIRSTENCOUNTER_DONE", // ??
                "MIRIAM_CHALLENGE_1",
                "MIRIAM_CHALLENGE_2",
                "MIRIAM_CHALLENGE_3",
                "MIRIAM_CHALLENGE_4",
                "MIRIAM_CHALLENGE_5",
            };
        }
    }
}
