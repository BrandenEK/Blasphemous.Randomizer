using System;

namespace Framework.Randomizer
{
	public struct ItemData
	{
		public bool canBreakHoles
		{
			get
			{
				return this.swordLevel > 0 || (this.cherubBits & 131071) > 0;
			}
		}

		public bool bridgeAccess
		{
			get
			{
				return this.holyWounds >= 3 && this.blood && this.scapular;
			}
		}

		public bool hasCherubBit(int bitfield)
		{
			return (this.cherubBits & bitfield) > 0;
		}

		public bool blood;

		public bool root;

		public bool lung;

		public bool nail;

		public bool linen;

		public bool shroud;

		public int cherubs;

		public int bones;

		public bool bronzeKey;

		public bool silverKey;

		public bool goldKey;

		public bool elderKey;

		public bool bridge;

		public int masks;

		public bool lungDamage;

		public bool peaksKey;

		public int redWax;

		public int blueWax;

		public bool chalice;

		public bool lightningAir;

		public bool lightningAny;

		public bool guiltBead;

		public bool egg;

		public bool hatchedEgg;

		public bool cloth;

		public bool hand;

		public bool emptyThimble;

		public bool fullThimble;

		public bool driedFlowers;

		public bool cord;

		public int herbs;

		public bool scapular;

		public bool woodKey;

		public bool trueHeart;

		public int ceremonyItems;

		public int marksOfRefuge;

		public int limestones;

		public int tentudiaRemains;

		public int traitorEyes;

		public int swordLevel;

		public bool bell;

		public int verses;

		public int tears;

		public int fervourLevel;

		public int cherubBits;

		public int holyWounds;

		public bool wheel;

		public bool dawnHeart;
	}
}
