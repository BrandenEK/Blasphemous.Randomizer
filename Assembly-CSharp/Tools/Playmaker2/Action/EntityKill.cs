using System;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Kills the selected entity.")]
	public class EntityKill : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.entity != null)
			{
				this.entity.Kill();
			}
		}

		public Entity entity;
	}
}
