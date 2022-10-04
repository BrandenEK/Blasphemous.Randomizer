using System;
using Gameplay.GameControllers.Bosses.PietyMonster;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat
{
	public class PietatDeathBreathingBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._pietyMonster == null)
			{
				this._pietyMonster = animator.GetComponentInParent<PietyMonster>();
			}
			this._pietyMonster.DamageArea.DamageAreaCollider.enabled = false;
			this._pietyMonster.BodyBarrier.gameObject.SetActive(false);
		}

		private PietyMonster _pietyMonster;
	}
}
