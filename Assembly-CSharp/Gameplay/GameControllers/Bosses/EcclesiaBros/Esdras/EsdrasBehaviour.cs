using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras.AI;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras.Animator;
using Gameplay.GameControllers.Bosses.EcclesiaBros.Perpetua;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.EcclesiaBros.Esdras
{
	public class EsdrasBehaviour : EnemyBehaviour
	{
		public Esdras Esdras { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<EsdrasBehaviour> OnActionFinished;

		public bool KeepRunningAnimation()
		{
			return this._keepRunningAnimation;
		}

		public bool AttackIfEnemyClose()
		{
			return this._attackIfEnemyClose;
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this.stIntro = new Esdras_StIntro();
			this.stAction = new Esdras_StAction();
			this.stRun = new Esdras_StRun();
			this.stDeath = new Esdras_StDeath();
			this._fsm = new StateMachine<EsdrasBehaviour>(this, this.stIntro, null, null);
			this.results = new RaycastHit2D[1];
			this.currentlyAvailableAttacks = new List<EsdrasBehaviour.Esdras_ATTACKS>
			{
				EsdrasBehaviour.Esdras_ATTACKS.HEAVY_ATTACK,
				EsdrasBehaviour.Esdras_ATTACKS.LIGHT_ATTACK,
				EsdrasBehaviour.Esdras_ATTACKS.SINGLE_SPIN
			};
		}

		public override void OnStart()
		{
			base.OnStart();
			this.Esdras = (Esdras)this.Entity;
			this.ActivateCollisions(false);
			this.ChangeBossState(BOSS_STATES.WAITING);
			PoolManager.Instance.CreatePool(this.shockwave, 1);
			this.firstAttackDone = false;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
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

		public void LaunchAction(EsdrasBehaviour.Esdras_ATTACKS atk)
		{
			switch (atk)
			{
			case EsdrasBehaviour.Esdras_ATTACKS.SINGLE_SPIN:
				this.IssueHorizontalAttack();
				break;
			case EsdrasBehaviour.Esdras_ATTACKS.HEAVY_ATTACK:
				this.IssueHeavyAttack();
				break;
			case EsdrasBehaviour.Esdras_ATTACKS.SPIN_LOOP:
				this.IssueSpinLoopAttack();
				break;
			case EsdrasBehaviour.Esdras_ATTACKS.LIGHT_ATTACK:
				this.IssueLightAttack();
				break;
			case EsdrasBehaviour.Esdras_ATTACKS.SUMMON_PERPETUA:
				this.IssueSummonPerpetua();
				break;
			case EsdrasBehaviour.Esdras_ATTACKS.COUNTER_IMPACT:
				this.IssueCounterImpact();
				break;
			case EsdrasBehaviour.Esdras_ATTACKS.SPIN_PROJECTILE:
				this.IssueProjectiles();
				break;
			}
			this.lastAttack = atk;
		}

		public EsdrasBehaviour.Esdras_ATTACKS GetNewAttack()
		{
			if (this.queuedActions != null && this.queuedActions.Count > 0)
			{
				return this.QueuedActionsPop();
			}
			EsdrasBehaviour.Esdras_ATTACKS[] array = new EsdrasBehaviour.Esdras_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<EsdrasBehaviour.Esdras_ATTACKS> list = new List<EsdrasBehaviour.Esdras_ATTACKS>(array);
			list.Remove(this.lastAttack);
			EsdrasBehaviour.Esdras_ATTACKS result;
			if (!this.firstAttackDone)
			{
				result = EsdrasBehaviour.Esdras_ATTACKS.SPIN_LOOP;
			}
			else
			{
				result = list[UnityEngine.Random.Range(0, list.Count)];
			}
			this.firstAttackDone = true;
			return result;
		}

		public IEnumerator WaitForState(State<EsdrasBehaviour> st)
		{
			while (!this._fsm.IsInState(st))
			{
				yield return null;
			}
			yield break;
		}

		public void LaunchRandomAction()
		{
			this.LaunchAction(this.GetNewAttack());
		}

		private void QueuedActionsPush(EsdrasBehaviour.Esdras_ATTACKS atk)
		{
			if (this.queuedActions == null)
			{
				this.queuedActions = new List<EsdrasBehaviour.Esdras_ATTACKS>();
			}
			this.queuedActions.Add(atk);
		}

		private EsdrasBehaviour.Esdras_ATTACKS QueuedActionsPop()
		{
			EsdrasBehaviour.Esdras_ATTACKS esdras_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(esdras_ATTACKS);
			return esdras_ATTACKS;
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public float GetHealthPercentage()
		{
			return this.Esdras.CurrentLife / this.Esdras.Stats.Life.Base;
		}

		private void SetPhase(EsdrasBehaviour.EsdrasPhases p)
		{
			this.currentlyAvailableAttacks = p.availableAttacks;
			this._currentPhase = p.phaseId;
			if (this._currentPhase == EsdrasBehaviour.ESDRAS_PHASES.LAST)
			{
				this.QueuedActionsPush(EsdrasBehaviour.Esdras_ATTACKS.SUMMON_PERPETUA);
			}
		}

		private void ChangePhase(EsdrasBehaviour.ESDRAS_PHASES p)
		{
			EsdrasBehaviour.EsdrasPhases phase = this.phases.Find((EsdrasBehaviour.EsdrasPhases x) => x.phaseId == p);
			this.SetPhase(phase);
		}

		private void CheckNextPhase()
		{
			float healthPercentage = this.GetHealthPercentage();
			EsdrasBehaviour.ESDRAS_PHASES currentPhase = this._currentPhase;
			if (currentPhase != EsdrasBehaviour.ESDRAS_PHASES.FIRST)
			{
				if (currentPhase != EsdrasBehaviour.ESDRAS_PHASES.SECOND)
				{
					if (currentPhase != EsdrasBehaviour.ESDRAS_PHASES.LAST)
					{
					}
				}
				else if (healthPercentage < 0.3f)
				{
					this.ChangePhase(EsdrasBehaviour.ESDRAS_PHASES.LAST);
				}
			}
			else if (healthPercentage < 0.6f)
			{
				this.ChangePhase(EsdrasBehaviour.ESDRAS_PHASES.SECOND);
			}
		}

		private IEnumerator GetIntoStateAndCallback(State<EsdrasBehaviour> newSt, float waitSeconds, Action callback)
		{
			this._fsm.ChangeState(newSt);
			yield return new WaitForSeconds(2f);
			callback();
			yield break;
		}

		private void StartWaitingPeriod(float seconds)
		{
			this.ChangeBossState(BOSS_STATES.WAITING);
			this.SetCurrentCoroutine(base.StartCoroutine(this.WaitingPeriodCoroutine(seconds, new Action(this.AfterWaitingPeriod))));
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

		public void StartIntroSequence()
		{
			this._fsm.ChangeState(this.stIntro);
			this.ActivateCollisions(false);
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.IntroSequenceCoroutine()));
		}

		private IEnumerator IntroSequenceCoroutine()
		{
			this.ChangePhase(EsdrasBehaviour.ESDRAS_PHASES.FIRST);
			this.Esdras.AnimatorInyector.Taunt();
			yield return new WaitForSeconds(1.5f);
			this.LookAtPenitent();
			yield return new WaitForSeconds(1.5f);
			base.BehaviourTree.StartBehaviour();
			this.ActivateCollisions(true);
			this.StartWaitingPeriod(0.1f);
			yield break;
		}

		private void ActivateCollisions(bool activate)
		{
			this.Esdras.DamageArea.DamageAreaCollider.enabled = activate;
		}

		private void Shake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 1f, 12, 0.2f, 0f, default(Vector3), 0f, false);
		}

		private void Wave()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.7f, 0.3f, 2f);
		}

		private void GetInRange(float range)
		{
			this._attackIfEnemyClose = true;
			this._runPoint = base.GetTarget().position;
			Vector2 vector = base.transform.position - this._runPoint;
			this._runPoint += vector.normalized * range;
			if (Mathf.Abs(this._runPoint.x - base.transform.position.x) < range)
			{
				this.LookAtPenitent();
				this._attackIfEnemyClose = false;
				this._fsm.ChangeState(this.stAction);
			}
			else
			{
				this._fsm.ChangeState(this.stRun);
			}
		}

		private void IssueHorizontalAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.HorizontalAttackCoroutine()));
		}

		private IEnumerator HorizontalAttackCoroutine()
		{
			this.SetGhostTrail(true);
			float runDistance = 1.5f;
			Vector3 dir = base.GetTarget().position - base.transform.position;
			this._runPoint = base.transform.position + dir.normalized * runDistance;
			this._keepRunningAnimation = true;
			this._fsm.ChangeState(this.stRun);
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			this.Esdras.AnimatorInyector.SpinAttack();
			this.Esdras.Audio.QueueSlideAttack_AUDIO();
			yield return new WaitForSeconds(0.3f);
			if (this._currentPhase == EsdrasBehaviour.ESDRAS_PHASES.SECOND)
			{
				Vector2 vector = new Vector2(dir.x, 0f);
				this.LaunchArcProjectile(vector.normalized);
			}
			this._keepRunningAnimation = false;
			this.Esdras.AnimatorInyector.Run(false);
			yield return new WaitForSeconds(1.7f);
			this.Esdras.Audio.StopSlideAttack_AUDIO();
			this.SetGhostTrail(false);
			this.OnHorizontalAttackEnds();
			yield break;
		}

		private void OnHorizontalAttackEnds()
		{
			this.StartWaitingPeriod(0.6f);
		}

		private void IssueSpinLoopAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.SpinLoopAttackCoroutine()));
		}

		private Transform GetSpinLoopPoint()
		{
			if (Mathf.Abs(this.leftLimitTransform.position.x - base.transform.position.x) < Mathf.Abs(this.rightLimitTransform.position.x - base.transform.position.x))
			{
				return this.leftLimitTransform;
			}
			return this.rightLimitTransform;
		}

		private IEnumerator SpinLoopAttackCoroutine()
		{
			Transform p = this.GetSpinLoopPoint();
			Vector2 dir = (!(p == this.leftLimitTransform)) ? Vector2.left : Vector2.right;
			this._runPoint = p.position;
			this._keepRunningAnimation = true;
			this._fsm.ChangeState(this.stRun);
			this.Esdras.Audio.QueueSpinLoop();
			this.Esdras.AnimatorInyector.SpinLoop(true);
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			this._keepRunningAnimation = false;
			this.Esdras.AnimatorInyector.SpinLoop(true);
			this.spinLoopAttack.OnDashBlockedEvent += this.OnSpinDashBlocked;
			this.spinLoopAttack.OnDashAdvancedEvent += this.OnSpinDashAdvanced;
			this.spinLoopAttack.OnDashFinishedEvent += this.OnSpinDashFinished;
			this.spinLoopAttack.Dash(base.transform, dir, this.spinDistance, 0f, false);
			yield break;
		}

		private void OnSpinDashAdvanced(float value)
		{
			if (value > 0.8f)
			{
				this.Esdras.AnimatorInyector.Run(false);
				this.Esdras.AnimatorInyector.SpinLoop(false);
			}
		}

		private void OnSpinDashBlocked(BossDashAttack obj)
		{
			this.Esdras.AnimatorInyector.SpinLoop(false);
			this.SpinLoopEnds();
		}

		private void OnSpinDashFinished()
		{
			this.Esdras.AnimatorInyector.SpinLoop(false);
			this.SpinLoopEnds();
		}

		private void SpinLoopEnds()
		{
			this.Esdras.Audio.StopSpinLoop_AUDIO();
			this.spinLoopAttack.OnDashBlockedEvent -= this.OnSpinDashBlocked;
			this.spinLoopAttack.OnDashAdvancedEvent -= this.OnSpinDashAdvanced;
			this.spinLoopAttack.OnDashFinishedEvent -= this.OnSpinDashFinished;
			this.LookAtPenitent();
			this.StartWaitingPeriod(1.2f);
		}

		private void IssueProjectiles()
		{
			this.StartAttackAction();
			this.SubscribeToSpin(false);
			this.SetCurrentCoroutine(base.StartCoroutine(this.ProjectilesCoroutine()));
		}

		private IEnumerator ProjectilesCoroutine()
		{
			this._spinsToProjectile = 0;
			float spinProjectileDuration = 2f;
			this.SetGhostTrail(true);
			float runDistance = 1.5f;
			Vector3 dir = base.GetTarget().position - base.transform.position;
			this._runPoint = base.transform.position + dir.normalized * runDistance;
			this._keepRunningAnimation = true;
			this._fsm.ChangeState(this.stRun);
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			this.Esdras.AnimatorInyector.SpinLoop(true);
			this.Esdras.Audio.QueueSpinProjectile();
			this.SubscribeToSpin(true);
			yield return new WaitForSeconds(0.2f);
			this.SetGhostTrail(false);
			this.singleSpinAttack.DealsDamage = true;
			this.instantLightningAttack.SummonAreaOnPoint(base.transform.position, 0f, 1f, null);
			yield return new WaitForSeconds(spinProjectileDuration);
			this.singleSpinAttack.DealsDamage = false;
			this.instantLightningAttack.SummonAreaOnPoint(base.transform.position, 0f, 1f, null);
			this.SubscribeToSpin(false);
			this.Esdras.AnimatorInyector.SpinLoop(false);
			this.Esdras.Audio.StopSpinProjectile_AUDIO();
			this._keepRunningAnimation = false;
			this.Esdras.AnimatorInyector.Run(false);
			yield return new WaitForSeconds(0.5f);
			this.ProjectilesEnd();
			yield break;
		}

		private void SubscribeToSpin(bool subscribe)
		{
			if (subscribe)
			{
				this.Esdras.AnimatorInyector.OnSpinProjectilePoint += this.OnSpinProjectilePoint;
			}
			else
			{
				this.Esdras.AnimatorInyector.OnSpinProjectilePoint -= this.OnSpinProjectilePoint;
			}
		}

		private void OnSpinProjectilePoint(EsdrasAnimatorInyector arg1, Vector2 dir)
		{
			this.LaunchArcProjectile(dir);
		}

		private void LaunchArcProjectile(Vector2 dir)
		{
			if (this._spinsToProjectile >= this.maxSpinsToProjectile)
			{
				this.arcProjectiles.Shoot(dir, dir, 1f);
				this._spinsToProjectile = 0;
			}
			else
			{
				this._spinsToProjectile++;
			}
		}

		private void ProjectilesEnd()
		{
			this.LookAtPenitent();
			this.StartWaitingPeriod(1.2f);
		}

		private void IssueHeavyAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.HeavyAttackCoroutine()));
		}

		private IEnumerator HeavyAttackCoroutine()
		{
			this.GetInRange(2f);
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			this._attackIfEnemyClose = false;
			this.Esdras.AnimatorInyector.OnHeavyAttackLightningSummon += this.OnHeavyAttackLightningSummon;
			this.Esdras.AnimatorInyector.HeavyAttack();
			yield return new WaitForSeconds(1.67f);
			this.OnHeavyAttackEnds();
			yield break;
		}

		private void OnHeavyAttackLightningSummon(EsdrasAnimatorInyector obj)
		{
			obj.OnHeavyAttackLightningSummon -= this.OnHeavyAttackLightningSummon;
			Vector2 vector = Vector2.right;
			vector *= (float)((this.Esdras.Status.Orientation != EntityOrientation.Right) ? -1 : 1);
			this.lightningAttack.SummonAreas(vector);
		}

		private void OnHeavyAttackEnds()
		{
			this.StartWaitingPeriod(0.5f);
		}

		private void IssueLightAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.LightAttackCoroutine()));
		}

		private IEnumerator LightAttackCoroutine()
		{
			this.GetInRange(2.5f);
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			this._attackIfEnemyClose = false;
			this.Esdras.AnimatorInyector.LightAttack();
			yield return new WaitForSeconds(1.1f);
			this.OnLightAttackEnds();
			yield break;
		}

		private void OnLightAttackEnds()
		{
			this.StartWaitingPeriod(0.5f);
		}

		private void IssueCounterImpact()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.CounterImpactCoroutine()));
		}

		private IEnumerator CounterImpactCoroutine()
		{
			this.Esdras.AnimatorInyector.Taunt();
			yield return new WaitForSeconds(1.5f);
			this.OnCounterImpactEnds();
			yield break;
		}

		private void OnCounterImpactEnds()
		{
			this.StartWaitingPeriod(0.7f);
		}

		private void IssueSummonPerpetua()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.SummonPerpetuaCoroutine()));
		}

		private IEnumerator SummonPerpetuaCoroutine()
		{
			this.Esdras.AnimatorInyector.Taunt();
			yield return new WaitForSeconds(0.5f);
			this.lightningAttack.SummonAreas(Vector2.right);
			this.lightningAttack.SummonAreas(Vector2.left);
			yield return new WaitForSeconds(0.5f);
			this.Esdras.Audio.PlayCallSister_AUDIO();
			this.perpetuaSummoner.SpawnFightInPosition(base.transform.position);
			Core.Logic.ScreenFreeze.Freeze(0.05f, 0.2f, 0f, null);
			yield return new WaitForSeconds(1f);
			this.Esdras.Audio.ChangeEsdrasMusic();
			this.OnPerpetuaSummoned();
			yield break;
		}

		private void OnPerpetuaSummoned()
		{
			this.StartWaitingPeriod(2f);
		}

		public void CounterImpactShockwave()
		{
			float d = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.instantLightningAttack.SummonAreaOnPoint(base.transform.position - d * Vector3.right, 0f, 1f, null);
			PoolManager.Instance.ReuseObject(this.shockwave, base.transform.position, Quaternion.identity, false, 1);
		}

		public void OnHitReactionAnimationCompleted()
		{
			this.SetRecovering(false);
			this._currentRecoveryHits = 0;
		}

		public void AttackDisplacement(float duration = 0.4f, float displacement = 2f, bool trail = true)
		{
			this.SetGhostTrail(trail);
			this.Esdras.DamageByContact = false;
			Ease ease = Ease.OutQuad;
			float num = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			num *= displacement;
			Vector2 vector = Vector2.right * num;
			vector = this.ClampToFightBoundaries(vector);
			base.transform.DOMove(base.transform.position + vector, duration, false).SetEase(ease).OnComplete(delegate
			{
				this.AfterDisplacement();
			});
		}

		private void AfterDisplacement()
		{
			this.Esdras.DamageByContact = true;
			this.SetGhostTrail(false);
		}

		public void BackDisplacement(float duration = 0.4f, float displacement = 2f)
		{
			this.SetGhostTrail(true);
			this.Esdras.DamageByContact = false;
			Ease ease = Ease.OutQuad;
			float num = (this.Entity.Status.Orientation != EntityOrientation.Right) ? 1f : -1f;
			num *= displacement;
			Vector2 vector = Vector2.right * num;
			vector = this.ClampToFightBoundaries(vector);
			base.transform.DOMove(base.transform.position + vector, duration, false).SetEase(ease).OnComplete(delegate
			{
				this.AfterDisplacement();
			});
		}

		public bool IsRecovering()
		{
			return this._recovering;
		}

		public void SetRecovering(bool recovering)
		{
			this._recovering = recovering;
		}

		public void RunToPoint(Vector2 position)
		{
			this.LookAtTarget(position);
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.Esdras.Input.HorizontalInput = horizontalInput;
		}

		public void StopRunning()
		{
			this.Esdras.Input.HorizontalInput = 0f;
			this.Esdras.Controller.PlatformCharacterPhysics.HSpeed = 0f;
		}

		public bool CloseToPointX(Vector2 p, float closeDistance = 0.1f)
		{
			return Mathf.Abs(p.x - base.transform.position.x) < closeDistance;
		}

		public bool CloseToTarget(float closeDistance = 1f)
		{
			Transform target = base.GetTarget();
			return Mathf.Abs(target.position.x - base.transform.position.x) < closeDistance;
		}

		public void ChangeToAction()
		{
			this._fsm.ChangeState(this.stAction);
		}

		public void SetRunAnimation(bool run)
		{
			this.Esdras.AnimatorInyector.Run(run);
		}

		public Vector2 GetRunPoint()
		{
			return this._runPoint;
		}

		public void Death()
		{
			this.SetGhostTrail(false);
			this.perpetuaSummoner.DismissPerpetua();
			base.StopAllCoroutines();
			base.transform.DOKill(true);
			base.StopBehaviour();
			this.Esdras.AnimatorInyector.Death();
			this._fsm.ChangeState(this.stDeath);
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Parry()
		{
			base.Parry();
			base.StopAllCoroutines();
			this.SetGhostTrail(false);
			this.isBeingParried = true;
			base.transform.DOKill(false);
			this.Esdras.AnimatorInyector.Parry();
			this.BackDisplacement(0.5f, 1f);
			this.SetRecovering(true);
			this.StartWaitingPeriod(1f);
		}

		public void LookAtPenitent()
		{
			if (Core.Logic.Penitent)
			{
				this.LookAtTarget(Core.Logic.Penitent.transform.position);
			}
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public bool CanChase()
		{
			return true;
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			this.CheckNextPhase();
			if (this._recovering && this._currentRecoveryHits < this.maxHitsInRecovery)
			{
				base.StopAllCoroutines();
				this.Esdras.Audio.StopAll();
				this.Esdras.AnimatorInyector.Hurt();
				base.transform.DOKill(true);
				this.LookAtPenitent();
				this.BackDisplacement(0.3f, 0.4f);
				this._currentRecoveryHits++;
				if (this._currentRecoveryHits >= this.maxHitsInRecovery)
				{
					this.Esdras.CounterFlash();
					this.IssueCounterImpact();
				}
				else
				{
					this.StartWaitingPeriod(1f);
				}
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			this.Esdras.SetOrientation((targetPos.x <= this.Esdras.transform.position.x) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
		}

		public override void StopMovement()
		{
		}

		public void SetGhostTrail(bool active)
		{
			this.Esdras.GhostTrail.EnableGhostTrail = active;
		}

		private Vector2 ClampToFightBoundaries(Vector2 dir)
		{
			Vector2 vector = dir;
			UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + vector, Color.green, 5f);
			if (Physics2D.RaycastNonAlloc(base.transform.position, dir, this.results, dir.magnitude, this.fightBoundariesLayerMask) > 0)
			{
				UnityEngine.Debug.DrawLine(base.transform.position, this.results[0].point, Color.red, 5f);
				vector = vector.normalized * this.results[0].distance;
			}
			return vector;
		}

		public void OnDrawGizmos()
		{
		}

		[FoldoutGroup("Debug", true, 0)]
		public BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public EsdrasBehaviour.Esdras_ATTACKS lastAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack lightningAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack instantLightningAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossStraightProjectileAttack arcProjectiles;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private PerpetuaFightSpawner perpetuaSummoner;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public EsdrasMeleeAttack lightAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public EsdrasMeleeAttack heavyAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public EsdrasMeleeAttack singleSpinAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public BossDashAttack spinLoopAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public GameObject shockwave;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform leftLimitTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform rightLimitTransform;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private List<EsdrasBehaviour.EsdrasPhases> phases;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private float maxChaseTime = 5f;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private int maxHitsInRecovery = 3;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private int maxSpinsToProjectile = 2;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private float spinDistance = 10f;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private LayerMask fightBoundariesLayerMask;

		private bool _keepRunningAnimation;

		private bool _attackIfEnemyClose;

		private Transform currentTarget;

		private StateMachine<EsdrasBehaviour> _fsm;

		private State<EsdrasBehaviour> stAction;

		private State<EsdrasBehaviour> stRun;

		private State<EsdrasBehaviour> stIntro;

		private State<EsdrasBehaviour> stPosture;

		private State<EsdrasBehaviour> stDeath;

		private Coroutine currentCoroutine;

		private EsdrasBehaviour.ESDRAS_PHASES _currentPhase;

		private List<EsdrasBehaviour.Esdras_ATTACKS> currentlyAvailableAttacks;

		private List<EsdrasBehaviour.Esdras_ATTACKS> queuedActions;

		private RaycastHit2D[] results;

		private Vector2 _runPoint;

		private float _chaseCounter;

		private bool _tiredOfChasing;

		private bool _recovering;

		private int _currentRecoveryHits;

		private int _spinsToProjectile;

		private bool isBeingParried;

		private bool firstAttackDone;

		[Serializable]
		public struct EsdrasPhases
		{
			public EsdrasBehaviour.ESDRAS_PHASES phaseId;

			public List<EsdrasBehaviour.Esdras_ATTACKS> availableAttacks;
		}

		public enum ESDRAS_PHASES
		{
			FIRST,
			SECOND,
			LAST
		}

		[Serializable]
		public struct EsdrasAttackConfig
		{
			public EsdrasBehaviour.Esdras_ATTACKS attackType;

			public float preparationSeconds;

			public float waitingSecondsAfterAttack;
		}

		public enum Esdras_ATTACKS
		{
			SINGLE_SPIN,
			HEAVY_ATTACK,
			SPIN_LOOP,
			LIGHT_ATTACK,
			SUMMON_PERPETUA,
			COUNTER_IMPACT,
			SPIN_PROJECTILE
		}
	}
}
