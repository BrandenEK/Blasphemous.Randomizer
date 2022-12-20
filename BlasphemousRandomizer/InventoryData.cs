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
                if (item.name == "RW") { redWax++; return; }
                if (item.name == "RB20" || item.name == "RB21" || item.name == "RB22") { limestones++; return; }
                if (item.name == "BW") { blueWax++; return; }
                if (item.name == "RB41") { guiltBead = true; return; }
                if (item.name == "RB105") { cherubBits |= 131072; return; }
                if (item.name == "RB203") { cherubBits |= 524288; wheel = true; return; }
            }
            if  (item.type == 1)
            {
                if (item.name == "PR01") { cherubBits |= 1; return; }
                if (item.name == "PR03") { cherubBits |= 2; return; }
                if (item.name == "PR04") { cherubBits |= 4; return; }
                if (item.name == "PR05") { cherubBits |= 8; return; }
                if (item.name == "PR07") { cherubBits |= 16; return; }
                if (item.name == "PR08") { cherubBits |= 32; return; }
                if (item.name == "PR09") { cherubBits |= 64; return; }
                if (item.name == "PR10") { cherubBits |= 128; return; }
                if (item.name == "PR11") { cherubBits |= 256; return; }
                if (item.name == "PR12") { cherubBits |= 512; return; }
                if (item.name == "PR14") { cherubBits |= 1024; return; }
                if (item.name == "PR15") { cherubBits |= 2048; return; }
                if (item.name == "PR16") { cherubBits |= 4096; return; }
                if (item.name == "PR101") { cherubBits |= 8192; return; }
                if (item.name == "PR201") { cherubBits |= 16384; return; }
                if (item.name == "PR202") { cherubBits |= 32768; return; }
                if (item.name == "PR203") { cherubBits |= 65536; return; }
            }
            if (item.type == 2)
            {
                if (item.name == "RE01") { blood = true; return; }
                if (item.name == "RE03") { nail = true; return; }
                if (item.name == "RE04") { shroud = true; return; }
                if (item.name == "RE05") { linen = true; return; }
                if (item.name == "RE07") { lung = true; return; }
                if (item.name == "RE10") { root = true; return; }
            }
            if (item.type == 3)
            {
                if (item.name == "HE101") { dawnHeart = true; cherubBits |= 524288; return; }
                if (item.name == "HE201") { trueHeart = true; return; }
            }
            if (item.type == 4)
            {
                bones++;
            }
			if (item.type == 5)
			{
                if (item.name == "QI01") { cord = true; return; }
				if (item.name == "QI02" || item.name == "QI03" || item.name == "QI04") { marksOfRefuge++; return; }
				if (item.name == "QI06" || item.name == "QI07" || item.name == "QI08") { tentudiaRemains++; return; }
				if (item.name == "QI10" || item.name == "QI11" || item.name == "QI12") { ceremonyItems++; return; }
				if (item.name == "QI13") { egg = true; return; }
				if (item.name == "QI14") { hatchedEgg = true; return; }
                if (item.name == "QI19" || item.name == "QI20" || item.name == "QI37" || item.name == "QI63" || item.name == "QI64" || item.name == "QI65") { herbs++; return; }
                if (item.name == "QI38" || item.name == "QI39" || item.name == "QI40") { holyWounds++; return; }
                if (item.name == "BV") { flasks++; return; }
				if (item.name == "RK") { knots++; return; }
                if (item.name == "QI57") { fullThimble = true; return; }
				if (item.name == "QI58") { elderKey = true; return; }
				if (item.name == "QI59") { emptyThimble = true; return; }
				if (item.name == "QI60" || item.name == "QI61" || item.name == "QI62") { masks++; return; }
				if (item.name == "QI66") { cloth = true; return; }
				if (item.name == "QI67") { hand = true; return; }
				if (item.name == "QI68") { driedFlowers = true; return; }
				if (item.name == "QI69") { bronzeKey = true; return; }
				if (item.name == "QI70") { silverKey = true; return; }
				if (item.name == "QI71") { goldKey = true; return; }
				if (item.name == "QI72") { peaksKey = true; return; }
				if (item.name == "QI75") { chalice = true; return; }
                if (item.name == "QS") { quicksilver++; return; }
				if (item.name == "QI106") { bell = true; return; }
				if (item.name == "QI107" || item.name == "QI108" || item.name == "QI109" || item.name == "QI110") { verses++; return; }
				if (item.name == "QI201" || item.name == "QI202") { traitorEyes++; return; }
				if (item.name == "QI203") { scapular = true; return; }
				if (item.name == "QI204") { woodKey = true; return; }
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
