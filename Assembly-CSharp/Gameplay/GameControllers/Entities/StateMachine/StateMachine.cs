using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.StateMachine
{
	public class StateMachine : MonoBehaviour
	{
		private void Update()
		{
			if (this.CurrentState)
			{
				this.CurrentState.Update();
			}
		}

		private void LateUpdate()
		{
			if (this.CurrentState)
			{
				this.CurrentState.LateUpdate();
			}
		}

		public State GetCurrentState
		{
			get
			{
				return this.CurrentState;
			}
		}

		protected virtual bool SwitchState(State state)
		{
			bool result = false;
			if (state && state != this.CurrentState)
			{
				if (this.CurrentState)
				{
					this.CurrentState.OnStateExit();
				}
				this.CurrentState = state;
				this.CurrentState.OnStateEnter();
				result = true;
			}
			return result;
		}

		public virtual bool SwitchState<TStateType>() where TStateType : State, new()
		{
			bool result = false;
			bool flag = false;
			foreach (State state in this.StatesList)
			{
				if (state is TStateType)
				{
					flag = true;
					result = this.SwitchState(state);
					break;
				}
			}
			if (flag)
			{
				return result;
			}
			State state2 = Activator.CreateInstance<TStateType>();
			state2.OnStateInitialize(this);
			this.StatesList.Add(state2);
			result = this.SwitchState(state2);
			return result;
		}

		private void OnDestroy()
		{
			foreach (State state in this.StatesList)
			{
				state.Destroy();
			}
			this.StatesList.Clear();
		}

		protected List<State> StatesList = new List<State>();

		protected State CurrentState;
	}
}
