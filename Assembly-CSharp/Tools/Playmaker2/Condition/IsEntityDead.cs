using System;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Sends onSuccess event if the entity is dead, null, or is not an entity. Otherwise returns onFailure")]
	public class IsEntityDead : FsmStateAction
	{
		public override void OnEnter()
		{
			bool flag = true;
			if (this.entity.Value != null)
			{
				Entity component = this.entity.Value.GetComponent<Entity>();
				if (component != null && !component.Dead)
				{
					flag = false;
				}
			}
			base.Fsm.Event((!flag) ? this.onFailure : this.onSuccess);
			base.Finish();
		}

		public FsmGameObject entity;

		public FsmEvent onSuccess;

		public FsmEvent onFailure;
	}
}
