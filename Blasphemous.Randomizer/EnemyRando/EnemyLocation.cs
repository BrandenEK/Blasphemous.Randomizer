
namespace Blasphemous.Randomizer.EnemyRando
{
    [System.Serializable]
    public class EnemyLocation
    {
        public string locationId;
        public string originalEnemy;
        public bool arena;
        public float yOffset;

        public EnemyLocation(string locationId, string originalEnemy, bool arena, float yOffset)
        {
            this.locationId = locationId;
            this.originalEnemy = originalEnemy;
            this.arena = arena;
            this.yOffset = yOffset;
        }
    }
}
