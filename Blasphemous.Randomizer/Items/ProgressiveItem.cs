
namespace Blasphemous.Randomizer.Items;

public class ProgressiveItem(string id, string name, string hint, int type, bool progression, int count, string[] items, bool removePrevious)
    : Item(id, name, hint, type, progression, count)
{
    public string[] Items => items;
    public bool RemovePrevious => removePrevious;
}
