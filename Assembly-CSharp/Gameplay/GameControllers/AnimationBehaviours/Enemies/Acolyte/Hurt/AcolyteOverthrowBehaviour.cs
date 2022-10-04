using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Hurt
{
	public class AcolyteOverthrowBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				this._acolyte = animator.GetComponentInParent<Acolyte>();
			}
			this._acolyte.EnableEnemyLayer(false);
			if (this._acolyte.Status.Dead && !this._acolyte.EnemyFloorChecker().IsGrounded)
			{
				animator.Play(this._deathAnim);
			}
			else
			{
				this._acolyte.Audio.PlayOverthrow();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte.Status.Dead)
			{
				animator.Play(this._deathAnim);
			}
			if (!this._acolyte.Status.IsHurt)
			{
				this._acolyte.Status.IsHurt = true;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte.Status.IsHurt)
			{
				this._acolyte.Status.IsHurt = !this._acolyte.Status.IsHurt;
			}
		}

		private Acolyte _acolyte;

		private readonly int _deathAnim = Animator.StringToHash("Death");
	}
}
