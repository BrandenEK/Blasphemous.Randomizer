
namespace BlasphemousRandomizer.ItemRando
{
    [System.Serializable]
    public class ItemLocation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Hint { get; set; }

        public string Type { get; set; }
        public string OriginalItem { get; set; }

        public string Room { get; set; }
        public string Logic { get; set; }
    }
}
