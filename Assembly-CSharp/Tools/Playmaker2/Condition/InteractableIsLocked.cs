using System;
using HutongGames.PlayMaker;
using Tools.Level;
using UnityEngine;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	public class InteractableIsLocked : FsmStateAction
	{
		public override void OnEnter()
		{
			Interactable component = this.interactable.GetComponent<Interactable>();
			base.Fsm.Event((!component.Locked) ? this.onFailure : this.onSuccess);
			base.Finish();
		}

		[UIHint(10)]
		public GameObject interactable;

		public FsmEvent onSuccess;

		public FsmEvent onFailure;
	}
}
