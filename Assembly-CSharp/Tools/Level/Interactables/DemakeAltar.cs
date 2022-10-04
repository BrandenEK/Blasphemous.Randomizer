using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI;
using Gameplay.UI.Widgets;
using Sirenix.OdinInspector;
using Tools.DataContainer;
using UnityEngine;

namespace Tools.Level.Interactables
{
	[SelectionBase]
	public class DemakeAltar : Interactable
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
			base.CheckAnimationEvents();
		}

		protected override void ObjectEnable()
		{
			if (base.AnimatorEvent == null)
			{
				return;
			}
			base.AnimatorEvent.OnEventLaunched += this.OnAnimationEvent;
			Core.Audio.PlayEventNoCatalog(ref this.arcadeEventInstance, "event:/SFX/DEMAKE/ArcadeMusic", base.transform.position);
		}

		protected override void ObjectDisable()
		{
			if (base.AnimatorEvent == null)
			{
				return;
			}
			base.AnimatorEvent.OnEventLaunched -= this.OnAnimationEvent;
			Core.Audio.StopEvent(ref this.arcadeEventInstance);
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
			Core.Events.LaunchEvent(this.OnAltarUse, base.name);
			yield return this.DemakeAltarCourrutine();
			yield break;
		}

		private IEnumerator DemakeAltarCourrutine()
		{
			yield return this.StartConversationAndWait("DLG_QT_ALTARDEMAKE", (int)this.Price);
			if (this.DialogResponse == 0)
			{
				if (Core.Logic.Penitent.Stats.Purge.Current < this.Price)
				{
					yield return this.StartConversationAndWait("DLG_CONFESSOR_NOTENOUGHPURGE", 0);
					yield return new WaitForSeconds(0.5f);
					this.interactorAnimator.SetTrigger("KNEE_END");
					Core.Audio.PlaySfxOnCatalog(this.kneeEndId, this.kneeEndDelay);
				}
				else
				{
					Core.Audio.PlaySfx(this.insertCoinEvent, 0f);
					Core.Logic.Penitent.Stats.Purge.Current -= this.Price;
					Core.Persistence.SaveGame(false);
					yield return new WaitForSeconds(1.5f);
					Core.Audio.Ambient.StopCurrent();
					Core.Audio.StopEvent(ref this.arcadeEventInstance);
					Core.Audio.PlayNamedSound("event:/Background Layer/DemakePressStartScreen", "DEMAKE_INTRO");
					FadeWidget.OnFadeShowEnd += this.OnFadeShowEnd;
					FadeWidget.instance.StartEasyFade(new Color(0f, 0f, 0f, 0f), Color.black, 1f, true);
				}
			}
			else
			{
				this.interactorAnimator.SetTrigger("KNEE_END");
				Core.Audio.PlaySfxOnCatalog(this.kneeEndId, this.kneeEndDelay);
			}
			yield break;
		}

		private void OnFadeShowEnd()
		{
			FadeWidget.OnFadeShowEnd -= this.OnFadeShowEnd;
			UIController.instance.ShowIntroDemakeWidget(new Action(this.OnPressStart));
		}

		private void OnPressStart()
		{
			Core.DemakeManager.StartDemakeRun();
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

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private string FirstDemakeScene;

		[SerializeField]
		[BoxGroup("Config", true, false, 0)]
		private float Price = 2500f;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private string kneeStartId;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private string kneeEndId;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string insertCoinEvent;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float kneeStartDelay;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float kneeEndDelay;

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		[ReadOnly]
		private string OnAltarUse = "DEMAKE_ALTAR_ACTIVATED";

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		[Required]
		private SpriteRenderer entityRenderer;

		[FoldoutGroup("Intro cutscene", 0)]
		public CutsceneData demakeIntroCutscene;

		[FoldoutGroup("Intro cutscene", 0)]
		public bool mute;

		[FoldoutGroup("Intro cutscene", 0)]
		public float fadeTimeStart = 0.5f;

		[FoldoutGroup("Intro cutscene", 0)]
		public float fadeTimeEnd = 0.5f;

		[FoldoutGroup("Intro cutscene", 0)]
		public bool useBackground;

		private bool penitentKneeing;

		private int DialogResponse;

		private Level prevLevel;

		private EventInstance arcadeEventInstance;
	}
}
