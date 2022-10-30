using System;
using UnityEngine;

namespace Framework.Randomizer
{
	public class RewardInfo
	{
		public RewardInfo(string name, string desc, string notification, Sprite sprite)
		{
			this.name = name;
			this.description = desc;
			this.sprite = sprite;
			this.notification = notification;
		}

		public string name;

		public string description;

		public Sprite sprite;

		public string notification;
	}
}
