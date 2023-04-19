
namespace BlasphemousRandomizer.DoorRando
{
    [System.Serializable]
    public class DoorLocation
    {
        public string Id { get; set; }

        public int Direction { get; set; }
        public string OriginalDoor { get; set; }

        public bool SpecialFlag { get; set; }
        public string Logic { get; set; }

        public string Room
        {
            get
            {
                return Id.Substring(0, Id.IndexOf('['));
            }
        }

        public int OppositeDirection
        {
            get
            {
                if (Direction == 0) return 3;
                if (Direction == 3) return 0;
                if (Direction == 1) return 2;
                if (Direction == 2) return 1;
                if (Direction == 4) return 4;
                if (Direction == 5) return 6;
                if (Direction == 6) return 5;
                return -1;
            }
        }
    }
}
