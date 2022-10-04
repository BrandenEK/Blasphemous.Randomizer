using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Flagellant.Attack;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Flagellant.Animator
{
	public class FlagellantAttackAnimations : AttackAnimationsEvents
	{
		private void Awake()
		{
			this._flagellantAttack = base.transform.parent.GetComponentInChildren<FlagellantAttack>();
			this._flagellantMotionLerper = base.GetComponentInParent<MotionLerper>();
			this._flagellant = base.GetComponentInParent<Flagellant>();
		}

		public void MoveFlagellant(float distance)
		{
			if (this._flagellantAttack == null)
			{
				return;
			}
			if (this._flagellant.Status.IsOnCliffLede || this._flagellant.EnemyBehaviour.IsTrapDetected)
			{
				return;
			}
			EntityOrientation orientation = this._flagellant.Status.Orientation;
			distance = ((orientation != EntityOrientation.Left) ? distance : (-distance));
			this._flagellantMotionLerper.distanceToMove = distance;
			this._flagellantMotionLerper.TimeTakenDuringLerp = this.TimeTakenDuringLerp;
			Vector3 forwardTangent = this._flagellant.GetForwardTangent(this._flagellant.transform.right, this._flagellant.EnemyFloorChecker().EnemyFloorCollisionNormal);
			if (!this._flagellantMotionLerper.IsLerping)
			{
				this._flagellantMotionLerper.StartLerping(forwardTangent);
			}
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			if (this._flagellantAttack == null)
			{
				return;
			}
			this._flagellantAttack.CurrentWeaponAttack(damageType);
		}

		public override void WeaponBlowUp(float weaponBlowUp)
		{
			weaponBlowUp = Mathf.Clamp01(weaponBlowUp);
			if (this._flagellantAttack != null)
			{
				this._flagellantAttack.IsWeaponBlowingUp = (weaponBlowUp > 0f);
			}
		}

		public void PlayBasicAttack()
		{
			if (this._flagellant.Status.IsVisibleOnCamera)
			{
				this._flagellant.Audio.PlayBasicAttack();
			}
		}

		public void PlaySlashHit()
		{
			this._flagellant.Audio.PlayAttackHit();
		}

		public void PlaySelfLash()
		{
			if (this._flagellant.Status.IsVisibleOnCamera)
			{
				this._flagellant.Audio.PlaySelfLash();
			}
		}

		public void PlayDeath()
		{
			this._flagellant.Audio.PlayDeath();
		}

		public void PlayVaporizationDeath()
		{
			if (this._flagellant.Status.IsVisibleOnCamera)
			{
				this._flagellant.Audio.PlayVaporizationDeath();
			}
		}

		private Flagellant _flagellant;

		private FlagellantAttack _flagellantAttack;

		private MotionLerper _flagellantMotionLerper;

		[SerializeField]
		[Range(0.01f, 10f)]
		protected float TimeTakenDuringLerp;
	}
}
