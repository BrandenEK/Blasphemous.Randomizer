using System;
using Gameplay.UI;
using UnityEngine;

namespace Framework.Inventory
{
	public class InteractableInventoryShowUnlockSkills : MonoBehaviour
	{
		private void OnUsePost()
		{
			UIController.instance.ShowUnlockSKill();
		}
	}
}
