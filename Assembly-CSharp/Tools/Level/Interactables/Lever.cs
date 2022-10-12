using System;
using System.Collections;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Interactables
{
	public class Lever : Interactable
	{
		private void Start()
		{
			base.AnimatorEvent.OnEventLaunched += this.OnEventLaunched;
		}

		protected override IEnumerator OnUse()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.SpriteRenderer.enabled = false;
			penitent.DamageArea.enabled = false;
			penitent.SetOrientation(base.ObjectOrientation, true, false);
			this.PlayAudio(this.activationSound);
			this.interactableAnimator.SetBool("INSTANT_ANIM", false);
			this.interactableAnimator.SetBool("ACTIVE", !base.Consumed);
			this.interactorAnimator.SetTrigger((!base.Consumed) ? "DOWN" : "UP");
			yield return new WaitForSeconds(this.animationDelay);
			yield break;
		}

		[Button("RESET LEVER UP", ButtonSizes.Large)]
		public void SetLeverUp()
		{
			Debug.Log(base.gameObject.name + ": SET LEVER UP");
			this.PlayAudio(this.activationSound);
			this.interactableAnimator.SetBool("ACTIVE", false);
			base.StartCoroutine(this.DelayedCallback(new Action(this.OnLeverUpFinished), this.animationDelay));
		}

		[Button("SET LEVER UP INSTANTLY", ButtonSizes.Large)]
		public void SetLeverUpInstantly()
		{
			Debug.Log(base.gameObject.name + ": SET LEVER UP instant");
			this.interactableAnimator.Play("Up", 0, 1f);
			this.interactableAnimator.SetBool("ACTIVE", false);
			base.Consumed = false;
		}

		[Button("BLOCK LEVER DOWN", ButtonSizes.Large)]
		public void SetLeverDown()
		{
			Debug.Log(base.gameObject.name + ": SET LEVER DOWN");
			this.PlayAudio(this.activationSound);
			this.interactableAnimator.SetBool("ACTIVE", true);
			base.Consumed = true;
			base.StartCoroutine(this.DelayedCallback(new Action(this.OnLeverDownFinished), this.animationDelay));
		}

		[Button("SET LEVER DOWN INSTANTLY", ButtonSizes.Large)]
		public void SetLeverDownInstantly()
		{
			Debug.Log(base.gameObject.name + ": SET LEVER DOWN instant");
			this.interactableAnimator.Play("Down", 0, 1f);
			this.interactableAnimator.SetBool("ACTIVE", true);
			base.Consumed = true;
		}

		private void OnLeverDownFinished()
		{
			this.active = false;
			Core.Events.LaunchEvent((!this.active) ? this.onDeactivation : this.onActivation, string.Empty);
		}

		private void OnLeverUpFinished()
		{
			base.Consumed = false;
			this.active = true;
			Core.Events.LaunchEvent((!this.active) ? this.onDeactivation : this.onActivation, string.Empty);
		}

		protected IEnumerator DelayedCallback(Action callback, float seconds)
		{
			yield return new WaitForSeconds(seconds);
			callback();
			yield break;
		}

		protected override void InteractionEnd()
		{
			base.Consumed = !base.Consumed;
			Penitent penitent = Core.Logic.Penitent;
			penitent.SpriteRenderer.enabled = true;
			penitent.DamageArea.enabled = true;
			this.active = !this.active;
			Core.Events.LaunchEvent((!this.active) ? this.onDeactivation : this.onActivation, string.Empty);
		}

		private void OnEventLaunched(string id)
		{
			if (id.Equals("ACTIVATE"))
			{
				this.ActivateActionable(this.target);
			}
		}

		protected override void OnBlocked(bool blocked)
		{
			base.OnBlocked(blocked);
			Core.Events.LaunchEvent((!blocked) ? this.onUnlocked : this.onLocked, string.Empty);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!base.BeingUsed && base.PlayerInRange && base.InteractionTriggered && this.CanBeConsumed())
			{
				base.Use();
			}
			if (base.BeingUsed)
			{
				this.PlayerReposition();
			}
		}

		public override bool CanBeConsumed()
		{
			return this.mode == LeverMode.MultipleActivation || !base.Consumed;
		}

		protected override void PlayerReposition()
		{
			Core.Logic.Penitent.transform.position = this.interactableAnimator.transform.position;
		}

		private void ActivateActionable(GameObject[] gameObjects)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				if (!(gameObject == null))
				{
					IActionable component = gameObject.GetComponent<IActionable>();
					if (component != null && this.action == LeverAction.Interact)
					{
						component.Use();
					}
					if (component != null && this.action == LeverAction.Unlock)
					{
						component.Locked = false;
					}
				}
			}
		}

		private void PlayAudio(string activationSoundKey)
		{
			SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
			if (componentInChildren && componentInChildren.isVisible)
			{
				Core.Audio.PlaySfx(activationSoundKey, this.soundDelay);
			}
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			Lever.LeverPersistenceData leverPersistenceData = base.CreatePersistentData<Lever.LeverPersistenceData>();
			leverPersistenceData.AlreadyUsed = base.Consumed;
			return leverPersistenceData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			Lever.LeverPersistenceData leverPersistenceData = (Lever.LeverPersistenceData)data;
			base.Consumed = leverPersistenceData.AlreadyUsed;
			if (base.Consumed)
			{
				this.SetLeverDownInstantly();
			}
			else
			{
				this.SetLeverUpInstantly();
			}
		}

		private void OnDestroy()
		{
			base.AnimatorEvent.OnEventLaunched -= this.OnEventLaunched;
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected GameObject[] target = new GameObject[0];

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool IsBlocked;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float animationDelay;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private LeverMode mode;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private LeverAction action;

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		protected string onActivation;

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		protected string onDeactivation;

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		protected string onLocked;

		[SerializeField]
		[BoxGroup("Event Settings", true, false, 0)]
		protected string onUnlocked;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		protected string activationSound;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		protected float soundDelay;

		private bool active;

		private class LeverPersistenceData : PersistentManager.PersistentData
		{
			public LeverPersistenceData(string id) : base(id)
			{
			}

			public bool AlreadyUsed;
		}
	}
}
