using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Animations;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Runner.Animator
{
	public class RunnerAnimatorInjector : EnemyAnimatorInyector
	{
		public Runner Runner { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this.Runner = (Runner)this.OwnerEntity;
		}

		public void Run(bool run)
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetBool(RunnerAnimatorInjector.Run1, run);
		}

		public void Scream()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.Play("Scream");
		}

		public void TurnAround()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(RunnerAnimatorInjector.Around);
			base.EntityAnimator.SetBool(RunnerAnimatorInjector.Turning, true);
		}

		public void Death()
		{
			if (!base.EntityAnimator)
			{
				return;
			}
			base.EntityAnimator.SetTrigger(RunnerAnimatorInjector.Death1);
			this.SetPlayerOrientation();
		}

		public void Dispose()
		{
			this.OwnerEntity.gameObject.SetActive(false);
		}

		private void SetPlayerOrientation()
		{
			Vector3 position = Core.Logic.Penitent.transform.position;
			if (this.Runner.Status.Orientation == EntityOrientation.Right)
			{
				if (position.x <= this.OwnerEntity.transform.position.x)
				{
					this.OwnerEntity.SetOrientation(EntityOrientation.Left, true, false);
				}
			}
			else if (position.x > this.OwnerEntity.transform.position.x)
			{
				this.OwnerEntity.SetOrientation(EntityOrientation.Right, true, false);
			}
		}

		private static readonly int Around = Animator.StringToHash("TURN_AROUND");

		private static readonly int Turning = Animator.StringToHash("TURNING");

		private static readonly int Death1 = Animator.StringToHash("DEATH");

		private static readonly int Run1 = Animator.StringToHash("RUN");
	}
}
