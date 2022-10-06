using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Menina.AI
{
	public class MeninaBehaviour : EnemyBehaviour
	{
		public Menina Menina { get; private set; }

		public float CurrentChasingTime { get; set; }

		public float CurrentAttackLapse { get; set; }

		public bool IsAwake { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.Menina = (Menina)this.Entity;
			this.originalSpeed = this.Menina.Controller.MaxWalkingSpeed;
			this.Menina.OnDeath += this.OnDeath;
			this.Menina.StateMachine.SwitchState<MeninaStateBackwards>();
		}

		private void OnDeath()
		{
			this.Menina.OnDeath -= this.OnDeath;
			this.NormalSpeed();
			this.Menina.AnimatorInyector.Death();
			this.Menina.SpriteRenderer.GetComponent<BoxCollider2D>().enabled = false;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (base.IsDead() || this.isExecuted)
			{
				this.StopMovement();
				return;
			}
			if (!this.IsAwake && base.PlayerHeard)
			{
				this.Menina.StateMachine.SwitchState<MeninaStateChase>();
			}
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
		}

		public void StepBackwards()
		{
			this.NormalSpeed();
			this.CurrentChasingTime = 0f;
			this.Menina.AnimatorInyector.Step(false);
		}

		public override void Chase(Transform targetPosition)
		{
			this.NormalSpeed();
			this.CurrentChasingTime = 0f;
			this.Menina.AnimatorInyector.Step(true);
		}

		public override void Attack()
		{
			this.NormalSpeed();
			this.CurrentAttackLapse = 0f;
			this.Menina.IsAttacking = true;
			this.Menina.AnimatorInyector.Attack();
		}

		public void ResetAttackCoolDown()
		{
			this.CurrentAttackLapse = 0f;
		}

		public override void Damage()
		{
			if (this.playOnHurt)
			{
				Core.Audio.PlaySfx(this.OnHurt, 0f);
			}
		}

		public void SingleStep(bool forward)
		{
			this.Menina.AnimatorInyector.OnStepFinished += this.OnStepFinished;
			this.Menina.AnimatorInyector.Step(forward);
			this.SpeedUp(forward);
		}

		private void SpeedUp(bool changeContactDamage = false)
		{
			if (changeContactDamage)
			{
				this.Menina.Attack.ContactDamageType = DamageArea.DamageType.Heavy;
			}
			float num = 1.5f;
			this.Menina.Animator.speed = 1.5f;
			this.Menina.Controller.MaxWalkingSpeed = this.originalSpeed * num;
		}

		public bool ShouldRepeatSmash()
		{
			return Random.Range(0f, 1f) < this.repeatSmashProbability;
		}

		private void NormalSpeed()
		{
			this.Menina.Attack.ContactDamageType = DamageArea.DamageType.Normal;
			this.Menina.Animator.speed = 1f;
			this.Menina.Controller.MaxWalkingSpeed = this.originalSpeed;
		}

		private void OnStepFinished()
		{
			this.Menina.AnimatorInyector.OnStepFinished -= this.OnStepFinished;
			this.NormalSpeed();
			this.StopMovement();
			this.CurrentAttackLapse = this.AttackCooldown;
		}

		public override void StopMovement()
		{
			if (this.Menina == null)
			{
				return;
			}
			this.Menina.Inputs.HorizontalInput = 0f;
			this.Menina.AnimatorInyector.Stop();
		}

		public override void Execution()
		{
			base.Execution();
			this.isExecuted = true;
			this.Menina.gameObject.layer = LayerMask.NameToLayer("Default");
			this.Menina.Audio.StopAttack();
			this.Menina.Animator.Play("Idle");
			this.Menina.StateMachine.SwitchState<MeninaStateBackwards>();
			this.Menina.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.Menina.Attack.StopAttack();
			this.Menina.AnimatorInyector.gameObject.SetActive(false);
			if (!this.Menina.EntExecution)
			{
				return;
			}
			this.Menina.EntExecution.InstantiateExecution();
			this.Menina.EntExecution.enabled = true;
		}

		public override void Alive()
		{
			base.Alive();
			this.isExecuted = false;
			this.Menina.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.Menina.SpriteRenderer.enabled = true;
			this.Menina.AnimatorInyector.gameObject.SetActive(true);
			this.Menina.Animator.Play("Idle");
			this.Menina.CurrentLife = this.Menina.Stats.Life.Base / 2f;
			if (this.Menina.EntExecution != null)
			{
				this.Menina.EntExecution.enabled = false;
			}
		}

		private void OnDrawGizmos()
		{
		}

		private float originalSpeed;

		private bool isExecuted;

		public bool playOnHurt;

		[EventRef]
		public string OnHurt;

		public float AttackCooldown;

		public float repeatSmashProbability = 0.5f;

		public float AwaitBeforeBackward;
	}
}
