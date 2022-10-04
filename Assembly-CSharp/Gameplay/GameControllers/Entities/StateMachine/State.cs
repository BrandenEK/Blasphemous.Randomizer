using System;

namespace Gameplay.GameControllers.Entities.StateMachine
{
	public class State
	{
		public static implicit operator bool(State state)
		{
			return state != null;
		}

		public virtual void OnStateInitialize(StateMachine machine)
		{
			this.Machine = machine;
		}

		public virtual void OnStateEnter()
		{
		}

		public virtual void OnStateExit()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void LateUpdate()
		{
		}

		public virtual void Destroy()
		{
		}

		public StateMachine Machine;
	}
}
