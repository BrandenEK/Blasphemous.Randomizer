using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Legionary.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Legionary.AI
{
	public class LegionaryBehaviour : EnemyBehaviour
	{
		protected Legionary Legionary { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.Legionary = (Legionary)this.Entity;
			this.Legionary.StateMachine.SwitchState<LegionaryWanderState>();
			this.AttackRatio = this.LightMeleeAttackWeight + this.SpinMeleeAttackWeight;
			Core.Logic.Penitent.OnDeath += this.OnDeathPlayer;
			this.Legionary.OnDeath += this.OnDeath;
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			this.Legionary.AnimatorInjector.Walk(true);
			this.Move(this.MoveSpeed.x);
			if (!this.CanWalk)
			{
				this.ReverseOrientation();
			}
		}

		public override void Chase(Transform targetPosition)
		{
			this.Legionary.AnimatorInjector.Run(true);
			this.Legionary.Behaviour.LookAtTarget(targetPosition.position);
			this.Move(this.MoveSpeed.y);
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.Legionary.transform.position.x)
			{
				if (this.Legionary.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.Legionary.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.Legionary.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.Legionary.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public void Move(float speed)
		{
			float horizontalInput = (this.Legionary.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.Legionary.Controller.MaxWalkingSpeed = speed;
			this.Legionary.Inputs.HorizontalInput = horizontalInput;
		}

		public override void StopMovement()
		{
			this.Legionary.Controller.PlatformCharacterPhysics.HSpeed = 0f;
			this.Legionary.Inputs.HorizontalInput = 0f;
		}

		public void Stop()
		{
			this.Legionary.AnimatorInjector.Walk(false);
			this.Legionary.AnimatorInjector.Run(false);
			this.StopMovement();
		}

		public bool CanWalk
		{
			get
			{
				return !this.Legionary.MotionChecker.HitsBlock && this.Legionary.MotionChecker.HitsFloor;
			}
		}

		public bool CanSeeTarget
		{
			get
			{
				return this.Legionary.VisionCone.CanSeeTarget(this.Legionary.Target.transform, "Penitent", false);
			}
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			if (this.CanTakeHits)
			{
				this._hitsWhileHurtCounter++;
				this.Legionary.AnimatorInjector.Hurt();
			}
			else
			{
				this.Legionary.CanTakeDamage = false;
				this.Legionary.AnimatorInjector.LightningSummon();
			}
		}

		public override void Parry()
		{
			base.Parry();
			this.Legionary.AnimatorInjector.Parry();
			base.GotParry = true;
			this.Legionary.MotionLerper.StopLerping();
		}

		public override void Alive()
		{
			base.Alive();
			base.GotParry = false;
			this.Legionary.Animator.Play("Idle");
		}

		public void RandMeleeAttack()
		{
			int num = Random.Range(0, this.AttackRatio);
			if ((num -= this.LightMeleeAttackWeight) < 0)
			{
				this.MeleeLightAttack();
			}
			else if (num - this.SpinMeleeAttackWeight < 0)
			{
				this.MeleeSpinAttack();
			}
		}

		public bool CanTakeHits
		{
			get
			{
				return this._hitsWhileHurtCounter < this.MaxHitsWhileHurt;
			}
		}

		public void ResetHitsCounter()
		{
			if (this._hitsWhileHurtCounter >= this.MaxHitsWhileHurt)
			{
				this._hitsWhileHurtCounter = 0;
			}
		}

		public void LightningSummonAttack()
		{
			Vector3 position = this.Legionary.Target.transform.position;
			this.Legionary.LightningSummonAttack.SummonAreaOnPoint(position, 0f, 1f, null);
		}

		public void MeleeLightAttack()
		{
			this.Legionary.AnimatorInjector.LightAttack();
		}

		public void MeleeSpinAttack()
		{
			this.Legionary.AnimatorInjector.SpinAttack();
		}

		private void OnDeathPlayer()
		{
			Core.Logic.Penitent.OnDeath -= this.OnDeathPlayer;
			this.Legionary.StateMachine.enabled = false;
			this.Stop();
		}

		private void OnDeath()
		{
			this.Legionary.OnDeath -= this.OnDeath;
			LegionaryAudio componentInChildren = this.Legionary.GetComponentInChildren<LegionaryAudio>();
			if (componentInChildren)
			{
				componentInChildren.StopSlideAttack_AUDIO();
			}
			this.Legionary.StateMachine.enabled = false;
		}

		[MinMaxSlider(0f, 10f, false)]
		public Vector2 MoveSpeed;

		public float MinDistanceAttack = 3f;

		public int LightMeleeAttackWeight = 1;

		public int SpinMeleeAttackWeight = 1;

		public int MaxHitsWhileHurt = 3;

		[Tooltip("Time required in attack state to go patrolling if the player is not seen.")]
		public float TimeLapseToGoPatrolling = 5f;

		protected int AttackRatio;

		private int _hitsWhileHurtCounter;
	}
}
