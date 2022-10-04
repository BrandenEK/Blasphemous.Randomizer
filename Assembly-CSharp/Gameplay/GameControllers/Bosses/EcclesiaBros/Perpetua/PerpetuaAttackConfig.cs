using System;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua
{
	[Serializable]
	public struct PerpetuaAttackConfig
	{
		public PerpetuaBehaviour.Perpetua_ATTACKS attack;

		public float anticipationWait;

		public float recoveryWait;

		public int repetitions;

		public float waitBetweenRepetitions;
	}
}
