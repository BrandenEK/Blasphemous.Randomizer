using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Roller.AI
{
	public class RollerBehaviour : EnemyBehaviour
	{
		public Roller Roller { get; private set; }

		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public bool IsRolling { get; private set; }

		public bool IsEscaping { get; set; }

		public bool IsShooting { get; set; }

		public bool IsSetRandDir { get; private set; }

		public bool IsCharginAttack { get; set; }

		private float _rndDir { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.Roller = (Roller)this.Entity;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.isInTunnel = this.IsInTunnel();
			if (this.Roller == null)
			{
				return;
			}
			if (this.Roller.Status.Dead || this.isExecuted)
			{
				return;
			}
			this.DistanceToTarget = Vector2.Distance(this.Entity.transform.position, base.GetTarget().position);
			if (this.DistanceToTarget <= this.EscapeDistance && !this.IsEscaping && !this.IsCharginAttack && this._currentCoolDown <= 0f)
			{
				this.IsEscaping = true;
			}
			if (this.IsEscaping)
			{
				if ((this.DistanceToTarget <= this.ShotDistance && !this.ReachMaxTimeRolling) || this.IsInTunnel())
				{
					if (!this.IsSetRandDir)
					{
						this.IsSetRandDir = true;
						this._rndDir = this.GetRandomDir();
					}
					if (this.Roller.MotionChecker.HitsBlock || !this.Roller.MotionChecker.HitsFloor)
					{
						this._rndDir *= -1f;
					}
					this.Roll(this._rndDir);
				}
				else
				{
					this.StopMovement();
					this.IsSetRandDir = false;
					this.IsCharginAttack = true;
					this.IsEscaping = false;
					this.LookAtTarget(base.GetTarget().position);
				}
			}
			if (this.IsCharginAttack || this.VisualSensor.IsColliding())
			{
				this._currentCoolDown += Time.deltaTime;
				if (this._currentCoolDown >= this.ProjectileCoolDown)
				{
					this._currentCoolDown = 0f;
					this.IsCharginAttack = false;
					this.Roller.AnimatorInjector.Attack();
				}
			}
			if (this.HearingSensor.IsColliding() && !this.IsEscaping)
			{
				this.LookAtTarget(base.GetTarget().position);
			}
		}

		private bool IsInTunnel()
		{
			Vector2 a = base.transform.position + Vector2.up * this.tunnelDetectorYOffset;
			int mask = LayerMask.GetMask(new string[]
			{
				"Floor"
			});
			RaycastHit2D hit = Physics2D.Raycast(a + Vector2.right * this.tunnelDetectorRaySeparation / 2f, Vector2.up, this.Roller.MotionChecker.RangeGroundDetection, mask);
			RaycastHit2D hit2 = Physics2D.Raycast(a + Vector2.left * this.tunnelDetectorRaySeparation / 2f, Vector2.up, this.Roller.MotionChecker.RangeGroundDetection, mask);
			bool flag = (hit && hit.normal == Vector2.down) || (hit2 && hit2.normal == Vector2.down);
			return flag && this.Roller.MotionChecker.HitsFloor;
		}

		private void OnDrawGizmosSelected()
		{
			Vector2 a = base.transform.position + Vector2.up * this.tunnelDetectorYOffset;
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(a + Vector2.right * this.tunnelDetectorRaySeparation / 2f, a + Vector2.right * this.tunnelDetectorRaySeparation / 2f + Vector2.up * this.tunnelDetectorRange);
			Gizmos.DrawLine(a + Vector2.left * this.tunnelDetectorRaySeparation / 2f, a + Vector2.left * this.tunnelDetectorRaySeparation / 2f + Vector2.up * this.tunnelDetectorRange);
		}

		public void OnDisable()
		{
			if (this.Roller && this.Roller.Audio)
			{
				this.Roller.Audio.StopRolling();
			}
		}

		public void Roll(float dir)
		{
			if (this.Roller.Input == null)
			{
				return;
			}
			this._currentRollingTime += Time.deltaTime;
			this.Roller.Input.HorizontalInput = dir;
			this.IsRolling = true;
			this.Roller.SetOrientation((dir >= 0f) ? EntityOrientation.Right : EntityOrientation.Left, true, false);
			this.Roller.AnimatorInjector.Rolling(true);
			this.Roller.DamageCollider.enabled = false;
			this.Roller.Audio.PlayRolling();
			this.Roller.DamageByContact = false;
		}

		public override void StopMovement()
		{
			this.IsRolling = false;
			this._currentRollingTime = 0f;
			this.Roller.Input.HorizontalInput = 0f;
			this.Roller.Controller.PlatformCharacterPhysics.Velocity = Vector3.zero;
			this.Roller.AnimatorInjector.Rolling(false);
			this.Roller.DamageCollider.enabled = true;
			this.Roller.DamageByContact = true;
			this.Roller.Audio.StopRolling();
		}

		private float GetRandomDir()
		{
			float f = UnityEngine.Random.Range(-1f, 1f);
			return Mathf.Sign(f);
		}

		private bool ReachMaxTimeRolling
		{
			get
			{
				return this._currentRollingTime >= this.MaxRollingTime;
			}
		}

		public override void Execution()
		{
			base.Execution();
			this.isExecuted = true;
			this.Roller.gameObject.layer = LayerMask.NameToLayer("Default");
			this.Roller.Audio.StopRolling();
			this.Roller.Animator.Play("Idle");
			this.StopMovement();
			this.Roller.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.Roller.Attack.enabled = false;
			this.Roller.EntExecution.InstantiateExecution();
			if (this.Roller.EntExecution != null)
			{
				this.Roller.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.isExecuted = false;
			this.Roller.gameObject.layer = LayerMask.NameToLayer("Enemy");
			this.Roller.SpriteRenderer.enabled = true;
			this.Roller.Animator.Play("Idle");
			this.Roller.CurrentLife = this.Roller.Stats.Life.Base / 2f;
			this.Roller.Attack.enabled = true;
			base.StartBehaviour();
			if (this.Roller.EntExecution != null)
			{
				this.Roller.EntExecution.enabled = false;
			}
		}

		public override void Idle()
		{
		}

		public override void Wander()
		{
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
		public float EscapeDistance;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ShotDistance;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ProjectileCoolDown = 1f;

		public float tunnelDetectorYOffset = 1f;

		public float tunnelDetectorRange = 2f;

		public float tunnelDetectorRaySeparation = 0.25f;

		private float _currentCoolDown;

		private bool isExecuted;

		public bool isInTunnel;

		public float MaxRollingTime = 3.5f;

		private float _currentRollingTime;
	}
}
