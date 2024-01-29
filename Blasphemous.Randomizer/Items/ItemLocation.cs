
namespace Blasphemous.Randomizer.Items;

public class ItemLocation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Hint { get; set; }

    public int Type { get; set; }
    public string OriginalItem { get; set; }
    public string LocationFlag { get; set; }

    public string Room { get; set; }
    public string Logic { get; set; }

    public bool IsVanilla(RandomizerSettings settings)
    {
        if (Type == 9)
            return true;
        if (Type == 2 && !settings.ShuffleThorns)
            return true;
        if (Type == 1 && !settings.ShuffleSwordSkills)
            return true;
        return false;
    }
}
