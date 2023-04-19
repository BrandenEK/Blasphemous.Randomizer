using System.Collections.Generic;
using LogicParser;

namespace BlasphemousRandomizer.ItemRando
{
    public class BlasphemousInventory : InventoryData
    {
        // Relics
        private bool blood = false;
        private bool root = false;
        private bool linen = false;
        private bool nail = false;
        private bool shroud = false;
        private bool lung = false;

        // Keys
        private bool bronzeKey = false;
        private bool silverKey = false;
        private bool goldKey = false;
        private bool peaksKey = false;
        private bool elderKey = false;
        private bool woodKey = false;

        // Collections
        private int cherubs = 0;
        private int bones = 0;
        private int tears = 0;

        // Modded items
        private bool doubleJump = false;
        private bool boots = false;

        // Speed boosts
        private bool wheel = false;
        private bool dawnHeart = false;

        // Health boosts
        private int flasks = 0;
        private int quicksilver = 0;

        // Puzzles
        private int redWax = 0;
        private int blueWax = 0;
        private bool chalice = false;

        // Prayers
        private int cherubBitfield = 0;
        private bool tirana = false;
        private bool tiento = false;
        private bool ownAubade = false, ownTirana = false;

        // Stats
        private int healthLevel = 0, fervourLevel = 0, swordLevel = 0;

        // Skills
        private bool combo = false;
        private bool charged = false;
        private bool ranged = false;
        private bool dive = false;
        private bool lunge = false;

        // Main quest
        private int holyWounds = 0;
        private int masks = 0;
        private bool guiltBead = false;

        // LOTL quest
        private bool cloth = false;
        private bool hand = false;
        private bool hatchedEgg = false;

        // Tirso quest
        private int herbs = 0;

        // Tentudia quest
        private int tentudiaRemains = 0;

        // Gemino quest
        private bool emptyThimble = false;
        private bool fullThimble = false;
        private bool driedFlowers = false;

        // Altasgracias quest
        private int ceremonyItems = 0;
        private bool egg = false;

        // Redento quest
        private int limestones = 0;
        private int knots = 0;

        // Cleofas quest
        private int marksOfRefuge = 0;
        private bool cord = false;

        // Crisanta quest
        private bool scapular = false;
        private bool trueHeart = false;
        private int traitorEyes = 0;

        // Jibrael quest
        private bool bell = false;
        private int verses = 0;

        private bool canBreakHoles => charged || dive || (cherubBitfield & 0x1FFFF) > 0;

        private int bossPower => healthLevel + swordLevel + flasks + quicksilver;

        private int guiltRooms
        {
            get
            {
                int rooms = 0;
                if (doors.ContainsKey("D01Z04S17[W]")) rooms++;
                if (doors.ContainsKey("D02Z02S06[E]")) rooms++;
                if (doors.ContainsKey("D03Z03S14[W]")) rooms++;
                if (doors.ContainsKey("D04Z02S17[W]")) rooms++;
                if (doors.ContainsKey("D05Z01S17[W]")) rooms++;
                if (doors.ContainsKey("D09Z01S13[E]")) rooms++;
                if (doors.ContainsKey("D17Z01S12[E]")) rooms++;
                return rooms;
            }
        }

        private int swordRooms
        {
            get
            {
                int rooms = 0;
                if (doors.ContainsKey("D01Z02S06[W]") || doors.ContainsKey("D01Z02S06[E]")) rooms++;
                if (doors.ContainsKey("D01Z05S24[W]") || doors.ContainsKey("D01Z05S24[E]")) rooms++;
                if (doors.ContainsKey("D02Z03S13[W]")) rooms++;
                if (doors.ContainsKey("D04Z02S12[W]")) rooms++;
                if (doors.ContainsKey("D05Z01S13[E]")) rooms++;
                if (doors.ContainsKey("D06Z01S11[W]")) rooms++;
                if (doors.ContainsKey("D17Z01S08[E]")) rooms++;
                return rooms;
            }
        }

        private int miriamRooms
        {
            get
            {
                int rooms = 0;
                if (doors.ContainsKey("D02Z03S24[E]")) rooms++;
                if (doors.ContainsKey("D03Z03S19[E]")) rooms++;
                if (doors.ContainsKey("D04Z04S02[W]")) rooms++;
                if (doors.ContainsKey("D05Z01S24[E]")) rooms++;
                if (doors.ContainsKey("D06Z01S26[W]")) rooms++;
                return rooms;
            }
        }

        private int amanecidaRooms
        {
            get
            {
                int rooms = 0;
                if (doors.ContainsKey("D02Z03S23[E]") && doors.ContainsKey("D02Z02S14[W]")) rooms++;
                if (doors.ContainsKey("D06Z01S18[E]") && (doors.ContainsKey("D04Z01S04[W]") || doors.ContainsKey("D04Z01S04[E]") || doors.ContainsKey("D04Z01S04[Cherubs]"))) rooms++;
                if ((doors.ContainsKey("D20Z01S05[W]") || doors.ContainsKey("D20Z01S05[E]")) && (doors.ContainsKey("D03Z01S03[W]") || doors.ContainsKey("D03Z01S03[SW]"))) rooms++;
                if (doors.ContainsKey("D09BZ01S01[Cell24]") && (doors.ContainsKey("D09Z01S01[W]") || doors.ContainsKey("D09Z01S01[E]"))) rooms++;
                return rooms;
            }
        }

        private bool alberoElevator => doors.ContainsKey("D02Z02S11[W]") || doors.ContainsKey("D02Z02S11[SE]") || doors.ContainsKey("D02Z02S11[E]") || doors.ContainsKey("D02Z02S11[NW]") || doors.ContainsKey("D02Z02S11[NE]");

        private bool jondoBell => (doors.ContainsKey("D03Z02S05[W]") || doors.ContainsKey("D03Z02S05[S]") || doors.ContainsKey("D03Z02S05[E]") && dawnHeart && ranged) && (doors.ContainsKey("D03Z02S09[S]") || doors.ContainsKey("D03Z02S09[W]") || doors.ContainsKey("D03Z02S09[N]") || doors.ContainsKey("D03Z02S09[Cherubs]"));

        private bool teleportRoom => doors.ContainsKey("D01Z02S07[E]") || doors.ContainsKey("D01Z04S02[W]") || doors.ContainsKey("D03Z03S18[E]") || doors.ContainsKey("D02Z03S22[W]") || doors.ContainsKey("D04Z02S25[W]") || doors.ContainsKey("D05Z01S16[W]") || doors.ContainsKey("D06Z01S05[E]") || doors.ContainsKey("D08Z02S02[W]") || doors.ContainsKey("D17Z01S06[E]") || doors.ContainsKey("D20Z01S12[E]");

        private bool chaliceQuest => chalice && (doors.ContainsKey("D03Z01S01[W]") || doors.ContainsKey("D03Z01S01[NE]") || doors.ContainsKey("D03Z01S01[S]")) && (doors.ContainsKey("D05Z02S01[W]") || doors.ContainsKey("D05Z02S01[E]")) && (doors.ContainsKey("D09Z01S07[SW]") || doors.ContainsKey("D09Z01S07[SE]") || doors.ContainsKey("D09Z01S07[W]") || doors.ContainsKey("D09Z01S07[E]"));

        // Doors
        private Dictionary<string, bool> doors = new Dictionary<string, bool>();

        protected override Variable GetVariable(string variable)
        {
            if (variable[0] == 'D')
            {
                return new BoolVariable(doors.ContainsKey(variable) && doors[variable]);
            }

            switch (variable)
            {
                case "blood": return new BoolVariable(blood);
                case "root": return new BoolVariable(root);
                case "linen": return new BoolVariable(linen);
                case "nail": return new BoolVariable(nail);
                case "shroud": return new BoolVariable(shroud);
                case "lung": return new BoolVariable(lung);

                case "bronzeKey": return new BoolVariable(bronzeKey);
                case "silverKey": return new BoolVariable(silverKey);
                case "goldKey": return new BoolVariable(goldKey);
                case "peaksKey": return new BoolVariable(peaksKey);
                case "elderKey": return new BoolVariable(elderKey);
                case "woodKey": return new BoolVariable(woodKey);

                case "doubleJump": return new BoolVariable(doubleJump);
                case "boots": return new BoolVariable(boots);

                case "cherubs": return new IntVariable(cherubs);
                case "bones": return new IntVariable(bones);
                case "tears": return new IntVariable(tears);

                case "tirana": return new BoolVariable(tirana);
                case "tiento": return new BoolVariable(tiento);
                case "cherubBitfield": return new IntVariable(cherubBitfield);

                case "combo": return new BoolVariable(combo);
                case "charged": return new BoolVariable(charged);
                case "ranged": return new BoolVariable(ranged);
                case "dive": return new BoolVariable(dive);
                case "lunge": return new BoolVariable(lunge);

                case "wheel": return new BoolVariable(wheel);
                case "dawnHeart": return new BoolVariable(dawnHeart);

                case "redWax": return new IntVariable(redWax);
                case "blueWax": return new IntVariable(blueWax);
                case "chalice": return new BoolVariable(chalice);

                case "holyWounds": return new IntVariable(holyWounds);
                case "masks": return new IntVariable(masks);
                case "guiltBead": return new BoolVariable(guiltBead);

                case "cloth": return new BoolVariable(cloth);
                case "hand": return new BoolVariable(hand);
                case "hatchedEgg": return new BoolVariable(hatchedEgg);

                case "herbs": return new IntVariable(herbs);

                case "tentudiaRemains": return new IntVariable(tentudiaRemains);

                case "emptyThimble": return new BoolVariable(emptyThimble);
                case "fullThimble": return new BoolVariable(fullThimble);
                case "driedFlowers": return new BoolVariable(driedFlowers);

                case "ceremonyItems": return new IntVariable(ceremonyItems);
                case "egg": return new BoolVariable(egg);

                case "limestones": return new IntVariable(limestones);
                case "knots": return new IntVariable(knots);

                case "marksOfRefuge": return new IntVariable(marksOfRefuge);
                case "cord": return new BoolVariable(cord);

                case "scapular": return new BoolVariable(scapular);
                case "trueHeart": return new BoolVariable(trueHeart);
                case "traitorEyes": return new IntVariable(traitorEyes);

                case "bell": return new BoolVariable(bell);
                case "verses": return new IntVariable(verses);

                case "canBreakHoles": return new BoolVariable(canBreakHoles);

                case "guiltRooms": return new IntVariable(guiltRooms);
                case "swordRooms": return new IntVariable(swordRooms);
                case "miriamRooms": return new IntVariable(miriamRooms);
                case "amanecidaRooms": return new IntVariable(amanecidaRooms);

                // Will need to be changed with boss rando
                case "boss-brotherhood": return new BoolVariable(bossPower >= 0);
                case "boss-mercy": return new BoolVariable(bossPower >= 0);
                case "boss-convent": return new BoolVariable(bossPower >= 3);
                case "boss-grievance": return new BoolVariable(bossPower >= 3);
                case "boss-bridge": return new BoolVariable(bossPower >= 4);
                case "boss-mothers": return new BoolVariable(bossPower >= 4);
                case "boss-canvases": return new BoolVariable(bossPower >= 4);
                case "boss-wall": return new BoolVariable(bossPower >= 4);
                case "boss-rooftops": return new BoolVariable(bossPower >= 10);
                case "boss-ossuary": return new BoolVariable(bossPower >= 15);
                case "boss-mourning": return new BoolVariable(bossPower >= 9);
                case "boss-amanecida": return new BoolVariable(bossPower >= 12);
                case "boss-laudes": return new BoolVariable(bossPower >= 12);

                case "albero-elevator": return new BoolVariable(alberoElevator); // access to graveyard elevator room
                case "jondo-bell": return new BoolVariable(jondoBell); // access to both jondo bell rooms
                case "teleport-room": return new BoolVariable(teleportRoom); // access to any teleport room
                case "chalice-quest": return new BoolVariable(chaliceQuest); // chalice && access to the 3 enemy rooms

                default:
                    throw new System.Exception($"Error: Variable {variable} doesn't exist!");
            }
        }

        public void AddItem(string itemId)
        {
            if (itemId[0] == 'D')
            {
                if (!doors.ContainsKey(itemId))
                    doors.Add(itemId, true);
                return;
            }

            Item item = Main.Randomizer.data.items[itemId];
            switch (item.type)
            {
                case 0:
                    {
                        if (item.id == "RB20" || item.id == "RB21" || item.id == "RB22") limestones++;
                        else if (item.id == "RB41") guiltBead = true;
                        else if (item.id == "RB105") cherubBitfield |= 0x20000;
                        else if (item.id == "RB203") wheel = true;
                        else if (item.id == "RW") redWax++;
                        else if (item.id == "BW") blueWax++;
                        break;
                    }
                case 1:
                    {
                        if (item.id == "PR01") cherubBitfield |= 0x01;
                        else if (item.id == "PR03") cherubBitfield |= 0x02;
                        else if (item.id == "PR04") cherubBitfield |= 0x04;
                        else if (item.id == "PR05") cherubBitfield |= 0x08;
                        else if (item.id == "PR07") cherubBitfield |= 0x10;
                        else if (item.id == "PR08") cherubBitfield |= 0x20;
                        else if (item.id == "PR09") cherubBitfield |= 0x40;
                        else if (item.id == "PR10") cherubBitfield |= 0x80;
                        else if (item.id == "PR11") { cherubBitfield |= 0x100; tiento = true; }
                        else if (item.id == "PR12") cherubBitfield |= 0x200;
                        else if (item.id == "PR14") cherubBitfield |= 0x400;
                        else if (item.id == "PR15") cherubBitfield |= 0x800;
                        else if (item.id == "PR16") cherubBitfield |= 0x1000;
                        else if (item.id == "PR101") ownAubade = true;
                        else if (item.id == "PR201") cherubBitfield |= 0x4000;
                        else if (item.id == "PR202") cherubBitfield |= 0x8000;
                        else if (item.id == "PR203") ownTirana = true;
                        break;
                    }
                case 2:
                    {
                        if (item.id == "RE01") blood = true;
                        else if (item.id == "RE03") nail = true;
                        else if (item.id == "RE04") shroud = true;
                        else if (item.id == "RE05") linen = true;
                        else if (item.id == "RE07") lung = true;
                        else if (item.id == "RE10") root = true;
                        else if (item.id == "RE401") boots = true;
                        else if (item.id == "RE402") doubleJump = true;
                        break;
                    }
                case 3:
                    {
                        if (item.id == "HE101") dawnHeart = true;
                        else if (item.id == "HE201") trueHeart = true;
                        break;
                    }
                case 4:
                    {
                        bones++;
                        break;
                    }
                case 5:
                    {
                        if (item.id == "QI01") cord = true;
                        else if (item.id == "QI02" || item.id == "QI03" || item.id == "QI04") marksOfRefuge++;
                        else if (item.id == "QI06" || item.id == "QI07" || item.id == "QI08") tentudiaRemains++;
                        else if (item.id == "QI10" || item.id == "QI11" || item.id == "QI12") ceremonyItems++;
                        else if (item.id == "QI13") egg = true;
                        else if (item.id == "QI14") hatchedEgg = true;
                        else if (item.id == "QI19" || item.id == "QI20" || item.id == "QI37" || item.id == "QI63" || item.id == "QI64" || item.id == "QI65") herbs++;
                        else if (item.id == "QI38" || item.id == "QI39" || item.id == "QI40") holyWounds++;
                        else if (item.id == "QI57") fullThimble = true;
                        else if (item.id == "QI58") elderKey = true;
                        else if (item.id == "QI59") emptyThimble = true;
                        else if (item.id == "QI60" || item.id == "QI61" || item.id == "QI62") masks++;
                        else if (item.id == "QI66") cloth = true;
                        else if (item.id == "QI67") hand = true;
                        else if (item.id == "QI68") driedFlowers = true;
                        else if (item.id == "QI69") bronzeKey = true;
                        else if (item.id == "QI70") silverKey = true;
                        else if (item.id == "QI71") goldKey = true;
                        else if (item.id == "QI72") peaksKey = true;
                        else if (item.id == "QI75") chalice = true;
                        else if (item.id == "QI106") bell = true;
                        else if (item.id == "QI201" || item.id == "QI202") traitorEyes++;
                        else if (item.id == "QI203") scapular = true;
                        else if (item.id == "QI204") woodKey = true;
                        else if (item.id == "BV") flasks++;
                        else if (item.id == "RK") knots++;
                        else if (item.id == "QS") quicksilver++;
                        else if (item.id == "GV") verses++;
                        break;
                    }
                case 6:
                    {
                        cherubs++;
                        break;
                    }
                case 7:
                    {
                        healthLevel++;
                        break;
                    }
                case 8:
                    {
                        fervourLevel++;
                        break;
                    }
                case 9:
                    {
                        swordLevel++;
                        break;
                    }
                case 10:
                    {
                        tears += item.tearAmount;
                        break;
                    }
                case 11:
                    {
                        if (item.id == "COMBO") combo = true;
                        else if (item.id == "CHARGED") charged = true;
                        else if (item.id == "RANGED") { ranged = true; cherubBitfield |= 0x40000; }
                        else if (item.id == "DIVE") dive = true;
                        else if (item.id == "LUNGE") lunge = true;
                        break;
                    }
            }

            // Recalculate cherub bitfield based on fervour
            if (fervourLevel >= 2)
            {
                if (ownAubade)
                {
                    cherubBitfield |= 0x2000;
                }
                if (ownTirana)
                {
                    cherubBitfield |= 0x10000;
                    tirana = true;
                }
            }
        }
    }
}
