using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Lesmes
{
	public class LesmesBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public Lesmes Lesmes { get; private set; }

		public bool Awaken { get; private set; }

		public int multiTeleportAttackNumber { get; private set; }

		private int dashRemainings { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.Lesmes = (Lesmes)this.Entity;
			this.currentlyAvailableAttacks = this.GetCurrentStateAttacks();
		}

		private List<LesmesBehaviour.Lesmes_ATTACKS> GetCurrentStateAttacks()
		{
			if (this.currentLesmesState == LesmesBehaviour.Lesmes_STATE.SWORD)
			{
				return new List<LesmesBehaviour.Lesmes_ATTACKS>
				{
					LesmesBehaviour.Lesmes_ATTACKS.DASH,
					LesmesBehaviour.Lesmes_ATTACKS.TELEPORT,
					LesmesBehaviour.Lesmes_ATTACKS.PATH_THROW,
					LesmesBehaviour.Lesmes_ATTACKS.SWORD_TOSS
				};
			}
			return new List<LesmesBehaviour.Lesmes_ATTACKS>
			{
				LesmesBehaviour.Lesmes_ATTACKS.MULTI_TELEPORT,
				LesmesBehaviour.Lesmes_ATTACKS.MULTIDASH,
				LesmesBehaviour.Lesmes_ATTACKS.SWORD_RECOVERY
			};
		}

		public override void OnStart()
		{
			base.OnStart();
			this.ChangeBossState(LesmesBehaviour.BOSS_STATES.WAITING);
			this.SetLesmesState(LesmesBehaviour.Lesmes_STATE.SWORD);
			this.StartWaitingPeriod(1f);
		}

		private void Update()
		{
			this.DistanceToTarget = Vector2.Distance(this.Lesmes.transform.position, this.Lesmes.Target.transform.position);
			if (!base.IsAttacking)
			{
				this._currentAttackLapse += Time.deltaTime;
			}
			if (this.DistanceToTarget > this.ActivationDistance || base.BehaviourTree.isRunning || this.Awaken)
			{
				return;
			}
			this.Awaken = true;
			Debug.Log("Lesmes: STARTING BEHAVIOR TREE");
			base.BehaviourTree.StartBehaviour();
		}

		private void SetCurrentCoroutine(Coroutine c)
		{
			if (this.currentCoroutine != null)
			{
				Debug.Log(">>>>STOPPING CURRENT COROUTINE");
				base.StopCoroutine(this.currentCoroutine);
			}
			Debug.Log(">>NEW COROUTINE");
			this.currentCoroutine = c;
		}

		private void ChangeBossState(LesmesBehaviour.BOSS_STATES newState)
		{
			this.currentState = newState;
		}

		private void StartAttackAction()
		{
			this.ChangeBossState(LesmesBehaviour.BOSS_STATES.MID_ACTION);
		}

		public LesmesBehaviour.Lesmes_ATTACKS GetNewAttack()
		{
			LesmesBehaviour.Lesmes_ATTACKS[] array = new LesmesBehaviour.Lesmes_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<LesmesBehaviour.Lesmes_ATTACKS> list = new List<LesmesBehaviour.Lesmes_ATTACKS>(array);
			list.Remove(this.lastAttack);
			if (this.lastAttack == LesmesBehaviour.Lesmes_ATTACKS.PATH_THROW)
			{
				list.Remove(LesmesBehaviour.Lesmes_ATTACKS.TELEPORT);
			}
			else if (this.lastAttack == LesmesBehaviour.Lesmes_ATTACKS.SWORD_RECOVERY)
			{
				list.Remove(LesmesBehaviour.Lesmes_ATTACKS.TELEPORT);
				list.Remove(LesmesBehaviour.Lesmes_ATTACKS.SWORD_TOSS);
			}
			else if (this.lastAttack == LesmesBehaviour.Lesmes_ATTACKS.SWORD_TOSS)
			{
				list.Remove(LesmesBehaviour.Lesmes_ATTACKS.SWORD_RECOVERY);
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public void LaunchRandomAction()
		{
			this.LaunchAction(this.GetNewAttack(), true);
		}

		private void QueuedActionsPush(LesmesBehaviour.Lesmes_ATTACKS atk)
		{
			if (this.queuedActions == null)
			{
				this.queuedActions = new List<LesmesBehaviour.Lesmes_ATTACKS>();
			}
			this.queuedActions.Add(atk);
		}

		private LesmesBehaviour.Lesmes_ATTACKS QueuedActionsPop()
		{
			LesmesBehaviour.Lesmes_ATTACKS lesmes_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(lesmes_ATTACKS);
			return lesmes_ATTACKS;
		}

		public void LaunchAction(LesmesBehaviour.Lesmes_ATTACKS atk, bool checkReposition = true)
		{
			if (this.attacksConfiguration.Find((LesmesBehaviour.LesmesAttackConfig x) => x.attackType == atk).requiresReposition && checkReposition)
			{
				Vector3 zero = Vector3.zero;
				if (atk != LesmesBehaviour.Lesmes_ATTACKS.DASH)
				{
				}
				this.QueuedActionsPush(atk);
			}
			else
			{
				this.lastAttack = atk;
				if (atk == LesmesBehaviour.Lesmes_ATTACKS.DASH)
				{
					this.DashAttack();
				}
			}
		}

		public override void Idle()
		{
			Debug.Log("Lesmes: IDLE");
			this.StopMovement();
		}

		private void StartWaitingPeriod(float seconds)
		{
			this.ChangeBossState(LesmesBehaviour.BOSS_STATES.WAITING);
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
			this.ChangeBossState(LesmesBehaviour.BOSS_STATES.AVAILABLE_FOR_ACTION);
		}

		public void Reposition(Vector3 point)
		{
			this.StartAttackAction();
			this.LookAtTarget(point);
			Debug.Log("Lesmes: REPOSITION");
			this.Lesmes.AnimatorInyector.Dash(true);
			this.quickMove.OnDashFinishedEvent += this.OnRepositionFinished;
			this.quickMove.DashToPoint(base.transform, point, 0f);
		}

		private void OnRepositionFinished()
		{
			this.quickMove.OnDashFinishedEvent -= this.OnRepositionFinished;
			this.Lesmes.AnimatorInyector.Dash(false);
			LesmesBehaviour.Lesmes_ATTACKS atk = this.QueuedActionsPop();
			this.LaunchAction(atk, false);
		}

		public void DashAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingDashCoroutine()));
		}

		private IEnumerator PreparingDashCoroutine()
		{
			this.Lesmes.AnimatorInyector.BigDashPreparation();
			this.currentTarget = base.GetTarget();
			this.LookAtTarget(this.currentTarget.position);
			yield return new WaitForSeconds(this.attacksConfiguration.Find((LesmesBehaviour.LesmesAttackConfig x) => x.attackType == LesmesBehaviour.Lesmes_ATTACKS.DASH).preparationSeconds);
			Debug.Log("Lesmes: DASH ATTACK");
			this.Lesmes.AnimatorInyector.Dash(true);
			this.dashAttack.OnDashFinishedEvent += this.OnDashAttackFinished;
			float d = Mathf.Sign(this.currentTarget.position.x - base.transform.position.x);
			this.dashAttack.Dash(base.transform, Vector3.right * d, 20f, 0f, false);
			yield break;
		}

		private void OnDashAttackFinished()
		{
			this.dashAttack.OnDashFinishedEvent -= this.OnDashAttackFinished;
			this.Lesmes.AnimatorInyector.Dash(false);
			this.StartWaitingPeriod(this.GetAttackConfig(LesmesBehaviour.Lesmes_ATTACKS.DASH).waitingSecondsAfterAttack);
		}

		public void NDashAttack()
		{
			this.StartAttackAction();
			this.dashRemainings = this.GetAttackConfig(LesmesBehaviour.Lesmes_ATTACKS.MULTIDASH).multiAttackTimes;
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingMultiDash()));
		}

		private IEnumerator PreparingMultiDash()
		{
			this.Lesmes.AnimatorInyector.BigDashPreparation();
			yield return new WaitForSeconds(this.attacksConfiguration.Find((LesmesBehaviour.LesmesAttackConfig x) => x.attackType == LesmesBehaviour.Lesmes_ATTACKS.MULTIDASH).preparationSeconds);
			this.currentTarget = base.GetTarget();
			this.LookAtTarget(this.currentTarget.position);
			Debug.Log("Lesmes: NDASH ATTACK");
			this.Lesmes.AnimatorInyector.Dash(true);
			this.multiDashAttack.OnDashFinishedEvent += this.OnMultiDashAttackFinished;
			float d = Mathf.Sign(this.currentTarget.position.x - base.transform.position.x);
			this.multiDashAttack.Dash(base.transform, Vector3.right * d, 20f, 0f, false);
			yield break;
		}

		private void OnMultiDashAttackFinished()
		{
			this.dashRemainings--;
			this.multiDashAttack.OnDashFinishedEvent -= this.OnMultiDashAttackFinished;
			this.Lesmes.AnimatorInyector.Dash(false);
			LesmesBehaviour.LesmesAttackConfig attackConfig = this.GetAttackConfig(LesmesBehaviour.Lesmes_ATTACKS.MULTIDASH);
			if (this.dashRemainings > 0)
			{
				this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingMultiDash()));
			}
			else
			{
				this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack);
			}
		}

		public void TeleportAttack()
		{
			this.StartAttackAction();
			this.currentTarget = base.GetTarget();
			this.teleportAttack.OnTeleportInEvent += this.OnTeleportIn;
			Debug.Log("Lesmes: Teleport OUT");
			this.Lesmes.AnimatorInyector.TeleportOut();
			this.teleportAttack.Use(base.transform, this.currentTarget, Vector3.up * 4f);
		}

		private void OnTeleportIn()
		{
			this.Lesmes.AnimatorInyector.TeleportIn();
			this.teleportAttack.OnTeleportInEvent -= this.OnTeleportIn;
			this.PlungeAttack();
		}

		public void PlungeAttack()
		{
			Debug.Log("Lesmes: PLUNGE ATTACK");
			this.Lesmes.AnimatorInyector.Plunge(true);
			this.currentTarget = base.GetTarget();
			this.plungeAttack.OnDashFinishedEvent += this.OnPlungeAttackFinished;
			this.plungeAttack.Dash(base.transform, Vector3.down, 10f, 0f, false);
		}

		private void OnPlungeAttackFinished()
		{
			this.plungeAttack.OnDashFinishedEvent -= this.OnPlungeAttackFinished;
			this.Lesmes.AnimatorInyector.Plunge(false);
			this.StartWaitingPeriod(this.GetAttackConfig(LesmesBehaviour.Lesmes_ATTACKS.TELEPORT).waitingSecondsAfterAttack);
		}

		public void MultiTeleportAttack()
		{
			this.StartAttackAction();
			this.multiTeleportAttackNumber = this.GetAttackConfig(LesmesBehaviour.Lesmes_ATTACKS.MULTI_TELEPORT).multiAttackTimes;
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingMultiTeleport()));
		}

		private IEnumerator PreparingMultiTeleport()
		{
			this.currentTarget = base.GetTarget();
			this.multiTeleportAttack.OnTeleportInEvent += this.OnMultiTeleportIn;
			Debug.Log("Lesmes: MULTI Teleport OUT");
			this.Lesmes.AnimatorInyector.TeleportOut();
			this.multiTeleportAttack.Use(base.transform, this.currentTarget, Vector3.up * 4f);
			yield return null;
			yield break;
		}

		private void OnMultiTeleportIn()
		{
			Debug.Log("Lesmes: MULTI Teleport IN");
			this.Lesmes.AnimatorInyector.TeleportIn();
			this.multiTeleportAttack.OnTeleportInEvent -= this.OnMultiTeleportIn;
			this.MultiPlungeAttack();
		}

		public void MultiPlungeAttack()
		{
			Debug.Log("Lesmes: MULTI PLUNGE ATTACK");
			this.Lesmes.AnimatorInyector.Plunge(true);
			this.currentTarget = base.GetTarget();
			this.plungeAttack.OnDashFinishedEvent += this.OnMultiPlungeAttackFinished;
			this.plungeAttack.Dash(base.transform, Vector3.down, 10f, 0f, false);
		}

		private void OnMultiPlungeAttackFinished()
		{
			Debug.Log("Lesmes: PLUNGE ATTACK FINISHED NUMBER:" + this.multiTeleportAttackNumber);
			this.plungeAttack.OnDashFinishedEvent -= this.OnMultiPlungeAttackFinished;
			this.Lesmes.AnimatorInyector.Plunge(false);
			this.multiTeleportAttackNumber--;
			if (this.multiTeleportAttackNumber > 0)
			{
				this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingMultiTeleport()));
			}
			else
			{
				this.StartWaitingPeriod(this.GetAttackConfig(LesmesBehaviour.Lesmes_ATTACKS.MULTI_TELEPORT).waitingSecondsAfterAttack);
			}
		}

		private void UnsubscribeFromEverything()
		{
			Debug.Log("UNSUBSCRIBING FROM EVERY EVENT");
			this.multiTeleportAttack.OnTeleportInEvent -= this.OnMultiTeleportIn;
			this.plungeAttack.OnDashFinishedEvent -= this.OnMultiPlungeAttackFinished;
			this.plungeAttack.OnDashFinishedEvent -= this.OnPlungeAttackFinished;
			this.teleportAttack.OnTeleportInEvent -= this.OnTeleportIn;
			this.multiDashAttack.OnDashFinishedEvent -= this.OnMultiDashAttackFinished;
			this.dashAttack.OnDashFinishedEvent -= this.OnDashAttackFinished;
			this.quickMove.OnDashFinishedEvent -= this.OnRepositionFinished;
		}

		private LesmesBehaviour.LesmesAttackConfig GetAttackConfig(LesmesBehaviour.Lesmes_ATTACKS atk)
		{
			return this.attacksConfiguration.Find((LesmesBehaviour.LesmesAttackConfig x) => x.attackType == atk);
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == LesmesBehaviour.BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public bool TargetCanBeVisible()
		{
			float num = this.Lesmes.Target.transform.position.y - this.Lesmes.transform.position.y;
			num = ((num <= 0f) ? (-num) : num);
			return num <= this.MaxVisibleHeight;
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.Lesmes.transform.position.x)
			{
				if (this.Lesmes.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.Lesmes.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.Lesmes.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.Lesmes.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public void SetLesmesState(LesmesBehaviour.Lesmes_STATE st)
		{
			this.currentLesmesState = st;
			this.currentlyAvailableAttacks = this.GetCurrentStateAttacks();
		}

		public LesmesBehaviour.Lesmes_STATE GetLesmesState()
		{
			return this.currentLesmesState;
		}

		public override void Damage()
		{
		}

		public bool CanAttack()
		{
			return true;
		}

		public void Death()
		{
			base.StopAllCoroutines();
			this.UnsubscribeFromEverything();
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
			this.Lesmes.AnimatorInyector.Death();
		}

		public void ResetCoolDown()
		{
			if (this._currentAttackLapse > 0f)
			{
				this._currentAttackLapse = 0f;
			}
		}

		public override void Attack()
		{
			if (base.IsAttacking)
			{
				return;
			}
			this.StopMovement();
			Transform target = base.GetTarget();
			this.LookAtTarget(target.position);
			if (this._currentAttackLapse < this.AttackCoolDown)
			{
				return;
			}
			this._currentAttackLapse = 0f;
		}

		public override void StopMovement()
		{
			this.Lesmes.Input.HorizontalInput = 0f;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MaxVisibleHeight = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MinAttackDistance = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackCoolDown = 2f;

		private float _currentAttackLapse;

		private Transform currentTarget;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack dashAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossTeleportAttack teleportAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossTeleportAttack multiTeleportAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack plungeAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack multiDashAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossSplineFollowingProjectileAttack splineThrowAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossStraightProjectileAttack tossAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack quickMove;

		[FoldoutGroup("Attacks", true, 0)]
		public BossAreaSummonAttack areaSummonAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public List<LesmesBehaviour.LesmesAttackConfig> attacksConfiguration;

		[FoldoutGroup("Traits", true, 0)]
		public EntityMotionChecker motionChecker;

		private List<LesmesBehaviour.Lesmes_ATTACKS> currentlyAvailableAttacks;

		private List<LesmesBehaviour.Lesmes_ATTACKS> queuedActions;

		private LesmesBehaviour.Lesmes_STATE currentLesmesState;

		[FoldoutGroup("Debug", true, 0)]
		public LesmesBehaviour.BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public LesmesBehaviour.Lesmes_ATTACKS lastAttack;

		private Transform currentHang;

		private Coroutine currentCoroutine;

		[Serializable]
		public struct LesmesAttackConfig
		{
			public LesmesBehaviour.Lesmes_ATTACKS attackType;

			public bool requiresReposition;

			public bool invertedReposition;

			public float preparationSeconds;

			public int multiAttackTimes;

			public float waitingSecondsAfterAttack;
		}

		public enum BOSS_STATES
		{
			WAITING,
			MID_ACTION,
			AVAILABLE_FOR_ACTION
		}

		public enum Lesmes_STATE
		{
			SWORD,
			NO_SWORD
		}

		public enum Lesmes_ATTACKS
		{
			DASH,
			MULTIDASH,
			TELEPORT,
			PATH_THROW,
			SWORD_TOSS,
			MULTI_TELEPORT,
			SWORD_RECOVERY
		}
	}
}
