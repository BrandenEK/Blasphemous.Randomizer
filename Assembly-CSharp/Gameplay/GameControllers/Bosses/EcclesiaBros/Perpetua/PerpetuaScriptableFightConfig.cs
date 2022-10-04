using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua
{
	[CreateAssetMenu(menuName = "Blasphemous/Bosses/PerpetuaConfig")]
	public class PerpetuaScriptableFightConfig : ScriptableObject
	{
		public PerpetuaAttackConfig GetAttack(PerpetuaBehaviour.Perpetua_ATTACKS atk)
		{
			return this.attackList.Find((PerpetuaAttackConfig x) => x.attack == atk);
		}

		[FoldoutGroup("Attacks", 0)]
		public List<PerpetuaAttackConfig> attackList;
	}
}
