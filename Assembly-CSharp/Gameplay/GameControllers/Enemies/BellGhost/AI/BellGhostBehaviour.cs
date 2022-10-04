using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.BellGhost.Animator;
using Gameplay.GameControllers.Enemies.BellGhost.Attack;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.BellGhost.AI
{
	public class BellGhostBehaviour : EnemyBehaviour
	{
		public bool IsAwake { get; set; }

		public bool IsAppear { get; private set; }

		public bool Asleep { get; private set; }

		public Vector3 Origin { get; set; }

		public bool IsOutReach { get; set; }

		public bool IsBackToOrigin { get; set; }

		public bool IsRamming { get; set; }

		public BellGhostAnimatorInyector AnimatorInyector { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this._bellGhost = (BellGhost)this.Entity;
			this.AnimatorInyector = this._bellGhost.GetComponentInChildren<BellGhostAnimatorInyector>();
			this.minMaxOffsetX = new Vector2(-4f, 4f);
			this.minMaxOffsetY = new Vector2(-5f, 5f);
			this.randomOffset = new Vector2(UnityEngine.Random.Range(this.minMaxOffsetX.x, this.minMaxOffsetX.y), UnityEngine.Random.Range(this.minMaxOffsetY.x, this.minMaxOffsetY.y));
		}

		private void OnLerpStop()
		{
			if (this._bellGhost.IsAttacking)
			{
				this._bellGhost.IsAttacking = false;
			}
			this._bellGhost.GhostTrail.EnableGhostTrail = false;
			this._bellGhost.FloatingMotion.IsFloating = true;
			this.IsBackToOrigin = false;
			this.IsRamming = false;
			if (this.bellGhostVariant == BELL_GHOST_TYPES.RAMMING)
			{
				this._bellGhost.Audio.StopAttack(true);
				this._bellGhost.Audio.StopChargeAttack();
			}
		}

		public override void OnStart()
		{
			base.OnStart();
			this.SetRndTimeAttack();
			base.BehaviourTree = this._bellGhost.GetComponent<BehaviourTreeOwner>();
			this._bellGhost.OnDeath += this.BellGhostOnEntityDie;
			MotionLerper motionLerper = this._bellGhost.MotionLerper;
			motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			if (this._bellGhost.TargetDistance > this._bellGhost.ActivationRange)
			{
				this.Disappear(0f);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this._bellGhost.Target == null)
			{
				return;
			}
			this._chargingAttack = false;
			if (this.IsAwake)
			{
				this.IsOutReach = (this.CheckDistanceToOrigin() >= this.MaxDistanceToOrigin);
				if (Core.Logic.CurrentState == LogicStates.PlayerDead && this._bellGhost.BellGhostAttack != null)
				{
					this._bellGhost.BellGhostAttack.EnableWeaponAreaCollider = false;
					this.SetAsleep();
				}
				if (this.bellGhostVariant == BELL_GHOST_TYPES.RAMMING)
				{
					this._chargingAttack = this.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("ReadyAttack");
				}
			}
			if (this.bellGhostVariant == BELL_GHOST_TYPES.BULLET)
			{
				if (this._time < this._attackTime)
				{
					this._time += Time.deltaTime;
				}
				if (base.IsAttacking)
				{
					this.MoveWhileAttacking();
				}
			}
			else if (base.IsChasing && !this._bellGhost.Status.Dead)
			{
				this._time += Time.deltaTime;
				if (this._time < this._attackTime)
				{
					return;
				}
				this.SetRndTimeAttack();
				if (this._bellGhost.Status.IsVisibleOnCamera)
				{
					this._bellGhost.IsAttacking = true;
				}
				else if (this._chargingAttack)
				{
					this._bellGhost.IsAttacking = true;
				}
				else
				{
					this._bellGhost.IsAttacking = false;
				}
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
			if (this.AnimatorInyector.IsFading || !this._bellGhost.gameObject.activeSelf || !this._bellGhost.Target)
			{
				return;
			}
			if (this._bellGhost.SpriteRenderer.color.a < 1f && !base.IsChasing)
			{
				this.AnimatorInyector.Fade(1f, 0.3f);
				this._bellGhost.Audio.Appear();
			}
			if (this._target == null)
			{
				this._target = this._bellGhost.Target.transform;
			}
			if (this.bellGhostVariant == BELL_GHOST_TYPES.BULLET)
			{
				this.LookAtTarget(this._target.position);
			}
			base.IsChasing = true;
			float num = Vector2.Distance(base.transform.position, this._target.position);
			if (num > this.MinTargetDistance)
			{
				float num2 = Mathf.Sign(this._target.position.x - base.transform.position.x);
				Vector2 v;
				if (this.bellGhostVariant == BELL_GHOST_TYPES.BULLET)
				{
					v = new Vector3(this._target.position.x + num2 * 8f + this.randomOffset.x, this._target.position.y + 2f + this.randomOffset.y);
				}
				else
				{
					v = new Vector3(this._target.position.x + num2, this._target.position.y + 5f);
				}
				base.transform.position = Vector3.SmoothDamp(base.transform.position, v, ref this._velocity, this.ChasingElongation, this.Speed);
			}
			else if (num < this.FleeDistance && this.bellGhostVariant == BELL_GHOST_TYPES.BULLET)
			{
				int num3 = (this.Entity.Status.Orientation != EntityOrientation.Left) ? -1 : 1;
				Vector3 target = new Vector3(this._target.position.x + (float)(num3 * 8) + this.randomOffset.x, this._target.position.y + 1f + this.randomOffset.y);
				base.transform.position = Vector3.SmoothDamp(base.transform.position, target, ref this._velocity, this.ChasingElongation, this.Speed * 0.5f);
			}
			this._bellGhost.Audio.UpdateFloatingPanning();
		}

		private void MoveWhileAttacking()
		{
			float num = Vector2.Distance(base.transform.position, this._target.position);
			if (num < this.FleeDistance * 3f)
			{
				int num2 = (this.Entity.Status.Orientation != EntityOrientation.Left) ? -1 : 1;
				Vector3 target = new Vector3(this._target.position.x + (float)(num2 * 8), this._target.position.y + 1f);
				base.transform.position = Vector3.SmoothDamp(base.transform.position, target, ref this._velocity, this.ChasingElongation, this.Speed * 0.75f);
			}
		}

		public float GetDistanceToTarget()
		{
			this._target = base.GetTarget();
			if (this._target == null)
			{
				return 1000f;
			}
			return Vector3.Distance(this._target.position, base.transform.position);
		}

		public bool IsTargetInsideShootingRange()
		{
			return this.GetDistanceToTarget() < this.MinTargetDistance;
		}

		public bool CanShoot()
		{
			return this.IsTargetInsideShootingRange() && !this.AttackOnCooldown();
		}

		public float CheckDistanceToOrigin()
		{
			return Vector3.Distance(base.transform.position, this.Origin);
		}

		public override void Attack()
		{
			if (this._target == null)
			{
				return;
			}
			BELL_GHOST_TYPES bell_GHOST_TYPES = this.bellGhostVariant;
			if (bell_GHOST_TYPES != BELL_GHOST_TYPES.RAMMING)
			{
				if (bell_GHOST_TYPES == BELL_GHOST_TYPES.BULLET)
				{
					this.BulletVariantAttack();
				}
			}
			else
			{
				this.RammingVariantAttack();
			}
		}

		private void RammingVariantAttack()
		{
			this._bellGhost.Audio.UpdateAttackPanning();
			if (this._bellGhost.MotionLerper.IsLerping)
			{
				return;
			}
			if (!this.IsRamming)
			{
				this._bellGhost.Audio.PlayAttack();
				this._bellGhost.Audio.StopFloating();
				this.IsRamming = true;
				this._bellGhost.FloatingMotion.IsFloating = false;
				this.LookAtTarget(this._target.position);
				this.AnimatorInyector.Attack();
			}
		}

		private void BulletVariantAttack()
		{
			if (base.IsAttacking)
			{
				return;
			}
			this._bellGhost.Audio.StopFloating();
			this._bellGhost.FloatingMotion.IsFloating = true;
			this.LookAtTarget(this._target.position);
			this.AnimatorInyector.Attack();
		}

		public bool AttackOnCooldown()
		{
			return this._time < this._attackTime;
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public void Appear(float time)
		{
			this.SetColliderScale();
			if (this.IsAppear)
			{
				return;
			}
			this.IsAppear = true;
			this.IsAwake = true;
			this.AnimatorInyector.Appear(time);
			this._bellGhost.Audio.Appear();
		}

		public void Disappear(float time)
		{
			this.IsAppear = false;
			this.IsAwake = false;
			this.AnimatorInyector.Disappear(time);
			this._bellGhost.Audio.Dissapear();
		}

		public void MoveToOrigin()
		{
			if (!this.IsBackToOrigin)
			{
				this.IsBackToOrigin = true;
				base.IsChasing = false;
				this.SetRndTimeAttack();
				base.StartCoroutine(this.MoveToOriginCoroutine());
			}
		}

		public void HurtDisplacement(GameObject attackingEntity)
		{
			if (this._bellGhost.MotionLerper.IsLerping)
			{
				this._bellGhost.MotionLerper.StopLerping();
			}
			Vector2 v = (attackingEntity.transform.position.x < base.transform.position.x) ? Vector2.right : (-Vector2.right);
			this._bellGhost.GhostTrail.EnableGhostTrail = true;
			this._bellGhost.MotionLerper.distanceToMove = 3f;
			this._bellGhost.MotionLerper.TimeTakenDuringLerp = 0.5f;
			this._bellGhost.MotionLerper.StartLerping(v);
		}

		private IEnumerator MoveToOriginCoroutine()
		{
			float disappearTime = 1f;
			this.Disappear(disappearTime);
			yield return new WaitForSeconds(disappearTime);
			base.transform.position = this.Origin;
			this.IsOutReach = false;
			this.IsAppear = false;
			yield break;
		}

		public void Ramming()
		{
			this.LookAtTarget(this._target.position);
			this._bellGhost.GhostTrail.EnableGhostTrail = true;
			float num = Vector2.Distance(base.transform.position, this._target.position) * 1.5f;
			Vector3 normalized = (this._target.position - base.transform.position).normalized;
			this._bellGhost.MotionLerper.distanceToMove = num;
			this._bellGhost.MotionLerper.TimeTakenDuringLerp = num / this.AttackSpeed;
			this._bellGhost.MotionLerper.StartLerping(normalized);
		}

		public void Shoot()
		{
			((BellGhostVariantAttack)this.bulletAttack).target = this._target;
			this.bulletAttack.CurrentWeaponAttack();
			this.OnBulletShot();
		}

		private void OnBulletShot()
		{
			this._bellGhost.FloatingMotion.IsFloating = true;
			this.IsBackToOrigin = false;
			this.SetRndTimeAttack();
		}

		private void SetRndTimeAttack()
		{
			this._time = 0f;
			this._attackTime = UnityEngine.Random.Range(this.MinRndTimeAttack, this.MaxRndTimeAttack);
		}

		private void BellGhostOnEntityDie()
		{
			this._bellGhost.OnDeath -= this.BellGhostOnEntityDie;
			if (this._bellGhost.AttackArea != null)
			{
				this._bellGhost.AttackArea.WeaponCollider.enabled = false;
			}
			this._bellGhost.EntityDamageArea.DamageAreaCollider.enabled = false;
			if (base.BehaviourTree.isRunning)
			{
				base.BehaviourTree.StopBehaviour();
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.Entity.Status.Dead || this.AnimatorInyector.IsTurning)
			{
				return;
			}
			this.SetColliderScale();
			if (this.Entity.transform.position.x >= targetPos.x + 1f)
			{
				if (this.Entity.Status.Orientation != EntityOrientation.Left)
				{
					if (this.OnTurning != null)
					{
						this.OnTurning();
					}
					this._bellGhost.SetOrientation(EntityOrientation.Left, false, false);
				}
			}
			else if (this.Entity.transform.position.x < targetPos.x - 1f && this.Entity.Status.Orientation != EntityOrientation.Right)
			{
				if (this.OnTurning != null)
				{
					this.OnTurning();
				}
				this._bellGhost.SetOrientation(EntityOrientation.Right, false, false);
			}
		}

		private void SetColliderScale()
		{
			if (this.bellGhostVariant == BELL_GHOST_TYPES.BULLET)
			{
				this._bellGhost.EntityDamageArea.DamageAreaCollider.transform.localScale = new Vector3((float)((this._bellGhost.Status.Orientation != EntityOrientation.Right) ? -1 : 1), 1f, 1f);
			}
		}

		public void SetAsleep()
		{
			if (!this.Asleep)
			{
				this.Asleep = true;
			}
			if (base.BehaviourTree.isRunning)
			{
				base.BehaviourTree.StopBehaviour();
			}
			this.MoveToOrigin();
		}

		private void OnDestroy()
		{
			if (this._bellGhost)
			{
				MotionLerper motionLerper = this._bellGhost.MotionLerper;
				motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			}
		}

		private float _attackTime;

		private BellGhost _bellGhost;

		private bool _chargingAttack;

		private Transform _target;

		private float _time;

		private Vector3 _velocity = Vector3.zero;

		public BELL_GHOST_TYPES bellGhostVariant;

		public EnemyAttack bulletAttack;

		public float ChasingElongation = 0.5f;

		public float ElapseTimeBeforeRamming = 0.25f;

		public float MaxDistanceToOrigin = 10f;

		public float MaxRndTimeAttack = 6f;

		public float MinRndTimeAttack = 3f;

		public float MinTargetDistance = 1f;

		public float FleeDistance = 2f;

		public float Speed = 5f;

		public float AttackSpeed = 10f;

		private Vector2 randomOffset;

		private Vector2 minMaxOffsetX;

		private Vector2 minMaxOffsetY;
	}
}
