using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.WheelCarrier.IA
{
	public class WheelCarrierBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public WheelCarrier WheelCarrier { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.WheelCarrier = (WheelCarrier)this.Entity;
		}

		public override void OnStart()
		{
			base.OnStart();
			if (Core.Logic.Penitent != null)
			{
				Core.Logic.Penitent.OnDeath += this.PlayerOnDeath;
			}
		}

		private void Update()
		{
			if (base.IsDead() || base.GotParry)
			{
				return;
			}
			this.DistanceToTarget = Vector2.Distance(this.WheelCarrier.transform.position, this.WheelCarrier.Target.transform.position);
			if (this.DistanceToTarget < this.ActivationDistance && this.TargetCanBeVisible())
			{
				if (this.DistanceToTarget > this.MinAttackDistance)
				{
					Vector3 position = this.WheelCarrier.Target.transform.position;
					this.Chase(position);
				}
				else
				{
					this.Attack();
				}
			}
			else
			{
				this.Idle();
			}
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public bool TargetCanBeVisible()
		{
			return this.WheelCarrier.VisionCone.CanSeeTarget(this.WheelCarrier.Target.transform, "Penitent", false);
		}

		public void StartVulnerablePeriod()
		{
			this.WheelCarrier.VulnerablePeriod.StartVulnerablePeriod(this.WheelCarrier);
		}

		public void Chase(Vector3 position)
		{
			if (base.IsAttacking || base.IsHurt || !this.WheelCarrier.MotionChecker.HitsFloor)
			{
				this.StopMovement();
				return;
			}
			this.LookAtTarget(position);
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			if (this.canWalk)
			{
				this.WheelCarrier.Input.HorizontalInput = horizontalInput;
				this.WheelCarrier.AnimatorInyector.Walk();
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.WheelCarrier.transform.position.x < targetPos.x)
			{
				if (this.WheelCarrier.Status.Orientation != EntityOrientation.Right)
				{
					this.WheelCarrier.SetOrientation(EntityOrientation.Right, true, false);
				}
			}
			else if (this.WheelCarrier.Status.Orientation != EntityOrientation.Left)
			{
				this.WheelCarrier.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public override void Damage()
		{
		}

		public void Death()
		{
			this.StopMovement();
			this.WheelCarrier.AnimatorInyector.Death();
		}

		private void PlayerOnDeath()
		{
		}

		public void ResetCoolDown()
		{
			if (this.AttackLapse > 0f)
			{
				this.AttackLapse = 0f;
			}
		}

		public override void Attack()
		{
			this.StopMovement();
			this.AttackLapse += Time.deltaTime;
			if (this.AttackLapse < this.AttackCoolDown)
			{
				return;
			}
			this.AttackLapse = 0f;
			this.WheelCarrier.AnimatorInyector.Attack();
		}

		public override void Parry()
		{
			base.Parry();
			base.GotParry = true;
			this.WheelCarrier.AnimatorInyector.ParryReaction();
		}

		public override void Execution()
		{
			base.Execution();
			this.isExecuted = true;
			base.GotParry = true;
			this.StopMovement();
			this.WheelCarrier.gameObject.layer = LayerMask.NameToLayer("Default");
			this.WheelCarrier.SpriteRenderer.enabled = false;
			Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
			this.WheelCarrier.EntExecution.InstantiateExecution();
			this.WheelCarrier.Attack.gameObject.SetActive(false);
			if (this.WheelCarrier.EntExecution != null)
			{
				this.WheelCarrier.EntExecution.enabled = true;
			}
		}

		public override void Alive()
		{
			base.Alive();
			this.WheelCarrier.gameObject.layer = LayerMask.NameToLayer("Enemy");
			base.GotParry = false;
			this.WheelCarrier.SpriteRenderer.enabled = true;
			this.WheelCarrier.Attack.gameObject.SetActive(true);
			this.WheelCarrier.Animator.Play("Idle");
			this.WheelCarrier.CurrentLife = this.WheelCarrier.Stats.Life.Base / 2f;
			if (this.WheelCarrier.EntExecution != null)
			{
				this.WheelCarrier.EntExecution.enabled = false;
			}
		}

		public override void StopMovement()
		{
			this.WheelCarrier.AnimatorInyector.Stop();
			this.WheelCarrier.Input.HorizontalInput = 0f;
		}

		private void OnDestroy()
		{
			Core.Logic.Penitent.OnDeath -= this.PlayerOnDeath;
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void ReadSpawnerConfig(SpawnBehaviourConfig config)
		{
			base.ReadSpawnerConfig(config);
			this.canWalk = !config.dontWalk;
		}

		[FoldoutGroup("Attack Settings", true, 0)]
		protected float AttackLapse;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackCoolDown = 2f;

		private bool isExecuted;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MaxVisibleHeight = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MinAttackDistance = 2f;

		public bool canWalk = true;
	}
}
