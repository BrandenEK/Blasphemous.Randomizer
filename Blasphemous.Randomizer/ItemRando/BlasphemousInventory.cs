﻿using Basalt.LogicParser;
using Blasphemous.Randomizer.ItemRando.LogicResolvers;
using System.Collections.Generic;

namespace Blasphemous.Randomizer.ItemRando
{
    public class BlasphemousInventory : InventoryData
    {
        protected ILogicResolver _logicResolver;

        public BlasphemousInventory()
        {
            _logicResolver = new RuntimeLogicResolver();
        }

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
        private bool airImpulse = true;
        private bool boots = false;
        private bool doubleJump = false;

        // Speed boosts
        private bool wheel = false;
        private bool dawnHeart = false;

        // Health boosts
        private int ownedFlasks = 0;
        private int flasks => (HasDoor("D01Z05S18[E]") || HasDoor("D02Z02S09[E]") || HasDoor("D03Z02S14[E]") || HasDoor("D03Z03S03[SE]") || HasDoor("D04Z02S13[W]") || HasDoor("D05Z01S12[E]") || HasDoor("D20Z01S08[W]")) ? ownedFlasks : 0;
        private int ownedQuicksilver = 0;
        private int quicksilver => HasDoor("D01Z05S27[E]") ? ownedQuicksilver : 0;

        // Puzzles
        private int redWax = 0;
        private int blueWax = 0;
        private bool chalice = false;

        // Cherubs
        private bool debla = false;
        private bool lorquiana = false;
        private bool zarabanda = false;
        private bool taranto = false;
        private bool verdiales = false;
        private bool cante = false;
        private bool cantina = false;

        private bool ownAubade = false;
        private bool aubade => ownAubade && TotalFervour >= 90;
        private bool ownTirana = false;
        private bool tirana => ownTirana && TotalFervour >= 90;

        private bool ruby = false;
        private bool tiento = false;
        private bool anySmallPrayer = false;
        private bool pillar => debla || taranto || ruby;
        private bool canUseAnyPrayer => anySmallPrayer || tirana || aubade;

        // Stats
        private int healthLevel = 0, fervourLevel = 0, swordLevel = 0;
        private int TotalFervour => 60 + (20 * fervourLevel) + (10 * blueWax);

        // Skills
        private int combo = 0;
        private int charged = 0;
        private int ranged = 0;
        private int dive = 0;
        private int lunge = 0;
        private bool chargeBeam => charged >= 3;

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
        private int ownedKnots = 0;
        private int knots => HasDoor("D17Z01S09[E]") ? ownedKnots : 0;

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

        // Movement tech
        private bool canAirStall => ranged > 0 && logicDifficulty >= 1;
        private bool canDawnJump => dawnHeart && dash && logicDifficulty >= 1;
        private bool canWaterJump => nail || doubleJump;

        // Breakable tech
        private bool canBreakHoles => charged > 0 || dive > 0 || lunge >= 3 && dash || canUseAnyPrayer;
        private bool canDiveLaser => dive >= 3 && logicDifficulty >= 2;

        // Root tech
        private bool canWalkOnRoot => root;
        private bool canClimbOnRoot => root && wallClimb;

        // Lung tech
        private bool canSurvivePoison1 => lung || (logicDifficulty >= 1 && tiento) || logicDifficulty >= 2;
        private bool canSurvivePoison2 => lung || (logicDifficulty >= 1 && tiento);
        private bool canSurvivePoison3 => lung || (logicDifficulty >= 2 && tiento && TotalFervour >= 120);

        // Enemy tech
        private bool canEnemyBounce => airImpulse && enemySkipsAllowed;
        private bool canEnemyUpslash => combo >= 2 && enemySkipsAllowed;

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

        // Events that trigger in different scenes
        private bool openedDCGateW => HasDoor("D01Z05S24[W]") || HasDoor("D01Z05S24[E]");
        private bool openedDCGateE => HasDoor("D01Z05S12[W]") || HasDoor("D01Z05S12[E]");
        private bool openedDCLadder => HasDoor("D01Z05S20[W]") || HasDoor("D01Z05S20[N]");
        private bool openedWOTWCave => HasDoor("D02Z01S06[E]") || wallClimb && (HasDoor("D02Z01S06[W]") || HasDoor("D02Z01S06[Cherubs]"));
        private bool rodeGotPElevator => HasDoor("D02Z02S11[NW]") || HasDoor("D02Z02S11[NE]") || HasDoor("D02Z02S11[W]") || HasDoor("D02Z02S11[E]") || HasDoor("D02Z02S11[SE]");
        private bool openedConventLadder => HasDoor("D02Z03S11[S]") || HasDoor("D02Z03S11[W]") || HasDoor("D02Z03S11[NW]") || HasDoor("D02Z03S11[E]") || HasDoor("D02Z03S11[NE]");
        private bool brokeJondoBellW => HasDoor("D03Z02S09[S]") || HasDoor("D03Z02S09[W]") && dash || HasDoor("D03Z02S09[N]") || HasDoor("D03Z02S09[Cherubs]");
        private bool brokeJondoBellE => HasDoor("D03Z02S05[S]") || HasDoor("D03Z02S05[E]") || HasDoor("D03Z02S05[W]") && (canCrossGap5 || canEnemyBounce && canCrossGap3);
        private bool openedMoMLadder => HasDoor("D04Z02S06[NW]") || HasDoor("D04Z02S06[NE]") || HasDoor("D04Z02S06[N]") || HasDoor("D04Z02S06[S]") || HasDoor("D04Z02S06[SE]");
        private bool openedTSCGate => HasDoor("D05Z02S11[W]") || HasDoor("D05Z02S11[Cherubs]");
        private bool openedARLadder => HasDoor("D06Z01S23[Sword]") || HasDoor("D06Z01S23[E]") || HasDoor("D06Z01S23[S]") || HasDoor("D06Z01S23[Cherubs]");
        private bool brokeBotTCStatue => HasDoor("D08Z01S02[NE]") || HasDoor("D08Z01S02[SE]");
        private bool openedWotHPGate => HasDoor("D09Z01S05[W]") || HasDoor("D09Z01S05[SE]") || HasDoor("D09Z01S05[NE]");
        private bool openedBotSSLadder => HasDoor("D17Z01S04[N]") || HasDoor("D17Z01S04[FrontR]");

        // Special skips
        public bool upwarpSkipsAllowed => logicDifficulty >= 2;
        public bool mourningSkipAllowed => logicDifficulty >= 2;
        public bool enemySkipsAllowed => logicDifficulty >= 2 && !enemyShuffle;
        public bool obscureSkipsAllowed => logicDifficulty >= 2;
        public bool preciseSkipsAllowed => logicDifficulty >= 2;

        // Bosses
        private bool canBeatBrotherhoodBoss => HasBossStrength("warden") && (HasDoor("D17Z01S11[W]") || HasDoor("D17Z01S11[E]")); // These need to be changed for boss shuffle
        private bool canBeatMercyBoss => HasBossStrength("ten-piedad") && (HasDoor("D01Z04S18[W]") || HasDoor("D01Z04S18[E]"));
        private bool canBeatConventBoss => HasBossStrength("charred-visage") && (HasDoor("D02Z03S20[W]") || HasDoor("D02Z03S20[E]"));
        private bool canBeatGrievanceBoss => HasBossStrength("tres-angustias") && (wallClimb || doubleJump) && (HasDoor("D03Z03S15[W]") || HasDoor("D03Z03S15[E]"));
        private bool canBeatBridgeBoss => HasBossStrength("esdras") && (HasDoor("D08Z01S01[W]") || HasDoor("D08Z01S01[E]"));
        private bool canBeatMothersBoss => HasBossStrength("melquiades") && (HasDoor("D04Z02S22[W]") || HasDoor("D04Z02S22[E]"));
        private bool canBeatCanvasesBoss => HasBossStrength("exposito") && (HasDoor("D05Z02S14[W]") || HasDoor("D05Z02S14[E]"));
        private bool canBeatPrisonBoss => HasBossStrength("quirce") && (HasDoor("D09Z01S03[W]") || HasDoor("D09Z01S03[N]"));
        private bool canBeatRooftopsBoss => HasBossStrength("crisanta") && (HasDoor("D06Z01S25[W]") || HasDoor("D06Z01S25[E]"));
        private bool canBeatOssuaryBoss => HasBossStrength("isidora") && HasDoor("D01BZ08S01[W]");
        private bool canBeatMourningBoss => HasBossStrength("sierpes") && HasDoor("D20Z02S08[E]");
        private bool canBeatGraveyardBoss => HasBossStrength("amanecida") && wallClimb && HasDoor("D01BZ07S01[Santos]") && HasDoor("D02Z03S23[E]") && HasDoor("D02Z02S14[W]");
        private bool canBeatJondoBoss => HasBossStrength("amanecida") && HasDoor("D01BZ07S01[Santos]") && (HasDoor("D20Z01S05[W]") || HasDoor("D20Z01S05[E]")) && (HasDoor("D03Z01S03[W]") || HasDoor("D03Z01S03[SW]"));
        private bool canBeatPatioBoss => HasBossStrength("amanecida") && HasDoor("D01BZ07S01[Santos]") && HasDoor("D06Z01S18[E]") && (HasDoor("D04Z01S04[W]") || HasDoor("D04Z01S04[E]") || HasDoor("D04Z01S04[Cherubs]"));
        private bool canBeatWallBoss => HasBossStrength("amanecida") && HasDoor("D01BZ07S01[Santos]") && HasDoor("D09BZ01S01[Cell24]") && (HasDoor("D09Z01S01[W]") || HasDoor("D09Z01S01[E]"));
        private bool canBeatHallBoss => HasBossStrength("laudes") && (HasDoor("D08Z03S03[W]") || HasDoor("D08Z03S03[E]"));
        private bool canBeatPerpetua => HasBossStrength("perpetua");
        private bool canBeatLegionary => HasBossStrength("legionary");

        private bool HasBossStrength(string boss)
        {
            float playerStrength = healthLevel * 0.25f / 6 + swordLevel * 0.25f / 7 + fervourLevel * 0.20f / 6 + flasks * 0.15f / 8 + quicksilver * 0.15f / 5;
            float bossStrength = boss switch
            {
                "warden" => -0.10f,
                "ten-piedad" => 0.05f,
                "charred-visage" => 0.20f,
                "tres-angustias" => 0.15f,
                "esdras" => 0.25f,
                "melquiades" => 0.25f,
                "exposito" => 0.30f,
                "quirce" => 0.35f,
                "crisanta" => 0.50f,
                "isidora" => 0.70f,
                "sierpes" => 0.70f,
                "amanecida" => 0.60f,
                "laudes" => 0.60f,
                "perpetua" => -0.05f,
                "legionary" => 0.20f,
                _ => throw new LogicParserException($"Boss {boss} does not exist!"),
            };
            return playerStrength >= (logicDifficulty >= 2 ? bossStrength - 0.10f : (logicDifficulty >= 1 ? bossStrength : bossStrength + 0.10f));
        }

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
                                if (knots >= 1 && limestones >= 3 && (HasDoor("D04Z02S20[W]") || HasDoor("D04Z02S20[Redento]")))
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

        private int chaliceRooms
        {
            get
            {
                int rooms = 0;
                if (HasDoor("D03Z01S01[W]") || HasDoor("D03Z01S01[NE]") || HasDoor("D03Z01S01[S]")) rooms++;
                if (HasDoor("D05Z02S01[W]") || HasDoor("D05Z02S01[E]")) rooms++;
                if (HasDoor("D09Z01S07[SW]") || HasDoor("D09Z01S07[SE]") || HasDoor("D09Z01S07[W]") || HasDoor("D09Z01S07[E]")) rooms++;
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

        protected override object GetVariable(string variable)
        {
            if (_logicResolver.IsDoor(variable))
            {
                return HasDoor(variable);
            }

            return variable switch
            {
                // Relics
                "blood" => blood,
                "root" => throw new LogicParserException("Don't check for root directly - Use canWalkOnRoot or canClimbOnRoot instead!"),
                "linen" => linen,
                "nail" => nail,
                "shroud" => shroud,
                "lung" => throw new LogicParserException("Don't check for lung directly - Use canSurvivePoisonX instead!"),

                // Keys
                "bronzeKey" => bronzeKey,
                "silverKey" => silverKey,
                "goldKey" => goldKey,
                "peaksKey" => peaksKey,
                "elderKey" => elderKey,
                "woodKey" => woodKey,

                // Collections
                "cherubs" => cherubs,
                "bones" => bones,
                "tears" => tears,

                // Special items
                "dash" => dash,
                "wallClimb" => wallClimb,
                "airImpulse" => throw new LogicParserException("Air impulse is not randomized yet!"),
                "boots" => boots,
                "doubleJump" => doubleJump,

                // Speed boosts
                "wheel" => wheel,
                "dawnHeart" => throw new LogicParserException("Don't check for dawnHeart directly - Use canDawnJump instead!"),

                // Health boosts
                "flasks" => throw new LogicParserException("Don't check for flasks directly - This is only used in power calculation!"),
                "quicksilver" => throw new LogicParserException("Don't check for flasks directly - This is only used in power calculation!"),

                // Puzzles
                "redWax" => redWax,
                "blueWax" => blueWax,
                "chalice" => chalice,

                // Cherubs
                "debla" => debla,
                "lorquiana" => lorquiana,
                "zarabanda" => zarabanda,
                "taranto" => taranto,
                "verdiales" => verdiales,
                "cante" => cante,
                "cantina" => cantina,

                "aubade" => aubade,
                "tirana" => tirana,

                "ruby" => ruby,
                "tiento" => tiento,
                "anyPrayer" => throw new LogicParserException("Don't check for anyPrayer directly - This is only used for canBreakHoles!"),
                "pillar" => pillar,

                // Stats
                "healthLevel" => throw new LogicParserException("Don't check for healthLevel directly - This is only used in power calculation!"),
                "fervourLevel" => throw new LogicParserException("Don't check for fervourLevel directly - This is only used in power calculation!"),
                "swordLevel" => throw new LogicParserException("Don't check for swordLevel directly - This is only used in power calculation!"),

                // Skills
                "combo" => throw new LogicParserException("Don't check for combo directly - These are stored as ints!"),
                "charged" => throw new LogicParserException("Don't check for charged directly - These are stored as ints!"),
                "ranged" => throw new LogicParserException("Don't check for ranged directly - These are stored as ints!"),
                "dive" => throw new LogicParserException("Don't check for dive directly - These are stored as ints!"),
                "lunge" => throw new LogicParserException("Don't check for lunge directly - These are stored as ints!"),
                "chargeBeam" => chargeBeam,
                "rangedAttack" => ranged > 0,

                // Main quest
                "holyWounds" => holyWounds,
                "masks" => masks,
                "guiltBead" => guiltBead,

                // LOTL quest
                "cloth" => cloth,
                "hand" => hand,
                "hatchedEgg" => hatchedEgg,

                // Tirso quest
                "herbs" => herbs,

                // Tentudia quest
                "tentudiaRemains" => tentudiaRemains,

                // Gemino quest
                "emptyThimble" => emptyThimble,
                "fullThimble" => fullThimble,
                "driedFlowers" => driedFlowers,

                // Altasgracias quest
                "ceremonyItems" => ceremonyItems,
                "egg" => egg,

                // Redento quest
                "limestones" => limestones,
                "knots" => knots,

                // Cleofas quest
                "marksOfRefuge" => marksOfRefuge,
                "cord" => cord,

                // Crisanta quest
                "scapular" => scapular,
                "trueHeart" => trueHeart,
                "traitorEyes" => traitorEyes,

                // Jibrael quest
                "bell" => bell,
                "verses" => verses,

                // Movement tech
                "canAirStall" => canAirStall,
                "canDawnJump" => canDawnJump,
                "canWaterJump" => canWaterJump,

                // Breakable tech
                "canBreakHoles" => canBreakHoles,
                "canDiveLaser" => canDiveLaser,

                // Root tech
                "canWalkOnRoot" => canWalkOnRoot,
                "canClimbOnRoot" => canClimbOnRoot,

                // Lung tech
                "canSurvivePoison1" => canSurvivePoison1,
                "canSurvivePoison2" => canSurvivePoison2,
                "canSurvivePoison3" => canSurvivePoison3,

                // Enemy tech
                "canEnemyBounce" => canEnemyBounce,
                "canEnemyUpslash" => canEnemyUpslash,

                // Reaching rooms
                "guiltRooms" => guiltRooms,
                "swordRooms" => swordRooms,
                "redentoRooms" => redentoRooms,
                "miriamRooms" => miriamRooms,
                "amanecidaRooms" => amanecidaRooms,
                "chaliceRooms" => chaliceRooms,

                // Crossing gaps
                "canCrossGap1" => canCrossGap1,
                "canCrossGap2" => canCrossGap2,
                "canCrossGap3" => canCrossGap3,
                "canCrossGap4" => canCrossGap4,
                "canCrossGap5" => canCrossGap5,
                "canCrossGap6" => canCrossGap6,
                "canCrossGap7" => canCrossGap7,
                "canCrossGap8" => canCrossGap8,
                "canCrossGap9" => canCrossGap9,
                "canCrossGap10" => canCrossGap10,
                "canCrossGap11" => canCrossGap11,

                // Events in different scenes
                "openedDCGateW" => openedDCGateW,
                "openedDCGateE" => openedDCGateE,
                "openedDCLadder" => openedDCLadder,
                "openedWOTWCave" => openedWOTWCave,
                "rodeGotPElevator" => rodeGotPElevator,
                "openedConventLadder" => openedConventLadder,
                "brokeJondoBellW" => brokeJondoBellW,
                "brokeJondoBellE" => brokeJondoBellE,
                "openedMoMLadder" => openedMoMLadder,
                "openedTSCGate" => openedTSCGate,
                "openedARLadder" => openedARLadder,
                "brokeBotTCStatue" => brokeBotTCStatue,
                "openedWotHPGate" => openedWotHPGate,
                "openedBotSSLadder" => openedBotSSLadder,

                // Special skips
                "upwarpSkipsAllowed" => upwarpSkipsAllowed,
                "mourningSkipAllowed" => mourningSkipAllowed,
                "enemySkipsAllowed" => enemySkipsAllowed,
                "obscureSkipsAllowed" => obscureSkipsAllowed,
                "preciseSkipsAllowed" => preciseSkipsAllowed,

                // Bosses
                "canBeatBrotherhoodBoss" => canBeatBrotherhoodBoss,
                "canBeatMercyBoss" => canBeatMercyBoss,
                "canBeatConventBoss" => canBeatConventBoss,
                "canBeatGrievanceBoss" => canBeatGrievanceBoss,
                "canBeatBridgeBoss" => canBeatBridgeBoss,
                "canBeatMothersBoss" => canBeatMothersBoss,
                "canBeatCanvasesBoss" => canBeatCanvasesBoss,
                "canBeatPrisonBoss" => canBeatPrisonBoss,
                "canBeatRooftopsBoss" => canBeatRooftopsBoss,
                "canBeatOssuaryBoss" => canBeatOssuaryBoss,
                "canBeatMourningBoss" => canBeatMourningBoss,
                "canBeatGraveyardBoss" => canBeatGraveyardBoss,
                "canBeatJondoBoss" => canBeatJondoBoss,
                "canBeatPatioBoss" => canBeatPatioBoss,
                "canBeatWallBoss" => canBeatWallBoss,
                "canBeatHallBoss" => canBeatHallBoss,
                "canBeatPerpetua" => canBeatPerpetua,
                "canBeatLegionary" => canBeatLegionary,

                _ => throw new LogicParserException($"Error: Variable {variable} doesn't exist!"),
            };
        }

        public void AddItem(string itemId)
        {
            if (_logicResolver.IsDoor(itemId))
            {
                if (!doors.ContainsKey(itemId))
                    doors.Add(itemId, true);
                return;
            }

            if (!_logicResolver.IsItem(itemId))
                throw new System.Exception($"Attempting to add unknown door/item: {itemId}");

            Item item = _logicResolver.GetItem(itemId);
            switch (item.type)
            {
                case 0:
                    {
                        if (item.id == "RB20" || item.id == "RB21" || item.id == "RB22") limestones++;
                        else if (item.id == "RB41") guiltBead = true;
                        else if (item.id == "RB105") ruby = true;
                        else if (item.id == "RB203") wheel = true;
                        else if (item.id == "RW") redWax++;
                        else if (item.id == "BW") blueWax++;
                        break;
                    }
                case 1:
                    {
                        if (item.id != "PR101" && item.id != "PR203")
                            anySmallPrayer = true;

                        if (item.id == "PR03") debla = true;
                        else if (item.id == "PR07") lorquiana = true;
                        else if (item.id == "PR08") zarabanda = true;
                        else if (item.id == "PR09") taranto = true;
                        else if (item.id == "PR11") tiento = true;
                        else if (item.id == "PR12") cante = true;
                        else if (item.id == "PR14") verdiales = true;
                        else if (item.id == "PR101") ownAubade = true;
                        else if (item.id == "PR201") cantina = true;
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
                        else if (item.id == "BV") ownedFlasks++;
                        else if (item.id == "RK") ownedKnots++;
                        else if (item.id == "QS") ownedQuicksilver++;
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
                        else if (item.id == "RANGED") ranged++;
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
        }

        public void RemoveItem(string itemId)
        {
            if (_logicResolver.IsDoor(itemId))
            {
                if (doors.ContainsKey(itemId))
                    doors.Remove(itemId);
                return;
            }
            // Right now this is only used for removing doors during the door rando process
        }
    }
}
