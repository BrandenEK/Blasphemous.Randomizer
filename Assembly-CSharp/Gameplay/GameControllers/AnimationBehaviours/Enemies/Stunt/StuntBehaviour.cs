using System;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Tools.Level.Interactables;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Stunt
{
	public class StuntBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			this._execution = animator.GetComponentInParent<Execution>();
			this._enemy = this._execution.ExecutedEntity;
			this._currentStuntTime = 0f;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this._currentStuntTime += Time.deltaTime;
			if (this._currentStuntTime >= this._enemy.StuntTime)
			{
				EnemyBehaviour component = this._enemy.GetComponent<EnemyBehaviour>();
				if (component)
				{
					component.Alive();
				}
				Object.Destroy(animator.transform.parent.gameObject);
			}
		}

		private Execution _execution;

		private Enemy _enemy;

		private float _currentStuntTime;

		private const float UnAttacableTimeThreshold = 0.3f;
	}
}
