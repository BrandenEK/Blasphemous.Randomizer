using System.Collections.Generic;
using LogicParser;

namespace BlasphemousRandomizer.ItemRando
{
    public class BlasphemousInventory : InventoryData
    {
        private int logicDifficulty;
        private bool enemyShuffle;

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

        // Special items
        private bool dash = false;
        private bool wallClimb = false;
        private bool boots = false;
        private bool doubleJump = false;

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
        private bool taranto = false;
        private bool tiento = false;
        private bool tirana = false;
        private bool aubade = false;
        private bool ownAubade = false, ownTirana = false;

        // Stats
        private int healthLevel = 0, fervourLevel = 0, swordLevel = 0;

        // Skills
        private int combo = 0;
        private int charged = 0;
        private int ranged = 0;
        private int dive = 0;
        private int lunge = 0;

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

        private bool canBreakHoles => charged > 0 || dive > 0 || lunge >= 3 || (cherubBitfield & 0x1FFFF) > 0;

        private bool canDawnJump => dawnHeart && dash && logicDifficulty >= 1;

        private bool canWaterJump => nail || doubleJump;

        private bool canAirStall => ranged > 0 && logicDifficulty >= 1;

        private bool canDiveLaser => dive >= 3 && enemySkipsAllowed;

        private bool canSurvivePoison => lung || (logicDifficulty >= 1 && tiento) || logicDifficulty >= 2; // Fix up.  Cant go upwards with nothing

        private bool canBreakJondoBell => (HasDoor("D03Z02S05[W]") && canCrossGap5 || HasDoor("D03Z02S05[S]") || HasDoor("D03Z02S05[E]")) && (HasDoor("D03Z02S09[S]") || HasDoor("D03Z02S09[W]") && dash || HasDoor("D03Z02S09[N]") || HasDoor("D03Z02S09[Cherubs]"));
        private bool canRideAlberoElevator => HasDoor("D02Z02S11[NW]") || HasDoor("D02Z02S11[NE]") || HasDoor("D02Z02S11[W]") || HasDoor("D02Z02S11[E]") || HasDoor("D02Z02S11[SE]");
        private bool canFillChalice => chalice && (HasDoor("D03Z01S01[W]") || HasDoor("D03Z01S01[NE]") || HasDoor("D03Z01S01[S]")) && (HasDoor("D05Z02S01[W]") || HasDoor("D05Z02S01[E]")) && (HasDoor("D09Z01S07[SW]") || HasDoor("D09Z01S07[SE]") || HasDoor("D09Z01S07[W]") || HasDoor("D09Z01S07[E]"));

        // Crossing gaps
        private bool canCrossGap1 => doubleJump || canDawnJump || wheel || canAirStall;
        private bool canCrossGap2 => doubleJump || canDawnJump || wheel;
        private bool canCrossGap3 => doubleJump || canDawnJump || wheel && canAirStall;
        private bool canCrossGap4 => doubleJump || canDawnJump;
        private bool canCrossGap5 => doubleJump || canDawnJump && canAirStall;
        private bool canCrossGap6 => doubleJump;
        private bool canCrossGap7 => doubleJump && (canDawnJump || wheel || canAirStall);
        private bool canCrossGap8 => doubleJump && (canDawnJump || wheel);
        private bool canCrossGap9 => doubleJump && (canDawnJump || wheel && canAirStall);
        private bool canCrossGap10 => doubleJump && canDawnJump;
        private bool canCrossGap11 => doubleJump && canDawnJump && canAirStall;

        // Special skips
        public bool upwarpSkipsAllowed => logicDifficulty >= 2;
        public bool mourningSkipAllowed => logicDifficulty >= 2;
        public bool enemySkipsAllowed => logicDifficulty >= 2 && !enemyShuffle;
        public bool unknownSkipsAllowed => false;
        public bool preciseSkipsAllowed => false;

        // Bosses
        private int bossPower => healthLevel + swordLevel + flasks + quicksilver; // Will need to be changed with boss rando
        private bool canBeatBrotherhoodBoss => bossPower >= 0 && (HasDoor("D17Z01S11[W]") || HasDoor("D17Z01S11[E]"));
        private bool canBeatMercyBoss => bossPower >= 0 && (HasDoor("D01Z04S18[W]") || HasDoor("D01Z04S18[E]"));
        private bool canBeatConventBoss => bossPower >= 3 && (HasDoor("D02Z03S20[W]") || HasDoor("D02Z03S20[E]"));
        private bool canBeatGrievanceBoss => bossPower >= 3 && (HasDoor("D03Z03S15[W]") || HasDoor("D03Z03S15[E]"));
        private bool canBeatBridgeBoss => bossPower >= 4 && (HasDoor("D08Z01S01[W]") || HasDoor("D08Z01S01[E]"));
        private bool canBeatMothersBoss => bossPower >= 4 && (HasDoor("D04Z02S22[W]") || HasDoor("D04Z02S22[E]"));
        private bool canBeatCanvasesBoss => bossPower >= 4 && (HasDoor("D05Z02S14[W]") || HasDoor("D05Z02S14[E]"));
        private bool canBeatPrisonBoss => bossPower >= 4 && (HasDoor("D09Z01S03[W]") || HasDoor("D09Z01S03[N]"));
        private bool canBeatRooftopsBoss => bossPower >= 10 && (HasDoor("D06Z01S25[W]") || HasDoor("D06Z01S25[E]"));
        private bool canBeatOssuaryBoss => bossPower >= 15 && HasDoor("D01BZ08S01[W]");
        private bool canBeatMourningBoss => bossPower >= 9 && HasDoor("D20Z02S08[E]");
        private bool canBeatGraveyardBoss => bossPower >= 12 && HasDoor("D01BZ07S01[Santos]") && HasDoor("D02Z03S23[E]") && HasDoor("D02Z02S14[W]");
        private bool canBeatJondoBoss => bossPower >= 12 && HasDoor("D01BZ07S01[Santos]") && (HasDoor("D20Z01S05[W]") || HasDoor("D20Z01S05[E]")) && (HasDoor("D03Z01S03[W]") || HasDoor("D03Z01S03[SW]"));
        private bool canBeatPatioBoss => bossPower >= 12 && HasDoor("D01BZ07S01[Santos]") && HasDoor("D06Z01S18[E]") && (HasDoor("D04Z01S04[W]") || HasDoor("D04Z01S04[E]") || HasDoor("D04Z01S04[Cherubs]"));
        private bool canBeatWallBoss => bossPower >= 12 && HasDoor("D01BZ07S01[Santos]") && HasDoor("D09BZ01S01[Cell24]") && (HasDoor("D09Z01S01[W]") || HasDoor("D09Z01S01[E]"));
        private bool canBeatHallBoss => bossPower >= 12 && (HasDoor("D08Z03S03[W]") || HasDoor("D08Z03S03[E]"));

        private int guiltRooms
        {
            get
            {
                int rooms = 0;
                if (HasDoor("D01Z04S17[W]")) rooms++;
                if (HasDoor("D02Z02S06[E]")) rooms++;
                if (HasDoor("D03Z03S14[W]")) rooms++;
                if (HasDoor("D04Z02S17[W]")) rooms++;
                if (HasDoor("D05Z01S17[W]")) rooms++;
                if (HasDoor("D09Z01S13[E]")) rooms++;
                if (HasDoor("D17Z01S12[E]")) rooms++;
                return rooms;
            }
        }

        private int swordRooms
        {
            get
            {
                int rooms = 0;
                if (HasDoor("D01Z02S06[W]") || HasDoor("D01Z02S06[E]")) rooms++;
                if (HasDoor("D01Z05S24[W]") || HasDoor("D01Z05S24[E]")) rooms++;
                if (HasDoor("D02Z03S13[W]")) rooms++;
                if (HasDoor("D04Z02S12[W]")) rooms++;
                if (HasDoor("D05Z01S13[E]")) rooms++;
                if (HasDoor("D06Z01S11[W]")) rooms++;
                if (HasDoor("D17Z01S08[E]")) rooms++;
                return rooms;
            }
        }

        private int redentoRooms
        {
            get
            {
                if (HasDoor("D03Z01S03[W]") || HasDoor("D03Z01S03[SW]"))
                {
                    if (HasDoor("D17Z01S04[N]") || HasDoor("D17Z01S04[FrontR]"))
                    {
                        if (HasDoor("D01Z03S06[W]") || HasDoor("D01Z03S06[E]"))
                        {
                            if (HasDoor("D04Z01S04[W]") || HasDoor("D04Z01S04[E]") || HasDoor("D04Z01S04[Cherubs]"))
                            {
                                if (HasDoor("D04Z02S20[W]") || HasDoor("D04Z02S20[Redento]"))
                                {
                                    return 5;
                                }
                                return 4;
                            }
                            return 3;
                        }
                        return 2;
                    }
                    return 1;
                }
                return 0;
            }
        }

        private int miriamRooms
        {
            get
            {
                int rooms = 0;
                if (HasDoor("D02Z03S24[E]")) rooms++;
                if (HasDoor("D03Z03S19[E]")) rooms++;
                if (HasDoor("D04Z04S02[W]")) rooms++;
                if (HasDoor("D05Z01S24[E]")) rooms++;
                if (HasDoor("D06Z01S26[W]")) rooms++;
                return rooms;
            }
        }

        private int amanecidaRooms
        {
            get
            {
                int rooms = 0;
                if (canBeatGraveyardBoss) rooms++;
                if (canBeatJondoBoss) rooms++;
                if (canBeatPatioBoss) rooms++;
                if (canBeatWallBoss) rooms++;
                return rooms;
            }
        }

        // Doors
        private Dictionary<string, bool> doors = new Dictionary<string, bool>();

        public void SetConfigSettings(Config config)
        {
            logicDifficulty = config.LogicDifficulty;
            enemyShuffle = config.EnemyShuffleType > 0;
        }

        public bool HasDoor(string doorId)
        {
            return doors.ContainsKey(doorId);// && doors[doorId];
        }

        protected override Variable GetVariable(string variable)
        {
            if (variable[0] == 'D')
            {
                return new BoolVariable(HasDoor(variable));
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

                case "dash": return new BoolVariable(dash);
                case "wallClimb": return new BoolVariable(wallClimb);
                case "boots": return new BoolVariable(boots);
                case "doubleJump": return new BoolVariable(doubleJump);

                case "cherubs": return new IntVariable(cherubs);
                case "bones": return new IntVariable(bones);
                case "tears": return new IntVariable(tears);

                case "taranto": return new BoolVariable(taranto);
                case "tiento": return new BoolVariable(tiento);
                case "tirana": return new BoolVariable(tirana);
                case "aubade": return new BoolVariable(aubade);
                case "cherubBitfield": return new IntVariable(cherubBitfield);

                //case "combo": return new BoolVariable(combo);
                //case "charged": return new BoolVariable(charged);
                //case "ranged": return new BoolVariable(ranged);
                //case "dive": return new BoolVariable(dive);
                //case "lunge": return new BoolVariable(lunge);

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
                case "canDawnJump": return new BoolVariable(canDawnJump);
                case "canWaterJump": return new BoolVariable(canWaterJump);
                case "canAirStall": return new BoolVariable(canAirStall);
                case "canDiveLaser": return new BoolVariable(canDiveLaser);
                case "canSurvivePoison": return new BoolVariable(canSurvivePoison);

                case "canBreakJondoBell": return new BoolVariable(canBreakJondoBell); // access to both jondo bell rooms
                case "canRideAlberoElevator": return new BoolVariable(canRideAlberoElevator); // access to graveyard elevator room
                case "canFillChalice": return new BoolVariable(canFillChalice); // chalice && access to the 3 enemy rooms

                case "guiltRooms": return new IntVariable(guiltRooms);
                case "swordRooms": return new IntVariable(swordRooms);
                case "redentoRooms": return new IntVariable(redentoRooms);
                case "miriamRooms": return new IntVariable(miriamRooms);
                case "amanecidaRooms": return new IntVariable(amanecidaRooms);

                // Crossing gaps
                case "canCrossGap1": return new BoolVariable(canCrossGap1);
                case "canCrossGap2": return new BoolVariable(canCrossGap2);
                case "canCrossGap3": return new BoolVariable(canCrossGap3);
                case "canCrossGap4": return new BoolVariable(canCrossGap4);
                case "canCrossGap5": return new BoolVariable(canCrossGap5);
                case "canCrossGap6": return new BoolVariable(canCrossGap6);
                case "canCrossGap7": return new BoolVariable(canCrossGap7);
                case "canCrossGap8": return new BoolVariable(canCrossGap8);
                case "canCrossGap9": return new BoolVariable(canCrossGap9);
                case "canCrossGap10": return new BoolVariable(canCrossGap10);
                case "canCrossGap11": return new BoolVariable(canCrossGap11);

                // Special skips
                case "upwarpSkipsAllowed": return new BoolVariable(upwarpSkipsAllowed);
                case "mourningSkipAllowed": return new BoolVariable(mourningSkipAllowed);
                case "enemySkipsAllowed": return new BoolVariable(enemySkipsAllowed);

                // Bosses
                case "canBeatBrotherhoodBoss": return new BoolVariable(canBeatBrotherhoodBoss);
                case "canBeatMercyBoss": return new BoolVariable(canBeatMercyBoss);
                case "canBeatConventBoss": return new BoolVariable(canBeatConventBoss);
                case "canBeatGrievanceBoss": return new BoolVariable(canBeatGrievanceBoss);
                case "canBeatBridgeBoss": return new BoolVariable(canBeatBridgeBoss);
                case "canBeatMothersBoss": return new BoolVariable(canBeatMothersBoss);
                case "canBeatCanvasesBoss": return new BoolVariable(canBeatCanvasesBoss);
                case "canBeatPrisonBoss": return new BoolVariable(canBeatPrisonBoss);
                case "canBeatRooftopsBoss": return new BoolVariable(canBeatRooftopsBoss);
                case "canBeatOssuaryBoss": return new BoolVariable(canBeatOssuaryBoss);
                case "canBeatMourningBoss": return new BoolVariable(canBeatMourningBoss);
                case "canBeatGraveyardBoss": return new BoolVariable(canBeatGraveyardBoss);
                case "canBeatJondoBoss": return new BoolVariable(canBeatJondoBoss);
                case "canBeatPatioBoss": return new BoolVariable(canBeatPatioBoss);
                case "canBeatWallBoss": return new BoolVariable(canBeatWallBoss);
                case "canBeatHallBoss": return new BoolVariable(canBeatHallBoss);

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
                        else if (item.id == "PR09") { cherubBitfield |= 0x40; taranto = true; }
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
                        if (item.id == "COMBO") combo++;
                        else if (item.id == "CHARGED") charged++;
                        else if (item.id == "RANGED") { ranged++; cherubBitfield |= 0x40000; }
                        else if (item.id == "DIVE") dive++;
                        else if (item.id == "LUNGE") lunge++;
                        break;
                    }
                case 12:
                    {
                        if (item.id == "Slide") dash = true;
                        else if (item.id == "WallClimb") wallClimb = true;
                        break;
                    }
            }

            // Recalculate cherub bitfield based on fervour
            if (fervourLevel >= 2)
            {
                if (ownAubade)
                {
                    cherubBitfield |= 0x2000;
                    aubade = true;
                }
                if (ownTirana)
                {
                    cherubBitfield |= 0x10000;
                    tirana = true;
                }
            }
        }

        public void RemoveItem(string itemId)
        {
            if (itemId[0] == 'D')
            {
                if (doors.ContainsKey(itemId))
                    doors.Remove(itemId);
                return;
            }
            // Right now this is only used for removing doors during the door rando process
        }
    }
}
