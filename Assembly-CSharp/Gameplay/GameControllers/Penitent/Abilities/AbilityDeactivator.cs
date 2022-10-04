using System;
using System.Collections.Generic;
using System.Linq;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	[CreateAssetMenu(fileName = "AbilityDeactivator", menuName = "Blasphemous/Deactivator Skills")]
	public class AbilityDeactivator : SerializedScriptableObject
	{
		public void SetUp()
		{
			this.EnableAbilities(false);
		}

		private void EnableAbilities(bool enableAbility = true)
		{
			Penitent penitent = Core.Logic.Penitent;
			this.abillityList = penitent.GetComponentsInChildren<Ability>().ToList<Ability>();
			foreach (Ability ability in this.deactivableAbilities)
			{
				foreach (Ability ability2 in this.abillityList)
				{
					if (ability.GetType() == ability2.GetType())
					{
						ability2.enabled = enableAbility;
					}
				}
			}
		}

		public List<Ability> deactivableAbilities = new List<Ability>();

		private List<Ability> abillityList = new List<Ability>();
	}
}
