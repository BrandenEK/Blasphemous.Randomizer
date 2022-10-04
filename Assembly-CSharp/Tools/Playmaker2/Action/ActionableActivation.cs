using System;
using HutongGames.PlayMaker;
using Tools.Level;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Uses an actionable object.")]
	public class ActionableActivation : FsmStateAction
	{
		public override void OnEnter()
		{
			if (this.target.Value)
			{
				IActionable component = this.target.Value.GetComponent<IActionable>();
				if (component != null)
				{
					component.Use();
				}
			}
			base.Finish();
		}

		public FsmGameObject target;
	}
}
