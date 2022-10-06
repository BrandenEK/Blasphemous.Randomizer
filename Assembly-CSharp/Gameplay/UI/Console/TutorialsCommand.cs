using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Sirenix.Utilities;
using UnityEngine;

namespace Gameplay.UI.Console
{
	public class TutorialsCommand : ConsoleCommand
	{
		public override bool HasLowerParameters()
		{
			return false;
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"tutorial"
			};
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> list;
			string subcommand = base.GetSubcommand(parameters, out list);
			Tutorial[] array = Resources.FindObjectsOfTypeAll<Tutorial>();
			LinqExtensions.Sort<Tutorial>(array, (Tutorial t1, Tutorial t2) => (t1.order >= t2.order) ? 1 : -1);
			if (subcommand != null)
			{
				if (subcommand == "list")
				{
					base.Console.Write("Available tutorials:");
					foreach (Tutorial tutorial in array)
					{
						base.Console.Write(string.Format("{0}: {1}", tutorial.id, tutorial.description));
					}
					return;
				}
				if (subcommand == "show")
				{
					if (base.ValidateParams("tutorial list", 1, list))
					{
						string text = null;
						foreach (Tutorial tutorial2 in array)
						{
							if (tutorial2.id.ToUpper().StartsWith(list[0].ToUpper()))
							{
								text = tutorial2.id;
								break;
							}
						}
						if (text != null)
						{
							Singleton<Core>.Instance.StartCoroutine(Core.TutorialManager.ShowTutorial(text, true));
						}
						else
						{
							base.Console.Write("Unknown tutorial: " + list[0]);
						}
					}
					return;
				}
			}
			base.Console.Write("Available " + command + " commands:");
			base.Console.Write(command + " show TUTORIAL_ID: Show TUTORIAL_ID tutorial");
			base.Console.Write(command + " list: Show tutorials list");
		}
	}
}
