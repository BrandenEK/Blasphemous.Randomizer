using System;
using System.Collections;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Sirenix.OdinInspector;
using Tools.Level;
using UnityEngine;

public class CollectibleItem : Interactable
{
	protected override void OnStart()
	{
		try
		{
			this.animationEvent = this.interactorAnimator.GetComponent<AnimatorEvent>();
			this.animationEvent.OnEventLaunched += this.OnEventLaunched;
		}
		catch (NullReferenceException)
		{
			Log.Error("Collectible", "Missing references on collectible. Disabling item.", null);
			base.gameObject.SetActive(false);
		}
	}

	private void OnEventLaunched(string id)
	{
		if (id.Equals("REMOVE_ITEM") && this.interactableAnimator != null)
		{
			this.interactableAnimator.gameObject.SetActive(false);
		}
		if (id.Equals("INTERACTION_START"))
		{
			this.PlayCollectSound();
		}
	}

	protected override void OnUpdate()
	{
		if (!base.BeingUsed && base.PlayerInRange && !base.Consumed && base.InteractionTriggered)
		{
			Core.Logic.Penitent.IsPickingCollectibleItem = true;
			base.Use();
		}
		if (base.BeingUsed)
		{
			this.PlayerReposition();
		}
	}

	public void PlayCollectSound()
	{
		if (!string.IsNullOrEmpty(this.collectItemSound))
		{
			Core.Audio.PlaySfx(this.collectItemSound, 0f);
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

	protected override void InteractionEnd()
	{
		base.Consumed = true;
		Core.Logic.Penitent.IsPickingCollectibleItem = false;
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

	[SerializeField]
	[BoxGroup("Design Settings", true, false, 0)]
	protected CollectibleItem.CollectibleHeight height;

	protected AnimatorEvent animationEvent;

	[SerializeField]
	[BoxGroup("Audio Settings", true, false, 0)]
	[EventRef]
	private string collectItemSound;

	public enum CollectibleHeight
	{
		Halfheight,
		Floor
	}
}
