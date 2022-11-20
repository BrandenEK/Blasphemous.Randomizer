using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BlasphemousRandomizer
{
    [BepInPlugin("damocles.blasphemous.randomizer", "Blasphemous Randomizer", MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            Randomizer = new Randomizer();
            Patch();
        }

        private void Update()
        {
            Randomizer.update();
        }

        private void Patch()
        {
            Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }
}