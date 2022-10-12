using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI.Others.Buttons;
using Gameplay.UI.Others.MenuLogic;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class TeleportWidget : BasicUIBlockingWidget
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event TeleportWidget.TeleportCancelDelegate OnTeleportCancelled;

	protected override void OnWidgetShow()
	{
		base.OnWidgetShow();
		this.isCanceled = false;
		this.firstSelectionAfterShow = true;
		for (int i = 0; i < this.teleportRoot.childCount; i++)
		{
			this.teleportRoot.GetChild(i).gameObject.SetActive(false);
		}
		this.teleports.Clear();
		int num = 0;
		int slot = 0;
		foreach (TeleportDestination teleportDestination in Core.SpawnManager.GetAllUIActiveTeleports())
		{
			GameObject gameObject = this.teleportRoot.GetChild(teleportDestination.selectedSlot).gameObject;
			gameObject.name = string.Format("Teleport_{0}: {1}", num, teleportDestination.teleportName);
			gameObject.SetActive(true);
			Selectable component = gameObject.GetComponent<Selectable>();
			TeleportWidget.TeleportObject teleportObject = new TeleportWidget.TeleportObject
			{
				image = gameObject.GetComponent<Image>(),
				destination = teleportDestination,
				button = component.GetComponent<EventsButton>()
			};
			int elementNumber = num;
			teleportObject.button.onSelected = new EventsButton.ButtonSelectedEvent();
			teleportObject.button.onSelected.AddListener(delegate()
			{
				this.SelectElement(elementNumber);
			});
			teleportObject.button.onClick = new EventsButton.ButtonClickedEvent();
			teleportObject.button.onClick.AddListener(delegate()
			{
				this.ActivateElement(elementNumber);
			});
			if (teleportObject.destination.sceneName == Core.NewMapManager.CurrentScene.GetLevelName())
			{
				slot = num;
			}
			this.teleports.Add(teleportObject);
			num++;
		}
		this.UpdateNavigation();
		base.StartCoroutine(this.FocusSlotSecure(slot, true));
	}

	private void UpdateNavigation()
	{
		List<Selectable> list = new List<Selectable>();
		foreach (TeleportWidget.TeleportObject teleportObject in this.teleports)
		{
			list.Add(teleportObject.button);
		}
		foreach (TeleportWidget.TeleportObject teleportObject2 in this.teleports)
		{
			this.UpdateNavigationFor(teleportObject2.button, list);
		}
	}

	private void UpdateNavigationFor(Selectable s, List<Selectable> selectables)
	{
		Navigation navigation = s.navigation;
		navigation.mode = Navigation.Mode.Explicit;
		navigation.selectOnUp = s.FindSelectableFromList(Vector2.up, selectables);
		navigation.selectOnDown = s.FindSelectableFromList(Vector2.down, selectables);
		navigation.selectOnLeft = s.FindSelectableFromList(Vector2.left, selectables);
		navigation.selectOnRight = s.FindSelectableFromList(Vector2.right, selectables);
		s.navigation = navigation;
	}

	private void ActivateElement(int slot)
	{
		if (this.currentSelected < 0 || this.currentSelected >= this.teleports.Count)
		{
			return;
		}
		TeleportWidget.TeleportObject teleportObject = this.teleports[this.currentSelected];
		if (teleportObject.destination.sceneName.Equals(Core.LevelManager.currentLevel.LevelName))
		{
			this.isCanceled = true;
			base.FadeHide();
		}
		else
		{
			Core.SpawnManager.Teleport(teleportObject.destination);
			base.FadeHide();
		}
	}

	private void SelectElement(int elementNumber)
	{
		foreach (TeleportWidget.TeleportObject teleportObject in this.teleports)
		{
			teleportObject.image.overrideSprite = this.teleportSprite;
			teleportObject.image.SetNativeSize();
		}
		int targetIdx = elementNumber;
		Transform transform = this.teleports[targetIdx].image.transform;
		Vector2 sizeDelta = this.travelButton.sizeDelta;
		Vector3 b = (!this.teleports[targetIdx].destination.labelUnderIcon) ? this.travelButtonOffset : new Vector3(-sizeDelta.x * 0.5f, -sizeDelta.y - 8f);
		Vector3 vector = transform.position + b;
		if (this.displaceTravelButon && !this.firstSelectionAfterShow)
		{
			this.travelButton.DOKill(false);
			this.travelButton.DOMove(vector, this.movementTime, false).SetEase(this.movementEaseType).OnComplete(delegate
			{
				foreach (TeleportWidget.TeleportObject teleportObject2 in this.teleports)
				{
					teleportObject2.image.overrideSprite = this.teleportSprite;
					teleportObject2.image.SetNativeSize();
				}
				this.UpdateOnDestination(targetIdx);
			});
		}
		else
		{
			this.travelButton.position = vector;
			this.UpdateOnDestination(targetIdx);
		}
		this.firstSelectionAfterShow = false;
	}

	private void UpdateOnDestination(int targetIdx)
	{
		this.teleports[targetIdx].image.overrideSprite = this.selectedTeleportSprite;
		this.teleports[targetIdx].image.SetNativeSize();
		this.teleportDestinationText.text = Core.NewMapManager.GetZoneNameFromBundle(this.teleports[targetIdx].destination.sceneName);
		this.currentSelected = targetIdx;
	}

	private IEnumerator FocusSlotSecure(int slot, bool ignoreSound = true)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		if (this.teleports.Count > slot)
		{
			EventsButton button = this.teleports[slot].button;
			button.Select();
		}
		yield break;
	}

	private void Update()
	{
		if (ReInput.players.GetPlayer(0).GetButtonDown(51))
		{
			this.isCanceled = true;
		}
	}

	protected override void OnWidgetHide()
	{
		base.OnWidgetHide();
		if (this.isCanceled && TeleportWidget.OnTeleportCancelled != null)
		{
			TeleportWidget.OnTeleportCancelled();
		}
	}

	[BoxGroup("Graphics", true, false, 0)]
	public Sprite teleportSprite;

	[BoxGroup("Graphics", true, false, 0)]
	public Sprite selectedTeleportSprite;

	[BoxGroup("UI", true, false, 0)]
	public Transform teleportRoot;

	[BoxGroup("UI", true, false, 0)]
	public Text teleportDestinationText;

	[BoxGroup("UI", true, false, 0)]
	public bool displaceTravelButon = true;

	[BoxGroup("UI", true, false, 0)]
	public RectTransform travelButton;

	[BoxGroup("UI", true, false, 0)]
	public Vector2 travelButtonOffset = new Vector2(4f, 0f);

	[BoxGroup("UI Effects", true, false, 0)]
	public Ease movementEaseType = Ease.Linear;

	[BoxGroup("UI Effects", true, false, 0)]
	public float movementTime = 0.2f;

	private bool isCanceled;

	private readonly List<TeleportWidget.TeleportObject> teleports = new List<TeleportWidget.TeleportObject>();

	private int currentSelected = -1;

	private bool firstSelectionAfterShow = true;

	public delegate void TeleportCancelDelegate();

	private class TeleportObject
	{
		public Image image;

		public TeleportDestination destination;

		public EventsButton button;
	}
}
