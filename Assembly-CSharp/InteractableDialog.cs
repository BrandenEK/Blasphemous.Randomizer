using System;
using Framework.Managers;
using UnityEngine;

public class InteractableDialog : MonoBehaviour
{
	private void OnUsePost()
	{
		Core.Dialog.StartConversation(this.conversation, this.modal, this.onlyLastLine, true, 0, false);
	}

	public string conversation;

	public bool modal = true;

	public bool onlyLastLine;
}
