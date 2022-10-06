using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using BezierSplines;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua.AI;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua
{
	public class PerpetuaBehaviour : EnemyBehaviour
	{
		public Perpetua Perpetua { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PerpetuaBehaviour> OnActionFinished;

		public override void OnAwake()
		{
			base.OnAwake();
			this.stIntro = new Perpetua_StIntro();
			this.stFlapToPoint = new Perpetua_StFlapToPoint();
			this.stAction = new Perpetua_StAction();
			this.stFollowPlayer = new Perpetua_StFollowPlayer();
			this.stDeath = new Perpetua_StDeath();
			this._fsm = new StateMachine<PerpetuaBehaviour>(this, this.stIntro, null, null);
			this.Perpetua = (Perpetua)this.Entity;
			this.colliders = new Collider2D[1];
			ContactFilter2D contactFilter2D = default(ContactFilter2D);
			contactFilter2D.layerMask = LayerMask.NameToLayer("Penitent");
			contactFilter2D.useLayerMask = true;
			contactFilter2D.useTriggers = true;
			this.contactFilter = contactFilter2D;
			this.collider = base.GetComponent<BoxCollider2D>();
		}

		public override void OnStart()
		{
			base.OnStart();
			this.ChangeBossState(BOSS_STATES.WAITING);
			this.bezierL.transform.SetParent(base.transform.parent);
			this.bezierR.transform.SetParent(base.transform.parent);
			this.dashAttack.OnDashAdvancedEvent += this.OnDashing;
			this.dashAttack.OnDashFinishedEvent += this.OnStopDash;
			this.collider.enabled = false;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.Entity.Status.Dead)
			{
				base.BehaviourTree.StopBehaviour();
				return;
			}
			if (Core.Logic.Penitent)
			{
				this.LookAtTarget(Core.Logic.Penitent.transform.position);
			}
			this._fsm.DoUpdate();
		}

		public void InitIntro()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.IntroSequence()));
		}

		private IEnumerator IntroSequence()
		{
			yield return null;
			Vector3 center = this.Perpetua.PerpetuaPoints.centerPoint.position;
			this.Perpetua.AnimatorInyector.Appear();
			yield return new WaitForSeconds(1f);
			this.collider.enabled = true;
			yield return new WaitForSeconds(1f);
			this.Perpetua.AnimatorInyector.Spell();
			yield return new WaitForSeconds(1f);
			this.LaunchLightningSpear(center);
			yield return new WaitForSeconds(2f);
			base.StartBehaviour();
			this.StartWaitingPeriod(0.1f);
			yield break;
		}

		private void LaunchLightningSpear(Vector3 p)
		{
			this.instantProjectileAttack.Shoot(p + Vector3.up * 16f, Vector2.down);
		}

		private void SetCurrentCoroutine(Coroutine c)
		{
			if (this.currentCoroutine != null)
			{
				base.StopCoroutine(this.currentCoroutine);
			}
			this.currentCoroutine = c;
		}

		private void ChangeBossState(BOSS_STATES newState)
		{
			this.currentState = newState;
		}

		private void StartAttackAction()
		{
			this._fsm.ChangeState(this.stAction);
			this.ChangeBossState(BOSS_STATES.MID_ACTION);
		}

		private void ActionFinished()
		{
			this.ChangeBossState(BOSS_STATES.AVAILABLE_FOR_ACTION);
			if (this.OnActionFinished != null)
			{
				this.OnActionFinished(this);
			}
		}

		public void LaunchAction(PerpetuaBehaviour.Perpetua_ATTACKS atk)
		{
			this._currentConfig = this.config.GetAttack(atk);
			if (atk != PerpetuaBehaviour.Perpetua_ATTACKS.SPEAR_DASH)
			{
				if (atk != PerpetuaBehaviour.Perpetua_ATTACKS.LIGHTNING_STRIKE)
				{
					if (atk == PerpetuaBehaviour.Perpetua_ATTACKS.SINGLE_SPEAR)
					{
						this.IssueSpear();
					}
				}
				else
				{
					this.IssueMultiLightning();
				}
			}
			else
			{
				this.IssueSpearDash();
			}
			this.lastAttack = atk;
		}

		public PerpetuaBehaviour.Perpetua_ATTACKS GetNewAttack()
		{
			PerpetuaBehaviour.Perpetua_ATTACKS[] array = new PerpetuaBehaviour.Perpetua_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<PerpetuaBehaviour.Perpetua_ATTACKS> list = new List<PerpetuaBehaviour.Perpetua_ATTACKS>(array);
			if (list.Count > 1)
			{
				list.Remove(this.lastAttack);
			}
			return list[Random.Range(0, list.Count)];
		}

		public void LaunchRandomAction()
		{
			this.LaunchAction(this.GetNewAttack());
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public IEnumerator WaitForState(State<PerpetuaBehaviour> st)
		{
			while (!this._fsm.IsInState(st))
			{
				yield return null;
			}
			yield break;
		}

		private void StartWaitingPeriod(float seconds)
		{
			this.ChangeBossState(BOSS_STATES.WAITING);
			this.SetCurrentCoroutine(base.StartCoroutine(this.WaitingPeriodCoroutine(seconds, new Action(this.AfterWaitingPeriod))));
			this._fsm.ChangeState(this.stFollowPlayer);
		}

		private IEnumerator WaitingPeriodCoroutine(float seconds, Action callback)
		{
			yield return new WaitForSeconds(seconds);
			callback();
			yield break;
		}

		private void AfterWaitingPeriod()
		{
			this.ActionFinished();
		}

		private void IssueSpearDash()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PrepareSpearDash()));
		}

		private IEnumerator PrepareSpearDash()
		{
			Vector3 targetPos = Core.Logic.Penitent.transform.position + this.DashOffset;
			this.FlapToPoint(targetPos, this.GetDashDir.x < 0f);
			this.Perpetua.AnimatorInyector.Flap(true);
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			this.Perpetua.IsGuarding = true;
			this.Perpetua.AnimatorInyector.Flap(false);
			this.Perpetua.AnimatorInyector.ChargeDash(true);
			yield return new WaitForSeconds(this._currentConfig.anticipationWait);
			this.Perpetua.IsGuarding = false;
			this.Perpetua.AnimatorInyector.Dash();
			this.dashAttack.OnDashFinishedEvent += this.OnDashFinished;
			this.dashAttack.Dash(base.transform, this.GetDashDir, 12f, 0f, false);
			this.AttackOnStartDash();
			yield break;
		}

		private void AttackOnStartDash()
		{
			if (Physics2D.OverlapCollider(this.dashAttack.AttackArea.WeaponCollider, this.contactFilter, this.colliders) > 0)
			{
				this.Perpetua.Target.GetComponentInParent<IDamageable>().Damage(this.dashAttack.BossDashHit);
			}
		}

		private Vector3 GetDashDir
		{
			get
			{
				Vector3 vector = Vector3.right;
				if (Core.Logic.Penitent.transform.position.x < base.transform.position.x)
				{
					vector *= -1f;
				}
				return vector;
			}
		}

		private Vector3 DashOffset
		{
			get
			{
				float num = (this.Entity.Status.Orientation != EntityOrientation.Left) ? (-this.dashOffset.x) : this.dashOffset.x;
				return new Vector2(num, this.dashOffset.y);
			}
		}

		private void OnDashing(float value)
		{
			this.isDashing = true;
		}

		private void OnStopDash()
		{
			this.isDashing = false;
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.isDashing)
			{
				return;
			}
			if (targetPos.x > this.Entity.transform.position.x)
			{
				if (this.Entity.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.Entity.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.Entity.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.Entity.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		private void OnDashFinished()
		{
			this.Perpetua.AnimatorInyector.ChargeDash(false);
			this.StartWaitingPeriod(this._currentConfig.recoveryWait);
		}

		private void IssueMultiLightning()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PrepareSpearSummon()));
		}

		private Vector2 GetSummonPoint()
		{
			Vector2 centerPos = this.Perpetua.PerpetuaPoints.GetCenterPos();
			float num = 5f;
			return centerPos + Vector2.right * (Random.Range(-1f, 1f) * num);
		}

		private IEnumerator PrepareSpearSummon()
		{
			this.isSummoningAttack = true;
			this.Perpetua.AnimatorInyector.Flap(true);
			this.FlapToPoint(this.Perpetua.PerpetuaPoints.GetCenterPos() + Vector2.up * 4f, true);
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			this.Perpetua.AnimatorInyector.Flap(false);
			yield return new WaitForSeconds(this._currentConfig.anticipationWait);
			this.Perpetua.AnimatorInyector.Spell();
			yield return new WaitForSeconds(0.75f);
			this.StartSummonLoop();
			yield break;
		}

		private void StartSummonLoop()
		{
			this._currentRepetitions = 1 + this._currentConfig.repetitions;
			this.SetCurrentCoroutine(base.StartCoroutine(this.SpearSummonLoop()));
		}

		private IEnumerator SpearSummonLoop()
		{
			while (this._currentRepetitions >= 0)
			{
				Vector2 p = this.GetSummonPoint();
				this.summonAttack.SummonAreaOnPoint(p, 0f, 1f, null);
				this._currentRepetitions--;
				yield return new WaitForSeconds(this._currentConfig.waitBetweenRepetitions);
			}
			this.OnSummonFinished();
			yield break;
		}

		private void OnSummonFinished()
		{
			this.isSummoningAttack = false;
			this.StartWaitingPeriod(this._currentConfig.recoveryWait);
		}

		private void IssueSpear()
		{
			this.isSummoningAttack = true;
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PrepareSingleSpearSummon()));
		}

		private IEnumerator PrepareSingleSpearSummon()
		{
			yield return new WaitForSeconds(this._currentConfig.anticipationWait);
			this.Perpetua.AnimatorInyector.Spell();
			yield return new WaitForSeconds(0.5f);
			Vector2 t = base.GetTarget().position;
			yield return new WaitForSeconds(0.25f);
			this.LaunchLightningSpear(t);
			this.OnSingleSpearFinished();
			yield break;
		}

		private void OnSingleSpearFinished()
		{
			this.isSummoningAttack = false;
			this.StartWaitingPeriod(this._currentConfig.recoveryWait);
		}

		public void ResetVelocity()
		{
			this.followPlayerVelocity = Vector2.zero;
		}

		public void ChooseSide()
		{
			float num = Random.Range(0f, 1f);
			float num2 = (float)((num <= 0.5f) ? -1 : 1);
			this.followPlayerOffset = new Vector2(Random.Range(this.followPlayerMinOffset.x, this.followPlayerMaxOffset.x), Random.Range(this.followPlayerMinOffset.y, this.followPlayerMaxOffset.y));
			this.followPlayerOffset.x = Mathf.Abs(this.followPlayerOffset.x) * num2;
		}

		public void FollowPlayer()
		{
			if (this.IsSpelling)
			{
				return;
			}
			Vector2 vector = base.GetTarget().position + this.followPlayerOffset;
			Vector2 vector2 = (vector - base.transform.position).normalized * this.followPlayerAcceleration * Time.deltaTime;
			if (Vector2.Distance(vector, base.transform.position) < this.followBrakeRadius)
			{
				vector2 = -this.followPlayerVelocity * Time.deltaTime * (1f / this.followBrakeSeconds);
			}
			this.followPlayerVelocity += vector2;
			this.followPlayerVelocity = Vector2.ClampMagnitude(this.followPlayerVelocity, this.followPlayerMaxSpeed);
			Vector3 vector3 = this.followPlayerVelocity * Time.deltaTime;
			base.transform.position += vector3;
		}

		private void SetFirstPointToPosition(BezierSpline spline, Vector2 position)
		{
			Vector2 vector = spline.points[1] - spline.points[0];
			spline.points[0] = spline.transform.InverseTransformPoint(position);
			spline.points[1] = spline.points[0] + vector;
		}

		private void SetLastPointToPosition(BezierSpline spline, Vector2 position)
		{
			int num = spline.points.Length - 1;
			Vector2 vector = spline.points[num - 1] - spline.points[num];
			spline.points[num] = spline.transform.InverseTransformPoint(position);
			spline.points[num - 1] = spline.points[num] + vector;
		}

		public void FlapToPoint(Vector2 point, bool rightCurve)
		{
			BezierSpline spline = (!rightCurve) ? this.bezierL : this.bezierR;
			this.SetFirstPointToPosition(spline, base.transform.position);
			this.SetLastPointToPosition(spline, point);
			this.splineFollower.spline = spline;
			this.splineFollower.currentCounter = 0f;
			this.splineFollower.followActivated = true;
			this.splineFollower.OnMovementCompleted += this.OnMovementCompleted;
			this._flapPoint = point;
			this._fsm.ChangeState(this.stFlapToPoint);
		}

		private void OnMovementCompleted()
		{
			this.ChangeToAction();
			this.splineFollower.OnMovementCompleted -= this.OnMovementCompleted;
		}

		public bool IsCloseToFlapPoint(float closeRange = 0.5f)
		{
			float num = Vector2.Distance(base.transform.position, this._flapPoint);
			return num < closeRange;
		}

		public void ActivateSteering(bool enabled)
		{
			this.Perpetua.autonomousAgent.enabled = enabled;
			if (enabled)
			{
				this.Perpetua.autonomousAgent.currentVelocity = Vector3.zero;
			}
		}

		public void ChangeToAction()
		{
			this._fsm.ChangeState(this.stAction);
		}

		private void OnDestroy()
		{
			this.dashAttack.OnDashAdvancedEvent -= this.OnDashing;
			this.dashAttack.OnDashFinishedEvent -= this.OnStopDash;
		}

		public void Death()
		{
			base.StopAllCoroutines();
			this.summonAttack.StopAllCoroutines();
			this.summonAttack.ClearAll();
			this.Perpetua.AnimatorInyector.Death();
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
			throw new NotImplementedException();
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		private PerpetuaAttackConfig _currentConfig;

		private int _currentRepetitions;

		private Vector2 _flapPoint;

		private StateMachine<PerpetuaBehaviour> _fsm;

		public BezierSpline bezierL;

		public BezierSpline bezierR;

		[FoldoutGroup("Design settings", true, 0)]
		public float chargePositioningRandomRadius = 1f;

		public PerpetuaScriptableFightConfig config;

		private Coroutine currentCoroutine;

		public List<PerpetuaBehaviour.Perpetua_ATTACKS> currentlyAvailableAttacks;

		[FoldoutGroup("Debug", true, 0)]
		public BOSS_STATES currentState;

		private Transform currentTarget;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack dashAttack;

		public Vector2 dashOffset;

		public float followBrakeRadius = 1f;

		public float followBrakeSeconds = 1f;

		public float followPlayerAcceleration = 5f;

		public Vector2 followPlayerMaxOffset;

		public float followPlayerMaxSpeed = 10f;

		public Vector2 followPlayerMinOffset;

		public Vector2 followPlayerOffset;

		public Vector2 followPlayerVelocity;

		[FoldoutGroup("Attacks", true, 0)]
		public BossInstantProjectileAttack instantProjectileAttack;

		private bool isDashing;

		public bool IsSpelling;

		private bool isSummoningAttack;

		[FoldoutGroup("Debug", true, 0)]
		public PerpetuaBehaviour.Perpetua_ATTACKS lastAttack;

		private List<PerpetuaBehaviour.Perpetua_ATTACKS> queuedActions;

		public SplineFollower splineFollower;

		private State<PerpetuaBehaviour> stAction;

		private State<PerpetuaBehaviour> stDeath;

		private State<PerpetuaBehaviour> stFlapToPoint;

		private State<PerpetuaBehaviour> stFollowPlayer;

		private State<PerpetuaBehaviour> stIntro;

		[FoldoutGroup("Attacks", true, 0)]
		public BossAreaSummonAttack summonAttack;

		private Collider2D[] colliders;

		private ContactFilter2D contactFilter;

		private Collider2D collider;

		public enum Perpetua_ATTACKS
		{
			SPEAR_DASH,
			LIGHTNING_STRIKE,
			SINGLE_SPEAR
		}
	}
}
