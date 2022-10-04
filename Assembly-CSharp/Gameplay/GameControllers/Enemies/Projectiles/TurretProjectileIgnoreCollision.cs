using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Environment.Traps.Turrets;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	[RequireComponent(typeof(BasicTurret))]
	public class TurretProjectileIgnoreCollision : MonoBehaviour
	{
		private void Awake()
		{
			this.turret = base.GetComponent<BasicTurret>();
			if (!this.turret)
			{
				Debug.LogError("Component requires a BasicTurret component!");
				UnityEngine.Object.Destroy(this);
			}
		}

		private void OnEnable()
		{
			if (this.turret)
			{
				BasicTurret basicTurret = this.turret;
				basicTurret.onProjectileFired = (Action<Projectile>)Delegate.Combine(basicTurret.onProjectileFired, new Action<Projectile>(this.OnProjectileFired));
			}
		}

		private void OnDisable()
		{
			if (this.turret)
			{
				BasicTurret basicTurret = this.turret;
				basicTurret.onProjectileFired = (Action<Projectile>)Delegate.Remove(basicTurret.onProjectileFired, new Action<Projectile>(this.OnProjectileFired));
			}
		}

		private void OnProjectileFired(Projectile p)
		{
			if (this.collidersToIgnore.Count == 0)
			{
				return;
			}
			Collider2D componentInChildren = p.GetComponentInChildren<Collider2D>();
			if (componentInChildren)
			{
				foreach (Collider2D collider2D in this.collidersToIgnore)
				{
					if (collider2D)
					{
						Physics2D.IgnoreCollision(collider2D, componentInChildren);
					}
				}
			}
			else
			{
				Debug.LogWarning("The projectile has no collider?");
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = this.collidersColor;
			foreach (Collider2D collider2D in this.collidersToIgnore)
			{
				if (collider2D)
				{
					Gizmos.DrawCube(base.transform.TransformPoint(collider2D.transform.localPosition + collider2D.offset), collider2D.bounds.size);
				}
			}
		}

		public Color collidersColor = new Color(0f, 1f, 1f, 0.5f);

		private BasicTurret turret;

		public List<Collider2D> collidersToIgnore = new List<Collider2D>();
	}
}
