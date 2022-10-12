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
				Application.logMessageReceived += this.HandleException;
			}
		}

		private void HandleException(string condition, string stackTrace, LogType type)
		{
			switch (type)
			{
			case LogType.Error:
				break;
			case LogType.Assert:
				break;
			case LogType.Warning:
				break;
			case LogType.Log:
				break;
			case LogType.Exception:
				Debug.LogError("Ups! There was an unhandled exception: " + condition + "\n" + stackTrace);
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
		}

		public override void Update()
		{
			base.Update();
			if (Debug.developerConsoleVisible && Input.GetKeyDown(KeyCode.F2))
			{
				Debug.ClearDeveloperConsole();
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			Application.logMessageReceived -= this.HandleException;
		}
	}
}
