using System;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Stunt
{
	public class ParryBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			this._owner = animator.GetComponentInParent<Enemy>();
			this._behaviour = this._owner.GetComponentInChildren<EnemyBehaviour>();
			this._currentTimeParried = 0f;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this._currentTimeParried += Time.deltaTime;
			if (this._currentTimeParried >= this.RemainTimeParried && !Core.Logic.Penitent.IsOnExecution)
			{
				this._behaviour.Alive();
			}
		}

		private Enemy _owner;

		public float RemainTimeParried = 1f;

		private float _currentTimeParried;

		private EnemyBehaviour _behaviour;
	}
}
