using System;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
	public class BloodParticleFixer : MonoBehaviour
	{
		private void Awake()
		{
			this.ps = base.GetComponent<ParticleSystem>();
			this.collisionEvents = new List<ParticleCollisionEvent>();
		}

		public void OnParticleCollision(GameObject other)
		{
			int num = ParticlePhysicsExtensions.GetCollisionEvents(this.ps, other, this.collisionEvents);
		}

		public ParticleSystem decalParticleSystem;

		public ParticleSystem dripParticleSystem;

		private ParticleSystem ps;

		public List<ParticleCollisionEvent> collisionEvents;
	}
}
