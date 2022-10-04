using System;
using UnityEngine;

namespace Tools.Level.Layout
{
	[ExecuteInEditMode]
	public class LevelBuilder : MonoBehaviour
	{
		public Category Mode
		{
			get
			{
				return this.mode;
			}
		}

		[SerializeField]
		[Tooltip("Switches between level building modes. Shortcut: Ctrl + E")]
		private Category mode;
	}
}
