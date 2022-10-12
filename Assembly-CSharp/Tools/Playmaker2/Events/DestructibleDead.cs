using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using Tools.Level.Actionables;
using UnityEngine;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Event raised when a destructible is destroyed.")]
	public class DestructibleDead : FsmStateAction
	{
		public override void OnEnter()
		{
			BreakableObject.OnDead = (Core.ObjectEvent)Delegate.Combine(BreakableObject.OnDead, new Core.ObjectEvent(this.OnDead));
		}

		private void OnDead(GameObject destroyedObject)
		{
			this.destructible = destroyedObject;
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}

		public override void OnExit()
		{
			BreakableObject.OnDead = (Core.ObjectEvent)Delegate.Combine(BreakableObject.OnDead, new Core.ObjectEvent(this.OnDead));
		}

		[UIHint(UIHint.Variable)]
		public GameObject destructible;

		public FsmEvent onSuccess;
	}
}
