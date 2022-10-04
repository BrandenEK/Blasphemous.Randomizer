using System;
using System.Diagnostics;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.BellGhost;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Projectiles
{
	public class ParriableProjectile : StraightProjectile
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ParriableProjectile> OnProjectileParried;

		protected override void OnStart()
		{
			this.projectileWeapon = base.GetComponent<ProjectileWeapon>();
			if (this.spawnOnCollision != null)
			{
				PoolManager.Instance.CreatePool(this.spawnOnCollision, 1);
			}
			this.projectileWeapon.OnProjectileDeath += this.ProjectileWeapon_OnProjectileDeath;
			base.OnStart();
		}

		private void ProjectileWeapon_OnProjectileDeath(ProjectileWeapon obj)
		{
			this.ghostTrail.EnableGhostTrail = false;
			if (this.spawnOnCollision != null && !this.parried)
			{
				PoolManager.Instance.ReuseObject(this.spawnOnCollision, base.transform.position, Quaternion.identity, false, 1);
			}
		}

		public override void Init(Vector3 origin, Vector3 target, float speed)
		{
			if (this.ghostTrail == null)
			{
				this.ghostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			}
			this.ghostTrail.EnableGhostTrail = true;
			this.PlayFlying();
			this.parried = false;
			base.Init(origin, target, speed);
		}

		public override void Init(Vector3 direction, float speed)
		{
			if (this.ghostTrail == null)
			{
				this.ghostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			}
			this.ghostTrail.EnableGhostTrail = true;
			base.Init(direction, speed);
		}

		private void PlayParry()
		{
			this.animator.SetTrigger(this.parryAnimationTrigger);
		}

		private void PlayFlying()
		{
			this.animator.Play("FLY", 0);
			this.animator.Play("FLY", 1);
		}

		private void OnParryAnimation()
		{
			this.PlayParry();
			this.velocity = Vector2.zero;
			this.ghostTrail.EnableGhostTrail = false;
		}

		public void OnParry()
		{
			this.parried = true;
			this.OnParryAnimation();
			this.ShakeWave();
			if (this.OnProjectileParried != null)
			{
				this.OnProjectileParried(this);
			}
		}

		public void OnDeathAnimation()
		{
			this.projectileWeapon.ForceDestroy();
		}

		public void ShakeWave()
		{
			Core.Logic.ScreenFreeze.Freeze(0.05f, 0.2f, 0f, null);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.5f, 0.3f, 2f);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.75f, Vector3.down * 1f, 12, 0.2f, 0.01f, default(Vector3), 0.01f, true);
		}

		[SerializeField]
		private string parryAnimationTrigger;

		private ProjectileWeapon projectileWeapon;

		public Animator animator;

		private GhostTrailGenerator ghostTrail;

		public GameObject flipParent;

		public GameObject spawnOnCollision;

		private bool parried;
	}
}
