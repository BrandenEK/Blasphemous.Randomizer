using System;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Managers;
using Framework.Pooling;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

public class SimpleDamageableObject : PoolObject, IDamageable
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnDamageableDestroyed;

	public bool BleedOnImpact()
	{
		return this.bleedOnImpact;
	}

	private void Awake()
	{
		foreach (GameObject prefab in this.instantiateOnDestroy)
		{
			PoolManager.Instance.CreatePool(prefab, 1);
		}
	}

	private void Update()
	{
		if (this.hasLimitedDuration)
		{
			this.currentTtl -= Time.deltaTime;
			if (this.currentTtl < 0f)
			{
				this.DoDestroy();
			}
		}
	}

	public override void OnObjectReuse()
	{
		base.OnObjectReuse();
		this.currentHp = this.hpMax;
		if (this.usesRandomDurationRange)
		{
			this.durationToUse = Random.Range(this.durationRange.x, this.durationRange.y);
		}
		else
		{
			this.durationToUse = this.duration;
		}
		this.currentTtl = this.durationToUse;
		this.firstHitIgnored = false;
	}

	private void EffectsOnDeath()
	{
		if (this.screenfreezeOnDeath)
		{
			Core.Logic.ScreenFreeze.Freeze(0.15f, 0.1f, 0f, null);
		}
		if (this.shockwaveOnDeath)
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.3f, 0.1f, 0.4f);
		}
		if (this.screenshakeOnDeath)
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 1f, 12, 0.2f, 0.01f, default(Vector3), 0.01f, true);
		}
	}

	public void SetFlip(bool flip)
	{
		SpriteRenderer componentInChildren = base.GetComponentInChildren<SpriteRenderer>();
		if (componentInChildren != null)
		{
			componentInChildren.flipX = flip;
		}
	}

	public void Damage(Hit hit)
	{
		if (!this.firstHitIgnored && this.shouldIgnoreFirstHit)
		{
			this.firstHitIgnored = true;
		}
		else
		{
			this.currentHp -= hit.DamageAmount;
			if (this.currentHp <= 0f)
			{
				this.DoDestroy();
			}
		}
	}

	protected virtual void DoDestroy()
	{
		foreach (GameObject prefab in this.instantiateOnDestroy)
		{
			PoolManager.Instance.ReuseObject(prefab, base.transform.position + this.instantiationOffset, Quaternion.identity, false, 1);
		}
		this.EffectsOnDeath();
		if (this.OnDamageableDestroyed != null)
		{
			this.OnDamageableDestroyed();
		}
		base.Destroy();
	}

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	public bool SparkOnImpact()
	{
		return this.sparkOnImpact;
	}

	[FoldoutGroup("General", 0)]
	public float hpMax = 30f;

	[FoldoutGroup("Effects", 0)]
	public bool sparkOnImpact;

	[FoldoutGroup("Effects", 0)]
	public bool bleedOnImpact;

	[FoldoutGroup("Effects", 0)]
	public bool screenshakeOnDeath;

	[FoldoutGroup("Effects", 0)]
	public bool shockwaveOnDeath;

	[FoldoutGroup("Effects", 0)]
	public bool screenfreezeOnDeath;

	[FoldoutGroup("Destruction", 0)]
	public List<GameObject> instantiateOnDestroy;

	[FoldoutGroup("Destruction", 0)]
	public Vector2 instantiationOffset;

	[FoldoutGroup("Destruction", 0)]
	public bool shouldIgnoreFirstHit;

	[FoldoutGroup("Destruction", 0)]
	public bool hasLimitedDuration;

	[FoldoutGroup("Destruction", 0)]
	[ShowIf("hasLimitedDuration", true)]
	public bool usesRandomDurationRange;

	[FoldoutGroup("Destruction", 0)]
	[HideIf("usesRandomDurationRange", true)]
	public float duration;

	[FoldoutGroup("Destruction", 0)]
	[ShowIf("usesRandomDurationRange", true)]
	[MinMaxSlider(0f, 10f, true)]
	public Vector2 durationRange;

	private float currentHp;

	private bool firstHitIgnored;

	[HideInInspector]
	public float durationToUse;

	[HideInInspector]
	public float currentTtl;
}
