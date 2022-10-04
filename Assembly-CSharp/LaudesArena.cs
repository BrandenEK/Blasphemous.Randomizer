using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Bosses.Amanecidas;
using UnityEngine;

public class LaudesArena : MonoBehaviour
{
	public void SetNextArena(Amanecidas amanecida)
	{
		this.amanecida = amanecida;
		this.SetLaudesArena(amanecida, this.prevArena.transform.position, false);
	}

	public void SetLaudesArena(Amanecidas amanecida, Vector2 origin, bool onlySetBattleBounds = false)
	{
		this.amanecida = amanecida;
		AmanecidasAnimatorInyector.AMANECIDA_WEAPON wpn = amanecida.Behaviour.currentWeapon;
		AmanecidaArena arena = this.arenasByWeapon.Find((LaudesArena.ArenaByWeapon x) => x.weapon == wpn).arena;
		if (this.prevArena != null && this.prevArena != arena)
		{
			this.prevArena.StartDeactivateArena();
		}
		arena.ActivateArena(amanecida, origin, onlySetBattleBounds, AmanecidaArena.WEAPON_FIGHT_PHASE.FIRST);
		this.prevArena = arena;
	}

	public void StartIntro(AmanecidasAnimatorInyector.AMANECIDA_WEAPON wpn)
	{
		AmanecidaArena arena = this.arenasByWeapon.Find((LaudesArena.ArenaByWeapon x) => x.weapon == wpn).arena;
		if (arena != null)
		{
			arena.StartIntro();
		}
		else
		{
			Debug.LogError("No arena found in LaudesArena for weapon: " + wpn);
		}
	}

	public void ActivateGameObjectsByWeaponFightPhase(AmanecidasAnimatorInyector.AMANECIDA_WEAPON wpn, AmanecidaArena.WEAPON_FIGHT_PHASE phase)
	{
		AmanecidaArena arena = this.arenasByWeapon.Find((LaudesArena.ArenaByWeapon x) => x.weapon == wpn).arena;
		if (arena != null)
		{
			arena.ActivateArena(this.amanecida, arena.transform.position, false, phase);
		}
		else
		{
			Debug.LogError("No arena found in LaudesArena for weapon: " + wpn);
		}
	}

	public List<LaudesArena.ArenaByWeapon> arenasByWeapon;

	private AmanecidaArena prevArena;

	private Amanecidas amanecida;

	[Serializable]
	public struct ArenaByWeapon
	{
		public AmanecidaArena arena;

		public AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon;
	}
}
