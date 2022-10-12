using System;
using Framework.Achievements;
using UnityEngine;

namespace Framework.Randomizer
{
	public class RewardAchievement : Achievement
	{
		public RewardAchievement(string name, string type, Sprite image) : base(name)
		{
			this.Name = name;
			this.Description = type;
			this.Image = image;
		}
	}
}
