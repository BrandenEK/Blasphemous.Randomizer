using System;

namespace Gameplay.GameControllers.Enemies.Framework.Attack
{
	public interface IPaintAttackCollider
	{
		bool IsCurrentlyDealingDamage();

		void AttachShowScriptIfNeeded();
	}
}
