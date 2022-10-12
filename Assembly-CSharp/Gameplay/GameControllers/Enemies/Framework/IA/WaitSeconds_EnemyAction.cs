using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class WaitSeconds_EnemyAction : EnemyAction
	{
		public EnemyAction StartAction(EnemyBehaviour e, float _minSeconds, float _maxSeconds)
		{
			this.minSeconds = _minSeconds;
			this.maxSeconds = _maxSeconds;
			return base.StartAction(e);
		}

		public EnemyAction StartAction(EnemyBehaviour e, float _seconds)
		{
			return this.StartAction(e, _seconds, _seconds);
		}

		protected override IEnumerator BaseCoroutine()
		{
			float waitSeconds = UnityEngine.Random.Range(this.minSeconds, this.maxSeconds);
			yield return new WaitForSeconds(waitSeconds);
			base.FinishAction();
			yield break;
		}

		private float minSeconds;

		private float maxSeconds;
	}
}
