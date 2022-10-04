using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI;
using Gameplay.UI.Others.MenuLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Interactables
{
	[SelectionBase]
	public class PrieDieu : Interactable
	{
		private void OnAnimationEvent(string id)
		{
			if (id == "KNEE_END")
			{
				this.penitentKneeing = false;
			}
		}

		protected override void OnPlayerReady()
		{
			GameObject gameObject = null;
			this.interactableAnimatorLevel1.SetActive(false);
			this.interactableAnimatorLevel2.SetActive(false);
			this.interactableAnimatorLevel3.SetActive(false);
			int prieDieuLevel = Core.Alms.GetPrieDieuLevel();
			if (prieDieuLevel != 1)
			{
				if (prieDieuLevel != 2)
				{
					if (prieDieuLevel == 3)
					{
						gameObject = this.interactableAnimatorLevel3;
					}
				}
				else
				{
					gameObject = this.interactableAnimatorLevel2;
				}
			}
			else
			{
				gameObject = this.interactableAnimatorLevel1;
			}
			gameObject.SetActive(true);
			this.interactableAnimator = gameObject.GetComponent<Animator>();
			base.CheckAnimationEvents();
		}

		protected override IEnumerator OnUse()
		{
			if (this.Ligthed)
			{
				yield return this.ReActivationLogic();
			}
			else
			{
				yield return this.ActivationLogic();
			}
			Core.Logic.UsePrieDieu();
			Core.Logic.BreakableManager.Reset();
			yield break;
		}

		protected override void InteractionEnd()
		{
			Core.Logic.Penitent.SpriteRenderer.enabled = true;
			Core.Logic.Penitent.DamageArea.enabled = true;
		}

		protected override void ObjectEnable()
		{
			if (base.AnimatorEvent == null)
			{
				return;
			}
			base.AnimatorEvent.OnEventLaunched += this.OnAnimationEvent;
		}

		protected override void ObjectDisable()
		{
			if (base.AnimatorEvent == null)
			{
				return;
			}
			base.AnimatorEvent.OnEventLaunched -= this.OnAnimationEvent;
		}

		protected override void OnUpdate()
		{
			if (!base.BeingUsed && base.PlayerInRange && base.InteractionTriggered)
			{
				base.Use();
			}
			else if (!base.BeingUsed && base.PlayerInRange && !this.Ligthed && base.InteractionTriggered)
			{
				base.Use();
			}
			if (base.BeingUsed)
			{
				this.PlayerReposition();
			}
		}

		private IEnumerator ActivationLogic()
		{
			Core.Metrics.CustomEvent("PRIEDIEU_ACTIVATED", base.name, -1f);
			Core.Audio.PlaySfxOnCatalog(this.activationId, this.activationDelay);
			LogicManager coreLogic = Core.Logic;
			coreLogic.Penitent.SpriteRenderer.enabled = false;
			coreLogic.Penitent.DamageArea.enabled = false;
			if (this.entityRenderer != null)
			{
				this.entityRenderer.flipX = (Core.Logic.Penitent.Status.Orientation == EntityOrientation.Left);
			}
			this.interactorAnimator.SetTrigger("ACTIVATION");
			yield return new WaitForSeconds(this.initialDelay);
			this.ligthed = true;
			Core.Events.LaunchEvent(this.GenericFirstUse, base.name);
			Core.Events.LaunchEvent(this.OnFirstUse, string.Empty);
			this.ShallowActivationLogic();
			if (this.gotoAttrackWhenActivate)
			{
				Core.Logic.LoadAttrackScene();
			}
			Core.Logic.EnemySpawner.RespawnDeadEnemies();
			yield break;
		}

		private void ShallowActivationLogic()
		{
			Core.SpawnManager.ActivePrieDieu = this;
			Core.Logic.Penitent.Stats.Life.SetToCurrentMax();
			Core.Logic.Penitent.Stats.Flask.SetToCurrentMax();
			if (Core.Alms.GetPrieDieuLevel() > 1)
			{
				Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax();
			}
			Core.Persistence.SaveGame(true);
		}

		private IEnumerator ReActivationLogic()
		{
			this.ShallowActivationLogic();
			LogicManager logic = Core.Logic;
			Core.Audio.PlaySfxOnCatalog(this.kneeStartId, this.kneeStartDelay);
			logic.Penitent.SpriteRenderer.enabled = false;
			logic.Penitent.DamageArea.enabled = false;
			if (this.entityRenderer != null)
			{
				this.entityRenderer.flipX = (Core.Logic.Penitent.Status.Orientation == EntityOrientation.Left);
			}
			this.interactorAnimator.SetTrigger("KNEE_START");
			this.penitentKneeing = true;
			Core.Events.LaunchEvent(this.OnReuse, string.Empty);
			while (this.penitentKneeing)
			{
				yield return 0;
			}
			Core.Logic.EnemySpawner.RespawnDeadEnemies();
			bool flag = this.HaveAnySwordHearts();
			bool flag2 = Core.Alms.GetPrieDieuLevel() >= 3 || Core.Randomizer.gameConfig.unlockTeleportation;
			if (flag || flag2)
			{
				yield return base.StartCoroutine(this.KneeledMenuCoroutine(flag, flag2));
			}
			this.interactorAnimator.SetTrigger("KNEE_END");
			Core.Audio.PlaySfxOnCatalog(this.kneeEndId, this.kneeEndDelay);
			yield break;
		}

		private bool HaveAnySwordHearts()
		{
			return Core.InventoryManager.GetSwordsOwned().Count > 0;
		}

		private IEnumerator KneeledMenuCoroutine(bool canUseInventory, bool canUseTeleport)
		{
			bool active = true;
			bool shownInventory = false;
			KneelPopUpWidget.Modes mode = KneelPopUpWidget.Modes.PrieDieu_all;
			if (!canUseInventory)
			{
				mode = KneelPopUpWidget.Modes.PrieDieu_teleport;
			}
			else if (!canUseTeleport)
			{
				mode = KneelPopUpWidget.Modes.PrieDieu_sword;
			}
			UIController.instance.ShowKneelMenu(mode);
			Debug.Log("<color=magenta>KNEEL MENU</color>");
			while (active)
			{
				if (canUseInventory && UIController.instance.IsInventoryMenuPressed())
				{
					if (!shownInventory)
					{
						Debug.Log("<color=magenta>OPEN INVENTORY</color>");
						UIController.instance.SelectTab(NewInventoryWidget.TabType.Sword);
						UIController.instance.ToggleInventoryMenu();
						UIController.instance.MakeKneelMenuInvisible();
						shownInventory = true;
						yield return new WaitForSeconds(0.5f);
					}
					else
					{
						Debug.Log("<color=magenta>CLOSING INVENTORY</color>");
						UIController.instance.ToggleInventoryMenu();
					}
				}
				if (shownInventory && UIController.instance.IsInventoryClosed())
				{
					Debug.Log("<color=magenta>INVENTORY IS CLOSED, CLOSING KNEEL MENU</color>");
					active = false;
				}
				else if (!shownInventory && UIController.instance.IsStopKneelPressed())
				{
					Debug.Log("<color=magenta>CLOSING KNEEL MENU DIRECTLY</color>");
					active = false;
				}
				else if (canUseTeleport && !shownInventory && UIController.instance.IsTeleportMenuPressed())
				{
					UIController.instance.HideKneelMenu();
					active = false;
					yield return base.StartCoroutine(UIController.instance.ShowMapTeleport());
				}
				yield return null;
			}
			UIController.instance.HideKneelMenu();
			yield break;
		}

		protected override void PlayerReposition()
		{
			Core.Logic.Penitent.transform.position = this.interactorAnimator.transform.position;
		}

		public bool Ligthed
		{
			get
			{
				return this.ligthed;
			}
			set
			{
				this.interactableAnimator.SetBool("ACTIVE", value);
				this.ligthed = value;
			}
		}

		public void ShallowUse()
		{
			base.gameObject.SendMessage("OnUsePre", SendMessageOptions.DontRequireReceiver);
			this.ShallowActivationLogic();
			base.gameObject.SendMessage("OnUsePost", SendMessageOptions.DontRequireReceiver);
		}

		public override bool IsOpenOrActivated()
		{
			return this.Ligthed;
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			PrieDieu.PrieDieuPersistenceData prieDieuPersistenceData = base.CreatePersistentData<PrieDieu.PrieDieuPersistenceData>();
			prieDieuPersistenceData.lighted = this.Ligthed;
			return prieDieuPersistenceData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			PrieDieu.PrieDieuPersistenceData prieDieuPersistenceData = (PrieDieu.PrieDieuPersistenceData)data;
			this.Ligthed = prieDieuPersistenceData.lighted;
		}

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		[Required]
		private SpriteRenderer entityRenderer;

		[SerializeField]
		[BoxGroup("Advanced Settings", true, false, 0)]
		private float initialDeadLapse;

		[SerializeField]
		[BoxGroup("Advanced Settings", true, false, 0)]
		private float initialDelay;

		[SerializeField]
		[BoxGroup("Advanced Settings", true, false, 0)]
		private float timeToPrayerMenu;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public EntityOrientation spawnOrientation;

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		[ReadOnly]
		private string GenericFirstUse = "PRIEDIEU_ACTIVATED";

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		private string OnFirstUse;

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		private string OnReuse;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private string activationId;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private string kneeStartId;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private string kneeEndId;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float activationDelay;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float kneeStartDelay;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float kneeEndDelay;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		protected GameObject interactableAnimatorLevel1;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		protected GameObject interactableAnimatorLevel2;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		protected GameObject interactableAnimatorLevel3;

		[SerializeField]
		[BoxGroup("Attrack Mode", true, false, 0)]
		private bool gotoAttrackWhenActivate;

		private bool penitentKneeing;

		private bool ligthed;

		private class PrieDieuPersistenceData : PersistentManager.PersistentData
		{
			public PrieDieuPersistenceData(string id) : base(id)
			{
			}

			public bool lighted;
		}
	}
}
