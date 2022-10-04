using System;
using Framework.Managers;
using Gameplay.UI.Others.MenuLogic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Show a Message")]
	public class ShowMessage : FsmStateAction
	{
		public override void OnEnter()
		{
			string text = (this.textId == null) ? string.Empty : this.textId.Value;
			if (string.IsNullOrEmpty(text))
			{
				base.LogWarning("PlayMaker Action Show Message - textId title is blank");
				base.Finish();
				return;
			}
			if ("MSG_0501,MSG_BAPTISMAL,MSG_1801".Contains(text))
			{
				base.Finish();
				return;
			}
			bool blockPlayer = this.blockplayer != null && this.blockplayer.Value;
			int num = (this.line == null) ? 0 : this.line.Value;
			float num2 = (this.timeToWait == null) ? 0f : this.timeToWait.Value;
			string eventSound = (this.soundId == null) ? string.Empty : this.soundId.Value;
			if (this.IsModal)
			{
				PopUpWidget.OnDialogClose += this.DialogClose;
			}
			bool flag = Core.Dialog.ShowMessage(text, num, eventSound, num2, blockPlayer);
			if (num2 <= 0f && (!flag || !this.IsModal))
			{
				base.Finish();
				return;
			}
			this.timeLeft = num2;
		}

		public override void OnUpdate()
		{
			this.timeLeft -= Time.deltaTime;
			if (this.timeLeft < 0f)
			{
				this.timeLeft = 0f;
				base.Finish();
			}
		}

		public override void OnExit()
		{
			if (!this.IsModal)
			{
				PopUpWidget.OnDialogClose -= this.DialogClose;
			}
		}

		private void DialogClose()
		{
			base.Finish();
		}

		public FsmString category;

		public FsmString textId;

		public FsmInt line = 0;

		public FsmBool blockplayer;

		public FsmFloat timeToWait;

		public FsmString soundId;

		private bool IsModal;

		private float timeLeft;
	}
}
