using System;
using UnityEngine;

namespace Tools.Audio
{
	[Serializable]
	public struct ParameterData
	{
		[SerializeField]
		public string name;

		[SerializeField]
		[Range(0f, 10f)]
		public float targetValue;
	}
}
