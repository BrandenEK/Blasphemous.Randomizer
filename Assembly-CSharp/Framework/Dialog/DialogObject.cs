using System;
using System.Collections.Generic;
using Framework.Managers;

namespace Framework.Dialog
{
	public class DialogObject : BaseObject
	{
		public override string GetPrefix()
		{
			return "DLG_";
		}

		public bool useOverrideAudioKey;

		public string overrideKey = string.Empty;

		public bool modalDialog = true;

		public bool modalBoss;

		public DialogObject.DialogType dialogType;

		public List<string> dialogLines = new List<string>();

		public InventoryManager.ItemType itemType;

		public string item = string.Empty;

		public string externalAnswersId = string.Empty;

		public List<string> answersLines = new List<string>();

		public enum DialogType
		{
			Lines,
			GiveObject,
			GivePurge,
			BuyObject,
			PurgeGeneric
		}
	}
}
