using BepInEx;

namespace BlasphemousRandomizer
{
    [BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
    [BepInDependency("com.damocles.blasphemous.modding-api", "1.3.4")]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public const string MOD_ID = "com.damocles.blasphemous.randomizer";
        public const string MOD_NAME = "Randomizer";
        public const string MOD_VERSION = "2.0.0";

        public static Randomizer Randomizer;

        private void Start()
        {
            Randomizer = new Randomizer(MOD_ID, MOD_NAME, MOD_VERSION);
        }
    }

    public static class Extensions
    {
        // Check if an array contains a certain item
        public static bool Contains<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}