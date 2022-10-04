using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.Disclaimer
{
	public class DisclaimerText : MonoBehaviour
	{
		private void Awake()
		{
			this.disclaimerTextComponent = base.GetComponent<Text>();
			if (this.disclaimerTextComponent && !Debug.isDebugBuild)
			{
				this.disclaimerTextComponent.enabled = false;
			}
		}

		private void Start()
		{
			if (this.disclaimerTextComponent == null)
			{
				return;
			}
			this.disclaimerText = this.disclaimerTextComponent.text;
			if (!string.IsNullOrEmpty(this.disclaimerText))
			{
				try
				{
					string newValue = string.Format("{0} - {1}", "Steam", Application.platform);
					this.disclaimerText = this.disclaimerText.Replace("%1", "v." + Application.version);
					this.disclaimerText = this.disclaimerText.Replace("%2", newValue);
					this.disclaimerText = this.disclaimerText.Replace("%3", "October 2021");
					this.disclaimerTextComponent.text = this.disclaimerText;
				}
				catch (Exception ex)
				{
					Debug.Log(ex.Message);
				}
			}
		}

		private Text disclaimerTextComponent;

		private string disclaimerText;
	}
}
