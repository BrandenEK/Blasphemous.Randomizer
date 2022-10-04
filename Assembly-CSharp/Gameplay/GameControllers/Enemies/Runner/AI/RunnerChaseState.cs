using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Runner.AI
{
	public class RunnerChaseState : State
	{
		protected Runner Runner { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Runner = machine.GetComponent<Runner>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Runner.Behaviour.LookAtTarget(this.Runner.Target.transform.position);
			this.CurrentChasingImpasse = this.Runner.Behaviour.ChasingImpasse;
		}

		public override void Update()
		{
			base.Update();
			if (this.Runner.Behaviour.TurningAround || this.Runner.Behaviour.IsScreaming)
			{
				this.Runner.Behaviour.StopMovement();
				return;
			}
			if (this.Runner.Behaviour.CanChase)
			{
				if (this.IsTargetBehind)
				{
					this.CurrentChasingImpasse -= Time.deltaTime;
					if (this.CurrentChasingImpasse < 0f)
					{
						this.Runner.Behaviour.StopMovement();
						this.Runner.Behaviour.LookAtTarget(this.Runner.Target.transform.position);
						this.CurrentChasingImpasse = this.Runner.Behaviour.ChasingImpasse;
					}
					else
					{
						this.Runner.Behaviour.Chase(this.Runner.Target.transform);
					}
				}
				else
				{
					this.Runner.Behaviour.Chase(this.Runner.Target.transform);
				}
			}
			else
			{
				this.Runner.Behaviour.StopMovement();
				this.Runner.Behaviour.LookAtTarget(this.Runner.Target.transform.position);
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private bool IsTargetBehind
		{
			get
			{
				bool result = false;
				Vector3 position = this.Runner.Target.transform.position;
				EntityOrientation orientation = this.Runner.Status.Orientation;
				if (this.Runner.transform.position.x > position.x && orientation == EntityOrientation.Right)
				{
					result = true;
				}
				else if (this.Runner.transform.position.x < position.x && orientation == EntityOrientation.Left)
				{
					result = true;
				}
				return result;
			}
		}

		protected float CurrentChasingImpasse;
	}
}
