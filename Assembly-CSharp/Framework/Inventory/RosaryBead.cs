using System;
using Framework.Managers;

namespace Framework.Inventory
{
	public class RosaryBead : EquipableInventoryObject
	{
		public override InventoryManager.ItemType GetItemType()
		{
			return InventoryManager.ItemType.Bead;
		}

		public static class Id
		{
			public const string PigeonSkull = "RB01";

			public const string UvulaProclamation = "RB04";

			public const string HollowPearl = "RB05";

			public const string BallOfHair = "RB06";

			public const string FrozenOlive = "RB10";

			public const string RedWaxSmallBall = "RB17";

			public const string RedWaxMediumBall = "RB18";

			public const string RedWaxBigBall = "RB19";

			public const string LimestoneRingFinger = "RB22";

			public const string BlueWaxSmallBall = "RB24";

			public const string BlueWaxMediumBall = "RB25";

			public const string BlueWaxBigBall = "RB26";

			public const string PelicanEffigy = "RB28";
		}
	}
}
