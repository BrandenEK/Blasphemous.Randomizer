using System;
using System.Collections.Generic;
using Framework.Managers;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class PatchNotesWidget : BaseMenuScreen
	{
		public override void Open()
		{
			base.Open();
			base.gameObject.SetActive(true);
			this.gameObjectsToHide.ForEach(delegate(GameObject x)
			{
				x.SetActive(false);
			});
			this.canvasGroup = base.GetComponent<CanvasGroup>();
			this.canvasGroup.alpha = 1f;
			List<string> patchNotesToBeMarkedAsNew = Core.PatchNotesManager.GetPatchNotesToBeMarkedAsNew();
			foreach (PatchNotesElement patchNotesElement in this.patchNotesElements)
			{
				if (patchNotesToBeMarkedAsNew.Contains(patchNotesElement.versionText.text))
				{
					patchNotesElement.DisplayAsNew();
				}
				else
				{
					patchNotesElement.DisplayAsSeen();
				}
			}
			this.rewiredPlayer = ReInput.players.GetPlayer(0);
			this.isOpen = true;
		}

		public override void Close()
		{
			this.canvasGroup.alpha = 0f;
			this.scrollbar.value = 1f;
			this.scrollRect.verticalNormalizedPosition = 1f;
			this.framesSkipped = 0;
			this.isOpen = false;
			this.gameObjectsToHide.ForEach(delegate(GameObject x)
			{
				x.SetActive(true);
			});
			base.gameObject.SetActive(false);
			Core.PatchNotesManager.MarkPatchNotesAsSeen();
			base.Close();
		}

		private void Update()
		{
			if (this.rewiredPlayer == null || !this.isOpen)
			{
				return;
			}
			bool buttonDown = this.rewiredPlayer.GetButtonDown(51);
			if (buttonDown)
			{
				this.Close();
				return;
			}
			float axisRaw = this.rewiredPlayer.GetAxisRaw(49);
			if (Mathf.Abs(axisRaw) > 0.3f)
			{
				this.ProcessScrollInput(axisRaw);
			}
		}

		private void ProcessScrollInput(float scrollAxis)
		{
			float axisRawPrev = this.rewiredPlayer.GetAxisRawPrev(49);
			if (axisRawPrev == 0f)
			{
				this.framesSkipped = 0;
				this.ScrollContent(scrollAxis, 0.004f);
			}
			else
			{
				float axisTimeActive = this.rewiredPlayer.GetAxisTimeActive(49);
				this.framesSkipped++;
				if (this.framesSkipped % 3 == 0)
				{
					this.framesSkipped = 0;
					float scrollingSpeed = (axisTimeActive <= 2f) ? ((axisTimeActive <= 1f) ? 0.004f : 0.01f) : 0.02f;
					this.ScrollContent(scrollAxis, scrollingSpeed);
				}
			}
		}

		private void ScrollContent(float scrollAxis, float scrollingSpeed)
		{
			float num = Mathf.Clamp01(this.scrollbar.value + scrollAxis * scrollingSpeed);
			this.scrollbar.value = num;
			this.scrollRect.verticalNormalizedPosition = num;
		}

		public static Color newPatchNotesColor = Color.white;

		public static Color seenPatchNotesColor = new Color(0.5254902f, 0.4627451f, 0.4f);

		public RectTransform contentTransform;

		public ScrollRect scrollRect;

		public Scrollbar scrollbar;

		public List<GameObject> gameObjectsToHide;

		public List<PatchNotesElement> patchNotesElements;

		public bool isOpen;

		private const int maxNumberOfSkippedFrames = 3;

		private const float axisThreshold = 0.3f;

		private const float delaySecondsForFastScroll = 1f;

		private const float delaySecondsForVeryFastScroll = 2f;

		private const float slowScrollingSpeed = 0.004f;

		private const float fastScrollingSpeed = 0.01f;

		private const float veryFastScrollingSpeed = 0.02f;

		private CanvasGroup canvasGroup;

		private Player rewiredPlayer;

		private int framesSkipped;
	}
}
