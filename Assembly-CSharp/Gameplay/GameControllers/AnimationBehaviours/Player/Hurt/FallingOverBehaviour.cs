using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Hurt
{
	public class FallingOverBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._throwBack = this._penitent.GetComponentInChildren<ThrowBack>();
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (this._penitent.RigidBody.isKinematic)
			{
				return;
			}
			if (this._throwBack.IsOwnerFalling)
			{
				animator.Play(this._throwbackTransitionAnim);
			}
		}

		private Penitent _penitent;

		private readonly int _throwbackTransitionAnim = Animator.StringToHash("ThrowbackTrans");

		private ThrowBack _throwBack;
	}
}
