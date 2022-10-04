using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.UI.Widgets;
using Tools.DataContainer;
using UnityEngine;

namespace Framework.Managers
{
	public class SharedCommands : GameSystem
	{
		public override void Initialize()
		{
			this.LoadAllCommands();
		}

		public void RefreshCommands()
		{
			this.LoadAllCommands();
		}

		public List<SharedCommand> GetAllCommands()
		{
			return this.Commands.Values.ToList<SharedCommand>();
		}

		public bool ExecuteCommand(string Id)
		{
			SharedCommand commandFromName = this.GetCommandFromName(Id);
			if (commandFromName)
			{
				List<string> list = new List<string>(commandFromName.commands.Split(new char[]
				{
					'\n'
				}));
				foreach (string text in list)
				{
					string text2 = text.Replace("\r", string.Empty);
					{
						ConsoleWidget.Instance.ProcessCommand(text2);
					}
				}
			}
			return commandFromName != null;
		}

		private void LoadAllCommands()
		{
			this.Commands.Clear();
			SharedCommand[] array = Resources.LoadAll<SharedCommand>("SharedCommands/");
			foreach (SharedCommand sharedCommand in array)
			{
				sharedCommand.Id = sharedCommand.name.ToUpper();
				this.Commands[sharedCommand.Id] = sharedCommand;
			}
		}

		private SharedCommand GetCommandFromName(string id)
		{
			SharedCommand sharedCommand = null;
			string idUpper = id.ToUpper();
			if (this.Commands.ContainsKey(idUpper))
			{
				sharedCommand = this.Commands[idUpper];
			}
			if (!sharedCommand)
			{
				sharedCommand = (from e in this.Commands
				where e.Key.StartsWith(idUpper)
				select e into kv
				select kv.Value).FirstOrDefault<SharedCommand>();
			}
			if (!sharedCommand)
			{
				sharedCommand = (from e in this.Commands
				where e.Key.Contains(idUpper)
				select e into kv
				select kv.Value).FirstOrDefault<SharedCommand>();
			}
			return sharedCommand;
		}

		private const string COMMAND_DIRECTORY = "SharedCommands/";

		private Dictionary<string, SharedCommand> Commands = new Dictionary<string, SharedCommand>();
	}
}
