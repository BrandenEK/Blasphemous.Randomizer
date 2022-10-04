using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Induces the game into dialog mode.")]
	public class DialogStart : FsmStateAction
	{
		public override void Reset()
		{
			if (this.conversation != null)
			{
				this.conversation.Value = string.Empty;
			}
			if (this.modal == null)
			{
				this.modal = new FsmBool();
			}
			if (this.useOnlyLast == null)
			{
				this.useOnlyLast = new FsmBool();
			}
			if (this.dontCloseWidetAtEnd == null)
			{
				this.dontCloseWidetAtEnd = new FsmBool();
			}
			if (this.useFullScreenBackgound == null)
			{
				this.useFullScreenBackgound = new FsmBool();
			}
			if (this.remainDialogueMode == null)
			{
				this.remainDialogueMode = new FsmBool();
			}
			if (this.enablePlayerDialogueMode == null)
			{
				this.enablePlayerDialogueMode = new FsmBool();
			}
			this.modal.Value = true;
			this.dontCloseWidetAtEnd.Value = false;
			this.useOnlyLast.Value = false;
			this.useFullScreenBackgound.Value = false;
			this.remainDialogueMode.Value = false;
			this.enablePlayerDialogueMode = true;
			if (this.purge == null)
			{
				this.purge = new FsmFloat();
			}
			this.purge.Value = 0f;
		}

		public override void OnEnter()
		{
			string text = (this.conversation == null) ? string.Empty : this.conversation.Value;
			bool flag = this.modal == null || this.modal.Value;
			bool flag2 = this.useOnlyLast != null && this.useOnlyLast.Value;
			bool flag3 = this.dontCloseWidetAtEnd != null && this.dontCloseWidetAtEnd.Value;
			bool useBackground = this.useFullScreenBackgound != null && this.useFullScreenBackgound.Value;
			if (string.IsNullOrEmpty(text))
			{
				base.LogWarning("PlayMaker Action Start Conversation - conversation title is blank");
			}
			else if (Core.Dialog.StartConversation(text, flag, flag2, !flag3, (int)this.purge.Value, useBackground))
			{
				Core.Dialog.OnDialogFinished += this.DialogEnded;
			}
			Core.Logic.Penitent.Animator.SetBool("IS_DIALOGUE_MODE", this.enablePlayerDialogueMode.Value);
			if (!this.enablePlayerDialogueMode.Value)
			{
				this.remainDialogueMode.Value = false;
			}
		}

		public void DialogEnded(string id, int response)
		{
			Core.Dialog.OnDialogFinished -= this.DialogEnded;
			Core.Logic.Penitent.Animator.SetBool("IS_DIALOGUE_MODE", this.dontCloseWidetAtEnd.Value || this.remainDialogueMode.Value);
			switch (response)
			{
			case 0:
				base.Fsm.Event(this.answer1);
				break;
			case 1:
				base.Fsm.Event(this.answer2);
				break;
			case 2:
				base.Fsm.Event(this.answer3);
				break;
			case 3:
				base.Fsm.Event(this.answer4);
				break;
			case 4:
				base.Fsm.Event(this.answer5);
				break;
			default:
				base.Finish();
				break;
			}
		}

		[RequiredField]
		[Tooltip("The conversation to start")]
		public FsmString conversation;

		public FsmString category;

		public FsmBool modal;

		public FsmBool useOnlyLast;

		public FsmBool dontCloseWidetAtEnd;

		public FsmBool useFullScreenBackgound;

		public FsmBool remainDialogueMode;

		public FsmBool enablePlayerDialogueMode;

		public FsmFloat purge = 0f;

		public FsmEvent answer1;

		public FsmEvent answer2;

		public FsmEvent answer3;

		public FsmEvent answer4;

		public FsmEvent answer5;
	}
}
