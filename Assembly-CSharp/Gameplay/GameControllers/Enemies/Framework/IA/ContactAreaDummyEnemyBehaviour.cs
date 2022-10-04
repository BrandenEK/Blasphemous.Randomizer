using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class ContactAreaDummyEnemyBehaviour : EnemyBehaviour
	{
		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}
	}
}
