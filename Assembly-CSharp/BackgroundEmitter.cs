using System;
using Sirenix.OdinInspector;
using Tools.Audio;
using UnityEngine;

public class BackgroundEmitter : AudioTool
{
	public const int ENTERING = 0;

	public const int LEAVING = 1;

	public const int NOT_FADING = 2;

	[SerializeField]
	[BoxGroup("Audio Settings", true, false, 0)]
	[Range(0f, 10f)]
	private float transitionDistance;
}
