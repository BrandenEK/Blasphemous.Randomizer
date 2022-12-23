namespace BlasphemousRandomizer.Structures
{
    [System.Serializable]
    public class Enemy
    {
        public string id;
        public string name;
        public int type;
        public int difficulty;
        public float yOffset;

        public Enemy(string id, string name, int type, int difficulty, float yOffset)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.difficulty = difficulty;
            this.yOffset = yOffset;
        }
    }
}
