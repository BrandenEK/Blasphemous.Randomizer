namespace BlasphemousRandomizer.Structures
{
    public class ItemLocation
    {
        // Permanent data
        public string id;
        public string originalItem;
        public string type;
        public string requirements;

        // Temporary data
        public Item item;

        //Until json
        public delegate bool reachable(InventoryData data);
        public reachable isReachable;
        public ItemLocation(string id, string originalItem, string type, reachable isReachable)
        {
            this.id = id;
            this.originalItem = originalItem;
            this.type = type;
            this.isReachable = isReachable;
        }
    }
}
