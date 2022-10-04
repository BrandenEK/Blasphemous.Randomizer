using System;
using System.Collections;
using System.Collections.Generic;
using BezierSplines;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.AI;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Quirce
{
	public class QuirceBehaviour : EnemyBehaviour
	{
		[FoldoutGroup("Activation Settings", true, 0)]
		public float DistanceToTarget { get; private set; }

		public Quirce Quirce { get; private set; }

		public bool Awaken { get; private set; }

		public int multiTeleportAttackNumber { get; private set; }

		private int dashRemainings { get; set; }

		public override void OnAwake()
		{
			base.OnAwake();
			this.Quirce = (Quirce)this.Entity;
			this.currentlyAvailableAttacks = this.GetCurrentStateAttacks();
			this.ghostTrail = base.GetComponentInChildren<GhostTrailGenerator>();
			this.quickMove.SetRotatingFunction(new BossDashAttack.RotatingFunction(this.RotateToLookAt));
			PoolManager.Instance.CreatePool(this.vFXExplosion, 16);
		}

		private List<QuirceBehaviour.QUIRCE_ATTACKS> GetCurrentStateAttacks()
		{
			if (this.currentQuirceState == QuirceBehaviour.QUIRCE_STATE.SWORD)
			{
				return new List<QuirceBehaviour.QUIRCE_ATTACKS>
				{
					QuirceBehaviour.QUIRCE_ATTACKS.DASH,
					QuirceBehaviour.QUIRCE_ATTACKS.TELEPORT,
					QuirceBehaviour.QUIRCE_ATTACKS.PATH_THROW,
					QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS
				};
			}
			return new List<QuirceBehaviour.QUIRCE_ATTACKS>
			{
				QuirceBehaviour.QUIRCE_ATTACKS.MULTI_TELEPORT,
				QuirceBehaviour.QUIRCE_ATTACKS.MULTIDASH,
				QuirceBehaviour.QUIRCE_ATTACKS.SWORD_RECOVERY
			};
		}

		private void SetAttacksConfiguration()
		{
			if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.NEW_GAME_PLUS))
			{
				this.attacksConfiguration = this.ngPlusAttacksConfiguration;
			}
		}

		public override void OnStart()
		{
			base.OnStart();
			this.SetAttacksConfiguration();
			this.ChangeBossState(QuirceBehaviour.BOSS_STATES.WAITING);
			this.SetQuirceState(QuirceBehaviour.QUIRCE_STATE.SWORD);
			this.StartWaitingPeriod(1f);
			this.currentPoint = default(SplinePointInfo);
			this.currentPoint.nextValidPoints = new List<QuirceBossFightPoints.QUIRCE_FIGHT_SIDES>
			{
				QuirceBossFightPoints.QUIRCE_FIGHT_SIDES.LEFT,
				QuirceBossFightPoints.QUIRCE_FIGHT_SIDES.RIGHT
			};
			this._originAreas = this.areaSummonAttack.totalAreas;
			this._originAreaSeconds = this.areaSummonAttack.seconds;
		}

		private void LateUpdate()
		{
			this.Quirce.SpriteRenderer.gameObject.transform.localPosition = ((!this.Quirce.SpriteRenderer.flipY) ? Vector2.zero : this.reversedLocalPositionOffset);
		}

		private void ToggleShowSword(bool show)
		{
			this.sword.gameObject.SetActive(show);
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

		private void ChangeBossState(QuirceBehaviour.BOSS_STATES newState)
		{
			this.currentState = newState;
		}

		private void StartAttackAction()
		{
			this.Quirce.AnimatorInyector.ResetHurt();
			this.ChangeBossState(QuirceBehaviour.BOSS_STATES.MID_ACTION);
		}

		public int GetActionsCounter()
		{
			return this.actionsCounter;
		}

		public void ResetActionsCounter()
		{
			this.actionsCounter = 0;
		}

		public QuirceBehaviour.QUIRCE_STATE GetQUIRCE_STATE()
		{
			return this.currentQuirceState;
		}

		public QuirceBehaviour.QUIRCE_ATTACKS GetNewAttack()
		{
			QuirceBehaviour.QUIRCE_ATTACKS[] array = new QuirceBehaviour.QUIRCE_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<QuirceBehaviour.QUIRCE_ATTACKS> list = new List<QuirceBehaviour.QUIRCE_ATTACKS>(array);
			list.Remove(this.lastAttack);
			if (this.lastAttack == QuirceBehaviour.QUIRCE_ATTACKS.PATH_THROW)
			{
				list.Remove(QuirceBehaviour.QUIRCE_ATTACKS.TELEPORT);
			}
			else if (this.lastAttack == QuirceBehaviour.QUIRCE_ATTACKS.SWORD_RECOVERY)
			{
				list.Remove(QuirceBehaviour.QUIRCE_ATTACKS.TELEPORT);
			}
			list.Remove(QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS);
			list.Remove(QuirceBehaviour.QUIRCE_ATTACKS.SWORD_RECOVERY);
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public void LaunchRandomAction()
		{
			this.actionsCounter++;
			this.LaunchAction(this.GetNewAttack(), true);
		}

		public void LaunchRecoverAction()
		{
			this.LaunchAction(QuirceBehaviour.QUIRCE_ATTACKS.SWORD_RECOVERY, true);
		}

		public void LaunchTossAction()
		{
			this.LaunchAction(QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS, true);
		}

		private void QueuedActionsPush(QuirceBehaviour.QUIRCE_ATTACKS atk)
		{
			if (this.queuedActions == null)
			{
				this.queuedActions = new List<QuirceBehaviour.QUIRCE_ATTACKS>();
			}
			this.queuedActions.Add(atk);
		}

		private QuirceBehaviour.QUIRCE_ATTACKS QueuedActionsPop()
		{
			QuirceBehaviour.QUIRCE_ATTACKS quirce_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(quirce_ATTACKS);
			return quirce_ATTACKS;
		}

		public void LaunchAction(QuirceBehaviour.QUIRCE_ATTACKS atk, bool checkReposition = true)
		{
			QuirceBehaviour.QuirceAttackConfig lastAttackConfig = this.attacksConfiguration.Find((QuirceBehaviour.QuirceAttackConfig x) => x.attackType == atk);
			this._lastAttackConfig = lastAttackConfig;
			if (lastAttackConfig.requiresReposition && checkReposition)
			{
				Vector3 point = Vector3.zero;
				if (atk != QuirceBehaviour.QUIRCE_ATTACKS.DASH)
				{
					if (atk != QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS)
					{
						if (atk == QuirceBehaviour.QUIRCE_ATTACKS.PATH_THROW)
						{
							Transform hangTransform = this.Quirce.BossFightPoints.GetHangTransform(this.currentPoint.nextValidPoints);
							this.currentPoint = this.Quirce.BossFightPoints.GetHangPointInfo(hangTransform);
							point = hangTransform.position;
							this.currentHang = hangTransform;
						}
					}
					else
					{
						point = this.Quirce.BossFightPoints.GetTossPoint();
					}
				}
				else
				{
					Transform dashPointTransform = this.Quirce.BossFightPoints.GetDashPointTransform(this.currentPoint.nextValidPoints);
					point = dashPointTransform.position;
					this.currentPoint = this.Quirce.BossFightPoints.GetDashPointInfo(dashPointTransform);
				}
				this.QueuedActionsPush(atk);
				this.Reposition(point);
			}
			else
			{
				this.lastAttack = atk;
				switch (atk)
				{
				case QuirceBehaviour.QUIRCE_ATTACKS.DASH:
					this.DashAttack();
					break;
				case QuirceBehaviour.QUIRCE_ATTACKS.MULTIDASH:
					this.NDashAttack();
					break;
				case QuirceBehaviour.QUIRCE_ATTACKS.TELEPORT:
					this.TeleportAttack();
					break;
				case QuirceBehaviour.QUIRCE_ATTACKS.PATH_THROW:
					this.PathThrowAttack();
					break;
				case QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS:
					this.TossAttack();
					break;
				case QuirceBehaviour.QUIRCE_ATTACKS.MULTI_TELEPORT:
					this.MultiTeleportAttack();
					break;
				case QuirceBehaviour.QUIRCE_ATTACKS.SWORD_RECOVERY:
					this.SwordRecoveryAttack();
					break;
				}
			}
		}

		public override void Idle()
		{
			Debug.Log("Quirce: IDLE");
			this.StopMovement();
		}

		private void StartWaitingPeriod(float seconds)
		{
			this.ChangeBossState(QuirceBehaviour.BOSS_STATES.WAITING);
			if (Core.Logic.Penitent != null)
			{
				this.currentTarget = Core.Logic.Penitent.transform;
				this.LookAtTarget(this.currentTarget.position);
			}
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
			this.ChangeBossState(QuirceBehaviour.BOSS_STATES.AVAILABLE_FOR_ACTION);
		}

		public void Reposition(Vector3 point)
		{
			this.StartAttackAction();
			this.LookAtTarget(point);
			Debug.Log("Quirce: REPOSITION");
			base.StartCoroutine(this.PrepareAndReposition(point));
		}

		private IEnumerator PrepareAndReposition(Vector2 point)
		{
			this.sword.SetVisible(false);
			this.Quirce.AnimatorInyector.TeleportOut();
			float teleportTime = 0.8f;
			yield return new WaitForSeconds(teleportTime);
			this.SetReversed(this._lastAttackConfig.invertedReposition);
			base.transform.position = point;
			this.currentTarget = base.GetTarget();
			this.LookAtTarget(this.currentTarget.position);
			this.Quirce.AnimatorInyector.TeleportIn();
			this.Quirce.Audio.PlayTeleportIn();
			float teleportAnimationLength = 0.4f;
			yield return new WaitForSeconds(teleportAnimationLength);
			this.Quirce.AnimatorInyector.Landing();
			this.OnRepositionFinished();
			yield break;
		}

		private void OnRepositionFinished()
		{
			this.sword.SetVisible(true);
			QuirceBehaviour.QUIRCE_ATTACKS atk = this.QueuedActionsPop();
			this.LaunchAction(atk, false);
		}

		public void DashAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingDashCoroutine()));
		}

		private IEnumerator PreparingDashCoroutine()
		{
			this.sword.SetGhostTrail(true);
			this.ghostTrail.EnableGhostTrail = true;
			this.Quirce.Audio.PlayPreDash();
			this.Quirce.AnimatorInyector.BigDashPreparation();
			this.Quirce.IsGuarding = true;
			this.currentTarget = base.GetTarget();
			this.LookAtTarget(this.currentTarget.position);
			yield return new WaitForSeconds(this.attacksConfiguration.Find((QuirceBehaviour.QuirceAttackConfig x) => x.attackType == QuirceBehaviour.QUIRCE_ATTACKS.DASH).preparationSeconds);
			Debug.Log("Quirce: DASH ATTACK");
			this.Quirce.Audio.PlayBigDash();
			this.Quirce.AnimatorInyector.Dash(true);
			this.dashAttack.OnDashFinishedEvent += this.OnDashAttackFinished;
			this.dashAttack.OnDashBlockedEvent += this.OnDashBlocked;
			float d = (float)((this.Quirce.Status.Orientation != EntityOrientation.Right) ? -1 : 1);
			this.dashAttack.Dash(base.transform, Vector3.right * d, 20f, 0.5f, false);
			yield break;
		}

		private void OnDashBlocked(BossDashAttack obj)
		{
			Core.Logic.ScreenFreeze.Freeze(0.1f, 1f, 0f, this.Quirce.slowTimeCurve);
			this.Quirce.IsGuarding = false;
		}

		private void OnDashAttackFinished()
		{
			this.sword.SetGhostTrail(false);
			this.ghostTrail.EnableGhostTrail = false;
			this.dashAttack.OnDashBlockedEvent -= this.OnDashBlocked;
			this.dashAttack.OnDashFinishedEvent -= this.OnDashAttackFinished;
			this.Quirce.AnimatorInyector.Dash(false);
			this.Quirce.IsGuarding = false;
			this.StartWaitingPeriod(this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.DASH).waitingSecondsAfterAttack);
		}

		public void SwordRecoveryAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingSwordRecovery()));
		}

		private void SetAreaAttackValuesFromHP()
		{
			int num = 5;
			int originAreas = this._originAreas;
			float originAreaSeconds = this._originAreaSeconds;
			int num2 = Mathf.RoundToInt(Mathf.Lerp((float)num, (float)originAreas, 1f - this.GetHealthPercentage()));
			float num3 = originAreaSeconds * ((float)num2 / (float)originAreas);
			this.areaSummonAttack.totalAreas = num2;
		}

		private IEnumerator PreparingSwordRecovery()
		{
			this.Quirce.AnimatorInyector.TeleportOutSword();
			yield return new WaitForSeconds(1.6f);
			this.Quirce.transform.position = this.sword.transform.position;
			this.Quirce.SetOrientation(EntityOrientation.Right, true, false);
			this.ToggleShowSword(false);
			this.sword.SetAutoFollow(false);
			this.Quirce.AnimatorInyector.TeleportInSword();
			this.Quirce.Audio.PlayTeleportIn();
			yield return new WaitForSeconds(1.4f);
			this.ToggleShowSword(true);
			this.sword.SetVisible(false);
			Vector2 dir = (this.Quirce.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this.SetAreaAttackValuesFromHP();
			this.Quirce.Audio.PlayToss();
			this.areaSummonAttack.SummonAreas(dir);
			this.Quirce.BossFightPoints.ActivateWallMask(false);
			Debug.Log("Quirce: SWORD RECOVERY ATTACK");
			yield return new WaitForSeconds(1.6f);
			this.Quirce.transform.position = this.Quirce.BossFightPoints.GetCenter();
			this.Quirce.AnimatorInyector.TeleportIn();
			this.Quirce.Audio.PlayTeleportIn();
			this.sword.SetAutoFollow(true);
			yield return new WaitForSeconds(0.4f);
			this.sword.SetVisible(true);
			this.OnSwordRecoveryAttackFinished();
			yield break;
		}

		private void OnSwordRecoveryAttackFinished()
		{
			QuirceBehaviour.QuirceAttackConfig attackConfig = this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.SWORD_RECOVERY);
			this.SetQuirceState(QuirceBehaviour.QUIRCE_STATE.SWORD);
			this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack);
		}

		private IEnumerator RandomExplosions(float seconds, int totalExplosions, Transform center, float radius, GameObject poolableExplosion, Action OnExplosion = null, Action callback = null)
		{
			float counter = 0f;
			int expCounter = 0;
			while (counter < seconds)
			{
				counter += Time.deltaTime;
				float expRatio = (float)expCounter / (float)totalExplosions;
				if (counter / seconds > expRatio)
				{
					expCounter++;
					Vector2 v = center.position + new Vector3(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius));
					PoolManager.Instance.ReuseObject(poolableExplosion, v, Quaternion.identity, false, 1);
					if (OnExplosion != null)
					{
						OnExplosion();
					}
				}
				yield return null;
			}
			if (callback != null)
			{
				callback();
			}
			yield break;
		}

		public void NDashAttack()
		{
			this.StartAttackAction();
			this.dashRemainings = this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.MULTIDASH).multiAttackTimes;
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingMultiDash()));
		}

		private IEnumerator PreparingMultiDash()
		{
			this.currentTarget = base.GetTarget();
			this.LookAtTarget(this.currentTarget.position);
			float d = Mathf.Sign(this.currentTarget.position.x - base.transform.position.x);
			float waitSeconds = this.attacksConfiguration.Find((QuirceBehaviour.QuirceAttackConfig x) => x.attackType == QuirceBehaviour.QUIRCE_ATTACKS.MULTIDASH).preparationSeconds;
			base.StartCoroutine(this.RandomExplosions(waitSeconds * 0.8f, 20, base.transform, 0.6f, this.vFXExplosion, null, null));
			this.ghostTrail.EnableGhostTrail = true;
			this.Quirce.Audio.PlayPreDash();
			this.Quirce.AnimatorInyector.BigDashPreparation();
			yield return new WaitForSeconds(waitSeconds);
			Debug.Log("Quirce: NDASH ATTACK");
			this.Quirce.AnimatorInyector.Dash(true);
			this.Quirce.Audio.PlaySwordlessDash();
			this.multiDashAttack.OnDashFinishedEvent += this.OnMultiDashAttackFinished;
			this.multiDashAttack.Dash(base.transform, Vector3.right * d, 8f, 0.5f, false);
			yield break;
		}

		private void OnMultiDashAttackFinished()
		{
			this.ghostTrail.EnableGhostTrail = false;
			this.dashRemainings--;
			this.multiDashAttack.OnDashFinishedEvent -= this.OnMultiDashAttackFinished;
			this.Quirce.AnimatorInyector.Dash(false);
			QuirceBehaviour.QuirceAttackConfig attackConfig = this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.MULTIDASH);
			if (this.dashRemainings > 0)
			{
				this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingMultiDash()));
			}
			else
			{
				this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack);
			}
		}

		public void PathThrowAttack()
		{
			SplinePointInfo hangPointInfo = this.Quirce.BossFightPoints.GetHangPointInfo(this.currentHang);
			Vector2 b = hangPointInfo.spline.GetPoint(2f) - hangPointInfo.spline.GetPoint(0f);
			this.LookAtTarget(base.transform.position - b);
			this.sword.SetSpinning(true);
			this.Quirce.Audio.PlaySpinSword();
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingPathThrow()));
		}

		private IEnumerator PreparingPathThrow()
		{
			yield return new WaitForSeconds(this.attacksConfiguration.Find((QuirceBehaviour.QuirceAttackConfig x) => x.attackType == QuirceBehaviour.QUIRCE_ATTACKS.PATH_THROW).preparationSeconds);
			this.Quirce.AnimatorInyector.Throw();
			yield return new WaitForSeconds(0.6f);
			SplinePointInfo info = this.Quirce.BossFightPoints.GetHangPointInfo(this.currentHang);
			BezierSpline spline = info.spline;
			this.splineThrowAttack.Shoot(spline, info.speedCurve, info.time, this.sword.transform.position, spline.GetPoint((float)(spline.points.Length - 1)));
			this.splineThrowAttack.OnPathFinished += this.OnProjectilePathFinished;
			this.splineThrowAttack.OnPathAdvanced += this.OnProjectilePathAdvanced;
			this.ToggleShowSword(false);
			yield break;
		}

		private void OnProjectilePathAdvanced(BossSplineFollowingProjectileAttack atk, float maxS, float elapS)
		{
			if (maxS - elapS < 1f)
			{
				atk.OnPathAdvanced -= this.OnProjectilePathAdvanced;
				this.Quirce.Audio.EndSwordSpinSound();
			}
		}

		private void OnProjectilePathFinished(BossSplineFollowingProjectileAttack obj)
		{
			obj.OnPathFinished -= this.OnProjectilePathFinished;
			this.sword.SetSpinning(false);
			this.ToggleShowSword(true);
			QuirceBehaviour.QuirceAttackConfig attackConfig = this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.PATH_THROW);
			if (!base.IsDead())
			{
				this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack);
			}
		}

		private void CreateExplosion(Vector2 p)
		{
			PoolManager.Instance.ReuseObject(this.vFXExplosion, p, Quaternion.identity, false, 1);
		}

		private IEnumerator EffectsInLine(Vector2 origin, Vector2 end, int explosions, float seconds, Action<Vector2> effectFunction)
		{
			float counter = 0f;
			int i = 0;
			while (counter < seconds)
			{
				counter += Time.deltaTime;
				if (counter / seconds > (float)i / (float)explosions)
				{
					Vector2 obj = Vector2.Lerp(origin, end, counter / seconds);
					effectFunction(obj);
					i++;
				}
				yield return null;
			}
			yield break;
		}

		public void TossAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingTossAttack()));
		}

		private void BeforeTossTeleportInWall()
		{
			this.SetReversed(false);
			this.Quirce.transform.position = this.Quirce.BossFightPoints.GetTossPoint();
			this.Quirce.SetOrientation(EntityOrientation.Left, true, false);
			this.Quirce.Audio.PlayTeleportIn();
			this.Quirce.AnimatorInyector.TeleportInSword();
		}

		private void TossAttackAnticipation()
		{
			this.ToggleShowSword(false);
			this.sword.SetAutoFollow(false);
		}

		private void AfterTossReposition()
		{
			this.Quirce.transform.position = this.Quirce.BossFightPoints.GetCenter();
			this.Quirce.Audio.PlayTeleportIn();
			this.Quirce.AnimatorInyector.TeleportIn();
			this.Quirce.AnimatorInyector.SetToss(false);
		}

		private IEnumerator PreparingTossAttack()
		{
			float timeBeforeReappear = 0.4f;
			float timeBeforeGrabsSword = 0.4f;
			float timeGrabLoopAnticipation = 0.6f;
			float timeProjectileAppears = 0.25f;
			float timeTrailAppears = 0.5f;
			float timeQuirceBackToCenter = 2f;
			yield return new WaitForSeconds(this.attacksConfiguration.Find((QuirceBehaviour.QuirceAttackConfig x) => x.attackType == QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS).preparationSeconds);
			this.Quirce.AnimatorInyector.TeleportOutSword();
			this.Quirce.AnimatorInyector.SetToss(true);
			this.sword.SetVisible(false);
			yield return new WaitForSeconds(timeBeforeReappear);
			this.sword.doFollow = true;
			this.BeforeTossTeleportInWall();
			yield return new WaitForSeconds(timeBeforeGrabsSword);
			this.TossAttackAnticipation();
			yield return new WaitForSeconds(timeGrabLoopAnticipation);
			this.Quirce.AnimatorInyector.Throw();
			this.Quirce.Audio.PlayToss();
			yield return new WaitForSeconds(timeProjectileAppears);
			Vector2 dir = (this.Quirce.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			Vector2 origin = base.transform.position + Vector2.left + Vector2.up * 0.5f;
			BezierSpline s = this.Quirce.BossFightPoints.spiralPointInfo.spline;
			AnimationCurve c = this.Quirce.BossFightPoints.spiralPointInfo.speedCurve;
			float seconds = this.Quirce.BossFightPoints.spiralPointInfo.time;
			this.splineThrowAttack.Shoot(s, c, seconds);
			base.StartCoroutine(this.EffectsInLine(origin, this.Quirce.BossFightPoints.GetSwordWallPoint().position, 10, 0.8f, new Action<Vector2>(this.CreateExplosion)));
			yield return new WaitForSeconds(timeTrailAppears);
			this.instantProjectileAttack.Shoot(origin, dir);
			this.Quirce.Audio.PlayHitWall();
			this.SetSwordOnWall();
			yield return new WaitForSeconds(timeQuirceBackToCenter);
			this.AfterTossReposition();
			yield return new WaitForSeconds(0.4f);
			this.OnSwordTossAttackFinished();
			yield break;
		}

		private void OnSwordTossAttackFinished()
		{
			QuirceBehaviour.QuirceAttackConfig attackConfig = this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS);
			this.SetQuirceState(QuirceBehaviour.QUIRCE_STATE.NO_SWORD);
			this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack);
		}

		private IEnumerator PreparingSpiral()
		{
			yield return new WaitForSeconds(this.attacksConfiguration.Find((QuirceBehaviour.QuirceAttackConfig x) => x.attackType == QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS).preparationSeconds);
			Debug.Log("Quirce: SPIRAL ATTACK");
			this.Quirce.AnimatorInyector.Spiral(true);
			SplinePointInfo info = this.Quirce.BossFightPoints.spiralPointInfo;
			BezierSpline spline = info.spline;
			this.splineThrowAttack.Shoot(spline, info.speedCurve, info.time);
			this.splineThrowAttack.OnPathFinished += this.OnSpiralPathFinished;
			this.ToggleShowSword(false);
			this.sword.SetAutoFollow(false);
			yield break;
		}

		private void OnSpiralPathFinished(BossSplineFollowingProjectileAttack obj)
		{
			obj.OnPathFinished -= this.OnSpiralPathFinished;
			this.Quirce.AnimatorInyector.Spiral(false);
			this.SetSwordOnWall();
			this.SetQuirceState(QuirceBehaviour.QUIRCE_STATE.NO_SWORD);
			QuirceBehaviour.QuirceAttackConfig attackConfig = this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.SWORD_TOSS);
			if (!base.IsDead())
			{
				this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack);
			}
		}

		private void SetSwordOnWall()
		{
			this.sword.SetSpinning(false);
			this.Quirce.BossFightPoints.ActivateWallMask(true);
			Transform swordWallPoint = this.Quirce.BossFightPoints.GetSwordWallPoint();
			this.sword.transform.SetPositionAndRotation(swordWallPoint.position, swordWallPoint.rotation);
			this.sword.SetReversed(false);
			this.ToggleShowSword(true);
		}

		public void TeleportAttack()
		{
			this.StartAttackAction();
			this.sword.SetVisible(false);
			this.currentTarget = base.GetTarget();
			this.teleportAttack.OnTeleportInEvent += this.OnTeleportIn;
			Debug.Log("Quirce: Teleport OUT");
			this.Quirce.AnimatorInyector.TeleportOut();
			this.teleportAttack.Use(base.transform, this.currentTarget, Vector3.up * 3.5f);
		}

		private void OnTeleportIn()
		{
			this.SetReversed(false);
			this.Quirce.AnimatorInyector.TeleportIn();
			this.teleportAttack.OnTeleportInEvent -= this.OnTeleportIn;
			this.PlungeAttack();
		}

		public void PlungeAttack()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparePlungeAttack()));
		}

		private IEnumerator PreparePlungeAttack()
		{
			yield return new WaitForSeconds(this.attacksConfiguration.Find((QuirceBehaviour.QuirceAttackConfig x) => x.attackType == QuirceBehaviour.QUIRCE_ATTACKS.TELEPORT).preparationSeconds);
			Debug.Log("Quirce: PLUNGE ATTACK");
			this.Quirce.AnimatorInyector.Plunge(true);
			this.currentTarget = base.GetTarget();
			this.plungeAttack.OnDashFinishedEvent += this.OnPlungeAttackFinished;
			this.Quirce.Audio.PlayPlunge();
			this.plungeAttack.Dash(base.transform, Vector3.down, 10f, 0f, false);
			yield break;
		}

		private void OnPlungeAttackFinished()
		{
			this.plungeAttack.OnDashFinishedEvent -= this.OnPlungeAttackFinished;
			this.Quirce.AnimatorInyector.Plunge(false);
			QuirceBehaviour.QuirceAttackConfig attackConfig = this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.TELEPORT);
			this.currentPoint.nextValidPoints = new List<QuirceBossFightPoints.QUIRCE_FIGHT_SIDES>
			{
				QuirceBossFightPoints.QUIRCE_FIGHT_SIDES.LEFT,
				QuirceBossFightPoints.QUIRCE_FIGHT_SIDES.RIGHT
			};
			this.sword.SetVisible(true);
			this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack);
		}

		public void MultiTeleportAttack()
		{
			this.StartAttackAction();
			base.StartCoroutine(this.MultiTeleportAnticipation());
		}

		private IEnumerator MultiTeleportAnticipation()
		{
			QuirceBehaviour.QuirceAttackConfig qac = this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.MULTI_TELEPORT);
			this.Quirce.AnimatorInyector.TeleportOut();
			yield return new WaitForSeconds(qac.preparationSeconds);
			this.multiTeleportAttackNumber = qac.multiAttackTimes;
			if ((double)this.GetHealthPercentage() < 0.75)
			{
				this.multiTeleportAttackNumber++;
			}
			if ((double)this.GetHealthPercentage() < 0.25)
			{
				this.multiTeleportAttackNumber++;
			}
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingMultiTeleport()));
			yield break;
		}

		private IEnumerator PreparingMultiTeleport()
		{
			yield return new WaitForSeconds(0.1f);
			this.currentTarget = base.GetTarget();
			this.multiTeleportAttack.OnTeleportInEvent += this.OnMultiTeleportIn;
			this.Quirce.AnimatorInyector.TeleportOut();
			float waitTime = 1.2f;
			if (this.GetHealthPercentage() < 0.5f)
			{
				waitTime = 0.9f;
			}
			yield return new WaitForSeconds(waitTime);
			Transform teleportTarget = this.currentTarget;
			if (this.GetHealthPercentage() < 0.25f)
			{
				teleportTarget = this.Quirce.BossFightPoints.GetTeleportPlungeTransform();
			}
			this.multiTeleportAttack.Use(base.transform, teleportTarget, Vector3.up * 4f);
			yield break;
		}

		private void OnMultiTeleportIn()
		{
			Debug.Log("Quirce: MULTI Teleport IN");
			this.Quirce.AnimatorInyector.ResetTeleport();
			this.Quirce.AnimatorInyector.TeleportIn();
			this.multiTeleportAttack.OnTeleportInEvent -= this.OnMultiTeleportIn;
			base.StartCoroutine(this.MultiPlungeAnticipation());
		}

		private IEnumerator MultiPlungeAnticipation()
		{
			yield return new WaitForSeconds(0.3f);
			this.MultiPlungeAttack();
			yield break;
		}

		public void MultiPlungeAttack()
		{
			Debug.Log("Quirce: MULTI PLUNGE ATTACK");
			this.Quirce.AnimatorInyector.Plunge(true);
			this.currentTarget = base.GetTarget();
			this.multiPlungeAttack.OnDashFinishedEvent += this.OnMultiPlungeAttackFinished;
			this.Quirce.Audio.PlayPlunge();
			this.multiPlungeAttack.Dash(base.transform, Vector3.down, 10f, 0f, false);
		}

		private void OnMultiPlungeAttackFinished()
		{
			Debug.Log("Quirce: PLUNGE ATTACK FINISHED NUMBER:" + this.multiTeleportAttackNumber);
			this.multiPlungeAttack.OnDashFinishedEvent -= this.OnMultiPlungeAttackFinished;
			if (this.GetHealthPercentage() < 0.5f)
			{
				this.landingAreaSummon.SummonAreas(Vector3.right);
				this.landingAreaSummon.SummonAreas(Vector3.left);
			}
			this.Quirce.AnimatorInyector.Plunge(false);
			this.multiTeleportAttackNumber--;
			if (this.multiTeleportAttackNumber > 0)
			{
				this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingMultiTeleport()));
			}
			else
			{
				this.StartWaitingPeriod(this.GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS.MULTI_TELEPORT).waitingSecondsAfterAttack);
			}
		}

		public float GetHealthPercentage()
		{
			return this.Quirce.CurrentLife / this.Quirce.Stats.Life.Base;
		}

		public void OnTeleportOutAnimationStarts()
		{
			this.Quirce.ActivateColliders(false);
			this.Quirce.Audio.PlayTeleportOut();
			this.Quirce.Status.CastShadow = false;
		}

		public void OnTeleportOutAnimationFinished()
		{
		}

		public void OnTeleportInAnimationStarts()
		{
		}

		public void OnTeleportInAnimationFinished()
		{
			this.Quirce.Status.CastShadow = true;
			this.Quirce.ActivateColliders(true);
		}

		private void RotateToLookAt(Transform p, Vector3 point)
		{
			Vector3 vector = point - p.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (this.Quirce.Status.Orientation == EntityOrientation.Left)
			{
				num -= 180f;
			}
			p.rotation = Quaternion.Euler(0f, 0f, num);
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

		private QuirceBehaviour.QuirceAttackConfig GetAttackConfig(QuirceBehaviour.QUIRCE_ATTACKS atk)
		{
			return this.attacksConfiguration.Find((QuirceBehaviour.QuirceAttackConfig x) => x.attackType == atk);
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == QuirceBehaviour.BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public bool TargetCanBeVisible()
		{
			float num = this.Quirce.Target.transform.position.y - this.Quirce.transform.position.y;
			num = ((num <= 0f) ? (-num) : num);
			return num <= this.MaxVisibleHeight;
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.Quirce.transform.position.x)
			{
				if (this.Quirce.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.Quirce.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.Quirce.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.Quirce.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public void SetQuirceState(QuirceBehaviour.QUIRCE_STATE st)
		{
			this.currentQuirceState = st;
			this.currentlyAvailableAttacks = this.GetCurrentStateAttacks();
		}

		public QuirceBehaviour.QUIRCE_STATE GetQuirceState()
		{
			return this.currentQuirceState;
		}

		public override void Damage()
		{
			if (this.currentState != QuirceBehaviour.BOSS_STATES.MID_ACTION)
			{
				this.Quirce.AnimatorInyector.Hurt();
			}
		}

		public bool CanAttack()
		{
			return true;
		}

		public void Death()
		{
			base.StopAllCoroutines();
			this.ghostTrail.EnableGhostTrail = false;
			Debug.Log("DEATH REACHED");
			this.UnsubscribeFromEverything();
			base.BehaviourTree.StopBehaviour();
			this.Quirce.AnimatorInyector.Death();
			SplineFollowingProjectile currentProjectile = this.splineThrowAttack.GetCurrentProjectile();
			if (currentProjectile != null)
			{
				this.splineThrowAttack.SetProjectileWeaponDamage(currentProjectile, 0);
			}
		}

		public void ResetCoolDown()
		{
			if (this._currentAttackLapse > 0f)
			{
				this._currentAttackLapse = 0f;
			}
		}

		public void SetReversed(bool reversed)
		{
			this.Quirce.SpriteRenderer.flipY = reversed;
			this.sword.SetReversed(reversed);
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

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public Vector2 reversedLocalPositionOffset;

		[FoldoutGroup("Activation Settings", true, 0)]
		public float ActivationDistance;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MaxVisibleHeight = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float MinAttackDistance = 2f;

		[FoldoutGroup("Attack Settings", true, 0)]
		public float AttackCoolDown = 2f;

		private float _currentAttackLapse;

		public QuirceSwordBehaviour sword;

		private Transform currentTarget;

		private QuirceBehaviour.QuirceAttackConfig _lastAttackConfig;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack dashAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossTeleportAttack teleportAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossTeleportAttack multiTeleportAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack plungeAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack multiPlungeAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack multiDashAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossSplineFollowingProjectileAttack splineThrowAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossDashAttack quickMove;

		[FoldoutGroup("Attacks", true, 0)]
		public BossAreaSummonAttack areaSummonAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossAreaSummonAttack landingAreaSummon;

		[FoldoutGroup("Attacks", true, 0)]
		public BossInstantProjectileAttack instantProjectileAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossInstantProjectileAttack instantProjectileMoveAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public List<QuirceBehaviour.QuirceAttackConfig> attacksConfiguration;

		[FoldoutGroup("Attacks", true, 0)]
		public List<QuirceBehaviour.QuirceAttackConfig> ngPlusAttacksConfiguration;

		[FoldoutGroup("Traits", true, 0)]
		public EntityMotionChecker motionChecker;

		[FoldoutGroup("VFX", true, 0)]
		public GameObject vFXExplosion;

		private List<QuirceBehaviour.QUIRCE_ATTACKS> currentlyAvailableAttacks;

		private List<QuirceBehaviour.QUIRCE_ATTACKS> queuedActions;

		private QuirceBehaviour.QUIRCE_STATE currentQuirceState;

		[FoldoutGroup("Debug", true, 0)]
		public QuirceBehaviour.BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public QuirceBehaviour.QUIRCE_ATTACKS lastAttack;

		private Transform currentHang;

		private SplinePointInfo currentPoint;

		private GhostTrailGenerator ghostTrail;

		private int _originAreas;

		private float _originAreaSeconds;

		private int actionsCounter;

		private Coroutine currentCoroutine;

		[Serializable]
		public struct QuirceAttackConfig
		{
			public QuirceBehaviour.QUIRCE_ATTACKS attackType;

			public bool requiresReposition;

			public bool invertedReposition;

			public bool teleportReposition;

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

		public enum QUIRCE_STATE
		{
			SWORD,
			NO_SWORD
		}

		public enum QUIRCE_ATTACKS
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
