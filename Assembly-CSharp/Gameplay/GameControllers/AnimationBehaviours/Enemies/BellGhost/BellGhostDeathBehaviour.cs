using System;
using Gameplay.GameControllers.Enemies.BellGhost;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Enemies.BellGhost
{
	public class BellGhostDeathBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._bellGhost == null)
			{
				this._bellGhost = animator.GetComponentInParent<BellGhost>();
			}
			this._destroy = false;
			if (this._bellGhost.AttackArea != null)
			{
				this._bellGhost.AttackArea.WeaponCollider.enabled = false;
			}
			this._bellGhost.EntityDamageArea.DamageAreaCollider.enabled = false;
			if (this._bellGhost.MotionLerper.IsLerping)
			{
				this._bellGhost.MotionLerper.StopLerping();
			}
			this._bellGhost.Audio.StopFloating();
			this._bellGhost.Audio.StopChargeAttack();
			this._bellGhost.Audio.StopAttack(true);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= 0.95f && !this._destroy)
			{
				this._destroy = true;
				UnityEngine.Object.Destroy(this._bellGhost.gameObject);
			}
		}

		private BellGhost _bellGhost;

		private bool _destroy;
	}
}
