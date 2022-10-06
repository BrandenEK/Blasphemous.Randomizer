using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.UI.Console
{
	public class TimescaleCommand : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			base.Execute(command, parameters);
			if (parameters.Length != 1)
			{
				base.Console.Write("Command Timescale must be invoked with 1 parameter");
				return;
			}
			float num;
			if (float.TryParse(parameters[0], out num))
			{
				float timeScale = Mathf.Clamp01(num);
				Core.Logic.CurrentLevelConfig.TimeScale = timeScale;
			}
			else
			{
				base.Console.Write("Parameter must be a float number between 0 and 1.0");
			}
		}

		public override string GetName()
		{
			return "timescale";
		}

		public const string FormatErrorMessage = "Parameter must be a float number between 0 and 1.0";

		public const string ParameterErrorMessage = "Command Timescale must be invoked with 1 parameter";
	}
}
