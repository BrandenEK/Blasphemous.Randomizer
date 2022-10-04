using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Sparks;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Effects.Player
{
	public class SparkBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._swordSparkSpawner == null)
			{
				this._swordSparkSpawner = Core.Logic.Penitent.SwordSparkSpawner;
			}
			if (this._swordSpark == null)
			{
				this._swordSpark = animator.GetComponent<SwordSpark>();
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._swordSparkSpawner == null)
			{
				this._swordSparkSpawner = Core.Logic.Penitent.SwordSparkSpawner;
			}
			if (this._swordSpark != null)
			{
				this._swordSparkSpawner.StoreSwordSpark(this._swordSpark);
			}
		}

		private SwordSpark _swordSpark;

		private SwordSparkSpawner _swordSparkSpawner;
	}
}
