using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	[CreateAssetMenu(fileName = "RosaryPatterns", menuName = "Blasphemous/Bosses/BurntFace/CreateRosaryPatterns")]
	public class BurntFaceRosaryScriptablePattern : ScriptableObject
	{
		public BurntFaceRosaryPattern GetPattern(string ID)
		{
			return this.patterns.Find((BurntFaceRosaryPattern x) => x.ID == ID);
		}

		public List<BurntFaceRosaryPattern> patterns;
	}
}
