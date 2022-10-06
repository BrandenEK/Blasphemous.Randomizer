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
			float num = (Random.value <= 0.5f) ? 30f : -30f;
			Vector3 vector;
			vector..ctor(0f, 0f, num);
			ShortcutExtensions.DOLocalRotate(animator.gameObject.transform, vector, 0.5f, 0);
		}

		public void SetDefaultRotation(Animator animator)
		{
			ShortcutExtensions.DOLocalRotate(animator.gameObject.transform, Vector3.zero, 0.5f, 0);
		}
	}
}
