namespace Blasphemous.Randomizer.Extensions;

internal static class RandomizerExtensions
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

    // Get number of digits in positive integer (This is benchmarked to be the fastest
    public static int GetDigits(this int n)
    {
        if (n < 10) return 1;
        if (n < 100) return 2;
        if (n < 1000) return 3;
        if (n < 10000) return 4;
        if (n < 100000) return 5;
        if (n < 1000000) return 6;
        if (n < 10000000) return 7;
        if (n < 100000000) return 8;
        if (n < 1000000000) return 9;
        return 10;
    }
}
