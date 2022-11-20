using UnityEngine;

namespace BlasphemousRandomizer.Structures
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
