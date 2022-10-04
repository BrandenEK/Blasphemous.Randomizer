using System;
using System.Collections;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Framework.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.PontiffHusk.Animator;
using Gameplay.GameControllers.Entities;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.PontiffHusk.AI
{
	public class PontiffHuskMeleeBehaviour : EnemyBehaviour
	{
		public bool IsAwake { get; set; }

		public bool IsAppear { get; set; }

		public bool Asleep { get; private set; }

		public Vector3 Origin { get; set; }

		public bool IsOutReach { get; set; }

		public bool IsBackToOrigin { get; set; }

		public bool IsRamming { get; set; }

		public PontiffHuskAnimatorInyector AnimatorInyector { get; private set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this._PontiffHuskMelee = (PontiffHuskMelee)this.Entity;
			this.AnimatorInyector = this._PontiffHuskMelee.GetComponentInChildren<PontiffHuskAnimatorInyector>();
			this.minMaxOffsetX = new Vector2(-4f, 4f);
			this.minMaxOffsetY = new Vector2(-5f, 5f);
			this.randomOffset = new Vector2(UnityEngine.Random.Range(this.minMaxOffsetX.x, this.minMaxOffsetX.y), UnityEngine.Random.Range(this.minMaxOffsetY.x, this.minMaxOffsetY.y));
		}

		private void OnLerpStop()
		{
			if (this._PontiffHuskMelee.IsAttacking)
			{
				this._PontiffHuskMelee.IsAttacking = false;
			}
			this._PontiffHuskMelee.FloatingMotion.IsFloating = true;
			this.IsBackToOrigin = false;
			this.IsRamming = false;
			this._PontiffHuskMelee.Audio.StopAttack(true);
			this._PontiffHuskMelee.Audio.StopChargeAttack();
		}

		public override void OnStart()
		{
			base.OnStart();
			this.SetRndTimeAttack();
			base.BehaviourTree = this._PontiffHuskMelee.GetComponent<BehaviourTreeOwner>();
			this._PontiffHuskMelee.OnDeath += this.PontiffHuskMeleeOnEntityDie;
			MotionLerper motionLerper = this._PontiffHuskMelee.MotionLerper;
			motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Combine(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			if (this._PontiffHuskMelee.TargetDistance > this._PontiffHuskMelee.ActivationRange)
			{
				this.Disappear(0f);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this._PontiffHuskMelee.Target == null)
			{
				return;
			}
			this._chargingAttack = false;
			if (this.IsAwake)
			{
				this.IsOutReach = (this.CheckDistanceToOrigin() >= this.MaxDistanceToOrigin);
				if (Core.Logic.CurrentState == LogicStates.PlayerDead && this._PontiffHuskMelee.PontiffHuskMeleeAttack != null)
				{
					this._PontiffHuskMelee.PontiffHuskMeleeAttack.EnableWeaponAreaCollider = false;
					this.SetAsleep();
				}
				this._chargingAttack = this.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("ReadyAttack");
				if (!this.IsAppear && !this.AnimatorInyector.IsFading)
				{
					this._disappearedTime += Time.deltaTime;
				}
			}
			else
			{
				this._PontiffHuskMelee.PontiffHuskMeleeAttack.EnableWeaponAreaCollider = false;
			}
			if (base.IsChasing && !this._PontiffHuskMelee.Status.Dead)
			{
				if (!this.IsAppear)
				{
					return;
				}
				this._time += Time.deltaTime;
				if (this._time < this._attackTime)
				{
					return;
				}
				this.SetRndTimeAttack();
				if (this._PontiffHuskMelee.Status.IsVisibleOnCamera)
				{
					this._PontiffHuskMelee.IsAttacking = true;
				}
				else if (this._chargingAttack)
				{
					this._PontiffHuskMelee.IsAttacking = true;
				}
				else
				{
					this._PontiffHuskMelee.IsAttacking = false;
				}
			}
			if (this.AnimatorInyector.IsFading)
			{
				this._PontiffHuskMelee.PontiffHuskMeleeAttack.EnableWeaponAreaCollider = false;
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
			if (this.AnimatorInyector.IsFading || !this._PontiffHuskMelee.gameObject.activeSelf || !this._PontiffHuskMelee.Target)
			{
				return;
			}
			if (this._PontiffHuskMelee.SpriteRenderer.color.a < 1f && !base.IsChasing)
			{
				this.AnimatorInyector.Fade(1f, 0.3f);
				this._PontiffHuskMelee.Audio.Appear();
			}
			if (this._target == null)
			{
				this._target = this._PontiffHuskMelee.Target.transform;
			}
			base.IsChasing = true;
			float num = Vector2.Distance(base.transform.position, this._target.position);
			if (num > this.MinTargetDistance)
			{
				float num2 = (this.Entity.Status.Orientation != EntityOrientation.Left) ? (-this.ChaseHorizontalOffset) : this.ChaseHorizontalOffset;
				Vector2 v = new Vector3(this._target.position.x + num2, this._target.position.y + this.ChaseVerticalOffset);
				base.transform.position = Vector3.SmoothDamp(base.transform.position, v, ref this._velocity, this.ChasingElongation, this.Speed);
			}
			this._PontiffHuskMelee.Audio.UpdateFloatingPanning();
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
			this.RammingVariantAttack();
		}

		private void RammingVariantAttack()
		{
			this._PontiffHuskMelee.Audio.UpdateAttackPanning();
			if (this._PontiffHuskMelee.MotionLerper.IsLerping)
			{
				return;
			}
			if (!this.IsRamming)
			{
				this._PontiffHuskMelee.Audio.PlayAttack();
				this._PontiffHuskMelee.Audio.StopFloating();
				this.IsRamming = true;
				this._PontiffHuskMelee.FloatingMotion.IsFloating = false;
				this.LookAtTarget(this._target.position);
				this.AnimatorInyector.Attack();
			}
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
			if (this._disappearedTime < this.MinTimeDisappeared && this.IsAwake)
			{
				return;
			}
			if (this._PontiffHuskMelee.Target == null)
			{
				this._PontiffHuskMelee.Target = Core.Logic.Penitent.gameObject;
			}
			if (this._target == null)
			{
				this._target = this._PontiffHuskMelee.Target.transform;
			}
			this.LookAtTarget(this._target.position, true);
			this.SetColliderScale();
			if (this.IsAppear)
			{
				return;
			}
			this.IsAppear = true;
			this.IsAwake = true;
			this.AnimatorInyector.Appear(time);
			this._PontiffHuskMelee.Audio.Appear();
		}

		public void Disappear(float time)
		{
			this.IsAppear = false;
			this.IsAwake = false;
			this.AnimatorInyector.Disappear(time);
			this._PontiffHuskMelee.Audio.Dissapear();
			this._disappearedTime = 0f;
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
			float rammingDistance = this.RammingDistance;
			Vector2 v = (this.Entity.Status.Orientation != EntityOrientation.Left) ? Vector2.right : Vector2.left;
			this._PontiffHuskMelee.MotionLerper.distanceToMove = rammingDistance;
			this._PontiffHuskMelee.MotionLerper.TimeTakenDuringLerp = rammingDistance / this.AttackSpeed;
			this._PontiffHuskMelee.MotionLerper.StartLerping(v);
		}

		private void SetRndTimeAttack()
		{
			this._time = 0f;
			this._attackTime = UnityEngine.Random.Range(this.MinRndTimeAttack, this.MaxRndTimeAttack);
		}

		private void PontiffHuskMeleeOnEntityDie()
		{
			this._PontiffHuskMelee.OnDeath -= this.PontiffHuskMeleeOnEntityDie;
			if (this._PontiffHuskMelee.AttackArea != null)
			{
				this._PontiffHuskMelee.AttackArea.WeaponCollider.enabled = false;
			}
			this._PontiffHuskMelee.EntityDamageArea.DamageAreaCollider.enabled = false;
			if (base.BehaviourTree.isRunning)
			{
				base.BehaviourTree.StopBehaviour();
			}
		}

		public void LookAtTarget(Vector3 targetPos, bool force)
		{
			if (!force && this.CheckLookAtTargetConditions())
			{
				return;
			}
			this.SetColliderScale();
			this.SetOrientation(targetPos);
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (!this.CheckLookAtTargetConditions())
			{
				return;
			}
			this.SetColliderScale();
			this.SetOrientation(targetPos);
		}

		private bool CheckLookAtTargetConditions()
		{
			return !this.Entity.Status.Dead && !this.AnimatorInyector.IsFading && !base.IsAttacking && this.IsAppear;
		}

		public void SetOrientation(Vector3 targetPos)
		{
			if (this.Entity.transform.position.x >= targetPos.x + 1f)
			{
				if (this.Entity.Status.Orientation != EntityOrientation.Left)
				{
					this._PontiffHuskMelee.SetOrientation(EntityOrientation.Left, false, false);
				}
			}
			else if (this.Entity.transform.position.x < targetPos.x - 1f && this.Entity.Status.Orientation != EntityOrientation.Right)
			{
				this._PontiffHuskMelee.SetOrientation(EntityOrientation.Right, false, false);
			}
		}

		private void SetColliderScale()
		{
			this._PontiffHuskMelee.EntityDamageArea.DamageAreaCollider.transform.localScale = new Vector3((float)((this._PontiffHuskMelee.Status.Orientation != EntityOrientation.Right) ? -1 : 1), 1f, 1f);
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
			if (this._PontiffHuskMelee)
			{
				MotionLerper motionLerper = this._PontiffHuskMelee.MotionLerper;
				motionLerper.OnLerpStop = (Core.SimpleEvent)Delegate.Remove(motionLerper.OnLerpStop, new Core.SimpleEvent(this.OnLerpStop));
			}
		}

		private float _attackTime;

		private PontiffHuskMelee _PontiffHuskMelee;

		private bool _chargingAttack;

		private Transform _target;

		private float _time;

		private Vector3 _velocity = Vector3.zero;

		private float _disappearedTime;

		public EnemyAttack bulletAttack;

		public float ChasingElongation = 0.5f;

		public float ElapseTimeBeforeRamming = 0.25f;

		public float MaxDistanceToOrigin = 10f;

		public float MaxRndTimeAttack = 6f;

		public float MinRndTimeAttack = 3f;

		public float MinTargetDistance = 1f;

		public float FleeDistance = 2f;

		public float MinTimeDisappeared = 2f;

		public float Speed = 5f;

		public float AttackSpeed = 10f;

		public float RammingDistance = 14f;

		public float ChaseHorizontalOffset = 8f;

		public float ChaseVerticalOffset = -2f;

		private Vector2 randomOffset;

		private Vector2 minMaxOffsetX;

		private Vector2 minMaxOffsetY;
	}
}
