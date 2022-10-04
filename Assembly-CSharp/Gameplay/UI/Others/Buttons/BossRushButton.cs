using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.Buttons
{
	public class BossRushButton : MonoBehaviour
	{
		public void SetData(int idx, bool unlocked)
		{
			Button component = base.GetComponent<Button>();
			if (component != null)
			{
				component.interactable = unlocked;
			}
			if (this.EnableImage != null)
			{
				this.EnableImage.SetActive(unlocked);
			}
			if (this.LockedImage != null)
			{
				this.LockedImage.SetActive(!unlocked);
			}
			MenuButton component2 = base.GetComponent<MenuButton>();
			if (component2 != null)
			{
				component2.OnDeselect(null);
			}
		}

		[BoxGroup("Controls", true, false, 0)]
		public GameObject EnableImage;

		[BoxGroup("Controls", true, false, 0)]
		public GameObject LockedImage;
	}
}
