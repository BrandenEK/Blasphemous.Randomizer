using System;
using System.Collections.Generic;
using ModdingAPI.Commands;

namespace BlasphemousRandomizer
{
    public class RandomizerCommand : ModCommand
    {
        protected override string CommandName => "randomizer";

        protected override bool AllowUppercase => false;

        protected override Dictionary<string, Action<string[]>> AddSubCommands()
        {
            return new Dictionary<string, Action<string[]>>()
            {
                { "help", Help },
                { "autotracker", AutoTracker }
            };
        }

        private void Help(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            Write("Available RANDOMIZER commands:");
            Write("randomizer autotracker ON/OFF: Turn autotracking on or off");
        }

        private void AutoTracker(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 1)) return;

            if (parameters[0] == "on")
            {
                bool result = Main.Randomizer.tracker.Connect();
                Write(result ? "Autotracker has been enabled" : "Autotracker failed to start");
            }
            else if (parameters[0] == "off")
            {
                Main.Randomizer.tracker.Disconnect();
                Write("Autotracker has been disabled");
            }
            else
            {
                Write("Please type 'on' or 'off'");
            }
        }
    }
}
