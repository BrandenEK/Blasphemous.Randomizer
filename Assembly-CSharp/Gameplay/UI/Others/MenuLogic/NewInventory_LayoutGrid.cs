using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DG.Tweening;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI.Others.Buttons;
using Gameplay.UI.Others.UIGameLogic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class NewInventory_LayoutGrid : NewInventory_Layout
	{
		private void Awake()
		{
			this.ignoreSelectSound = false;
			if (this.objectsCached)
			{
				return;
			}
			this.cachedEquipped = new List<NewInventory_GridItem>();
			this.objectsCached = true;
			this.cachedGridElements = new List<NewInventory_GridItem>();
			if (this.gridElementBase && this.numGridElements > 0)
			{
				for (int i = 0; i < this.numGridElements; i++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.gridElementBase);
					gameObject.SetActive(true);
					gameObject.name = "Grid_Element" + i.ToString();
					gameObject.transform.SetParent(this.gridElementBase.transform.parent);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localPosition = Vector3.one;
					NewInventory_GridItem component = gameObject.GetComponent<NewInventory_GridItem>();
					component.SetObject(null);
					this.cachedGridElements.Add(component);
				}
			}
			if (this.NotUnequipableRoot)
			{
				this.NotUnequipableRoot.SetActive(false);
				for (int j = 0; j < 2; j++)
				{
					this.dialogResponseSelection[j] = this.dialogResponse[j].transform.Find("Img").gameObject;
					this.dialogResponseSelection[j].SetActive(false);
				}
				this.responsesUI = this.dialogResponse[0].transform.parent.gameObject;
				this.responsesUI.GetComponent<CanvasGroup>().alpha = 0f;
				this.responsesUI.SetActive(false);
			}
			this.InConfirmationNotUnequipable = false;
		}

		public override void RestoreFromLore()
		{
			base.StartCoroutine(this.FocusSlotSecure((this.currentSelected == -1) ? 0 : this.currentSelected, true));
		}

		public override void RestoreSlotPosition(int slotPosition)
		{
			slotPosition = Mathf.Clamp(slotPosition, 0, this.cachedGridElements.Count - 1);
			if (this.cachedGridElements[slotPosition].inventoryObject == null)
			{
				slotPosition = 0;
			}
			base.StartCoroutine(this.FocusSlotSecure(slotPosition, true));
		}

		public override int GetLastSlotSelected()
		{
			this.currentSelected = Mathf.Clamp(this.currentSelected, 0, this.cachedGridElements.Count - 1);
			return (!(this.cachedGridElements[this.currentSelected].inventoryObject != null)) ? 0 : this.currentSelected;
		}

		public override void ShowLayout(NewInventoryWidget.TabType tabType, bool editMode)
		{
			this.currentSelectedEquiped = -1;
			this.currentViewScroll = 0;
			this.pendingEquipableObject = null;
			if (this.scrollRect != null)
			{
				this.scrollRect.verticalNormalizedPosition = 1f;
				this.scrollBar.SetScrollbar(0f);
			}
			int num = this.currentSelected;
			if (this.cachedGridElements != null && (num < 0 || num >= this.cachedGridElements.Count))
			{
				num = 0;
			}
			if (this.description)
			{
				this.description.SetObject(null, NewInventoryWidget.EquipAction.None);
			}
			if (this.purgeControl)
			{
				bool flag = !Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH);
				this.purgeControl.transform.parent.gameObject.SetActive(flag);
				if (flag)
				{
					this.purgeControl.RefreshPoints(true);
				}
			}
			switch (tabType)
			{
			case NewInventoryWidget.TabType.Rosary:
				this.FillGridElements<RosaryBead>(InventoryManager.ItemType.Bead, Core.InventoryManager.GetRosaryBeadOwned());
				break;
			case NewInventoryWidget.TabType.Reliquary:
				this.FillGridElements<Relic>(InventoryManager.ItemType.Relic, Core.InventoryManager.GetRelicsOwned());
				break;
			case NewInventoryWidget.TabType.Quest:
				this.FillGridElements<QuestItem>(InventoryManager.ItemType.Quest, Core.InventoryManager.GetQuestItemOwned());
				break;
			case NewInventoryWidget.TabType.Sword:
				this.FillGridElements<Sword>(InventoryManager.ItemType.Sword, Core.InventoryManager.GetSwordsOwned());
				break;
			case NewInventoryWidget.TabType.Prayers:
				this.FillGridElements<Prayer>(InventoryManager.ItemType.Prayer, Core.InventoryManager.GetPrayersOwned());
				break;
			case NewInventoryWidget.TabType.Collectables:
				this.FillGridElements<Framework.Inventory.CollectibleItem>(InventoryManager.ItemType.Collectible, Core.InventoryManager.GetCollectibleItemOwned());
				break;
			}
			this.CacheEquipped();
			this.UpdateItemsBySwordHeartEquipability(tabType);
			if (this.scrollBar != null)
			{
				this.scrollBar.gameObject.SetActive(this.currentMaxScrolls > 1);
			}
			if (this.NotUnequipableRoot)
			{
				this.NotUnequipableRoot.SetActive(false);
			}
			base.StartCoroutine(this.FocusSlotSecure(num, false));
			this.UpdateEquipped(this.currentItemType);
			this.InConfirmationNotUnequipable = false;
		}

		private void UpdateItemsBySwordHeartEquipability(NewInventoryWidget.TabType tabType)
		{
			bool allowEquipSwords = Core.Logic.Penitent.AllowEquipSwords;
			bool flag = tabType == NewInventoryWidget.TabType.Sword;
			bool flag2 = !flag || (UIController.instance.CanEquipSwordHearts && (allowEquipSwords || Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH)));
			if (this.swordHeartText != null)
			{
				this.swordHeartText.SetActive(flag && !UIController.instance.CanEquipSwordHearts && allowEquipSwords);
			}
			if (this.swordHeartTextLocked != null)
			{
				this.swordHeartTextLocked.SetActive(flag && !allowEquipSwords);
			}
			foreach (NewInventory_GridItem newInventory_GridItem in this.cachedGridElements)
			{
				if (flag2)
				{
					newInventory_GridItem.DeactivateGrayscale();
				}
				else
				{
					newInventory_GridItem.ActivateGrayscale();
				}
			}
		}

		public override void GetSelectedLoreData(out string caption, out string lore)
		{
			BaseInventoryObject baseInventoryObject = null;
			if (this.currentSelected >= 0 && this.cachedGridElements[this.currentSelected] != null)
			{
				baseInventoryObject = this.cachedGridElements[this.currentSelected].inventoryObject;
			}
			if (baseInventoryObject != null && baseInventoryObject.HasLore())
			{
				caption = baseInventoryObject.caption;
				lore = baseInventoryObject.lore;
			}
			else
			{
				caption = string.Empty;
				lore = string.Empty;
			}
		}

		public override bool CanGoBack()
		{
			bool flag = !this.InConfirmationNotUnequipable;
			if (!flag)
			{
				this.HideNotUnequipableDialog();
			}
			return flag;
		}

		public override bool CanLore()
		{
			return !this.InConfirmationNotUnequipable;
		}

		public override int GetItemPosition(BaseInventoryObject item)
		{
			for (int i = 0; i < this.cachedGridElements.Count; i++)
			{
				if (this.cachedGridElements[i].inventoryObject == item)
				{
					return i;
				}
			}
			return this.GetLastSlotSelected();
		}

		public void ActivateGridElement(int slot)
		{
			if (this.InConfirmationNotUnequipable)
			{
				return;
			}
			BaseInventoryObject inventoryObject = this.cachedGridElements[slot].inventoryObject;
			if (!inventoryObject)
			{
				return;
			}
			NewInventoryWidget.EquipAction inventoryObjectAction = this.GetInventoryObjectAction(inventoryObject);
			if (inventoryObjectAction != NewInventoryWidget.EquipAction.Equip)
			{
				if (inventoryObjectAction != NewInventoryWidget.EquipAction.UnEquip)
				{
					if (inventoryObjectAction != NewInventoryWidget.EquipAction.Decipher)
					{
					}
				}
				else
				{
					this.UnEquipObject(inventoryObject);
				}
			}
			else if (inventoryObject.WillBlockSwords() && !Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH))
			{
				this.pendingEquipableObject = inventoryObject;
				this.pendingEquipableSlot = slot;
				this.ShowNotUnequipableDialog();
			}
			else
			{
				this.EquipObject(inventoryObject);
			}
			this.UpdateEquipped(this.currentItemType);
			this.SelectGridElement(slot, false);
		}

		public void SelectGridElement(int slot, bool playSound = true)
		{
			if (this.InConfirmationNotUnequipable)
			{
				return;
			}
			if (this.currentSelected != -1 && this.currentSelected < this.cachedGridElements.Count)
			{
				this.cachedGridElements[this.currentSelected].UpdateSelect(false);
			}
			this.currentSelected = slot;
			if (this.scrollRect && this.currentMaxScrolls > 1)
			{
				this.CheckScroll();
			}
			BaseInventoryObject inventoryObject = this.cachedGridElements[slot].inventoryObject;
			if (this.description)
			{
				NewInventoryWidget.EquipAction inventoryObjectAction = this.GetInventoryObjectAction(inventoryObject);
				this.description.SetObject(inventoryObject, inventoryObjectAction);
			}
			if (playSound)
			{
				if (this.ignoreSelectSound)
				{
					this.ignoreSelectSound = false;
				}
				else
				{
					Core.Audio.PlayOneShot(this.soundElementChange, default(Vector3));
				}
			}
			this.cachedGridElements[slot].UpdateStatus(true, true, this.IsEquipped(inventoryObject));
			this.CleanSelectedEquipped();
			if (inventoryObject)
			{
				this.UpdateEquipedSelected(inventoryObject.id);
			}
		}

		public void NoEquipableOptionSelected(int response)
		{
			if (this.WaitingToCloseNotUnequipable)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				this.dialogResponseSelection[i].SetActive(i == response);
				Text componentInChildren = this.dialogResponseSelection[i].transform.parent.GetComponentInChildren<Text>();
				componentInChildren.color = ((i != response) ? this.optionNormalColor : this.optionHighligterColor);
			}
		}

		public void NoEquipableResponsePressed(int response)
		{
			if (this.WaitingToCloseNotUnequipable)
			{
				return;
			}
			if (response == 0)
			{
				this.EquipObject(this.pendingEquipableObject);
				this.UpdateEquipped(this.currentItemType);
				this.UpdateItemsBySwordHeartEquipability(NewInventoryWidget.TabType.Sword);
			}
			this.HideNotUnequipableDialog();
		}

		private void CheckScroll()
		{
			int num = Mathf.FloorToInt((float)this.currentSelected / (float)this.colsInGrid) + 1;
			int num2 = Mathf.CeilToInt((float)num / (float)this.visibleRowsForScroll);
			if (this.currentViewScroll != num2)
			{
				this.currentViewScroll = num2;
				float scrollbar = (float)((this.currentViewScroll - 1) / (this.currentMaxScrolls - 1));
				this.scrollBar.SetScrollbar(scrollbar);
				float num3 = (float)((this.currentViewScroll - 1) * this.visibleRowsForScroll) * this.cellHeightForScroll;
				this.scrollRect.content.transform.localPosition = new Vector3(this.scrollRect.content.transform.localPosition.x, num3, this.scrollRect.content.transform.localPosition.z);
			}
		}

		private void CleanSelectedEquipped()
		{
			if (this.currentSelectedEquiped != -1 && this.currentSelectedEquiped < this.cachedEquipped.Count)
			{
				this.cachedEquipped[this.currentSelectedEquiped].UpdateStatus(true, false, false);
				this.currentSelectedEquiped = -1;
			}
		}

		private void UpdateItemById(string id)
		{
			for (int i = 0; i < this.cachedGridElements.Count; i++)
			{
				NewInventory_GridItem newInventory_GridItem = this.cachedGridElements[i];
				if (newInventory_GridItem.inventoryObject && newInventory_GridItem.inventoryObject.id == id)
				{
					newInventory_GridItem.UpdateStatus(true, false, false);
				}
			}
		}

		private void UpdateEquipedSelected(string id)
		{
			for (int i = 0; i < this.cachedEquipped.Count; i++)
			{
				NewInventory_GridItem newInventory_GridItem = this.cachedEquipped[i];
				if (newInventory_GridItem.inventoryObject && newInventory_GridItem.inventoryObject.id == id)
				{
					newInventory_GridItem.UpdateStatus(true, true, true);
					this.currentSelectedEquiped = i;
				}
			}
		}

		private void UpdateEquipped(InventoryManager.ItemType itemType)
		{
			for (int i = 0; i < this.cachedEquipped.Count; i++)
			{
				NewInventory_GridItem newInventory_GridItem = this.cachedEquipped[i];
				BaseInventoryObject @object = null;
				switch (itemType)
				{
				case InventoryManager.ItemType.Relic:
					@object = Core.InventoryManager.GetRelicInSlot(i);
					break;
				case InventoryManager.ItemType.Prayer:
					@object = Core.InventoryManager.GetPrayerInSlot(i);
					break;
				case InventoryManager.ItemType.Bead:
					@object = Core.InventoryManager.GetRosaryBeadInSlot(i);
					break;
				case InventoryManager.ItemType.Sword:
					@object = Core.InventoryManager.GetSwordInSlot(i);
					break;
				}
				newInventory_GridItem.SetObject(@object);
				newInventory_GridItem.UpdateStatus(i < this.availableEquipables, this.currentSelectedEquiped == i, this.currentSelectedEquiped == i);
			}
		}

		private void ShowMaxSlotsForCurrentTabType()
		{
			if (this.currentTypeConfiguration.slots <= 0)
			{
				return;
			}
			for (int i = 0; i < this.cachedGridElements.Count; i++)
			{
				NewInventory_GridItem newInventory_GridItem = this.cachedGridElements[i];
				bool active = i < this.currentTypeConfiguration.slots;
				newInventory_GridItem.gameObject.SetActive(active);
			}
		}

		private void FillGridElements<T>(InventoryManager.ItemType type, ReadOnlyCollection<T> list) where T : BaseInventoryObject
		{
			this.currentItemType = type;
			this.currentTypeConfiguration = this.typeConfiguration[this.currentItemType];
			this.ShowMaxSlotsForCurrentTabType();
			int num = 0;
			int i = 0;
			int j = 0;
			int num2 = this.numGridElements / this.colsInGrid;
			int[,] array = new int[this.colsInGrid, num2];
			for (int k = 0; k < this.cachedGridElements.Count; k++)
			{
				NewInventory_GridItem newInventory_GridItem = this.cachedGridElements[k];
				T t = (T)((object)null);
				if (num < list.Count)
				{
					t = list[num];
					num++;
				}
				array[j, i] = k;
				int elementNumber = k;
				EventsButton button = newInventory_GridItem.Button;
				button.onClick = new EventsButton.ButtonClickedEvent();
				button.onClick.AddListener(delegate()
				{
					this.ActivateGridElement(elementNumber);
				});
				button.onSelected = new EventsButton.ButtonSelectedEvent();
				button.onSelected.AddListener(delegate()
				{
					this.SelectGridElement(elementNumber, true);
				});
				newInventory_GridItem.Button.interactable = true;
				newInventory_GridItem.SetObject(t);
				newInventory_GridItem.UpdateStatus(base.enabled, false, this.IsEquipped(t));
				j++;
				if (j >= this.colsInGrid)
				{
					j = 0;
					i++;
				}
			}
			int num3 = Mathf.CeilToInt((float)list.Count / (float)this.colsInGrid);
			this.currentMaxScrolls = Mathf.CeilToInt((float)num3 / (float)this.visibleRowsForScroll);
			if (this.currentMaxScrolls == 0)
			{
				this.currentMaxScrolls = 1;
			}
			int num4 = this.currentMaxScrolls * this.visibleRowsForScroll;
			for (j = 0; j < this.colsInGrid; j++)
			{
				for (i = 0; i < num4; i++)
				{
					int slot = array[j, i];
					this.LinkNavigation(slot, array, j, i, num4);
				}
			}
			if (type != InventoryManager.ItemType.Quest)
			{
				this.LinkLastSlotToLastRowFirstSlot();
			}
		}

		private void LinkLastSlotToLastRowFirstSlot()
		{
			int slots = this.currentTypeConfiguration.slots;
			int num = this.colsInGrid * (slots / this.colsInGrid);
			Navigation navigation = this.cachedGridElements[num].Button.navigation;
			int num2 = slots - 1;
			Navigation navigation2 = this.cachedGridElements[num2].Button.navigation;
			navigation.selectOnLeft = this.GetActiveButton(num2);
			navigation2.selectOnRight = this.GetActiveButton(num);
			this.cachedGridElements[num].Button.navigation = navigation;
			this.cachedGridElements[num2].Button.navigation = navigation2;
		}

		private void LinkNavigation(int slot, int[,] indexgrid, int col, int row, int maxRow)
		{
			Navigation navigation = this.cachedGridElements[slot].Button.navigation;
			int num = col - 1;
			if (num < 0)
			{
				num = this.colsInGrid - 1;
			}
			int slot2 = indexgrid[num, row];
			navigation.selectOnLeft = this.GetActiveButton(slot2);
			num = col + 1;
			if (num >= this.colsInGrid)
			{
				num = 0;
			}
			slot2 = indexgrid[num, row];
			navigation.selectOnRight = this.GetActiveButton(slot2);
			int num2 = row - 1;
			if (num2 < 0)
			{
				num2 = maxRow - 1;
			}
			slot2 = indexgrid[col, num2];
			navigation.selectOnUp = this.GetActiveButton(slot2);
			num2 = row + 1;
			if (num2 >= maxRow)
			{
				num2 = 0;
			}
			slot2 = indexgrid[col, num2];
			navigation.selectOnDown = this.GetActiveButton(slot2);
			this.cachedGridElements[slot].Button.navigation = navigation;
		}

		private EventsButton GetActiveButton(int slot)
		{
			EventsButton eventsButton = this.cachedGridElements[slot].Button;
			if (!eventsButton.interactable)
			{
				eventsButton = null;
			}
			return eventsButton;
		}

		private void CacheEquipped()
		{
			this.cachedEquipped.Clear();
			if (this.typeConfiguration == null)
			{
				return;
			}
			this.availableEquipables = ((this.currentItemType != InventoryManager.ItemType.Bead) ? 9999 : Core.InventoryManager.GetRosaryBeadSlots());
			foreach (KeyValuePair<InventoryManager.ItemType, NewInventory_LayoutGrid.TypeConfiguration> keyValuePair in this.typeConfiguration)
			{
				bool flag = keyValuePair.Key == this.currentItemType;
				if (keyValuePair.Value.rootLayout)
				{
					keyValuePair.Value.rootLayout.SetActive(flag);
				}
				if (flag && keyValuePair.Value.equipablesRoot)
				{
					for (int i = 0; i < keyValuePair.Value.equipablesRoot.transform.childCount; i++)
					{
						Transform child = keyValuePair.Value.equipablesRoot.transform.GetChild(i);
						NewInventory_GridItem component = child.GetComponent<NewInventory_GridItem>();
						if (component)
						{
							this.cachedEquipped.Add(component);
						}
					}
				}
			}
		}

		private IEnumerator FocusSlotSecure(int slot, bool ignoreSound = true)
		{
			slot = Mathf.Clamp(slot, 0, this.cachedGridElements.Count - 1);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			int maxSlot = this.currentMaxScrolls * this.visibleRowsForScroll * this.colsInGrid;
			if (slot >= maxSlot)
			{
				int num = Mathf.FloorToInt((float)slot / (float)this.colsInGrid) + 1;
				int num2 = Mathf.CeilToInt((float)num / (float)this.visibleRowsForScroll);
				slot -= (num2 - 1) * this.visibleRowsForScroll * this.colsInGrid;
			}
			GameObject focusElement = this.cachedGridElements[slot].Button.gameObject;
			this.ignoreSelectSound = ignoreSound;
			EventSystem.current.SetSelectedGameObject(focusElement);
			this.SelectGridElement(slot, false);
			yield break;
		}

		private void ShowNotUnequipableDialog()
		{
			this.InConfirmationNotUnequipable = true;
			this.WaitingToCloseNotUnequipable = false;
			this.NotUnequipableRoot.SetActive(true);
			this.responsesUI.SetActive(true);
			CanvasGroup component = this.responsesUI.GetComponent<CanvasGroup>();
			component.alpha = 0f;
			CanvasGroup component2 = this.NotUnequipableRoot.GetComponent<CanvasGroup>();
			component2.alpha = 0f;
			base.StartCoroutine(this.ShowFirstSecure());
			DOTween.defaultTimeScaleIndependent = true;
			TweenExtensions.Play<Sequence>(TweenSettingsExtensions.Append(TweenSettingsExtensions.AppendInterval(TweenSettingsExtensions.Append(DOTween.Sequence(), ShortcutExtensions46.DOFade(component2, 1f, this.NotUnequipableFadeTime)), this.NotUnequipableWaitTime), ShortcutExtensions46.DOFade(component, 1f, this.NotUnequipableFadeResponseTime)));
		}

		private void HideNotUnequipableDialog()
		{
			this.WaitingToCloseNotUnequipable = true;
			CanvasGroup component = this.NotUnequipableRoot.GetComponent<CanvasGroup>();
			DOTween.defaultTimeScaleIndependent = true;
			TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions46.DOFade(component, 0f, this.NotUnequipableEndFadeTime), delegate()
			{
				this.NotUnequipableRoot.SetActive(false);
				this.InConfirmationNotUnequipable = false;
				this.WaitingToCloseNotUnequipable = false;
				base.StartCoroutine(this.FocusSlotSecure(this.pendingEquipableSlot, true));
			});
		}

		public IEnumerator ShowFirstSecure()
		{
			yield return new WaitForFixedUpdate();
			EventSystem.current.SetSelectedGameObject(this.dialogResponse[1].gameObject);
			yield return new WaitForFixedUpdate();
			EventSystem.current.SetSelectedGameObject(this.dialogResponse[0].gameObject);
			this.NoEquipableOptionSelected(0);
			yield break;
		}

		private void EquipObject(BaseInventoryObject obj)
		{
			if (this.currentItemType == InventoryManager.ItemType.Prayer || this.currentItemType == InventoryManager.ItemType.Sword)
			{
				InventoryManager.ItemType itemType = this.currentItemType;
				if (itemType != InventoryManager.ItemType.Prayer)
				{
					if (itemType == InventoryManager.ItemType.Sword)
					{
						Sword swordInSlot = Core.InventoryManager.GetSwordInSlot(0);
						if (swordInSlot != null)
						{
							this.UnEquipObject(swordInSlot);
							this.UpdateItemById(swordInSlot.id);
						}
						Core.InventoryManager.SetSwordInSlot(0, obj.id);
					}
				}
				else
				{
					Prayer prayerInSlot = Core.InventoryManager.GetPrayerInSlot(0);
					if (prayerInSlot != null)
					{
						this.UnEquipObject(prayerInSlot);
						this.UpdateItemById(prayerInSlot.id);
					}
					Core.InventoryManager.SetPrayerInSlot(0, obj.id);
				}
			}
			else
			{
				int firstEmptySlot = this.GetFirstEmptySlot();
				if (firstEmptySlot < 0)
				{
					return;
				}
				InventoryManager.ItemType itemType2 = this.currentItemType;
				if (itemType2 != InventoryManager.ItemType.Bead)
				{
					if (itemType2 == InventoryManager.ItemType.Relic)
					{
						Core.InventoryManager.SetRelicInSlot(firstEmptySlot, obj.id);
					}
				}
				else
				{
					Core.InventoryManager.SetRosaryBeadInSlot(firstEmptySlot, obj.id);
				}
			}
			if (this.equipSounds.ContainsKey(this.currentItemType))
			{
				string text = this.equipSounds[this.currentItemType];
				if (text != string.Empty)
				{
					Core.Audio.PlayOneShot(text, default(Vector3));
				}
			}
		}

		private void UnEquipObject(BaseInventoryObject obj)
		{
			if (this.InConfirmationNotUnequipable)
			{
				return;
			}
			switch (this.currentItemType)
			{
			case InventoryManager.ItemType.Relic:
			{
				int slot = Core.InventoryManager.GetRelicSlot((Relic)obj);
				Core.InventoryManager.SetRelicInSlot(slot, null);
				break;
			}
			case InventoryManager.ItemType.Prayer:
			{
				int slot = Core.InventoryManager.GetPrayerInSlot((Prayer)obj);
				Core.InventoryManager.SetPrayerInSlot(slot, null);
				break;
			}
			case InventoryManager.ItemType.Bead:
			{
				int slot = Core.InventoryManager.GetRosaryBeadSlot((RosaryBead)obj);
				Core.InventoryManager.SetRosaryBeadInSlot(slot, null);
				break;
			}
			case InventoryManager.ItemType.Sword:
			{
				int slot = Core.InventoryManager.GetSwordInSlot((Sword)obj);
				Core.InventoryManager.SetSwordInSlot(slot, null);
				break;
			}
			}
			if (this.unequipSounds.ContainsKey(this.currentItemType))
			{
				string text = this.unequipSounds[this.currentItemType];
				if (text != string.Empty)
				{
					Core.Audio.PlayOneShot(text, default(Vector3));
				}
			}
		}

		private NewInventoryWidget.EquipAction GetInventoryObjectAction(BaseInventoryObject obj)
		{
			NewInventoryWidget.EquipAction result = NewInventoryWidget.EquipAction.None;
			if (obj)
			{
				if (this.IsEquipped(obj))
				{
					result = NewInventoryWidget.EquipAction.UnEquip;
				}
				else if (this.currentItemType == InventoryManager.ItemType.Prayer || this.currentItemType == InventoryManager.ItemType.Sword)
				{
					result = NewInventoryWidget.EquipAction.Equip;
				}
				else
				{
					int firstEmptySlot = this.GetFirstEmptySlot();
					if (firstEmptySlot != -1)
					{
						result = NewInventoryWidget.EquipAction.Equip;
					}
				}
			}
			if (obj is Sword && (!UIController.instance.CanEquipSwordHearts || (!Core.Logic.Penitent.AllowEquipSwords && !Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH))))
			{
				result = NewInventoryWidget.EquipAction.None;
			}
			return result;
		}

		private bool IsEquipped(BaseInventoryObject obj)
		{
			bool result = false;
			if (obj)
			{
				switch (this.currentItemType)
				{
				case InventoryManager.ItemType.Relic:
					result = Core.InventoryManager.IsRelicEquipped(obj.id);
					break;
				case InventoryManager.ItemType.Prayer:
					result = Core.InventoryManager.IsPrayerEquipped(obj.id);
					break;
				case InventoryManager.ItemType.Bead:
					result = Core.InventoryManager.IsRosaryBeadEquipped(obj.id);
					break;
				case InventoryManager.ItemType.Sword:
					result = Core.InventoryManager.IsSwordEquipped(obj.id);
					break;
				}
			}
			return result;
		}

		private int GetFirstEmptySlot()
		{
			int result = -1;
			int num = 0;
			switch (this.currentItemType)
			{
			case InventoryManager.ItemType.Relic:
				num = 3;
				break;
			case InventoryManager.ItemType.Prayer:
				num = 1;
				break;
			case InventoryManager.ItemType.Bead:
				num = Core.InventoryManager.GetRosaryBeadSlots();
				break;
			case InventoryManager.ItemType.Sword:
				num = 1;
				break;
			}
			for (int i = 0; i < num; i++)
			{
				BaseInventoryObject baseInventoryObject = null;
				switch (this.currentItemType)
				{
				case InventoryManager.ItemType.Relic:
					baseInventoryObject = Core.InventoryManager.GetRelicInSlot(i);
					break;
				case InventoryManager.ItemType.Prayer:
					baseInventoryObject = Core.InventoryManager.GetPrayerInSlot(i);
					break;
				case InventoryManager.ItemType.Bead:
					baseInventoryObject = Core.InventoryManager.GetRosaryBeadInSlot(i);
					break;
				case InventoryManager.ItemType.Sword:
					baseInventoryObject = Core.InventoryManager.GetSwordInSlot(i);
					break;
				}
				if (!baseInventoryObject)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private GameObject gridElementBase;

		[SerializeField]
		[BoxGroup("Controls", true, false, 0)]
		private NewInventory_Description description;

		[BoxGroup("Controls", true, false, 0)]
		[SerializeField]
		private PlayerPurgePoints purgeControl;

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private int numGridElements = 72;

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private int colsInGrid = 8;

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private int visibleRowsForScroll = 3;

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private float cellHeightForScroll = 33f;

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private ScrollRect scrollRect;

		[SerializeField]
		[BoxGroup("Grid", true, false, 0)]
		private FixedScrollBar scrollBar;

		[OdinSerialize]
		[BoxGroup("Type Configuration", true, false, 0)]
		private Dictionary<InventoryManager.ItemType, NewInventory_LayoutGrid.TypeConfiguration> typeConfiguration;

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string soundElementChange = "event:/SFX/UI/ChangeSelection";

		[SerializeField]
		[BoxGroup("Sounds by type", true, false, 0)]
		private Dictionary<InventoryManager.ItemType, string> equipSounds = new Dictionary<InventoryManager.ItemType, string>
		{
			{
				InventoryManager.ItemType.Bead,
				"event:/SFX/UI/EquipBead"
			},
			{
				InventoryManager.ItemType.Prayer,
				"event:/SFX/UI/EquipPrayer"
			},
			{
				InventoryManager.ItemType.Relic,
				"event:/SFX/UI/EquipItem"
			},
			{
				InventoryManager.ItemType.Sword,
				"event:/SFX/UI/EquipItem"
			}
		};

		[SerializeField]
		[BoxGroup("Sounds by type", true, false, 0)]
		private Dictionary<InventoryManager.ItemType, string> unequipSounds = new Dictionary<InventoryManager.ItemType, string>
		{
			{
				InventoryManager.ItemType.Bead,
				"event:/SFX/UI/UnEquipBead"
			},
			{
				InventoryManager.ItemType.Prayer,
				"event:/SFX/UI/UnequipItem"
			},
			{
				InventoryManager.ItemType.Relic,
				"event:/SFX/UI/UnequipItem"
			},
			{
				InventoryManager.ItemType.Sword,
				"event:/SFX/UI/UnequipItem"
			}
		};

		[BoxGroup("Extras", true, false, 0)]
		public GameObject swordHeartText;

		[BoxGroup("Extras", true, false, 0)]
		public GameObject swordHeartTextLocked;

		private List<NewInventory_GridItem> cachedGridElements;

		private List<NewInventory_GridItem> cachedEquipped;

		private bool objectsCached;

		private int currentSelected = -1;

		private int currentSelectedEquiped = -1;

		private int availableEquipables = 9999;

		private InventoryManager.ItemType currentItemType;

		private bool ignoreSelectSound;

		private NewInventory_LayoutGrid.TypeConfiguration currentTypeConfiguration;

		private int currentViewScroll;

		private int currentMaxScrolls;

		private BaseInventoryObject pendingEquipableObject;

		private int pendingEquipableSlot;

		private bool WaitingToCloseNotUnequipable;

		private const int MAX_RESPONSES = 2;

		[BoxGroup("NotUnequipable", true, false, 0)]
		[SerializeField]
		private GameObject NotUnequipableRoot;

		[BoxGroup("NotUnequipable", true, false, 0)]
		[SerializeField]
		private Text[] dialogResponse = new Text[2];

		[BoxGroup("NotUnequipable", true, false, 0)]
		[SerializeField]
		private Color optionNormalColor = new Color(0.972549f, 0.89411765f, 0.78039217f);

		[BoxGroup("NotUnequipable", true, false, 0)]
		[SerializeField]
		private Color optionHighligterColor = new Color(0.80784315f, 0.84705883f, 0.49803922f);

		[BoxGroup("NotUnequipable", true, false, 0)]
		[SerializeField]
		private float NotUnequipableFadeTime = 0.3f;

		[BoxGroup("NotUnequipable", true, false, 0)]
		[SerializeField]
		private float NotUnequipableFadeResponseTime = 0.3f;

		[BoxGroup("NotUnequipable", true, false, 0)]
		[SerializeField]
		private float NotUnequipableEndFadeTime = 0.2f;

		[BoxGroup("NotUnequipable", true, false, 0)]
		[SerializeField]
		private float NotUnequipableWaitTime = 1.5f;

		private GameObject[] dialogResponseSelection = new GameObject[2];

		private bool InConfirmationNotUnequipable;

		private GameObject responsesUI;

		[Serializable]
		private class TypeConfiguration
		{
			public GameObject rootLayout;

			public GameObject equipablesRoot;

			public int slots;
		}
	}
}
