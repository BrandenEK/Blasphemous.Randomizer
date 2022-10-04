using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	[Serializable]
	public class SpawnBehaviourConfig
	{
		public float TryGetFloat(string n)
		{
			float result = -1f;
			if (this.floatParams != null)
			{
				result = this.floatParams.Find((SpawnBehaviorFloatParam x) => x.name == n).value;
			}
			return result;
		}

		[FoldoutGroup("Spawn config", 0)]
		public bool dontWalk;

		[FoldoutGroup("Spawn config", 0)]
		public List<SpawnBehaviorFloatParam> floatParams;
	}
}
