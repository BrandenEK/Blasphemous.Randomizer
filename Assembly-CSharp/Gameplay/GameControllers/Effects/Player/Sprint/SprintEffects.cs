using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Sprint
{
	public class SprintEffects : MonoBehaviour
	{
		private void Awake()
		{
			PoolManager.Instance.CreatePool(this.sprintStartPrefab, 4);
			PoolManager.Instance.CreatePool(this.sprintFootstepPrefab, 10);
		}

		public void EmitFeet(Vector2 position)
		{
			PoolManager.Instance.ReuseObject(this.sprintFootstepPrefab, position, Quaternion.identity, false, 1);
		}

		private void SetScaleFromOrientation()
		{
			Vector3 one = Vector3.one;
			Vector3 vector;
			vector..ctor(-1f, 1f, 1f);
			this.feetParticle.transform.localScale = ((Core.Logic.Penitent.Status.Orientation != EntityOrientation.Right) ? vector : one);
		}

		public void EmitOnStart()
		{
			PoolManager.Instance.ReuseObject(this.sprintStartPrefab, this.startParticles.transform.position, Quaternion.identity, false, 1);
		}

		public ParticleSystem feetParticle;

		public ParticleSystem startParticles;

		public ParticleSystem constantParticles;

		public GameObject sprintStartPrefab;

		public GameObject sprintFootstepPrefab;
	}
}
