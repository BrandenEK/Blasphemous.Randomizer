using System;
using Gameplay.GameControllers.Enemies.JarThrower;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.JarThrower
{
	public class JarThrowerMaxHeightAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.JarThrower == null)
			{
				this.JarThrower = animator.GetComponentInParent<JarThrower>();
			}
			this.JarThrower.Controller.PlatformCharacterPhysics.VSpeed = 0f;
			this.DefaultGravityScale = this.JarThrower.Controller.PlatformCharacterPhysics.GravityScale;
			this.JarThrower.Controller.PlatformCharacterPhysics.GravityScale = 0f;
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this.JarThrower.Controller.PlatformCharacterPhysics.GravityScale = this.DefaultGravityScale;
		}

		protected JarThrower JarThrower;

		protected float DefaultGravityScale;
	}
}
