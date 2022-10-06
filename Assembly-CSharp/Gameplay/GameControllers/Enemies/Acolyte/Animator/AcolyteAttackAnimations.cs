using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.Animations;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Acolyte.Animator
{
	public class AcolyteAttackAnimations : AttackAnimationsEvents
	{
		public ColorFlash ColorFlash { get; private set; }

		public bool ReboundAttack { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this._acolyte = base.GetComponentInParent<Acolyte>();
			if (this._acolyte != null)
			{
				this._acolyteMotionLerper = this._acolyte.MotionLerper;
			}
			this.ReboundAttack = false;
			SpawnManager.OnPlayerSpawn += this.OnPenitentReady;
			if (this._acolyte != null)
			{
				this._acolyteAttack = this._acolyte.GetComponentInChildren<AcolyteAttack>();
			}
			this.ColorFlash = base.GetComponentInChildren<ColorFlash>();
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPenitentReady;
		}

		private void OnPenitentReady(Penitent penitent)
		{
			this._penitent = penitent;
		}

		public override void WeaponBlowUp(float weaponBlowUp)
		{
		}

		public void GetCurrentPlayerPosition()
		{
		}

		public void EnableAttackFlag()
		{
			this._acolyte.IsAttacking = true;
			this._acolyte.Behaviour.IsAttackWindowOpen = true;
		}

		public void DisableAttackFlag()
		{
			this._acolyte.IsAttacking = false;
			this._acolyte.Behaviour.IsAttackWindowOpen = false;
		}

		public override void CurrentWeaponAttack(DamageArea.DamageType damageType)
		{
			if (this._acolyteAttack == null)
			{
				return;
			}
			this._acolyteAttack.CurrentWeaponAttack(damageType);
		}

		public void StopAttackDisplacement()
		{
			if (this._acolyte.RigidbodyType != null)
			{
				return;
			}
			this._acolyte.Rigidbody.velocity = Vector2.zero;
			this._acolyte.RigidbodyType = 1;
		}

		public void AttackDisplacement()
		{
			if (this._acolyteMotionLerper == null || this.ReboundAttack || this._acolyteMotionLerper.IsLerping)
			{
				return;
			}
			this.ReboundAttack = false;
			float num = (this._acolyte.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this._acolyteMotionLerper.StartLerping(this._acolyte.transform.right * (num * this.attackDisplacement));
		}

		private bool AttackAreaOverlapsPlayer(EntityOrientation playerOrientation)
		{
			Vector2 vector = this._penitent.DamageArea.DamageAreaCollider.bounds.center;
			return this._acolyte.AttackArea.WeaponCollider.OverlapPoint(vector);
		}

		public override void Rebound()
		{
			if (!this.AttackAreaOverlapsPlayer(this._penitent.Status.Orientation) || !this._penitent.HasFlag("SIDE_BLOCKED") || this._acolyte.Status.Orientation == this._penitent.Status.Orientation || this.ReboundAttack || !this._acolyte.IsAttacking)
			{
				return;
			}
			this._acolyteMotionLerper.StopLerping();
			this._acolyteMotionLerper.distanceToMove *= -1f;
			Vector3 forwardTangent = this._acolyte.GetForwardTangent(this._acolyte.transform.right, this._acolyte.EnemyFloorChecker().EnemyFloorCollisionNormal);
			this.ReboundAttack = true;
			this._acolyteMotionLerper.StartLerping(forwardTangent);
		}

		public void PlayChargeAttack()
		{
			if (this._acolyte.IsVisible())
			{
				this._acolyte.Audio.PlayChargeAttack();
			}
		}

		public void PlayReleaseChargeAttack()
		{
			if (this._acolyte.IsVisible())
			{
				this._acolyte.Audio.PlayReleaseAttack();
			}
		}

		public void PlayOverhrow()
		{
			if (this._acolyte.IsVisible())
			{
				this._acolyte.Audio.PlayOverthrow();
			}
		}

		public void PlayDeathOnCliffLede()
		{
			if (this._acolyte.IsVisible())
			{
				this._acolyte.Audio.PlayDeathOnCliffLede();
			}
		}

		public void PlayDeath()
		{
			if (this._acolyte.IsVisible())
			{
				this._acolyte.Audio.PlayDeath();
			}
		}

		public void PlayVaporizationDeath()
		{
			if (this._acolyte.IsVisible())
			{
				this._acolyte.Audio.PlayVaporizationDeath();
			}
		}

		private Acolyte _acolyte;

		private AcolyteAttack _acolyteAttack;

		private MotionLerper _acolyteMotionLerper;

		private Penitent _penitent;

		[Tooltip("The displacement distance when acolyte attacks")]
		public float attackDisplacement;

		[Tooltip("The speed of the displacement (lerping time)")]
		public float displacementSpeed;
	}
}
