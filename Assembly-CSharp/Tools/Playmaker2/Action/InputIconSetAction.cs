using System;
using HutongGames.PlayMaker;
using RewiredConsts;
using Tools.UI;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Set the InputIcon action.")]
	public class InputIconSetAction : FsmStateAction
	{
		public override void OnEnter()
		{
			InputIcon inputIcon = this.InputIconComponet.Value as InputIcon;
			inputIcon.action = (int)this.enumAction.RawValue;
			inputIcon.axisCheck = (InputIcon.AxisCheck)this.enumAxis.RawValue;
			inputIcon.RefreshIcon();
			base.Finish();
		}

		[ObjectType(typeof(InputIcon))]
		public FsmObject InputIconComponet;

		[ObjectType(typeof(RewiredEnum))]
		public FsmEnum enumAction;

		[ObjectType(typeof(InputIcon.AxisCheck))]
		public FsmEnum enumAxis;
	}
}
