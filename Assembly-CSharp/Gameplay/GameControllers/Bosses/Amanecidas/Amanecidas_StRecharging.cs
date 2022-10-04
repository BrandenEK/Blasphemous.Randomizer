using System;
using Maikel.StatelessFSM;

namespace Gameplay.GameControllers.Bosses.Amanecidas
{
	public class Amanecidas_StRecharging : State<AmanecidasBehaviour>
	{
		public override void Enter(AmanecidasBehaviour owner)
		{
			owner.InitShieldRechargeDamage();
		}

		public override void Execute(AmanecidasBehaviour owner)
		{
			owner.CheckShieldRechargeDamage();
		}

		public override void Exit(AmanecidasBehaviour owner)
		{
		}
	}
}
