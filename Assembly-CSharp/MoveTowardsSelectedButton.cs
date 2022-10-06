using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.UI.Others.Buttons;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoveTowardsSelectedButton : SerializedMonoBehaviour
{
	private void Start()
	{
		this.rt = base.GetComponent<RectTransform>();
		foreach (MenuButton menuButton in this.buttonsAndPositionMarkers.Keys)
		{
			menuButton.OnMenuButtonSelected += this.Item_OnMenuButtonSelected;
		}
	}

	private void Item_OnMenuButtonSelected(MenuButton obj)
	{
		ShortcutExtensions.DOKill(this.rt, false);
		RectTransform rectTransform = this.buttonsAndPositionMarkers[obj];
		base.transform.SetParent(rectTransform.parent, false);
		TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions46.DOAnchorPos(this.rt, rectTransform.anchoredPosition, this.travelSeconds, false), 7);
	}

	public float travelSeconds = 0.2f;

	[DictionaryDrawerSettings(DisplayMode = 2, IsReadOnly = false)]
	public Dictionary<MenuButton, RectTransform> buttonsAndPositionMarkers;

	private RectTransform rt;
}
