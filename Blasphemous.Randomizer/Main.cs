using BepInEx;

namespace Blasphemous.Randomizer
{
    [BepInPlugin(ModInfo.MOD_ID, ModInfo.MOD_NAME, ModInfo.MOD_VERSION)]
    [BepInDependency("Blasphemous.ModdingAPI", "2.0.1")]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer { get; private set; }

        private void Start()
        {
            Randomizer = new Randomizer();
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