using System;
using System.Collections.ObjectModel;
using WebSocketSharp.Server;
using Framework.Managers;
using Framework.Inventory;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Tracker
{
    public class AutoTracker
    {
        private WebSocketServer server;
        private UATService service;

        public bool TrackerActive => server != null && server.IsListening && service != null;

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
            foreach (RosaryBead bead in Core.InventoryManager.GetRosaryBeadOwned())
                NewItem(bead.id, 0);
            foreach (Prayer prayer in Core.InventoryManager.GetPrayersOwned())
                NewItem(prayer.id, 1);
            foreach (Relic relic in Core.InventoryManager.GetRelicsOwned())
                NewItem(relic.id, 2);
            foreach (Sword sword in Core.InventoryManager.GetSwordsOwned())
                NewItem(sword.id, 3);
            NewItem("CO", 4);
            foreach (QuestItem questitem in Core.InventoryManager.GetQuestItemOwned())
                NewItem(questitem.id, 5);
            NewItem("CH", 6);
            string[] skills = new string[] { "CHARGED_1", "VERTICAL_1", "RANGED_1" };
            foreach (string skill in skills)
            {
                if (Core.SkillManager.IsSkillUnlocked(skill))
                    NewItem(skill, 11);
            }

            // Send all locations
            foreach (string locationId in Main.Randomizer.data.itemLocations.Keys)
            {
                if (Core.Events.GetFlag("LOCATION_" + locationId))
                    NewLocation(locationId);
            }
        }

        public void NewItem(string item, byte type)
        {
            if (!TrackerActive) return;

            byte value = 1;
            switch (type)
            {
                case 4:
                    value = (byte)Core.InventoryManager.GetCollectibleItemOwned().Count;
                    break;
                case 6:
                    value = (byte)CherubCaptorPersistentObject.CountRescuedCherubs();
                    break;
            }

            Main.Randomizer.LogWarning($"New item: {item} ({value})");
            service.VariableChanged(item, value);
        }

        public void NewLocation(string locationId)
        {
            if (!TrackerActive) return;

            Main.Randomizer.LogWarning("New location: " + locationId);
            service.VariableChanged("@" + locationId, 1);
        }
    }
}
