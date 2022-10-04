using System;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.HighWills.Attack;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk.Attack
{
	public class PontiffHuskRangedVariantAttack : EnemyAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.MinePrefab, 6);
		}

		public RangedMine Shoot()
		{
			Vector3 position = (!base.EntityOwner.SpriteRenderer.flipX) ? this.LeftShootingPoint.transform.position : this.RightShootingPoint.transform.position;
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.MinePrefab, position, Quaternion.Euler(0f, 0f, -90f), false, 1);
			return objectInstance.GameObject.GetComponent<RangedMine>();
		}

		public GameObject MinePrefab;

		public GameObject LeftShootingPoint;

		public GameObject RightShootingPoint;
	}
}
