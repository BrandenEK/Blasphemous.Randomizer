using System;
using Gameplay.GameControllers.Bosses.PietyMonster.Attack;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.PietyRoot
{
	public class PietyRootRiseFallBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._pietyRoot == null)
			{
				this._pietyRoot = animator.GetComponent<PietyRoot>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime <= 0.9f)
			{
				return;
			}
			if (!this._pietyRoot.gameObject.activeSelf)
			{
				return;
			}
			this._pietyRoot.Manager.StoreRoot(this._pietyRoot.gameObject);
			this._pietyRoot.gameObject.SetActive(false);
		}

		private PietyRoot _pietyRoot;
	}
}
