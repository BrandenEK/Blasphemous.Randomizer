using System;
using System.Collections.Generic;
using Gameplay.UI.Others.MenuLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class PatchNotesElement : MonoBehaviour
{
	[Button(ButtonSizes.Small)]
	public void GetReferences()
	{
		foreach (Text text in base.transform.GetComponentsInChildren<Text>())
		{
			if (text.gameObject.name.Equals("Version"))
			{
				this.versionText = text;
			}
			else if (text.gameObject.name.Equals("NewIndicator") && !this.newIndicators.Contains(text.gameObject))
			{
				this.newIndicators.Add(text.gameObject);
			}
			else if (text.gameObject.name.Equals("Text") && !this.patchNotesTexts.Contains(text))
			{
				this.patchNotesTexts.Add(text);
			}
		}
	}

	public void DisplayAsSeen()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponentInParent<RectTransform>());
		this.newIndicators.ForEach(delegate(GameObject x)
		{
			x.SetActive(false);
		});
		this.patchNotesTexts.ForEach(delegate(Text x)
		{
			x.CrossFadeColor(PatchNotesWidget.seenPatchNotesColor, 0f, true, false);
		});
	}

	public void DisplayAsNew()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponentInParent<RectTransform>());
		this.newIndicators.ForEach(delegate(GameObject x)
		{
			x.SetActive(true);
		});
		this.patchNotesTexts.ForEach(delegate(Text x)
		{
			x.CrossFadeColor(PatchNotesWidget.newPatchNotesColor, 0f, true, false);
		});
	}

	public Text versionText;

	public List<GameObject> newIndicators;

	public List<Text> patchNotesTexts;
}
