using System;
using System.Collections.Generic;
using WebSocketSharp.Server;
using WebSocketSharp;
using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer.Tracker
{
    public class AutoTracker
    {
        private WebSocketServer server;
        private UATService service;

        private bool TrackerActive => server != null && server.IsListening && service != null;

        public void Start()
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

        public void NewItem(Item item)
        {
            string trackerItem = item.id;
            if (item.type == 5) trackerItem = "CO";
            else if (item.type == 6) trackerItem = "CH";
            else if (item.type == 11) trackerItem = item.name;
            // Dont send certain items

            Main.Randomizer.LogWarning("New item: " + trackerItem);
            if (TrackerActive)
            {
                service.VariableChanged(trackerItem, 1);
            }
        }
    }

    public class UATService : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            if (ConnectionState == WebSocketState.Open)
            {
                Main.Randomizer.LogWarning("Poptracker has connected");
                string jsonString = Main.Randomizer.FileUtil.jsonString(new Info(0, "Blasphemous", Main.MOD_VERSION));
                Send("[" + jsonString + "]");
            }

            base.OnOpen();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            Main.Randomizer.LogWarning(e.Data);

            // If message is sync, send all variables
        }

        public void VariableChanged(string name, byte value)
        {
            if (ConnectionState == WebSocketState.Open)
            {
                Main.Randomizer.LogWarning("Sending new variable");
                string jsonString = Main.Randomizer.FileUtil.jsonString(new Var(name, value));
                Send("[" + jsonString + "]");
            }
        }

        class Info
        {
            public string cmd;
            public int protocol;
            public string name;
            public string version;

            public Info(int protocol, string name, string version)
            {
                cmd = "Info";
                this.protocol = protocol;
                this.name = name;
                this.version = version;
            }
        }

        class Var
        {
            public string cmd;
            public string name;
            public byte value;

            public Var(string name, byte value)
            {
                cmd = "Var";
                this.name = name;
                this.value = value;
            }
        }
    }
}
