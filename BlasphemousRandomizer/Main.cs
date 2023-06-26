using BepInEx;
using UnityEngine;
using System.Text;

namespace BlasphemousRandomizer
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.damocles.blasphemous.modding-api", "1.4.0")]
    [BepInProcess("Blasphemous.exe")]
    public class Main : BaseUnityPlugin
    {
        public static Randomizer Randomizer;
        public static Main Instance { get; private set; }

        private void Start()
        {
            if (Instance == null)
                Instance = this;
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

        // Recursive method that returns the entire hierarchy of an object
        public static string DisplayHierarchy(this Transform transform, int maxLevel, bool includeComponents)
        {
            return transform.DisplayHierarchy_INTERNAL(new StringBuilder(), 0, maxLevel, includeComponents).ToString();
        }

        public static StringBuilder DisplayHierarchy_INTERNAL(this Transform transform, StringBuilder currentHierarchy, int currentLevel, int maxLevel, bool includeComponents)
        {
            // Indent
            for (int i = 0; i < currentLevel; i++)
                currentHierarchy.Append("\t");

            // Add this object
            currentHierarchy.Append(transform.name);

            // Add components
            if (includeComponents)
            {
                currentHierarchy.Append(" - ");
                foreach (Component c in transform.GetComponents<Component>())
                    currentHierarchy.Append(c.ToString() + ", ");
            }
            currentHierarchy.AppendLine();

            // Add children
            if (currentLevel < maxLevel)
            {
                for (int i = 0; i < transform.childCount; i++)
                    currentHierarchy = transform.GetChild(i).DisplayHierarchy_INTERNAL(currentHierarchy, currentLevel + 1, maxLevel, includeComponents);
            }

            // Return output
            return currentHierarchy;
        }
    }
}