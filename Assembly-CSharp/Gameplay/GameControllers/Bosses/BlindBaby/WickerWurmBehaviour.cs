using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using BezierSplines;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.BlindBaby.AI;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BlindBaby
{
	public class WickerWurmBehaviour : EnemyBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<WickerWurmBehaviour> OnActionFinished;

		public SplineFollower SplineFollower { get; set; }

		public BodyChainMaster ChainMaster { get; set; }

		public WickerWurm WickerWurm { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.currentlyAvailableAttacks = new List<WickerWurmBehaviour.WICKERWURM_ATTACKS>
			{
				WickerWurmBehaviour.WICKERWURM_ATTACKS.MOVEMENT,
				WickerWurmBehaviour.WICKERWURM_ATTACKS.MULTI_ATTACK
			};
			this.availableMultiAttacks = new List<WickerWurmBehaviour.MULTI_ATTACKS>
			{
				WickerWurmBehaviour.MULTI_ATTACKS.TACKLE,
				WickerWurmBehaviour.MULTI_ATTACKS.BOOMERANG,
				WickerWurmBehaviour.MULTI_ATTACKS.BOUNCING
			};
			this.queuedActions = new List<WickerWurmBehaviour.WICKERWURM_ATTACKS>();
		}

		public override void OnStart()
		{
			base.OnStart();
			this.WickerWurm = (WickerWurm)this.Entity;
			this.SplineFollower = base.GetComponent<SplineFollower>();
			this.ChainMaster = base.GetComponent<BodyChainMaster>();
			this.ChangeBossState(BOSS_STATES.WAITING);
			this.stIntro = new WickerWurm_StIntro();
			this.stFixed = new WickerWurm_StFixed();
			this.stMoving = new WickerWurm_StMoving();
			this.stStun = new WickerWurm_StStun();
			this.stDead = new WickerWurm_StDeath();
			this._fsm = new StateMachine<WickerWurmBehaviour>(this, this.stIntro, null, null);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
			if (this.lookAtPlayer)
			{
				this.UpdateLookAtPlayer();
			}
		}

		public void StartIntroSequence()
		{
			this._fsm.ChangeState(this.stIntro);
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.IntroSequenceCoroutine()));
		}

		private IEnumerator IntroSequenceCoroutine()
		{
			this.WickerWurm.Audio.PlayBabyAppear_AUDIO();
			this.blindBaby.BabyIntro();
			yield return new WaitForSeconds(3f);
			this.ChangePhase(WickerWurmBehaviour.WICKERWURM_PHASES.FIRST);
			this.MoveRightIntro();
			yield return new WaitForSeconds(1f);
			yield return base.StartCoroutine(this.WaitForState(this.stFixed));
			this.lastAttack = WickerWurmBehaviour.WICKERWURM_ATTACKS.MOVEMENT;
			base.BehaviourTree.StartBehaviour();
			this.PushToActionQueue(WickerWurmBehaviour.WICKERWURM_ATTACKS.BABY_GRAB);
			this.StartWaitingPeriod(0.1f);
			yield break;
		}

		private void Shake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 1f, 12, 0.2f, 0f, default(Vector3), 0f, false);
		}

		private void Wave()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.7f, 0.3f, 2f);
		}

		public float GetHealthPercentage()
		{
			return this.WickerWurm.CurrentLife / this.WickerWurm.Stats.Life.Base;
		}

		private void SetPhase(WickerWurmBehaviour.WickerWurmPhases p)
		{
			this.currentlyAvailableAttacks = p.availableAttacks;
			this._currentPhase = p.phaseId;
		}

		private void OnSpawnFinished()
		{
		}

		private void ChangePhase(WickerWurmBehaviour.WICKERWURM_PHASES p)
		{
			WickerWurmBehaviour.WickerWurmPhases phase = this.phases.Find((WickerWurmBehaviour.WickerWurmPhases x) => x.phaseId == p);
			this.SetPhase(phase);
		}

		private void CheckNextPhase()
		{
			float healthPercentage = this.GetHealthPercentage();
			WickerWurmBehaviour.WICKERWURM_PHASES currentPhase = this._currentPhase;
			if (currentPhase != WickerWurmBehaviour.WICKERWURM_PHASES.FIRST)
			{
				if (currentPhase != WickerWurmBehaviour.WICKERWURM_PHASES.SECOND)
				{
					if (currentPhase != WickerWurmBehaviour.WICKERWURM_PHASES.LAST)
					{
					}
				}
				else if (healthPercentage < 0.5f)
				{
					this.ChangePhase(WickerWurmBehaviour.WICKERWURM_PHASES.LAST);
				}
			}
			else if (healthPercentage < 0.75f)
			{
				this.ChangePhase(WickerWurmBehaviour.WICKERWURM_PHASES.SECOND);
			}
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

		public void LaunchAction(WickerWurmBehaviour.WICKERWURM_ATTACKS atk)
		{
			this.lastAttack = atk;
			switch (atk)
			{
			case WickerWurmBehaviour.WICKERWURM_ATTACKS.MOVEMENT:
				this.MoveToOtherSide();
				break;
			case WickerWurmBehaviour.WICKERWURM_ATTACKS.MULTI_ATTACK:
				this.IssueMultiAttack();
				this.multiAttackCounter++;
				break;
			case WickerWurmBehaviour.WICKERWURM_ATTACKS.BABY_GRAB:
				this.multiAttackCounter = 0;
				this.IssueBabyGrabAttack();
				break;
			case WickerWurmBehaviour.WICKERWURM_ATTACKS.HEAD_BOB:
				this.IssueHeadBob();
				break;
			case WickerWurmBehaviour.WICKERWURM_ATTACKS.TAIL_COMBO:
				this.IssueTailStingCombo();
				break;
			}
		}

		private void PushToActionQueue(WickerWurmBehaviour.WICKERWURM_ATTACKS atk)
		{
			this.queuedActions.Add(atk);
		}

		private WickerWurmBehaviour.WICKERWURM_ATTACKS PopFromActionQueue()
		{
			WickerWurmBehaviour.WICKERWURM_ATTACKS wickerwurm_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(wickerwurm_ATTACKS);
			return wickerwurm_ATTACKS;
		}

		public WickerWurmBehaviour.WICKERWURM_ATTACKS GetNewAttack()
		{
			if (this.queuedActions.Count > 0)
			{
				return this.PopFromActionQueue();
			}
			WickerWurmBehaviour.WICKERWURM_ATTACKS[] array = new WickerWurmBehaviour.WICKERWURM_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<WickerWurmBehaviour.WICKERWURM_ATTACKS> list = new List<WickerWurmBehaviour.WICKERWURM_ATTACKS>(array);
			list.Remove(this.lastAttack);
			WickerWurmBehaviour.WICKERWURM_ATTACKS result = list[Random.Range(0, list.Count)];
			if (this.multiAttackCounter == 2)
			{
				result = WickerWurmBehaviour.WICKERWURM_ATTACKS.BABY_GRAB;
			}
			return result;
		}

		public void LaunchRandomAction()
		{
			if (!this.WickerWurm.Status.Dead)
			{
				this.LaunchAction(this.GetNewAttack());
			}
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public IEnumerator WaitForState(State<WickerWurmBehaviour> st)
		{
			while (!this._fsm.IsInState(st))
			{
				yield return null;
			}
			yield break;
		}

		private IEnumerator GetIntoStateAndCallback(State<WickerWurmBehaviour> newSt, float waitSeconds, Action callback)
		{
			this._fsm.ChangeState(newSt);
			yield return new WaitForSeconds(waitSeconds);
			if (callback != null)
			{
				callback();
			}
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

		private void IssueTackle()
		{
			this.ChainMaster.SnakeAttack(this.snakeAttackOffset, null);
		}

		private void IssueHeadBob()
		{
			this.StartAttackAction();
			this.lookAtPlayer = true;
			this.SetCurrentCoroutine(base.StartCoroutine(this.HeadBobCoroutine(this.headbobPathConfig, 2, new Action(this.EndHeadBob))));
		}

		private void IssueIdleHeadBob()
		{
			this.lookAtPlayer = true;
			this.moveHead = true;
			if (this.currentHeadBobCoroutine != null)
			{
				base.StopCoroutine(this.currentHeadBobCoroutine);
			}
			this.currentHeadBobCoroutine = base.StartCoroutine(this.SimpleHeadBobCoroutine(this.headbobPathConfig, new Action(this.EndHeadBob)));
		}

		private void EndSimpleHeadBob()
		{
			this.lookAtPlayer = false;
		}

		private void EndHeadBob()
		{
			this.lookAtPlayer = false;
			this.StartWaitingPeriod(1f);
		}

		private void StartNewHeadbobLoop()
		{
			this.currentBobSpline.spline.gameObject.transform.position = base.transform.position;
			this.WickerWurm.Audio.PlaySnakeLongMove_AUDIO();
			this.SetFirstPointToPosition(this.currentBobSpline.spline, true);
			this.SetSplineConfig(this.currentBobSpline);
		}

		private IEnumerator WaitHeadbobSplineCompleted()
		{
			while (!this.SplineFollower.HasFinished() && this.moveHead)
			{
				yield return null;
			}
			yield break;
		}

		private IEnumerator SimpleHeadBobCoroutine(BlindBabyPoints.WickerWurmPathConfig headbobSpline, Action OnFinish = null)
		{
			this.moveHead = true;
			bool right = this.currentSide == WickerWurmBehaviour.WICKERWURM_SIDES.RIGHT;
			this.currentBobSpline = headbobSpline;
			List<Transform> allPoints = this.bossfightPoints.GetMultiAttackPoints(this.currentSide);
			Vector2 point = allPoints[0].position;
			Vector2 point2 = allPoints[1].position;
			Vector2 point3 = allPoints[2].position;
			bool first = true;
			while (this.moveHead)
			{
				if (first)
				{
					Tween tween = this.ChainMaster.MoveWithEase(point2, 0.5f, 6, null);
					yield return TweenExtensions.WaitForCompletion(tween);
					first = false;
				}
				this.StartNewHeadbobLoop();
				yield return base.StartCoroutine(this.WaitHeadbobSplineCompleted());
			}
			if (OnFinish != null)
			{
				OnFinish();
			}
			yield break;
		}

		private void IssueTailStingCombo()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.TailStingCombo()));
		}

		private void ShootBall()
		{
			Vector2 vector = Vector2.right;
			if (this.currentSide == WickerWurmBehaviour.WICKERWURM_SIDES.RIGHT)
			{
				vector *= -1f;
			}
			vector += Vector2.up * 0.2f;
			this.bouncingAttack.Shoot(vector);
			ShortcutExtensions.DOPunchPosition(base.transform, -vector, 0.2f, 10, 1f, false);
		}

		private IEnumerator ShootBallCoroutine(int balls, float delayBeforeBall, float delayBeforeClosingMouth)
		{
			this.SplineFollower.followActivated = false;
			this.WickerWurm.AnimatorInyector.PlayGrowl();
			yield return new WaitForSeconds(0.7f);
			this.SplineFollower.followActivated = false;
			this.WickerWurm.AnimatorInyector.SetOpen(true);
			for (int i = 0; i < balls; i++)
			{
				yield return new WaitForSeconds(delayBeforeBall);
				this.ShootBall();
			}
			yield return new WaitForSeconds(delayBeforeClosingMouth);
			this.lookAtPlayer = true;
			this.WickerWurm.AnimatorInyector.SetOpen(false);
			yield break;
		}

		private IEnumerator ShootBoomerangCoroutine(int balls, float delayBeforeShooting, float delayBeforeClosingMouth)
		{
			this.SplineFollower.followActivated = false;
			this.WickerWurm.AnimatorInyector.PlayGrowl();
			yield return new WaitForSeconds(0.5f);
			this.SplineFollower.followActivated = false;
			this.WickerWurm.AnimatorInyector.SetOpen(true);
			for (int i = 0; i < balls; i++)
			{
				yield return new WaitForSeconds(delayBeforeShooting);
				this.boomerangAttack.Shoot(Core.Logic.Penitent.transform);
			}
			yield return new WaitForSeconds(delayBeforeClosingMouth);
			this.lookAtPlayer = true;
			this.WickerWurm.AnimatorInyector.SetOpen(false);
			yield break;
		}

		private IEnumerator TailStingCombo()
		{
			bool right = this.currentSide == WickerWurmBehaviour.WICKERWURM_SIDES.RIGHT;
			float maxSecondsToAttack = 3f;
			float minSecondsToAttack = 2f;
			this.IssueIdleHeadBob();
			for (int i = 3; i > 0; i--)
			{
				Vector2 tailPoint = Core.Logic.Penitent.transform.position;
				this.tailAttack.ShowTail(tailPoint, right, 0f);
				float r = Random.Range(minSecondsToAttack, maxSecondsToAttack);
				yield return new WaitForSeconds(r - 1f);
				this.lookAtPlayer = false;
				yield return base.StartCoroutine(this.ShootBallCoroutine(3, 0.75f, 0.4f));
				this.SplineFollower.followActivated = true;
				this.QuickStingAttackToPoint(right, tailPoint);
				yield return new WaitForSeconds(3.5f);
			}
			this.moveHead = false;
			this.StartWaitingPeriod(2f);
			yield break;
		}

		private IEnumerator HeadBobCoroutine(BlindBabyPoints.WickerWurmPathConfig headbobSpline, int loops, Action OnFinish = null)
		{
			int i = 0;
			bool right = this.currentSide == WickerWurmBehaviour.WICKERWURM_SIDES.RIGHT;
			float maxSecondsToAttack = 5f;
			float minSecondsToAttack = 3f;
			float counter = Random.Range(minSecondsToAttack, maxSecondsToAttack);
			while (i < loops)
			{
				headbobSpline.spline.gameObject.transform.position = base.transform.position;
				this.WickerWurm.Audio.PlaySnakeLongMove_AUDIO();
				this.SetFirstPointToPosition(headbobSpline.spline, true);
				this.SetSplineConfig(headbobSpline);
				yield return new WaitForSeconds(this.SplineFollower.duration);
				i++;
				if (i == loops - 1)
				{
					this.WickerWurm.AnimatorInyector.PlayGrowl();
					this.blindBaby.StartGrabAttack();
				}
			}
			Vector2 tailPoint = Core.Logic.Penitent.transform.position;
			this.tailAttack.ShowTail(tailPoint, right, 0f);
			yield return new WaitForSeconds(0.4f);
			this.WickerWurm.AnimatorInyector.PlayAttack();
			yield return new WaitForSeconds(0.4f);
			this.AttackWithTailStingToPoint(right, tailPoint);
			yield return new WaitForSeconds(6f);
			if (OnFinish != null)
			{
				OnFinish();
			}
			yield break;
		}

		private void AttackWithTailStingToPoint(bool right, Vector2 p)
		{
			Vector2 vector = Vector2.right * 2f * (float)((!right) ? 1 : -1);
			base.StartCoroutine(this.DelayedFunction(new Action(this.WickerWurm.Audio.PlayScorpion1_AUDIO), 0f));
			base.StartCoroutine(this.DelayedFunction(new Action(this.WickerWurm.Audio.PlayScorpion2_AUDIO), 1.5f));
			base.StartCoroutine(this.DelayedFunction(new Action(this.WickerWurm.Audio.PlayScorpionHit_AUDIO), 2f));
			base.StartCoroutine(this.DelayedTailAttack(this.tailAttack, p, 0f, right, 4f));
			base.StartCoroutine(this.DelayedTailAttack(this.tailAttackTop, p + vector, 1f, right, 1f));
		}

		private void AttackWithTailSting(bool right)
		{
			Vector3 vector = Vector2.right * 2f * (float)((!right) ? 1 : -1);
			base.StartCoroutine(this.DelayedFunction(new Action(this.WickerWurm.Audio.PlayScorpion1_AUDIO), 0f));
			base.StartCoroutine(this.DelayedFunction(new Action(this.WickerWurm.Audio.PlayScorpion2_AUDIO), 1.5f));
			base.StartCoroutine(this.DelayedFunction(new Action(this.WickerWurm.Audio.PlayScorpionHit_AUDIO), 2f));
			base.StartCoroutine(this.DelayedTailAttack(this.tailAttack, Core.Logic.Penitent.transform.position, 0f, right, 4f));
			base.StartCoroutine(this.DelayedTailAttack(this.tailAttackTop, Core.Logic.Penitent.transform.position + vector, 1f, right, 1f));
		}

		private void QuickStingAttack(bool right)
		{
			this.WickerWurm.Audio.PlayScorpion1_AUDIO();
			base.StartCoroutine(this.DelayedTailAttack(this.tailAttack, Core.Logic.Penitent.transform.position, 0f, right, 0.5f));
		}

		private void QuickStingAttackToPoint(bool right, Vector2 p)
		{
			this.WickerWurm.Audio.PlayScorpion1_AUDIO();
			base.StartCoroutine(this.DelayedTailAttack(this.tailAttack, p, 0f, right, 0.5f));
		}

		private IEnumerator DelayedFunction(Action function, float delay)
		{
			yield return new WaitForSeconds(delay);
			function();
			yield break;
		}

		private IEnumerator DelayedTailAttack(WickerWurmTailAttack atk, Vector3 point, float seconds, bool right, float delay)
		{
			yield return new WaitForSeconds(seconds);
			atk.TailAttack(point, right, delay);
			yield break;
		}

		public void MoveToOtherSide()
		{
			this.StartAttackAction();
			if (this.currentSide == WickerWurmBehaviour.WICKERWURM_SIDES.RIGHT)
			{
				this.MoveLeft();
			}
			else
			{
				this.MoveRight();
			}
		}

		public void MoveRightIntro()
		{
			this.WickerWurm.Audio.PlayAlive_AUDIO();
			BlindBabyPoints.WickerWurmPathConfig pathConfig = this.bossfightPoints.GetPathConfig(0);
			base.StartCoroutine(this.MoveToFixPosition(pathConfig, this.bossfightPoints.GetPathConfig(BlindBabyPoints.WURM_PATHS.RIGHT_TO_FIX), null));
		}

		public void MoveRight()
		{
			base.StartCoroutine(this.MoveToFixPosition(this.bossfightPoints.GetPathConfig(BlindBabyPoints.WURM_PATHS.TO_RIGHT), this.bossfightPoints.GetPathConfig(BlindBabyPoints.WURM_PATHS.RIGHT_TO_FIX), new Action(this.OnMovementManeouverFinished)));
		}

		public void MoveLeft()
		{
			base.StartCoroutine(this.MoveToFixPosition(this.bossfightPoints.GetPathConfig(BlindBabyPoints.WURM_PATHS.TO_LEFT), this.bossfightPoints.GetPathConfig(BlindBabyPoints.WURM_PATHS.LEFT_TO_FIX), new Action(this.OnMovementManeouverFinished)));
		}

		private void SetFirstPointToPosition(BezierSpline spline, bool alsoLast = false)
		{
			Vector2 vector = spline.points[1] - spline.points[0];
			spline.points[0] = spline.transform.InverseTransformPoint(base.transform.position);
			spline.points[1] = spline.points[0] + vector;
			if (alsoLast)
			{
				int num = spline.points.Length - 1;
				Vector2 vector2 = spline.points[num - 1] - spline.points[num];
				spline.points[num] = spline.transform.InverseTransformPoint(base.transform.position);
			}
		}

		private IEnumerator MoveToFixPosition(BlindBabyPoints.WickerWurmPathConfig movementSplineConfig, BlindBabyPoints.WickerWurmPathConfig fixSplineConfig, Action OnFinish = null)
		{
			bool right = this.currentSide == WickerWurmBehaviour.WICKERWURM_SIDES.LEFT;
			this._fsm.ChangeState(this.stMoving);
			this.WickerWurm.Audio.PlaySnakeLongMove_AUDIO();
			this.SetFirstPointToPosition(movementSplineConfig.spline, false);
			this.ChainMaster.FlipAllSprites(!right);
			this.SetSplineConfig(movementSplineConfig);
			yield return new WaitForSeconds(this.SplineFollower.duration);
			this.WickerWurm.Audio.PlayPreAttack_AUDIO();
			this.ChainMaster.FlipAllSprites(right);
			this.SetSplineConfig(fixSplineConfig);
			yield return new WaitForSeconds(this.SplineFollower.duration);
			this._fsm.ChangeState(this.stFixed);
			this.currentSide = fixSplineConfig.side;
			if (OnFinish != null)
			{
				OnFinish();
			}
			yield break;
		}

		private void OnMovementManeouverFinished()
		{
			this.StartWaitingPeriod(0.2f);
		}

		private void SetSplineConfig(BlindBabyPoints.WickerWurmPathConfig config)
		{
			this.SplineFollower.spline = config.spline;
			this.SplineFollower.currentCounter = 0f;
			this.SplineFollower.duration = config.duration;
			this.SplineFollower.movementCurve = config.curve;
			this.SplineFollower.followActivated = true;
		}

		private void IssueBabyGrabAttack()
		{
			this.StartAttackAction();
			this.IssueIdleHeadBob();
			this.SetCurrentCoroutine(base.StartCoroutine(this.BabyGrabSequence()));
		}

		private IEnumerator BabyGrabSequence()
		{
			float dir = (float)((this.currentSide != WickerWurmBehaviour.WICKERWURM_SIDES.LEFT) ? -1 : 1);
			this.blindBaby.StartGrabAttack();
			this._fsm.ChangeState(this.stStun);
			yield return new WaitForSeconds(7f);
			this._fsm.ChangeState(this.stFixed);
			this.moveHead = false;
			this.StartWaitingPeriod(0.5f);
			yield break;
		}

		private void IssueMultiAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.MultiAttackCoroutine()));
		}

		private IEnumerator MultiAttackCoroutine()
		{
			List<WickerWurmBehaviour.MULTI_ATTACKS> list = new List<WickerWurmBehaviour.MULTI_ATTACKS>();
			list.Add(WickerWurmBehaviour.MULTI_ATTACKS.BOOMERANG);
			list.Add(WickerWurmBehaviour.MULTI_ATTACKS.BOUNCING);
			list.Add(WickerWurmBehaviour.MULTI_ATTACKS.TACKLE);
			int counter = 3;
			List<Transform> allPoints = this.bossfightPoints.GetMultiAttackPoints(this.currentSide);
			Vector2 point = allPoints[0].position;
			Vector2 point2 = allPoints[1].position;
			Vector2 point3 = allPoints[2].position;
			Debug.DrawLine(point, point + Vector2.up, Color.red, 2f);
			Debug.DrawLine(point2, point2 + Vector2.up, Color.yellow, 2f);
			Debug.DrawLine(point3, point3 + Vector2.up, Color.green, 2f);
			List<Vector2> points = new List<Vector2>
			{
				point,
				point2,
				point3
			};
			int r = Random.Range(0, this.availableMultiAttacks.Count);
			for (int i = 0; i < counter; i++)
			{
				this.WickerWurm.Audio.PlaySnakeMove_AUDIO();
				Tween tween = this.ChainMaster.MoveWithEase(points[i], 0.5f, 7, null);
				yield return TweenExtensions.WaitForCompletion(tween);
				WickerWurmBehaviour.MULTI_ATTACKS curAttack = this.availableMultiAttacks[r];
				if (curAttack == WickerWurmBehaviour.MULTI_ATTACKS.BOUNCING)
				{
					yield return this.ShootBallCoroutine(3, 0.6f, 0.6f);
				}
				else if (curAttack == WickerWurmBehaviour.MULTI_ATTACKS.BOOMERANG)
				{
					yield return this.ShootBoomerangCoroutine(1, 1f, 0.4f);
				}
				else
				{
					this.LaunchSingleAttack(curAttack);
				}
				yield return new WaitForSeconds(this.multiAttacksConfig.Find((WickerWurmBehaviour.MultiAttackConfig x) => x.atk == curAttack).waitingTimeAfterAttack);
			}
			this.StartWaitingPeriod(0.2f);
			yield break;
		}

		private void SnakeAttackCallback()
		{
			this.WickerWurm.Audio.PlayAttack_AUDIO();
		}

		private void LaunchSingleAttack(WickerWurmBehaviour.MULTI_ATTACKS atk)
		{
			if (atk != WickerWurmBehaviour.MULTI_ATTACKS.TACKLE)
			{
				if (atk != WickerWurmBehaviour.MULTI_ATTACKS.BOOMERANG)
				{
					if (atk != WickerWurmBehaviour.MULTI_ATTACKS.BOUNCING)
					{
					}
				}
			}
			else
			{
				this.ChainMaster.SnakeAttack(this.snakeAttackOffset, new Action(this.SnakeAttackCallback));
			}
		}

		public void EnterStun()
		{
		}

		public void ExitStun()
		{
		}

		public void UpdateLookAtPlayer()
		{
			Vector2 vector = Core.Logic.Penitent.transform.position + Vector3.up * 3.5f;
			if ((this.currentSide == WickerWurmBehaviour.WICKERWURM_SIDES.RIGHT && vector.x < base.transform.position.x - 2f) || (this.currentSide == WickerWurmBehaviour.WICKERWURM_SIDES.LEFT && vector.x < base.transform.position.x + 2f))
			{
				this.ChainMaster.LookAtTarget(vector, 2f);
			}
			else
			{
				this.ChainMaster.LookAtTarget(base.transform.position + Vector3.right * (float)((this.currentSide != WickerWurmBehaviour.WICKERWURM_SIDES.RIGHT) ? 1 : -1), 2f);
			}
		}

		public void UpdateLookAtPath()
		{
			this.ChainMaster.LookAtTarget(base.transform.position + this.SplineFollower.GetDirection(), 5f);
		}

		public void EnableDamage(bool enable)
		{
			this.WickerWurm.DamageArea.DamageAreaCollider.enabled = enable;
		}

		public void SetMoving(bool moving)
		{
			this.SplineFollower.followActivated = moving;
		}

		public void AffixBody(bool affix)
		{
			this.ChainMaster.AffixBody(affix, 13);
		}

		public void StartDeathSequence()
		{
			this.ClearAll();
			this.WickerWurm.AnimatorInyector.Death();
			this.WickerWurm.Audio.PlayDeath_AUDIO();
			this.blindBaby.PlayDeath();
			this.SetCurrentCoroutine(base.StartCoroutine(this.DeathSequenceCoroutine()));
		}

		private void ClearAll()
		{
			base.StopAllCoroutines();
			this.blindBaby.StopAllCoroutines();
		}

		private IEnumerator DeathSequenceCoroutine()
		{
			Debug.Log("STARTING DEATH SEQUENCE");
			this.StartAttackAction();
			this.WickerWurm.Behaviour.StopBehaviour();
			this.ChainMaster.StartDeathSequence();
			this.deathEffects.ActivateEffects();
			this.WickerWurm.Audio.PlayFire_AUDIO();
			Core.Logic.Penitent.Status.Invulnerable = true;
			Core.Logic.Penitent.Status.Unattacable = true;
			yield return new WaitForSeconds(6f);
			Core.Logic.Penitent.Status.Invulnerable = false;
			Core.Logic.Penitent.Status.Unattacable = false;
			yield break;
		}

		public void Death()
		{
			this._fsm.ChangeState(this.stDead);
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
			this.WickerWurm.Audio.PlayHit_AUDIO();
			this.CheckNextPhase();
			if (this.CanBeRepulloed())
			{
				this.ChainMaster.Repullo();
				this.StartWaitingPeriod(1.5f);
			}
		}

		public bool HasGrabbedPenitent()
		{
			return this.blindBaby.HasGrabbedPenitent();
		}

		private bool CanBeRepulloed()
		{
			return this._fsm.IsInState(this.stFixed) && this.ChainMaster.IsAttacking;
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public void OnShootPointTouched()
		{
			this.shootingAttack.Shoot(Vector2.down);
		}

		public void OnDrawGizmos()
		{
		}

		[FoldoutGroup("Debug", true, 0)]
		public BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public WickerWurmBehaviour.WICKERWURM_ATTACKS lastAttack;

		[FoldoutGroup("Debug", true, 0)]
		public bool skipIntro;

		[FoldoutGroup("Design settings", 0)]
		public List<WickerWurmBehaviour.WickerWurmPhases> phases;

		[FoldoutGroup("Design settings", 0)]
		public List<WickerWurmBehaviour.MultiAttackConfig> multiAttacksConfig;

		[FoldoutGroup("Design settings", 0)]
		public Vector2 snakeAttackOffset;

		[FoldoutGroup("References", 0)]
		public BlindBabyGrabManager blindBaby;

		[FoldoutGroup("References", 0)]
		public BlindBabyPoints bossfightPoints;

		[FoldoutGroup("References", 0)]
		public WickerWurmTailAttack tailAttack;

		[FoldoutGroup("References", 0)]
		public WickerWurmTailAttack tailAttackTop;

		[FoldoutGroup("References", 0)]
		public EffectsOnBabyDeath deathEffects;

		public BlindBabyPoints.WickerWurmPathConfig headbobPathConfig;

		public bool moveHead;

		private Coroutine currentHeadBobCoroutine;

		private Transform currentTarget;

		private StateMachine<WickerWurmBehaviour> _fsm;

		private State<WickerWurmBehaviour> stIntro;

		private State<WickerWurmBehaviour> stMoving;

		private State<WickerWurmBehaviour> stFixed;

		private State<WickerWurmBehaviour> stStun;

		private State<WickerWurmBehaviour> stDead;

		private Coroutine currentCoroutine;

		private WickerWurmBehaviour.WICKERWURM_PHASES _currentPhase;

		private List<WickerWurmBehaviour.WICKERWURM_ATTACKS> currentlyAvailableAttacks;

		private List<WickerWurmBehaviour.MULTI_ATTACKS> availableMultiAttacks;

		private List<WickerWurmBehaviour.WICKERWURM_ATTACKS> queuedActions;

		public BossStraightProjectileAttack shootingAttack;

		public BossBoomerangProjectileAttack boomerangAttack;

		public BossStraightProjectileAttack bouncingAttack;

		public WickerWurmBehaviour.WICKERWURM_SIDES currentSide;

		public bool lookAtPlayer = true;

		private int multiAttackCounter;

		private BlindBabyPoints.WickerWurmPathConfig currentBobSpline;

		[Serializable]
		public struct WickerWurmPhases
		{
			public WickerWurmBehaviour.WICKERWURM_PHASES phaseId;

			public List<WickerWurmBehaviour.WICKERWURM_ATTACKS> availableAttacks;
		}

		[Serializable]
		public struct MultiAttackConfig
		{
			public WickerWurmBehaviour.MULTI_ATTACKS atk;

			public float waitingTimeAfterAttack;
		}

		public enum WICKERWURM_PHASES
		{
			FIRST,
			SECOND,
			LAST
		}

		public enum MULTI_ATTACKS
		{
			TACKLE,
			BOOMERANG,
			BOUNCING
		}

		public enum WICKERWURM_SIDES
		{
			LEFT,
			RIGHT
		}

		public enum WICKERWURM_ATTACKS
		{
			MOVEMENT,
			MULTI_ATTACK,
			BABY_GRAB,
			HEAD_BOB,
			TAIL_COMBO
		}
	}
}
