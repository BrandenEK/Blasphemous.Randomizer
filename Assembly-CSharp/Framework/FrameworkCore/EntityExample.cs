using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gameplay.GameControllers.Entities;

namespace Framework.FrameworkCore
{
	public class EntityExample
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EntityExample.EntityFlagEvent OnFlagChanged;

		public EntityStats Attributes { get; set; }

		public EntityStatus Status { get; set; }

		public void SetFlag(string flag, bool active)
		{
			if (active && !this.HasFlag(flag))
			{
				this.flags.Add(flag);
				if (this.OnFlagChanged != null)
				{
					this.OnFlagChanged(flag, true);
				}
			}
			else if (!active && this.HasFlag(flag))
			{
				this.flags.Remove(flag);
				if (this.OnFlagChanged != null)
				{
					this.OnFlagChanged(flag, false);
				}
			}
		}

		public bool HasFlag(string key)
		{
			return this.flags.Contains(key);
		}

		private List<string> flags;

		public delegate void EntityFlagEvent(string key, bool active);
	}
}
