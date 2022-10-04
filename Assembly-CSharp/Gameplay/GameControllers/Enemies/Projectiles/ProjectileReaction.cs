using System;
using System.Diagnostics;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class ProjectileReaction : MonoBehaviour, IDamageable
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ProjectileReaction> OnProjectileHit;

		public void Damage(Hit hit)
		{
			if (this.IsPhysical)
			{
				this.TakeDamage(hit, null);
			}
			else if (hit.DestroysProjectiles)
			{
				this.TakeDamage(hit, null);
			}
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private void TakeDamage(Hit hit, Action callback = null)
		{
			string eventKey = string.IsNullOrEmpty(this.hitSound) ? hit.HitSoundId : this.hitSound;
			this.DestructorHit = hit.DestroysProjectiles;
			Core.Audio.EventOneShotPanned(eventKey, base.transform.position);
			if (Math.Abs(this.slowTimeDuration) > Mathf.Epsilon)
			{
				Core.Logic.ScreenFreeze.Freeze(0.1f, this.slowTimeDuration, 0f, this.slowTimeCurve);
			}
			if (this.OnProjectileHit != null)
			{
				this.OnProjectileHit(this);
			}
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return this.IsPhysical;
		}

		public Projectile owner;

		[FormerlySerializedAs("IsMaterial")]
		public bool IsPhysical;

		public bool DestroyedByNormalHits;

		public AnimationCurve slowTimeCurve;

		public float slowTimeDuration;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string hitSound;

		public bool DestructorHit;
	}
}
