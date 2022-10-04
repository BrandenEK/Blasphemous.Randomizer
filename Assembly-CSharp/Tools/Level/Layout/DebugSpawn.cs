using System;
using UnityEngine;

namespace Tools.Level.Layout
{
	public class DebugSpawn : MonoBehaviour
	{
		[TextArea(3, 20)]
		public string initialCommands = string.Empty;
	}
}
