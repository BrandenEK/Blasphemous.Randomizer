using System;
using DG.Tweening;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class TriggerReceiver : PersistentObject, IActionable
	{
		public bool Locked { get; set; }

		protected virtual void OnUsed()
		{
		}

		public void Use()
		{
			if (this.alreadyUsed)
			{
				return;
			}
			this.OnUsed();
			this.ActivateActionable(this.target);
			this.animator.SetTrigger("ACTIVATE");
			Core.Audio.PlayOneShot(this.OnActivationSound, default(Vector3));
			this.alreadyUsed = true;
			this.DeactivateCollisions();
			if (this.showReactionTween)
			{
				base.transform.DOPunchScale(Vector3.one * 0.25f, 0.5f, 10, 1f);
			}
		}

		private void SetUsedAnimation()
		{
			this.animator.Play("USED");
			this.DeactivateCollisions();
		}

		private void DeactivateCollisions()
		{
			Collider2D component = base.GetComponent<Collider2D>();
			if (component != null)
			{
				component.enabled = false;
			}
		}

		private void ActivateActionable(GameObject[] gameObjects)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				if (!(gameObject == null))
				{
					IActionable component = gameObject.GetComponent<IActionable>();
					if (component != null)
					{
						component.Use();
					}
				}
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			TrapTriggererArea component = collision.gameObject.GetComponent<TrapTriggererArea>();
			if (component != null && component.triggerID == this.triggerID)
			{
				this.Use();
			}
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			TriggerReceiver.TriggerReceiverPersistentData triggerReceiverPersistentData = base.CreatePersistentData<TriggerReceiver.TriggerReceiverPersistentData>();
			triggerReceiverPersistentData.used = this.alreadyUsed;
			return triggerReceiverPersistentData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			TriggerReceiver.TriggerReceiverPersistentData triggerReceiverPersistentData = (TriggerReceiver.TriggerReceiverPersistentData)data;
			this.alreadyUsed = triggerReceiverPersistentData.used;
			if (this.alreadyUsed)
			{
				this.SetUsedAnimation();
			}
		}

		[SerializeField]
		[BoxGroup("References", true, false, 0)]
		public Animator animator;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected GameObject[] target = new GameObject[0];

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public string triggerID;

		[SerializeField]
		[BoxGroup("Debug settings", true, false, 0)]
		private bool showReactionTween;

		private bool alreadyUsed;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		protected string OnActivationSound;

		private class TriggerReceiverPersistentData : PersistentManager.PersistentData
		{
			public TriggerReceiverPersistentData(string id) : base(id)
			{
			}

			public bool used;
		}
	}
}
