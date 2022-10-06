using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI.Others.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class UpgradeFlasksWidget : BaseMenuScreen
	{
		public void Open(float price, Action onUpgradeFlask, Action onContinueWithoutUpgrading)
		{
			this.price = price;
			this.onUpgradeFlask = onUpgradeFlask;
			this.onContinueWithoutUpgrading = onContinueWithoutUpgrading;
			this.Open();
		}

		public override void Open()
		{
			base.Open();
			base.gameObject.SetActive(true);
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			this.UpdateQIText();
			this.UpdateFlaskText();
			this.UpdatePriceText();
			this.UpdateAnswerMessages();
			this.chosenAnswer = UpgradeFlasksWidget.UpgradeFlasksAnswer.None;
			DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 1f, 1f);
		}

		public override void Close()
		{
			TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(DOTween.To(() => this.canvasGroup.alpha, delegate(float x)
			{
				this.canvasGroup.alpha = x;
			}, 0f, 1f), new TweenCallback(this.OnClose));
			base.Close();
		}

		protected override void OnClose()
		{
			if (this.chosenAnswer == UpgradeFlasksWidget.UpgradeFlasksAnswer.Upgrade)
			{
				this.onUpgradeFlask();
			}
			else
			{
				this.onContinueWithoutUpgrading();
			}
			base.gameObject.SetActive(false);
		}

		public void Option_UpgradeFlasks()
		{
			if (this.canvasGroup.alpha < 0.9f || this.chosenAnswer != UpgradeFlasksWidget.UpgradeFlasksAnswer.None)
			{
				return;
			}
			this.chosenAnswer = UpgradeFlasksWidget.UpgradeFlasksAnswer.Upgrade;
			this.Close();
		}

		public void Option_Back()
		{
			if (this.canvasGroup.alpha < 0.9f || this.chosenAnswer != UpgradeFlasksWidget.UpgradeFlasksAnswer.None)
			{
				return;
			}
			this.chosenAnswer = UpgradeFlasksWidget.UpgradeFlasksAnswer.Back;
			this.Close();
		}

		private void UpdateQIText()
		{
			bool flag = this.CanGiveQuestItem();
			this.qiText.color = ((!flag) ? this.errorColor : this.defaultColor);
		}

		private void UpdateFlaskText()
		{
			bool flag = this.CanSacrificeFlask();
			this.flaskText.color = ((!flag) ? this.errorColor : this.defaultColor);
		}

		private void UpdatePriceText()
		{
			bool flag = this.CanAffordUpgrade();
			this.priceText.text = this.price.ToString();
			this.priceText.color = ((!flag) ? this.errorColor : this.defaultColor);
		}

		private void UpdateAnswerMessages()
		{
			bool flag = this.IsFlasksUpgradePossible();
			this.errorMessageGameObject.SetActive(!flag);
			this.upgradeMessageGameObject.SetActive(flag);
			this.upgradeMessageGameObject.GetComponent<MenuButton>().OnDeselect(null);
		}

		private bool CanGiveQuestItem()
		{
			bool result = false;
			ReadOnlyCollection<QuestItem> questItemOwned = Core.InventoryManager.GetQuestItemOwned();
			foreach (string idQuestItem in this.givableQuestItemIds)
			{
				QuestItem questItem = Core.InventoryManager.GetQuestItem(idQuestItem);
				if (questItem != null && questItemOwned.Contains(questItem))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private bool CanSacrificeFlask()
		{
			return Core.Logic.Penitent.Stats.Flask.PermanetBonus > -1f;
		}

		private bool CanAffordUpgrade()
		{
			return this.price <= Core.Logic.Penitent.Stats.Purge.Current;
		}

		private bool IsFlasksUpgradePossible()
		{
			return this.CanAffordUpgrade() && this.CanSacrificeFlask() && this.CanGiveQuestItem();
		}

		public GameObject errorMessageGameObject;

		public GameObject upgradeMessageGameObject;

		public Text qiText;

		public Text flaskText;

		public Text priceText;

		public Color defaultColor;

		public Color errorColor;

		private readonly List<string> givableQuestItemIds = new List<string>(new string[]
		{
			"QI101",
			"QI102",
			"QI103",
			"QI104",
			"QI105"
		});

		private CanvasGroup canvasGroup;

		private Action onUpgradeFlask;

		private Action onContinueWithoutUpgrading;

		private float price;

		private UpgradeFlasksWidget.UpgradeFlasksAnswer chosenAnswer;

		public enum UpgradeFlasksAnswer
		{
			None,
			Upgrade,
			Back
		}
	}
}
