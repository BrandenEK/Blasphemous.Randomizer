using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Familiar.AI
{
	public class FamiliarChasePlayerState : State
	{
		private Familiar Familiar { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.Familiar = machine.GetComponent<Familiar>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Familiar.Behaviour.ChasingElongation = 0.1f;
		}

		public override void Update()
		{
			base.Update();
			this.Familiar.Behaviour.Floating();
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			FamiliarBehaviour behaviour = this.Familiar.Behaviour;
			if (!Core.Logic.Penitent)
			{
				return;
			}
			behaviour.ChasingEntity(Core.Logic.Penitent, behaviour.PlayerOffsetPosition);
			behaviour.SetOrientationByVelocity(behaviour.ChaseVelocity);
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}
	}
}
