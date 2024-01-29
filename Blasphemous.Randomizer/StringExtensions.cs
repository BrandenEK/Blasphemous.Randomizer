
namespace Blasphemous.Randomizer;

public static class StringExtensions
{
    // Capitalize first letter of each word
    public static string Capitalize(this string str)
    {
        string output = string.Empty;
        foreach (string word in str.ToLower().Split(' '))
            output += char.ToUpper(word[0]) + word.Substring(1) + ' ';
        return output.Trim();
    }
}
