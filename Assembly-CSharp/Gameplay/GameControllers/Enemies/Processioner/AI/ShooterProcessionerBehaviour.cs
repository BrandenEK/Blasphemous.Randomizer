using System;
using System.Collections;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Processioner.Animator;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Processioner.AI
{
	public class ShooterProcessionerBehaviour : ProcessionerBehaviour
	{
		public ShooterProcessioner ShooterProcessioner { get; private set; }

		public ShooterProcessionerAnimator ProcessionerAnimator { get; private set; }

		private protected Vector3 OriginPosition { protected get; private set; }

		private protected float DistanceToOrigin { protected get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.ShooterProcessioner = (ShooterProcessioner)this.Entity;
			this.ProcessionerAnimator = (ShooterProcessionerAnimator)this.ShooterProcessioner.ProcessionerAnimator;
			this.OriginPosition = new Vector2(base.transform.position.x, base.transform.position.y);
			this.ShooterProcessioner.ProjectileAttack.SetProjectileWeaponDamage((int)this.ShooterProcessioner.Stats.Strength.Final);
			this.isMovingForward = true;
			this.ResetCoolDown();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.ShooterProcessioner.Status.Dead || this.ShooterProcessioner.IsAttacking)
			{
				this.StopMovement();
				return;
			}
			this._currentShootCoolDown -= Time.deltaTime;
			if (base.TargetSeen && this._currentShootCoolDown <= 0f)
			{
				base.StartCoroutine(this.ShootCoroutine(1.5f));
				this.ResetCoolDown();
			}
			this.DistanceToOrigin = Vector2.Distance(this.OriginPosition, base.transform.position);
			if (this.DistanceToOrigin <= this.MaxDistanceReached)
			{
				this.Move(this.isMovingForward);
			}
			else
			{
				this.isMovingForward = !this.isMovingForward;
				this.OriginPosition = new Vector2(base.transform.position.x, base.transform.position.y);
			}
		}

		private IEnumerator ShootCoroutine(float delay)
		{
			this.ProcessionerAnimator.Shoot();
			yield return new WaitForSeconds(delay);
			this.ProcessionerAnimator.ChargeLoop(false);
			yield break;
		}

		private void Move(bool forward = true)
		{
			float horizontalInput;
			if (this.ShooterProcessioner.Status.Orientation == EntityOrientation.Right)
			{
				horizontalInput = ((!forward) ? -1f : 1f);
			}
			else
			{
				horizontalInput = ((!forward) ? 1f : -1f);
			}
			this.ShooterProcessioner.Input.HorizontalInput = horizontalInput;
			this.ProcessionerAnimator.WalkBackward(!forward);
		}

		public override void StopMovement()
		{
			this.ShooterProcessioner.Input.HorizontalInput = 0f;
		}

		public void ResetCoolDown()
		{
			if (this._currentShootCoolDown < this.ShootCoolDown)
			{
				this._currentShootCoolDown = this.ShootCoolDown;
			}
		}

		public float MaxDistanceReached = 5f;

		public float ShootCoolDown = 3f;

		public float _currentShootCoolDown;

		private bool isMovingForward;
	}
}
