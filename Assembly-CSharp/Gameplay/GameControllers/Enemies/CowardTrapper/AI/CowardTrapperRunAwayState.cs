using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CowardTrapper.AI
{
	public class CowardTrapperRunAwayState : State
	{
		protected CowardTrapper CowardTrapper { get; set; }

		private Vector2 CurrentRunAwayDirection { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.CowardTrapper = machine.GetComponent<CowardTrapper>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this._currentTimeRunning = this.CowardTrapper.Behaviour.TimeRunning;
			this._currentTimeAwaiting = this.CowardTrapper.Behaviour.TimeAwaiting;
			this._targetPosition = this.CowardTrapper.Target.transform.position;
			Debug.Log(this.CowardTrapper.Behaviour.ReverseDirection);
			this.CurrentRunAwayDirection = new Vector2(this.GetRunningDirection.x, this.GetRunningDirection.y);
			if (this.CowardTrapper.Behaviour.ReverseDirection)
			{
				this.CurrentRunAwayDirection *= -1f;
			}
		}

		public override void Update()
		{
			base.Update();
			this._currentTimeRunning -= Time.deltaTime;
			if (this._currentTimeRunning > 0f)
			{
				this.ResetTimeAwaiting();
				if (!this.CowardTrapper.Behaviour.IsBlocked)
				{
					this.CowardTrapper.Behaviour.IsRunAway = true;
					this.CowardTrapper.Behaviour.RunAway(this.CurrentRunAwayDirection);
				}
				else
				{
					this.CowardTrapper.Behaviour.ReverseDirection = true;
					this.CowardTrapper.Behaviour.StopMovement();
					this._currentTimeRunning = 0f;
				}
			}
			else
			{
				this.CowardTrapper.Behaviour.IsRunAway = false;
				this._currentTimeAwaiting -= Time.deltaTime;
				if (this._currentTimeAwaiting < 0f)
				{
					this.ResetTimeRunning();
				}
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private Vector2 GetRunningDirection
		{
			get
			{
				return (this.CowardTrapper.Target.transform.position.x >= this.CowardTrapper.transform.position.x) ? Vector2.right : Vector2.left;
			}
		}

		private void ResetTimeRunning()
		{
			if (this._currentTimeRunning < this.CowardTrapper.Behaviour.TimeRunning)
			{
				this._currentTimeRunning = this.CowardTrapper.Behaviour.TimeRunning;
			}
		}

		private void ResetTimeAwaiting()
		{
			if (this._currentTimeAwaiting < this.CowardTrapper.Behaviour.TimeAwaiting)
			{
				this._currentTimeAwaiting = this.CowardTrapper.Behaviour.TimeAwaiting;
			}
		}

		private float _currentTimeRunning;

		private float _currentTimeAwaiting;

		private Vector2 _targetPosition;
	}
}
