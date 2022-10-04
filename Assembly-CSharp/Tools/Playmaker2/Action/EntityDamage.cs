using System;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Damages the selected entity.")]
	public class EntityDamage : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.entity != null)
			{
				this.entity.Damage(this.value.Value, string.Empty);
			}
		}

		public Entity entity;

		public FsmFloat value;
	}
}
