using System;
using UnityEngine;

namespace Tools.DataContainer
{
	[CreateAssetMenu(menuName = "Blasphemous/Shared Command")]
	public class SharedCommand : ScriptableObject
	{
		[HideInInspector]
		public string Id;

		public string Description;

		[TextArea(30, 60)]
		public string commands;
	}
}
