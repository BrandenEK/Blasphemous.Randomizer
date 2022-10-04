using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Enemies.Processioner.AI;
using Gameplay.GameControllers.Enemies.Processioner.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Processioner.Animator
{
	public class ShooterProcessionerAnimator : ProcessionerAnimator
	{
		protected ShooterProcessioner Processioner { get; set; }

		protected ProcesionerAudio Audio { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Processioner = (ShooterProcessioner)this.OwnerEntity;
			this.Audio = this.Processioner.GetComponentInChildren<ProcesionerAudio>();
			this.SetProjectileShootPosition();
		}

		public void WalkBackward(bool backward)
		{
			base.EntityAnimator.SetBool("BACK", backward);
		}

		public void Shoot()
		{
			base.EntityAnimator.SetTrigger("SHOOT");
			this.ChargeLoop(true);
		}

		public void ChargeLoop(bool charge = true)
		{
			this.Audio.StartChargeLoop();
			base.EntityAnimator.SetBool("CHARGING", charge);
		}

		public void SetAttacking(int attacking)
		{
			this.Processioner.IsAttacking = (attacking > 0);
		}

		public void Death()
		{
			base.EntityAnimator.ResetTrigger("SHOOT");
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void Dispose()
		{
			this.Audio.StopChargeLoop();
			this.Processioner.gameObject.SetActive(false);
		}

		public void LaunchProjectile()
		{
			if (this.Processioner.Target == null)
			{
				return;
			}
			Transform transform = this.Processioner.Target.transform;
			if (this.Processioner.Status.Dead)
			{
				return;
			}
			Vector3 normalized = (transform.position - this.Processioner.transform.position).normalized;
			this.Processioner.ProjectileAttack.Shoot(normalized);
			this.Audio.StopChargeLoop();
			ShooterProcessionerBehaviour shooterProcessionerBehaviour = this.Processioner.Behaviour as ShooterProcessionerBehaviour;
			if (shooterProcessionerBehaviour)
			{
				shooterProcessionerBehaviour.ResetCoolDown();
			}
		}

		private void SetProjectileShootPosition()
		{
			if (this.Processioner.Status.Orientation != EntityOrientation.Left)
			{
				return;
			}
			Vector3 localScale = this.Processioner.ProjectileAttack.gameObject.transform.localScale;
			localScale.x *= -1f;
			this.Processioner.ProjectileAttack.gameObject.transform.localScale = localScale;
		}

		private void OnDestroy()
		{
			if (this.Audio)
			{
				this.Audio.StopChargeLoop();
			}
		}
	}
}
