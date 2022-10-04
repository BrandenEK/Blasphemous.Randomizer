using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.GameControllers.Environment.AreaEffects;
using Sirenix.OdinInspector;
using UnityEngine;

public class WindAreaParticleEffector : MonoBehaviour
{
	private void Awake()
	{
		this._windZone = base.GetComponent<WindAreaEffect>();
	}

	private void Start()
	{
		foreach (ParticleSystem particleSystem in this.psList)
		{
			this.startingXMultipliers.Add(particleSystem, particleSystem.velocityOverLifetime.xMultiplier);
		}
	}

	private void Update()
	{
		if (this.stopIfWindDisabled && this._windZone.IsDisabled)
		{
			if (this.resetPending)
			{
				this.Reset();
			}
			return;
		}
		Vector3 windForce = this._windZone.GetWindForce();
		foreach (ParticleSystem ps in this.psList)
		{
			this.UpdatePS(ps, windForce.x, false);
		}
		this.resetPending = true;
	}

	private void Reset()
	{
		this.resetPending = false;
		foreach (ParticleSystem particleSystem in this.psList)
		{
			float num = (float)Math.Sign(this.startingXMultipliers[particleSystem]);
			this.UpdatePS(particleSystem, num * 1f / this.xMultiplierFactor, true);
		}
	}

	private void UpdatePS(ParticleSystem ps, float v, bool resetting = false)
	{
		ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = ps.velocityOverLifetime;
		if (this.modifyXMultiplierRelatively)
		{
			float num = (v <= 0f) ? (-Math.Abs(v * this.startingXMultipliers[ps])) : Math.Abs(v * this.startingXMultipliers[ps]);
			if (resetting && this.resetMultiplierSmoothly)
			{
				DOTween.To(() => velocityOverLifetime.xMultiplier, delegate(float x)
				{
					velocityOverLifetime.xMultiplier = x;
				}, num * this.xMultiplierFactor, this.lerpTime);
			}
			else
			{
				velocityOverLifetime.xMultiplier = num * this.xMultiplierFactor;
			}
		}
		else
		{
			velocityOverLifetime.xMultiplier = v;
		}
		if (this.changeEmissionRate)
		{
			float num2 = Mathf.Abs(v);
			ps.emission.rateOverTime = Mathf.Lerp(this.minRate, this.maxRate, num2 / this.maxForce);
		}
	}

	public List<ParticleSystem> psList;

	public bool changeEmissionRate = true;

	[ShowIf("changeEmissionRate", true)]
	public float maxRate = 10f;

	[ShowIf("changeEmissionRate", true)]
	public float minRate = 10f;

	[ShowIf("changeEmissionRate", true)]
	public float maxForce = 15f;

	public bool stopIfWindDisabled;

	public bool modifyXMultiplierRelatively;

	[ShowIf("modifyXMultiplierRelatively", true)]
	public float xMultiplierFactor = 0.2f;

	[ShowIf("stopIfWindDisabled", true)]
	public bool resetMultiplierSmoothly;

	[ShowIf("resetMultiplierSmoothly", true)]
	public float lerpTime = 0.5f;

	private WindAreaEffect _windZone;

	private Dictionary<ParticleSystem, float> startingXMultipliers = new Dictionary<ParticleSystem, float>();

	private bool resetPending;
}
