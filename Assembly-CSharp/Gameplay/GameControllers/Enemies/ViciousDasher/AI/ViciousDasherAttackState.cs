using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ViciousDasher.AI
{
	public class ViciousDasherAttackState : State
	{
		public ViciousDasher ViciousDasher { get; private set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.ViciousDasher = machine.GetComponent<ViciousDasher>();
			MotionLerper motionLerper = this.ViciousDasher.MotionLerper;
			motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
		}

		private void OnLerpStop()
		{
			if (this.ViciousDasher.DistanceToTarget <= this.ViciousDasher.ViciousDasherBehaviour.CloseRange)
			{
				this.ViciousDasher.IsAttacking = true;
				this.ViciousDasher.AnimatorInjector.ResetDash();
				this.ViciousDasher.AnimatorInjector.Attack();
			}
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.ViciousDasher.AnimatorInjector.Dash();
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this._currentAttackTime = 0f;
			this.ViciousDasher.AnimatorInjector.StopAttack();
		}

		public override void Update()
		{
			base.Update();
			bool flag = this.ViciousDasher.DistanceToTarget <= this.ViciousDasher.ViciousDasherBehaviour.CloseRange;
			if (this.ViciousDasher.IsAttacking)
			{
				this._currentAttackTime += Time.deltaTime;
				if (this._currentAttackTime < this.ViciousDasher.ViciousDasherBehaviour.AttackTime)
				{
					return;
				}
				this._currentAttackTime = 0f;
				this.ViciousDasher.IsAttacking = false;
			}
			else if (flag)
			{
				this.ViciousDasher.AnimatorInjector.ResetDash();
				this.ViciousDasher.MotionLerper.StopLerping();
			}
			else
			{
				this.ViciousDasher.AnimatorInjector.StopAttack();
				this.ViciousDasher.AnimatorInjector.Dash();
			}
			if (this.ViciousDasher.MotionLerper.IsLerping)
			{
				this.ViciousDasher.AnimatorInjector.StopAttack();
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			if (this.ViciousDasher != null)
			{
				MotionLerper motionLerper = this.ViciousDasher.MotionLerper;
				motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			}
		}

		private float _currentAttackTime;
	}
}
