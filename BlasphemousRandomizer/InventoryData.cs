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

        // Skills
        public bool combo;
        public bool charged;
        public bool ranged;
        public bool dive;
        public bool lunge;

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
            get { return (holyWounds >= 3 && canBeatBoss("esdras")) || (blood && dawnHeart && ranged) || (blood && cherubAttack(0x10000)); }
        }

        public bool canBreakHoles
        {
            get { return charged || dive || cherubAttack(0x1FFFF); }
        }

        public int power
        {
            get { return healthLevel + swordLevel + flasks + quicksilver; }
        }

        public int swordRooms
        {
            get
            {
                if (bridgeAccess)
                {
                    int num = 4;
                    if (canBeatBoss("exposito")) num++;
                    if (masks > 1 && blood && lung) num++;
                    if (chalice && masks > 0 && bronzeKey && ((linen && (ranged || root)) || (lung && nail && (root || dawnHeart || wheel && ranged)))) num++;
                    return num;
                }
                return 3;
            }
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
                if (item.id == "RW") { redWax++; return; }
                if (item.id == "RB20" || item.id == "RB21" || item.id == "RB22") { limestones++; return; }
                if (item.id == "BW") { blueWax++; return; }
                if (item.id == "RB41") { guiltBead = true; return; }
                if (item.id == "RB105") { cherubBits |= 0x20000; return; }
                if (item.id == "RB203") { wheel = true; return; }
            }
            if (item.type == 1)
            {
                if (item.id == "PR01") { cherubBits |= 0x01; return; }
                if (item.id == "PR03") { cherubBits |= 0x02; return; }
                if (item.id == "PR04") { cherubBits |= 0x04; return; }
                if (item.id == "PR05") { cherubBits |= 0x08; return; }
                if (item.id == "PR07") { cherubBits |= 0x10; return; }
                if (item.id == "PR08") { cherubBits |= 0x20; return; }
                if (item.id == "PR09") { cherubBits |= 0x40; return; }
                if (item.id == "PR10") { cherubBits |= 0x80; return; }
                if (item.id == "PR11") { cherubBits |= 0x100; return; }
                if (item.id == "PR12") { cherubBits |= 0x200; return; }
                if (item.id == "PR14") { cherubBits |= 0x400; return; }
                if (item.id == "PR15") { cherubBits |= 0x800; return; }
                if (item.id == "PR16") { cherubBits |= 0x1000; return; }
                if (item.id == "PR101") { cherubBits |= 0x2000; return; }
                if (item.id == "PR201") { cherubBits |= 0x4000; return; }
                if (item.id == "PR202") { cherubBits |= 0x8000; return; }
                if (item.id == "PR203") { cherubBits |= 0x10000; return; }
            }
            if (item.type == 2)
            {
                if (item.id == "RE01") { blood = true; return; }
                if (item.id == "RE03") { nail = true; return; }
                if (item.id == "RE04") { shroud = true; return; }
                if (item.id == "RE05") { linen = true; return; }
                if (item.id == "RE07") { lung = true; return; }
                if (item.id == "RE10") { root = true; return; }
            }
            if (item.type == 3)
            {
                if (item.id == "HE101") { dawnHeart = true; return; }
                if (item.id == "HE201") { trueHeart = true; return; }
            }
            if (item.type == 4)
            {
                bones++;
            }
            if (item.type == 5)
            {
                if (item.id == "QI01") { cord = true; return; }
                if (item.id == "QI02" || item.id == "QI03" || item.id == "QI04") { marksOfRefuge++; return; }
                if (item.id == "QI06" || item.id == "QI07" || item.id == "QI08") { tentudiaRemains++; return; }
                if (item.id == "QI10" || item.id == "QI11" || item.id == "QI12") { ceremonyItems++; return; }
                if (item.id == "QI13") { egg = true; return; }
                if (item.id == "QI14") { hatchedEgg = true; return; }
                if (item.id == "QI19" || item.id == "QI20" || item.id == "QI37" || item.id == "QI63" || item.id == "QI64" || item.id == "QI65") { herbs++; return; }
                if (item.id == "QI38" || item.id == "QI39" || item.id == "QI40") { holyWounds++; return; }
                if (item.id == "BV") { flasks++; return; }
                if (item.id == "RK") { knots++; return; }
                if (item.id == "QI57") { fullThimble = true; return; }
                if (item.id == "QI58") { elderKey = true; return; }
                if (item.id == "QI59") { emptyThimble = true; return; }
                if (item.id == "QI60" || item.id == "QI61" || item.id == "QI62") { masks++; return; }
                if (item.id == "QI66") { cloth = true; return; }
                if (item.id == "QI67") { hand = true; return; }
                if (item.id == "QI68") { driedFlowers = true; return; }
                if (item.id == "QI69") { bronzeKey = true; return; }
                if (item.id == "QI70") { silverKey = true; return; }
                if (item.id == "QI71") { goldKey = true; return; }
                if (item.id == "QI72") { peaksKey = true; return; }
                if (item.id == "QI75") { chalice = true; return; }
                if (item.id == "QS") { quicksilver++; return; }
                if (item.id == "QI106") { bell = true; return; }
                if (item.id == "GV") { verses++; return; }
                if (item.id == "QI201" || item.id == "QI202") { traitorEyes++; return; }
                if (item.id == "QI203") { scapular = true; return; }
                if (item.id == "QI204") { woodKey = true; return; }
            }
            if (item.type == 6)
            {
                cherubs++; return;
            }
            if (item.type == 8)
            {
                fervourLevel++; return;
            }
            if (item.type == 9)
            {
                swordLevel++; return;
            }
            if (item.type == 10)
            {
                tears += item.tearAmount; return;
            }
            if (item.type == 11)
            {
                if (item.id == "COMBO") { combo = true; return; }
                if (item.id == "CHARGED") { charged = true; return; }
                if (item.id == "RANGED") { ranged = true; cherubBits |= 0x40000; return; }
                if (item.id == "DIVE") { dive = true; return; }
                if (item.id == "LUNGE") { lunge = true; return; }
            }
        }
    }
}
