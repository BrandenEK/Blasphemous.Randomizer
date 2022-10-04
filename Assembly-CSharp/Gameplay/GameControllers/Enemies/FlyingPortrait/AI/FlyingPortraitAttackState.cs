using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.FlyingPortrait.AI
{
	public class FlyingPortraitAttackState : State
	{
		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this._flyingPortrait = machine.GetComponent<FlyingPortrait>();
			this._target = Core.Logic.Penitent.transform;
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
		}

		public override void Update()
		{
			base.Update();
			if (this._flyingPortrait.Behaviour.IsPlayerHeard() && !this._flyingPortrait.Behaviour.IsAwake)
			{
				this.SetOrientation();
				this._flyingPortrait.AnimatorInjector.UnHang();
			}
			if (!this._flyingPortrait.Behaviour.IsAwake)
			{
				return;
			}
			this.SetOrientation();
			this._attackCoolDownLapse += Time.deltaTime;
			if (Vector2.Distance(this._flyingPortrait.transform.position, this._target.position) > this._flyingPortrait.Behaviour.DistanceAttack)
			{
				this._flyingPortrait.Behaviour.Chase(this._target);
				this._stopChase = false;
			}
			else
			{
				this.StopChase();
				if (this._attackCoolDownLapse < this._flyingPortrait.Behaviour.AttackCoolDown)
				{
					return;
				}
				this._attackCoolDownLapse = 0f;
				this._flyingPortrait.AnimatorInjector.Attack();
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this._flyingPortrait.AnimatorInjector.ResetAttack();
		}

		private void StopChase()
		{
			if (this._stopChase)
			{
				return;
			}
			this._stopChase = true;
			bool flag = this._flyingPortrait.Status.Orientation == EntityOrientation.Right;
			float endValue = this._flyingPortrait.transform.position.x + ((!flag) ? 0.5f : -0.5f);
			this._flyingPortrait.transform.DOMoveX(endValue, 1f, false);
		}

		private void SetOrientation()
		{
			if (this._flyingPortrait.Behaviour.IsAttacking)
			{
				return;
			}
			GameObject gameObject = Core.Logic.Penitent.gameObject;
			EntityOrientation orientation = this._flyingPortrait.Status.Orientation;
			float num = (orientation != EntityOrientation.Left) ? 0.1f : -0.1f;
			bool flag = gameObject.transform.position.x + num > this._flyingPortrait.transform.position.x;
			this._flyingPortrait.SetOrientation((!flag) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
		}

		private FlyingPortrait _flyingPortrait;

		private StateMachine _stateMachine;

		private Transform _target;

		private float _attackCoolDownLapse;

		private bool _stopChase;
	}
}
