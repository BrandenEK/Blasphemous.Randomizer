using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the chosen flags exists.")]
	public class FlagExists : FsmStateAction
	{
		public override void Reset()
		{
			this.outValue = new FsmBool
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			string text = this.flagName.Value.ToUpper().Replace(' ', '_');
			Core.Randomizer.Log("Check flag: " + text, 0);
			bool flag = Core.Events.GetFlag(text);
			string levelName = Core.LevelManager.currentLevel.LevelName;
			if (levelName == "D08Z01S01")
			{
				if (text == "D01Z06S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI38");
				}
				else if (text == "D02Z05S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI39");
				}
				else if (text == "D03Z04S01_BOSSDEAD")
				{
					flag = Core.InventoryManager.IsQuestItemOwned("QI40");
				}
			}
			if (levelName == "D01Z02S02" && text == "D09Z01S03_BOSSDEAD" && (!Core.Events.GetFlag("TIRSO_QI19_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI20_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI37_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI63_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI64_DELIVERED") || !Core.Events.GetFlag("TIRSO_QI65_DELIVERED")))
			{
				flag = false;
			}
			if (levelName == "D02Z01S01" && Core.Randomizer.gameConfig.items.disableNPCDeath && (text == "D01Z06S01_BOSSDEAD" || text == "D03Z04S01_BOSSDEAD" || text == "D08Z01S01_BOSSDEAD"))
			{
				flag = false;
			}
			if (levelName == "D04BZ02S01" && text == "REDENTO_QI54_USED")
			{
				flag = Core.Events.GetFlag("LOCATION_QI54");
			}
			if (levelName == "D03Z03S10" && text == "ALTASGRACIAS_LAST_REWARD")
			{
				flag = Core.Events.GetFlag("LOCATION_RB06");
			}
			if ((levelName == "D02Z01S01" || levelName == "D02Z01S04") && text == "GEMINO_RB10_REWARD")
			{
				flag = Core.Events.GetFlag("LOCATION_RB10");
			}
			if ((levelName == "D02Z01S04" || levelName == "D02Z01S01") && text == "GEMINO_OFFERING_DONE")
			{
				flag = Core.Events.GetFlag("LOCATION_QI68");
			}
			if (this.outValue != null)
			{
				this.outValue.Value = flag;
			}
			if (flag)
			{
				base.Fsm.Event(this.flagAvailable);
			}
			else
			{
				base.Fsm.Event(this.flagUnavailable);
			}
			base.Finish();
		}

		public FsmString category;

		public FsmString flagName;

		public bool runtimeFlag;

		public FsmEvent flagAvailable;

		public FsmEvent flagUnavailable;

		public FsmBool outValue;
	}
}
