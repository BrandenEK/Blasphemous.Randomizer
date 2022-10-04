using System;
using System.Collections.Generic;
using Framework.Dialog;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class DialogCommand : ConsoleCommand
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
				if (command == "dialog")
				{
					this.ParseDialog(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"dialog"
			};
		}

		private void ParseDialog(string command, List<string> paramList)
		{
			string command2 = "dialog " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available DIALOG commands:");
						base.Console.Write("dialog list: List all dialogs");
						base.Console.Write("dialog start IDDIALOG: Start a dialog");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("All dialogs ID:");
						foreach (DialogObject dialogObject in Core.Dialog.GetAllDialogs())
						{
							base.Console.Write(dialogObject.id.ToString() + " - " + dialogObject.sortDescription);
						}
					}
					return;
				}
				if (command == "start")
				{
					if (base.ValidateParams(command2, 1, paramList))
					{
						base.WriteCommandResult(command2, Core.Dialog.StartConversation(paramList[0], true, false, true, 0, false));
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use dialog help");
		}
	}
}
