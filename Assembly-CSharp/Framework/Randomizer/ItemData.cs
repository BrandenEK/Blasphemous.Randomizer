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

		public void addItem(Reward item)
		{
			if (item.type == 0)
			{
				if (item.id == 17 || item.id == 18 || item.id == 19)
				{
					this.redWax++;
					return;
				}
				if (item.id == 20 || item.id == 21 || item.id == 22)
				{
					this.limestones++;
					return;
				}
				if (item.id == 24 || item.id == 25 || item.id == 26)
				{
					this.blueWax++;
					return;
				}
				if (item.id == 38 || item.id == 39 || item.id == 40 || item.id == 41)
				{
					this.guiltBead = true;
					return;
				}
				if (item.id == 105)
				{
					this.cherubBits |= 131072;
					return;
				}
				if (item.id == 203)
				{
					this.cherubBits |= 524288;
					this.wheel = true;
					return;
				}
			}
			else if (item.type == 1)
			{
				if (item.id == 1)
				{
					this.cherubBits |= 1;
					return;
				}
				if (item.id == 3)
				{
					this.cherubBits |= 2;
					return;
				}
				if (item.id == 4)
				{
					this.cherubBits |= 4;
					return;
				}
				if (item.id == 5)
				{
					this.cherubBits |= 8;
					return;
				}
				if (item.id == 7)
				{
					this.cherubBits |= 16;
					return;
				}
				if (item.id == 8)
				{
					this.cherubBits |= 32;
					return;
				}
				if (item.id == 9)
				{
					this.cherubBits |= 64;
					return;
				}
				if (item.id == 10)
				{
					this.cherubBits |= 128;
					return;
				}
				if (item.id == 11)
				{
					this.cherubBits |= 256;
					return;
				}
				if (item.id == 12)
				{
					this.cherubBits |= 512;
					return;
				}
				if (item.id == 14)
				{
					this.cherubBits |= 1024;
					return;
				}
				if (item.id == 15)
				{
					this.cherubBits |= 2048;
					return;
				}
				if (item.id == 16)
				{
					this.cherubBits |= 4096;
					return;
				}
				if (item.id == 101)
				{
					this.cherubBits |= 8192;
					return;
				}
				if (item.id == 201)
				{
					this.cherubBits |= 16384;
					return;
				}
				if (item.id == 202)
				{
					this.cherubBits |= 32768;
					return;
				}
				if (item.id == 203)
				{
					this.cherubBits |= 65536;
					return;
				}
			}
			else if (item.type == 2)
			{
				if (item.id == 1)
				{
					this.blood = true;
					return;
				}
				if (item.id == 3)
				{
					this.nail = true;
					return;
				}
				if (item.id == 4)
				{
					this.shroud = true;
					return;
				}
				if (item.id == 5)
				{
					this.linen = true;
					return;
				}
				if (item.id == 7)
				{
					this.lung = true;
					return;
				}
				if (item.id == 10)
				{
					this.root = true;
					return;
				}
			}
			else if (item.type == 3)
			{
				if (item.id == 101)
				{
					this.dawnHeart = true;
					this.cherubBits |= 524288;
					return;
				}
				if (item.id == 201)
				{
					this.trueHeart = true;
					return;
				}
			}
			else
			{
				if (item.type == 4)
				{
					this.bones++;
					return;
				}
				if (item.type == 5)
				{
					if (item.id == 1)
					{
						this.cord = true;
						return;
					}
					if (item.id == 2 || item.id == 3 || item.id == 4)
					{
						this.marksOfRefuge++;
						return;
					}
					if (item.id == 6 || item.id == 7 || item.id == 8)
					{
						this.tentudiaRemains++;
						return;
					}
					if (item.id == 10 || item.id == 11 || item.id == 12)
					{
						this.ceremonyItems++;
						return;
					}
					if (item.id == 13)
					{
						this.egg = true;
						return;
					}
					if (item.id == 14)
					{
						this.hatchedEgg = true;
						return;
					}
					if (item.id == 19 || item.id == 20 || item.id == 37 || item.id == 63 || item.id == 64 || item.id == 65)
					{
						this.herbs++;
						return;
					}
					if (item.id == 38 || item.id == 39 || item.id == 40)
					{
						this.holyWounds++;
						return;
					}
					if (item.id == 57)
					{
						this.fullThimble = true;
						return;
					}
					if (item.id == 58)
					{
						this.elderKey = true;
						return;
					}
					if (item.id == 59)
					{
						this.emptyThimble = true;
						return;
					}
					if (item.id == 60 || item.id == 61 || item.id == 62)
					{
						this.masks++;
						return;
					}
					if (item.id == 66)
					{
						this.cloth = true;
						return;
					}
					if (item.id == 67)
					{
						this.hand = true;
						return;
					}
					if (item.id == 68)
					{
						this.driedFlowers = true;
						return;
					}
					if (item.id == 69)
					{
						this.bronzeKey = true;
						return;
					}
					if (item.id == 70)
					{
						this.silverKey = true;
						return;
					}
					if (item.id == 71)
					{
						this.goldKey = true;
						return;
					}
					if (item.id == 72)
					{
						this.peaksKey = true;
						return;
					}
					if (item.id == 75)
					{
						this.chalice = true;
						return;
					}
					if (item.id == 106)
					{
						this.bell = true;
						return;
					}
					if (item.id == 107 || item.id == 108 || item.id == 109 || item.id == 110)
					{
						this.verses++;
						return;
					}
					if (item.id == 201 || item.id == 202)
					{
						this.traitorEyes++;
						return;
					}
					if (item.id == 203)
					{
						this.scapular = true;
						return;
					}
					if (item.id == 204)
					{
						this.woodKey = true;
						return;
					}
				}
				else
				{
					if (item.type == 6)
					{
						this.cherubs++;
						return;
					}
					if (item.type == 8)
					{
						this.fervourLevel++;
						return;
					}
					if (item.type == 9)
					{
						this.swordLevel++;
						return;
					}
					if (item.type == 10)
					{
						this.tears += item.id;
					}
					if (item.type == 99)
					{
						ProgressiveReward progressive = item as ProgressiveReward;
						if (progressive.items[0] == "RB17")
						{
							this.redWax++;
							return;
						}
						if (progressive.items[0] == "RB24")
						{
							this.blueWax++;
							return;
						}
					}
				}
			}
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
