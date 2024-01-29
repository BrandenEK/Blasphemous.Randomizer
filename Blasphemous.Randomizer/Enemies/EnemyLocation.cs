
namespace Blasphemous.Randomizer.Enemies;

public class EnemyLocation(string id, string originalEnemy, bool arena, float yOffset)
{
    public string Id => id;
    public string OriginalEnemy => originalEnemy;
    public bool Arena => arena;
    public float YOffset => yOffset;
}
