using System;

namespace Gameplay.GameControllers.Enemies.Framework.Attack
{
	public interface ISpawnerAttack
	{
		void CreateSpawnsHits();

		void SetSpawnsDamage(int damage);
	}
}
