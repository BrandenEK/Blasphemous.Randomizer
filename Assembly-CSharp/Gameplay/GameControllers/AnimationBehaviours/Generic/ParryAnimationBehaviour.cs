using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Generic
{
	public class ParryAnimationBehaviour : StateMachineBehaviour
	{
		private protected Entity Entity { protected get; private set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.Entity == null)
			{
				this.Entity = animator.GetComponentInParent<Entity>();
			}
			Vector3 position = this.Entity.transform.position;
			this.DefaultSpritePosition = new Vector2(position.x, position.y);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.Entity.transform.position = this.DefaultSpritePosition;
		}

		private void Wobbling()
		{
			this.Entity.transform.position = new Vector3(this.DefaultSpritePosition.x + (float)Math.Sin((double)Time.time) * this.WobblingSpeed, this.Entity.transform.position.y, this.Entity.transform.position.z);
		}

		public float WobblingSpeed;

		public float WobblingAmplitude;

		protected Vector2 DefaultSpritePosition;
	}
}
