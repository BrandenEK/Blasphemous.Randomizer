using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using Tools.Level;
using UnityEngine;

public class CustomInteraction : Interactable
{
	[BoxGroup("Design Settings", true, false, 0)]
	[ReadOnly]
	protected override void OnUpdate()
	{
		if (!base.BeingUsed && base.PlayerInRange && !base.Consumed && base.InteractionTriggered)
		{
			base.Use();
		}
	}

	protected override IEnumerator OnUse()
	{
		this.interactableAnimator.SetTrigger("INTERACTED");
		this.interactorAnimator.SetTrigger("INTERACTED");
		if (this.RepositionBeforeInteract)
		{
			Core.Logic.Penitent.DrivePlayer.MoveToPosition(this.InteractionPosition(), this.orientation);
		}
		yield return new WaitForEndOfFrame();
		yield break;
	}

	private void Activate()
	{
	}

	private Vector2 InteractionPosition()
	{
		if (this.Waypoint != null)
		{
			return this.Waypoint.position;
		}
		Vector2 result;
		result..ctor(base.transform.position.x, base.transform.position.y);
		if (this.orientation == EntityOrientation.Right)
		{
			result.x -= 1f;
		}
		else
		{
			result.x += 1f;
		}
		return result;
	}

	protected override void InteractionEnd()
	{
		if (this.useOnce)
		{
			base.Consumed = true;
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
	}

	[SerializeField]
	[BoxGroup("Design Settings", true, false, 0)]
	private bool useOnce;
}
