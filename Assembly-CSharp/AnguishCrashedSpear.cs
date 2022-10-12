using System;
using DG.Tweening;
using Tools.Level.Actionables;
using UnityEngine;

public class AnguishCrashedSpear : MonoBehaviour
{
	private void Start()
	{
		this.trap.OnUsedEvent += this.OnUsed;
	}

	private void OnUsed(TriggerBasedTrap obj)
	{
		obj.OnUsedEvent -= this.OnUsed;
		base.GetComponentInChildren<BreakableDamageArea>().DamageAreaCollider.enabled = false;
		obj.transform.DOPunchPosition(Vector3.up * 0.2f, obj.lastDelay, 20, 1f, false).OnComplete(delegate
		{
			this.breakable.Use();
		});
	}

	public TriggerBasedTrap trap;

	public BreakableObject breakable;
}
