using System;
using UnityEngine;

namespace Framework.Inventory
{
	[Serializable]
	public class BaseElement : ScriptableObject
	{
		public string id = string.Empty;

		public string caption = string.Empty;

		[TextArea(3, 10)]
		public string description = string.Empty;

		[TextArea(6, 10)]
		public string lore = string.Empty;

		public Texture2D picture;

		public bool carryonstart;
	}
}
