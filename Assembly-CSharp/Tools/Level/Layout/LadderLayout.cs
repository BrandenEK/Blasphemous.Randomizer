using System;
using UnityEngine;

namespace Tools.Level.Layout
{
	[ExecuteInEditMode]
	public class LadderLayout : MonoBehaviour
	{
		[SerializeField]
		[Range(0f, 20f)]
		private int _size;

		[SerializeField]
		private SpriteRenderer _sprite;

		[SerializeField]
		private BoxCollider2D _trigger;
	}
}
