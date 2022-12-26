using BepInEx;
using HarmonyLib;

namespace BlasphemousRandomizer
{
    [BepInPlugin("com.damocles.blasphemous.randomizer", "Blasphemous Randomizer", MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer;
        private static Main instance;

        private void Awake()
        {
            Randomizer = new Randomizer();
            instance = this;
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

        private void Log(string message)
        {
            Logger.LogMessage(message);
        }

        public static void UnityLog(string message)
        {
            instance.Log(message);
        }
    }
}