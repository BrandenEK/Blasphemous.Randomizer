using System;
using Rewired.Dev;

namespace RewiredConsts
{
	public static class Player
	{
		[PlayerIdFieldInfo(friendlyName = "System")]
		public const int System = 9999999;

		[PlayerIdFieldInfo(friendlyName = "Player0")]
		public const int Player0 = 0;
	}
}
