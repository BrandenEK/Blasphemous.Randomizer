using System;
using HutongGames.PlayMaker;
using Tools.Level;

namespace Tools.Playmaker2.Deprecated
{
	[ActionCategory("Blasphemous Deprecated")]
	[Tooltip("Uses an actionable object.")]
	public class UnlockActionable : FsmStateAction
	{
		public override void OnEnter()
		{
			IActionable component = this.target.Value.GetComponent<IActionable>();
			if (component != null)
			{
				component.Locked = this.locked.Value;
			}
			base.Finish();
		}

		public FsmGameObject target;

		public FsmBool locked;
	}
}
