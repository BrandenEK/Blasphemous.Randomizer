using UnityEngine;

namespace Blasphemous.Randomizer.Notifications
{
    public class RewardInfo
    {
		public RewardInfo(string name, string description, string notification, Sprite sprite)
		{
			this.name = name;
			this.description = description;
			this.sprite = sprite;
			this.notification = notification;
		}

		public string name;
		public string description;
		public string notification;
		public Sprite sprite;
	}
}
