
namespace Blasphemous.Randomizer.EnemyRando
{
    [System.Serializable]
    public class EnemyData
    {
        public string id;
        public string name;
        public int type;
        public int difficulty;
        public float yOffset;

        public EnemyData(string id, string name, int type, int difficulty, float yOffset)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.difficulty = difficulty;
            this.yOffset = yOffset;
        }
    }
}
