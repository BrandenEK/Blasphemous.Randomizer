using System;
using Gameplay.GameControllers.Enemies.NewFlagellant.Animator;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.NewFlagellant.AI
{
	public class NewFlagellantAttackState : State
	{
		public NewFlagellant NewFlagellant { get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.NewFlagellant = machine.GetComponent<NewFlagellant>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.NewFlagellant.NewFlagellantBehaviour.StopMovement();
		}

		private void AnimatorInjector_OnAttackAnimationFinished(NewFlagellantAnimatorInyector obj)
		{
			obj.OnAttackAnimationFinished -= this.AnimatorInjector_OnAttackAnimationFinished;
			this.NewFlagellant.NewFlagellantBehaviour.LookAtPenitent();
			this.NewFlagellant.StateMachine.SwitchState<NewFlagellantChaseState>();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
		}

		public override void Update()
		{
			base.Update();
			if (this.NewFlagellant.NewFlagellantBehaviour.CanAttack())
			{
				this.NewFlagellant.AnimatorInyector.OnAttackAnimationFinished -= this.AnimatorInjector_OnAttackAnimationFinished;
				this.NewFlagellant.AnimatorInyector.OnAttackAnimationFinished += this.AnimatorInjector_OnAttackAnimationFinished;
				float num = UnityEngine.Random.Range(0f, 1f);
				if ((double)num < 0.33)
				{
					this.NewFlagellant.AnimatorInyector.FastAttack();
				}
				else
				{
					this.NewFlagellant.AnimatorInyector.Attack();
				}
				this.NewFlagellant.NewFlagellantBehaviour.ResetCooldown();
			}
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private float _currentAttackTime;
	}
}
