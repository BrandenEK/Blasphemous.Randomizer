using System;
using System.Collections;
using CreativeSpore.SmartColliders;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Runner.AI
{
	public class RunnerBehaviour : EnemyBehaviour
	{
		public Runner Runner { get; private set; }

		public bool IsScreaming { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.Runner = (Runner)this.Entity;
			ContactDamage contactDamage = this.Runner.ContactDamage;
			contactDamage.OnContactDamage = (Core.GenericEvent)Delegate.Combine(contactDamage.OnContactDamage, new Core.GenericEvent(this.OnContactDamage));
			this.Runner.OnDeath += this.OnDeath;
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnPlayerDead));
			this.subscribedToEvents = true;
		}

		private void OnPlayerDead()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.OnPlayerDead));
			this.Runner.StateMachine.SwitchState<RunnerIdleState>();
			base.enabled = false;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Runner.Status.Dead || this.isExecuted)
			{
				this.Runner.StateMachine.SwitchState<RunnerIdleState>();
			}
			else if (this.Runner.VisionCone.CanSeeTarget(this.Runner.Target.transform, "Penitent", false))
			{
				this._currentChasingTime = this.ChasingRemainTime;
				this.Runner.StateMachine.SwitchState<RunnerChaseState>();
			}
			else
			{
				this._currentChasingTime -= Time.deltaTime;
				if (this._currentChasingTime < 0f)
				{
					this.Runner.StateMachine.SwitchState<RunnerIdleState>();
				}
			}
		}

		public override void Chase(Transform targetPosition)
		{
			this.Runner.Controller.MaxWalkingSpeed = 7f;
			this.Runner.Attack.SetContactDamageType(DamageArea.DamageType.Heavy);
			this.Runner.Attack.SetContactDamage(this.Runner.Stats.Strength.Final);
			float horizontalInput = (this.Runner.Status.Orientation != EntityOrientation.Left) ? 1f : -1f;
			this.Runner.Input.HorizontalInput = horizontalInput;
			base.IsChasing = true;
			this.Runner.IsAttacking = true;
			this.Runner.AnimatorInjector.Run(base.IsChasing);
		}

		private bool LessThanHalfHP()
		{
			return this.Runner.Stats.Life.Current < this.Runner.Stats.Life.CurrentMax * 0.5f;
		}

		private void SetFrenzy()
		{
			this.frenzy = true;
			this.Runner.Animator.speed = 1f * this.frenzyFactor;
			PlatformCharacterController component = base.GetComponent<PlatformCharacterController>();
			float maxWalkingSpeed = component.MaxWalkingSpeed;
			component.MaxWalkingSpeed = maxWalkingSpeed * this.frenzyFactor;
			this.angryParticles.Play();
		}

		public void Scream()
		{
			if (!this.frenzy && this.LessThanHalfHP())
			{
				this.SetFrenzy();
			}
			this.Runner.AnimatorInjector.Scream();
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.Runner.Status.Orientation == EntityOrientation.Right)
			{
				if (this.Runner.transform.position.x >= targetPos.x)
				{
					this.Runner.AnimatorInjector.TurnAround();
				}
			}
			else if (this.Runner.transform.position.x <= targetPos.x)
			{
				this.Runner.AnimatorInjector.TurnAround();
			}
		}

		private void OnContactDamage(UnityEngine.Object param)
		{
			this.Scream();
		}

		public bool CanChase
		{
			get
			{
				return !this.Runner.MotionChecker.HitsBlock && this.Runner.MotionChecker.HitsFloor;
			}
		}

		public override void StopMovement()
		{
			if (this.Runner.Controller.MaxWalkingSpeed >= 7f)
			{
				base.StartCoroutine(this.ReduceSpeed());
			}
			base.IsChasing = false;
			this.Runner.IsAttacking = false;
			this.Runner.Attack.SetContactDamageType(DamageArea.DamageType.Normal);
			this.Runner.Attack.SetContactDamage(this.Runner.Attack.ContactDamageAmount);
			this.Runner.AnimatorInjector.Run(base.IsChasing);
		}

		private IEnumerator ReduceSpeed()
		{
			float currentSpeed = this.Runner.Controller.MaxWalkingSpeed;
			while (currentSpeed > 0f)
			{
				if (!this.CanChase)
				{
					this.Stop();
					yield break;
				}
				currentSpeed -= Time.deltaTime * this.DecelerationFactor;
				this.Runner.Controller.MaxWalkingSpeed = currentSpeed;
				yield return null;
			}
			this.Stop();
			yield break;
		}

		public void Stop()
		{
			this.Runner.Input.HorizontalInput = 0f;
			this.Runner.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.Runner.Controller.MaxWalkingSpeed = 0f;
		}

		public override void Execution()
		{
			base.Execution();
			this.isExecuted = true;
			this.Runner.gameObject.layer = LayerMask.NameToLayer("Default");
			this.Runner.Audio.StopScream();
			this.Runner.Animator.Play("Idle");
			this.Stop();
			this.Runner.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.Runner.Attack.enabled = false;
			this.Runner.EntExecution.InstantiateExecution();
			if (this.Runner.EntExecution != null)
			{
				this.Runner.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.isExecuted = false;
			this.Runner.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.Runner.SpriteRenderer.enabled = true;
			this.Runner.Animator.Play("Idle");
			this.Runner.CurrentLife = this.Runner.Stats.Life.Base / 2f;
			this.Runner.Attack.enabled = true;
			if (this.Runner.EntExecution != null)
			{
				this.Runner.EntExecution.enabled = false;
			}
		}

		private void OnDeath()
		{
			this.UnSubscribeEvents();
		}

		private void OnDestroy()
		{
			this.UnSubscribeEvents();
		}

		private void UnSubscribeEvents()
		{
			if (!this.Runner || !this.subscribedToEvents)
			{
				return;
			}
			this.subscribedToEvents = false;
			this.Runner.OnDeath -= this.OnDeath;
			ContactDamage contactDamage = this.Runner.ContactDamage;
			contactDamage.OnContactDamage = (Core.GenericEvent)Delegate.Remove(contactDamage.OnContactDamage, new Core.GenericEvent(this.OnContactDamage));
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public float ChasingRemainTime = 2f;

		public float ChasingImpasse = 1f;

		private float _currentChasingTime;

		private const float MaxSpeed = 7f;

		public float DecelerationFactor = 2f;

		public float frenzyFactor = 1.4f;

		private bool frenzy;

		public ParticleSystem angryParticles;

		private bool isExecuted;

		private bool subscribedToEvents;
	}
}
