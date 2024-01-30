using Framework.Achievements;
using UnityEngine;

namespace Blasphemous.Randomizer.Notifications;

public class RewardAchievement : Achievement
{
    public RewardAchievement(string name, string type, Sprite image) : base(name)
    {
        Name = name;
        Description = type;
        Image = image;
    }
}
