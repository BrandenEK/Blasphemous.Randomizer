using System;
using System.Collections.Generic;

namespace Framework.Randomizer
{
	public class Logger
	{
		public Logger(int length)
		{
			this.maxLength = length;
			this.log = new List<string>();
		}

		public void Log(string message)
		{
			this.log.Add(message);
			if (this.log.Count > this.maxLength)
			{
				this.log.RemoveAt(0);
			}
		}

		public string getLog()
		{
			string text = "";
			for (int i = 0; i < this.log.Count; i++)
			{
				text = text + this.log[i] + " * ";
			}
			return text;
		}

		public void Clear()
		{
			this.log.Clear();
		}

		private List<string> log;

		private int maxLength;
	}
}
