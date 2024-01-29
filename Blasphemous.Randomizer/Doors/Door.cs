using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blasphemous.Randomizer.Doors;

public class Door
{
    public string Id { get; set; }

    public int Direction { get; set; }
    public string OriginalDoor { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public Visibility VisibilityFlags { get; set; }
    public int Type { get; set; }

    public string[] RequiredDoors { get; set; }

    public string Logic { get; set; }

    [System.Flags]
    public enum Visibility
    {
        None = 0x00,
        ThisDoor = 0x01,
        RequiredDoors = 0x02,
        DoubleJump = 0x04,
        NormalLogic = 0x08,
        NormalLogicAndDoubleJump = 0x10,
        HardLogic = 0x20,
        HardLogicAndDoubleJump = 0x40,
        EnemySkips = 0x80,
        EnemySkipsAndDoubleJump = 0x100,
    }
}
