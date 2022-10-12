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
		this.rt.DOKill(false);
		RectTransform rectTransform = this.buttonsAndPositionMarkers[obj];
		base.transform.SetParent(rectTransform.parent, false);
		this.rt.DOAnchorPos(rectTransform.anchoredPosition, this.travelSeconds, false).SetEase(Ease.InOutQuad);
	}

	public float travelSeconds = 0.2f;

	[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.CollapsedFoldout, IsReadOnly = false)]
	public Dictionary<MenuButton, RectTransform> buttonsAndPositionMarkers;

	private RectTransform rt;
}
