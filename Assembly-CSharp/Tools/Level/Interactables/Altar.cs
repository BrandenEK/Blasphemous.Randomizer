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
	public class Altar : Interactable
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
			this.interactableAnimatorLevel1.SetActive(false);
			this.interactableAnimatorLevel2.SetActive(false);
			this.interactableAnimatorLevel3.SetActive(false);
			this.interactableAnimatorLevel4.SetActive(false);
			this.interactableAnimatorLevel5.SetActive(false);
			this.interactableAnimatorLevel6.SetActive(false);
			this.interactableAnimatorLevel7.SetActive(false);
			GameObject gameObject;
			switch (Core.Alms.GetAltarLevel())
			{
			case 1:
				gameObject = this.interactableAnimatorLevel1;
				break;
			case 2:
				gameObject = this.interactableAnimatorLevel2;
				break;
			case 3:
				gameObject = this.interactableAnimatorLevel3;
				break;
			case 4:
				gameObject = this.interactableAnimatorLevel4;
				break;
			case 5:
				gameObject = this.interactableAnimatorLevel5;
				break;
			case 6:
				gameObject = this.interactableAnimatorLevel6;
				break;
			case 7:
				gameObject = this.interactableAnimatorLevel7;
				break;
			default:
				gameObject = this.interactableAnimatorLevel7;
				break;
			}
			gameObject.SetActive(true);
			this.interactableAnimator = gameObject.GetComponent<Animator>();
			base.CheckAnimationEvents();
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

		protected override IEnumerator OnUse()
		{
			yield return this.ActivationLogic();
			yield break;
		}

		protected override void InteractionEnd()
		{
			Core.Logic.Penitent.SpriteRenderer.enabled = true;
			Core.Logic.Penitent.DamageArea.enabled = true;
		}

		protected override void OnUpdate()
		{
			if (!base.BeingUsed && base.PlayerInRange && base.InteractionTriggered)
			{
				base.Use();
			}
			if (base.BeingUsed)
			{
				this.PlayerReposition();
			}
		}

		protected override void PlayerReposition()
		{
			Core.Logic.Penitent.transform.position = this.interactorAnimator.transform.position;
		}

		private IEnumerator ActivationLogic()
		{
			if (this.RepositionBeforeInteract)
			{
				Core.Logic.Penitent.DrivePlayer.MoveToPosition(this.Waypoint.position, this.orientation);
				do
				{
					yield return null;
				}
				while (Core.Logic.Penitent.DrivePlayer.Casting);
			}
			Core.Logic.Penitent.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.DamageArea.enabled = false;
			Core.Audio.PlaySfxOnCatalog(this.kneeStartId, this.kneeStartDelay);
			if (this.entityRenderer != null)
			{
				this.entityRenderer.flipX = (Core.Logic.Penitent.Status.Orientation == EntityOrientation.Left);
			}
			this.interactorAnimator.SetTrigger("KNEE_START");
			this.penitentKneeing = true;
			Core.Events.LaunchEvent(this.OnAltarUse, base.name);
			while (this.penitentKneeing)
			{
				yield return null;
			}
			yield return new WaitForSecondsRealtime(Core.Alms.Config.InitialDelay);
			if (Core.Alms.IsMaxTier())
			{
				yield return this.ConfessorCourrutine();
			}
			else if (Core.Alms.GetAltarLevel() > 1)
			{
				yield return base.StartCoroutine(this.KneeledMenuCoroutine());
			}
			else
			{
				yield return base.StartCoroutine(UIController.instance.ShowAlmsWidgetCourrutine());
			}
			this.interactorAnimator.SetTrigger("KNEE_END");
			Core.Audio.PlaySfxOnCatalog(this.kneeEndId, this.kneeEndDelay);
			yield break;
		}

		private IEnumerator KneeledMenuCoroutine()
		{
			bool active = true;
			UIController.instance.ShowKneelMenu(KneelPopUpWidget.Modes.Altar);
			while (active)
			{
				if (UIController.instance.IsInventoryMenuPressed())
				{
					UIController.instance.MakeKneelMenuInvisible();
					yield return this.ConfessorCourrutine();
					active = false;
				}
				if (UIController.instance.IsTeleportMenuPressed())
				{
					UIController.instance.HideKneelMenu();
					yield return base.StartCoroutine(UIController.instance.ShowAlmsWidgetCourrutine());
					active = false;
				}
				if (UIController.instance.IsStopKneelPressed())
				{
					active = false;
				}
				yield return null;
			}
			UIController.instance.HideKneelMenu();
			yield break;
		}

		private IEnumerator ConfessorCourrutine()
		{
			int guilt = Core.GuiltManager.GetDropsCount();
			if (guilt == 0)
			{
				yield return this.ShowMessage("MSG_CONFESSOR_NOGUILT", 2.5f);
			}
			else
			{
				float meaCulpa = Core.Logic.Penitent.Stats.MeaCulpa.Final + 1f;
				float purgePrice = meaCulpa * this.PurgeBasePrice * (float)guilt;
				if (Core.Alms.GetAltarLevel() >= 6)
				{
					purgePrice = 0f;
				}
				yield return this.StartConversationAndWait("DLG_QT_CONFESSOR", (int)purgePrice);
				if (this.DialogResponse == 0)
				{
					if (Core.Logic.Penitent.Stats.Purge.Current < purgePrice)
					{
						yield return this.StartConversationAndWait("DLG_CONFESSOR_NOTENOUGHPURGE", 0);
					}
					else
					{
						Core.GuiltManager.ResetGuilt(true);
						Core.Logic.Penitent.Stats.Purge.Current -= purgePrice;
						yield return new WaitForSecondsRealtime(1.5f);
						Core.Audio.PlaySfx("event:/Key Event/GetCulpa", 0f);
						yield return this.ShowMessage("MSG_CONFESSOR_GUILTREMOVED", 3f);
					}
				}
			}
			yield return new WaitForSeconds(0.5f);
			yield break;
		}

		private IEnumerator ShowMessage(string Id, float time)
		{
			Core.Dialog.ShowMessage(Id, 0, string.Empty, time, true);
			yield return new WaitForSecondsRealtime(time);
			yield break;
		}

		private IEnumerator StartConversationAndWait(string Id, int purge)
		{
			int noResponse = -10;
			this.DialogResponse = noResponse;
			Core.Dialog.OnDialogFinished += this.DialogEnded;
			Core.Logic.Penitent.Animator.SetBool("IS_DIALOGUE_MODE", true);
			Core.Dialog.StartConversation(Id, true, false, true, purge, false);
			while (this.DialogResponse == noResponse)
			{
				yield return null;
			}
			yield return null;
			yield break;
		}

		private void DialogEnded(string id, int response)
		{
			Core.Dialog.OnDialogFinished -= this.DialogEnded;
			Core.Logic.Penitent.Animator.SetBool("IS_DIALOGUE_MODE", false);
			this.DialogResponse = response;
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			return base.CreatePersistentData<Altar.AltarPersistenceData>();
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			Altar.AltarPersistenceData altarPersistenceData = (Altar.AltarPersistenceData)data;
		}

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float PurgeBasePrice = 100f;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private string kneeStartId;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private string kneeEndId;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float kneeStartDelay;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float kneeEndDelay;

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		[ReadOnly]
		private string OnAltarUse = "ALTAR_ACTIVATED";

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		[Required]
		private SpriteRenderer entityRenderer;

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
		[BoxGroup("Attached References", true, false, 0)]
		protected GameObject interactableAnimatorLevel4;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		protected GameObject interactableAnimatorLevel5;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		protected GameObject interactableAnimatorLevel6;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		protected GameObject interactableAnimatorLevel7;

		private bool penitentKneeing;

		private int DialogResponse;

		private class AltarPersistenceData : PersistentManager.PersistentData
		{
			public AltarPersistenceData(string id) : base(id)
			{
			}

			public bool lighted;
		}
	}
}
