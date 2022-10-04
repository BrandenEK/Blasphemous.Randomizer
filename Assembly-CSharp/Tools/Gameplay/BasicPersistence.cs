using System;
using Framework.Managers;

namespace Tools.Gameplay
{
	public class BasicPersistence : PersistentManager.PersistentData
	{
		public BasicPersistence(string id) : base(id)
		{
		}

		public bool triggered;
	}
}
