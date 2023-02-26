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

        private bool waitingToSendItems = false;

        public void Connect()
        {
            try
            {
                server = new WebSocketServer("ws://localhost:65399");
                server.AddWebSocketService("/", (UATService uat) => { service = uat; });

                server.Start();
                Main.Randomizer.LogWarning("Server started!");
            }
            catch (Exception e)
            {
                Main.Randomizer.LogWarning("Server failed: " + e.Message);
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
            SendAllItems(Core.LevelManager.currentLevel.LevelName);
        }

        public void LevelLoaded(string scene)
        {
            SendAllItems(scene);
        }

        private void SendAllItems(string scene)
        {
            if (!waitingToSendItems || scene == "MainMenu") return;

            Main.Randomizer.LogWarning("Sending all items!");
            waitingToSendItems = false;
            // Send all items
        }

        public void NewItem(Item item)
        {
            if (item.type > 6 && item.type != 11) return;

            string trackerItem = item.id;
            if (item.type == 5) trackerItem = "CO";
            else if (item.type == 6) trackerItem = "CH";
            // Dont send certain items

            Main.Randomizer.LogWarning("New item: " + trackerItem);
            if (TrackerActive)
            {
                service.VariableChanged(trackerItem, 1);
            }
        }
    }
}
