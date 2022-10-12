using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using BezierSplines;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Bosses.TresAngustias.AI;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.TrinityMinion.AI;
using Gameplay.GameControllers.Entities;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.TresAngustias
{
	public class SingleAnguishBehaviour : EnemyBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<SingleAnguishBehaviour> OnActionFinished;

		public void SetScroll(bool s)
		{
			this.followScroll = s;
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this.stDance = new SingleAnguishSt_Dance();
			this.stGoToDancePoint = new SingleAnguishSt_GoToDance();
			this.stAction = new SingleAnguishSt_Action();
			this.stGoToMergePoint = new SingleAnguishSt_GoToMergePoint();
			this.stGoToComboPoint = new SingleAnguishSt_GoToComboPoint();
			this.stMerged = new SingleAnguishSt_Merged();
			this.stDeath = new SingleAnguishSt_Death();
			this.stIntro = new SingleAnguishSt_Intro();
			this.attackWarning = base.GetComponentInChildren<BossAttackWarning>();
			this.scrollManager.OnUpdateHeight += this.ScrollManager_OnUpdateHeight;
			this._fsm = new StateMachine<SingleAnguishBehaviour>(this, this.stIntro, null, null);
		}

		private void ScrollManager_OnUpdateHeight(float delta)
		{
			if (this.followScroll)
			{
				base.transform.position += Vector3.up * delta;
			}
		}

		public override void OnStart()
		{
			base.OnStart();
			this.SingleAnguish = (SingleAnguish)this.Entity;
			this.ChangeBossState(BOSS_STATES.WAITING);
			PoolManager.Instance.CreatePool(this.spearStartFx, 1);
		}

		private void SetCurrentCoroutine(Coroutine c)
		{
			if (this.currentCoroutine != null)
			{
				UnityEngine.Debug.Log(">>>>STOPPING CURRENT COROUTINE");
				base.StopCoroutine(this.currentCoroutine);
			}
			UnityEngine.Debug.Log(">>NEW COROUTINE");
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

		public void IssueSpearAttack(int n = 1, float delay = 1f)
		{
			this._multiAttacksRemaining = n;
			this.LaunchSpearAttack(delay);
		}

		private void LaunchSpearAttack(float delay)
		{
			this.spear.ChangeState(FloatingWeapon.FLOATING_WEAPON_STATES.AIMING);
			if (this.spear.hidden)
			{
				this.spear.Show(true);
			}
			this.SingleAnguish.Audio.PlayLanceCharge();
			this.spear.AimToPlayer();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingSpearAttack(delay, 0.75f)));
		}

		private IEnumerator PreparingSpearAttack(float aimTime, float recoveryTime = 0.75f)
		{
			yield return new WaitForSeconds(aimTime);
			this.spear.ChangeState(FloatingWeapon.FLOATING_WEAPON_STATES.STOP);
			GameObject go = PoolManager.Instance.ReuseObject(this.spearStartFx, this.spear.transform.position, Quaternion.identity, false, 1).GameObject;
			go.transform.SetParent(this.spear.transform);
			this.SingleAnguish.Audio.PlayLanceShot();
			go.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(this.spear.transform.right.y, this.spear.transform.right.x) * 57.29578f);
			yield return new WaitForSeconds(0.22f);
			this.attackWarning.ShowWarning(this.spear.transform.position);
			yield return new WaitForSeconds(0.4f);
			UnityEngine.Debug.Log("SPEAR ATTACK");
			this.spear.Hide(true);
			this.spearAttack.Shoot(this.spear.transform.position, this.spear.transform.right);
			base.transform.DOPunchPosition(-this.spear.transform.right * 0.5f, 0.8f, 2, 1f, false);
			yield return new WaitForSeconds(recoveryTime);
			this.AfterSpearAttack(aimTime);
			yield break;
		}

		private void AfterSpearAttack(float delay)
		{
			this._multiAttacksRemaining--;
			if (this._multiAttacksRemaining > 0)
			{
				this.LaunchSpearAttack(delay);
			}
			else
			{
				this.spear.ChangeState(FloatingWeapon.FLOATING_WEAPON_STATES.FLOATING);
				this.spear.Show(true);
				this.ActionFinished();
			}
		}

		public void IssueMaceAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingPathThrow()));
		}

		private IEnumerator PreparingPathThrow()
		{
			this.attackWarning.ShowWarning(base.transform.position);
			SingleAnguishBehaviour.SingleAnguishAttackConfig config = this.attacksConfiguration.Find((SingleAnguishBehaviour.SingleAnguishAttackConfig x) => x.attackType == SingleAnguishBehaviour.ANGUISH_ATTACKS.MACE);
			SplinePointInfo info = this.bossfightConfig.GetMaceSplineInfo();
			BezierSpline spline = info.spline;
			Vector2 launchDir = spline.GetPoint(1f) - spline.GetPoint(0f);
			this.mace.transform.DOPunchPosition(launchDir.normalized, config.preparationSeconds, 1, 1f, false).SetEase(Ease.InCubic);
			yield return new WaitForSeconds(config.preparationSeconds);
			this.mace.Hide(false);
			this.maceAttack.Shoot(spline, info.speedCurve, info.time, base.transform.position);
			this.SingleAnguish.Audio.PlayMace();
			this.maceAttack.OnPathFinished += this.OnProjectilePathFinished;
			yield break;
		}

		private void OnProjectilePathFinished(BossSplineFollowingProjectileAttack obj)
		{
			this.SingleAnguish.Audio.StopMace();
			obj.OnPathFinished -= this.OnProjectilePathFinished;
			this.mace.transform.position = obj.lastPosition;
			this.mace.Show(false);
			this.ActionFinished();
		}

		public void IssueMaceSurroundAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingSurround()));
		}

		private IEnumerator PreparingSurround()
		{
			this.attackWarning.ShowWarning(base.transform.position);
			yield return new WaitForSeconds(this.attacksConfiguration.Find((SingleAnguishBehaviour.SingleAnguishAttackConfig x) => x.attackType == SingleAnguishBehaviour.ANGUISH_ATTACKS.MACE).preparationSeconds);
			SplinePointInfo info = this.maceSurroundInfo;
			BezierSpline spline = info.spline;
			this.mace.Hide(false);
			this.maceAttack.Shoot(spline, info.speedCurve, info.time, base.transform.position);
			this.SingleAnguish.Audio.PlayMace();
			this.maceAttack.OnPathFinished += this.OnProjectilePathFinished;
			yield break;
		}

		public void IssueShieldAttack()
		{
			base.StartCoroutine(this.ShieldSummons());
		}

		private IEnumerator ShieldSummons()
		{
			int i = 3;
			for (int j = 0; j < i; j++)
			{
				Enemy e = this.shieldAttack.Spawn(base.transform.position, base.transform.right, 1f, new Action(this.OnSpawnFinished));
				this.SingleAnguish.Audio.PlaySpawn();
				TrinityMinionBehaviour tmb = e.GetComponent<TrinityMinionBehaviour>();
				this.currentTarget = base.GetTarget();
				tmb.SetTarget(this.currentTarget);
				yield return new WaitForSeconds(0.5f);
			}
			this.OnSpawnFinished();
			yield break;
		}

		private void OnSpawnFinished()
		{
			this.ActionFinished();
		}

		public void IssueMerge(Vector2 point)
		{
			this._fsm.ChangeState(this.stGoToMergePoint);
			this._currentTargetPoint = point;
		}

		public void StopDancing()
		{
			UnityEngine.Debug.Log("STOP DANCING");
			this._fsm.ChangeState(this.stAction);
		}

		public void BackToDance()
		{
			UnityEngine.Debug.Log("BACK TO DANCE");
			this._fsm.ChangeState(this.stGoToDancePoint);
		}

		public void BackToAction()
		{
			this._fsm.ChangeState(this.stAction);
		}

		public void SetPath(BezierSpline s)
		{
			this.currentPath = s;
			UnityEngine.Debug.Log("CURRENT PATH CHANGED");
		}

		public Vector3 GetNextPathPoint()
		{
			float t = this.currentCurve.Evaluate((this._updateCounter + this.pathOffset) % this.secondsToFullLoop / this.secondsToFullLoop);
			return this.currentPath.GetPoint(t);
		}

		public void ForceWait(float s)
		{
			this.StartWaitingPeriod(s);
		}

		public void IssueCombo(Vector2 point)
		{
			this.StartAttackAction();
			this._fsm.ChangeState(this.stGoToComboPoint);
			this._currentTargetPoint = point;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
			if (!this._fsm.IsInState(this.stIntro))
			{
				this._updateCounter += Time.deltaTime;
			}
		}

		public void UpdateDanceState()
		{
			base.transform.position = this.GetNextPathPoint();
		}

		public void UpdateGoToDancePointState()
		{
			this._currentTargetPoint = this.GetNextPathPoint();
			this.SingleAnguish.arriveBehaviour.target = this._currentTargetPoint;
		}

		public void UpdateGoToTargetPoint()
		{
			this.SingleAnguish.arriveBehaviour.target = this._currentTargetPoint;
		}

		public void StartIntro()
		{
			this.StartAttackAction();
			this.SingleAnguish.Audio.PlayAppear();
			this.SingleAnguish.SpriteRenderer.DOFade(1f, 3f).OnComplete(delegate
			{
				this.ActionFinished();
			});
		}

		public void NotifyMaster()
		{
			if (this.OnActionFinished != null)
			{
				this.OnActionFinished(this);
			}
		}

		public void ActivateWeapon(bool activate)
		{
			if (this.spear != null)
			{
				this.spear.Activate(activate, true);
			}
			if (this.shield != null)
			{
				this.shield.Activate(activate, true);
			}
			if (this.mace != null)
			{
				this.mace.Activate(activate, true);
			}
		}

		public bool IsCloseToTargetPoint(float closeRange = 0.5f)
		{
			float num = Vector2.Distance(base.transform.position, this._currentTargetPoint);
			return num < closeRange;
		}

		public void ActivateSteering(bool enabled)
		{
			this.SingleAnguish.autonomousAgent.enabled = enabled;
			if (enabled)
			{
				this.SingleAnguish.autonomousAgent.currentVelocity = Vector3.zero;
			}
		}

		public void ActivateGhostMode(bool enabled)
		{
			this.ghostTrail.EnableGhostTrail = enabled;
		}

		public void ActivateCollider(bool enabled)
		{
			this.SingleAnguish.DamageArea.DamageAreaCollider.enabled = enabled;
		}

		public void ActivateSprite(bool activate)
		{
			this.SingleAnguish.SpriteRenderer.enabled = activate;
			this.SingleAnguish.DamageArea.DamageAreaCollider.enabled = activate;
		}

		public void ChangeToMerged()
		{
			this._fsm.ChangeState(this.stMerged);
		}

		public void ChangeToDance()
		{
			this._fsm.ChangeState(this.stDance);
		}

		public void ChangeToAction()
		{
			this._fsm.ChangeState(this.stAction);
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this.Entity.Status.Dead)
			{
				return;
			}
			if (this.Entity.transform.position.x >= targetPos.x + 1f)
			{
				if (this.Entity.Status.Orientation != EntityOrientation.Left)
				{
					if (this.OnTurning != null)
					{
						this.OnTurning();
					}
					this.Entity.SetOrientation(EntityOrientation.Left, true, false);
				}
			}
			else if (this.Entity.transform.position.x < targetPos.x - 1f && this.Entity.Status.Orientation != EntityOrientation.Right)
			{
				if (this.OnTurning != null)
				{
					this.OnTurning();
				}
				this.Entity.SetOrientation(EntityOrientation.Right, true, false);
			}
		}

		public void Death()
		{
			this.SingleAnguish.AnimatorInyector.Death();
			this.ActivateSteering(false);
			this._fsm.ChangeState(this.stDeath);
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

		public TresAngustiasMaster master;

		[FoldoutGroup("Dance path", 0)]
		public BezierSpline currentPath;

		[FoldoutGroup("Dance path", 0)]
		public AnimationCurve currentCurve;

		[FoldoutGroup("Dance path", 0)]
		public float secondsToFullLoop;

		[FoldoutGroup("Dance path", 0)]
		public float pathOffset;

		[FoldoutGroup("Boss attacks ref", 0)]
		public BossInstantProjectileAttack spearAttack;

		[FoldoutGroup("Boss attacks ref", 0)]
		public BossSplineFollowingProjectileAttack maceAttack;

		[FoldoutGroup("Boss attacks ref", 0)]
		public BossEnemySpawn shieldAttack;

		[FoldoutGroup("Boss attacks ref", 0)]
		public SplinePointInfo maceSurroundInfo;

		[FoldoutGroup("FX", 0)]
		public GameObject spearStartFx;

		[FoldoutGroup("Boss attacks config ", true, 0)]
		public List<SingleAnguishBehaviour.SingleAnguishAttackConfig> attacksConfiguration;

		[FoldoutGroup("Boss attacks config ", true, 0)]
		public AnguishBossfightConfig bossfightConfig;

		[FoldoutGroup("Debug", true, 0)]
		public BOSS_STATES currentState;

		public FloatingWeapon spear;

		public FloatingWeapon mace;

		public FloatingWeapon shield;

		public GhostTrailGenerator ghostTrail;

		public ScrollableModulesManager scrollManager;

		private SingleAnguish SingleAnguish;

		private Transform currentTarget;

		private StateMachine<SingleAnguishBehaviour> _fsm;

		private State<SingleAnguishBehaviour> stDance;

		private State<SingleAnguishBehaviour> stGoToDancePoint;

		private State<SingleAnguishBehaviour> stGoToMergePoint;

		private State<SingleAnguishBehaviour> stGoToComboPoint;

		private State<SingleAnguishBehaviour> stAction;

		private State<SingleAnguishBehaviour> stDeath;

		private State<SingleAnguishBehaviour> stMerged;

		private State<SingleAnguishBehaviour> stIntro;

		private Coroutine currentCoroutine;

		private BossAttackWarning attackWarning;

		private Vector2 _currentTargetPoint;

		private int _multiAttacksRemaining;

		private float _updateCounter;

		private bool followScroll;

		[Serializable]
		public struct SingleAnguishAttackConfig
		{
			public SingleAnguishBehaviour.ANGUISH_ATTACKS attackType;

			public float preparationSeconds;

			public float waitingSecondsAfterAttack;
		}

		public enum ANGUISH_ATTACKS
		{
			SPEAR,
			SHIELD,
			MACE
		}

		public enum SINGLE_ANGUISH_STATES
		{
			DANCE,
			GO_TO_DANCE_POINT,
			STOP,
			GO_TO_MERGE_POINT,
			MERGED
		}
	}
}
