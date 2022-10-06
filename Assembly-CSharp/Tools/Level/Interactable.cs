using System;
using System.Collections;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Rewired;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools.UI;
using UnityEngine;

namespace Tools.Level
{
	[SelectionBase]
	public class Interactable : PersistentObject, IActionable
	{
		private protected AnimatorEvent AnimatorEvent { protected get; private set; }

		public bool OverlappedInteractor { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.InteractableEvent SConsumed;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.InteractableEvent Created;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.InteractableEvent SPenitentExit;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.InteractableEvent SPenitentEnter;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.InteractableEvent SLocked;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.InteractableEvent SUnlocked;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.InteractableEvent SInteractionStarted;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Core.InteractableEvent SInteractionEnded;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnStartUsing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnStopUsing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnLocked;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnUnlocked;

		public bool BeingUsed { get; private set; }

		public EntityOrientation ObjectOrientation
		{
			get
			{
				return this.orientation;
			}
			set
			{
				this.orientation = value;
				this.RefreshOrientaiton();
			}
		}

		public bool Locked
		{
			get
			{
				this.OnBlocked(this.blocked);
				return this.blocked;
			}
			set
			{
				this.blocked = value;
				if (this.blocked && this.OnLocked != null)
				{
					this.OnLocked();
				}
				if (this.blocked && Interactable.SLocked != null)
				{
					Interactable.SLocked(this);
				}
				if (!this.blocked && this.OnUnlocked != null)
				{
					this.OnUnlocked();
				}
				if (this.blocked && Interactable.SUnlocked != null)
				{
					Interactable.SUnlocked(this);
				}
			}
		}

		public EntityOrientation PlayerDirection
		{
			get
			{
				return (base.transform.InverseTransformPoint(Core.Logic.Penitent.transform.position).x >= 0f) ? EntityOrientation.Right : EntityOrientation.Left;
			}
		}

		public bool InteractionTriggered
		{
			get
			{
				return ReInput.isReady && ReInput.players.GetPlayer(0).GetButtonDown(8) && (!Core.Logic.Penitent.IsJumping || this.interactableWhileJumping) && !Core.Logic.Penitent.IsGrabbingCliffLede && !Core.Input.InputBlocked && !this.OverlappedInteractor;
			}
		}

		public bool PlayerInRange { get; set; }

		public bool Consumed
		{
			get
			{
				return this.consumed;
			}
			set
			{
				if (this.consumed != value)
				{
					if (this.consumed && Interactable.SConsumed != null)
					{
						Interactable.SConsumed(this);
					}
					this.consumed = value;
				}
			}
		}

		public virtual bool CanBeConsumed()
		{
			return !this.Consumed;
		}

		public virtual bool AllwaysShowIcon()
		{
			return false;
		}

		public void Use()
		{
			base.StartCoroutine(this.UseCorroutine());
		}

		public void UseEvenIfInputBlocked()
		{
			base.StartCoroutine(this.UseEvenIfInputBlockedCorroutine());
		}

		protected virtual void InteractionStart()
		{
		}

		protected virtual void InteractionEnd()
		{
		}

		protected virtual IEnumerator OnUse()
		{
			yield return 0;
			yield break;
		}

		protected virtual IEnumerator UseCorroutine()
		{
			if (!Core.Input.InputBlocked && !this.Locked)
			{
				if (this.RepositionBeforeInteract)
				{
					Core.Logic.Penitent.DrivePlayer.OnStopMotion += this.OnStopReposition;
				}
				if (this.OnStartUsing != null)
				{
					this.OnStartUsing();
				}
				if (Interactable.SInteractionStarted != null && !this.RepositionBeforeInteract)
				{
					Interactable.SInteractionStarted(this);
				}
				Log.Trace("Interactable", "Starting using: " + base.name, null);
				base.gameObject.SendMessage("OnUsePre", 1);
				yield return base.StartCoroutine(this.OnUse());
				base.gameObject.SendMessage("OnUsePost", 1);
				Log.Trace("Interactable", "Finished using: " + base.name, null);
				if (this.OnStopUsing != null)
				{
					this.OnStopUsing();
				}
				if (Interactable.SInteractionEnded != null)
				{
					Interactable.SInteractionEnded(this);
				}
				yield return null;
			}
			yield break;
		}

		protected virtual IEnumerator UseEvenIfInputBlockedCorroutine()
		{
			if (!this.Locked)
			{
				if (this.RepositionBeforeInteract)
				{
					Core.Logic.Penitent.DrivePlayer.OnStopMotion += this.OnStopReposition;
				}
				if (this.OnStartUsing != null)
				{
					this.OnStartUsing();
				}
				if (Interactable.SInteractionStarted != null && !this.RepositionBeforeInteract)
				{
					Interactable.SInteractionStarted(this);
				}
				Log.Trace("Interactable", "Starting using: " + base.name, null);
				base.gameObject.SendMessage("OnUsePre", 1);
				yield return base.StartCoroutine(this.OnUse());
				base.gameObject.SendMessage("OnUsePost", 1);
				Log.Trace("Interactable", "Finished using: " + base.name, null);
				if (this.OnStopUsing != null)
				{
					this.OnStopUsing();
				}
				if (Interactable.SInteractionEnded != null)
				{
					Interactable.SInteractionEnded(this);
				}
				yield return null;
			}
			yield break;
		}

		private void OnStopReposition()
		{
			if (Interactable.SInteractionStarted != null)
			{
				Interactable.SInteractionStarted(this);
			}
			Core.Logic.Penitent.DrivePlayer.OnStopMotion -= this.OnStopReposition;
		}

		protected virtual void OnPlayerReady()
		{
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnDispose()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void ObjectEnable()
		{
		}

		protected virtual void ObjectDisable()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnEditorValidate()
		{
		}

		protected virtual void TriggerEnter(Entity entity)
		{
		}

		protected virtual void TriggerExit(Entity entity)
		{
		}

		protected virtual void OnBlocked(bool blocked)
		{
		}

		protected virtual void PlayerReposition()
		{
			Core.Logic.Penitent.transform.position = this.interactorAnimator.transform.position;
		}

		private void OnPenitentReady(Penitent penitent)
		{
			this.OnPlayerReady();
			SpawnManager.OnPlayerSpawn -= this.OnPenitentReady;
		}

		private void Awake()
		{
			SpawnManager.OnPlayerSpawn += this.OnPenitentReady;
			for (int i = 0; i < this.sensors.Length; i++)
			{
				if (!(this.sensors[i] == null))
				{
					this.sensors[i].OnEntityEnter += this.OnEntityEnter;
					this.sensors[i].OnEntityExit += this.OnEntityExit;
					this.sensors[i].SensorTriggerStay += this.SensorTriggerStay;
				}
			}
			this.CheckAnimationEvents();
			this.OnAwake();
			if (Interactable.Created != null)
			{
				Interactable.Created(this);
			}
		}

		protected void CheckAnimationEvents()
		{
			if (this.AnimatorEvent != null)
			{
				this.AnimatorEvent.OnEventLaunched -= this.OnEventLaunched;
			}
			if (this.interactorAnimator != null)
			{
				this.AnimatorEvent = this.interactorAnimator.GetComponent<AnimatorEvent>();
			}
			if (this.AnimatorEvent != null)
			{
				this.AnimatorEvent.OnEventLaunched += this.OnEventLaunched;
			}
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPenitentReady;
			for (int i = 0; i < this.sensors.Length; i++)
			{
				if (!(this.sensors[i] == null))
				{
					this.sensors[i].OnEntityEnter -= this.OnEntityEnter;
					this.sensors[i].OnEntityExit -= this.OnEntityExit;
					this.sensors[i].SensorTriggerStay -= this.SensorTriggerStay;
				}
			}
			if (this.AnimatorEvent != null)
			{
				this.AnimatorEvent.OnEventLaunched -= this.OnEventLaunched;
			}
			if (this.RepositionBeforeInteract && Core.Logic.Penitent)
			{
				Core.Logic.Penitent.DrivePlayer.OnStopMotion -= this.OnStopReposition;
			}
			this.OnDispose();
		}

		private void OnEventLaunched(string id)
		{
			if (id == "INTERACTION_START")
			{
				this.BeingUsed = true;
				Core.Input.SetBlocker("INTERACTABLE", true);
				this.InteractionStart();
			}
			else if (id == "INTERACTION_END")
			{
				this.BeingUsed = false;
				Core.Input.SetBlocker("INTERACTABLE", false);
				this.InteractionEnd();
			}
		}

		private void Start()
		{
			this.OnStart();
		}

		private void Update()
		{
			this.OnUpdate();
		}

		private void OnEnable()
		{
			this.ObjectEnable();
		}

		private void OnDisable()
		{
			this.ObjectDisable();
		}

		private void OnEntityEnter(Entity entity)
		{
			this.Locked = !this.HasRequiredItem();
			if (entity.CompareTag("Penitent"))
			{
				this.PlayerInRange = true;
				if (Interactable.SPenitentEnter != null)
				{
					Interactable.SPenitentEnter(this);
				}
			}
			this.TriggerEnter(entity);
		}

		private void OnEntityExit(Entity entity)
		{
			if (entity.CompareTag("Penitent"))
			{
				this.PlayerInRange = false;
				if (Interactable.SPenitentExit != null)
				{
					Interactable.SPenitentExit(this);
				}
			}
			this.TriggerExit(entity);
		}

		private void SensorTriggerStay(Collider2D col)
		{
			if (col.CompareTag("Penitent"))
			{
				this.PlayerInRange = true;
			}
		}

		private bool HasRequiredItem()
		{
			if (this.needObject && this.requiredItem != null && !StringExtensions.IsNullOrWhitespace(this.requiredItem.id))
			{
				BaseInventoryObject invObject = this.requiredItem.GetInvObject();
				return Core.InventoryManager.IsBaseObjectEquipped(invObject);
			}
			return true;
		}

		public void EnableInputIcon(bool enable)
		{
			InputNotifier componentInChildren = base.gameObject.GetComponentInChildren<InputNotifier>();
			if (componentInChildren != null)
			{
				componentInChildren.enabled = enable;
			}
		}

		protected void RefreshOrientaiton()
		{
			if (this.interactableAnimator == null)
			{
				return;
			}
			SpriteRenderer[] componentsInChildren = this.interactableAnimator.GetComponentsInChildren<SpriteRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].flipX = (this.orientation == EntityOrientation.Left);
			}
		}

		protected virtual void ShowPlayer(bool show)
		{
			Core.Logic.Penitent.SpriteRenderer.enabled = show;
			Core.Logic.Penitent.DamageArea.enabled = show;
		}

		private void OnValidate()
		{
			this.RefreshOrientaiton();
			this.OnEditorValidate();
		}

		[SerializeField]
		[BoxGroup("Interaction Reposition", true, false, 0)]
		protected bool RepositionBeforeInteract = true;

		[SerializeField]
		[BoxGroup("Interaction Reposition", true, false, 0)]
		[ShowIf("RepositionBeforeInteract", true)]
		protected Transform Waypoint;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected bool needObject;

		[ShowIf("needObject", true)]
		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected InventoryObjectInspector requiredItem;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected bool interactableWhileJumping;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected EntityOrientation orientation;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 10)]
		protected CollisionSensor[] sensors;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 10)]
		protected Animator interactableAnimator;

		[SerializeField]
		[BoxGroup("Attached References", true, false, 10)]
		protected Animator interactorAnimator;

		private bool consumed;

		private bool blocked;

		protected class InteractablePersistence : PersistentManager.PersistentData
		{
			public InteractablePersistence(string id) : base(id)
			{
			}

			public bool Consumed;
		}
	}
}
