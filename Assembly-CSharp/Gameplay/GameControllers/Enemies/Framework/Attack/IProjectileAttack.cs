using System;
using Gameplay.GameControllers.Enemies.Projectiles;

namespace Gameplay.GameControllers.Enemies.Framework.Attack
{
	public interface IProjectileAttack
	{
		void SetProjectileWeaponDamage(int damage);

		void SetProjectileWeaponDamage(Projectile projectile, int damage);
	}
}
