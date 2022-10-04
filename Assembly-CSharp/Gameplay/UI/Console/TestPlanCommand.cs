using System;
using System.Collections.Generic;
using Gameplay.UI.Widgets;

namespace Gameplay.UI.Console
{
	public class TestPlanCommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "testplan")
				{
					this.ParseTestPlan(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"testplan"
			};
		}

		private void ParseTestPlan(string command, List<string> paramList)
		{
			string command2 = "testplan " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available TESTPLAN commands:");
						base.Console.Write("1: Test plan command macro that includes progression tiers 1, 2 and 3");
					}
					return;
				}
				if (command == "1")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						ConsoleWidget.Instance.ProcessCommand("prayer add pr14");
						ConsoleWidget.Instance.ProcessCommand("bead add rb01");
						ConsoleWidget.Instance.ProcessCommand("bead add rb42");
						ConsoleWidget.Instance.ProcessCommand("relic add re04");
						ConsoleWidget.Instance.ProcessCommand("relic add re07");
						ConsoleWidget.Instance.ProcessCommand("relic add re01");
						ConsoleWidget.Instance.ProcessCommand("sword add he02");
						ConsoleWidget.Instance.ProcessCommand("sword add he03");
						ConsoleWidget.Instance.ProcessCommand("sword add he06");
						ConsoleWidget.Instance.ProcessCommand("sword add he10");
						ConsoleWidget.Instance.ProcessCommand("prayer add pr01");
						ConsoleWidget.Instance.ProcessCommand("prayer add pr15");
						ConsoleWidget.Instance.ProcessCommand("bead add rb04");
						ConsoleWidget.Instance.ProcessCommand("bead add rb05");
						ConsoleWidget.Instance.ProcessCommand("bead add rb07");
						ConsoleWidget.Instance.ProcessCommand("bead add rb08");
						ConsoleWidget.Instance.ProcessCommand("bead add rb09");
						ConsoleWidget.Instance.ProcessCommand("bead add rb13");
						ConsoleWidget.Instance.ProcessCommand("bead add rb15");
						ConsoleWidget.Instance.ProcessCommand("bead add rb17");
						ConsoleWidget.Instance.ProcessCommand("bead add rb22");
						ConsoleWidget.Instance.ProcessCommand("bead add rb24");
						ConsoleWidget.Instance.ProcessCommand("bead add rb32");
						ConsoleWidget.Instance.ProcessCommand("bead add rb37");
						ConsoleWidget.Instance.ProcessCommand("bead add rb38");
						ConsoleWidget.Instance.ProcessCommand("relic add re10");
						ConsoleWidget.Instance.ProcessCommand("sword add he05");
						ConsoleWidget.Instance.ProcessCommand("sword add he11");
						ConsoleWidget.Instance.ProcessCommand("prayer add pr03");
						ConsoleWidget.Instance.ProcessCommand("prayer add pr04");
						ConsoleWidget.Instance.ProcessCommand("prayer add pr10");
						ConsoleWidget.Instance.ProcessCommand("prayer add pr16");
						ConsoleWidget.Instance.ProcessCommand("bead add rb02");
						ConsoleWidget.Instance.ProcessCommand("bead add rb06");
						ConsoleWidget.Instance.ProcessCommand("bead add rb10");
						ConsoleWidget.Instance.ProcessCommand("bead add rb20");
						ConsoleWidget.Instance.ProcessCommand("bead add rb28");
						ConsoleWidget.Instance.ProcessCommand("questitem add qi63");
						ConsoleWidget.Instance.ProcessCommand("questitem add qi75");
						ConsoleWidget.Instance.ProcessCommand("flask upgrade");
						ConsoleWidget.Instance.ProcessCommand("flask upgrade");
						ConsoleWidget.Instance.ProcessCommand("flask upgrade");
						ConsoleWidget.Instance.ProcessCommand("flask upgrade");
						ConsoleWidget.Instance.ProcessCommand("health upgrade");
						ConsoleWidget.Instance.ProcessCommand("health upgrade");
						ConsoleWidget.Instance.ProcessCommand("health upgrade");
						ConsoleWidget.Instance.ProcessCommand("fervour upgrade");
						ConsoleWidget.Instance.ProcessCommand("fervour upgrade");
						ConsoleWidget.Instance.ProcessCommand("fervour upgrade");
						ConsoleWidget.Instance.ProcessCommand("meaculpa upgrade");
						ConsoleWidget.Instance.ProcessCommand("meaculpa upgrade");
						ConsoleWidget.Instance.ProcessCommand("meaculpa upgrade");
						ConsoleWidget.Instance.ProcessCommand("bead setslots 3");
						ConsoleWidget.Instance.ProcessCommand("skill unlock combo_1");
						ConsoleWidget.Instance.ProcessCommand("skill unlock combo_2");
						ConsoleWidget.Instance.ProcessCommand("skill unlock charged_1");
						ConsoleWidget.Instance.ProcessCommand("skill unlock charged_2");
						ConsoleWidget.Instance.ProcessCommand("skill unlock ranged_1");
						ConsoleWidget.Instance.ProcessCommand("skill unlock vertical_1");
						ConsoleWidget.Instance.ProcessCommand("skill unlock vertical_2");
						ConsoleWidget.Instance.ProcessCommand("skill unlock lunge_1");
						ConsoleWidget.Instance.ProcessCommand("skill unlock lunge_2");
						ConsoleWidget.Instance.ProcessCommand("flag set D17Z01_BOSSDEAD");
						ConsoleWidget.Instance.ProcessCommand("flag set D01Z06S01_BOSSDEAD");
						ConsoleWidget.Instance.ProcessCommand("flag set D02Z05S01_BOSSDEAD");
						ConsoleWidget.Instance.ProcessCommand("flag set D03Z04S01_BOSSDEAD");
						ConsoleWidget.Instance.ProcessCommand("flag set D08Z01S01_BOSSDEAD");
						ConsoleWidget.Instance.ProcessCommand("flag set CONFESSOR_1_DESTROYED");
						ConsoleWidget.Instance.ProcessCommand("flag set CONFESSOR_1_ARENACOMPLETED");
						ConsoleWidget.Instance.ProcessCommand("flag set CONFESSOR_2_DESTROYED");
						ConsoleWidget.Instance.ProcessCommand("flag set CONFESSOR_2_ARENACOMPLETED");
						ConsoleWidget.Instance.ProcessCommand("flag set CONFESSOR_3_DESTROYED");
						ConsoleWidget.Instance.ProcessCommand("flag set CONFESSOR_3_ARENACOMPLETED");
						ConsoleWidget.Instance.ProcessCommand("flag set CONFESSOR_4_DESTROYED");
						ConsoleWidget.Instance.ProcessCommand("flag set CONFESSOR_4_ARENACOMPLETED");
						ConsoleWidget.Instance.ProcessCommand("flag set ATTRITION_ALTAR_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set COMPUNCTION_ALTAR_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set CONTRITION_ALTAR_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set DEOSGRACIAS_CUTSCENE_PLAYED");
						ConsoleWidget.Instance.ProcessCommand("flag set DEOSGRACIAS_KEY_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set DEOSGRACIAS_ATTRITION_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set DEOSGRACIAS_CONTRITION_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set DEOSGRACIAS_COMPUNCTION_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set REDENTO_LOCATION_4");
						ConsoleWidget.Instance.ProcessCommand("flag set TIRSO_PIETY_FILTERED");
						ConsoleWidget.Instance.ProcessCommand("flag set TIRSO_ANGUISH_FILTERED");
						ConsoleWidget.Instance.ProcessCommand("flag set TIRSO_BURNTFACE_FILTERED");
						ConsoleWidget.Instance.ProcessCommand("flag set TIRSO_ITEMS_USED_3");
						ConsoleWidget.Instance.ProcessCommand("flag set TIRSO_QI19_DELIVERED");
						ConsoleWidget.Instance.ProcessCommand("flag set TIRSO_QI20_DELIVERED");
						ConsoleWidget.Instance.ProcessCommand("flag set TIRSO_QI37_DELIVERED");
						ConsoleWidget.Instance.ProcessCommand("flag set TIRSO_FIRSTCONVERSATION_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set LVDOVICO_QUEST_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set PENITENT_MET_LVDOVICO");
						ConsoleWidget.Instance.ProcessCommand("flag set DAGGER_ENCOUNTER_1_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set DAGGER_ENCOUNTER_2_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set DAGGER_ENCOUNTER_3_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set VIRIDIANA_DEAD");
						ConsoleWidget.Instance.ProcessCommand("flag set VIRIDIANA_DEAD_BOSS02");
						ConsoleWidget.Instance.ProcessCommand("flag set GEMINO_ENTRANCE_OPEN");
						ConsoleWidget.Instance.ProcessCommand("flag set GEMINO_TOMB_OPEN");
						ConsoleWidget.Instance.ProcessCommand("flag set BROTHERS_EVENT1_COMPLETED");
						ConsoleWidget.Instance.ProcessCommand("flag set BROTHERS_EVENT2_COMPLETED");
						ConsoleWidget.Instance.ProcessCommand("flag set BROTHERS_EVENTPERPETVUA_COMPLETED");
						ConsoleWidget.Instance.ProcessCommand("flag set CANDELARIA_FIRST_CONVERSATION_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set CANDELARIA_FAKEITEM_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set LOTL_1OFFERING_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set LOTL_2OFFERING_DONE");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_16");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_17");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_07");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_08");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_27");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_10");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_30");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_23");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_31");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_26");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_25");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_09");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_11");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_14");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_12");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_15");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_18");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_37");
						ConsoleWidget.Instance.ProcessCommand("flag set RESCUED_CHERUB_19");
						ConsoleWidget.Instance.ProcessCommand("flag set ALTASGRACIAS_EGG_BROKEN");
						ConsoleWidget.Instance.ProcessCommand("health fill");
						ConsoleWidget.Instance.ProcessCommand("flask fill");
						ConsoleWidget.Instance.ProcessCommand("purge set 0");
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use testplan help");
		}
	}
}
