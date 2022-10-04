using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputIconLayout", menuName = "Blasphemous/Input/InputIconLayout")]
public class InputIconLayout : ScriptableObject
{
	public Sprite defaultIcon;

	public ButtonDescription[] buttons = new ButtonDescription[0];
}
