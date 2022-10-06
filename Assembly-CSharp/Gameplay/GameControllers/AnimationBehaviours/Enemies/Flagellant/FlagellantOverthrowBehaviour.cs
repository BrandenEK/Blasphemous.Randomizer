using System;
using Gameplay.GameControllers.Enemies.Flagellant;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Flagellant
{
	public class FlagellantOverthrowBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant == null)
			{
				this._flagellant = animator.GetComponentInParent<Flagellant>();
			}
			if (this._flagellant.Status.Dead)
			{
				animator.Play(this._death);
			}
			if (!this._flagellant.Status.IsHurt)
			{
				this._flagellant.Status.IsHurt = true;
			}
			if (!this._flagellant.Status.Dead)
			{
				this._flagellant.Audio.PlayOverThrow();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= 0.95f && this._flagellant.Stats.Life.Current <= 0f)
			{
				animator.speed = 0f;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._flagellant.RigidBody.bodyType != 1)
			{
				this._flagellant.RigidBody.velocity = Vector3.zero;
				this._flagellant.RigidBody.bodyType = 1;
			}
			if (this._flagellant.Status.IsHurt)
			{
				this._flagellant.Status.IsHurt = !this._flagellant.Status.IsHurt;
			}
		}

		private Flagellant _flagellant;

		private readonly int _death = Animator.StringToHash("Death");
	}
}
