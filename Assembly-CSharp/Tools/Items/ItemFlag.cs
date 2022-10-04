using System;
using Framework.Inventory;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Items
{
	public class ItemFlag : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			Core.Events.SetFlag(this.flagName, true, false);
			return true;
		}

		protected override void OnRemoveEffect()
		{
			Core.Events.SetFlag(this.flagName, false, false);
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private string flagName;
	}
}
