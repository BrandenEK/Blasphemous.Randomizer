using System;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class SendEvent : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			if (parameters.Length == 0)
			{
				return;
			}
			string text = parameters[0].Replace(' ', '_').ToUpper();
			base.Console.Write("Sending event: " + text);
			Core.Events.LaunchEvent(text, string.Empty);
		}

		public override string GetName()
		{
			return "sendevent";
		}
	}
}
