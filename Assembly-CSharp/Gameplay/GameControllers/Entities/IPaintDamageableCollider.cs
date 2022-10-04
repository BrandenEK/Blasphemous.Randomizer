using System;

namespace Gameplay.GameControllers.Entities
{
	public interface IPaintDamageableCollider
	{
		bool IsCurrentlyDamageable();

		void AttachShowScriptIfNeeded();
	}
}
