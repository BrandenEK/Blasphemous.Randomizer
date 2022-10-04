using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if tip is unlocked.")]
	public class IsTipUnlocked : FsmStateAction
	{
		public override void Reset()
		{
			if (this.popupId == null)
			{
				this.popupId = new FsmString();
				this.popupId.UseVariable = false;
			}
			this.popupId.Value = string.Empty;
		}

		public override void OnEnter()
		{
			string text = (this.popupId == null) ? string.Empty : this.popupId.Value;
			if (string.IsNullOrEmpty(text))
			{
				base.LogWarning("PlayMaker Action IsTipUnlocked - popupId is blank");
			}
			else if (Core.TutorialManager.IsTutorialUnlocked(text))
			{
				base.Fsm.Event(this.popupUnlocked);
			}
			else
			{
				base.Fsm.Event(this.popupLocked);
			}
			base.Finish();
		}

		public FsmString popupId;

		public FsmEvent popupLocked;

		public FsmEvent popupUnlocked;
	}
}
