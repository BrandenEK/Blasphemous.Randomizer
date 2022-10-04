using System;
using System.Collections.Generic;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.FrameworkCore
{
	[CreateAssetMenu(fileName = "teleport", menuName = "Blasphemous/Teleport Destination")]
	public class TeleportDestination : ScriptableObject
	{
		private IList<string> ScenesList()
		{
			return null;
		}

		[BoxGroup("General", true, false, 0)]
		public string id = string.Empty;

		[BoxGroup("General", true, false, 0)]
		public string caption = string.Empty;

		[BoxGroup("General", true, false, 0)]
		[ValueDropdown("ScenesList")]
		public string sceneName = string.Empty;

		[BoxGroup("General", true, false, 0)]
		public string teleportName = string.Empty;

		[BoxGroup("Esthetics", true, false, 0)]
		public bool UseFade = true;

		[BoxGroup("Esthetics", true, false, 0)]
		public bool UseCustomLoadColor;

		[BoxGroup("Esthetics", true, false, 0)]
		[ShowIf("UseCustomLoadColor", true)]
		public Color CustomLoadColor = Color.black;

		[BoxGroup("UI", true, false, 0)]
		public bool useInUI = true;

		[BoxGroup("UI", true, false, 0)]
		[ShowIf("useInUI", true)]
		public bool activeAtStart;

		[BoxGroup("UI", true, false, 0)]
		[ShowIf("useInUI", true)]
		public int selectedSlot = -1;

		[BoxGroup("UI", true, false, 0)]
		[ShowIf("useInUI", true)]
		public int order;

		[BoxGroup("UI", true, false, 0)]
		[ShowIf("useInUI", true)]
		public bool labelUnderIcon;

		[NonSerialized]
		public bool isActive;

		[BoxGroup("General", true, false, 0)]
		public bool useInCompletition;

		private ValueDropdownList<PersistentManager.PercentageType> TeleportPercentages = new ValueDropdownList<PersistentManager.PercentageType>
		{
			{
				"Teleport_A_1.5",
				PersistentManager.PercentageType.Teleport_A
			},
			{
				"Teleport_B_3",
				PersistentManager.PercentageType.Teleport_B
			}
		};

		[BoxGroup("General", true, false, 0)]
		[ShowIf("useInCompletition", true)]
		[ValueDropdown("TeleportPercentages")]
		public PersistentManager.PercentageType percentageType = PersistentManager.PercentageType.Teleport_A;

		private static List<string> scenesCache = new List<string>();
	}
}
