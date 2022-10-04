using System;
using UnityEngine;

public class CauldronTrapAnimatorEvents : MonoBehaviour
{
	public void TriggerLiquid()
	{
		this.ownerTrap.StartFall();
	}

	public void StopLiquid()
	{
		this.ownerTrap.StopFall();
	}

	public CauldronTrap ownerTrap;
}
