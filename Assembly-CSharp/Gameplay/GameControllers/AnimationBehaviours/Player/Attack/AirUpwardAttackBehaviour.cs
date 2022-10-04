using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class AirUpwardAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateEnter(animator, stateInfo, layerIndex);
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
				this._defaultAttackAreaOffset = new Vector2(this._penitent.AttackArea.WeaponCollider.offset.x, this._penitent.AttackArea.WeaponCollider.offset.y);
				this._defaultAttackAreaSize = new Vector2(this._penitent.AttackArea.WeaponCollider.bounds.size.x, this._penitent.AttackArea.WeaponCollider.bounds.size.y);
			}
			animator.SetBool("AIR_ATTACKING", true);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateUpdate(animator, stateInfo, layerIndex);
			this._penitent.AttackArea.SetOffset(this.AttackAreaOffset);
			this._penitent.AttackArea.SetSize(this.AttackAreaSize);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			base.OnStateExit(animator, stateInfo, layerIndex);
			animator.SetBool("AIR_ATTACKING", false);
			this._penitent.AttackArea.SetOffset(this._defaultAttackAreaOffset);
			this._penitent.AttackArea.SetSize(this._defaultAttackAreaSize);
		}

		private Penitent _penitent;

		public Vector2 AttackAreaOffset;

		public Vector2 AttackAreaSize;

		private Vector2 _defaultAttackAreaOffset;

		private Vector2 _defaultAttackAreaSize;
	}
}
