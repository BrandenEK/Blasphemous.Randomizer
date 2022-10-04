using System;
using UnityEngine;

namespace Tools.Level.Layout
{
	[SelectionBase]
	public class LayoutElement : MonoBehaviour
	{
		public LevelBuilder LevelBuilder { get; private set; }

		public SpriteRenderer SpriteRenderer { get; private set; }

		private void Awake()
		{
			this.SpriteRenderer = base.GetComponent<SpriteRenderer>();
		}

		private void Start()
		{
			this.LevelBuilder = base.GetComponentInParent<LevelBuilder>();
		}

		public bool showInGame;

		public Category category;
	}
}
