using System;
using System.Collections;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Interactables
{
	public class Chest : Interactable
	{
		protected override IEnumerator OnUse()
		{
			this.ShowPlayer(false);
			Core.Audio.PlaySfx(this.activationSound, this.soundDelay);
			this.interactableAnimator.SetBool("USED", true);
			this.interactorAnimator.SetTrigger("USED");
			Core.Logic.Penitent.SetOrientation(base.ObjectOrientation, true, false);
			while (!base.Consumed)
			{
				yield return new WaitForEndOfFrame();
			}
			this.ShowPlayer(true);
			yield break;
		}

		protected override void OnUpdate()
		{
			if (!base.BeingUsed && this.mode == ChestMode.Interactable && base.PlayerInRange && !base.Consumed && base.InteractionTriggered)
			{
				base.Use();
			}
			if (base.BeingUsed)
			{
				this.PlayerReposition();
			}
		}

		protected override void InteractionEnd()
		{
			base.Consumed = true;
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			Log.Trace("Chest", "Getting current persistence.", null);
			Chest.ChestPersistenceData chestPersistenceData = base.CreatePersistentData<Chest.ChestPersistenceData>();
			chestPersistenceData.AlreadyUsed = base.Consumed;
			return chestPersistenceData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			Log.Trace("Chest", "Setting current persistence.", null);
			Chest.ChestPersistenceData chestPersistenceData = (Chest.ChestPersistenceData)data;
			base.Consumed = chestPersistenceData.AlreadyUsed;
			this.interactableAnimator.SetBool("NOANIMUSED", base.Consumed);
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private ChestMode mode;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string activationSound;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private float soundDelay;

		private class ChestPersistenceData : PersistentManager.PersistentData
		{
			public ChestPersistenceData(string id) : base(id)
			{
			}

			public bool AlreadyUsed;
		}
	}
}
