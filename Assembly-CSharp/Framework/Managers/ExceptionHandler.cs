using System;
using UnityEngine;

namespace Framework.Managers
{
	public class ExceptionHandler : GameSystem
	{
		public override void Initialize()
		{
			base.Initialize();
			if (!Application.isEditor)
			{
				Application.logMessageReceived += new Application.LogCallback(this.HandleException);
			}
		}

		private void HandleException(string condition, string stackTrace, LogType type)
		{
			switch (type)
			{
			case 0:
				break;
			case 1:
				break;
			case 2:
				break;
			case 3:
				break;
			case 4:
				Debug.LogError("Ups! There was an unhandled exception: " + condition + "\n" + stackTrace);
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
		}

		public override void Update()
		{
			base.Update();
			if (Debug.developerConsoleVisible && Input.GetKeyDown(283))
			{
				Debug.ClearDeveloperConsole();
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			Application.logMessageReceived -= new Application.LogCallback(this.HandleException);
		}
	}
}
