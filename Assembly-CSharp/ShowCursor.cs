using System;
using Gameplay.UI.Console;
using Gameplay.UI.Widgets;
using UnityEngine;

public class ShowCursor : ConsoleCommand
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
		return "showcursor";
	}

	private void RunningCommand(string parameter)
	{
		if (parameter != null)
		{
			if (parameter == "y")
			{
				ShowCursor.ShowMouseCursor(true);
				base.Console.Write(string.Format("Show Cursor {0}", "enabled."));
				return;
			}
			if (parameter == "n")
			{
				ShowCursor.ShowMouseCursor(false);
				base.Console.Write(string.Format("Show Cursor {0}", "disabled."));
				return;
			}
		}
		base.Console.Write("Parameter must be Y/n");
	}

	public static void ShowMouseCursor(bool enable = true)
	{
		DebugInformation debugInformation = UnityEngine.Object.FindObjectOfType<DebugInformation>();
		if (debugInformation)
		{
			debugInformation.showCursor = enable;
		}
	}

	public const string ErrorMessage = "Parameter must be Y/n";

	public const string SuccessMessage = "Show Cursor {0}";
}
