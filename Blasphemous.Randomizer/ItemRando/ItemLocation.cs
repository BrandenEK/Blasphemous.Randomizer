
namespace Blasphemous.Randomizer.ItemRando
{
    [System.Serializable]
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

        public bool IsVanilla(Config config)
        {
            if (Type == 9)
                return true;
            if (Type == 2 && !config.ShuffleThorns)
                return true;
            if (Type == 1 && !config.ShuffleSwordSkills)
                return true;
            return false;
        }
    }
}
