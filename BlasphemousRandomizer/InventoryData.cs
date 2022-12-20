using BlasphemousRandomizer.Structures;

namespace BlasphemousRandomizer
{
    public struct InventoryData
    {
        // Relics
        public bool blood;
        public bool root;
        public bool lung;
        public bool lungDamage;
        public bool nail;
        public bool linen;
        public bool shroud;

        // Keys
        public bool bronzeKey;
        public bool silverKey;
        public bool goldKey;
        public bool elderKey;
        public bool peaksKey;
        public bool woodKey;

        // Stats
        public int healthLevel;
        public int fervourLevel;
        public int swordLevel;

        // Puzzles
        public int redWax;
        public int blueWax;
        public bool chalice;
        public int cherubBits;

        // Blessing items
        public bool cloth;
        public bool hand;
        public bool hatchedEgg;

        // Helpful items
        public int flasks;
        public int quicksilver;
        public int knots;

        // Speed items
        public bool wheel;
        public bool dawnHeart;

        // Collections
        public int cherubs;
        public int bones;
        public int tears;

        // Main story
        public int holyWounds;
        public int masks;
        public bool guiltBead;

        // Quest - Tirso
        public int herbs;

        // Quest - Tentudia
        public int tentudiaRemains;

        // Quest - Gemino
        public bool emptyThimble;
        public bool fullThimble;
        public bool driedFlowers;

        // Quest - Altasgracias
        public int ceremonyItems;
        public bool egg;

        // Quest - Redento
        public int limestones;

        // Quest - Cleofas
        public int marksOfRefuge;
        public bool cord;

        // Quest - Crisanta
        public bool scapular;
        public int traitorEyes;
        public bool trueHeart;

        // Quest - Jibrael
        public bool bell;
        public int verses;

        public bool bridgeAccess
        {
            get { return (holyWounds >= 3 && canBeatBoss("esdras")) || (blood && dawnHeart && swordLevel > 1) || (blood && cherubAttack(65536)); }
        }

        public bool canBreakHoles
        {
            get { return swordLevel > 0 || cherubAttack(131071); }
        }

        public int power
        {
            get { return healthLevel + swordLevel + flasks + quicksilver; }
        }

        public bool cherubAttack(int bitfield)
        {
            return (cherubBits & bitfield) > 0;
        }

        public bool canBeatBoss(string boss)
        {
            // First five bosses are not used as requirements yet
            if (boss == "warden") return power >= 0;
            else if (boss == "ten-piedad") return power >= 0;
            else if (boss == "charred-lady") return power >= 0;
            else if (boss == "tres-angustias") return power >= 0;
            else if (boss == "esdras") return power >= 0; // would change first seeding
            else if (boss == "melquiades") return power >= 4;
            else if (boss == "exposito") return power >= 4;
            else if (boss == "quirce") return power >= 4;
            else if (boss == "crisanta") return power >= 10;
            else if (boss == "amanecida") return power >= 12;
            else if (boss == "laudes") return power >= 12;
            else if (boss == "sierpes") return power >= 7;
            else if (boss == "isidora") return power >= 15;
            else return false;
        }

        public void addItem(Item item)
        {
            if (item.type == 0)
            {
                int id = int.Parse(item.name.Substring(2));
                if (id == 17 || id == 18 || id == 19) { redWax++; return; }
                if (id == 20 || id == 21 || id == 22) { limestones++; return; }
                if (id == 24 || id == 25 || id == 26) { blueWax++; return; }
                if (id == 41) { guiltBead = true; return; }
                if (id == 105) { cherubBits |= 131072; return; }
                if (id == 203) { cherubBits |= 524288; wheel = true; return; }
            }
            if  (item.type == 1)
            {
                int id = int.Parse(item.name.Substring(2));
                if (id == 1) { cherubBits |= 1; return; }
                if (id == 3) { cherubBits |= 2; return; }
                if (id == 4) { cherubBits |= 4; return; }
                if (id == 5) { cherubBits |= 8; return; }
                if (id == 7) { cherubBits |= 16; return; }
                if (id == 8) { cherubBits |= 32; return; }
                if (id == 9) { cherubBits |= 64; return; }
                if (id == 10) { cherubBits |= 128; return; }
                if (id == 11) { cherubBits |= 256; return; }
                if (id == 12) { cherubBits |= 512; return; }
                if (id == 14) { cherubBits |= 1024; return; }
                if (id == 15) { cherubBits |= 2048; return; }
                if (id == 16) { cherubBits |= 4096; return; }
                if (id == 101) { cherubBits |= 8192; return; }
                if (id == 201) { cherubBits |= 16384; return; }
                if (id == 202) { cherubBits |= 32768; return; }
                if (id == 203) { cherubBits |= 65536; return; }
            }
            if (item.type == 2)
            {
                int id = int.Parse(item.name.Substring(2));
                if (id == 1) { blood = true; return; }
                if (id == 3) { nail = true; return; }
                if (id == 4) { shroud = true; return; }
                if (id == 5) { linen = true; return; }
                if (id == 7) { lung = true; return; }
                if (id == 10) { root = true; return; }
            }
            if (item.type == 3)
            {
                int id = int.Parse(item.name.Substring(2));
                if (id == 101) { dawnHeart = true; cherubBits |= 524288; return; }
                if (id == 201) { trueHeart = true; return; }
            }
            if (item.type == 4)
            {
                bones++;
            }
			if (item.type == 5)
			{
                int id = int.Parse(item.name.Substring(2));
                if (id == 1) { cord = true; return; }
				if (id == 2 || id == 3 || id == 4) { marksOfRefuge++; return; }
				if (id == 6 || id == 7 || id == 8) { tentudiaRemains++; return; }
				if (id == 10 || id == 11 || id == 12) { ceremonyItems++; return; }
				if (id == 13) { egg = true; return; }
				if (id == 14) { hatchedEgg = true; return; }
                if (id == 19 || id == 20 || id == 37 || id == 63 || id == 64 || id == 65) { herbs++; return; }
                if (id == 38 || id == 39 || id == 40) { holyWounds++; return; }
                if (id == 41 || id == 45 || id == 46 || id == 47|| id == 48 || id == 49 || id == 50 || id == 51) { flasks++; return; }
				if (id == 44 || id == 52 || id == 53 || id == 54 || id == 55 || id == 56 ) { knots++; return; }
                if (id == 57) { fullThimble = true; return; }
				if (id == 58) { elderKey = true; return; }
				if (id == 59) { emptyThimble = true; return; }
				if (id == 60 || id == 61 || id == 62) { masks++; return; }
				if (id == 66) { cloth = true; return; }
				if (id == 67) { hand = true; return; }
				if (id == 68) { driedFlowers = true; return; }
				if (id == 69) { bronzeKey = true; return; }
				if (id == 70) { silverKey = true; return; }
				if (id == 71) { goldKey = true; return; }
				if (id == 72) { peaksKey = true; return; }
				if (id == 75) { chalice = true; return; }
                if (id == 101 || id == 102 || id == 103 || id == 104 || id == 105) { quicksilver++; return; }
				if (id == 106) { bell = true; return; }
				if (id == 107 || id == 108 || id == 109 || id == 110) { verses++; return; }
				if (id == 201 || id == 202) { traitorEyes++; return; }
				if (id == 203) { scapular = true; return; }
				if (id == 204) { woodKey = true; return; }
			}
            if (item.type == 6)
            {
                cherubs++;
            }
            if (item.type == 8)
            {
                fervourLevel++;
            }
            if (item.type == 9)
            {
                swordLevel++;
            }
            if (item.type == 10)
            {
                tears += item.tearAmount;
            }
        }
    }
}
