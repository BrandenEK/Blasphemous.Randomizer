using System;
using Gameplay.GameControllers.Entities.Animations;

namespace Gameplay.GameControllers.Bosses.PietyMonster.Animation
{
	public class PietyMonsterAnimatorInyector : EnemyAnimatorInyector
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._pietyMonster = (PietyMonster)this.OwnerEntity;
		}

		public void Idle()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("TURN_AROUND", false);
			this.ResetAttacks();
		}

		public void Walk()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", true);
			base.EntityAnimator.SetBool("TURN_AROUND", false);
			base.EntityAnimator.ResetTrigger("STOMP_ATTACK");
		}

		public void Stop()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
		}

		public void CanMove(int allow)
		{
			if (this._pietyMonster == null)
			{
				return;
			}
			this._pietyMonster.CanMove = (allow > 0);
		}

		public void AvoidBarrierCollision(int allow)
		{
			if (this._pietyMonster == null)
			{
				return;
			}
			if (allow > 0)
			{
				this._pietyMonster.BodyBarrier.EnableCollider();
			}
			else
			{
				this._pietyMonster.BodyBarrier.DisableCollider();
			}
		}

		public void StompAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("TURN_AROUND", false);
			base.EntityAnimator.SetTrigger("STOMP_ATTACK");
		}

		public void ClawAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("TURN_AROUND", false);
			base.EntityAnimator.SetTrigger("SLASH_ATTACK");
		}

		public void AreaAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("TURN_AROUND", false);
			base.EntityAnimator.SetTrigger("SMASH_ATTACK");
		}

		public void SpitAttack()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			base.EntityAnimator.SetBool("WALK", false);
			base.EntityAnimator.SetBool("TURN_AROUND", false);
			base.EntityAnimator.SetTrigger("SPIT_ATTACK");
			base.EntityAnimator.SetBool("SPITING", true);
		}

		public void StopSpiting()
		{
			base.EntityAnimator.ResetTrigger("SPIT_ATTACK");
			base.EntityAnimator.SetBool("SPITING", false);
		}

		public void ResetAttacks()
		{
			base.EntityAnimator.ResetTrigger("SPIT_ATTACK");
			base.EntityAnimator.SetBool("SPITING", false);
			base.EntityAnimator.ResetTrigger("SMASH_ATTACK");
			base.EntityAnimator.ResetTrigger("STOMP_ATTACK");
			base.EntityAnimator.ResetTrigger("SLASH_ATTACK");
		}

		public void TurnAround()
		{
			if (base.EntityAnimator == null)
			{
				return;
			}
			if (!this._pietyMonster.EnemyBehaviour.TurningAround)
			{
				base.EntityAnimator.SetBool("WALK", false);
				base.EntityAnimator.SetBool("TURN_AROUND", true);
			}
		}

		public void Death()
		{
			this.ResetAttacks();
			base.EntityAnimator.SetTrigger("DEATH");
		}

		public void StopTurning()
		{
			base.EntityAnimator.SetBool("TURN_AROUND", false);
		}

		private PietyMonster _pietyMonster;
	}
}
