using System;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Rewired;
using RewiredConsts;
using Sirenix.OdinInspector;
using Tools.Level;
using UnityEngine;

namespace Tools.UI
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class InputNotifier : MonoBehaviour
	{
		private void Start()
		{
			if (!this.Ready)
			{
				return;
			}
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
			this.CreateInputIcon();
			if (this.connectToInteractable != null && this.connectToInteractable.AllwaysShowIcon())
			{
				this.AllwaysShow = true;
				this.Showing = true;
				this.inputIcon.Fade(1f, this.fadeTime);
				this.inputIcon.RefreshIcon();
				this.spriteRenderer.DOFade(1f, this.fadeTime);
			}
			else
			{
				for (int i = 0; i < this.sensor.Length; i++)
				{
					if (!(this.sensor[i] == null))
					{
						this.sensor[i].OnPenitentEnter += this.ShowNotification;
						this.sensor[i].OnPenitentExit += this.HideNotification;
					}
				}
				Core.Input.OnInputLocked += this.HideNotification;
				Core.Input.OnInputUnlocked += this.ShowNotification;
			}
		}

		private void OnDestroy()
		{
			if (!this.AllwaysShow)
			{
				for (int i = 0; i < this.sensor.Length; i++)
				{
					if (!(this.sensor[i] == null))
					{
						this.sensor[i].OnPenitentEnter -= this.ShowNotification;
						this.sensor[i].OnPenitentExit -= this.HideNotification;
					}
				}
				Core.Input.OnInputLocked -= this.HideNotification;
				Core.Input.OnInputUnlocked -= this.ShowNotification;
			}
		}

		private void Update()
		{
			if (this.AllwaysShow && this.Showing && (!this.InteractableCanBeConsumed || this.InteractableIsBeingUsed))
			{
				this.Showing = false;
				this.HideNotification();
			}
		}

		private void CreateInputIcon()
		{
			this.inputIcon = UnityEngine.Object.Instantiate<InputIcon>(this.inputIconPrefab);
			SpriteRenderer componentInChildren = this.inputIcon.GetComponentInChildren<SpriteRenderer>();
			TextMesh componentInChildren2 = this.inputIcon.GetComponentInChildren<TextMesh>();
			Color color = componentInChildren.color;
			color.a = 0f;
			componentInChildren.color = color;
			color = componentInChildren2.color;
			color.a = 0f;
			componentInChildren2.color = color;
			this.inputIcon.transform.parent = base.transform;
			this.inputIcon.transform.localPosition = Vector3.zero;
			this.inputIcon.action = this.action;
		}

		private void ShowNotification()
		{
			if (!this.inputIcon)
			{
				this.CreateInputIcon();
			}
			base.Invoke("DelayerShowNotification", 0.1f);
		}

		private void DelayerShowNotification()
		{
			if (!this.Ready)
			{
				return;
			}
			if (this.requiresInteractable && (!this.InteractableCanBeConsumed || !this.InteractablePlayerInside || this.InteractableLocked))
			{
				return;
			}
			if (Core.Input.InputBlocked && !this.showWhenInputBlocked)
			{
				return;
			}
			this.inputIcon.Fade(1f, this.fadeTime);
			this.inputIcon.RefreshIcon();
			this.spriteRenderer.DOFade(1f, this.fadeTime);
		}

		private void HideNotification()
		{
			if (!this.Ready)
			{
				return;
			}
			this.inputIcon.Fade(0f, this.fadeTime);
		}

		private bool Ready
		{
			get
			{
				return this.sensor != null;
			}
		}

		private bool InteractableCanBeConsumed
		{
			get
			{
				return this.connectToInteractable != null && this.connectToInteractable.CanBeConsumed();
			}
		}

		private bool InteractableIsBeingUsed
		{
			get
			{
				return this.connectToInteractable != null && this.connectToInteractable.BeingUsed;
			}
		}

		private bool InteractableLocked
		{
			get
			{
				return this.connectToInteractable != null && this.connectToInteractable.Locked;
			}
		}

		private bool InteractablePlayerInside
		{
			get
			{
				return this.connectToInteractable != null && this.connectToInteractable.PlayerInRange;
			}
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[ActionIdProperty(typeof(RewiredConsts.Action))]
		private int action;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool requiresInteractable = true;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("requiresInteractable", true)]
		private Interactable connectToInteractable;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool showWhenInputBlocked;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private InputIcon inputIconPrefab;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 0)]
		private CollisionSensor[] sensor;

		private float fadeTime = 0.15f;

		private SpriteRenderer spriteRenderer;

		private InputIcon inputIcon;

		private bool AllwaysShow;

		private bool Showing;
	}
}
