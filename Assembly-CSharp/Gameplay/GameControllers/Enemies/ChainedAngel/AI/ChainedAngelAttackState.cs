using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ChainedAngel.AI
{
	public class ChainedAngelAttackState : State
	{
		private ChainedAngel ChainedAngel { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.ChainedAngel = machine.GetComponent<ChainedAngel>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this._currentAttackLapse = 0f;
			this.ChainedAngel.FloatingMotion.IsFloating = false;
		}

		public override void Update()
		{
			base.Update();
			this.ChainedAngel.Behaviour.LookAtTarget(this.ChainedAngel.Target.transform.position);
			float deltaTime = Time.deltaTime;
			if (!this.ChainedAngel.BodyChainMaster.IsAttacking)
			{
				this._currentAttackLapse += deltaTime;
			}
			if (!this.ChainedAngel.Behaviour.CanSeeTarget)
			{
				this._currentAttackLapse = 0f;
				this._attackAwaitingLapse += deltaTime;
				if (this._attackAwaitingLapse > 1f)
				{
					this.ChainedAngel.StateMachine.SwitchState<ChainedAngelIdleState>();
				}
			}
			else
			{
				this._attackAwaitingLapse = 0f;
			}
			if (this._currentAttackLapse <= this.ChainedAngel.Behaviour.AttackLapse)
			{
				return;
			}
			this._currentAttackLapse = 0f;
			this.ChainedAngel.Behaviour.Attack();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		private float _currentAttackLapse;

		private float _attackAwaitingLapse;
	}
}
