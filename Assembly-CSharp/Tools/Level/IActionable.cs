using System;

namespace Tools.Level
{
	public interface IActionable
	{
		void Use();

		bool Locked { get; set; }
	}
}
