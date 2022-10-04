using System;
using System.Collections.Generic;
using Framework.Managers;
using Framework.Penitences;

namespace Gameplay.UI.Console
{
	public class PenitenceCommand : ConsoleCommand
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
				if (command == "penitence")
				{
					this.ParsePenitence(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"penitence"
			};
		}

		private void ParsePenitence(string command, List<string> paramList)
		{
			string command2 = "penitence " + command;
			if (command.ToLowerInvariant().StartsWith("pe"))
			{
				if (base.ValidateParams(command2, 0, paramList))
				{
					string text = command.ToUpperInvariant();
					if (Core.PenitenceManager.CheckPenitenceExists(text))
					{
						Core.PenitenceManager.ActivatePenitence(text);
						base.Console.Write("Penitence '" + text + "' has been activated.");
					}
					else
					{
						base.Console.Write("A penitence with name: " + text + " doesn't exist!");
					}
				}
			}
			else
			{
				switch (command)
				{
				case "help":
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available PENITENCE commands:");
						base.Console.Write("current: gets the currently active penitence, if any.");
						base.Console.Write("activate PEXX: activates the penitence with the given id.");
						base.Console.Write("deactivate: deactivates the currently active penitence.");
						base.Console.Write("abandon: abandons the currently active penitence.");
						base.Console.Write("complete: completes the currently active penitence.");
						base.Console.Write("listall: lists all the penitences.");
						base.Console.Write("listabandoned: lists all the abandoned penitences.");
						base.Console.Write("listcompleted: lists all the completed penitences.");
					}
					return;
				case "current":
					if (base.ValidateParams(command2, 0, paramList))
					{
						IPenitence currentPenitence = Core.PenitenceManager.GetCurrentPenitence();
						if (currentPenitence != null)
						{
							base.Console.Write("Current Penitence: " + currentPenitence.Id);
						}
						else
						{
							base.Console.Write("There is no penitence currently active!");
						}
					}
					return;
				case "activate":
					if (base.ValidateParams(command2, 1, paramList))
					{
						string text2 = paramList[0].ToUpperInvariant();
						if (Core.PenitenceManager.CheckPenitenceExists(text2))
						{
							Core.PenitenceManager.ActivatePenitence(text2);
							base.Console.Write("Penitence '" + text2 + "' has been activated.");
						}
						else
						{
							base.Console.Write("A penitence with name: " + text2 + " doesn't exist!");
						}
					}
					return;
				case "deactivate":
					if (base.ValidateParams(command2, 0, paramList))
					{
						IPenitence currentPenitence2 = Core.PenitenceManager.GetCurrentPenitence();
						if (currentPenitence2 != null)
						{
							Core.PenitenceManager.DeactivateCurrentPenitence();
							base.Console.Write("Previously current penitence '" + currentPenitence2.Id + "' has been deactivated.");
						}
						else
						{
							base.Console.Write("There is no penitence currently active!");
						}
					}
					return;
				case "abandon":
					if (base.ValidateParams(command2, 0, paramList))
					{
						IPenitence currentPenitence3 = Core.PenitenceManager.GetCurrentPenitence();
						if (currentPenitence3 != null)
						{
							Core.PenitenceManager.MarkCurrentPenitenceAsAbandoned();
							base.Console.Write("Previously current penitence '" + currentPenitence3.Id + "' has been marked as abandoned.");
						}
						else
						{
							base.Console.Write("There is no penitence currently active!");
						}
					}
					return;
				case "complete":
					if (base.ValidateParams(command2, 0, paramList))
					{
						IPenitence currentPenitence4 = Core.PenitenceManager.GetCurrentPenitence();
						if (currentPenitence4 != null)
						{
							Core.PenitenceManager.MarkCurrentPenitenceAsCompleted();
							base.Console.Write("Previously current penitence '" + currentPenitence4.Id + "' has been marked as completed.");
						}
						else
						{
							base.Console.Write("There is no penitence currently active!");
						}
					}
					return;
				case "listall":
					if (base.ValidateParams(command2, 0, paramList))
					{
						List<IPenitence> allPenitences = Core.PenitenceManager.GetAllPenitences();
						if (allPenitences.Count > 0)
						{
							allPenitences.ForEach(delegate(IPenitence p)
							{
								base.Console.Write("Penitence: " + p.Id);
							});
						}
						else
						{
							base.Console.Write("Something went wrong! It seems that there are no penitences.");
						}
					}
					return;
				case "listabandoned":
					if (base.ValidateParams(command2, 0, paramList))
					{
						List<IPenitence> allAbandonedPenitences = Core.PenitenceManager.GetAllAbandonedPenitences();
						if (allAbandonedPenitences.Count > 0)
						{
							allAbandonedPenitences.ForEach(delegate(IPenitence p)
							{
								base.Console.Write("Abandoned penitence: " + p.Id);
							});
						}
						else
						{
							base.Console.Write("There are no abandoned penitences.");
						}
					}
					return;
				case "listcompleted":
					if (base.ValidateParams(command2, 0, paramList))
					{
						List<IPenitence> allCompletedPenitences = Core.PenitenceManager.GetAllCompletedPenitences();
						if (allCompletedPenitences.Count > 0)
						{
							allCompletedPenitences.ForEach(delegate(IPenitence p)
							{
								base.Console.Write("Completed penitence: " + p.Id);
							});
						}
						else
						{
							base.Console.Write("There are no completed penitences.");
						}
					}
					return;
				}
				base.Console.Write("Command unknow, use penitence help");
			}
		}
	}
}
