using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Effects.Player.Sparks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Blood Fx Table", menuName = "VFXTables/Blood")]
public class BloodVFXTable : ScriptableObject
{
	public BloodFXTableElement GetRandomElementOfType(BloodSpawner.BLOOD_FX_TYPES type)
	{
		List<BloodFXTableElement> list = this.bloodVFXList.FindAll((BloodFXTableElement x) => x.type == type);
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public List<BloodFXTableElement> bloodVFXList;
}
