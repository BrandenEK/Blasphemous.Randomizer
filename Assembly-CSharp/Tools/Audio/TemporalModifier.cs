using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Audio
{
	[RequireComponent(typeof(Collider2D))]
	public class TemporalModifier : AudioTool
	{
		public bool Triggered { get; set; }

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private AudioTool target;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[Range(0f, 5f)]
		private float transitionTime;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private ParameterData[] parameters;

		private float[] startValues;

		private float lerpProgress;
	}
}
