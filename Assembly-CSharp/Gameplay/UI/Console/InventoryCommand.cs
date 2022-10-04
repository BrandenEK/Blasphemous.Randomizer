using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Framework.FrameworkCore.Attributes;
using Framework.Inventory;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class InventoryCommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			switch (command)
			{
			case "relic":
				this.ParseRelic(subcommand, paramList);
				break;
			case "questitem":
				this.ParseQuest(subcommand, paramList);
				break;
			case "collectible":
				this.ParseCollectible(subcommand, paramList);
				break;
			case "bead":
				this.ParseBead(subcommand, paramList);
				break;
			case "prayer":
				this.ParsePrayer(subcommand, paramList);
				break;
			case "sword":
				this.ParseSword(subcommand, paramList);
				break;
			case "key":
				this.ParseKey(subcommand, paramList);
				break;
			case "invtest":
				base.Console.Write("Adding all items:");
				Core.InventoryManager.TestAddAllObjects();
				break;
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"relic",
				"questitem",
				"bead",
				"prayer",
				"collectible",
				"invtest",
				"sword",
				"key"
			};
		}

		private void ParseRelic(string command, List<string> paramList)
		{
			string command2 = "relic " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available RELIC commands:");
					base.Console.Write("relic list: List all relics");
					base.Console.Write("relic listowned: List all relics owned by player");
					base.Console.Write("relic add [IDRELIC|all]: Add a relic (or all)");
					base.Console.Write("relic remove [IDRELIC|all]: Remove the relic (or all)");
					base.Console.Write("relic equiped: Show the eqquiped relics");
					base.Console.Write("relic equip IDRELIC SLOT: Equip the relic in the slot");
					base.Console.Write("relic unequip SLOT: Unequip the relic in the slot");
				}
				return;
			case "list":
				if (base.ValidateParams(command2, 0, paramList))
				{
					this.ListInventoryList<Relic>("All relics:", Core.InventoryManager.GetAllRelics());
				}
				return;
			case "listowned":
				if (base.ValidateParams(command2, 0, paramList))
				{
					this.ListInventoryList<Relic>("Owned relics:", Core.InventoryManager.GetRelicsOwned());
				}
				return;
			case "add":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (paramList[0].ToLower() == "all")
					{
						Core.InventoryManager.AddAll(InventoryManager.ItemType.Relic);
						base.Console.Write("Adding all relics");
					}
					else if (this.CheckInvObject("relic", paramList[0], Core.InventoryManager.GetRelic(paramList[0])))
					{
						base.WriteCommandResult("Add relic", Core.InventoryManager.AddRelic(paramList[0]));
					}
				}
				return;
			case "remove":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (paramList[0].ToLower() == "all")
					{
						Core.InventoryManager.RemoveAll(InventoryManager.ItemType.Relic);
						base.Console.Write("Removing all relics");
					}
					else if (this.CheckInvObject("relic", paramList[0], Core.InventoryManager.GetRelic(paramList[0])))
					{
						base.WriteCommandResult("Remove relic", Core.InventoryManager.RemoveRelic(paramList[0]));
					}
				}
				return;
			case "equiped":
				base.Console.Write("Relics slots");
				for (int i = 0; i < 3; i++)
				{
					Relic relicInSlot = Core.InventoryManager.GetRelicInSlot(i);
					string text = "Slot " + i.ToString() + ": ";
					if (relicInSlot)
					{
						text = text + relicInSlot.id + "  - " + relicInSlot.caption;
					}
					else
					{
						text += "empty";
					}
					base.Console.Write(text);
				}
				return;
			case "equip":
			{
				int slot;
				if (base.ValidateParams(command2, 2, paramList) && this.CheckInvObject("relic", paramList[0], Core.InventoryManager.GetRelic(paramList[0])) && base.ValidateParam(paramList[1], out slot, 0, 2))
				{
					base.WriteCommandResult("Equip relic", Core.InventoryManager.SetRelicInSlot(slot, paramList[0]));
				}
				return;
			}
			case "unequip":
			{
				int slot2;
				if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out slot2, 0, 2))
				{
					Relic relic = null;
					base.WriteCommandResult("Unrquip relic", Core.InventoryManager.SetRelicInSlot(slot2, relic));
				}
				return;
			}
			}
			base.Console.Write("Command unknow, use relic help");
		}

		private void ParseQuest(string command, List<string> paramList)
		{
			string command2 = "questitem " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available QUEST ITEM commands:");
						base.Console.Write("questitem list: List all quest items");
						base.Console.Write("questitem listowned: List all quest items owned by player");
						base.Console.Write("questitem add [IDQUESTITEM|all]: Add a quest item (or all)");
						base.Console.Write("questitem remove [IDQUESTITEM|all]: Remove the quest item (or all)");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						this.ListInventoryList<QuestItem>("All quest items:", Core.InventoryManager.GetAllQuestItems());
					}
					return;
				}
				if (command == "listowned")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						this.ListInventoryList<QuestItem>("Owned quest items:", Core.InventoryManager.GetQuestItemOwned());
					}
					return;
				}
				if (command == "add")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						if (paramList[0].ToLower() == "all")
						{
							Core.InventoryManager.AddAll(InventoryManager.ItemType.Quest);
							base.Console.Write("Adding all quest items");
						}
						else if (this.CheckInvObject("quest item", paramList[0], Core.InventoryManager.GetQuestItem(paramList[0])))
						{
							base.WriteCommandResult("Add quest item", Core.InventoryManager.AddQuestItem(paramList[0]));
						}
					}
					return;
				}
				if (command == "remove")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						if (paramList[0].ToLower() == "all")
						{
							Core.InventoryManager.RemoveAll(InventoryManager.ItemType.Quest);
							base.Console.Write("Removing all quest items");
						}
						else if (this.CheckInvObject("quest item", paramList[0], Core.InventoryManager.GetQuestItem(paramList[0])))
						{
							base.WriteCommandResult("Remove quest item", Core.InventoryManager.RemoveQuestItem(paramList[0]));
						}
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use questitem help");
		}

		private void ParseCollectible(string command, List<string> paramList)
		{
			string command2 = "collectible " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available COLLECTIBLE commands:");
						base.Console.Write("collectible list: List all collectible items");
						base.Console.Write("collectible listowned: List all collectible items owned by player");
						base.Console.Write("collectible add [IDCOLLECTIBLEITEM|all]: Add a collectible item (or all)");
						base.Console.Write("collectible remove [IDCOLLECTIBLEITEM|all]: Remove the collectible item (or all)");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						this.ListInventoryList<Framework.Inventory.CollectibleItem>("All collectible items:", Core.InventoryManager.GetAllCollectibleItems());
					}
					return;
				}
				if (command == "listowned")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						this.ListInventoryList<Framework.Inventory.CollectibleItem>("Owned collectible items:", Core.InventoryManager.GetCollectibleItemOwned());
					}
					return;
				}
				if (command == "add")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						if (paramList[0].ToLower() == "all")
						{
							Core.InventoryManager.AddAll(InventoryManager.ItemType.Collectible);
							base.Console.Write("Adding all collectibles");
						}
						else if (this.CheckInvObject("collectible item", paramList[0], Core.InventoryManager.GetCollectibleItem(paramList[0])))
						{
							base.WriteCommandResult("Add collectible item", Core.InventoryManager.AddCollectibleItem(paramList[0]));
						}
					}
					return;
				}
				if (command == "remove")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						if (paramList[0].ToLower() == "all")
						{
							Core.InventoryManager.RemoveAll(InventoryManager.ItemType.Collectible);
							base.Console.Write("Removing all collectibles");
						}
						else if (this.CheckInvObject("collectible item", paramList[0], Core.InventoryManager.GetCollectibleItem(paramList[0])))
						{
							base.WriteCommandResult("Remove collectible item", Core.InventoryManager.RemoveCollectibleItem(paramList[0]));
						}
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use collectible help");
		}

		private void ParseKey(string command, List<string> paramList)
		{
			string command2 = "key " + command;
			int slot = 0;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available KEY commands:");
						base.Console.Write("key list: List all key status");
						base.Console.Write("key add [idx|all]: Add the idx key (or all)");
						base.Console.Write("key remove [idx|all]: Remove the idx key (or all)");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Keys:");
						for (int i = 0; i < 4; i++)
						{
							string message = "Slot " + i.ToString() + ": " + Core.InventoryManager.CheckBossKey(i).ToString();
							base.Console.Write(message);
						}
					}
					return;
				}
				if (command == "add")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						if (paramList[0].ToLower() == "all")
						{
							for (int j = 0; j < 4; j++)
							{
								Core.InventoryManager.AddBossKey(j);
							}
							base.Console.Write("Adding all Boss Keys");
						}
						else if (base.ValidateParam(paramList[0], out slot, 0, 3))
						{
							Core.InventoryManager.AddBossKey(slot);
							base.Console.Write("BossKey added");
						}
					}
					return;
				}
				if (command == "remove")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						if (paramList[0].ToLower() == "all")
						{
							for (int k = 0; k < 4; k++)
							{
								Core.InventoryManager.RemoveBossKey(k);
							}
							base.Console.Write("Removing all Boss Keys");
						}
						else if (base.ValidateParam(paramList[0], out slot, 0, 3))
						{
							Core.InventoryManager.RemoveBossKey(slot);
							base.Console.Write("BossKey removed");
						}
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use key help");
		}

		private void ParseBead(string command, List<string> paramList)
		{
			string command2 = "bead " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available BEADS commands:");
					base.Console.Write("bead list: List all beads");
					base.Console.Write("bead listowned: List all beads owned by player");
					base.Console.Write("bead setslots SLOTS: Sets the beads slots");
					base.Console.Write("bead add [IDBEAD|all]: Add a bead (or all)");
					base.Console.Write("bead remove [IDBEAD|all]: Remove the bead (or all)");
					base.Console.Write("bead equiped: List the eqquiped beads");
					base.Console.Write("bead equip IDBEAD SLOT: Equip the bead in the slot");
					base.Console.Write("bead unequip SLOT: Unequip the bead in the slot");
				}
				return;
			case "list":
				if (base.ValidateParams(command2, 0, paramList))
				{
					this.ListInventoryList<RosaryBead>("All beads:", Core.InventoryManager.GetAllRosaryBeads());
				}
				return;
			case "listowned":
				if (base.ValidateParams(command2, 0, paramList))
				{
					this.ListInventoryList<RosaryBead>("Owned beads:", Core.InventoryManager.GetRosaryBeadOwned());
				}
				return;
			case "add":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (paramList[0].ToLower() == "all")
					{
						Core.InventoryManager.AddAll(InventoryManager.ItemType.Bead);
						base.Console.Write("Adding all rosary beads");
					}
					else if (this.CheckInvObject("rosary bead", paramList[0], Core.InventoryManager.GetRosaryBead(paramList[0])))
					{
						base.WriteCommandResult(command2, Core.InventoryManager.AddRosaryBead(paramList[0]));
					}
				}
				return;
			case "setslots":
			{
				int num2;
				if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out num2, 2, 7))
				{
					BeadSlots beadSlots = base.Penitent.Stats.BeadSlots;
					beadSlots.SetPermanentBonus((float)num2);
					base.WriteCommandResult(command2, true);
				}
				return;
			}
			case "remove":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (paramList[0].ToLower() == "all")
					{
						Core.InventoryManager.RemoveAll(InventoryManager.ItemType.Bead);
						base.Console.Write("Removing all rosary beads");
					}
					else if (this.CheckInvObject("rosary bead", paramList[0], Core.InventoryManager.GetRosaryBead(paramList[0])))
					{
						base.WriteCommandResult(command2, Core.InventoryManager.RemoveRosaryBead(paramList[0]));
					}
				}
				return;
			case "equiped":
				base.Console.Write("Rosary Beads slots");
				for (int i = 0; i < Core.InventoryManager.GetRosaryBeadSlots(); i++)
				{
					RosaryBead rosaryBeadInSlot = Core.InventoryManager.GetRosaryBeadInSlot(i);
					string text = "Slot " + i.ToString() + ": ";
					if (rosaryBeadInSlot)
					{
						text = text + rosaryBeadInSlot.id + "  - " + rosaryBeadInSlot.caption;
					}
					else
					{
						text += "empty";
					}
					base.Console.Write(text);
				}
				return;
			case "equip":
			{
				int slot;
				if (base.ValidateParams(command2, 2, paramList) && this.CheckInvObject("rosary bead", paramList[0], Core.InventoryManager.GetRosaryBead(paramList[0])) && base.ValidateParam(paramList[1], out slot, 0, Core.InventoryManager.GetRosaryBeadSlots() - 1))
				{
					base.WriteCommandResult(command2, Core.InventoryManager.SetRosaryBeadInSlot(slot, paramList[0]));
				}
				return;
			}
			case "unequip":
			{
				int slot2;
				if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out slot2, 0, Core.InventoryManager.GetRosaryBeadSlots() - 1))
				{
					RosaryBead bead = null;
					base.WriteCommandResult(command2, Core.InventoryManager.SetRosaryBeadInSlot(slot2, bead));
				}
				return;
			}
			}
			base.Console.Write("Command unknow, use bead help");
		}

		private void ParseSword(string command, List<string> paramList)
		{
			string command2 = "sword " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available SWORD commands:");
					base.Console.Write("sword list: List all sword");
					base.Console.Write("sword listowned: List all sword owned by player");
					base.Console.Write("sword add [IDSWORD|all]: Add a sword (or all)");
					base.Console.Write("sword remove [IDSWORD|all]: Remove the sword (or all)");
					base.Console.Write("sword equiped: Show the eqquiped swords");
					base.Console.Write("sword equip IDSWORD SLOT: Equip the sword in the slot");
					base.Console.Write("sword unequip SLOT: Unequip the sword in the slot");
				}
				return;
			case "list":
				if (base.ValidateParams(command2, 0, paramList))
				{
					this.ListInventoryList<Sword>("All swords:", Core.InventoryManager.GetAllSwords());
				}
				return;
			case "listowned":
				if (base.ValidateParams(command2, 0, paramList))
				{
					this.ListInventoryList<Sword>("Owned swords:", Core.InventoryManager.GetSwordsOwned());
				}
				return;
			case "add":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (paramList[0].ToLower() == "all")
					{
						Core.InventoryManager.AddAll(InventoryManager.ItemType.Sword);
						base.Console.Write("Adding all swords");
					}
					else if (this.CheckInvObject("sword", paramList[0], Core.InventoryManager.GetSword(paramList[0])))
					{
						base.WriteCommandResult("Add sword", Core.InventoryManager.AddSword(paramList[0]));
					}
				}
				return;
			case "remove":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (paramList[0].ToLower() == "all")
					{
						Core.InventoryManager.RemoveAll(InventoryManager.ItemType.Sword);
						base.Console.Write("Removing all swords");
					}
					else if (this.CheckInvObject("sword", paramList[0], Core.InventoryManager.GetSword(paramList[0])))
					{
						base.WriteCommandResult("Remove sword", Core.InventoryManager.RemoveSword(paramList[0]));
					}
				}
				return;
			case "equiped":
				base.Console.Write("Sword slots");
				for (int i = 0; i < 1; i++)
				{
					Sword swordInSlot = Core.InventoryManager.GetSwordInSlot(i);
					string text = "Slot " + i.ToString() + ": ";
					if (swordInSlot)
					{
						text = text + swordInSlot.id + "  - " + swordInSlot.caption;
					}
					else
					{
						text += "empty";
					}
					base.Console.Write(text);
				}
				return;
			case "equip":
			{
				int slot;
				if (base.ValidateParams(command2, 2, paramList) && this.CheckInvObject("sword", paramList[0], Core.InventoryManager.GetSword(paramList[0])) && base.ValidateParam(paramList[1], out slot, 0, 0))
				{
					base.WriteCommandResult("Equip sword", Core.InventoryManager.SetSwordInSlot(slot, paramList[0]));
				}
				return;
			}
			case "unequip":
			{
				int slot2;
				if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out slot2, 0, 0))
				{
					Sword sword = null;
					base.WriteCommandResult("Unrquip sword", Core.InventoryManager.SetSwordInSlot(slot2, sword));
				}
				return;
			}
			}
			base.Console.Write("Command unknow, use relic help");
		}

		private void ParsePrayer(string command, List<string> paramList)
		{
			string command2 = "prayer " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available PRAYER commands:");
					base.Console.Write("prayer list: List all prayers");
					base.Console.Write("prayer listowned: List all prayers owned by player");
					base.Console.Write("prayer add [IDPRAYER|all]: Add a prayer (or all)");
					base.Console.Write("prayer remove [IDPRAYER|all]: Remove the prayer (or all)");
					base.Console.Write("prayer equiped: List the eqquiped prayers");
					base.Console.Write("prayer equip IDPRAYER SLOT: Equip the prayer");
					base.Console.Write("prayer unequip SLOT: Unequip the prayer type");
					base.Console.Write("prayer decipher IDPRAYER NUMBER: Add decipher");
				}
				return;
			case "list":
				if (base.ValidateParams(command2, 0, paramList))
				{
					this.ListInventoryList<Prayer>("All prayers:", Core.InventoryManager.GetAllPrayers());
				}
				return;
			case "listowned":
				if (base.ValidateParams(command2, 0, paramList))
				{
					this.ListInventoryList<Prayer>("Owned prayers:", Core.InventoryManager.GetPrayersOwned());
				}
				return;
			case "add":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (paramList[0].ToLower() == "all")
					{
						Core.InventoryManager.AddAll(InventoryManager.ItemType.Prayer);
						base.Console.Write("Adding all prayers");
					}
					else if (this.CheckInvObject("prayer", paramList[0], Core.InventoryManager.GetPrayer(paramList[0])))
					{
						base.WriteCommandResult("Add prayer", Core.InventoryManager.AddPrayer(paramList[0]));
					}
				}
				return;
			case "remove":
				if (base.ValidateParams(command2, 1, paramList))
				{
					if (paramList[0].ToLower() == "all")
					{
						Core.InventoryManager.RemoveAll(InventoryManager.ItemType.Prayer);
						base.Console.Write("Removing all prayers");
					}
					else if (this.CheckInvObject("prayer", paramList[0], Core.InventoryManager.GetPrayer(paramList[0])))
					{
						base.WriteCommandResult("Remove prayer", Core.InventoryManager.RemovePrayer(paramList[0]));
					}
				}
				return;
			case "equiped":
				base.Console.Write("Prayer slots");
				for (int i = 0; i < 1; i++)
				{
					Prayer prayerInSlot = Core.InventoryManager.GetPrayerInSlot(i);
					string text = "Slot " + i.ToString() + ": ";
					if (prayerInSlot)
					{
						text = text + prayerInSlot.id + "  - " + prayerInSlot.caption;
					}
					else
					{
						text += "empty";
					}
					base.Console.Write(text);
				}
				return;
			case "equip":
			{
				int num2;
				if (base.ValidateParams(command2, 2, paramList) && this.CheckInvObject("prayer", paramList[0], Core.InventoryManager.GetPrayer(paramList[0])) && base.ValidateParam(paramList[1], out num2, 0, 0))
				{
					Prayer prayer = Core.InventoryManager.GetPrayer(paramList[0]);
					Core.InventoryManager.AddPrayer(paramList[0]);
					if (prayer == null)
					{
						return;
					}
					prayer.AddDecipher(prayer.decipherMax);
					Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
					bool result = Core.InventoryManager.SetPrayerInSlot(0, prayer);
					base.WriteCommandResult(command2, result);
				}
				return;
			}
			case "unequip":
			{
				int slot;
				if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out slot, 0, 0))
				{
					Prayer prayer2 = null;
					base.WriteCommandResult(command2, Core.InventoryManager.SetPrayerInSlot(slot, prayer2));
				}
				return;
			}
			case "decipher":
				if (base.ValidateParams(command2, 2, paramList))
				{
					Prayer prayer3 = Core.InventoryManager.GetPrayer(paramList[0]);
					int number;
					if (this.CheckInvObject("prayer", paramList[0], prayer3) && base.ValidateParam(paramList[1], out number, 1, 20))
					{
						bool flag = !prayer3.IsDeciphered();
						if (flag)
						{
							prayer3.AddDecipher(number);
						}
						base.WriteCommandResult("Decipher prayer", flag);
					}
				}
				return;
			}
			base.Console.Write("Command unknow, use prayer help");
		}

		private bool GetParamPrayerType(string param, out Prayer.PrayerType prayerType)
		{
			bool result = true;
			prayerType = Prayer.PrayerType.Hymn;
			string text = param.ToLower();
			if (text != null)
			{
				if (text == "hymn")
				{
					prayerType = Prayer.PrayerType.Hymn;
					return result;
				}
				if (text == "lament")
				{
					prayerType = Prayer.PrayerType.Laments;
					return result;
				}
				if (text == "thanks")
				{
					prayerType = Prayer.PrayerType.Thanksgiving;
					return result;
				}
			}
			base.Console.Write("The prayer type must be hymn, lament, or thanks");
			result = false;
			return result;
		}

		private void ListInventoryList<T>(string caption, ReadOnlyCollection<T> list) where T : BaseInventoryObject
		{
			base.Console.Write(caption);
			foreach (T t in list)
			{
				base.Console.Write(t.id + " - " + t.caption);
			}
		}

		private bool CheckInvObject(string invType, string name, BaseInventoryObject invObj)
		{
			bool flag = invObj != null;
			if (!flag)
			{
				base.Console.Write(name + " is not a valid " + invType + " ID, please use list subcommand");
			}
			return flag;
		}
	}
}
