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
            get { return holyWounds >= 3 && blood && scapular && canBeatBoss("esdras"); }
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
                if (item.id == 17 || item.id == 18 || item.id == 19) { redWax++; return; }
                if (item.id == 20 || item.id == 21 || item.id == 22) { limestones++; return; }
                if (item.id == 24 || item.id == 25 || item.id == 26) { blueWax++; return; }
                if (item.id == 41) { guiltBead = true; return; }
                if (item.id == 105) { cherubBits |= 131072; return; }
                if (item.id == 203) { cherubBits |= 524288; wheel = true; return; }
            }
            if  (item.type == 1)
            {
                if (item.id == 1) { cherubBits |= 1; return; }
                if (item.id == 3) { cherubBits |= 2; return; }
                if (item.id == 4) { cherubBits |= 4; return; }
                if (item.id == 5) { cherubBits |= 8; return; }
                if (item.id == 7) { cherubBits |= 16; return; }
                if (item.id == 8) { cherubBits |= 32; return; }
                if (item.id == 9) { cherubBits |= 64; return; }
                if (item.id == 10) { cherubBits |= 128; return; }
                if (item.id == 11) { cherubBits |= 256; return; }
                if (item.id == 12) { cherubBits |= 512; return; }
                if (item.id == 14) { cherubBits |= 1024; return; }
                if (item.id == 15) { cherubBits |= 2048; return; }
                if (item.id == 16) { cherubBits |= 4096; return; }
                if (item.id == 101) { cherubBits |= 8192; return; }
                if (item.id == 201) { cherubBits |= 16384; return; }
                if (item.id == 202) { cherubBits |= 32768; return; }
                if (item.id == 203) { cherubBits |= 65536; return; }
            }
            if (item.type == 2)
            {
                if (item.id == 1) { blood = true; return; }
                if (item.id == 3) { nail = true; return; }
                if (item.id == 4) { shroud = true; return; }
                if (item.id == 5) { linen = true; return; }
                if (item.id == 7) { lung = true; return; }
                if (item.id == 10) { root = true; return; }
            }
            if (item.type == 3)
            {
                if (item.id == 101) { dawnHeart = true; cherubBits |= 524288; return; }
                if (item.id == 201) { trueHeart = true; return; }
            }
            if (item.type == 4)
            {
                bones++;
            }
			if (item.type == 5)
			{
				if (item.id == 1) { cord = true; return; }
				if (item.id == 2 || item.id == 3 || item.id == 4) { marksOfRefuge++; return; }
				if (item.id == 6 || item.id == 7 || item.id == 8) { tentudiaRemains++; return; }
				if (item.id == 10 || item.id == 11 || item.id == 12) { ceremonyItems++; return; }
				if (item.id == 13) { egg = true; return; }
				if (item.id == 14) { hatchedEgg = true; return; }
                if (item.id == 19 || item.id == 20 || item.id == 37 || item.id == 63 || item.id == 64 || item.id == 65) { herbs++; return; }
                if (item.id == 38 || item.id == 39 || item.id == 40) { holyWounds++; return; }
                if (item.id == 41 || item.id == 45 || item.id == 46 || item.id == 47|| item.id == 48 || item.id == 49 || item.id == 50 || item.id == 51) { flasks++; return; }
				if (item.id == 44 || item.id == 52 || item.id == 53 || item.id == 54 || item.id == 55 || item.id == 56 ) { knots++; return; }
                if (item.id == 57) { fullThimble = true; return; }
				if (item.id == 58) { elderKey = true; return; }
				if (item.id == 59) { emptyThimble = true; return; }
				if (item.id == 60 || item.id == 61 || item.id == 62) { masks++; return; }
				if (item.id == 66) { cloth = true; return; }
				if (item.id == 67) { hand = true; return; }
				if (item.id == 68) { driedFlowers = true; return; }
				if (item.id == 69) { bronzeKey = true; return; }
				if (item.id == 70) { silverKey = true; return; }
				if (item.id == 71) { goldKey = true; return; }
				if (item.id == 72) { peaksKey = true; return; }
				if (item.id == 75) { chalice = true; return; }
                if (item.id == 101 || item.id == 102 || item.id == 103 || item.id == 104 || item.id == 105) { quicksilver++; return; }
				if (item.id == 106) { bell = true; return; }
				if (item.id == 107 || item.id == 108 || item.id == 109 || item.id == 110) { verses++; return; }
				if (item.id == 201 || item.id == 202) { traitorEyes++; return; }
				if (item.id == 203) { scapular = true; return; }
				if (item.id == 204) { woodKey = true; return; }
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
                tears += item.id;
            }
        }
    }
}
