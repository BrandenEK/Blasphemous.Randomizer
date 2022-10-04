using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Familiar.AI
{
	public class FamiliarChaseCherubState : State
	{
		private Familiar Familiar { get; set; }

		private bool IsCloseToCherub { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Familiar = machine.GetComponent<Familiar>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Familiar.Behaviour.ChasingElongation = 1f;
		}

		public override void Update()
		{
			base.Update();
			this.Familiar.Behaviour.Floating();
			if (!this.Familiar.Behaviour.CherubInstance)
			{
				return;
			}
			float num = Vector2.Distance(this.Familiar.transform.position, this.Familiar.Behaviour.CherubInstance.transform.position);
			this.IsCloseToCherub = (num < this.Familiar.Behaviour.CherubCriticalDistance);
			this.Familiar.Behaviour.ChasingElongation = ((!this.IsCloseToCherub) ? 1f : 0.2f);
			this.Familiar.GhostTrail.EnableGhostTrail = !this.IsCloseToCherub;
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			FamiliarBehaviour behaviour = this.Familiar.Behaviour;
			if (!behaviour.CherubInstance)
			{
				return;
			}
			behaviour.ChasingEntity(behaviour.CherubInstance, behaviour.CherubOffsetPosition);
			behaviour.SetOrientationByVelocity(behaviour.ChaseVelocity);
		}

		private float GetDistanceColorParam(float min, float max, Transform a, Transform b)
		{
			float num = Vector2.Distance(a.position, b.position);
			num = Mathf.Clamp(num, min, max);
			return (max - num) / (max - min);
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
