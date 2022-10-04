using System;

namespace Gameplay.GameControllers.Enemies.Framework.Attack
{
	public interface IDirectAttack
	{
		void CreateHit();

		void SetDamage(int damage);
	}
}
