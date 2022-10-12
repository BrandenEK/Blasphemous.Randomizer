using System;
using HutongGames.PlayMaker;
using Tools.Level;
using UnityEngine;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	public class InteractableIsConsumed : FsmStateAction
	{
		public override void OnEnter()
		{
			Interactable component = this.interactable.GetComponent<Interactable>();
			base.Fsm.Event((!component.Consumed) ? this.onFailure : this.onSuccess);
			base.Finish();
		}

		[UIHint(UIHint.Variable)]
		public GameObject interactable;

		public FsmEvent onSuccess;

		public FsmEvent onFailure;
	}
}
