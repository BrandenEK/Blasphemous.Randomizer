
namespace Blasphemous.Randomizer.Enemies;

public class Enemy(string id, string name, int type, int difficulty, float yOffset)
{
    public string Id => id;
    public string Name => name;
    public int Type => type;
    public int Difficulty => difficulty;
    public float YOffset => yOffset;
}
