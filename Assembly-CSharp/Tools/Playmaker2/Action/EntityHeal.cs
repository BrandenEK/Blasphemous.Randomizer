using System;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Heals the selected entity.")]
	public class EntityHeal : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.entity != null)
			{
				this.entity.SetHealth(this.entity.CurrentLife + this.value.Value);
			}
		}

		public Entity entity;

		public FsmFloat value;
	}
}
