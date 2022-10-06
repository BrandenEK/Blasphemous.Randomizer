using System;
using Framework.FrameworkCore;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageLineSpacing : MonoBehaviour
{
	public void Start()
	{
		this.textComponent = null;
		this.textProComponent = null;
		LocalizationManager.OnLocalizeEvent += new LocalizationManager.OnLocalizeCallback(this.OnLocalize);
		this.textComponent = base.GetComponent<Text>();
		if (this.textComponent != null)
		{
			this.textLineSpacing = this.textComponent.lineSpacing;
		}
		else
		{
			this.textProComponent = base.GetComponent<TextMeshProUGUI>();
			if (this.textProComponent != null)
			{
				this.textMeshProLineSpacing = this.textProComponent.lineSpacing;
			}
		}
		this.OnLocalize();
	}

	private void OnDestroy()
	{
		LocalizationManager.OnLocalizeEvent -= new LocalizationManager.OnLocalizeCallback(this.OnLocalize);
	}

	private void OnLocalize()
	{
		if (this.textComponent != null)
		{
			float num = 1f;
			if (GameConstants.LanguageLineSpacingFactor.ContainsKey(LocalizationManager.CurrentLanguageCode))
			{
				num = GameConstants.LanguageLineSpacingFactor[LocalizationManager.CurrentLanguageCode];
			}
			this.textComponent.lineSpacing = this.textLineSpacing * num;
		}
		if (this.textProComponent != null)
		{
			float num2 = 0f;
			if (GameConstants.LanguageLineSpacingTextPro.ContainsKey(LocalizationManager.CurrentLanguageCode))
			{
				num2 = GameConstants.LanguageLineSpacingTextPro[LocalizationManager.CurrentLanguageCode];
			}
			this.textProComponent.lineSpacing = this.textMeshProLineSpacing + num2;
		}
	}

	private Text textComponent;

	private TextMeshProUGUI textProComponent;

	private float textLineSpacing;

	private float textMeshProLineSpacing;
}
