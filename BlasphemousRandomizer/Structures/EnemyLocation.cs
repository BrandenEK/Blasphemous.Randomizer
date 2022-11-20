namespace BlasphemousRandomizer.Structures
{
    public class EnemyLocation
    {
        public int locationId;
        public int enemyType;
        public bool arena;

        public string enemy;

        public EnemyLocation(int location, string enemy, int type, bool arena)
        {
            this.locationId = location;
            this.enemy = enemy;
            this.enemyType = type;
            this.arena = arena;
        }
    }
}
