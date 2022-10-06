using System;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Roller.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Roller.AI
{
	public class AxeRollerBehaviour : EnemyBehaviour
	{
		public AxeRoller Roller { get; private set; }

		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public bool IsRolling { get; private set; }

		public bool IsChargingAttack { get; set; }

		public bool IsHurting { get; set; }

		public override void OnStart()
		{
			base.OnStart();
			this.Roller = (AxeRoller)this.Entity;
			this.results = new RaycastHit2D[2];
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Roller == null)
			{
				return;
			}
			if (this.Roller.Status.Dead || this.isExecuted || this.IsHurting)
			{
				return;
			}
			this.DistanceToTarget = Vector2.Distance(this.Entity.transform.position, base.GetTarget().position);
			if (this.DistanceToTarget <= this.EscapeDistance && !this.IsRolling && !this.IsChargingAttack && this.currentCoolDown <= 0f && this.CanSeePenitent() && this.CanRoll())
			{
				this.AnticipateRoll();
				return;
			}
			if (this.IsRolling)
			{
				this.currentRollingTime += Time.deltaTime;
				if (this.HasReachedMaxTimeRolling() || !this.CanRoll())
				{
					if (this.HasAnotherRollerBehind())
					{
						this.RollBackwards();
					}
					else
					{
						this.StopMovement();
						this.IsChargingAttack = false;
						this.LookAtTarget(base.GetTarget().position);
					}
				}
			}
			if (this.IsChargingAttack || this.CanSeePenitent())
			{
				this.currentCoolDown += Time.deltaTime;
				if (this.currentCoolDown >= this.GetProjectileCoolDown())
				{
					this.currentCoolDown = 0f;
					this.IsChargingAttack = false;
					this.Roller.IsAttacking = true;
					this.Roller.AnimatorInjector.Attack();
					this.hasAlreadyAttackedBefore = true;
				}
			}
			if (this.CanSeePenitent() && !this.IsRolling)
			{
				this.LookAtTarget(base.GetTarget().position);
			}
		}

		private float GetProjectileCoolDown()
		{
			return (!this.hasAlreadyAttackedBefore) ? this.FirstProjectileWaitTime : this.ProjectileCoolDown;
		}

		private bool HasAnotherRollerBehind()
		{
			return Physics2D.RaycastNonAlloc(base.transform.position, Vector2.down, this.results, 0.1f, this.keepRollingLayerMask) > 1;
		}

		private bool CanRoll()
		{
			return !this.Roller.MotionChecker.HitsBlock && this.Roller.MotionChecker.HitsFloor;
		}

		public bool CanSeePenitent()
		{
			return this.Roller.VisionCone.CanSeeTarget(base.GetTarget(), "Penitent", false);
		}

		public void AnticipateRoll()
		{
			if (this.Roller.Input == null || this.IsRolling)
			{
				return;
			}
			this.Roller.Audio.PlayAnticipateRoll();
			this.IsRolling = true;
			this.Roller.AnimatorInjector.Rolling(true);
		}

		public void Roll()
		{
			if (this.Roller.Input == null)
			{
				return;
			}
			this.Roller.GhostTrailGenerator.EnableGhostTrail = true;
			AxeRollerMeleeAttack rollingAttack = this.Roller.RollingAttack;
			rollingAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Combine(rollingAttack.OnAttackGuarded, new Core.SimpleEvent(this.RollBackwards));
			this.Roller.Input.HorizontalInput = ((this.Roller.Status.Orientation != EntityOrientation.Right) ? -1f : 1f);
			this.Roller.Audio.PlayRolling();
			this.Roller.DamageByContact = false;
			this.Roller.RollingAttack.damageOnEnterArea = true;
			this.Roller.RollingAttack.CurrentWeaponAttack();
		}

		private void RollBackwards()
		{
			this.currentRollingTime = this.MaxRollingTime - 0.5f + Random.Range(-0.1f, 0.1f);
			this.Roller.Input.HorizontalInput = ((this.Roller.Status.Orientation != EntityOrientation.Right) ? 1f : -1f);
			if (this.Roller.MotionChecker.RangeBlockDetection != -6f)
			{
				this.normalRangeBlockDetection = this.Roller.MotionChecker.RangeBlockDetection;
				this.Roller.MotionChecker.RangeBlockDetection = -6f;
			}
		}

		public void HitDisplacement(Vector3 attakingEntityPos)
		{
			if (this.IsRolling || base.IsAttacking || this.HasReachedMaxHitReactions())
			{
				return;
			}
			this.IsHurting = true;
			this.currentHitReactionsDone++;
			this.Roller.AnimatorInjector.Damage();
			float num = (this.Entity.transform.position.x < attakingEntityPos.x) ? (-this.HurtDisplacement) : this.HurtDisplacement;
			TweenSettingsExtensions.OnComplete<Tweener>(ShortcutExtensions.DOMoveX(this.Roller.transform, this.Roller.transform.position.x + num, 0.55f, false), delegate()
			{
				this.IsHurting = false;
			});
		}

		public void RollIfCantSeePenitent()
		{
			if (this.IsRolling || base.IsAttacking || this.IsHurting || this.CanSeePenitent())
			{
				return;
			}
			if (!this.CanRoll())
			{
				this.Roller.SetOrientation((this.Roller.Status.Orientation != EntityOrientation.Right) ? EntityOrientation.Right : EntityOrientation.Left, true, false);
			}
			this.AnticipateRoll();
		}

		private bool HasReachedMaxHitReactions()
		{
			return this.currentHitReactionsDone >= this.MaxHitReactions;
		}

		public override void StopMovement()
		{
			AxeRollerMeleeAttack rollingAttack = this.Roller.RollingAttack;
			rollingAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(rollingAttack.OnAttackGuarded, new Core.SimpleEvent(this.RollBackwards));
			this.IsRolling = false;
			this.currentRollingTime = 0f;
			this.Roller.Input.HorizontalInput = 0f;
			this.Roller.Controller.PlatformCharacterPhysics.Velocity = Vector3.zero;
			this.Roller.AnimatorInjector.Rolling(false);
			this.Roller.Audio.StopRolling();
			this.Roller.GhostTrailGenerator.EnableGhostTrail = false;
			this.Roller.DamageByContact = true;
			this.Roller.RollingAttack.damageOnEnterArea = false;
			if (this.Roller.MotionChecker.RangeBlockDetection == -6f)
			{
				this.Roller.MotionChecker.RangeBlockDetection = this.normalRangeBlockDetection;
			}
		}

		public void OnDisable()
		{
			if (this.Roller && this.Roller.Audio)
			{
				this.Roller.Audio.StopRolling();
			}
		}

		public void StopAttackIfPenitentIsTooFar()
		{
			if (this.CanSeePenitent())
			{
				return;
			}
			this.Roller.IsAttacking = false;
			this.Roller.AnimatorInjector.StopAttack();
		}

		private bool HasReachedMaxTimeRolling()
		{
			return this.currentRollingTime >= this.MaxRollingTime;
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
			this.Roller.AxeAttack.enabled = false;
			this.Roller.RollingAttack.enabled = false;
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
			this.Roller.AxeAttack.enabled = true;
			this.Roller.RollingAttack.enabled = true;
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

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float EscapeDistance;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float FirstProjectileWaitTime = 1f;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ProjectileCoolDown = 1f;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float MaxRollingTime;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private LayerMask keepRollingLayerMask;

		[FoldoutGroup("Hurt", 0)]
		[Tooltip("Displacement when the enemy is hit")]
		public float HurtDisplacement = 1f;

		[FoldoutGroup("Hurt", 0)]
		[Tooltip("Max number of hit reactions")]
		public int MaxHitReactions = 3;

		public const float HurtAnimDuration = 0.55f;

		private const float BackwardsRangeBlockDetection = -6f;

		private float currentCoolDown;

		private bool isExecuted;

		private float currentRollingTime;

		private int currentHitReactionsDone;

		private RaycastHit2D[] results;

		private bool hasAlreadyAttackedBefore;

		private float normalRangeBlockDetection;
	}
}
