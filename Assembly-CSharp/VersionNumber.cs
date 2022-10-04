using System;
using Framework.Randomizer;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionNumber : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<Text>().text = Randomizer.getVersion();
	}
}
