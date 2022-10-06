using System;
using System.Collections.Generic;
using System.Text;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.UI.Console
{
	public class CameraCommand : ConsoleCommand
	{
		public override string GetName()
		{
			return "camera";
		}

		public override bool HasLowerParameters()
		{
			return false;
		}

		public override void Execute(string command, string[] parameters)
		{
			List<string> list;
			string subcommand = base.GetSubcommand(parameters, out list);
			if (command != null)
			{
				if (command == "camera")
				{
					this.ShowCamera(subcommand);
				}
			}
		}

		private void ShowAllCameras()
		{
			this.ShowCamera("game");
			this.ShowCamera("virtual");
			this.ShowCamera("ui");
		}

		private void ShowCamera(string camera)
		{
			string text = camera.ToUpper();
			if (text != null)
			{
				if (text == "SCENE")
				{
					this.ShowSceneCameras();
					return;
				}
				if (text == "ALL")
				{
					this.ShowAllCameras();
					return;
				}
				if (text == "UI")
				{
					this.ShowCamera(Core.Screen.UICamera);
					return;
				}
				if (text == "GAME")
				{
					this.ShowCamera(Core.Screen.GameCamera);
					return;
				}
				if (text == "VIRTUAL")
				{
					this.ShowCamera(Core.Screen.VirtualCamera);
					return;
				}
			}
			base.Console.Write("Shows camera information.");
			base.Console.Write("USAGE: camera ARGUMENT");
			base.Console.Write("Valid values for ARGUMENT are:");
			base.Console.Write("ui: Shows info for the UI camera");
			base.Console.Write("game: Shows info for the game camera");
			base.Console.Write("virtual: Shows info for the virtual (final composition) camera");
			base.Console.Write("all: Shows info for the ui, game and virtual cameras");
			base.Console.Write("scene: Shows info every loaded camera (dynamic)");
		}

		private void ShowSceneCameras()
		{
			foreach (Camera cam in Object.FindObjectsOfType<Camera>())
			{
				this.ShowCamera(cam);
			}
		}

		private void ShowCamera(Camera cam)
		{
			StringBuilder stringBuilder = new StringBuilder();
			base.Console.Write(this.SEPARATOR);
			base.Console.WriteFormat("Camera name: {0}", new object[]
			{
				cam.name
			});
			base.Console.WriteFormat("Camera position: {0}", new object[]
			{
				cam.transform.position
			});
			base.Console.WriteFormat("Camera aspect: {0}", new object[]
			{
				cam.aspect
			});
			base.Console.WriteFormat("Camera rect: {0}", new object[]
			{
				cam.rect
			});
			base.Console.WriteFormat("Camera isOrtho: {0}", new object[]
			{
				cam.orthographic
			});
			base.Console.WriteFormat("Camera orthoSize: {0}", new object[]
			{
				cam.orthographicSize
			});
			base.Console.WriteFormat("Camera pixelSize: ({0}, {1})", new object[]
			{
				cam.pixelWidth,
				cam.pixelHeight
			});
			base.Console.WriteFormat("Camera far plane: {0}", new object[]
			{
				cam.farClipPlane
			});
			base.Console.WriteFormat("Camera near plane: {0}", new object[]
			{
				cam.nearClipPlane
			});
			base.Console.WriteFormat("Camera FOV: {0}", new object[]
			{
				cam.fieldOfView
			});
			base.Console.Write(this.SEPARATOR);
			base.Console.Write(stringBuilder.ToString());
		}

		private readonly string SEPARATOR = "-------------------";
	}
}
