using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Condition
{
	[ActionCategory("Blasphemous Condition")]
	[Tooltip("Checks if the chosen DLCs is present.")]
	public class IsDLCDownloaded : FsmStateAction
	{
		public override void Reset()
		{
			if (this.dlcId == null)
			{
				this.dlcId = new FsmString();
			}
			if (this.checkAgain == null)
			{
				this.checkAgain = new FsmBool();
			}
			this.dlcId.Value = string.Empty;
			this.checkAgain.Value = false;
		}

		public override void OnEnter()
		{
			string text = (this.dlcId == null) ? string.Empty : this.dlcId.Value;
			bool recheck = this.checkAgain != null && this.checkAgain.Value;
			if (string.IsNullOrEmpty(text))
			{
				base.LogWarning("PlayMaker Action IsDLCDownloaded - dlcId is blank");
			}
			if (Core.DLCManager.IsDLCDownloaded(text, recheck))
			{
				base.Fsm.Event(this.dlcAvailable);
			}
			else
			{
				base.Fsm.Event(this.dlcUnavailable);
			}
			base.Finish();
		}

		[RequiredField]
		public FsmString dlcId;

		public FsmBool checkAgain;

		public FsmEvent dlcAvailable;

		public FsmEvent dlcUnavailable;
	}
}
