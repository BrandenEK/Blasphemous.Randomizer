using WebSocketSharp;
using WebSocketSharp.Server;

namespace BlasphemousRandomizer.Tracker
{
    public class UATService : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            if (ConnectionState == WebSocketState.Open)
            {
                Main.Randomizer.Log("Poptracker has connected");
                string jsonString = Main.Randomizer.FileUtil.jsonString(new Info(0, "Blasphemous", Main.MOD_VERSION));
                Send("[" + jsonString + "]");
            }

            base.OnOpen();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);
            Main.Randomizer.LogWarning("Received autotracker cmd: " + e.Data);
            if (e.Data.Contains("Sync"))
                Main.Randomizer.tracker.TrackerConnected();
        }

        public void VariableChanged(string name, byte value)
        {
            if (ConnectionState == WebSocketState.Open)
            {
                Main.Randomizer.LogWarning("Sending new variable: " + name);
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
