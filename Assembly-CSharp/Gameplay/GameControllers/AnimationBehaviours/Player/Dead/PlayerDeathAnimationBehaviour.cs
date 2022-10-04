using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Dead
{
	public class PlayerDeathAnimationBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			if (this._penitent.IsSmashed)
			{
				this._penitent.IsSmashed = !this._penitent.IsSmashed;
				animator.Play(this._deathFallAnimHash);
			}
			else if (!this._penitent.IsImpaled)
			{
				this._penitent.Audio.PlayDeath();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this._penitent.GrabLadder.EnableClimbLadderAbility(false);
			if (stateInfo.normalizedTime >= 0.95f && this._penitent.Status.Dead)
			{
				animator.enabled = false;
			}
		}

		private Penitent _penitent;

		private readonly int _deathFallAnimHash = Animator.StringToHash("Death Fall");
	}
}
