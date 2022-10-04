using System;
using UnityEngine;

public class Demo2 : MonoBehaviour
{
	private void OnGUI()
	{
		GUI.Label(new Rect(5f, 5f, 200f, 22f), "Hello scene 2");
	}
}
