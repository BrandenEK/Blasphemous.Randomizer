using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BlasphemousRandomizer
{
    [BepInPlugin("damocles.blasphemous.randomizer", "Blasphemous Randomizer", "1.0.0")]
    [BepInProcess("Blasphemous.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            if (main == null)
                main = this;

            console = new ConsoleOverride();
            Patch();
        }

        private void Patch()
        {
            Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }

        private void Update()
        {
            console.update();
        }

        public void Log(string message)
        {
            Logger.LogInfo(message);
        }

        //Temporary
        public void giveItem(string id)
        {
            main.Log("Giving item: " + id);
        }

        ConsoleOverride console;
        public static Plugin main;
    }
}