using System;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	internal class WaitUntilActionCustomCallback : CustomYieldInstruction
	{
		public WaitUntilActionCustomCallback(EnemyAction action)
		{
			this.action = action;
		}

		public override bool keepWaiting
		{
			get
			{
				return !this.action.CallbackCalled;
			}
		}

		private EnemyAction action;
	}
}
