
namespace Blasphemous.Randomizer.Items;

public interface Item
{
    public string Id { get; }
    public string Name { get; }
    public string Hint { get; }
    public int Type { get; }
    public bool Progrssion { get; }
    public int Count { get; }
}
