using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemsController : MonoBehaviour
{
	public void CopyParentScaleToChildren()
	{
		if (this.childrenSystems == null || this.childrenSystems.Count == 0)
		{
			this.childrenSystems = new List<ParticleSystem>(base.GetComponentsInChildren<ParticleSystem>());
		}
		foreach (ParticleSystem particleSystem in this.childrenSystems)
		{
			particleSystem.transform.localScale = base.transform.localScale;
		}
	}

	public void PlayParticles()
	{
		if (this.rootSystem == null)
		{
			this.rootSystem = base.GetComponent<ParticleSystem>();
		}
		this.rootSystem.Play();
	}

	public void PlayParticlesInChildren()
	{
		if (this.childrenSystems == null || this.childrenSystems.Count == 0)
		{
			this.childrenSystems = new List<ParticleSystem>(base.GetComponentsInChildren<ParticleSystem>());
		}
		foreach (ParticleSystem particleSystem in this.childrenSystems)
		{
			particleSystem.Play();
		}
	}

	public void StopParticles()
	{
		if (this.rootSystem == null)
		{
			this.rootSystem = base.GetComponent<ParticleSystem>();
		}
		this.rootSystem.Stop();
	}

	public void StopParticlesInChildren()
	{
		if (this.childrenSystems == null || this.childrenSystems.Count == 0)
		{
			this.childrenSystems = new List<ParticleSystem>(base.GetComponentsInChildren<ParticleSystem>());
		}
		foreach (ParticleSystem particleSystem in this.childrenSystems)
		{
			particleSystem.Stop();
		}
	}

	private ParticleSystem rootSystem;

	private List<ParticleSystem> childrenSystems;
}
