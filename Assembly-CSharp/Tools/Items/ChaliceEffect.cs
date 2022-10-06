using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using I2.Loc;

namespace Tools.Items
{
	public class ChaliceEffect : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			this.ClearEvents();
			this.SuscribeToEvents();
			return base.OnApplyEffect();
		}

		private void ClearEvents()
		{
			SpawnManager.OnTeleport -= this.OnTeleport;
			SpawnManager.OnTeleportPrieDieu -= this.OnTeleportPrieDieu;
			Entity.Death -= this.OnEntityDead;
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			Core.Events.OnFlagChanged -= this.OnFlagChanged;
		}

		private void SuscribeToEvents()
		{
			SpawnManager.OnTeleport += this.OnTeleport;
			SpawnManager.OnTeleportPrieDieu += this.OnTeleportPrieDieu;
			Entity.Death += this.OnEntityDead;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			Core.Events.OnFlagChanged += this.OnFlagChanged;
		}

		private void OnFlagChanged(string flag, bool flagactive)
		{
			if (flag == "D01Z05S23_CHALICEPUZZLE" && flagactive)
			{
				this.ClearEvents();
			}
		}

		protected override void OnRemoveEffect()
		{
			this.ClearEvents();
			base.OnRemoveEffect();
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			if ((newLevel == null || newLevel.LevelName.StartsWith("Main")) && (oldLevel == null || !oldLevel.LevelName.StartsWith("Main")))
			{
				this.ClearEvents();
			}
			else if (ChaliceEffect.ShouldUnfillChalice)
			{
				ChaliceEffect.ShouldUnfillChalice = false;
				this.ClearEnemiesFlags();
				this.ClearEvents();
				this.UnfillChalice();
			}
		}

		private void OnTeleport(string spawnId)
		{
			bool flag = false;
			foreach (TeleportDestination teleportDestination in Core.SpawnManager.GetAllUIActiveTeleports())
			{
				if (teleportDestination.teleportName.Equals(spawnId))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			LevelManager.OnLevelLoaded += this.OnLevelLoadedTeleport;
		}

		private void OnTeleportPrieDieu(string spawnId)
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoadedTeleport;
		}

		private void OnLevelLoadedTeleport(Level oldLevel, Level newLevel)
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoadedTeleport;
			if (newLevel.LevelName.StartsWith("D19Z01") || newLevel.LevelName.StartsWith("D13Z01") || newLevel.LevelName.StartsWith("D18Z01") || oldLevel.LevelName.StartsWith("D19Z01") || oldLevel.LevelName.StartsWith("D13Z01") || oldLevel.LevelName.StartsWith("D18Z01"))
			{
				return;
			}
			this.ClearEnemiesFlags();
			this.ClearEvents();
			this.UnfillChalice();
		}

		private void OnEntityDead(Entity entity)
		{
			Enemy enemy = entity as Enemy;
			if (enemy)
			{
				this.CheckAndFillChalice(enemy);
			}
			else
			{
				Penitent penitent = entity as Penitent;
				if (!penitent)
				{
					return;
				}
				if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE) || Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH))
				{
					return;
				}
				this.ClearEnemiesFlags();
				this.ClearEvents();
				this.UnfillChalice();
			}
		}

		private void ClearEnemiesFlags()
		{
			foreach (string text in this.EnemiesNames)
			{
				Core.Events.SetFlag(text.ToUpper() + "_CHALICE_DEAD", false, false);
			}
		}

		private void UnfillChalice()
		{
			string currentChaliceId = this.CurrentChaliceId;
			if (currentChaliceId != null)
			{
				if (!(currentChaliceId == "QI75"))
				{
					if (!(currentChaliceId == "QI76"))
					{
						if (currentChaliceId == "QI77")
						{
							Core.InventoryManager.RemoveQuestItem(Core.InventoryManager.GetQuestItem("QI77"));
							Core.InventoryManager.AddQuestItem(Core.InventoryManager.GetQuestItem("QI75"));
							UIController.instance.ShowPopUp(ScriptLocalization.UI_Inventory.TEXT_QI76_OR_QI77_UNFILLS, string.Empty, 5f, false);
						}
					}
					else
					{
						Core.InventoryManager.RemoveQuestItem(Core.InventoryManager.GetQuestItem("QI76"));
						Core.InventoryManager.AddQuestItem(Core.InventoryManager.GetQuestItem("QI75"));
						UIController.instance.ShowPopUp(ScriptLocalization.UI_Inventory.TEXT_QI76_OR_QI77_UNFILLS, string.Empty, 5f, false);
					}
				}
			}
		}

		private void CheckAndFillChalice(Enemy enemy)
		{
			foreach (string text in this.EnemiesNames)
			{
				if (enemy.name.StartsWith(text))
				{
					if (!Core.Events.GetFlag(text.ToUpper() + "_CHALICE_DEAD"))
					{
						Core.Events.SetFlag(text.ToUpper() + "_CHALICE_DEAD", true, false);
						string currentChaliceId = this.CurrentChaliceId;
						if (currentChaliceId != null)
						{
							if (!(currentChaliceId == "QI75"))
							{
								if (!(currentChaliceId == "QI76"))
								{
									if (!(currentChaliceId == "QI77"))
									{
									}
								}
								else
								{
									bool flag = true;
									foreach (string text2 in this.EnemiesNames)
									{
										if (!Core.Events.GetFlag(text2.ToUpper() + "_CHALICE_DEAD"))
										{
											flag = false;
											break;
										}
									}
									if (flag)
									{
										this.ClearEvents();
										Core.InventoryManager.RemoveQuestItem(Core.InventoryManager.GetQuestItem("QI76"));
										Core.InventoryManager.AddQuestItem(Core.InventoryManager.GetQuestItem("QI77"));
										UIController.instance.ShowPopUp(ScriptLocalization.UI_Inventory.TEXT_QI76_FILLS, string.Empty, 5f, false);
									}
									else
									{
										UIController.instance.ShowPopUp(ScriptLocalization.UI_Inventory.TEXT_QI75_FILLS, string.Empty, 5f, false);
									}
								}
							}
							else
							{
								this.ClearEvents();
								Core.InventoryManager.RemoveQuestItem(Core.InventoryManager.GetQuestItem("QI75"));
								Core.InventoryManager.AddQuestItem(Core.InventoryManager.GetQuestItem("QI76"));
								UIController.instance.ShowPopUp(ScriptLocalization.UI_Inventory.TEXT_QI75_FILLS, string.Empty, 5f, false);
							}
						}
					}
					break;
				}
			}
		}

		public List<string> EnemiesNames;

		public string CurrentChaliceId;

		public const string FLAGNAME_PUZZLE_COMPLETED = "D01Z05S23_CHALICEPUZZLE";

		private const string FIRST_CHALICE_ID = "QI75";

		private const string SECOND_CHALICE_ID = "QI76";

		private const string THIRD_CHALICE_ID = "QI77";

		public static bool ShouldUnfillChalice;
	}
}
