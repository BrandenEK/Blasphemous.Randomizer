using System;
using Framework.FrameworkCore;
using Sirenix.Utilities;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.UI.Console
{
	public class Graybox : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			if (parameters.Length < 1)
			{
				base.Console.Write("Invalid parameters. Type help for more information.");
			}
			else if (parameters[0] == "show")
			{
				Graybox.ShowGraybox(true);
				base.Console.Write("Graybox is now showing.");
			}
			else if (parameters[0] == "hide")
			{
				Graybox.ShowGraybox(false);
				base.Console.Write("Graybox is now hidding.");
			}
		}

		private static void ShowGraybox(bool b)
		{
			LayoutElement[] array = UnityEngine.Object.FindObjectsOfType<LayoutElement>();
			Log.Trace("Decoration", string.Concat(new object[]
			{
				"Modifiying graybox visibility. Visible: ",
				b,
				" Afecteds: ",
				array.Length
			}), null);
			for (int i = 0; i < array.Length; i++)
			{
				SpriteRenderer[] componentsInChildren = array[i].GetComponentsInChildren<SpriteRenderer>();
				if (componentsInChildren != null && array[i].showInGame)
				{
					componentsInChildren.ForEach(delegate(SpriteRenderer x)
					{
						x.enabled = true;
					});
				}
				else if (componentsInChildren != null)
				{
					componentsInChildren.ForEach(delegate(SpriteRenderer x)
					{
						x.enabled = b;
					});
				}
			}
		}

		public override string GetName()
		{
			return "graybox";
		}
	}
}
