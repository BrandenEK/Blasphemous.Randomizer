using System;
using System.Collections.Generic;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	[CreateAssetMenu(fileName = "RangeAttackDamageBalance", menuName = "Blasphemous/RangeAttack")]
	public class RangeAttackBalance : ScriptableObject
	{
		public float GetDamageBySwordLevel
		{
			get
			{
				float result = 0f;
				int num = (int)Core.SkillManager.GetCurrentMeaCulpa();
				num = ((num > 0) ? num : 1);
				foreach (RangeAttackBalance.AttackDamage attackDamage in this.AttackDamagesByLevel)
				{
					if (attackDamage.SwordLevel == num)
					{
						result = attackDamage.Damage;
						break;
					}
				}
				if (num > this.AttackDamagesByLevel.Count)
				{
					result = this.AttackDamagesByLevel[this.AttackDamagesByLevel.Count - 1].Damage;
				}
				return result;
			}
		}

		[SerializeField]
		public List<RangeAttackBalance.AttackDamage> AttackDamagesByLevel;

		[Serializable]
		public struct AttackDamage
		{
			public int SwordLevel;

			public float Damage;
		}
	}
}
