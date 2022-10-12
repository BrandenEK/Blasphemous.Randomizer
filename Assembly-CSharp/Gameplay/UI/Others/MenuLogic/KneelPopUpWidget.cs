using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Managers;
using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Gameplay.UI.Others.MenuLogic
{
	public class KneelPopUpWidget : SerializedMonoBehaviour
	{
		public bool IsShowing { get; private set; }

		private void Awake()
		{
			this.IsShowing = false;
			this.waitingEnd = false;
			this.animator = base.GetComponent<Animator>();
			this.messageRoot.SetActive(false);
		}

		private void OnEnable()
		{
			I2.Loc.LocalizationManager.OnLocalizeEvent += this.OnLanguageChanged;
		}

		private void OnDisable()
		{
			I2.Loc.LocalizationManager.OnLocalizeEvent -= this.OnLanguageChanged;
		}

		private void OnLanguageChanged()
		{
			this.UpdateText();
		}

		private void UpdateText()
		{
			if (!this.IsShowing)
			{
				return;
			}
			string localizedText = this.Config[this.CurrentMode];
			this.text.text = Framework.Managers.LocalizationManager.ParseMeshPro(localizedText, this.Config[this.CurrentMode].mTerm, null);
		}

		public void ShowPopUp(KneelPopUpWidget.Modes mode)
		{
			this.CurrentMode = mode;
			this.messageRoot.SetActive(true);
			this.IsShowing = true;
			this.waitingEnd = false;
			this.animator.SetBool("IsEnabled", true);
			this.UpdateText();
		}

		public void HidePopUp()
		{
			base.StartCoroutine(this.SafeEnd());
		}

		private IEnumerator SafeEnd()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			this.animator.SetBool("IsEnabled", false);
			this.IsShowing = false;
			this.waitingEnd = false;
			yield return new WaitForSecondsRealtime(0.15f);
			this.messageRoot.SetActive(false);
			yield break;
		}

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private GameObject messageRoot;

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private TextMeshProUGUI text;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private Dictionary<KneelPopUpWidget.Modes, LocalizedString> Config = new Dictionary<KneelPopUpWidget.Modes, LocalizedString>();

		private const string ANIMATOR_VARIABLE = "IsEnabled";

		private Animator animator;

		private bool waitingEnd;

		private KneelPopUpWidget.Modes CurrentMode;

		public enum Modes
		{
			PrieDieu_sword,
			PrieDieu_teleport,
			PrieDieu_all,
			Altar
		}
	}
}
