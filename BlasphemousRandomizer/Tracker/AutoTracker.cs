using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using System.Net;
using System.Net.Sockets;

namespace BlasphemousRandomizer.Tracker
{
    public class AutoTracker
    {
        public void Start()
        {
            //WebSocket socket = new WebSocket("test");
            //socket.Connect();

            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 65399);
            server.Start();
            Main.Randomizer.LogWarning("Server started!");

            if (!server.Pending())
            {
                Main.Randomizer.LogWarning("No client found!");
                server.Stop();
                return;
            }

            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            Main.Randomizer.LogWarning("Client connected!");

            while (true)
            {
                while (!stream.DataAvailable);

                byte[] bytes = new byte[client.Available];
                stream.Read(bytes, 0, bytes.Length);
                Main.Randomizer.LogWarning("Message: " + Encoding.UTF8.GetString(bytes));
            }
            // Close client at end
        }
    }
}
