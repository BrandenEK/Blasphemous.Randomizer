using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Sends onSuccess event if the skin is unlocked. Otherwise returns onFailure")]
	public class IsSkinUnlock : FsmStateAction
	{
		public override void OnEnter()
		{
			bool flag = Core.ColorPaletteManager.IsColorPaletteUnlocked(this.skinId.Value);
			base.Fsm.Event((!flag) ? this.onFailure : this.onSuccess);
			base.Finish();
		}

		public FsmString skinId;

		public FsmEvent onSuccess;

		public FsmEvent onFailure;
	}
}
