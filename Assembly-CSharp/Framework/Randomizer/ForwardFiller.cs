using System;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Managers;

namespace Framework.Randomizer
{
	internal class ForwardFiller
	{
		public bool Fill(int seed, Dictionary<string, Reward> output)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.random = new Random(seed);
			List<Location> list = new List<Location>();
			List<Location> vanillaLocations = new List<Location>();
			List<Reward> list2 = new List<Reward>();
			List<Reward> list3 = new List<Reward>();
			this.getLists(list, list2);
			if (list.Count != list2.Count)
			{
				Core.Randomizer.Log("Invalid number of locations & rewards!", 0);
				return true;
			}
			this.fillVanillaItems(list, vanillaLocations, list2);
			this.getProgressionItems(list2, list3);
			this.fillProgressionItems(list, vanillaLocations, list3);
			if (list3.Count > 0)
			{
				return false;
			}
			this.fillExtraItems(list, list2);
			if (list2.Count > 0)
			{
				return false;
			}
			output.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				output.Add(list[i].id, list[i].reward);
			}
			if (this.newGame)
			{
				string str = new Spoiler().createSpoiler(output);
				stopwatch.Stop();
				string str2 = string.Format("Filling seed {0}\nExecution time: {1} ms\n\n", seed, stopwatch.ElapsedMilliseconds);
				string fileName = "spoiler" + PersistentManager.GetAutomaticSlot() + ".txt";
				new FileIO().writeAll(str2 + str, fileName);
			}
			return true;
		}

		private int rand(int max)
		{
			return this.random.Next(max);
		}

		private void fillVanillaItems(List<Location> locations, List<Location> vanillaLocations, List<Reward> rewards)
		{
			List<Reward> list = new List<Reward>();
			for (int i = 0; i < locations.Count; i++)
			{
				if (!this.randoSettings.Contains(locations[i].type))
				{
					locations[i].reward = rewards[i];
					if (rewards[i].progression)
					{
						vanillaLocations.Add(locations[i]);
					}
					list.Add(rewards[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				rewards.Remove(list[j]);
			}
		}

		private void getProgressionItems(List<Reward> rewards, List<Reward> progressionRewards)
		{
			for (int i = 0; i < rewards.Count; i++)
			{
				if (rewards[i].progression)
				{
					progressionRewards.Add(rewards[i]);
					rewards.RemoveAt(i);
					i--;
				}
			}
		}

		private void fillProgressionItems(List<Location> locations, List<Location> vanillaLocations, List<Reward> progressionRewards)
		{
			List<Location> list = new List<Location>();
			List<Reward> list2 = new List<Reward>();
			this.shuffleList<Reward>(progressionRewards);
			int num = -1;
			for (int i = 0; i < progressionRewards.Count; i++)
			{
				if (progressionRewards[i].type == 2 && progressionRewards[i].id == 1)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				Reward item = progressionRewards[num];
				progressionRewards.RemoveAt(num);
				progressionRewards.Add(item);
			}
			else
			{
				Core.Randomizer.Log("Blood item could not be found", 0);
			}
			this.findReachable(locations, vanillaLocations, list, list2);
			while (list.Count > 0 && progressionRewards.Count > 0)
			{
				int index = this.rand(list.Count);
				Location location = list[index];
				Reward reward = progressionRewards[progressionRewards.Count - 1];
				location.reward = reward;
				list2.Add(reward);
				progressionRewards.RemoveAt(progressionRewards.Count - 1);
				this.findReachable(locations, vanillaLocations, list, list2);
			}
		}

		private void findReachable(List<Location> locations, List<Location> vanillaLocations, List<Location> reachableLocations, List<Reward> itemsOwned)
		{
			List<Reward> list = new List<Reward>();
			ItemData itemData;
			for (;;)
			{
				itemData = this.calculateItemData(itemsOwned);
				this.checkVanillaLocations(vanillaLocations, list, itemData);
				if (list.Count == 0)
				{
					break;
				}
				itemsOwned.AddRange(list);
			}
			reachableLocations.Clear();
			for (int i = 0; i < locations.Count; i++)
			{
				if (locations[i].reward == null && locations[i].isReachable(itemData))
				{
					reachableLocations.Add(locations[i]);
				}
			}
		}

		private void checkVanillaLocations(List<Location> vanillaLocations, List<Reward> newItems, ItemData data)
		{
			newItems.Clear();
			for (int i = 0; i < vanillaLocations.Count; i++)
			{
				if (vanillaLocations[i].isReachable(data))
				{
					newItems.Add(vanillaLocations[i].reward);
					vanillaLocations.RemoveAt(i);
					i--;
				}
			}
		}

		private void fillExtraItems(List<Location> locations, List<Reward> rewards)
		{
			this.shuffleList<Location>(locations);
			for (int i = 0; i < locations.Count; i++)
			{
				if (locations[i].reward == null)
				{
					locations[i].reward = rewards[rewards.Count - 1];
					rewards.RemoveAt(rewards.Count - 1);
				}
			}
		}

		private void shuffleList<T>(List<T> list)
		{
			List<T> list2 = new List<T>();
			while (list.Count > 0)
			{
				int index = this.rand(list.Count);
				list2.Add(list[index]);
				list.RemoveAt(index);
			}
			list.AddRange(list2);
		}

		private ItemData calculateItemData(List<Reward> itemsOwned)
		{
			ItemData itemData = default(ItemData);
			int num = 0;
			for (int i = 0; i < itemsOwned.Count; i++)
			{
				Reward reward = itemsOwned[i];
				if (reward.type == 0)
				{
					int id = reward.id;
					switch (id)
					{
					case 17:
					case 18:
					case 19:
						itemData.redWax++;
						break;
					case 20:
					case 21:
					case 22:
						itemData.limestones++;
						break;
					case 23:
						break;
					case 24:
					case 25:
					case 26:
						itemData.blueWax++;
						break;
					default:
						if (id == 38)
						{
							itemData.guiltBead = true;
						}
						break;
					}
				}
				else if (reward.type == 1)
				{
					if (reward.id == 3)
					{
						itemData.lightningAir = true;
						itemData.lightningAny = true;
					}
					else if (reward.id == 14)
					{
						itemData.lightningAny = true;
					}
				}
				else if (reward.type == 2)
				{
					switch (reward.id)
					{
					case 1:
						itemData.blood = true;
						break;
					case 3:
						itemData.nail = true;
						break;
					case 4:
						itemData.shroud = true;
						break;
					case 5:
						itemData.linen = true;
						break;
					case 7:
						itemData.lung = true;
						break;
					case 10:
						itemData.root = true;
						break;
					}
				}
				else if (reward.type == 3 && reward.id == 201)
				{
					itemData.trueHeart = true;
				}
				else if (reward.type == 4)
				{
					itemData.bones++;
				}
				else if (reward.type == 5)
				{
					int id2 = reward.id;
					switch (id2)
					{
					case 1:
						itemData.cord = true;
						goto IL_4D4;
					case 2:
					case 3:
					case 4:
						itemData.marksOfRefuge++;
						goto IL_4D4;
					case 5:
					case 9:
					case 15:
					case 16:
					case 17:
					case 18:
						goto IL_4D4;
					case 6:
					case 7:
					case 8:
						itemData.tentudiaRemains++;
						goto IL_4D4;
					case 10:
					case 11:
					case 12:
						itemData.ceremonyItems++;
						goto IL_4D4;
					case 13:
						itemData.egg = true;
						goto IL_4D4;
					case 14:
						itemData.hatchedEgg = true;
						goto IL_4D4;
					case 19:
					case 20:
						break;
					default:
						switch (id2)
						{
						case 37:
						case 63:
						case 64:
						case 65:
							goto IL_4B1;
						case 38:
						case 39:
						case 40:
							num++;
							goto IL_4D4;
						case 41:
						case 42:
						case 43:
						case 44:
						case 45:
						case 46:
						case 47:
						case 48:
						case 49:
						case 50:
						case 51:
						case 52:
						case 53:
						case 54:
						case 55:
						case 56:
						case 73:
						case 74:
							goto IL_4D4;
						case 57:
							itemData.fullThimble = true;
							goto IL_4D4;
						case 58:
							itemData.elderKey = true;
							goto IL_4D4;
						case 59:
							itemData.emptyThimble = true;
							goto IL_4D4;
						case 60:
						case 61:
						case 62:
							itemData.masks++;
							goto IL_4D4;
						case 66:
							itemData.cloth = true;
							goto IL_4D4;
						case 67:
							itemData.hand = true;
							goto IL_4D4;
						case 68:
							itemData.driedFlowers = true;
							goto IL_4D4;
						case 69:
							itemData.bronzeKey = true;
							goto IL_4D4;
						case 70:
							itemData.silverKey = true;
							goto IL_4D4;
						case 71:
							itemData.goldKey = true;
							goto IL_4D4;
						case 72:
							itemData.peaksKey = true;
							goto IL_4D4;
						case 75:
							itemData.chalice = true;
							goto IL_4D4;
						case 107:
						case 108:
						case 109:
						case 110:
							itemData.verses++;
							goto IL_4D4;
						}
						switch (id2)
						{
						case 201:
						case 202:
							itemData.traitorEyes++;
							goto IL_4D4;
						case 203:
							itemData.scapular = true;
							goto IL_4D4;
						case 204:
							itemData.woodKey = true;
							goto IL_4D4;
						default:
							goto IL_4D4;
						}
						break;
					}
					IL_4B1:
					itemData.herbs++;
				}
				else if (reward.type == 6)
				{
					itemData.cherubs++;
				}
				IL_4D4:;
			}
			itemData.lungDamage = true;
			itemData.bridge = (num >= 3 && itemData.blood && itemData.scapular);
			return itemData;
		}

		private void getLists(List<Location> locations, List<Reward> rewards)
		{
			locations.Clear();
			locations.Add(new Location("RB01", "item", (ItemData d) => true));
			locations.Add(new Location("RB02", "shop", (ItemData d) => true));
			locations.Add(new Location("RB03", "item", (ItemData d) => d.shroud));
			locations.Add(new Location("RB04", "item", (ItemData d) => true));
			locations.Add(new Location("RB05", "shop", (ItemData d) => true));
			locations.Add(new Location("RB06", "altasgracias2", (ItemData d) => d.hatchedEgg && d.root));
			locations.Add(new Location("RB07", "item", (ItemData d) => d.blood));
			locations.Add(new Location("RB08", "item", (ItemData d) => d.lung));
			locations.Add(new Location("RB09", "shop", (ItemData d) => true));
			locations.Add(new Location("RB10", "gemino1", (ItemData d) => d.fullThimble));
			locations.Add(new Location("RB11", "item", (ItemData d) => d.bridge && d.masks > 0));
			locations.Add(new Location("RB12", "shop", (ItemData d) => d.bridge));
			locations.Add(new Location("RB13", "item", (ItemData d) => true));
			locations.Add(new Location("RB14", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("RB15", "item", (ItemData d) => true));
			locations.Add(new Location("RB16", "item", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("RB17", "item", (ItemData d) => true));
			locations.Add(new Location("RB18", "candle", (ItemData d) => d.redWax > 0));
			locations.Add(new Location("RB19", "candle", (ItemData d) => d.bridge && d.redWax > 0));
			locations.Add(new Location("RB20", "redento1", (ItemData d) => true));
			locations.Add(new Location("RB21", "redento1", (ItemData d) => d.bridge));
			locations.Add(new Location("RB22", "redento1", (ItemData d) => true));
			locations.Add(new Location("RB24", "item", (ItemData d) => true));
			locations.Add(new Location("RB25", "candle", (ItemData d) => d.blueWax > 0));
			locations.Add(new Location("RB26", "candle", (ItemData d) => d.bridge && d.blueWax > 0));
			locations.Add(new Location("RB28", "item", (ItemData d) => d.root));
			locations.Add(new Location("RB30", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("RB31", "item", (ItemData d) => d.bridge && d.blood && d.root && d.lung));
			locations.Add(new Location("RB32", "item", (ItemData d) => true));
			locations.Add(new Location("RB33", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("RB34", "guiltArena", (ItemData d) => d.guiltBead));
			locations.Add(new Location("RB35", "guiltArena", (ItemData d) => d.guiltBead && d.bridge && (d.blood || (d.masks > 0 && d.bronzeKey && d.silverKey))));
			locations.Add(new Location("RB36", "guiltArena", (ItemData d) => d.guiltBead && d.bridge && d.blood && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("RB37", "shop", (ItemData d) => true));
			locations.Add(new Location("RB38", "guiltBead", (ItemData d) => true));
			locations.Add(new Location("RB101", "penitence", (ItemData d) => true));
			locations.Add(new Location("RB102", "penitence", (ItemData d) => true));
			locations.Add(new Location("RB103", "penitence", (ItemData d) => true));
			locations.Add(new Location("RB104", "church", (ItemData d) => true));
			locations.Add(new Location("RB105", "church", (ItemData d) => true));
			locations.Add(new Location("RB106", "item", (ItemData d) => d.blood && d.root));
			locations.Add(new Location("RB107", "item", (ItemData d) => true));
			locations.Add(new Location("RB108", "item", (ItemData d) => true));
			locations.Add(new Location("RB201", "item", (ItemData d) => d.bridge && d.root));
			locations.Add(new Location("RB202", "item", (ItemData d) => true));
			locations.Add(new Location("RB203", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("RB204", "item", (ItemData d) => d.blood && d.linen));
			locations.Add(new Location("RB301", "crisanta", (ItemData d) => d.bridge && d.woodKey));
			locations.Add(new Location("PR01", "item", (ItemData d) => true));
			locations.Add(new Location("PR03", "tentudia", (ItemData d) => d.tentudiaRemains >= 3));
			locations.Add(new Location("PR04", "gemino2", (ItemData d) => (d.fullThimble || d.linen) && d.driedFlowers));
			locations.Add(new Location("PR05", "jocinero", (ItemData d) => d.bridge && d.cherubs >= 38));
			locations.Add(new Location("PR07", "item", (ItemData d) => d.bridge && d.blood));
			locations.Add(new Location("PR08", "viridiana", (ItemData d) => true));
			locations.Add(new Location("PR09", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("PR10", "item", (ItemData d) => d.root));
			locations.Add(new Location("PR11", "cleofas", (ItemData d) => d.bridge && d.marksOfRefuge >= 3 && d.cord));
			locations.Add(new Location("PR12", "item", (ItemData d) => d.bridge && d.masks > 0 && d.linen && d.root));
			locations.Add(new Location("PR14", "item", (ItemData d) => true));
			locations.Add(new Location("PR15", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("PR16", "item", (ItemData d) => d.nail || d.linen));
			locations.Add(new Location("PR101", "amanecida2", (ItemData d) => d.bell && d.swordLevel > 0 && d.bridge && d.blood && d.root && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("PR201", "miriam", (ItemData d) => d.bridge && d.masks > 1 && d.linen && d.blood && d.root && d.lung));
			locations.Add(new Location("PR202", "item", (ItemData d) => d.blood || d.bridge));
			locations.Add(new Location("PR203", "item", (ItemData d) => d.blood));
			locations.Add(new Location("RE01", "item", (ItemData d) => d.elderKey));
			locations.Add(new Location("RE02", "blessing", (ItemData d) => d.hand));
			locations.Add(new Location("RE03", "redento1", (ItemData d) => d.bridge && d.limestones >= 3));
			locations.Add(new Location("RE04", "blessing", (ItemData d) => d.cloth));
			locations.Add(new Location("RE05", "jocinero", (ItemData d) => d.bridge && d.cherubs >= 20));
			locations.Add(new Location("RE07", "item", (ItemData d) => true));
			locations.Add(new Location("RE10", "blessing", (ItemData d) => d.hatchedEgg));
			locations.Add(new Location("HE01", "item", (ItemData d) => d.bridge && d.blood));
			locations.Add(new Location("HE02", "item", (ItemData d) => true));
			locations.Add(new Location("HE03", "item", (ItemData d) => d.lung));
			locations.Add(new Location("HE04", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("HE05", "item", (ItemData d) => d.root));
			locations.Add(new Location("HE06", "item", (ItemData d) => true));
			locations.Add(new Location("HE07", "item", (ItemData d) => d.bridge && d.redWax >= 3 && d.blueWax >= 3));
			locations.Add(new Location("HE10", "guiltArena", (ItemData d) => d.guiltBead));
			locations.Add(new Location("HE11", "item", (ItemData d) => true));
			locations.Add(new Location("HE101", "amanecida1", (ItemData d) => d.bridge && d.verses >= 4));
			locations.Add(new Location("HE201", "crisanta", (ItemData d) => d.bridge && d.woodKey && d.traitorEyes >= 2));
			locations.Add(new Location("CO01", "item", (ItemData d) => true));
			locations.Add(new Location("CO02", "item", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.goldKey));
			locations.Add(new Location("CO03", "item", (ItemData d) => true));
			locations.Add(new Location("CO04", "item", (ItemData d) => d.swordLevel > 0));
			locations.Add(new Location("CO05", "item", (ItemData d) => true));
			locations.Add(new Location("CO06", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO07", "item", (ItemData d) => true));
			locations.Add(new Location("CO08", "item", (ItemData d) => true));
			locations.Add(new Location("CO09", "item", (ItemData d) => true));
			locations.Add(new Location("CO10", "item", (ItemData d) => d.bridge && d.masks > 0));
			locations.Add(new Location("CO11", "item", (ItemData d) => true));
			locations.Add(new Location("CO12", "item", (ItemData d) => d.lungDamage));
			locations.Add(new Location("CO13", "item", (ItemData d) => true));
			locations.Add(new Location("CO14", "item", (ItemData d) => true));
			locations.Add(new Location("CO15", "item", (ItemData d) => true));
			locations.Add(new Location("CO16", "item", (ItemData d) => true));
			locations.Add(new Location("CO17", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO18", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO19", "item", (ItemData d) => (d.fullThimble || d.linen) && d.blood));
			locations.Add(new Location("CO20", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO21", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO22", "item", (ItemData d) => d.bridge && d.root));
			locations.Add(new Location("CO23", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO24", "item", (ItemData d) => d.bridge && d.masks > 0 && d.silverKey));
			locations.Add(new Location("CO25", "item", (ItemData d) => true));
			locations.Add(new Location("CO26", "item", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.goldKey));
			locations.Add(new Location("CO27", "item", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey));
			locations.Add(new Location("CO28", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO29", "item", (ItemData d) => true));
			locations.Add(new Location("CO30", "item", (ItemData d) => true));
			locations.Add(new Location("CO31", "item", (ItemData d) => d.bridge && d.linen));
			locations.Add(new Location("CO32", "item", (ItemData d) => d.nail && d.lung));
			locations.Add(new Location("CO33", "item", (ItemData d) => true));
			locations.Add(new Location("CO34", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO35", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("CO36", "item", (ItemData d) => true));
			locations.Add(new Location("CO37", "item", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("CO38", "item", (ItemData d) => true));
			locations.Add(new Location("CO39", "item", (ItemData d) => d.bridge && d.root));
			locations.Add(new Location("CO40", "item", (ItemData d) => d.bridge && d.masks > 0 && d.lung && d.root && d.blood));
			locations.Add(new Location("CO41", "item", (ItemData d) => true));
			locations.Add(new Location("CO42", "item", (ItemData d) => d.blood || d.swordLevel > 0));
			locations.Add(new Location("CO43", "item", (ItemData d) => true));
			locations.Add(new Location("CO44", "item", (ItemData d) => d.linen));
			locations.Add(new Location("QI01", "cleofas", (ItemData d) => d.bridge && d.marksOfRefuge >= 3));
			locations.Add(new Location("QI02", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("QI03", "item", (ItemData d) => d.bridge && d.masks > 0));
			locations.Add(new Location("QI04", "item", (ItemData d) => d.bridge && d.masks > 1));
			locations.Add(new Location("QI06", "item", (ItemData d) => d.blood));
			locations.Add(new Location("QI07", "item", (ItemData d) => true));
			locations.Add(new Location("QI08", "item", (ItemData d) => d.blood && d.root));
			locations.Add(new Location("QI10", "item", (ItemData d) => d.blood));
			locations.Add(new Location("QI11", "shop", (ItemData d) => true));
			locations.Add(new Location("QI12", "item", (ItemData d) => true));
			locations.Add(new Location("QI13", "altasgracias1", (ItemData d) => d.ceremonyItems >= 3));
			locations.Add(new Location("QI14", "altasgracias1", (ItemData d) => d.egg));
			locations.Add(new Location("QI19", "herb", (ItemData d) => true));
			locations.Add(new Location("QI20", "herb", (ItemData d) => true));
			locations.Add(new Location("QI31", "deosgracias", (ItemData d) => true));
			locations.Add(new Location("QI32", "guiltUpgrade", (ItemData d) => d.guiltBead));
			locations.Add(new Location("QI33", "guiltUpgrade", (ItemData d) => d.guiltBead));
			locations.Add(new Location("QI34", "guiltUpgrade", (ItemData d) => d.guiltBead));
			locations.Add(new Location("QI35", "guiltUpgrade", (ItemData d) => d.guiltBead));
			locations.Add(new Location("QI37", "herb", (ItemData d) => d.bridge));
			locations.Add(new Location("QI38", "visage", (ItemData d) => true));
			locations.Add(new Location("QI39", "visage", (ItemData d) => true));
			locations.Add(new Location("QI40", "visage", (ItemData d) => true));
			locations.Add(new Location("QI41", "item", (ItemData d) => true));
			locations.Add(new Location("QI44", "item", (ItemData d) => d.lungDamage));
			locations.Add(new Location("QI45", "item", (ItemData d) => true));
			locations.Add(new Location("QI46", "item", (ItemData d) => true));
			locations.Add(new Location("QI47", "item", (ItemData d) => d.blood));
			locations.Add(new Location("QI48", "item", (ItemData d) => true));
			locations.Add(new Location("QI49", "shop", (ItemData d) => d.bridge));
			locations.Add(new Location("QI50", "item", (ItemData d) => d.bridge && d.swordLevel > 0));
			locations.Add(new Location("QI51", "item", (ItemData d) => d.bridge && d.masks > 0 && d.goldKey));
			locations.Add(new Location("QI52", "item", (ItemData d) => true));
			locations.Add(new Location("QI53", "item", (ItemData d) => true));
			locations.Add(new Location("QI54", "redento2", (ItemData d) => d.bridge && d.limestones >= 3));
			locations.Add(new Location("QI55", "item", (ItemData d) => d.nail && d.blood && d.swordLevel > 0));
			locations.Add(new Location("QI56", "tirso2", (ItemData d) => d.herbs >= 6 && d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("QI57", "gemino1", (ItemData d) => d.emptyThimble));
			locations.Add(new Location("QI58", "shop", (ItemData d) => true));
			locations.Add(new Location("QI59", "gemino1", (ItemData d) => true));
			locations.Add(new Location("QI60", "mask", (ItemData d) => d.bridge));
			locations.Add(new Location("QI61", "mask", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey && d.peaksKey));
			locations.Add(new Location("QI62", "mask", (ItemData d) => d.bridge));
			locations.Add(new Location("QI63", "herb", (ItemData d) => d.blood));
			locations.Add(new Location("QI64", "herb", (ItemData d) => d.bridge));
			locations.Add(new Location("QI65", "herb", (ItemData d) => true));
			locations.Add(new Location("QI66", "tirso1", (ItemData d) => d.herbs > 0));
			locations.Add(new Location("QI67", "item", (ItemData d) => d.nail));
			locations.Add(new Location("QI68", "gemino1", (ItemData d) => d.fullThimble));
			locations.Add(new Location("QI69", "item", (ItemData d) => d.bridge && d.masks > 0));
			locations.Add(new Location("QI70", "item", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey));
			locations.Add(new Location("QI71", "shop", (ItemData d) => d.bridge));
			locations.Add(new Location("QI72", "item", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("QI75", "chalice", (ItemData d) => d.lung && d.nail && d.root));
			locations.Add(new Location("QI79", "guiltUpgrade", (ItemData d) => d.guiltBead && d.bridge));
			locations.Add(new Location("QI80", "guiltUpgrade", (ItemData d) => d.guiltBead && d.bridge && (d.blood || (d.masks > 0 && d.bronzeKey && d.silverKey))));
			locations.Add(new Location("QI81", "guiltUpgrade", (ItemData d) => d.guiltBead && d.bridge && d.blood && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("QI101", "item", (ItemData d) => d.swordLevel > 0));
			locations.Add(new Location("QI102", "item", (ItemData d) => d.bridge && ((d.blood && d.root) || (d.masks > 0 && d.bronzeKey))));
			locations.Add(new Location("QI103", "item", (ItemData d) => true));
			locations.Add(new Location("QI104", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("QI105", "item", (ItemData d) => d.bridge));
			locations.Add(new Location("QI106", "amanecida1", (ItemData d) => true));
			locations.Add(new Location("QI107", "amanecida2", (ItemData d) => d.bell && d.swordLevel > 0));
			locations.Add(new Location("QI108", "amanecida2", (ItemData d) => d.bell && d.swordLevel > 0 && ((d.blood && d.root) || d.bridge)));
			locations.Add(new Location("QI109", "amanecida2", (ItemData d) => d.bell && d.swordLevel > 0 && d.bridge && ((d.blood && d.root) || (d.masks > 0 && d.bronzeKey && d.silverKey))));
			locations.Add(new Location("QI110", "amanecida2", (ItemData d) => d.bell && d.swordLevel > 0 && d.bridge && d.blood && d.root && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("QI201", "crisanta", (ItemData d) => d.bones >= 30));
			locations.Add(new Location("QI202", "crisanta", (ItemData d) => d.bridge));
			locations.Add(new Location("QI203", "crisanta", (ItemData d) => d.blood || d.bridge));
			locations.Add(new Location("QI204", "crisanta", (ItemData d) => d.bridge && d.blood && d.scapular));
			locations.Add(new Location("QI301", "crisanta", (ItemData d) => d.bridge && d.masks >= 3 && d.trueHeart && d.blood));
			locations.Add(new Location("RESCUED_CHERUB_01", "cherub", (ItemData d) => d.bridge));
			locations.Add(new Location("RESCUED_CHERUB_02", "cherub", (ItemData d) => d.bridge));
			locations.Add(new Location("RESCUED_CHERUB_03", "cherub", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey));
			locations.Add(new Location("RESCUED_CHERUB_04", "cherub", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("RESCUED_CHERUB_05", "cherub", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("RESCUED_CHERUB_06", "cherub", (ItemData d) => d.blood && (d.linen || d.root)));
			locations.Add(new Location("RESCUED_CHERUB_07", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_08", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_09", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_10", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_11", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_12", "cherub", (ItemData d) => d.lung && d.nail));
			locations.Add(new Location("RESCUED_CHERUB_13", "cherub", (ItemData d) => d.linen || d.nail || d.lightningAir));
			locations.Add(new Location("RESCUED_CHERUB_14", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_15", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_16", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_17", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_18", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_19", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_20", "cherub", (ItemData d) => d.root || d.lightningAir));
			locations.Add(new Location("RESCUED_CHERUB_21", "cherub", (ItemData d) => d.blood && d.lightningAny));
			locations.Add(new Location("RESCUED_CHERUB_22", "cherub", (ItemData d) => d.linen || (d.lightningAir && d.lung && d.nail)));
			locations.Add(new Location("RESCUED_CHERUB_23", "cherub", (ItemData d) => d.root));
			locations.Add(new Location("RESCUED_CHERUB_24", "cherub", (ItemData d) => d.linen || d.lightningAir));
			locations.Add(new Location("RESCUED_CHERUB_25", "cherub", (ItemData d) => d.blood || d.lightningAir));
			locations.Add(new Location("RESCUED_CHERUB_26", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_27", "cherub", (ItemData d) => ((d.fullThimble || d.linen) && d.lightningAny) || d.swordLevel > 1));
			locations.Add(new Location("RESCUED_CHERUB_28", "cherub", (ItemData d) => d.bridge));
			locations.Add(new Location("RESCUED_CHERUB_29", "cherub", (ItemData d) => d.bridge));
			locations.Add(new Location("RESCUED_CHERUB_30", "cherub", (ItemData d) => d.bridge));
			locations.Add(new Location("RESCUED_CHERUB_31", "cherub", (ItemData d) => d.blood || d.lightningAir));
			locations.Add(new Location("RESCUED_CHERUB_32", "cherub", (ItemData d) => d.bridge && d.blood && d.root));
			locations.Add(new Location("RESCUED_CHERUB_33", "cherub", (ItemData d) => d.bridge && d.lightningAir));
			locations.Add(new Location("RESCUED_CHERUB_34", "cherub", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("RESCUED_CHERUB_35", "cherub", (ItemData d) => d.bridge));
			locations.Add(new Location("RESCUED_CHERUB_36", "cherub", (ItemData d) => d.bridge && d.masks > 0));
			locations.Add(new Location("RESCUED_CHERUB_37", "cherub", (ItemData d) => true));
			locations.Add(new Location("RESCUED_CHERUB_38", "cherub", (ItemData d) => d.swordLevel > 1));
			locations.Add(new Location("-933363712", "lady", (ItemData d) => true));
			locations.Add(new Location("-927137792", "lady", (ItemData d) => true));
			locations.Add(new Location("-840761344", "lady", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey && d.peaksKey));
			locations.Add(new Location("-831062016", "lady", (ItemData d) => true));
			locations.Add(new Location("1213038592", "lady", (ItemData d) => d.bridge && d.lung && d.root && d.blood));
			locations.Add(new Location("1271136256", "lady", (ItemData d) => d.bridge));
			locations.Add(new Location("-910753792", "oil", (ItemData d) => d.blood));
			locations.Add(new Location("-886603776", "oil", (ItemData d) => true));
			locations.Add(new Location("-815923200", "oil", (ItemData d) => true));
			locations.Add(new Location("1203666944", "oil", (ItemData d) => d.bridge));
			locations.Add(new Location("1232338944", "oil", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("1233780736", "oil", (ItemData d) => d.bridge));
			locations.Add(new Location("-932151296", "sword", (ItemData d) => d.bridge && d.chalice && d.masks > 0 && d.bronzeKey && d.lung && d.root && d.nail));
			locations.Add(new Location("-865337344", "sword", (ItemData d) => true));
			locations.Add(new Location("-835715072", "sword", (ItemData d) => true));
			locations.Add(new Location("741515178", "sword", (ItemData d) => true));
			locations.Add(new Location("1189150720", "sword", (ItemData d) => d.bridge));
			locations.Add(new Location("1240137728", "sword", (ItemData d) => d.bridge));
			locations.Add(new Location("1330243584", "sword", (ItemData d) => d.bridge && d.masks > 0 && d.blood && d.root && d.lung));
			locations.Add(new Location("500.Tirso", "tirso2", (ItemData d) => d.herbs >= 2));
			locations.Add(new Location("1000.Tirso", "tirso2", (ItemData d) => d.herbs >= 3));
			locations.Add(new Location("2000.Tirso", "tirso2", (ItemData d) => d.herbs >= 4));
			locations.Add(new Location("5000.Tirso", "tirso2", (ItemData d) => d.herbs >= 5));
			locations.Add(new Location("10000.Tirso", "tirso2", (ItemData d) => d.herbs >= 6));
			locations.Add(new Location("500.Lvdovico", "tentudia", (ItemData d) => d.tentudiaRemains >= 1));
			locations.Add(new Location("1000.Lvdovico", "tentudia", (ItemData d) => d.tentudiaRemains >= 2));
			locations.Add(new Location("1000.Arena_NailManager", "guiltArena", (ItemData d) => d.guiltBead));
			locations.Add(new Location("3000.Arena_NailManager", "guiltArena", (ItemData d) => d.guiltBead));
			locations.Add(new Location("5000.Arena_NailManager", "guiltArena", (ItemData d) => d.guiltBead && d.bridge));
			locations.Add(new Location("250.Undertaker", "ossuary", (ItemData d) => d.bones >= 4));
			locations.Add(new Location("500.Undertaker", "ossuary", (ItemData d) => d.bones >= 8));
			locations.Add(new Location("750.Undertaker", "ossuary", (ItemData d) => d.bones >= 12));
			locations.Add(new Location("1000.Undertaker", "ossuary", (ItemData d) => d.bones >= 16));
			locations.Add(new Location("1250.Undertaker", "ossuary", (ItemData d) => d.bones >= 20));
			locations.Add(new Location("1500.Undertaker", "ossuary", (ItemData d) => d.bones >= 24));
			locations.Add(new Location("1750.Undertaker", "ossuary", (ItemData d) => d.bones >= 28));
			locations.Add(new Location("2000.Undertaker", "ossuary", (ItemData d) => d.bones >= 32));
			locations.Add(new Location("2500.Undertaker", "ossuary", (ItemData d) => d.bones >= 36));
			locations.Add(new Location("3000.Undertaker", "ossuary", (ItemData d) => d.bones >= 40));
			locations.Add(new Location("5000.Undertaker", "ossuary", (ItemData d) => d.bones >= 44));
			locations.Add(new Location("BS01", "boss", (ItemData d) => true));
			locations.Add(new Location("BS03", "boss", (ItemData d) => true));
			locations.Add(new Location("BS04", "boss", (ItemData d) => true));
			locations.Add(new Location("BS05", "boss", (ItemData d) => d.bridge));
			locations.Add(new Location("BS06", "boss", (ItemData d) => d.bridge));
			locations.Add(new Location("BS12", "crisanta", (ItemData d) => d.bridge));
			locations.Add(new Location("BS13", "boss", (ItemData d) => true));
			locations.Add(new Location("BS14", "boss", (ItemData d) => d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("BS16", "boss", (ItemData d) => d.bridge && d.masks >= 3 && d.trueHeart));
			locations.Add(new Location("18000.D03Z01S03", "amanecida1", (ItemData d) => d.bell && d.swordLevel > 0));
			locations.Add(new Location("18000.D02Z02S14", "amanecida1", (ItemData d) => d.bell && d.swordLevel > 0 && d.blood && d.root));
			locations.Add(new Location("18000.D04Z01S04", "amanecida1", (ItemData d) => d.bell && d.swordLevel > 0 && d.bridge));
			locations.Add(new Location("18000.D09Z01S01", "amanecida1", (ItemData d) => d.bell && d.swordLevel > 0 && d.bridge && d.masks > 0 && d.bronzeKey && d.silverKey));
			locations.Add(new Location("30000.LaudesBossTrigger", "amanecida1", (ItemData d) => d.bridge && d.verses >= 4));
			locations.Add(new Location("5000.BossTrigger", "boss", (ItemData d) => d.bridge));
			rewards.Clear();
			rewards.Add(new Reward(0, 1, false));
			rewards.Add(new Reward(0, 2, false));
			rewards.Add(new Reward(0, 3, false));
			rewards.Add(new Reward(0, 4, false));
			rewards.Add(new Reward(0, 5, false));
			rewards.Add(new Reward(0, 6, false));
			rewards.Add(new Reward(0, 7, false));
			rewards.Add(new Reward(0, 8, false));
			rewards.Add(new Reward(0, 9, false));
			rewards.Add(new Reward(0, 10, false));
			rewards.Add(new Reward(0, 11, false));
			rewards.Add(new Reward(0, 12, false));
			rewards.Add(new Reward(0, 13, false));
			rewards.Add(new Reward(0, 14, false));
			rewards.Add(new Reward(0, 15, false));
			rewards.Add(new Reward(0, 16, false));
			rewards.Add(new Reward(0, 17, true));
			rewards.Add(new Reward(0, 18, true));
			rewards.Add(new Reward(0, 19, true));
			rewards.Add(new Reward(0, 20, true));
			rewards.Add(new Reward(0, 21, true));
			rewards.Add(new Reward(0, 22, true));
			rewards.Add(new Reward(0, 24, true));
			rewards.Add(new Reward(0, 25, true));
			rewards.Add(new Reward(0, 26, true));
			rewards.Add(new Reward(0, 28, false));
			rewards.Add(new Reward(0, 30, false));
			rewards.Add(new Reward(0, 31, false));
			rewards.Add(new Reward(0, 32, false));
			rewards.Add(new Reward(0, 33, false));
			rewards.Add(new Reward(0, 34, false));
			rewards.Add(new Reward(0, 35, false));
			rewards.Add(new Reward(0, 36, false));
			rewards.Add(new Reward(0, 37, false));
			rewards.Add(new Reward(0, 38, true));
			rewards.Add(new Reward(0, 101, false));
			rewards.Add(new Reward(0, 102, false));
			rewards.Add(new Reward(0, 103, false));
			rewards.Add(new Reward(0, 104, false));
			rewards.Add(new Reward(0, 105, false));
			rewards.Add(new Reward(0, 106, false));
			rewards.Add(new Reward(0, 107, false));
			rewards.Add(new Reward(0, 108, false));
			rewards.Add(new Reward(0, 201, false));
			rewards.Add(new Reward(0, 202, false));
			rewards.Add(new Reward(0, 203, false));
			rewards.Add(new Reward(0, 204, false));
			rewards.Add(new Reward(0, 301, false));
			rewards.Add(new Reward(1, 1, false));
			rewards.Add(new Reward(1, 3, true));
			rewards.Add(new Reward(1, 4, false));
			rewards.Add(new Reward(1, 5, false));
			rewards.Add(new Reward(1, 7, false));
			rewards.Add(new Reward(1, 8, false));
			rewards.Add(new Reward(1, 9, false));
			rewards.Add(new Reward(1, 10, false));
			rewards.Add(new Reward(1, 11, false));
			rewards.Add(new Reward(1, 12, false));
			rewards.Add(new Reward(1, 14, true));
			rewards.Add(new Reward(1, 15, false));
			rewards.Add(new Reward(1, 16, false));
			rewards.Add(new Reward(1, 101, false));
			rewards.Add(new Reward(1, 201, false));
			rewards.Add(new Reward(1, 202, false));
			rewards.Add(new Reward(1, 203, false));
			rewards.Add(new Reward(2, 1, true));
			rewards.Add(new Reward(2, 2, false));
			rewards.Add(new Reward(2, 3, true));
			rewards.Add(new Reward(2, 4, true));
			rewards.Add(new Reward(2, 5, true));
			rewards.Add(new Reward(2, 7, true));
			rewards.Add(new Reward(2, 10, true));
			rewards.Add(new Reward(3, 1, false));
			rewards.Add(new Reward(3, 2, false));
			rewards.Add(new Reward(3, 3, false));
			rewards.Add(new Reward(3, 4, false));
			rewards.Add(new Reward(3, 5, false));
			rewards.Add(new Reward(3, 6, false));
			rewards.Add(new Reward(3, 7, false));
			rewards.Add(new Reward(3, 10, false));
			rewards.Add(new Reward(3, 11, false));
			rewards.Add(new Reward(3, 101, false));
			rewards.Add(new Reward(3, 201, true));
			for (int i = 1; i <= 44; i++)
			{
				rewards.Add(new Reward(4, i, true));
			}
			rewards.Add(new Reward(5, 1, true));
			rewards.Add(new Reward(5, 2, true));
			rewards.Add(new Reward(5, 3, true));
			rewards.Add(new Reward(5, 4, true));
			rewards.Add(new Reward(5, 6, true));
			rewards.Add(new Reward(5, 7, true));
			rewards.Add(new Reward(5, 8, true));
			rewards.Add(new Reward(5, 10, true));
			rewards.Add(new Reward(5, 11, true));
			rewards.Add(new Reward(5, 12, true));
			rewards.Add(new Reward(5, 13, true));
			rewards.Add(new Reward(5, 14, true));
			rewards.Add(new Reward(5, 19, true));
			rewards.Add(new Reward(5, 20, true));
			rewards.Add(new Reward(5, 31, true));
			rewards.Add(new Reward(5, 32, true));
			rewards.Add(new Reward(5, 33, true));
			rewards.Add(new Reward(5, 34, true));
			rewards.Add(new Reward(5, 35, true));
			rewards.Add(new Reward(5, 37, true));
			rewards.Add(new Reward(5, 38, true));
			rewards.Add(new Reward(5, 39, true));
			rewards.Add(new Reward(5, 40, true));
			rewards.Add(new Reward(5, 41, false));
			rewards.Add(new Reward(5, 44, false));
			rewards.Add(new Reward(5, 45, false));
			rewards.Add(new Reward(5, 46, false));
			rewards.Add(new Reward(5, 47, false));
			rewards.Add(new Reward(5, 48, false));
			rewards.Add(new Reward(5, 49, false));
			rewards.Add(new Reward(5, 50, false));
			rewards.Add(new Reward(5, 51, false));
			rewards.Add(new Reward(5, 52, false));
			rewards.Add(new Reward(5, 53, false));
			rewards.Add(new Reward(5, 54, false));
			rewards.Add(new Reward(5, 55, false));
			rewards.Add(new Reward(5, 56, false));
			rewards.Add(new Reward(5, 57, true));
			rewards.Add(new Reward(5, 58, true));
			rewards.Add(new Reward(5, 59, true));
			rewards.Add(new Reward(5, 60, true));
			rewards.Add(new Reward(5, 61, true));
			rewards.Add(new Reward(5, 62, true));
			rewards.Add(new Reward(5, 63, true));
			rewards.Add(new Reward(5, 64, true));
			rewards.Add(new Reward(5, 65, true));
			rewards.Add(new Reward(5, 66, true));
			rewards.Add(new Reward(5, 67, true));
			rewards.Add(new Reward(5, 68, true));
			rewards.Add(new Reward(5, 69, true));
			rewards.Add(new Reward(5, 70, true));
			rewards.Add(new Reward(5, 71, true));
			rewards.Add(new Reward(5, 72, true));
			rewards.Add(new Reward(5, 75, true));
			rewards.Add(new Reward(5, 79, true));
			rewards.Add(new Reward(5, 80, true));
			rewards.Add(new Reward(5, 81, true));
			rewards.Add(new Reward(5, 101, false));
			rewards.Add(new Reward(5, 102, false));
			rewards.Add(new Reward(5, 103, false));
			rewards.Add(new Reward(5, 104, false));
			rewards.Add(new Reward(5, 105, false));
			rewards.Add(new Reward(5, 106, true));
			rewards.Add(new Reward(5, 107, true));
			rewards.Add(new Reward(5, 108, true));
			rewards.Add(new Reward(5, 109, true));
			rewards.Add(new Reward(5, 110, true));
			rewards.Add(new Reward(5, 201, true));
			rewards.Add(new Reward(5, 202, true));
			rewards.Add(new Reward(5, 203, true));
			rewards.Add(new Reward(5, 204, true));
			rewards.Add(new Reward(5, 301, true));
			for (int j = 1; j <= 38; j++)
			{
				rewards.Add(new Reward(6, j, true));
			}
			for (int k = 0; k < 6; k++)
			{
				rewards.Add(new Reward(7, 0, false));
			}
			for (int l = 0; l < 6; l++)
			{
				rewards.Add(new Reward(8, 0, false));
			}
			for (int m = 0; m < 7; m++)
			{
				rewards.Add(new Reward(9, 0, true));
			}
			rewards.Add(new Reward(10, 500, false));
			rewards.Add(new Reward(10, 1000, false));
			rewards.Add(new Reward(10, 2000, false));
			rewards.Add(new Reward(10, 5000, false));
			rewards.Add(new Reward(10, 10000, false));
			rewards.Add(new Reward(10, 500, false));
			rewards.Add(new Reward(10, 1000, false));
			rewards.Add(new Reward(10, 1000, false));
			rewards.Add(new Reward(10, 3000, false));
			rewards.Add(new Reward(10, 5000, false));
			rewards.Add(new Reward(10, 250, false));
			rewards.Add(new Reward(10, 500, false));
			rewards.Add(new Reward(10, 750, false));
			rewards.Add(new Reward(10, 1000, false));
			rewards.Add(new Reward(10, 1250, false));
			rewards.Add(new Reward(10, 1500, false));
			rewards.Add(new Reward(10, 1750, false));
			rewards.Add(new Reward(10, 2000, false));
			rewards.Add(new Reward(10, 2500, false));
			rewards.Add(new Reward(10, 3000, false));
			rewards.Add(new Reward(10, 5000, false));
			bool hardMode = Core.Randomizer.gameConfig.hardMode;
			rewards.Add(new Reward(10, hardMode ? 750 : 625, false));
			rewards.Add(new Reward(10, hardMode ? 3120 : 2600, false));
			rewards.Add(new Reward(10, hardMode ? 2520 : 2100, false));
			rewards.Add(new Reward(10, hardMode ? 6600 : 5500, false));
			rewards.Add(new Reward(10, hardMode ? 10800 : 9000, false));
			rewards.Add(new Reward(10, hardMode ? 5160 : 4300, false));
			rewards.Add(new Reward(10, hardMode ? 360 : 300, false));
			rewards.Add(new Reward(10, hardMode ? 13500 : 11250, false));
			rewards.Add(new Reward(10, hardMode ? 21600 : 18000, false));
			rewards.Add(new Reward(10, 18000, false));
			rewards.Add(new Reward(10, 18000, false));
			rewards.Add(new Reward(10, 18000, false));
			rewards.Add(new Reward(10, 18000, false));
			rewards.Add(new Reward(10, 30000, false));
			rewards.Add(new Reward(10, 5000, false));
		}

		public ForwardFiller(List<string> randoSettings, bool newGame)
		{
			this.randoSettings = randoSettings;
			this.newGame = newGame;
		}

		private Random random;

		private List<string> randoSettings;

		private bool newGame;
	}
}
