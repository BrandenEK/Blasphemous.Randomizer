using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Menina.AI
{
	public class MeninaStateAttack : State
	{
		private Menina Menina { get; set; }

		private MeninaBehaviour Behaviour { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			if (!this.Menina)
			{
				this.Menina = machine.GetComponent<Menina>();
			}
			if (!this.Behaviour)
			{
				this.Behaviour = this.Menina.GetComponent<MeninaBehaviour>();
			}
			this.availableActions = new List<MeninaStateAttack.MENINA_ACTIONS>
			{
				MeninaStateAttack.MENINA_ACTIONS.SMASH,
				MeninaStateAttack.MENINA_ACTIONS.STEP_BACK,
				MeninaStateAttack.MENINA_ACTIONS.STEP_FORWARD
			};
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.walkBalance = 0;
		}

		public override void Update()
		{
			base.Update();
			this.Behaviour.CurrentAttackLapse += Time.deltaTime;
			if (this.Behaviour.CurrentAttackLapse < this.Behaviour.AttackCooldown)
			{
				return;
			}
			this.Behaviour.CurrentAttackLapse = 0f;
			this.ChooseAttackAction();
			this.CheckNewState();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.Behaviour.CurrentAttackLapse = 0f;
			this.Menina.AnimatorInyector.ResetAttackTrigger();
		}

		private void ChooseAttackAction()
		{
			this.Behaviour.StopMovement();
			float num = Vector2.Distance(this.Menina.StartPosition, this.Menina.transform.position);
			MeninaStateAttack.MENINA_ACTIONS action = this.GetAction();
			Debug.Log("ACTION = " + action.ToString());
			if (action == MeninaStateAttack.MENINA_ACTIONS.STEP_FORWARD)
			{
				this.Behaviour.SingleStep(true);
			}
			else if (action == MeninaStateAttack.MENINA_ACTIONS.STEP_BACK && num > 1f)
			{
				this.Behaviour.SingleStep(false);
			}
			else
			{
				this.Menina.EnemyBehaviour.Attack();
				this.Menina.StateMachine.SwitchState<MeninaStateAttack>();
			}
			this.lastAction = action;
		}

		private void CheckNewState()
		{
			if (this.Behaviour.PlayerSeen)
			{
				return;
			}
			if (this.Behaviour.PlayerHeard)
			{
				this.Menina.StateMachine.SwitchState<MeninaStateChase>();
			}
			else
			{
				this.Menina.StateMachine.SwitchState<MeninaStateBackwards>();
			}
		}

		private bool ShouldRepeatSmash()
		{
			return this.Behaviour.ShouldRepeatSmash();
		}

		private MeninaStateAttack.MENINA_ACTIONS GetAction()
		{
			if (this.lastAction != MeninaStateAttack.MENINA_ACTIONS.SMASH || (this.lastAction == MeninaStateAttack.MENINA_ACTIONS.SMASH && this.ShouldRepeatSmash()))
			{
				return MeninaStateAttack.MENINA_ACTIONS.SMASH;
			}
			if (this.walkBalance < 0)
			{
				this.walkBalance++;
				return MeninaStateAttack.MENINA_ACTIONS.STEP_FORWARD;
			}
			this.walkBalance--;
			return MeninaStateAttack.MENINA_ACTIONS.STEP_BACK;
		}

		protected MeninaStateAttack.MENINA_ACTIONS lastAction = MeninaStateAttack.MENINA_ACTIONS.STEP_BACK;

		protected List<MeninaStateAttack.MENINA_ACTIONS> availableActions;

		private int walkBalance;

		protected enum MENINA_ACTIONS
		{
			SMASH,
			STEP_BACK,
			STEP_FORWARD
		}
	}
}
