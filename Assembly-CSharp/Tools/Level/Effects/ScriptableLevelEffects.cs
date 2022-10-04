using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Effects
{
	[CreateAssetMenu(menuName = "Blasphemous/Level/Create LevelEffectsDatabase", fileName = "LevelEffectsDatabase")]
	public class ScriptableLevelEffects : SerializedScriptableObject
	{
		public Dictionary<LEVEL_COLOR_CONFIGS, LevelColorEffectData> levelColorConfigs;
	}
}
