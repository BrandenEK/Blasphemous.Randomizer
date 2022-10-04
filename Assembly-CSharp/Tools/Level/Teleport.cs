using System;
using Framework.FrameworkCore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level
{
	public class Teleport : PersistentObject
	{
		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public string telportName;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public EntityOrientation spawnOrientation;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public bool showOnMap;
	}
}
