using System;
using System.Collections.Generic;

namespace Framework.Randomizer
{
	public class Spoiler
	{
		private string getTemplate()
		{
			return "Albero (35)\r\n\r\nSick house: RB01\r\nOutside ossuary: CO43\r\nGraveyard: CO16\r\nWarp room: QI65\r\nElevator cherub: RESCUED_CHERUB_08\r\nBless cloth: RE04\r\nBless egg: RE10\r\nBless hand: RE02\r\nCleofas gift initial: QI01\r\nCleofas gift final: PR11\r\nTirso reward 1: QI66\r\nTirso reward 2: 500.Tirso\r\nTirso reward 3: 1000.Tirso\r\nTirso reward 4: 2000.Tirso\r\nTirso reward 5: 5000.Tirso\r\nTirso reward 6: 10000.Tirso\r\nTirso reward final: QI56\r\nTentudia reward 1: 500.Lvdovico\r\nTentudia reward 2: 1000.Lvdovico\r\nTentudia reward 3: PR03\r\nIsidora reward: QI201\r\nSword room: 741515178\r\nChurch donation 1: RB104\r\nChurch donation 2: RB105\r\nOssuary reward 1: 250.Undertaker\r\nOssuary reward 2: 500.Undertaker\r\nOssuary reward 3: 750.Undertaker\r\nOssuary reward 4: 1000.Undertaker\r\nOssuary reward 5: 1250.Undertaker\r\nOssuary reward 6: 1500.Undertaker\r\nOssuary reward 7: 1750.Undertaker\r\nOssuary reward 8: 2000.Undertaker\r\nOssuary reward 9: 2500.Undertaker\r\nOssuary reward 10: 3000.Undertaker\r\nOssuary reward 11: 5000.Undertaker\r\n\r\nAll the Tears of the Sea (1)\r\n\r\nMiriam gift: PR201\r\n\r\nArchcathedral Rooftops (11)\r\n\r\nBridge fight 1: QI02\r\nBridge fight 2: QI03\r\nBridge fight 3: QI04\r\nWestern shaft ledge: CO06\r\nWestern shaft cherub: RESCUED_CHERUB_36\r\nWestern shaft chest: PR12\r\nMoM east entrance: HE04\r\nLady room: 1213038592\r\nSecond checkpoint ledge: CO40\r\nSword room: 1330243584\r\nCrisanta: BS16\r\n\r\nBridge of the Three Calvaries (2)\r\n\r\nEsdras: BS12\r\nEsdras gift intial: PR09\r\n\r\nBrotherhood of the Silent Sorrow (11)\r\n\r\nBeginning gift: QI106\r\nInitial room cherub: RESCUED_CHERUB_06\r\nInitial room ledge: RB204\r\nElder Brother's room: RE01\r\nSword room: -865337344\r\nSpike gaunlet exit: CO25\r\nBlue candle: RB25\r\nChurch entrance: PR203\r\nEsdras gift final: QI204\r\nCrisanta gift: QI301\r\nWarden of the Silent Sorrow: BS13\r\n\r\nConvent of our Lady of the Charred Visage (13)\r\n\r\nBlood platform ledge: CO05\r\nGhost death room: CO15\r\nCentral lung room: RB08\r\nSouthwest lung room: HE03\r\nLady room: -840761344\r\nSword room: -835715072\r\nRed candle: RB18\r\nBlue candle: RB24\r\nOutside area: RB107\r\nBurning oil fountain: QI57\r\nOur Lady of the Charred Visage: BS03\r\nHoly Visage gift: QI40\r\nMask room: QI61\r\n\r\nDeabulatory of his Holiness (3)\r\n\r\nComplete Penitence 1: RB101\r\nComplete Penitence 2: RB102\r\nComplete Penitence 3: RB103\r\n\r\nDesecrated Cistern (20)\r\n\r\nMeD lady room: -933363712\r\nMeD entrance: CO41\r\nWater room cherub: RESCUED_CHERUB_11\r\nEastern lower tunnel chest: QI45\r\nEastern upper tunnel chest: PR16\r\nEastern upper tunnel cherub: RESCUED_CHERUB_13\r\nHidden hand room: QI67\r\nWaBC entrance: CO09\r\nOil room: -886603776\r\nVeil room cherub: RESCUED_CHERUB_14\r\nVeil room ledge: QI12\r\nLung tunnel cherub: RESCUED_CHERUB_12\r\nLung tunnel ledge: CO32\r\nShroud puzzle: RB03\r\nChalice room: QI75\r\nSword room: -932151296\r\nGrA lady room: -831062016\r\nElevator exit cherub: RESCUED_CHERUB_15\r\nElevator shaft cherub: RESCUED_CHERUB_22\r\nElevator shaft ledge: CO44\r\n\r\nEchoes of Salt (2)\r\n\r\nMoED entrance: RB108\r\nNear elevator shaft: RB202\r\n\r\nGraveyard of the Peaks (20)\r\n\r\nShop cave cherub: RESCUED_CHERUB_31\r\nShop cave hole: CO42\r\nShop left: QI11\r\nShop middle: RB37\r\nShop right: RB02\r\nGuilt room: RB38\r\nElevator shaft cherub: RESCUED_CHERUB_26\r\nElevator shaft ledge: QI53\r\nLady room: -927137792\r\nBleed room: HE11\r\nEastern shaft lower: QI46\r\nEastern shaft middle: CO29\r\nEastern shaft upper: QI08\r\nAmanecida ledge: RB106\r\nWestern shaft cherub: RESCUED_CHERUB_25\r\nWestern shaft lower: RB32\r\nWestern shaft upper: CO01\r\nCenter shaft cherub: RESCUED_CHERUB_24\r\nCenter shaft ledge: RB15\r\nOil room: -815923200\r\n\r\nGrievance Ascends (12)\r\n\r\nWestern lung ledge: QI44\r\nLung room upper: RE07\r\nLung room cherub: RESCUED_CHERUB_19\r\nLung room lower: CO12\r\nOil room: -910753792\r\nBlood tunnel ledge: QI10\r\nBlood tunnel cherub: RESCUED_CHERUB_21\r\nAltasgracias cherub: RESCUED_CHERUB_20\r\nAltasgracias gift: QI13\r\nAltasgracias cacoon: RB06\r\nTres Angustias: BS04\r\nHoly Visage gift: QI39\r\n\r\nHall of the Dawning (1)\r\n\r\nMirror room: QI105\r\n\r\nJondo (13)\r\n\r\nEastern entrance ledge: CO08\r\nEastern entrance chest: PR10\r\nEastern shaft bell chargers: CO33\r\nEastern shaft bell trap: QI19\r\nEastern shaft cherub: RESCUED_CHERUB_18\r\nSpike tunnel cherub: RESCUED_CHERUB_37\r\nSpike tunnel ledge: HE06\r\nEcS entrance: QI103\r\nWestern shaft lower slide: CO07\r\nWestern shaft bell trap: QI41\r\nWestern shaft bell puzzle: QI52\r\nWestern shaft root puzzle: RB28\r\nWestern shaft cherub: RESCUED_CHERUB_17\r\n\r\nKnot of the Three Words (1)\r\n\r\nFourth Visage gift: HE201\r\n\r\nLibrary of the Negated Words (18)\r\n\r\nPlatform room cherub: RESCUED_CHERUB_01\r\nPlatform room ledge: CO18\r\nUpper cathedral ledge: CO22\r\nHidden floor: QI50\r\nLung ambush chest: RB31\r\nLady room: 1271136256\r\nBone puzzle: PR15\r\nDiosdado ledge: CO28\r\nPlatform puzzle chest: PR07\r\nFinal shaft ledge: RB30\r\nFinal shaft cherub: RESCUED_CHERUB_02\r\nOil room: 1233780736\r\nElevator cherub: RESCUED_CHERUB_32\r\nMask room: QI62\r\nSword room: 1189150720\r\nRed candle: RB19\r\nDiosdado gift: RB203\r\nFourth Visage hidden wall: RB301\r\n\r\nMercy Dreams (15)\r\n\r\nFirst section hidden wall: CO30\r\nSecond section ghost ambush: PR01\r\nSecond section ledge: CO03\r\nSecond section cherub: RESCUED_CHERUB_09\r\nRed candle: RB17\r\nShop left: QI58\r\nShop middle: RB05\r\nShop right: RB09\r\nThird section hidden wall: QI48\r\nThird section lower corridor: CO38\r\nTen Piedad: BS01\r\nHoly Visage gift: QI38\r\nBlue candle: RB26\r\nSlC entrance cherub: RESCUED_CHERUB_33\r\nSlC entrance ledge: CO03\r\n\r\nMother of Mothers (14)\r\n\r\nOil room: 1203666944\r\nEastern room upper: RB33\r\nEastern room lower: CO35\r\nWestern room cherub: RESCUED_CHERUB_30\r\nWestern room ledge: CO17\r\nRedento prayer room: RE03\r\nRedento corpse: QI54\r\nBlood incense shaft: HE01\r\nOutside Cleofas room: CO34\r\nCenter room ledge: CO20\r\nCenter room cherub: RESCUED_CHERUB_29\r\nSword room: 1240137728\r\nMelquiades: BS05\r\nMask room: QI60\r\n\r\nMountains of the Endless Dusk (7)\r\n\r\nDeC entrance: CO13\r\nPerpetua gift: RB13\r\nBell gap cherub: RESCUED_CHERUB_16\r\nBell gap ledge: QI47\r\nRedento meeting 1: RB22\r\nBlood platform: QI63\r\nEgg hatching: QI14\r\n\r\nMourning and Havoc (3)\r\n\r\nWestern chest: PR202\r\nEastern chest: RB201\r\nSierpes reward: QI202\r\n\r\nPatio of the Silent Steps (8)\r\n\r\nGarden 1 cherub: RESCUED_CHERUB_35\r\nGarden 1 ledge: CO23\r\nGarden 2 ledge: RB14\r\nGarden 3 cherub: RESCUED_CHERUB_28\r\nGarden 3 lower ledge: QI37\r\nGarden 3 upper ledge: CO39\r\nNorthern shaft: QI102\r\nRedento meeting 4: RB21\r\n\r\nPetrous (1)\r\n\r\nEntrance room: QI101\r\n\r\nResting Place of the Sister (1)\r\n\r\nPerpetua shrine: QI203\r\n\r\nSleeping Canvases (10)\r\n\r\nHerb shaft: QI64\r\nWax bleed puzzle: HE07\r\nShop left: RB12\r\nShop middle: QI49\r\nShop right: QI71\r\nLow tunnel blade trap: QI104\r\nExpositio: BS06\r\nLinen drop room: CO31\r\nJocinero gift intial: RE05\r\nJocinero gift final: PR05\r\n\r\nThe Holy Line (6)\r\n\r\nDeosgracias gift: QI31\r\nMud ledge lower: PR14\r\nMud ledge upper: RB07\r\nMud cherub: RESCUED_CHERUB_07\r\nCave ledge: CO04\r\nCave chest: QI55\r\n\r\nWall of the Holy Prohibitions (18)\r\n\r\nQ1 lift puzzle: RB11\r\nQ1 drop room upper: CO10\r\nQ1 drop room lower: QI69\r\nQ1 upper bronze door: RESCUED_CHERUB_03\r\nQ1 upper silver door: CO24\r\nQ1 middle gold door: QI51\r\nQ2 middle gold door: CO26\r\nQ3 lower gold door: CO02\r\nQ3 upper silver door: RESCUED_CHERUB_34\r\nQ3 upper ledge: RB16\r\nQ4 hidden ledge: CO27\r\nQ4 lower silver door: RESCUED_CHERUB_04\r\nQ4 upper bronze door: QI70\r\nQ4 upper silver door: CO37\r\nCoLCV entrance: RESCUED_CHERUB_05\r\nOil room: 1232338944\r\nQuirce: BS14\r\nQuirce room: QI72\r\n\r\nWasteland of the Buried Churches (8)\r\n\r\nLower tree path: RB04\r\nBuilding slide: CO14\r\nExterior ledge: CO36\r\nExterior cherub: RESCUED_CHERUB_10\r\nUnderneath MeD bridge: QI06\r\nCliffside ledge: HE02\r\nCliffside cherub: RESCUED_CHERUB_38\r\nRedento meeting 3: RB20\r\n\r\nWhere Olive Trees Wither (11)\r\n\r\nWaBC entrance: CO11\r\nHealing cave: QI20\r\nWhite lady flower: QI68\r\nWhite lady tomb: PR04\r\nWhite lady cave cherub: RESCUED_CHERUB_27\r\nWhite lady cave ledge: CO19\r\nEastern root cherub: RESCUED_CHERUB_23\r\nEastern root ledge: HE05\r\nDeath run: QI07\r\nGemino gift intial: QI59\r\nGemino gift final: RB10\r\n\r\nVarious (15)\r\n\r\nGuilt arena 1 extra: 1000.Arena_NailManager\r\nGuilt arena 1 main: QI32\r\nGuilt arena 2 extra: HE10\r\nGuilt arena 2 main: QI33\r\nGuilt arena 3 extra: 3000.Arena_NailManager\r\nGuilt arena 3 main: QI34\r\nGuilt arena 4 extra: RB34\r\nGuilt arena 4 main: QI35\r\nGuilt arena 5 extra: 5000.Arena_NailManager\r\nGuilt arena 5 main: QI79\r\nGuilt arena 6 extra: RB35\r\nGuilt arena 6 main: QI80\r\nGuilt arena 7 extra: RB36\r\nGuilt arena 7 main: QI81\r\nViridiana gift: PR08\r\n\r\nUnknown (6)\r\n\r\n?? amanecida: QI107\r\n?? amanecida: QI108\r\n?? amanecida: QI109\r\n?? amanecida: QI110\r\nLaudes reward: HE101\r\nAll amanecidas: PR101\r\n";
		}

		private Dictionary<string, string> getNames()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("x", "???");
			dictionary.Add("CO", "Collectible");
			dictionary.Add("CH", "Cherub");
			dictionary.Add("LU", "Life upgrade");
			dictionary.Add("FU", "Fervour upgrade");
			dictionary.Add("SU", "Mea Culpa upgrade");
			dictionary.Add("TR", "Tears");
			string text = "RB01: Dove Skull\r\nRB02: Ember of the Holy Cremation\r\nRB03: Silver Grape\r\nRB04: Uvula of Proclamation\r\nRB05: Hollow Pearl\r\nRB06: Knot of Hair\r\nRB07: Painted Wood Bead\r\nRB08: Piece of a Golden Mask\r\nRB09: Moss Preserved in Glass\r\nRB10: Frozen Olive\r\nRB11: Quirce's Scorched Bead\r\nRB12: Wicker Knot\r\nRB13: Perpetva's Protection\r\nRB14: Thorned Symbol\r\nRB15: Piece of a Tombstone\r\nRB16: Sphere of the Sacred Smoke\r\nRB17: Small Bead of Red Wax\r\nRB18: Medium Bead of Red Wax\r\nRB19: Big Bead of Red Wax\r\nRB20: Little Toe made of Limestone\r\nRB21: Big Toe made of Limestone\r\nRB22: Fourth Toe made of Limestone\r\nRB24: Small Bead of Blue Wax\r\nRB25: Medium Bead of Blue Wax\r\nRB26: Big Bead of Blue Wax\r\nRB28: Pelican Effigy\r\nRB30: Drop of Coagulated Ink\r\nRB31: Amber Eye\r\nRB32: Muted Bell\r\nRB33: Consecrated Amythyst\r\nRB34: Embers of a Broken Star\r\nRB35: Scaly Coin\r\nRB36: Seashell of the Inverted Spiral\r\nRB37: Calcified Eye of Erudition\r\nRB38: Immaculate Bead\r\nRB101: Reliquary of the Fervent Heart\r\nRB102: Reliquary of the Suffering Heart\r\nRB103: Reliquary of the Sorrowful Heart\r\nRB104: Token of Appreciation\r\nRB105: Cloistered Ruby\r\nRB106: Bead of Gold Thread\r\nRB107: Cloistered Sapphire\r\nRB108: Fire Enclosed in Enamel\r\nRB201: Light of the Lady of the Lamp\r\nRB202: Scale of Burnished Alabaster\r\nRB203: The Young Mason's Wheel\r\nRB204: Crown of Gnawed Iron\r\nRB301: Crimson Heart of a Miura\r\nPR01: Seguiriya to your Eyes like Stars\r\nPR03: Debla of the Lights\r\nPR04: Saeta Dolorosa\r\nPR05: Companillero to the Sons of the Aurora\r\nPR07: Lorquiana\r\nPR08: Zarabanda of the Safe Haven\r\nPR09: Taranto to my Sister\r\nPR10: Solea of Excommunication\r\nPR11: Tiento to your Thorned Hairs\r\nPR12: Cante Jondo of the Three Sisters\r\nPR14: Verdiales of the Forsaken Hamlet\r\nPR15: Romance to the Crimson Mist\r\nPR16: Zambra to the Resplendent Crown\r\nPR101: Aubade of the Nameless Guardian\r\nPR201: Cantina of the Blue Rose\r\nPR202: Mirabras of the Return to Port\r\nPR203: Tirana of the Celstial Bastion\r\nRE01: Blood Perpetuated in Sand\r\nRE02: Incorrupt Hand of the Fraternal Master\r\nRE03: Nail Uprooted from Dirt\r\nRE04: Shroud of Dreamt Sins\r\nRE05: Linen of Golden Thread\r\nRE07: Silvered Lung of Dolphos\r\nRE10: Three Gnarled Tongues\r\nHE01: Smoking Heart of Incense\r\nHE02: Heart of the Virtuous Pain\r\nHE03: Heart of Saltpeter Blood\r\nHE04: Heart of Oils\r\nHE05: Heart of Cerulean Incense\r\nHE06: Heart of the Holy Purge\r\nHE07: Molten Heart of Boiling Blood\r\nHE10: Heart of the Single Tone\r\nHE11: Heart of the Unnamed Minstrel\r\nHE101: Brilliant Heart of Dawn\r\nHE201: Apodictic Heart of Mea Culpa\r\nQI01: Cord of the True Burying\r\nQI02: Mark of the First Refuge\r\nQI03: Mark of the Second Refuge\r\nQI04: Mark of the Third Refuge\r\nQI06: Tentudia's Carnal Remains\r\nQI07: Remains of Tentudia's Hair\r\nQI08: Tentudia's Skeletal Remains\r\nQI10: Melted Golden Coins\r\nQI11: Torn Bridal Ribbon\r\nQI12: Black Greiving Veil\r\nQI13: Egg of Deformity\r\nQI14: Hatched Egg of Deformity\r\nQI19: Bouquet of Rosemary\r\nQI20: Incense Garlic\r\nQI31: Thorn\r\nQI32: Budding Sprout\r\nQI33: Grown Sprout\r\nQI34: Thorny Briar\r\nQI35: Bloodstained Briar\r\nQI37: Olive Seeds\r\nQI38: Holy Wound of Attrition\r\nQI39: Holy Wound of Contrition\r\nQI40: Holy Wound of Compunction\r\nQI41: Empty Bile Vessel\r\nQI44: Knot of Rosary Rope\r\nQI45: Empty Bile Vessel\r\nQI46: Empty Bile Vessel\r\nQI47: Empty Bile Vessel\r\nQI48: Empty Bile Vessel\r\nQI49: Empty Bile Vessel\r\nQI50: Empty Bile Vessel\r\nQI51: Empty Bile Vessel\r\nQI52: Knot of Rosary Rope\r\nQI53: Knot of Rosary Rope\r\nQI54: Knot of Rosary Rope\r\nQI55: Knot of Rosary Rope\r\nQI56: Knot of Rosary Rope\r\nQI57: Golden Thimble Filled with Burning Oil\r\nQI58: Key to the Chamber of the Eldest Brother\r\nQI59: Empty Golden Thimble\r\nQI60: Deformed Mask of Orestes\r\nQI61: Mirrored Mask of Dolphos\r\nQI62: Embossed Mask of Cresente\r\nQI63: Dried Clove\r\nQI64: Sooty Garlic\r\nQI65: Bouquet of Thyme\r\nQI66: Linen Cloth\r\nQI67: Severed Hand\r\nQI68: Dried Flowers bathed in Tears\r\nQI69: Key of the Secular\r\nQI70: Key of the Scribe\r\nQI71: Key of the Inquisitor\r\nQI72: Key of the High Peaks\r\nQI75: Chalice of Inverted Verses\r\nQI79: Crimson Briar\r\nQI80: Vine Braided in Blood\r\nQI81: Cvstodia of Sin\r\nQI101: Quicksilver\r\nQI102: Quicksilver\r\nQI103: Quicksilver\r\nQI104: Quicksilver\r\nQI105: Quicksilver\r\nQI106: Petrified Bell\r\nQI107: Verses Spun from Gold\r\nQI108: Verses Spun from Gold\r\nQI109: Verses Spun from Gold\r\nQI110: Verses Spun from Gold\r\nQI201: Severed Right Eye of the Traitor\r\nQI202: Broken Left Eye of the Traitor\r\nQI203: Incomplete Scapular\r\nQI204: Key Grown from Twisted Wood\r\nQI301: Holy Wound of Abnegation\r\n";
			int lineIdx = text.IndexOf('\r');
			int start = 0;
			while (lineIdx != -1)
			{
				string sub = text.Substring(start, lineIdx - start);
				int colon = sub.IndexOf(':');
				dictionary.Add(sub.Substring(0, colon), sub.Substring(colon + 2));
				start = lineIdx + 2;
				lineIdx = text.IndexOf('\r', start);
			}
			return dictionary;
		}

		private string GetRewardId(string id, Dictionary<string, Reward> rewards)
		{
			if (!rewards.ContainsKey(id))
			{
				return "x";
			}
			Reward reward = rewards[id];
			switch (reward.type)
			{
			case 0:
				return "RB" + reward.id.ToString("00");
			case 1:
				return "PR" + reward.id.ToString("00");
			case 2:
				return "RE" + reward.id.ToString("00");
			case 3:
				return "HE" + reward.id.ToString("00");
			case 4:
				return "CO";
			case 5:
				return "QI" + reward.id.ToString("00");
			case 6:
				return "CH";
			case 7:
				return "LU";
			case 8:
				return "FU";
			case 9:
				return "SU";
			case 10:
				return "TR";
			default:
				return "x";
			}
		}

		public string createSpoiler(Dictionary<string, Reward> rewards)
		{
			Dictionary<string, string> names = this.getNames();
			string text = this.getTemplate();
			string output = "";
			int num;
			for (int start = 0; start != text.Length; start = num)
			{
				int colonIdx = text.IndexOf(':', start);
				if (colonIdx < 0)
				{
					break;
				}
				num = text.IndexOf('\r', colonIdx + 1);
				if (num < 0)
				{
					num = text.Length;
				}
				string id = this.GetRewardId(text.Substring(colonIdx + 2, num - colonIdx - 2), rewards);
				string display = "x";
				if (names.ContainsKey(id))
				{
					display = names[id];
				}
				output = output + text.Substring(start, colonIdx + 2 - start) + display;
			}
			return output;
		}
	}
}
