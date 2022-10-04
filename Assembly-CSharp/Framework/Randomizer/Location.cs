using System;

namespace Framework.Randomizer
{
	public class Location
	{
		public Location(string id, string type, Location.reachable isReachable)
		{
			this.id = id;
			this.type = type;
			this.isReachable = isReachable;
		}

		public override string ToString()
		{
			if (this.reward == null)
			{
				return "Location " + this.id + " has no reward.";
			}
			return string.Format("Location {0} has the reward: ({1}) {2}.", this.id, this.reward.type, this.reward.id);
		}

		public string id;

		public Reward reward;

		public Location.reachable isReachable;

		public string type;

		public delegate bool reachable(ItemData d);
	}
}
