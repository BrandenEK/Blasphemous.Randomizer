namespace BlasphemousRandomizer.Structures
{
    [System.Serializable]
    public class DoorLocation
    {
        // Permanent data
        public string id;
        public int direction;
        public int type;
        public string newDoors;

        // Temporary data
        public string targetDoor;
        public bool available;
    }
}
