using System;
using System.Collections.Generic;

namespace Gameplay.GameControllers.Environment.Breakable
{
	public class BreakableManager
	{
		public void AddBreakable(int breakableObjectId)
		{
			if (!this.breakablesId.Contains(breakableObjectId))
			{
				this.breakablesId.Add(breakableObjectId);
			}
		}

		public void RemoveBreakable(int breakableObjectId)
		{
			if (this.breakablesId.Contains(breakableObjectId))
			{
				this.breakablesId.Remove(breakableObjectId);
			}
		}

		public bool ContainsBreakable(int breakableId)
		{
			return this.breakablesId.Contains(breakableId);
		}

		public void Reset()
		{
			this.breakablesId.Clear();
		}

		private List<int> breakablesId = new List<int>();
	}
}
