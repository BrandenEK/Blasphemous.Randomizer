using System;
using System.Collections;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Jumper.AI
{
	public class JumperBehaviour : EnemyBehaviour
	{
		public float DistanceToTarget { get; private set; }

		public Jumper Jumper { get; set; }

		public bool IsPlayerDead { get; set; }

		public bool IsJumping { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.Jumper = (Jumper)this.Entity;
			this._shortLapseWaiting = new WaitForSeconds(0.01f);
			this._longTimeJumpWaiting = new WaitForSeconds(1f);
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnDead));
			this.Jumper.OnDeath += this.OnDeathJumper;
			this._defaultJumpSpeed = this.Jumper.Controller.JumpingSpeed;
		}

		private void OnDead()
		{
			this.IsPlayerDead = true;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.IsPlayerDead || this.Jumper.Status.Dead || !this.TargetIsOnSight)
			{
				return;
			}
			this.DistanceToTarget = Vector2.Distance(this.Jumper.transform.position, base.GetTarget().position);
			this.Jumper.Status.IsGrounded = this.Jumper.Controller.IsGrounded;
			if (this.DistanceToTarget <= this.ActivationDistance && !this.IsJumping)
			{
				this.IsJumping = true;
				this.Jumper.Animator.Play("JumpReady");
			}
		}

		private bool TargetIsOnSight
		{
			get
			{
				Transform target = base.GetTarget();
				float num = target.position.y - base.transform.position.y;
				num = ((num <= 0f) ? (-num) : num);
				return num <= 2f;
			}
		}

		public override void StopMovement()
		{
			this.Jumper.Inputs.HorizontalInput = 0f;
		}

		public void Jump()
		{
			if (this.Jumper == null)
			{
				return;
			}
			bool flag = this.DistanceToTarget > this.LongJumpRange;
			this.Jumper.Controller.JumpingSpeed = ((!flag) ? this._defaultJumpSpeed : this.LongJumpSpeed);
			base.StartCoroutine(this.JumpPress(flag));
		}

		private IEnumerator JumpPress(bool isLongPress)
		{
			this.Jumper.Inputs.Jump = true;
			WaitForSeconds awaitType = (!isLongPress) ? this._shortLapseWaiting : this._longTimeJumpWaiting;
			yield return awaitType;
			this.Jumper.Inputs.Jump = false;
			yield break;
		}

		private void OnDeathJumper()
		{
			this.Jumper.Attack.AttackArea.WeaponCollider.enabled = false;
			this.Jumper.Controller.enabled = false;
			this.Jumper.DamageByContact = false;
			this.Jumper.AnimatorInjector.Death();
		}

		private void OnDestroy()
		{
			if (Core.Logic.Penitent)
			{
				Penitent penitent = Core.Logic.Penitent;
				penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.OnDead));
			}
			if (this.Jumper != null)
			{
				this.Jumper.OnDeath -= this.OnDeathJumper;
			}
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float LongJumpRange;

		[FoldoutGroup("Motion Settings", true, 0)]
		public float LongJumpSpeed = 10f;

		private float _defaultJumpSpeed;

		public const float LongJumpLapse = 1f;

		public const float ShortJumpLapse = 0.01f;

		private WaitForSeconds _shortLapseWaiting;

		private WaitForSeconds _longTimeJumpWaiting;
	}
}
