using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;

namespace Gameplay.GameControllers.Bosses.BurntFace
{
	[Serializable]
	public struct BurntFaceRosaryPattern
	{
		public string ID;

		public float activeTime;

		public float maxSpeed;

		public float radiusOffset;

		public List<BurntFaceRosaryAngles> activeSections;

		public BurntFaceRosaryBead.ROSARY_BEAD_TYPE beadType;

		public float projectileSpeed;

		public float projectileFireRate;

		public float beamWarningTime;
	}
}
