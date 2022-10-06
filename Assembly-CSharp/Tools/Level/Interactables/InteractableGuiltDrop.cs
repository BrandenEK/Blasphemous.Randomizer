using System;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Effects;
using Gameplay.UI;
using I2.Loc;
using UnityEngine;

namespace Tools.Level.Interactables
{
	public class InteractableGuiltDrop : MonoBehaviour
	{
		public void SetDropId(string dropid)
		{
			this.dropId = dropid;
		}

		private void Start()
		{
			this.guiltDropCollective = base.GetComponent<CollectibleItem>();
			this.SetOverlappedInteractable();
		}

		private void Update()
		{
			for (int i = 0; i < this.overlappedInteractables.Count; i++)
			{
				this.overlappedInteractables[i].OverlappedInteractor = true;
			}
		}

		private void SetOverlappedInteractable()
		{
			foreach (Interactable interactable in Object.FindObjectsOfType<Interactable>())
			{
				if (!interactable.Equals(this.guiltDropCollective))
				{
					if (Vector2.Distance(interactable.transform.position, base.transform.position) < 1f)
					{
						interactable.OverlappedInteractor = true;
						this.overlappedInteractables.Add(interactable);
					}
				}
			}
		}

		private void ReleaseOverlappedDoors()
		{
			if (this.overlappedInteractables.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < this.overlappedInteractables.Count; i++)
			{
				this.overlappedInteractables[i].OverlappedInteractor = false;
			}
			this.overlappedInteractables.Clear();
		}

		private void OnUsePost()
		{
			if (Core.Logic.Penitent.Dead)
			{
				return;
			}
			Core.GuiltManager.OnDropTaken(this.dropId);
			UIController.instance.ShowPopUp(ScriptLocalization.UI.GET_GUILTDROP_TEXT, this.sound, this.timeToWait, false);
			GuiltDropRecover componentInChildren = Core.Logic.Penitent.GetComponentInChildren<GuiltDropRecover>();
			if (componentInChildren)
			{
				componentInChildren.TriggerEffect();
			}
			this.ReleaseOverlappedDoors();
		}

		public string sound = "event:/Key Event/UseQuestItem";

		public float timeToWait = 2f;

		private string dropId = string.Empty;

		private List<Interactable> overlappedInteractables = new List<Interactable>();

		private CollectibleItem guiltDropCollective;
	}
}
