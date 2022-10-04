using System;
using Gameplay.GameControllers.Bosses.Amanecidas;
using Gameplay.GameControllers.Effects.Entity;
using UnityEngine;

public class AmanecidaCrystal : MonoBehaviour
{
	private void Awake()
	{
		this.damageable = base.GetComponent<SimpleDamageableObject>();
		this.damageable.OnDamageableDestroyed += this.Damageable_OnDamageableDestroyed;
		this.fx = base.GetComponentInChildren<MasterShaderEffects>();
	}

	private void Damageable_OnDamageableDestroyed()
	{
		this.Explode();
	}

	private void OnEnable()
	{
		this.ama = AmanecidasFightSpawner.Instance.currentBoss.GetComponent<AmanecidasBehaviour>();
	}

	private void Update()
	{
		this.UpdateColorByTTL();
	}

	private void UpdateColorByTTL()
	{
		float colorizeStrength = 1f - this.damageable.currentTtl / this.damageable.durationToUse;
		this.fx.SetColorizeStrength(colorizeStrength);
	}

	private void Explode()
	{
		if (this.ama != null)
		{
			this.ama.OnCrystalExplode(this);
		}
	}

	public void MultiplyCurrentTimeToLive(float multiplier, float maxTtl)
	{
		this.damageable.currentTtl *= multiplier;
		if (this.damageable.currentTtl > maxTtl)
		{
			this.damageable.currentTtl = maxTtl;
		}
	}

	private AmanecidasBehaviour ama;

	private SimpleDamageableObject damageable;

	private MasterShaderEffects fx;
}
