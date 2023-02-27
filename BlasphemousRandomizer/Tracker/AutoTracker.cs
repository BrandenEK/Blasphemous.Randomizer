using System;
using System.Collections.Generic;
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
    }
}
