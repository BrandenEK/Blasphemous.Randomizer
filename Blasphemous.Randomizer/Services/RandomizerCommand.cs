using Blasphemous.CheatConsole;
using Framework.Managers;
using System;
using System.Collections.Generic;

namespace Blasphemous.Randomizer.Services
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
                { "respawn", Respawn },
            };
        }

        private void Help(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            Write("Available RANDOMIZER commands:");
            Write("randomizer respawn: Respawn from your chosen starting location");
        }

        private void Respawn(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            string currentLevel = Core.LevelManager.currentLevel.LevelName;
            if (currentLevel != "MainMenu" && currentLevel != Main.Randomizer.GameSettings.RealStartingLocation.Room)
            {
                Write("Respawning from starting location!");
                Core.Events.SetFlag("CHERUB_RESPAWN", true);
                Core.SpawnManager.ResetPersistence();
                Core.SpawnManager.SpawnFromCustom(true, null);
            }
            else
            {
                Write("Can only respawn while in game and in a different room!");
            }
        }
    }
}
