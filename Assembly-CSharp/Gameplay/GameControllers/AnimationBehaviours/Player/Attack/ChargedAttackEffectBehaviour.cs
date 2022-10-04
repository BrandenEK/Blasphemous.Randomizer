using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class ChargedAttackEffectBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.Audio.StopLoadingChargedAttack();
			this._penitent.Audio.PlayLoadedChargedAttack();
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!this._penitent.IsAttackCharged)
			{
				this._penitent.IsAttackCharged = true;
			}
		}

		private Penitent _penitent;
	}
}
