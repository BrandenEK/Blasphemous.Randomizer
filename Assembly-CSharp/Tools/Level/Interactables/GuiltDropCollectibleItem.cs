using System;
using System.Collections;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using UnityEngine;

namespace Tools.Level.Interactables
{
	public class GuiltDropCollectibleItem : CollectibleItem
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this.animationEvent = this.interactorAnimator.GetComponent<AnimatorEvent>();
		}

		protected override void OnUpdate()
		{
			if (!base.BeingUsed && base.PlayerInRange && !base.Consumed && base.InteractionTriggered)
			{
				base.Use();
			}
			if (base.BeingUsed)
			{
				this.PlayerReposition();
			}
		}

		protected override IEnumerator OnUse()
		{
			this.ShowPlayer(false);
			if (this.interactorAnimator)
			{
				this.interactorAnimator.Play(this.Animation);
			}
			yield return new WaitForEndOfFrame();
			while (!base.Consumed)
			{
				yield return new WaitForEndOfFrame();
			}
			this.ShowPlayer(true);
			yield break;
		}

		protected override IEnumerator UseCorroutine()
		{
			this.InteractionStart();
			yield return new WaitForSeconds(this.VanishAnimDuration);
			this.InteractionEnd();
			yield break;
		}

		protected override void InteractionStart()
		{
			base.InteractionStart();
			base.gameObject.SendMessage("OnUsePre", SendMessageOptions.DontRequireReceiver);
			this.interactableAnimator.SetTrigger("TAKE");
			this.PlayVanishGuilt();
		}

		protected override void InteractionEnd()
		{
			base.Consumed = true;
			base.gameObject.SendMessage("OnUsePost", SendMessageOptions.DontRequireReceiver);
			this.interactableAnimator.transform.parent.gameObject.SetActive(false);
		}

		private string Animation
		{
			get
			{
				if (this.height == CollectibleItem.CollectibleHeight.Floor)
				{
					return "Floor Collection";
				}
				if (this.height == CollectibleItem.CollectibleHeight.Halfheight)
				{
					return "Halfheight Collection";
				}
				Log.Error("Collectible", "Error selecting animation.", null);
				return string.Empty;
			}
		}

		private void PlayVanishGuilt()
		{
			if (string.IsNullOrEmpty(this.VanishingGuiltFx))
			{
				return;
			}
			Core.Audio.PlaySfx(this.VanishingGuiltFx, 0f);
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			Interactable.InteractablePersistence interactablePersistence = base.CreatePersistentData<Interactable.InteractablePersistence>();
			interactablePersistence.Consumed = base.Consumed;
			return interactablePersistence;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			Interactable.InteractablePersistence interactablePersistence = (Interactable.InteractablePersistence)data;
			base.Consumed = interactablePersistence.Consumed;
			if (base.Consumed)
			{
				this.interactableAnimator.gameObject.SetActive(false);
			}
		}

		public float VanishAnimDuration = 0.45f;

		[EventRef]
		public string VanishingGuiltFx;
	}
}
