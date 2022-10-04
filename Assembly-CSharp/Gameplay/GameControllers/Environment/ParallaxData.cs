using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	[Serializable]
	public struct ParallaxData
	{
		public GameObject layer;

		[Range(-1f, 1f)]
		public float speed;
	}
}
