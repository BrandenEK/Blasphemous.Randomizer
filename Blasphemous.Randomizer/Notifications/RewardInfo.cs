using UnityEngine;

namespace Blasphemous.Randomizer.Notifications;

public class RewardInfo(string name, string description, string notification, Sprite image)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string Notification { get; } = notification;
    public Sprite Image { get; } = image;
}
