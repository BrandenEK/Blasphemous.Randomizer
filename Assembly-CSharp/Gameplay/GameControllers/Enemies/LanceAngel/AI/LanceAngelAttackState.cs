using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.LanceAngel.AI
{
	public class LanceAngelAttackState : State
	{
		protected LanceAngel LanceAngel { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.LanceAngel = machine.GetComponent<LanceAngel>();
			this._behaviour = this.LanceAngel.Behaviour;
			this.LanceAngel.DashAttack.OnDashFinishedEvent += this.OnDashFinished;
			this.LanceAngel.OnDamaged += this.OnDamaged;
			LanceAngelBehaviour behaviour = this._behaviour;
			behaviour.OnParry = (Core.SimpleEvent)Delegate.Combine(behaviour.OnParry, new Core.SimpleEvent(this.OnParry));
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.ResetCooldown();
		}

		public override void Update()
		{
			base.Update();
			this._cooldownCounter -= Time.deltaTime;
			this._behaviour.Floating();
			Vector3 position = this.LanceAngel.Target.transform.position;
			float num = Vector2.Distance(this.LanceAngel.transform.position, position);
			this._behaviour.LookAtTarget(position);
			if (num > this._behaviour.DistanceAttack || this.IsCooling)
			{
				this.LanceAngel.Behaviour.Chasing(position);
			}
			else if (!this._behaviour.IsRepositioning)
			{
				this.LanceAngel.AnimatorInjector.AttackReady();
				this.LanceAngel.GhostSprites.EnableGhostTrail = true;
				this._behaviour.Reposition(new Action(this.LanceAngel.AnimatorInjector.AttackStart));
			}
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			if (this._behaviour.IsRepositioning)
			{
				this.LanceAngel.Spline.transform.position = this._behaviour.PathOrigin;
			}
		}

		private bool IsCooling
		{
			get
			{
				return this._cooldownCounter > 0f;
			}
		}

		private void ResetCooldown()
		{
			if (this._cooldownCounter < this.LanceAngel.Behaviour.AttackCooldown)
			{
				this._cooldownCounter = this.LanceAngel.Behaviour.AttackCooldown;
			}
		}

		private void OnDashFinished()
		{
			this._behaviour.IsRepositioning = false;
			this.LanceAngel.GhostSprites.EnableGhostTrail = false;
			this.LanceAngel.Spline.transform.localPosition = Vector3.zero;
			this.LanceAngel.AnimatorInjector.StopAttack();
			this.ResetCooldown();
		}

		private void OnDamaged()
		{
			this._cooldownCounter = this.LanceAngel.MotionLerper.TimeTakenDuringLerp;
		}

		private void OnParry()
		{
			this.LanceAngel.StateMachine.SwitchState<LanceAngelParryState>();
		}

		public override void Destroy()
		{
			base.Destroy();
			if (this.LanceAngel != null)
			{
				this.LanceAngel.DashAttack.OnDashFinishedEvent -= this.OnDashFinished;
				this.LanceAngel.OnDamaged -= this.OnDamaged;
				LanceAngelBehaviour behaviour = this._behaviour;
				behaviour.OnParry = (Core.SimpleEvent)Delegate.Remove(behaviour.OnParry, new Core.SimpleEvent(this.OnParry));
			}
		}

		private float _cooldownCounter;

		private LanceAngelBehaviour _behaviour;
	}
}
