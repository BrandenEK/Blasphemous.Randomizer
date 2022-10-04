using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Abilities;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	public class PlayerUseHealing : FsmStateAction
	{
		public override void OnEnter()
		{
			Core.Logic.Penitent.GetComponentInChildren<Healing>().Cast();
		}
	}
}
