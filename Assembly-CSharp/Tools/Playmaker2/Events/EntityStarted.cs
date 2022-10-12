using System;
using Gameplay.GameControllers.Entities;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised when an entity dies.")]
	public class EntityStarted : FsmStateAction
	{
		public override void OnEnter()
		{
			Entity.Started += this.Started;
		}

		private void Started(Entity spawnedEntity)
		{
			this.entity.Value = spawnedEntity.gameObject;
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}

		public override void OnExit()
		{
			Entity.Started -= this.Started;
		}

		[UIHint(UIHint.Variable)]
		public FsmGameObject entity;

		public FsmEvent onSuccess;
	}
}
