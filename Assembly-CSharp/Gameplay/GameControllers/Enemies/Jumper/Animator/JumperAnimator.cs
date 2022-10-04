using System;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Jumper.Animator
{
	public class JumperAnimator : EnemyAnimatorInyector
	{
		protected Jumper Jumper { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Jumper = (Jumper)this.OwnerEntity;
		}

		public void Death()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetTrigger("DEATH");
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("JumpAscending"))
			{
				this._currentTimeAscending += Time.deltaTime;
				if (this._currentTimeAscending > 0.1f && this.Jumper.Controller.PlatformCharacterPhysics.VSpeed <= -0.1f)
				{
					this._currentTimeAscending = 0f;
					base.EntityAnimator.Play("JumpMax");
				}
			}
			if (base.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("JumpDescending") && this.Jumper.Controller.IsGrounded)
			{
				base.EntityAnimator.Play("Landing");
			}
		}

		private float _currentTimeAscending;
	}
}
