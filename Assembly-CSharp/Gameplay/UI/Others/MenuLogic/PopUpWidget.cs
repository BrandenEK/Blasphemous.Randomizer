using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.MenuLogic
{
	public class PopUpWidget : SerializedMonoBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.SimpleEvent OnDialogClose;

		public bool IsShowing { get; private set; }

		public bool WaitingToShowArea { get; internal set; }

		private void Awake()
		{
			this.IsShowing = false;
			this.waitingEnd = false;
			this.waitingForKey = false;
			this.timeToWait = 0f;
			this.animator = base.GetComponent<Animator>();
			this.messageText = this.messageRoot.GetComponentInChildren<Text>();
			this.areaText = this.areaRoot.GetComponentInChildren<Text>();
			this.messageRoot.SetActive(false);
			this.objectRoot.SetActive(false);
			this.cherubRoot.SetActive(false);
			this.cherubText = this.cherubRoot.GetComponentInChildren<Text>();
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			if (!Core.Events.GetFlag("CHERUB_RESPAWN"))
			{
				Core.Input.SetBlocker("POP_UP", false);
			}
		}

		private void Update()
		{
			if (this.waitingForKey && this.IsShowing && !this.waitingEnd)
			{
				Player player = ReInput.players.GetPlayer(0);
				bool flag = player.GetButtonDown(8) || player.GetButtonDown(5) || player.GetButtonDown(7) || player.GetButtonDown(6);
				if (flag)
				{
					this.waitingEnd = true;
					this.waitingForKey = false;
					base.StartCoroutine(this.SafeEnd());
				}
			}
			if (this.timeToWait > 0f && this.IsShowing && !this.waitingEnd)
			{
				this.timeToWait -= Time.unscaledDeltaTime;
				if (this.timeToWait <= 0f)
				{
					this.waitingEnd = true;
					this.timeToWait = 0f;
					base.StartCoroutine(this.SafeEnd());
				}
			}
		}

		public void ShowPopUp(string message, string eventSound, float timeToWait = 0f, bool blockPlayer = true)
		{
			this.pendingTutorial = string.Empty;
			this.messageRoot.SetActive(true);
			this.objectRoot.SetActive(false);
			this.areaRoot.SetActive(false);
			this.cherubRoot.SetActive(false);
			this.messageText.text = message;
			this.CommonShow(eventSound, timeToWait, blockPlayer);
		}

		public void ShowCherubPopUp(string message, string eventSound, float timeToWait = 0f, bool blockPlayer = true)
		{
			this.pendingTutorial = "13_CHERUBS";
			this.messageRoot.SetActive(false);
			this.objectRoot.SetActive(false);
			this.areaRoot.SetActive(false);
			this.cherubRoot.SetActive(true);
			this.cherubText.text = message;
			this.CommonShow(eventSound, timeToWait, blockPlayer);
		}

		public void ShowAreaPopUp(string area, float timeToWait = 3f, bool blockPlayer = false)
		{
			if (!this.WaitingToShowArea)
			{
				return;
			}
			this.pendingTutorial = string.Empty;
			this.messageRoot.SetActive(false);
			this.objectRoot.SetActive(false);
			this.areaRoot.SetActive(true);
			this.cherubRoot.SetActive(false);
			this.areaText.text = area;
			this.CommonShow(this.areaEventSound, timeToWait, blockPlayer);
		}

		public void HideAreaPopup()
		{
			this.WaitingToShowArea = false;
			CanvasGroup component = base.GetComponent<CanvasGroup>();
			this.areaRoot.SetActive(false);
			this.areaText.text = string.Empty;
			component.alpha = 0f;
			this.IsShowing = false;
			this.animator.SetBool("IsEnabled", false);
		}

		public void ShowItemGet(string message, string itemName, Sprite image, InventoryManager.ItemType objType, float timeToWait = 3f, bool blockPlayer = false)
		{
			switch (objType)
			{
			case InventoryManager.ItemType.Relic:
				this.pendingTutorial = this.TutorialRelic;
				goto IL_67;
			case InventoryManager.ItemType.Bead:
				this.pendingTutorial = this.TutorialBead;
				goto IL_67;
			case InventoryManager.ItemType.Sword:
				this.pendingTutorial = this.TutorialSword;
				goto IL_67;
			}
			this.pendingTutorial = string.Empty;
			IL_67:
			if (this.pendingTutorial != string.Empty && !Core.TutorialManager.IsTutorialUnlocked(this.pendingTutorial))
			{
				blockPlayer = true;
			}
			this.messageRoot.SetActive(false);
			this.objectRoot.SetActive(true);
			this.areaRoot.SetActive(false);
			this.cherubRoot.SetActive(false);
			this.objectText.text = itemName;
			this.objectImage.sprite = image;
			this.objectMessage.text = message;
			this.CommonShow(this.itemAddSounds[objType], timeToWait, blockPlayer);
		}

		private void CommonShow(string eventSound, float timeToWait, bool blockPlayer)
		{
			this.waitingForKey = false;
			if (blockPlayer)
			{
				Core.Input.SetBlocker("POP_UP", true);
			}
			this.timeToWait = timeToWait;
			this.pressKeyText.SetActive(timeToWait == 0f);
			if (timeToWait == 0f)
			{
				base.StartCoroutine(this.SetInputSafe());
			}
			this.IsShowing = true;
			this.waitingEnd = false;
			this.animator.SetBool("IsEnabled", true);
			if (eventSound != string.Empty)
			{
				Core.Audio.PlayOneShot(eventSound, default(Vector3));
			}
		}

		private IEnumerator SafeEnd()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			this.animator.SetBool("IsEnabled", false);
			this.IsShowing = false;
			this.waitingEnd = false;
			this.waitingForKey = false;
			this.timeToWait = 0f;
			yield return new WaitForSecondsRealtime(0.15f);
			Core.Input.SetBlocker("POP_UP", false);
			if (PopUpWidget.OnDialogClose != null)
			{
				PopUpWidget.OnDialogClose();
			}
			if (this.pendingTutorial != string.Empty && !Core.TutorialManager.IsTutorialUnlocked(this.pendingTutorial))
			{
				base.StartCoroutine(Core.TutorialManager.ShowTutorial(this.pendingTutorial, true));
				this.pendingTutorial = string.Empty;
			}
			yield break;
		}

		private IEnumerator SetInputSafe()
		{
			yield return new WaitForSecondsRealtime(0.2f);
			this.waitingForKey = true;
			yield break;
		}

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private GameObject messageRoot;

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private GameObject objectRoot;

		[SerializeField]
		[BoxGroup("Item Get", true, false, 0)]
		private Text objectMessage;

		[SerializeField]
		[BoxGroup("Item Get", true, false, 0)]
		private Text objectText;

		[SerializeField]
		[BoxGroup("Item Get", true, false, 0)]
		private Image objectImage;

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private GameObject pressKeyText;

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private GameObject areaRoot;

		[SerializeField]
		[BoxGroup("Base", true, false, 0)]
		private GameObject cherubRoot;

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private Dictionary<InventoryManager.ItemType, string> itemAddSounds = new Dictionary<InventoryManager.ItemType, string>
		{
			{
				InventoryManager.ItemType.Relic,
				"event:/Key Event/RelicCollected"
			},
			{
				InventoryManager.ItemType.Quest,
				"event:/Key Event/Quest Item"
			},
			{
				InventoryManager.ItemType.Prayer,
				"event:/Key Event/PrayerCollected"
			},
			{
				InventoryManager.ItemType.Bead,
				"event:/Key Event/PrayerCollected"
			},
			{
				InventoryManager.ItemType.Collectible,
				"event:/Key Event/Quest Item"
			},
			{
				InventoryManager.ItemType.Sword,
				"event:/Key Event/PrayerCollected"
			}
		};

		[SerializeField]
		[BoxGroup("Sounds", true, false, 0)]
		private string areaEventSound = "event:/Key Event/ZoneInfo";

		[SerializeField]
		[TutorialId]
		[BoxGroup("Tutorial", true, false, 0)]
		private string TutorialSword;

		[SerializeField]
		[TutorialId]
		[BoxGroup("Tutorial", true, false, 0)]
		private string TutorialBead;

		[SerializeField]
		[TutorialId]
		[BoxGroup("Tutorial", true, false, 0)]
		private string TutorialRelic;

		private const string ANIMATOR_VARIBLE = "IsEnabled";

		private Animator animator;

		private Text messageText;

		private Text areaText;

		private Text cherubText;

		private bool waitingEnd;

		private bool waitingForKey;

		private float timeToWait;

		private string pendingTutorial = string.Empty;
	}
}
