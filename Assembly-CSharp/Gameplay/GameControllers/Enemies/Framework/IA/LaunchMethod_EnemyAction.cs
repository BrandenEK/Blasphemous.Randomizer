using System;
using System.Collections;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class LaunchMethod_EnemyAction : EnemyAction
	{
		public EnemyAction StartAction(EnemyBehaviour e, Action _method)
		{
			this.method = _method;
			return base.StartAction(e);
		}

		protected override void DoOnStart()
		{
			base.DoOnStart();
			this.method();
		}

		protected override IEnumerator BaseCoroutine()
		{
			yield return null;
			base.FinishAction();
			yield break;
		}

		private Action method;
	}
}
