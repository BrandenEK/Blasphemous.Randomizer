using System;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised when an entity is attacked.")]
	public class EntityAttacked : FsmStateAction
	{
		public override void OnEnter()
		{
			Entity.Damaged += this.Damaged;
		}

		private void Damaged(Entity damagedEntity)
		{
			this.entity.Value = damagedEntity.gameObject;
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}

		public override void OnExit()
		{
			Entity.Damaged -= this.Damaged;
		}

		[UIHint(10)]
		public FsmGameObject entity;

		public FsmEvent onSuccess;
	}
}
