using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class LaunchMethodWithVector_EnemyAction : EnemyAction
	{
		public EnemyAction StartAction(EnemyBehaviour e, Action<Vector2> method, Vector2 vector)
		{
			this.method = method;
			this.vector = vector;
			return base.StartAction(e);
		}

		protected override void DoOnStart()
		{
			base.DoOnStart();
			this.method(this.vector);
		}

		protected override IEnumerator BaseCoroutine()
		{
			yield return null;
			base.FinishAction();
			yield break;
		}

		private Action<Vector2> method;

		private Vector2 vector = Vector2.zero;
	}
}
