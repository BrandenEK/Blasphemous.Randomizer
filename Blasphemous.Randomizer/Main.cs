﻿using BepInEx;

namespace Blasphemous.Randomizer
{
    [BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
    [BepInDependency("Blasphemous.ModdingAPI", "2.4.1")]
    [BepInDependency("Blasphemous.Framework.Credits", "0.1.1")]
    [BepInDependency("Blasphemous.Framework.Levels", "0.1.4")]
    [BepInDependency("Blasphemous.Framework.Menus", "0.3.4")]
    [BepInDependency("Blasphemous.Framework.UI", "0.1.2")]
    [BepInDependency("Blasphemous.CheatConsole", "1.0.1")]
    internal class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer { get; private set; }

        private void Start()
        {
            Randomizer = new Randomizer();
        }
    }
}