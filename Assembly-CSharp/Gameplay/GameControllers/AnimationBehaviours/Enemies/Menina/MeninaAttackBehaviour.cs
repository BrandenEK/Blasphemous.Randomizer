using System;
using System.Collections;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Menina;
using Gameplay.GameControllers.Enemies.Menina.AI;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Menina
{
	public class MeninaAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._menina == null)
			{
				this._menina = animator.GetComponentInParent<Menina>();
			}
			this._menina.IsAttacking = true;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			this._menina.GetComponent<MeninaBehaviour>().ResetAttackCoolDown();
			this._menina.Audio.StopAttack();
			Singleton<Core>.Instance.StartCoroutine(this.UnsetEntityAttack(0.5f));
		}

		private IEnumerator UnsetEntityAttack(float delay)
		{
			yield return new WaitForSeconds(delay);
			this._menina.IsAttacking = false;
			yield break;
		}

		private Menina _menina;
	}
}
