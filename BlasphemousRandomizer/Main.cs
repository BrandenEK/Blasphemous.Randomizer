using BepInEx;

namespace BlasphemousRandomizer
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.damocles.blasphemous.modding-api", "1.4.0")]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer;

        private void Start()
        {
            Randomizer = new Randomizer(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION);
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

        // Capitalize first letter of each word
        public static string Capitalize(this string str)
        {
            string output = string.Empty;
            foreach (string word in str.ToLower().Split(' '))
                output += char.ToUpper(word[0]) + word.Substring(1) + ' ';
            return output.Trim();
        }
    }
}