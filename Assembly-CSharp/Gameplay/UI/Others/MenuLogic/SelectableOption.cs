using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	[Serializable]
	public struct SelectableOption
	{
		public GameObject parent;

		public GameObject selectionTransform;

		public Text highlightableText;
	}
}
