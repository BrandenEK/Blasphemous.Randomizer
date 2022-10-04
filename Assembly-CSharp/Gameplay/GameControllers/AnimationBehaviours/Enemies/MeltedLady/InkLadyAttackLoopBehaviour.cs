using System;
using System.Collections;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.MeltedLady;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.MeltedLady
{
	public class InkLadyAttackLoopBehaviour : StateMachineBehaviour
	{
		private InkLady InkLady { get; set; }

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this.InkLady == null)
			{
				this.InkLady = animator.GetComponentInParent<InkLady>();
			}
			animator.SetBool(InkLadyAttackLoopBehaviour.Firing, true);
			Singleton<Core>.Instance.StartCoroutine(this.StopAttackCoroutine(animator));
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this.InkLady.Behaviour.TeleportCooldownLapse = 0f;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			animator.SetBool(InkLadyAttackLoopBehaviour.Firing, false);
			this.InkLady.IsAttacking = false;
		}

		private IEnumerator StopAttackCoroutine(Animator animator)
		{
			float attackTime = Mathf.Clamp(this.InkLady.BeamAttackTime - 0.55f, 0f, this.InkLady.BeamAttackTime);
			yield return new WaitForSeconds(attackTime);
			if (animator)
			{
				animator.SetBool(InkLadyAttackLoopBehaviour.Firing, false);
			}
			yield break;
		}

		private static readonly int Firing = Animator.StringToHash("FIRING");
	}
}
