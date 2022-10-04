using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.UI.Others.Screen;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.UI.Widgets
{
	public class EndScreenWidget : UIWidget
	{
		private void Awake()
		{
			EndScreenWidget.instance = this;
			this.emailInputField = base.GetComponentInChildren<InputField>(true);
			this.endScreenBody = base.GetComponentInChildren<EndScreenBody>(true);
		}

		public void LoadMainMenu()
		{
			Core.Logic.LoadMenuScene(true);
		}

		private string getInputText()
		{
			string result = string.Empty;
			if (this.emailInputField != null)
			{
				result = this.emailInputField.text;
			}
			return result;
		}

		private void emptyInputText()
		{
			if (this.emailInputField != null && !string.IsNullOrEmpty(this.emailInputField.text))
			{
				this.emailInputField.text = string.Empty;
			}
		}

		public void StoreEmail()
		{
			string inputText = this.getInputText();
			if (!string.IsNullOrEmpty(inputText))
			{
				HandleTextFile.WriteString(inputText);
				this.emptyInputText();
			}
		}

		public void VisitBlasphemousKickstarterPage()
		{
		}

		private void openURL(string url)
		{
			try
			{
				Application.OpenURL(url);
			}
			catch (UnityException ex)
			{
				Debug.LogError(ex.Message);
			}
		}

		public void EnableEndSceneBody()
		{
			if (this.congratsText != null && !this.congratsText.gameObject.activeSelf)
			{
				this.congratsText.gameObject.SetActive(true);
			}
			if (!this.endScreenBody.gameObject.activeSelf)
			{
				this.endScreenBody.gameObject.SetActive(true);
				this.setEndMenuButtonsSelectables();
			}
		}

		private void setEndMenuButtonsSelectables()
		{
			if (this.endMenuFirstSelected != null)
			{
				EventSystem.current.SetSelectedGameObject(this.endMenuFirstSelected);
			}
		}

		public static EndScreenWidget instance;


		private InputField emailInputField;

		private EndScreenBody endScreenBody;

		[SerializeField]
		protected GameObject congratsText;

		[SerializeField]
		protected GameObject endMenuFirstSelected;
	}
}
