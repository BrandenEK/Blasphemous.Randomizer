using System;

namespace Gameplay.GameControllers.Enemies.GoldenCorpse.AI
{
	public interface ICorpseBehaviour
	{
		void Awaken();

		void Sleep();

		void SleepForever();
	}
}
