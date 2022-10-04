using System;
using Gameplay.GameControllers.Entities.StateMachine;

namespace Gameplay.GameControllers.Enemies.LanceAngel.AI
{
	public class LanceAngelParryState : State
	{
		protected LanceAngel LanceAngel { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.LanceAngel = machine.GetComponent<LanceAngel>();
			this._defaultDisplacement = this.LanceAngel.MotionLerper.distanceToMove;
			this._defaultLapse = this.LanceAngel.MotionLerper.TimeTakenDuringLerp;
			this.LanceAngel.OnDamaged += this.OnDamaged;
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.LanceAngel.MotionLerper.distanceToMove = 0.25f;
			this.LanceAngel.MotionLerper.TimeTakenDuringLerp = 0.5f;
			this.LanceAngel.Behaviour.HurtDisplacement();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.LanceAngel.MotionLerper.distanceToMove = this._defaultDisplacement;
			this.LanceAngel.MotionLerper.TimeTakenDuringLerp = this._defaultLapse;
		}

		private void OnDamaged()
		{
			this.LanceAngel.StateMachine.SwitchState<LanceAngelAttackState>();
		}

		public override void Destroy()
		{
			base.Destroy();
			if (this.LanceAngel)
			{
				this.LanceAngel.OnDamaged -= this.OnDamaged;
			}
		}

		private float _defaultDisplacement;

		private float _defaultLapse;
	}
}
