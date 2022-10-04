using System;
using System.Collections;
using System.Diagnostics;
using Com.LuisPedroFonseca.ProCamera2D;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using Gameplay.UI.Widgets;
using I2.Loc;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Tools.Level.Interactables
{
	public class Door : Interactable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.SimpleEvent OnDoorEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.SimpleEvent OnDoorExit;

		public void ExitFromThisDoor()
		{
			this.objectUsed = true;
			this.delayedOpen();
		}

		private IEnumerator ExitDoorSafe()
		{
			this.ShowPlayer(true);
			Core.Logic.Penitent.Physics.EnableColliders(false);
			Core.Logic.Penitent.Physics.EnablePhysics(false);
			yield return new WaitForEndOfFrame();
			Penitent currentPenitent = Core.Logic.Penitent;
			Vector3 spawnPointPos = this.spawnPoint.transform.position;
			currentPenitent.transform.position = spawnPointPos;
			currentPenitent.SetOrientation(this.exitOrientation, true, false);
			yield return new WaitForEndOfFrame();
			Core.Logic.Penitent.Physics.EnableColliders(true);
			yield return new WaitForEndOfFrame();
			Core.Logic.Penitent.Physics.EnablePhysics(true);
			Core.Input.SetBlocker("DOOR", false);
			Core.Logic.Penitent.Status.CastShadow = true;
			Core.Logic.Penitent.DamageArea.DamageAreaCollider.enabled = true;
			if (Door.OnDoorExit != null)
			{
				Door.OnDoorExit();
			}
			yield break;
		}

		private bool CheckRequirements()
		{
			bool flag = true;
			if (this.objectNeeded && !this.objectUsed)
			{
				QuestItem questItem = Core.InventoryManager.GetQuestItem(this.objectId);
				flag = (questItem && Core.InventoryManager.IsQuestItemOwned(questItem));
				if (!flag && this.showMessageIfNot)
				{
					string valueWithParam = Core.Localization.GetValueWithParam(ScriptLocalization.UI_Inventory.TEXT_DOOR_NEED_OBJECT, "object_caption", questItem.caption);
					UIController.instance.ShowPopUp(valueWithParam, this.soundPopupIfNot, 0f, true);
				}
				if (flag)
				{
					if (this.showMessageOnOpen)
					{
						UIController.instance.ShowPopUpObjectUse(questItem.caption, this.soundPopupOpen);
						flag = false;
						base.StartCoroutine(this.WaitForPopupToEnter());
					}
					if (this.removeObject)
					{
						Core.InventoryManager.RemoveQuestItem(questItem);
					}
				}
			}
			return flag;
		}

		private void EnterDoor()
		{
			if (this.ladderRequired && !Core.Logic.Penitent.IsClimbingLadder)
			{
				return;
			}
			if (base.PlayerInRange && !this.closed)
			{
				this.interactorAnimator.SetTrigger("OPEN_ENTER");
			}
			else if (base.PlayerInRange && this.closed)
			{
				this.interactorAnimator.SetTrigger("CLOSED_ENTER");
			}
			this.objectUsed = true;
			Core.Input.SetBlocker("DOOR", true);
			FadeWidget.OnFadeShowEnd += this.OnFadeShowEnd;
			FadeWidget.instance.Fade(true, 0.2f, (!this.autoEnter) ? this.enterDelay : 0f, null);
			this.passingThrougDoor = true;
			if (this.autoEnter)
			{
				Core.Logic.Penitent.Physics.EnablePhysics(false);
			}
			Core.Logic.Penitent.Status.CastShadow = false;
			Core.Logic.Penitent.DamageArea.DamageAreaCollider.enabled = false;
			if (Door.OnDoorEnter != null)
			{
				Door.OnDoorEnter();
			}
		}

		protected override void InteractionStart()
		{
			this.ShowPlayer(false);
		}

		protected override void InteractionEnd()
		{
		}

		private IEnumerator WaitForPopupToEnter()
		{
			while (UIController.instance.IsShowingPopUp())
			{
				yield return new WaitForEndOfFrame();
			}
			this.EnterDoor();
			yield break;
		}

		protected override IEnumerator OnUse()
		{
			yield return 0;
			yield break;
		}

		protected override void OnAwake()
		{
			this.Closed = this.startClosed;
		}

		protected override void OnStart()
		{
			base.AnimatorEvent.OnEventLaunched += this.OnEventLaunched;
			base.Locked = false;
			this.interactableAnimator.SetBool("CLOSED", this.closed);
			if (this.autoEnter)
			{
				base.EnableInputIcon(false);
			}
		}

		private void OnEventLaunched(string id)
		{
			if (id == "OPEN_DOOR")
			{
				Core.Audio.PlaySfx(this.openSound, 0f);
				this.Closed = false;
			}
		}

		protected override void OnUpdate()
		{
			if (Core.Logic != null && Core.Logic.Penitent == null)
			{
				return;
			}
			if (base.PlayerInRange && base.InteractionTriggered && !base.Locked && this.CheckRequirements())
			{
				this.EnterDoor();
			}
			if (base.PlayerInRange && this.autoEnter && !base.Locked && !this.passingThrougDoor && this.CheckRequirements())
			{
				this.EnterDoor();
			}
			if (base.BeingUsed)
			{
				this.PlayerReposition();
			}
		}

		private void OnFadeShowEnd()
		{
			ProCamera2D.Instance.HorizontalFollowSmoothness = 0f;
			ProCamera2D.Instance.VerticalFollowSmoothness = 0f;
			FadeWidget.OnFadeShowEnd -= this.OnFadeShowEnd;
			this.OnDoorActivated();
		}

		private void OnDoorActivated()
		{
			Log.Trace("Door", "Door has been activated.", null);
			Core.SpawnManager.SpawnFromDoor(this.targetScene, this.targetDoor, this.useFade);
		}

		[UsedImplicitly]
		public bool SceneNotInBuildIndex()
		{
			return SceneManager.GetActiveScene().buildIndex == -1;
		}

		public void InstaOpen()
		{
			base.Invoke("delayedOpen", 0.1f);
		}

		private void delayedOpen()
		{
			this.closed = false;
			this.interactableAnimator.SetTrigger("INSTA_OPEN");
			this.interactableAnimator.SetBool("CLOSED", false);
		}

		public bool Closed
		{
			get
			{
				return this.closed;
			}
			set
			{
				this.closed = value;
				if (this.interactableAnimator != null)
				{
					this.interactableAnimator.SetBool("CLOSED", value);
				}
			}
		}

		public override bool IsOpenOrActivated()
		{
			return !this.Closed;
		}

		public bool CanBeUsed()
		{
			return this.targetDoor != null && this.targetScene != null;
		}

		public bool EnterTriggered
		{
			get
			{
				return Core.Logic.Penitent.PlatformCharacterInput.ClampledVerticalAxis > 0f;
			}
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			Door.DoorPersistenceData doorPersistenceData = base.CreatePersistentData<Door.DoorPersistenceData>();
			doorPersistenceData.objectUsed = this.objectUsed;
			doorPersistenceData.closed = this.Closed;
			return doorPersistenceData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			Door.DoorPersistenceData doorPersistenceData = (Door.DoorPersistenceData)data;
			this.objectUsed = doorPersistenceData.objectUsed;
			this.Closed = doorPersistenceData.closed;
			if (!this.Closed)
			{
				this.interactableAnimator.SetTrigger("INSTA_OPEN");
			}
			this.OnBlocked(base.Locked);
		}

		[InfoBox("This level is not in the build index.This door may not work as expected.", InfoMessageType.Error, "SceneNotInBuildIndex")]
		[BoxGroup("Design Settings", true, false, 0)]
		[FormerlySerializedAs("doorId")]
		public string identificativeName;

		[BoxGroup("Design Settings", true, false, 0)]
		public bool autoEnter;

		[BoxGroup("Design Settings", true, false, 0)]
		public bool ladderRequired;

		[BoxGroup("Design Settings", true, false, 0)]
		public EntityOrientation exitOrientation;

		[BoxGroup("Design Settings", true, false, 0)]
		public bool startClosed;

		[BoxGroup("Design Settings", true, false, 0)]
		public float enterDelay = 2f;

		[BoxGroup("Design Settings", true, false, 0)]
		public bool showTextLockDoor = true;

		[BoxGroup("Design Settings", true, false, 0)]
		public string targetScene;

		[BoxGroup("Design Settings", true, false, 0)]
		public string targetDoor;

		[BoxGroup("Design Settings", true, false, 0)]
		public bool streamingLevel = true;

		[BoxGroup("Design Settings", true, false, 0)]
		[EventRef]
		public string openSound;

		[BoxGroup("Design Settings", true, false, 0)]
		public bool useFade = true;

		[BoxGroup("Design Settings", true, false, 0)]
		public bool objectNeeded;

		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("objectNeeded", true)]
		[InventoryId(InventoryManager.ItemType.Quest)]
		public string objectId;

		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("objectNeeded", true)]
		public bool showMessageIfNot = true;

		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("objectNeeded", true)]
		public string soundPopupIfNot = string.Empty;

		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("objectNeeded", true)]
		public bool showMessageOnOpen = true;

		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("objectNeeded", true)]
		public string soundPopupOpen = string.Empty;

		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("objectNeeded", true)]
		public bool removeObject = true;

		[BoxGroup("Attached References", true, false, 0)]
		public Transform spawnPoint;

		private bool objectUsed;

		private bool passingThrougDoor;

		private bool closed;

		private class DoorPersistenceData : PersistentManager.PersistentData
		{
			public DoorPersistenceData(string id) : base(id)
			{
			}

			public bool closed;

			public bool objectUsed;
		}
	}
}
