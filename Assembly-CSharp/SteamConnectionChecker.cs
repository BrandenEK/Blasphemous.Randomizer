using System;
using UnityEngine;

[DefaultExecutionOrder(-40000)]
public class SteamConnectionChecker : MonoBehaviour
{
	private void Awake()
	{
		if (SteamManager.Initialized)
		{
			Debug.Log("Steam client OK");
		}
	}
}
