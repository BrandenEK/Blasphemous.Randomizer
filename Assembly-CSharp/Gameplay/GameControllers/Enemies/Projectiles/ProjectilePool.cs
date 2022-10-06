using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class ProjectilePool : MonoBehaviour
	{
		public Projectile Spawn(Vector3 position, Entity owner)
		{
			Projectile projectile;
			if (this.instantiatedProjectiles.Count > 0)
			{
				projectile = this.instantiatedProjectiles[this.instantiatedProjectiles.Count - 1];
				this.instantiatedProjectiles.Remove(projectile);
				projectile.gameObject.SetActive(true);
				projectile.transform.position = position;
				projectile.transform.rotation = Quaternion.identity;
			}
			else
			{
				projectile = Object.Instantiate<Projectile>(this.projectilePrefab, position, Quaternion.identity);
			}
			projectile.owner = owner;
			return projectile;
		}

		public void StoreProjectile(Projectile p)
		{
			if (this.instantiatedProjectiles.Contains(p))
			{
				return;
			}
			this.instantiatedProjectiles.Add(p);
			p.gameObject.SetActive(false);
		}

		public void Initialize(int n)
		{
			for (int i = 0; i < n; i++)
			{
				Projectile p = Object.Instantiate<Projectile>(this.projectilePrefab, base.transform.position, Quaternion.identity);
				this.StoreProjectile(p);
			}
		}

		private void OnDestroy()
		{
			if (this.instantiatedProjectiles.Count > 0)
			{
				this.instantiatedProjectiles.Clear();
			}
		}

		public Enemy enemyOwner;

		public Projectile projectilePrefab;

		private readonly List<Projectile> instantiatedProjectiles = new List<Projectile>();
	}
}
