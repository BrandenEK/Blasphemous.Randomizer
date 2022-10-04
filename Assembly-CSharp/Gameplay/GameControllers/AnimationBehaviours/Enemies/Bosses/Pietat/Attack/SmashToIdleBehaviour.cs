using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Bosses.PietyMonster;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.Pietat.Attack
{
	public class SmashToIdleBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._pietyMonster == null)
			{
				this._pietyMonster = animator.GetComponentInParent<PietyMonster>();
			}
			this._pietyMonster.BodyBarrier.EnableCollider();
			this._target = this._pietyMonster.PietyRootsManager.Target;
			Vector3 position = this._target.transform.position;
			if (this._pietyMonster.transform.position.x >= position.x && this._pietyMonster.Status.Orientation == EntityOrientation.Right)
			{
				animator.Play(this._smashToIdle);
			}
			if (this._pietyMonster.transform.position.x < position.x && this._pietyMonster.Status.Orientation == EntityOrientation.Left)
			{
				this._pietyMonster.SetOrientation(EntityOrientation.Right, true, false);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			if (this._pietyMonster.PietyBehaviour.ReadyToAttack)
			{
				this._pietyMonster.PietyBehaviour.ReadyToAttack = false;
			}
		}

		private PietyMonster _pietyMonster;

		private GameObject _target;

		private readonly int _smashToIdle = Animator.StringToHash("SmashToIdleReverse");
	}
}
