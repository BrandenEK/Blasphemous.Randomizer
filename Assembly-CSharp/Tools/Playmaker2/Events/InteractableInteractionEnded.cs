using System;
using HutongGames.PlayMaker;
using Tools.Level;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised when an interactable is interacted.")]
	public class InteractableInteractionEnded : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.listenOnlySelf.Value)
			{
				Interactable.SInteractionEnded += this.ListenToSelf;
			}
			else
			{
				Interactable.SInteractionEnded += this.ListenToAll;
			}
		}

		private void ListenToAll(Interactable go)
		{
			this.interactable.Value = go.gameObject;
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}

		private void ListenToSelf(Interactable go)
		{
			Interactable componentInChildren = base.Owner.GetComponentInChildren<Interactable>();
			if (go.Equals(componentInChildren))
			{
				this.interactable.Value = go.gameObject;
				base.Fsm.Event(this.onSuccess);
			}
			base.Finish();
		}

		public override void OnExit()
		{
			if (this.listenOnlySelf.Value)
			{
				Interactable.SInteractionEnded -= this.ListenToSelf;
			}
			else
			{
				Interactable.SInteractionEnded -= this.ListenToAll;
			}
		}

		[UIHint(10)]
		public FsmGameObject interactable;

		public FsmBool listenOnlySelf;

		public FsmEvent onSuccess;
	}
}
