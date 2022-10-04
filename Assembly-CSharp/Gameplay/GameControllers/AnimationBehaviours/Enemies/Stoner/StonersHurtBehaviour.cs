using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Stoners;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Stoner
{
	public class StonersHurtBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._stoner == null)
			{
				this._stoner = animator.GetComponentInParent<Stoners>();
			}
			if (this._stoner.Status.Orientation == this.CurrentAnimationOrientation)
			{
				return;
			}
			int properHurtAnimationClip = this.GetProperHurtAnimationClip(this._stoner.Status.Orientation);
			animator.Play(properHurtAnimationClip);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (!this._stoner.Status.Dead)
			{
				return;
			}
			string properDeathAnimationClip = StonersDeathBehaviour.GetProperDeathAnimationClip(this.CurrentAnimationOrientation);
			animator.Play(properDeathAnimationClip);
		}

		public int GetProperHurtAnimationClip(EntityOrientation currentEntityOrientation)
		{
			return (currentEntityOrientation != EntityOrientation.Left) ? this._hurtRight : this._hurtLeft;
		}

		private Stoners _stoner;

		public EntityOrientation CurrentAnimationOrientation;

		private readonly int _hurtLeft = Animator.StringToHash("HurtLeft");

		private readonly int _hurtRight = Animator.StringToHash("HurtRight");
	}
}
