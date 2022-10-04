using System;
using Gameplay.GameControllers.Bosses.BurntFace.Rosary;
using Gameplay.GameControllers.Enemies.Framework.Attack;

namespace Gameplay.GameControllers.Enemies.MeltedLady.Attack
{
	public class FloatingLadyAttack : EnemyAttack
	{
		protected override void OnStart()
		{
			base.OnStart();
			if (!this.BeamAttack)
			{
				return;
			}
			this.BeamAttack.SetDamage((int)base.EntityOwner.Stats.Strength.Final);
		}

		public BurntFaceBeamAttack BeamAttack;
	}
}
