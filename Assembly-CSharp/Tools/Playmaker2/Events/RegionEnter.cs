using System;
using HutongGames.PlayMaker;
using Tools.Level;

namespace Tools.Playmaker2.Events
{
	[ActionCategory("Blasphemous Event")]
	[Tooltip("Event raised when an entity enters a region.")]
	public class RegionEnter : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.listenOnlySelf.Value)
			{
				Region.OnRegionEnter += this.ListenToSelf;
			}
			else
			{
				Region.OnRegionEnter += this.ListenToAll;
			}
		}

		private void ListenToAll(Region go)
		{
			this.region.Value = go.gameObject;
			base.Fsm.Event(this.onSuccess);
			base.Finish();
		}

		private void ListenToSelf(Region go)
		{
			Region componentInChildren = base.Owner.GetComponentInChildren<Region>();
			if (go.Equals(componentInChildren))
			{
				this.region.Value = go.gameObject;
				base.Fsm.Event(this.onSuccess);
			}
			base.Finish();
		}

		public override void OnExit()
		{
			if (this.listenOnlySelf.Value)
			{
				Region.OnRegionEnter -= this.ListenToSelf;
			}
			else
			{
				Region.OnRegionEnter -= this.ListenToAll;
			}
		}

		[UIHint(10)]
		public FsmGameObject region;

		public FsmBool listenOnlySelf;

		public FsmEvent onSuccess;
	}
}
