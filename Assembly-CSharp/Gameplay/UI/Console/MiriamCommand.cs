using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class MiriamCommand : ConsoleCommand
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
				if (command == "miriam")
				{
					this.ParseMiriam(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"miriam"
			};
		}

		private void ParseMiriam(string command, List<string> paramList)
		{
			string command2 = "miriam " + command;
			switch (command)
			{
			case "help":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Available MIRIAM commands:");
					base.Console.Write("miriam status: Write the quest status");
					base.Console.Write("miriam start: Start miriam quest");
					base.Console.Write("miriam end: End miriam quest");
					base.Console.Write("miriam activateportal: Start the next miriam portal and teleport");
					base.Console.Write("miriam deactivateportal: End the current portal and teleport outside");
				}
				return;
			case "status":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.Console.Write("Miriam started: " + Core.Events.IsMiriamQuestStarted);
					base.Console.Write("Miriam finished: " + Core.Events.IsMiriamQuestFinished);
					base.Console.Write("Miriam current portal: " + Core.Events.MiriamCurrentScenePortal);
					ReadOnlyCollection<string> miriamClosedPortals = Core.Events.GetMiriamClosedPortals();
					base.Console.Write("Miriam closed portals: " + miriamClosedPortals.Count);
					foreach (string str in miriamClosedPortals)
					{
						base.Console.Write("   Portal from scene " + str);
					}
				}
				return;
			case "start":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.WriteCommandResult("miriam start", Core.Events.StartMiriamQuest());
				}
				return;
			case "end":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.WriteCommandResult("miriam end", Core.Events.FinishMiriamQuest());
				}
				return;
			case "activateportal":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.WriteCommandResult("miriam activateportal", Core.Events.ActivateMiriamPortalAndTeleport(true));
				}
				return;
			case "deactivateportal":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.WriteCommandResult("miriam deactivateportal", Core.Events.EndMiriamPortalAndReturn(true));
				}
				return;
			case "gotogoal":
				if (base.ValidateParams(command2, 0, paramList))
				{
					base.WriteCommandResult("miriam gotogoal", Core.Events.TeleportPenitentToGoal());
				}
				return;
			}
			base.Console.Write("Command unknow, use miriam help");
		}
	}
}
