using System;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using HutongGames.PlayMaker;
using Tools.Level;
using Tools.Level.Actionables;
using Tools.NPCs;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	public class ObjectIsType : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.entity.Value == null)
			{
				base.Finish();
				return;
			}
			switch (this.type)
			{
			case ObjectCategory.Penitent:
			{
				Penitent componentInParent = this.entity.Value.GetComponentInParent<Penitent>();
				base.Fsm.Event((!(componentInParent != null)) ? this.onFailure : this.onSuccess);
				break;
			}
			case ObjectCategory.Enemy:
			{
				Enemy componentInParent2 = this.entity.Value.GetComponentInParent<Enemy>();
				base.Fsm.Event((!(componentInParent2 != null)) ? this.onFailure : this.onSuccess);
				break;
			}
			case ObjectCategory.NPC:
			{
				NPC componentInParent3 = this.entity.Value.GetComponentInParent<NPC>();
				base.Fsm.Event((!(componentInParent3 != null)) ? this.onFailure : this.onSuccess);
				break;
			}
			case ObjectCategory.Interactable:
			{
				Interactable componentInParent4 = this.entity.Value.GetComponentInParent<Interactable>();
				base.Fsm.Event((!(componentInParent4 != null)) ? this.onFailure : this.onSuccess);
				break;
			}
			case ObjectCategory.Destructible:
			{
				BreakableObject componentInParent5 = this.entity.Value.GetComponentInParent<BreakableObject>();
				base.Fsm.Event((!(componentInParent5 != null)) ? this.onFailure : this.onSuccess);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			base.Finish();
		}

		public FsmGameObject entity;

		public ObjectCategory type;

		public FsmEvent onSuccess;

		public FsmEvent onFailure;
	}
}
