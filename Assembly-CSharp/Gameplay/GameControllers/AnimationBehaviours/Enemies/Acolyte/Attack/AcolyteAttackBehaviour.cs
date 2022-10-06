using System;
using Gameplay.GameControllers.Enemies.Acolyte;
using Gameplay.GameControllers.Enemies.Acolyte.IA;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Acolyte.Attack
{
	public class AcolyteAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte == null)
			{
				this._acolyte = animator.GetComponentInParent<Acolyte>();
			}
			if (this._behaviour == null)
			{
				this._behaviour = (AcolyteBehaviour)this._acolyte.EnemyBehaviour;
			}
			if (this._attack == null)
			{
				this._attack = (AcolyteAttack)this._acolyte.EnemyAttack();
			}
			if (animator.speed > 1f)
			{
				animator.speed = 1f;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			if (stateInfo.normalizedTime < 0.1f)
			{
				this._attack.TriggerAttack = false;
			}
			this._acolyte.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._acolyte.RigidbodyType != 1)
			{
				this._acolyte.Rigidbody.velocity = Vector2.zero;
				this._acolyte.RigidbodyType = 1;
			}
			this._behaviour.IsAttackWindowOpen = false;
			this._acolyte.IsAttacking = false;
		}

		private Acolyte _acolyte;

		private AcolyteAttack _attack;

		[Range(0f, 1f)]
		public float desiredPlaybackTimeOnRunning = 0.4f;

		private AcolyteBehaviour _behaviour;
	}
}
