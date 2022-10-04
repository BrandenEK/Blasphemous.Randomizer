using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI.Widgets;

namespace Gameplay.UI.Console
{
	public class ConsoleCommand
	{
		private protected ConsoleWidget Console { protected get; private set; }

		private protected Penitent Penitent { protected get; private set; }

		public void Initialize(ConsoleWidget console)
		{
			this.Console = console;
			this.Penitent = Core.Logic.Penitent;
		}

		public virtual void Start()
		{
		}

		public virtual void Update()
		{
		}

		public virtual bool ToLowerAll()
		{
			return true;
		}

		public virtual bool HasLowerParameters()
		{
			return true;
		}

		public virtual void Execute(string command, string[] parameters)
		{
		}

		public virtual string GetName()
		{
			return string.Empty;
		}

		public virtual List<string> GetNames()
		{
			return new List<string>
			{
				this.GetName()
			};
		}

		protected string GetSubcommand(string[] parameters, out List<string> listParameters)
		{
			listParameters = new List<string>(parameters);
			if (listParameters.Count == 0)
			{
				listParameters.Add("help");
			}
			string result = listParameters[0];
			listParameters.RemoveAt(0);
			return result;
		}

		protected bool ValidateParams(string command, int number, List<string> parameters)
		{
			bool result = true;
			if (number != parameters.Count)
			{
				result = false;
				string text = string.Concat(new object[]
				{
					"The command ",
					command,
					" needs ",
					number,
					" params"
				});
				text = text + ". You passed " + parameters.Count;
				this.Console.Write(text);
			}
			return result;
		}

		protected bool ValidateParam(string param, out int resultValue, int minValue, int maxValue)
		{
			bool flag = false;
			if (int.TryParse(param, out resultValue))
			{
				flag = (resultValue >= minValue && resultValue <= maxValue);
			}
			if (!flag)
			{
				this.Console.Write(string.Concat(new object[]
				{
					"The parameter ",
					param,
					" must be an integer between ",
					minValue,
					" and ",
					maxValue
				}));
			}
			return flag;
		}

		protected bool ValidateParam(string param, out float resultValue, float minValue, float maxValue)
		{
			bool flag = false;
			if (float.TryParse(param, out resultValue))
			{
				flag = (resultValue >= minValue && resultValue <= maxValue);
			}
			if (!flag)
			{
				this.Console.Write(string.Concat(new object[]
				{
					"The parameter ",
					param,
					" must be a float between ",
					minValue,
					" and ",
					maxValue
				}));
			}
			return flag;
		}

		protected void WriteCommandResult(string command, bool result)
		{
			string text = "The command " + command;
			if (result)
			{
				text += " executed successfully";
			}
			else
			{
				text += " has a problem and return false";
			}
			this.Console.Write(text);
		}
	}
}
