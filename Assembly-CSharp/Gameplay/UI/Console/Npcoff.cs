using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.UI.Console
{
	public class Npcoff : ConsoleCommand
	{
		public override void Execute(string command, string[] parameters)
		{
			this.enabled = !this.enabled;
			DamageArea[] array = Object.FindObjectsOfType<DamageArea>();
			foreach (DamageArea damageArea in array)
			{
				damageArea.enabled = this.enabled;
			}
		}

		public override string GetName()
		{
			return "npcoff";
		}

		private bool enabled;
	}
}
