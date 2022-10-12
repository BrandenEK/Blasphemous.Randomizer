using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewInventoryWidget : SerializedMonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event NewInventoryWidget.TeleportCancelDelegate OnTeleportCancelled;

		private void Awake()
		{
			this.currentMenuState = NewInventoryWidget.MenuState.OFF;
			this.animator = base.GetComponent<Animator>();
			foreach (KeyValuePair<NewInventoryWidget.TabType, NewInventoryWidget.TypeConfig> keyValuePair in this.TypeConfiguration)
			{
				if (keyValuePair.Value.layout != null)
				{
					keyValuePair.Value.layout.enabled = true;
					keyValuePair.Value.layout.gameObject.SetActive(true);
				}
			}
			int num = 1;
			IEnumerator enumerator2 = Enum.GetValues(typeof(NewInventoryWidget.TabType)).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object obj = enumerator2.Current;
					NewInventoryWidget.TabType key = (NewInventoryWidget.TabType)obj;
					string name = this.tabPrefix + num.ToString();
					Transform transform = this.tabParent.transform.Find(name);
					this.TypeConfiguration[key].cachedButtonOn = transform.Find("ButtonOn").gameObject;
					this.TypeConfiguration[key].cachedButtonOff = transform.Find("ButtonOff").gameObject;
					num++;
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator2 as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void Show(bool p_active)
		{
			if (p_active && this.IsGUILocked())
			{
				return;
			}
			Core.Input.SetBlocker("INVENTORY", p_active);
			if (p_active)
			{
				this.SetState(NewInventoryWidget.MenuState.Normal);
				NewInventoryWidget.TabType tabType = this.lastOpenTabType;
				bool anyLastUsedObjectUntilLastCalled = Core.InventoryManager.AnyLastUsedObjectUntilLastCalled;
				if (anyLastUsedObjectUntilLastCalled)
				{
					switch (Core.InventoryManager.LastAddedObjectType)
					{
					case InventoryManager.ItemType.Relic:
						tabType = NewInventoryWidget.TabType.Reliquary;
						break;
					case InventoryManager.ItemType.Prayer:
						tabType = NewInventoryWidget.TabType.Prayers;
						break;
					case InventoryManager.ItemType.Bead:
						tabType = NewInventoryWidget.TabType.Rosary;
						break;
					case InventoryManager.ItemType.Quest:
						tabType = NewInventoryWidget.TabType.Quest;
						break;
					case InventoryManager.ItemType.Collectible:
						tabType = NewInventoryWidget.TabType.Collectables;
						break;
					case InventoryManager.ItemType.Sword:
						tabType = NewInventoryWidget.TabType.Sword;
						break;
					}
				}
				this.SelectTab(tabType, false);
				Core.Logic.PauseGame();
				int slotPosition = this.currentLayout.GetLastSlotSelected();
				if (anyLastUsedObjectUntilLastCalled)
				{
					slotPosition = this.currentLayout.GetItemPosition(Core.InventoryManager.LastAddedObject);
				}
				this.currentLayout.RestoreSlotPosition(slotPosition);
			}
			else
			{
				if (this.currentLayout != null)
				{
					this.lastSlot = this.currentLayout.GetLastSlotSelected();
				}
				if (this.currentLayout != null && this.currentLayout is NewInventory_LayoutSkill)
				{
					(this.currentLayout as NewInventory_LayoutSkill).CancelEditMode();
				}
				this.SetState(NewInventoryWidget.MenuState.OFF);
				this.lastOpenTabType = this.currentTabType;
				EventSystem.current.SetSelectedGameObject(null);
				Core.Logic.ResumeGame();
			}
		}

		public void ShowSkills(bool p_active)
		{
			if (p_active && this.IsGUILocked())
			{
				return;
			}
			Core.Input.SetBlocker("INVENTORY", p_active);
			if (p_active)
			{
				this.SetState(NewInventoryWidget.MenuState.UnlockSkills);
				this.SelectTab(NewInventoryWidget.TabType.Abilities, false);
				Core.Logic.PauseGame();
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
				this.SetState(NewInventoryWidget.MenuState.OFF);
				Core.Logic.ResumeGame();
			}
		}

		public void GoBack()
		{
			bool flag = true;
			NewInventoryWidget.MenuState menuState = this.currentMenuState;
			if (menuState != NewInventoryWidget.MenuState.Normal && menuState != NewInventoryWidget.MenuState.UnlockSkills)
			{
				if (menuState == NewInventoryWidget.MenuState.Lore)
				{
					this.SetState(this.previoudMenuState);
					this.currentLayout.RestoreFromLore();
				}
			}
			else if (this.currentLayout.CanGoBack())
			{
				this.Show(false);
			}
			else
			{
				flag = false;
			}
			if (flag && this.soundBack != string.Empty)
			{
				Core.Audio.PlayOneShot(this.soundBack, default(Vector3));
			}
		}

		public bool currentlyActive
		{
			get
			{
				return this.currentMenuState != NewInventoryWidget.MenuState.OFF;
			}
		}

		public void SetTab(NewInventoryWidget.TabType tab)
		{
			this.lastOpenTabType = tab;
		}

		public void ShowLore()
		{
			if ((this.currentMenuState == NewInventoryWidget.MenuState.Normal || this.currentMenuState == NewInventoryWidget.MenuState.UnlockSkills) && this.currentLayout != null && this.currentLayout.CanLore())
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				this.currentLayout.GetSelectedLoreData(out empty2, out empty);
				if (empty != string.Empty)
				{
					if (this.soundLore != string.Empty)
					{
						Core.Audio.PlayOneShot(this.soundLore, default(Vector3));
					}
					Core.Metrics.CustomEvent("LORE_BUTTON_PRESSED", string.Empty, -1f);
					this.loreCaption.text = empty2;
					this.loreDescription.text = empty;
					this.loreScroll.NewContentSetted();
					EventSystem.current.SetSelectedGameObject(null);
					this.SetState(NewInventoryWidget.MenuState.Lore);
				}
			}
		}

		public void SelectNextCategory()
		{
			if (this.currentMenuState != NewInventoryWidget.MenuState.Normal)
			{
				return;
			}
			int num = this.tabOrder.FindIndex((NewInventoryWidget.TabType x) => x == this.currentTabType);
			NewInventoryWidget.TabType tabType = this.tabOrder[(num + 1) % this.tabOrder.Count];
			this.lastSlot = 0;
			this.SelectTab(tabType, true);
		}

		public void SelectPreviousCategory()
		{
			if (this.currentMenuState != NewInventoryWidget.MenuState.Normal)
			{
				return;
			}
			int num = this.tabOrder.FindIndex((NewInventoryWidget.TabType x) => x == this.currentTabType);
			int num2 = num - 1;
			if (num2 < 0)
			{
				num2 = this.tabOrder.Count - 1;
			}
			NewInventoryWidget.TabType tabType = this.tabOrder[num2];
			this.lastSlot = 0;
			this.SelectTab(tabType, true);
		}

		private void SetState(NewInventoryWidget.MenuState state)
		{
			this.previoudMenuState = this.currentMenuState;
			this.currentMenuState = state;
			this.animator.SetInteger("STATE", (int)this.currentMenuState);
			this.normalHeader.SetActive(this.currentMenuState == NewInventoryWidget.MenuState.Normal || this.currentMenuState == NewInventoryWidget.MenuState.Lore);
			this.unlockHeader.SetActive(this.currentMenuState == NewInventoryWidget.MenuState.UnlockSkills);
		}

		private void SelectTab(NewInventoryWidget.TabType tabType, bool playSound = true)
		{
			foreach (KeyValuePair<NewInventoryWidget.TabType, NewInventoryWidget.TypeConfig> keyValuePair in this.TypeConfiguration)
			{
				if (keyValuePair.Value.layout != null)
				{
					keyValuePair.Value.layout.gameObject.SetActive(false);
				}
			}
			this.currentTabType = tabType;
			NewInventoryWidget.TypeConfig typeConfig = this.TypeConfiguration[this.currentTabType];
			this.currentLayout = typeConfig.layout;
			this.currentLayout.gameObject.SetActive(true);
			this.currentLayout.ShowLayout(tabType, this.currentMenuState == NewInventoryWidget.MenuState.UnlockSkills);
			if (this.tabLabel && typeConfig.caption != null)
			{
				this.tabLabel.text = Core.Localization.Get(typeConfig.caption);
			}
			IEnumerator enumerator2 = Enum.GetValues(typeof(NewInventoryWidget.TabType)).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object obj = enumerator2.Current;
					NewInventoryWidget.TabType tabType2 = (NewInventoryWidget.TabType)obj;
					bool flag = tabType2 == this.currentTabType;
					this.TypeConfiguration[tabType2].cachedButtonOn.SetActive(flag);
					this.TypeConfiguration[tabType2].cachedButtonOff.SetActive(!flag);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator2 as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			if (playSound)
			{
				Core.Audio.PlayOneShot(this.soundChangeTab, default(Vector3));
			}
			this.currentLayout.RestoreSlotPosition(this.lastSlot);
		}

		private bool IsGUILocked()
		{
			return (Core.Input.InputBlocked && !Core.Input.HasBlocker("INTERACTABLE") && !Core.Input.HasBlocker("PLAYER_LOGIC")) || SceneManager.GetActiveScene().name == "MainMenu" || FadeWidget.instance.Fading || UIController.instance.Paused;
		}

		private const string ANIMATOR_STATE = "STATE";

		[SerializeField]
		[BoxGroup("Common", true, false, 0)]
		private GameObject normalHeader;

		[SerializeField]
		[BoxGroup("Common", true, false, 0)]
		private GameObject unlockHeader;

		[SerializeField]
		[BoxGroup("Tabs", true, false, 0)]
		private Text tabLabel;

		[SerializeField]
		[BoxGroup("Tabs", true, false, 0)]
		private GameObject tabParent;

		[SerializeField]
		[BoxGroup("Tabs", true, false, 0)]
		private string tabPrefix = "Tab_";

		[SerializeField]
		[BoxGroup("Lore", true, false, 0)]
		private Text loreCaption;

		[SerializeField]
		[BoxGroup("Lore", true, false, 0)]
		private Text loreDescription;

		[SerializeField]
		[BoxGroup("Lore", true, false, 0)]
		private CustomScrollView loreScroll;

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundChangeTab = "event:/SFX/UI/ChangeTab";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundLore = "event:/SFX/UI/ChangeTab";

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundBack = "event:/SFX/UI/ChangeTab";

		[OdinSerialize]
		private Dictionary<NewInventoryWidget.TabType, NewInventoryWidget.TypeConfig> TypeConfiguration;

		private List<NewInventoryWidget.TabType> tabOrder = new List<NewInventoryWidget.TabType>
		{
			NewInventoryWidget.TabType.Rosary,
			NewInventoryWidget.TabType.Reliquary,
			NewInventoryWidget.TabType.Quest,
			NewInventoryWidget.TabType.Sword,
			NewInventoryWidget.TabType.Prayers,
			NewInventoryWidget.TabType.Abilities,
			NewInventoryWidget.TabType.Collectables
		};

		private NewInventoryWidget.MenuState currentMenuState;

		private NewInventoryWidget.MenuState previoudMenuState;

		private NewInventoryWidget.TabType currentTabType;

		private NewInventoryWidget.TabType lastOpenTabType;

		private int lastSlot;

		private NewInventory_Layout currentLayout;

		private Animator animator;

		public enum TabType
		{
			Rosary,
			Reliquary,
			Quest,
			Sword,
			Prayers,
			Abilities,
			Collectables
		}

		private enum MenuState
		{
			OFF,
			Normal,
			UnlockSkills,
			Lore
		}

		public enum EquipAction
		{
			None,
			Equip,
			UnEquip,
			Decipher
		}

		[Serializable]
		private class TypeConfig
		{
			public NewInventory_Layout layout;

			public string caption = string.Empty;

			[NonSerialized]
			public GameObject cachedButtonOn;

			[NonSerialized]
			public GameObject cachedButtonOff;
		}

		public delegate void TeleportCancelDelegate();
	}
}
