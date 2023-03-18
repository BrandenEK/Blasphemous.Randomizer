using BepInEx;

namespace BlasphemousRandomizer
{
    [BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
    [BepInDependency("com.damocles.blasphemous.modding-api", "1.3.0")]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public const string MOD_ID = "com.damocles.blasphemous.randomizer";
        public const string MOD_NAME = "Randomizer";
        public const string MOD_VERSION = "1.4.3";

        public static Randomizer Randomizer;

        private void Start()
        {
            Randomizer = new Randomizer(MOD_ID, MOD_NAME, MOD_VERSION);
        }

        // Checks if an array contains a certain object
        public static bool arrayContains<T>(T[] arr, T id)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(id))
                {
                    return true;
                }
            }
            return false;
        }
    }
}