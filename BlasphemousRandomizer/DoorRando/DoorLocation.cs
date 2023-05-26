using BlasphemousRandomizer.ItemRando;

namespace BlasphemousRandomizer.DoorRando
{
    [System.Serializable]
    public class DoorLocation
    {
        public string Id { get; set; }

        public int Direction { get; set; }
        public string OriginalDoor { get; set; }

        public int Type { get; set; }
        public int VisibilityFlags { get; set; }
        public string[] RequiredDoors { get; set; }

        public string Logic { get; set; }

        public string Room => Id.Substring(0, Id.IndexOf('['));

        public string IdentityName
        {
            get
            {
                int start = Id.IndexOf('[') + 1;
                int end = Id.IndexOf(']');
                return Id.Substring(start, end - start);
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
                if (Direction == 4) return 7;
                if (Direction == 5) return 6;
                if (Direction == 6) return 5;
                if (Direction == 7) return 4;
                return -1;
            }
        }

        public bool ShouldBeVanillaDoor(Config config)
        {
            if (Type == 9 || config.DoorShuffleType <= 0)
                return true;

            if (Type != 1 && config.DoorShuffleType == 1)
                return true;

            return false;
        }

        public bool ShouldBeMadeVisible(Config config, BlasphemousInventory inventory)
        {
            if (Direction == 5) return false;
            if (VisibilityFlags == 0) return true;

            bool visible = false;
            if (!visible && (VisibilityFlags & 1) > 0)
            {
                // If first bit, make door visible if the door itself is accessible
                visible = inventory.HasDoor(Id);
            }
            if (!visible && (VisibilityFlags & 2) > 0)
            {
                // If second bit, make door visible if any required door is accessible
                foreach (string door in RequiredDoors)
                {
                    if (inventory.HasDoor(door))
                    {
                        visible = true;
                        break;
                    }
                }
            }
            if (!visible && (VisibilityFlags & 4) > 0)
            {
                // If third bit, make door visible if hard logic
                visible = config.LogicDifficulty >= 2;
            }
            if (!visible && (VisibilityFlags & 8) > 0)
            {
                // If fourth bit, make door visible if double jump is possible to obtain
                visible = config.ShufflePurifiedHand;
            }
            if (!visible && (VisibilityFlags & 16) > 0)
            {
                // If fifth bit, make door visible if double jump OR no enemy shuffle AND hard logic (Only MoM skip)
                visible = (config.ShufflePurifiedHand || config.EnemyShuffleType < 1) && config.LogicDifficulty >= 2;
            }

            return visible;
        }
    }
}
