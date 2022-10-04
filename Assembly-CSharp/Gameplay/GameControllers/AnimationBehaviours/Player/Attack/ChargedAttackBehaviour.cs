using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.AnimationBehaviours.Player.Attack
{
	public class ChargedAttackBehaviour : StateMachineBehaviour
	{
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent == null)
			{
				this._penitent = Core.Logic.Penitent;
			}
			this._penitent.CurrentOutputDamage = this.DamageAmount;
			this._penitent.Audio.AudioManager.StopSfx("PenitentLoadingChargedAttack", false);
			this._penitent.AnimatorInyector.RaiseAttackEvent();
			Vector2 offset = this._penitent.AttackArea.WeaponCollider.offset;
			this.defaultAttackAreaOffset = new Vector2(offset.x, offset.y);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.normalizedTime >= 0.25f && stateInfo.normalizedTime <= 0.75f)
			{
				this._penitent.PenitentAttack.IsWeaponBlowingUp = true;
			}
			else
			{
				this._penitent.PenitentAttack.IsWeaponBlowingUp = false;
			}
			this._penitent.ReleaseChargedAttack = true;
			this._penitent.PenitentAttack.IsHeavyAttacking = true;
			this._penitent.AttackArea.SetSize(this.attackAreaSize);
			this._penitent.AttackArea.SetOffset(this.attackAreaOffset);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this._penitent.IsAttackCharged)
			{
				this._penitent.IsAttackCharged = !this._penitent.IsAttackCharged;
			}
			this._penitent.ReleaseChargedAttack = false;
			this._penitent.EntityAttack.IsHeavyAttacking = false;
			this._penitent.ChargedAttack.ResizeAttackArea(false);
			this._penitent.AttackArea.SetOffset(this.defaultAttackAreaOffset);
		}

		private Penitent _penitent;

		[Tooltip("The damage amount done by the penitent in this attack")]
		public int DamageAmount;

		private readonly Vector2 attackAreaSize = new Vector2(5.25f, 3.26f);

		private readonly Vector2 attackAreaOffset = new Vector2(0.82f, 2.26f);

		private Vector2 defaultAttackAreaOffset;
	}
}
