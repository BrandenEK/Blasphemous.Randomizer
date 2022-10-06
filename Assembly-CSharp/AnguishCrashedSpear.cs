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
		TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOPunchPosition(obj.transform, Vector3.up * 0.2f, obj.lastDelay, 20, 1f, false), delegate()
		{
			this.breakable.Use();
		});
	}

	public TriggerBasedTrap trap;

	public BreakableObject breakable;
}
