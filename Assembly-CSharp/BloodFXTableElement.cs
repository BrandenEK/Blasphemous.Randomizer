using System;
using Gameplay.GameControllers.Effects.Player.Sparks;
using UnityEngine;

[Serializable]
public class BloodFXTableElement
{
	public GameObject prefab;

	public BloodSpawner.BLOOD_FX_TYPES type;

	public int poolSize = 3;
}
