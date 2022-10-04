using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Projectiles;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BurntFace.AI
{
	public class BurntFaceBehaviour : EnemyBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BurntFaceBehaviour> OnActionFinished;

		public BurntFace BurntFace { get; set; }

		public int HandAttackCounter { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.stEyes = new BurntFaceSt_Eyes();
			this.stHidden = new BurntFaceSt_Hidden();
			this.stHead = new BurntFaceSt_Head();
			this.stIntro = new BurntFaceSt_Intro();
			this.stDeath = new BurntFaceSt_Death();
			this.currentlyAvailableAttacks = new List<BurntFaceBehaviour.BURNTFACE_ATTACKS>
			{
				BurntFaceBehaviour.BURNTFACE_ATTACKS.ROSARY_CIRCLE,
				BurntFaceBehaviour.BURNTFACE_ATTACKS.ROSARY_CROSS
			};
			this.queuedActions = new List<BurntFaceBehaviour.BURNTFACE_ATTACKS>();
		}

		public override void OnStart()
		{
			base.OnStart();
			this.BurntFace = (BurntFace)this.Entity;
			this.ChangeBossState(BOSS_STATES.WAITING);
			this._fsm = new StateMachine<BurntFaceBehaviour>(this, this.stHidden, null, null);
			this._fsm.ChangeState(this.stHidden);
		}

		public void StartBossFight()
		{
			this._fsm.ChangeState(this.stIntro);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
		}

		public void StartIntroSequence()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.IntroSequenceCoroutine()));
		}

		private IEnumerator IntroSequenceCoroutine()
		{
			this._fsm.ChangeState(this.stEyes);
			this.Wave();
			this.ChangePhase(BurntFaceBehaviour.BURNTFACE_PHASES.FIRST);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 1f, 12, 0.2f, 0f, default(Vector3), 0.1f, false);
			yield return new WaitForSeconds(1f);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(2.5f, Vector3.down * 1f, 50, 0.2f, 0f, default(Vector3), 0.1f, false);
			base.transform.DOMoveY(base.transform.position.y + 3f, 3.5f, false);
			yield return new WaitForSeconds(1.5f);
			this._fsm.ChangeState(this.stHead);
			this.ActivateColliders(false);
			this.MoveHandBackIn(this.firstHand, 4.5f, false);
			yield return new WaitForSeconds(4.5f);
			this.HandAttackCounter = 0;
			this.ActivateColliders(true);
			this.PushToActionQueue(BurntFaceBehaviour.BURNTFACE_ATTACKS.TRIPLEBEAM);
			this.StartWaitingPeriod(0.1f);
			yield break;
		}

		private void ActivateColliders(bool activate)
		{
			this.BurntFace.DamageArea.DamageAreaCollider.enabled = activate;
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
			return this.BurntFace.CurrentLife / this.BurntFace.Stats.Life.Base;
		}

		private void SetPhase(BurntFaceBehaviour.BurntfacePhases p)
		{
			this.currentlyAvailableAttacks = p.availableAttacks;
			this._currentPhase = p.phaseId;
		}

		private void OnSpawnFinished()
		{
		}

		private void ChangePhase(BurntFaceBehaviour.BURNTFACE_PHASES p)
		{
			BurntFaceBehaviour.BurntfacePhases phase = this.phases.Find((BurntFaceBehaviour.BurntfacePhases x) => x.phaseId == p);
			this.SetPhase(phase);
		}

		private void CheckNextPhase()
		{
			float healthPercentage = this.GetHealthPercentage();
			BurntFaceBehaviour.BURNTFACE_PHASES currentPhase = this._currentPhase;
			if (currentPhase != BurntFaceBehaviour.BURNTFACE_PHASES.FIRST)
			{
				if (currentPhase != BurntFaceBehaviour.BURNTFACE_PHASES.SECOND)
				{
					if (currentPhase != BurntFaceBehaviour.BURNTFACE_PHASES.LAST)
					{
					}
				}
				else if (healthPercentage < 0.5f)
				{
					this.ChangePhase(BurntFaceBehaviour.BURNTFACE_PHASES.LAST);
					UnityEngine.Debug.Log("PUSHING RELEASE SECOND HAND INTO QUEUE");
					this.PushToActionQueue(BurntFaceBehaviour.BURNTFACE_ATTACKS.RELEASE_SECOND_HAND);
				}
			}
			else if (healthPercentage < 0.75f)
			{
				this.ChangePhase(BurntFaceBehaviour.BURNTFACE_PHASES.SECOND);
			}
		}

		private void SetCurrentCoroutine(Coroutine c)
		{
			if (this.currentCoroutine != null)
			{
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

		public void LaunchAction(BurntFaceBehaviour.BURNTFACE_ATTACKS atk)
		{
			if (atk != BurntFaceBehaviour.BURNTFACE_ATTACKS.RELEASE_SECOND_HAND && this.HandAttackCounter > 2)
			{
				this.MoveHead(this.GetNewPosition(), 0.5f);
				Core.Logic.CameraManager.ProCamera2DShake.Shake(1f, Vector3.down * 1f, 30, 0.2f, 0f, default(Vector3), 0f, false);
				this.HandAttackCounter = 0;
				return;
			}
			this.lastAttack = atk;
			switch (atk)
			{
			case BurntFaceBehaviour.BURNTFACE_ATTACKS.ROSARY_CROSS:
				this.IssueRosaryAttack(this.firstHand, "CROSS");
				if (this.useBothHands)
				{
					this.IssueRosaryAttack(this.secondHand, "CIRCLE");
				}
				this.HandAttackCounter++;
				break;
			case BurntFaceBehaviour.BURNTFACE_ATTACKS.ROSARY_CIRCLE:
				this.IssueRosaryAttack(this.firstHand, "CIRCLE");
				if (this.useBothHands)
				{
					this.IssueHomingLaser(this.secondHand);
				}
				this.HandAttackCounter++;
				break;
			case BurntFaceBehaviour.BURNTFACE_ATTACKS.ROSARY_SWEEP:
				this.IssueRosaryAttack(this.firstHand, "SWEEP");
				if (this.useBothHands)
				{
					this.IssueHomingBalls(this.secondHand);
				}
				this.HandAttackCounter++;
				break;
			case BurntFaceBehaviour.BURNTFACE_ATTACKS.ROSARY_INVERTED_CIRCLE:
				this.IssueRosaryAttack(this.firstHand, "INVERTED_CIRCLE");
				if (this.useBothHands)
				{
					this.IssueHomingBalls(this.secondHand);
				}
				this.HandAttackCounter++;
				break;
			case BurntFaceBehaviour.BURNTFACE_ATTACKS.HOMING_LASER:
				this.IssueHomingLaser(this.firstHand);
				if (this.useBothHands)
				{
					this.IssueHomingLaser(this.secondHand);
				}
				this.HandAttackCounter++;
				break;
			case BurntFaceBehaviour.BURNTFACE_ATTACKS.HOMING_BALLS:
				this.IssueHomingBalls(this.firstHand);
				if (this.useBothHands)
				{
					this.IssueRosaryAttack(this.secondHand, "CIRCLE");
				}
				this.HandAttackCounter++;
				break;
			case BurntFaceBehaviour.BURNTFACE_ATTACKS.RELEASE_SECOND_HAND:
				this.IssueReleaseSecondHand();
				break;
			case BurntFaceBehaviour.BURNTFACE_ATTACKS.TRIPLEBEAM:
				this.IssueTripleHomingLaser(this.firstHand);
				if (this.useBothHands)
				{
					this.IssueHomingLaser(this.secondHand);
				}
				this.PushToActionQueue(BurntFaceBehaviour.BURNTFACE_ATTACKS.ROSARY_INVERTED_CIRCLE);
				break;
			}
		}

		private void PushToActionQueue(BurntFaceBehaviour.BURNTFACE_ATTACKS atk)
		{
			this.queuedActions.Add(atk);
		}

		private BurntFaceBehaviour.BURNTFACE_ATTACKS PopFromActionQueue()
		{
			BurntFaceBehaviour.BURNTFACE_ATTACKS burntface_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(burntface_ATTACKS);
			UnityEngine.Debug.Log("<color=red> POPPING ACTION FROM QUEUE</color>");
			return burntface_ATTACKS;
		}

		public BurntFaceBehaviour.BURNTFACE_ATTACKS GetNewAttack()
		{
			if (this.queuedActions.Count > 0)
			{
				return this.PopFromActionQueue();
			}
			BurntFaceBehaviour.BURNTFACE_ATTACKS[] array = new BurntFaceBehaviour.BURNTFACE_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<BurntFaceBehaviour.BURNTFACE_ATTACKS> list = new List<BurntFaceBehaviour.BURNTFACE_ATTACKS>(array);
			list.Remove(this.lastAttack);
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public void LaunchRandomAction()
		{
			this.LaunchAction(this.GetNewAttack());
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public IEnumerator WaitForState(State<BurntFaceBehaviour> st)
		{
			while (!this._fsm.IsInState(st))
			{
				yield return null;
			}
			yield break;
		}

		private BOSS_POSITIONS GetNewPosition()
		{
			List<BOSS_POSITIONS> list = new List<BOSS_POSITIONS>
			{
				BOSS_POSITIONS.LEFT,
				BOSS_POSITIONS.RIGHT,
				BOSS_POSITIONS.CENTER
			};
			list.Remove(this._currentPosition);
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		private void MoveHead(BOSS_POSITIONS newPos, float waitingPeriodOnEnd)
		{
			this.StartAttackAction();
			UnityEngine.Debug.Log(">Moving head to position: " + newPos.ToString());
			this.SetCurrentCoroutine(base.StartCoroutine(this.MovingIntoRosaryPoint(newPos, waitingPeriodOnEnd)));
		}

		private IEnumerator MovingIntoRosaryPoint(BOSS_POSITIONS pos, float waitingPeriodOnEnd)
		{
			this._currentPosition = pos;
			Vector2 point = this.BurntFace.bossFightPoints.GetHeadPoint(pos.ToString()).position;
			this.MoveHandOut(2f, true);
			if (this.useBothHands)
			{
				this.MoveHandOut(2f, false);
			}
			yield return base.StartCoroutine(this.GetIntoStateAndCallback(this.stEyes, 1.1f, null));
			base.StartCoroutine(this.MoveFaceToPosition(point));
			yield return new WaitForSeconds(2.5f);
			this.OnHeadMoveFinished(waitingPeriodOnEnd);
			yield break;
		}

		private IEnumerator MoveFaceToPosition(Vector2 p)
		{
			float d = Vector2.Distance(p, base.transform.position);
			float time = 1.5f;
			if (d < 4f)
			{
				time = 1f;
			}
			yield return base.StartCoroutine(GameplayUtils.LerpMoveWithCurveCoroutine(base.transform, base.transform.position, p, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), time, null, null));
			yield break;
		}

		private void MoveHandOut(float seconds, bool rightHand = true)
		{
			BurntFaceHandBehaviour burntFaceHandBehaviour = (!rightHand) ? this.secondHand : this.firstHand;
			burntFaceHandBehaviour.Hide(seconds * 0.5f);
			UnityEngine.Debug.Log(">Moving hand out of screen");
			burntFaceHandBehaviour.MoveToPosition(new Vector2(burntFaceHandBehaviour.transform.position.x, this.BurntFace.bossFightPoints.transform.position.y - 15f), seconds, new Action<BurntFaceHandBehaviour>(this.AfterHandOutOfScreen));
		}

		private void AfterHandOutOfScreen(BurntFaceHandBehaviour hand)
		{
			UnityEngine.Debug.Log("Hand moved out of screen");
			bool flag = hand == this.secondHand;
			BOSS_POSITIONS currentPosition = this._currentPosition;
			if (currentPosition != BOSS_POSITIONS.LEFT)
			{
				if (currentPosition != BOSS_POSITIONS.CENTER)
				{
					if (currentPosition == BOSS_POSITIONS.RIGHT)
					{
						hand.SetFlipX(!flag);
					}
				}
				else
				{
					hand.SetFlipX(flag);
				}
			}
			else
			{
				hand.SetFlipX(flag);
			}
			this.MoveHandBackIn(hand, 1.5f, true);
		}

		private void DrawDebugCross(Vector2 point, Color c, float seconds)
		{
			float d = 0.6f;
			UnityEngine.Debug.DrawLine(point - Vector2.up * d, point + Vector2.up * d, c, seconds);
			UnityEngine.Debug.DrawLine(point - Vector2.right * d, point + Vector2.right * d, c, seconds);
		}

		private void MoveHandBackIn(BurntFaceHandBehaviour hand, float seconds, bool reposition = true)
		{
			hand.Show(seconds * 0.5f);
			Vector2 handPosition = this.GetHandPosition(hand);
			if (!reposition)
			{
				handPosition.x = this.firstHand.transform.position.x;
			}
			Vector2 v = new Vector2(handPosition.x, handPosition.y - 8f);
			hand.transform.position = v;
			hand.MoveToPosition(handPosition, seconds, new Action<BurntFaceHandBehaviour>(this.AfterHandBackIn));
		}

		private Vector2 GetHandPosition(BurntFaceHandBehaviour hand)
		{
			Transform rosaryPoint = this.BurntFace.bossFightPoints.GetRosaryPoint(this._currentPosition.ToString(), hand == this.secondHand);
			return rosaryPoint.position;
		}

		private void AfterHandBackIn(BurntFaceHandBehaviour h)
		{
		}

		private void OnHeadMoveFinished(float waitingPeriod)
		{
			UnityEngine.Debug.Log("<color=cyan>Head move finished. Waiting period: </color>" + waitingPeriod);
			this.SetCurrentCoroutine(base.StartCoroutine(this.GetIntoStateAndCallback(this.stHead, 0.3f, null)));
			if (waitingPeriod >= 0f)
			{
				this.StartWaitingPeriod(waitingPeriod);
			}
		}

		private void IssueRosaryAttack(BurntFaceHandBehaviour hand, string patternId)
		{
			if (!this.IsSecondHand(hand))
			{
				this.StartAttackAction();
			}
			this._currentPattern = patternId;
			Vector2 handPosition = this.GetHandPosition(hand);
			hand.MoveToPosition(handPosition, 1f, new Action<BurntFaceHandBehaviour>(this.OnHandReachedPosition));
		}

		private void OnHandReachedPosition(BurntFaceHandBehaviour hand)
		{
			if (!this.BurntFace.Status.Dead)
			{
				this.StartRosaryAttack(hand);
			}
		}

		private void StartRosaryAttack(BurntFaceHandBehaviour hand)
		{
			hand.SetMuzzleFlash(true);
			this.SetCurrentCoroutine(base.StartCoroutine(this.RosaryAttack(hand)));
		}

		private IEnumerator RosaryAttack(BurntFaceHandBehaviour hand)
		{
			yield return new WaitForSeconds(0.1f);
			hand.rosary.SetPatternFromDatabase(this._currentPattern);
			hand.rosary.OnPatternEnded += this.OnRosaryPatternEnded;
			yield break;
		}

		private void OnRosaryPatternEnded(BurntFaceRosaryManager rosary)
		{
			rosary.OnPatternEnded -= this.OnRosaryPatternEnded;
			BurntFaceHandBehaviour hand = (!(this.firstHand.rosary == rosary)) ? this.secondHand : this.firstHand;
			this.OnRosaryAttackEnds(hand);
		}

		private void OnRosaryAttackEnds(BurntFaceHandBehaviour hand)
		{
			hand.SetMuzzleFlash(false);
			if (hand == this.firstHand)
			{
				this.StartWaitingPeriod(0.1f);
			}
		}

		private void IssueTripleHomingLaser(BurntFaceHandBehaviour hand)
		{
			if (!this.IsSecondHand(hand))
			{
				this.StartAttackAction();
			}
			Vector2 handPosition = this.GetHandPosition(hand);
			this._laserAttacks = this.maxLaserAttacks;
			hand.MoveToPosition(handPosition, 0.75f, new Action<BurntFaceHandBehaviour>(this.OnHandReachedTripleLaserPosition));
		}

		private void IssueHomingLaser(BurntFaceHandBehaviour hand)
		{
			if (!this.IsSecondHand(hand))
			{
				this.StartAttackAction();
			}
			Vector2 handPosition = this.GetHandPosition(hand);
			hand.MoveToPosition(handPosition, 1f, new Action<BurntFaceHandBehaviour>(this.OnHandReachedLaserPosition));
		}

		private void OnHandReachedLaserPosition(BurntFaceHandBehaviour hand)
		{
			hand.SetMuzzleFlash(true);
			float duration = (!this.IsSecondHand(hand)) ? 2f : 1.5f;
			hand.targetedBeamAttack.DelayedTargetedBeam(Core.Logic.Penitent.transform, 0.75f, duration, EntityOrientation.Right, false);
			hand.targetedBeamAttack.OnAttackFinished += this.OnLaserAttackFinished;
		}

		private void OnHandReachedTripleLaserPosition(BurntFaceHandBehaviour hand)
		{
			hand.SetMuzzleFlash(true);
			float duration = (!this.IsSecondHand(hand)) ? 1f : 1f;
			hand.targetedBeamAttack.DelayedTargetedBeam(Core.Logic.Penitent.transform, 0.5f, duration, EntityOrientation.Right, false);
			hand.targetedBeamAttack.OnAttackFinished += this.OnNumberedLaserAttackFinished;
		}

		private bool IsSecondHand(BurntFaceHandBehaviour hand)
		{
			return hand == this.secondHand;
		}

		private void OnNumberedLaserAttackFinished(BossHomingLaserAttack laser)
		{
			BurntFaceHandBehaviour burntFaceHandBehaviour = (!(this.firstHand.targetedBeamAttack == laser)) ? this.secondHand : this.firstHand;
			burntFaceHandBehaviour.SetMuzzleFlash(false);
			burntFaceHandBehaviour.targetedBeamAttack.OnAttackFinished -= this.OnNumberedLaserAttackFinished;
			this._laserAttacks--;
			Vector2 handPosition = this.GetHandPosition(burntFaceHandBehaviour);
			if (this._laserAttacks > 0)
			{
				burntFaceHandBehaviour.MoveToPosition(handPosition, 1f, new Action<BurntFaceHandBehaviour>(this.OnHandReachedTripleLaserPosition));
			}
			else if (burntFaceHandBehaviour == this.firstHand)
			{
				this.StartWaitingPeriod(0.1f);
			}
		}

		private void OnLaserAttackFinished(BossHomingLaserAttack laser)
		{
			BurntFaceHandBehaviour burntFaceHandBehaviour = (!(this.firstHand.targetedBeamAttack == laser)) ? this.secondHand : this.firstHand;
			burntFaceHandBehaviour.SetMuzzleFlash(false);
			burntFaceHandBehaviour.targetedBeamAttack.OnAttackFinished -= this.OnLaserAttackFinished;
			if (burntFaceHandBehaviour == this.firstHand)
			{
				this.StartWaitingPeriod(0.1f);
			}
		}

		private void IssueReleaseSecondHand()
		{
			this.StartAttackAction();
			UnityEngine.Debug.Log("RELEASING SECOND HAND");
			Vector2 handPosition = this.GetHandPosition(this.firstHand);
			this.firstHand.MoveToPosition(handPosition, 1f, new Action<BurntFaceHandBehaviour>(this.OnHandReachedReleasePosition));
		}

		private void OnHandReachedReleasePosition(BurntFaceHandBehaviour hand)
		{
			this.AfterHandOutOfScreen(this.secondHand);
			this.EndReleaseHand();
		}

		private void EndReleaseHand()
		{
			this.useBothHands = true;
			this.StartWaitingPeriod(2f);
		}

		private void IssueHomingBalls(BurntFaceHandBehaviour hand)
		{
			if (!this.IsSecondHand(hand))
			{
				this.StartAttackAction();
			}
			Vector2 handPosition = this.GetHandPosition(hand);
			hand.MoveToPosition(handPosition, 1f, new Action<BurntFaceHandBehaviour>(this.OnHandReachedHomingBallsPosition));
		}

		private void OnHandReachedHomingBallsPosition(BurntFaceHandBehaviour hand)
		{
			this.StartHomingBallsAttack(hand);
		}

		private void StartHomingBallsAttack(BurntFaceHandBehaviour hand)
		{
			hand.SetMuzzleFlash(true);
			this.SetCurrentCoroutine(base.StartCoroutine(this.HomingBallsCoroutine(hand)));
		}

		private IEnumerator HomingBallsCoroutine(BurntFaceHandBehaviour hand)
		{
			yield return new WaitForSeconds(0.5f);
			int i = 8;
			i += UnityEngine.Random.Range(-1, 3);
			float minrandomX = -1f;
			float maxrandomX = 1f;
			float minrandomY = -0.5f;
			float maxrandomY = 0.2f;
			for (int j = 0; j < i; j++)
			{
				hand.homingBallsLauncher.projectileSource = hand.targetedBeamAttack.transform;
				Vector2 dirToPen = this.GetDirToPenitent(hand.targetedBeamAttack.transform.position);
				Vector2 dir = dirToPen;
				dir += new Vector2(UnityEngine.Random.Range(minrandomX, maxrandomX), UnityEngine.Random.Range(minrandomY, maxrandomY));
				StraightProjectile p = hand.homingBallsLauncher.Shoot(dir.normalized);
				AcceleratedProjectile ap = p.GetComponent<AcceleratedProjectile>();
				ap.SetAcceleration(dirToPen.normalized * 6f);
				ap.SetBouncebackData(this.damageCenterTransform, Vector2.zero, Mathf.RoundToInt(Core.Logic.Penitent.Stats.Strength.Final * 4f));
				yield return new WaitForSeconds(0.5f);
			}
			this.OnCastHomingBallsEnds(hand);
			yield break;
		}

		private Vector2 GetDirToPenitent(Vector3 from)
		{
			return Core.Logic.Penitent.transform.position + Vector3.up * 0.8f - from;
		}

		private void OnCastHomingBallsEnds(BurntFaceHandBehaviour hand)
		{
			hand.SetMuzzleFlash(false);
			if (hand == this.firstHand)
			{
				this.StartWaitingPeriod(0.1f);
			}
		}

		private IEnumerator GetIntoStateAndCallback(State<BurntFaceBehaviour> newSt, float waitSeconds, Action callback)
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

		public void SetHidingLevel(int i)
		{
			this.BurntFace.AnimatorInyector.SetHidingLevel(i);
		}

		public void EnableDamage(bool enable)
		{
			this.BurntFace.DamageArea.DamageAreaCollider.enabled = enable;
		}

		public void ShowEyes(bool show)
		{
			this.eyesGameObject.SetActive(show);
		}

		public void StartDeathSequence()
		{
			Core.Logic.Penitent.Status.Invulnerable = true;
			this.BurntFace.AnimatorInyector.Death();
			base.StopAllCoroutines();
			this.SetCurrentCoroutine(base.StartCoroutine(this.DeathSequenceCoroutine()));
		}

		private void ClearScene()
		{
			GameplayUtils.DestroyAllProjectiles();
			this.UnsubscribeFromAll();
			this.secondHand.ClearAll();
			this.firstHand.ClearAll();
		}

		private void UnsubscribeFromAll()
		{
			BurntFaceHandBehaviour burntFaceHandBehaviour = this.firstHand;
			burntFaceHandBehaviour.rosary.OnPatternEnded -= this.OnRosaryPatternEnded;
			burntFaceHandBehaviour.targetedBeamAttack.OnAttackFinished -= this.OnNumberedLaserAttackFinished;
			burntFaceHandBehaviour.targetedBeamAttack.OnAttackFinished -= this.OnLaserAttackFinished;
			burntFaceHandBehaviour = this.secondHand;
			burntFaceHandBehaviour.rosary.OnPatternEnded -= this.OnRosaryPatternEnded;
			burntFaceHandBehaviour.targetedBeamAttack.OnAttackFinished -= this.OnNumberedLaserAttackFinished;
			burntFaceHandBehaviour.targetedBeamAttack.OnAttackFinished -= this.OnLaserAttackFinished;
		}

		private IEnumerator DeathSequenceCoroutine()
		{
			this.ClearScene();
			this.StartAttackAction();
			this.BurntFace.Behaviour.StopBehaviour();
			Core.Logic.CameraManager.ProCamera2DShake.Shake(1.5f, Vector3.down * 2f, 30, 0.2f, 0f, default(Vector3), 0.1f, false);
			this.firstHand.Hide(4f);
			this.secondHand.Hide(4f);
			yield return new WaitForSeconds(3f);
			base.transform.DOMoveY(base.transform.position.y - 25f, 7.5f, false).SetEase(Ease.InOutQuad);
			yield return new WaitForSeconds(5f);
			Core.Logic.Penitent.Status.Invulnerable = false;
			yield break;
		}

		public void Death()
		{
			this.ActivateColliders(false);
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
			this.CheckNextPhase();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public bool IsPlayerInRegion(string regionName)
		{
			return this.awareness.AreaContainsPlayer(regionName);
		}

		public bool IsPlayerInAnyTrapRegion()
		{
			return this.awareness.AreaContainsPlayer("WALL_W") || this.awareness.AreaContainsPlayer("WALL_E");
		}

		public void OnDrawGizmos()
		{
			if (this._fsm != null)
			{
				if (this._fsm.IsInState(this.stEyes))
				{
					Gizmos.color = Color.yellow;
				}
				else if (this._fsm.IsInState(this.stHead))
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.black;
				}
				Gizmos.DrawWireSphere(base.transform.position, 0.5f);
			}
		}

		[FoldoutGroup("Debug", true, 0)]
		public BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public BurntFaceBehaviour.BURNTFACE_ATTACKS lastAttack;

		[FoldoutGroup("References", 0)]
		public GameObject eyesGameObject;

		[FoldoutGroup("References", 0)]
		public BossPlayerAwareness awareness;

		[FoldoutGroup("References", 0)]
		public BurntFaceHandBehaviour firstHand;

		[FoldoutGroup("References", 0)]
		public BurntFaceHandBehaviour secondHand;

		[FoldoutGroup("Design settings", 0)]
		public List<BurntFaceBehaviour.BurntfacePhases> phases;

		public Transform damageCenterTransform;

		private Transform currentTarget;

		private StateMachine<BurntFaceBehaviour> _fsm;

		private State<BurntFaceBehaviour> stHidden;

		private State<BurntFaceBehaviour> stEyes;

		private State<BurntFaceBehaviour> stHead;

		private State<BurntFaceBehaviour> stIntro;

		private State<BurntFaceBehaviour> stDeath;

		private GameObject _currentPoisonFog;

		private Coroutine currentCoroutine;

		private Vector2 _currentTargetPoint;

		private int _multiAttacksRemaining;

		private string _currentPattern;

		private BOSS_POSITIONS _currentPosition = BOSS_POSITIONS.CENTER;

		private BurntFaceBehaviour.BURNTFACE_PHASES _currentPhase;

		private List<BurntFaceBehaviour.BURNTFACE_ATTACKS> currentlyAvailableAttacks;

		private List<BurntFaceBehaviour.BURNTFACE_ATTACKS> queuedActions;

		private bool useBothHands;

		public int maxLaserAttacks = 3;

		private int _laserAttacks;

		private const float WAITING_PERIOD_AFTER_HEAD_REPOSITION = 0.5f;

		private const string DEACTIVATED_ROSARY_PATTERN = "EMPTY";

		private const float FAST_MOVEMENT_DISTANCE = 4f;

		[Serializable]
		public struct BurntFaceAttackConfig
		{
			public BurntFaceBehaviour.BURNTFACE_ATTACKS attackType;

			public float preparationSeconds;

			public float waitingSecondsAfterAttack;
		}

		[Serializable]
		public struct BurntfacePhases
		{
			public BurntFaceBehaviour.BURNTFACE_PHASES phaseId;

			public List<BurntFaceBehaviour.BURNTFACE_ATTACKS> availableAttacks;
		}

		public enum BURNTFACE_PHASES
		{
			FIRST,
			SECOND,
			LAST
		}

		public enum BURNTFACE_ATTACKS
		{
			ROSARY_CROSS,
			ROSARY_CIRCLE,
			ROSARY_SWEEP,
			ROSARY_INVERTED_CIRCLE,
			HOMING_LASER = 5,
			HOMING_BALLS,
			RELEASE_SECOND_HAND,
			TRIPLEBEAM
		}
	}
}
