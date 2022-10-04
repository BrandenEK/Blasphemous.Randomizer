using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Legionary.AI
{
	public class LegionaryAttackState : State
	{
		private protected Legionary Legionary { protected get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Legionary = machine.GetComponent<Legionary>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.TargetLostLapse = this.Legionary.Behaviour.TimeLapseToGoPatrolling;
		}

		public override void Update()
		{
			base.Update();
			bool canWalk = this.Legionary.Behaviour.CanWalk;
			Transform transform = this.Legionary.Target.transform;
			float num = Vector2.Distance(transform.position, this.Legionary.transform.position);
			if (this.Legionary.Behaviour.GotParry)
			{
				this.Legionary.Behaviour.Stop();
				return;
			}
			if (!this.Legionary.Behaviour.CanSeeTarget)
			{
				this.TargetLostLapse -= Time.deltaTime;
				if (this.TargetLostLapse <= 0f)
				{
					this.Legionary.StateMachine.SwitchState<LegionaryWanderState>();
				}
			}
			else
			{
				this.TargetLostLapse = this.Legionary.Behaviour.TimeLapseToGoPatrolling;
			}
			if (!canWalk)
			{
				this.Legionary.MotionLerper.StopLerping();
			}
			if (!this.Legionary.IsAttacking && !this.Legionary.Behaviour.IsHurt)
			{
				if (num > this.Legionary.Behaviour.MinDistanceAttack && canWalk)
				{
					this.Legionary.Behaviour.Chase(transform);
				}
				if (num <= this.Legionary.Behaviour.MinDistanceAttack)
				{
					this.Legionary.Behaviour.Stop();
					this.Legionary.Behaviour.RandMeleeAttack();
				}
			}
			else
			{
				if (!canWalk)
				{
					this.Legionary.Behaviour.LookAtTarget(transform.position);
				}
				this.Legionary.Behaviour.Stop();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		protected float TargetLostLapse;
	}
}
