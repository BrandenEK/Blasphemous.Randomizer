using System;
using DG.Tweening;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.BejeweledBoss
{
	public class BsBasicAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
		}

		public void RotateArm(EntityOrientation orientation, Animator animator)
		{
			float z = (UnityEngine.Random.value <= 0.5f) ? 30f : -30f;
			Vector3 endValue = new Vector3(0f, 0f, z);
			animator.gameObject.transform.DOLocalRotate(endValue, 0.5f, RotateMode.Fast);
		}

		public void SetDefaultRotation(Animator animator)
		{
			animator.gameObject.transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast);
		}
	}
}
