using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.UI.Console
{
	public class ExecutionCommand : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			base.Execute(command, parameters);
			if (parameters.Length != 1)
			{
				base.Console.Write("Parameter must be Y/n");
				return;
			}
			string parameter = parameters[0].ToLower();
			this.RunningCommand(parameter);
		}

		public override string GetName()
		{
			return "execution";
		}

		private void RunningCommand(string parameter)
		{
			if (parameter != null)
			{
				if (parameter == "y")
				{
					ExecutionCommand.EnableDebugExecution(true);
					base.Console.Write(string.Format("Debug execution {0}", "enabled."));
					return;
				}
				if (parameter == "n")
				{
					ExecutionCommand.EnableDebugExecution(false);
					base.Console.Write(string.Format("Debug execution {0}", "disabled."));
					return;
				}
			}
			base.Console.Write("Parameter must be Y/n");
		}

		public static void EnableDebugExecution(bool enable = true)
		{
			Enemy[] array = UnityEngine.Object.FindObjectsOfType<Enemy>();
			foreach (Enemy enemy in array)
			{
				enemy.DebugExecutionActive = enable;
			}
			Core.Logic.DebugExecutionEnabled = enable;
		}

		public const string ErrorMessage = "Parameter must be Y/n";

		public const string SuccessMessage = "Debug execution {0}";
	}
}
