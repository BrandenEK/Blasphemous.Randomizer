using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	internal class WaitUntilActionFinishes : CustomYieldInstruction
	{
		public WaitUntilActionFinishes(EnemyAction action)
		{
			this.action = action;
		}

		public override bool keepWaiting
		{
			get
			{
				return !this.action.Finished;
			}
		}

		private EnemyAction action;
	}
}
