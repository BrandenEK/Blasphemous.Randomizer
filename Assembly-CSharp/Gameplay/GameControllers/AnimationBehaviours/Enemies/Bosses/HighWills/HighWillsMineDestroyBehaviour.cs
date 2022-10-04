using System;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.Bosses.HighWills
{
	public class HighWillsMineDestroyBehaviour : StateMachineBehaviour
	{
		private bool ShowExplosionGO
		{
			get
			{
				return this.State == HighWillsMineDestroyBehaviour.MineState.EXPLODING;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			HighWillsMineDestroyBehaviour.MineState state = this.State;
			if (state != HighWillsMineDestroyBehaviour.MineState.EXPLODING)
			{
				if (state != HighWillsMineDestroyBehaviour.MineState.BEING_DESTROYED)
				{
				}
			}
			else
			{
				PoolManager.Instance.ReuseObject(this.ExplosionGO, animator.gameObject.transform.position, Quaternion.identity, true, 1);
			}
			animator.gameObject.SetActive(false);
		}

		[EnumToggleButtons]
		public HighWillsMineDestroyBehaviour.MineState State;

		[ShowIf("ShowExplosionGO", true)]
		public GameObject ExplosionGO;

		public enum MineState
		{
			EXPLODING,
			BEING_DESTROYED
		}
	}
}
