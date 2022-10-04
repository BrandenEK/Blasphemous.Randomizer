using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Disables opening the Inventory and Unequips all items except sword heart.")]
	public class DisableInventoryAndUnequip : FsmStateAction
	{
		public override void OnEnter()
		{
			UIController.instance.CanOpenInventory = false;
			Core.InventoryManager.RemoveBeads();
			Core.InventoryManager.RemovePrayers();
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			base.Finish();
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			if (newLevel != oldLevel)
			{
				UIController.instance.CanOpenInventory = true;
			}
		}
	}
}
