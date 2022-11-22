using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BlasphemousRandomizer
{
    [BepInPlugin("com.damocles.blasphemous.randomizer", "Blasphemous Randomizer", MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer;

        private void Awake()
        {
            Randomizer = new Randomizer();
            Patch();
        }

        private void Update()
        {
            Randomizer.update();
        }

        private void Patch()
        {
            Harmony harmony = new Harmony("com.damocles.blasphemous.randomizer");
            harmony.PatchAll();
        }
    }
}