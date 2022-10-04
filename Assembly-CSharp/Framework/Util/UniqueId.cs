using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Util
{
	[ExecuteInEditMode]
	public class UniqueId : MonoBehaviour
	{
		private static readonly Dictionary<string, UniqueId> allGuids = new Dictionary<string, UniqueId>();

		[UniqueIdentifier]
		public string uniqueId = string.Empty;
	}
}
