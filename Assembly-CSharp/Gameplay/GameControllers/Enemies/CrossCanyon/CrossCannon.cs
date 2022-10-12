using System;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.GameControllers.Enemies.CrossCanyon
{
	public class CrossCannon : MonoBehaviour
	{
		private protected Entity Owner { protected get; private set; }

		private void Start()
		{
			this._currentShotInterval = this.ShotInterval;
			this.Owner = base.GetComponentInParent<Entity>();
			this.SetCannonsDamage();
			if (this.Owner != null)
			{
				return;
			}
			Debug.LogError("This component requires an entity component.");
			base.enabled = false;
			this.Owner.OnDeath += this.OnDeath;
		}

		private void Update()
		{
			this._currentShotInterval -= Time.deltaTime;
			if (this._currentShotInterval > 0f || this.Owner.Status.Dead)
			{
				return;
			}
			this._currentShotInterval = this.ShotInterval;
			this.ShootCannons();
		}

		private void Rotate()
		{
			Quaternion rhs = Quaternion.Euler(0f, 0f, -45f);
			base.transform.rotation *= rhs;
		}

		private void ShootCannons()
		{
			foreach (CrossCannon.Cannon cannon in this.Cannons)
			{
				Vector2 shotDirection = this.GetShotDirection(cannon.Direction);
				cannon.ProjectileAttack.Shoot(shotDirection);
			}
			this.Rotate();
		}

		private void SetCannonsDamage()
		{
			IProjectileAttack[] componentsInChildren = base.GetComponentsInChildren<IProjectileAttack>();
			foreach (IProjectileAttack projectileAttack in componentsInChildren)
			{
				projectileAttack.SetProjectileWeaponDamage((int)this.Owner.Stats.Strength.Final);
			}
		}

		private Vector2 GetShotDirection(CrossCannon.Direction dir)
		{
			Vector2 result = Vector2.right;
			switch (dir)
			{
			case CrossCannon.Direction.Up:
				result = base.transform.up;
				break;
			case CrossCannon.Direction.Down:
				result = -base.transform.up;
				break;
			case CrossCannon.Direction.Left:
				result = -base.transform.right;
				break;
			case CrossCannon.Direction.Right:
				result = base.transform.right;
				break;
			}
			return result;
		}

		private void OnDeath()
		{
			this.Owner.OnDeath -= this.OnDeath;
			this._currentShotInterval = this.ShotInterval;
			base.enabled = false;
		}

		public float ShotInterval;

		private float _currentShotInterval;

		[FormerlySerializedAs("Canyons")]
		public CrossCannon.Cannon[] Cannons;

		public enum Direction
		{
			Up,
			Down,
			Left,
			Right
		}

		[Serializable]
		public struct Cannon
		{
			public CrossCannon.Direction Direction;

			public BossStraightProjectileAttack ProjectileAttack;
		}
	}
}
