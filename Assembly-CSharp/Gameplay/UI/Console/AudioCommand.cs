using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Gameplay.UI.Console
{
	public class AudioCommand : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			List<string> paramList;
			string subcommand = base.GetSubcommand(parameters, out paramList);
			if (command != null)
			{
				if (command == "audio")
				{
					this.ParseAudio(subcommand, paramList);
				}
			}
		}

		public override List<string> GetNames()
		{
			return new List<string>
			{
				"audio"
			};
		}

		private void ParseAudio(string command, List<string> paramList)
		{
			float num = 0f;
			string command2 = "audio " + command;
			if (command != null)
			{
				if (command == "help")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Available AUDIO commands:");
						base.Console.Write("audio list: List all volumes");
						base.Console.Write("audio master VALUE: Change the master volumen, 0-1");
						base.Console.Write("audio sfx VALUE: Change the sfx volumen, 0-1");
						base.Console.Write("audio music VALUE: Change the music volumen, 0-1");
						base.Console.Write("audio voiceover VALUE: Change the voiceover volumen, 0-1");
					}
					return;
				}
				if (command == "master")
				{
					if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out num, 0f, 1f))
					{
						Core.Audio.MasterVolume = num;
						base.Console.Write("Master volume setted");
					}
					return;
				}
				if (command == "sfx")
				{
					if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out num, 0f, 1f))
					{
						Core.Audio.SetSfxVolume(num);
						base.Console.Write("Sfx volume setted");
					}
					return;
				}
				if (command == "music")
				{
					if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out num, 0f, 1f))
					{
						Core.Audio.SetMusicVolume(num);
						base.Console.Write("Music volume setted");
					}
					return;
				}
				if (command == "voiceover")
				{
					if (base.ValidateParams(command2, 1, paramList) && base.ValidateParam(paramList[0], out num, 0f, 1f))
					{
						Core.Audio.SetVoiceoverVolume(num);
						base.Console.Write("Voiceover volume setted");
					}
					return;
				}
				if (command == "list")
				{
					if (base.ValidateParams(command2, 0, paramList))
					{
						base.Console.Write("Master volume: " + Core.Audio.MasterVolume.ToString());
						base.Console.Write("Sfx volume: " + Core.Audio.GetSfxVolume());
						base.Console.Write("Music volume: " + Core.Audio.GetMusicVolume());
						base.Console.Write("Voiceover volume: " + Core.Audio.GetVoiceoverVolume());
					}
					return;
				}
			}
			base.Console.Write("Command unknow, use audio help");
		}
	}
}
