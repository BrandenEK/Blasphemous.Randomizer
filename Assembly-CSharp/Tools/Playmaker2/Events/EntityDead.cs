using System;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised when an entity dies.")]
	public class EntityDead : FsmStateAction
	{
		public override void OnEnter()
		{
			Entity.Death += this.Dead;
		}

		private void Dead(Entity deadEntity)
		{
			this.entity.Value = deadEntity.gameObject;
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}

		public override void OnExit()
		{
			Entity.Death -= this.Dead;
		}

		[UIHint(UIHint.Variable)]
		public FsmGameObject entity;

		public FsmEvent onSuccess;
	}
}
