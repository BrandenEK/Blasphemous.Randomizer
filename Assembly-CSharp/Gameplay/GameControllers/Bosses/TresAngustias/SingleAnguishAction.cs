using System;

namespace Gameplay.GameControllers.Bosses.TresAngustias
{
	public class SingleAnguishAction
	{
		public SingleAnguishAction(Action a)
		{
			this.action = a;
		}

		public float preparationSeconds;

		public float recoverySeconds;

		public Action action;
	}
}
