using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	[CreateAssetMenu(fileName = "FlaskRegenerationBalance", menuName = "Blasphemous/Flask Regeneration", order = 0)]
	public class FlaskRegenerationBalance : ScriptableObject
	{
		public float GetTimeByFlaskLevel(int flaskLevel)
		{
			foreach (FlaskRegenerationBalance.FlaskRegeneration flaskRegeneration in this.regenerationByLevel)
			{
				if (flaskRegeneration.flaskLevel == flaskLevel)
				{
					return flaskRegeneration.regenerationTime;
				}
			}
			Debug.LogErrorFormat("Can't find regeneration time for flask level {0}!. Using last in list: ({1})", new object[]
			{
				flaskLevel,
				this.regenerationByLevel[this.regenerationByLevel.Count - 1].regenerationTime
			});
			return this.regenerationByLevel[this.regenerationByLevel.Count - 1].regenerationTime;
		}

		[SerializeField]
		public List<FlaskRegenerationBalance.FlaskRegeneration> regenerationByLevel;

		[Serializable]
		public struct FlaskRegeneration
		{
			public int flaskLevel;

			public float regenerationTime;
		}
	}
}
