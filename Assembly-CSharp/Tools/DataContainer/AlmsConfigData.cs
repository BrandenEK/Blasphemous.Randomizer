using System;
using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.DataContainer
{
	[CreateAssetMenu(fileName = "AlmsConfig", menuName = "Blasphemous/Alms")]
	public class AlmsConfigData : SerializedScriptableObject
	{
		public List<int> GetTearsList()
		{
			List<int> list = new List<int>(this.TearsToUnlock);
			list.Sort();
			return list;
		}

		[BoxGroup("PrieDieus", true, false, 0)]
		public int Level2PrieDieus = 3;

		[BoxGroup("PrieDieus", true, false, 0)]
		public int Level3PrieDieus = 5;

		[BoxGroup("Widget Sounds", true, false, 0)]
		[InfoBox("Sound when alms added ok but no new tier reached", 1, null)]
		[EventRef]
		public string SoundAddedOk;

		[BoxGroup("Widget Sounds", true, false, 0)]
		[InfoBox("Sound when change value, the timing is determned by SoundChangeUpdate", 1, null)]
		[EventRef]
		public string SoundChange;

		[BoxGroup("Widget Sounds", true, false, 0)]
		[InfoBox("Time to wait curve to play sound change sound", 1, null)]
		public AnimationCurve SoundChangeUpdate;

		[BoxGroup("Widget Sounds", true, false, 0)]
		[InfoBox("Sound when alms added ok and new tier reached", 1, null)]
		[EventRef]
		public string SoundNewTier;

		[BoxGroup("Widget", true, false, 0)]
		[InfoBox("Time to wait until widget is showed", 1, null)]
		public float InitialDelay = 1f;

		[BoxGroup("Widget", true, false, 0)]
		[InfoBox("Speed in numbers per second with factor 1 in NumberFactorByTime", 1, null)]
		public float NumberSpeed;

		[BoxGroup("Widget", true, false, 0)]
		[InfoBox("Max number of tiers that can give in one action", 1, null)]
		public int MaxNumber;

		[InfoBox("Factor curve to multiply value depending on time pressing control to acelerate", 1, null)]
		[BoxGroup("Widget", true, false, 0)]
		public AnimationCurve NumberFactorByTime;

		[BoxGroup("Tiers", true, false, 0)]
		[ListDrawerSettings(IsReadOnly = true)]
		public int[] TearsToUnlock = new int[]
		{
			100,
			200,
			300,
			400,
			500,
			600,
			700
		};
	}
}
