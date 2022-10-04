using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class DebugText_EnemyAction : EnemyAction
	{
		public EnemyAction StartAction(EnemyBehaviour e, string _text)
		{
			this.text = _text;
			return base.StartAction(e);
		}

		protected override void DoOnStart()
		{
			base.DoOnStart();
			Debug.Log("<color=blue>DebugText_EA:" + this.text + "</color>");
		}

		protected override IEnumerator BaseCoroutine()
		{
			yield return null;
			base.FinishAction();
			yield break;
		}

		private string text;
	}
}
