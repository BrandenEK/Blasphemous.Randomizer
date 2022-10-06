using System;
using System.Collections;
using System.Collections.Generic;
using BezierSplines;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Tools.Level.Actionables;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.TresAngustias
{
	public class TresAngustiasMasterBehaviour : EnemyBehaviour
	{
		public override void OnAwake()
		{
			base.OnAwake();
			PoolManager.Instance.CreatePool(this.explosionPrefab, 12);
			this._pathOrigin = this.currentPath.transform.localPosition;
			this.StartWaitingPeriod(3f);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.followPath)
			{
				this.FollowPathUpdate();
			}
		}

		private void LateUpdate()
		{
			this.currentPath.transform.localPosition = this._pathOrigin;
		}

		public MASTER_ANGUISH_STATES GetAnguishState()
		{
			return this.currentMasterAnguishState;
		}

		private void SetCurrentCoroutine(Coroutine c)
		{
			if (this._currentCoroutine != null)
			{
				Debug.Log(">>>>STOPPING CURRENT COROUTINE");
				base.StopCoroutine(this._currentCoroutine);
			}
			Debug.Log(">>NEW COROUTINE");
			this._currentCoroutine = c;
		}

		public void SetPath(BezierSpline s)
		{
			this.currentPath = s;
			Debug.Log("CURRENT PATH CHANGED");
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
			this._currentlyAvailableAttacks = this.GetCurrentStateAttacks();
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
			this.ChangeBossState(BOSS_STATES.AVAILABLE_FOR_ACTION);
			this._currentlyAvailableAttacks = this.GetCurrentStateAttacks();
		}

		public void LaunchIntro()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.IntroCoroutine()));
		}

		private IEnumerator IntroCoroutine()
		{
			this.singleAnguishLance.Behaviour.StartIntro();
			yield return new WaitForSeconds(1.5f);
			this.TresAngustias.bossfightPoints.ShowFlameWall();
			this.TresAngustias.Invencible = false;
			this.scrollManager.ActivateDeathCollider();
			this.singleAnguishMace.Behaviour.StartIntro();
			yield return new WaitForSeconds(1f);
			this.singleAnguishShield.Behaviour.StartIntro();
			base.StartCoroutine(this.WaitAllFreeAndCallback(new Action(this.OnIntroFinished)));
			yield break;
		}

		private void OnIntroFinished()
		{
			this.singleAnguishShield.Behaviour.BackToDance();
			this.singleAnguishLance.Behaviour.BackToDance();
			this.singleAnguishMace.Behaviour.BackToDance();
			this.scrollManager.scrollActive = true;
			this.scrollManager.ActivateDeathCollider();
			this.StartWaitingPeriod(2f);
		}

		private List<MASTER_ANGUISH_ATTACKS> GetCurrentStateAttacks()
		{
			float healthPercentage = this.GetHealthPercentage();
			List<MASTER_ANGUISH_ATTACKS> list;
			if (this.currentMasterAnguishState == MASTER_ANGUISH_STATES.DIVIDED)
			{
				list = new List<MASTER_ANGUISH_ATTACKS>
				{
					MASTER_ANGUISH_ATTACKS.SPEAR,
					MASTER_ANGUISH_ATTACKS.MACE,
					MASTER_ANGUISH_ATTACKS.MACE_AROUND,
					MASTER_ANGUISH_ATTACKS.SHIELD,
					MASTER_ANGUISH_ATTACKS.MERGE,
					MASTER_ANGUISH_ATTACKS.COMBO2
				};
				if (healthPercentage < 0.6f)
				{
					list.Add(MASTER_ANGUISH_ATTACKS.COMBO1);
				}
			}
			else if (this.currentMasterAnguishState == MASTER_ANGUISH_STATES.MERGED)
			{
				list = new List<MASTER_ANGUISH_ATTACKS>
				{
					MASTER_ANGUISH_ATTACKS.AREA,
					MASTER_ANGUISH_ATTACKS.DIVIDE
				};
			}
			else
			{
				list = new List<MASTER_ANGUISH_ATTACKS>
				{
					MASTER_ANGUISH_ATTACKS.AREA,
					MASTER_ANGUISH_ATTACKS.MULTIAREA
				};
			}
			return list;
		}

		public void LaunchRandomAction()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.LaunchAction(this.GetNewAttack())));
		}

		public MASTER_ANGUISH_ATTACKS GetNewAttack()
		{
			MASTER_ANGUISH_ATTACKS[] array = new MASTER_ANGUISH_ATTACKS[this._currentlyAvailableAttacks.Count];
			this._currentlyAvailableAttacks.CopyTo(array);
			List<MASTER_ANGUISH_ATTACKS> list = new List<MASTER_ANGUISH_ATTACKS>(array);
			list.Remove(this._lastAttack);
			if (this._lastAttack == MASTER_ANGUISH_ATTACKS.MERGE)
			{
				list.Remove(MASTER_ANGUISH_ATTACKS.DIVIDE);
			}
			else if (this._lastAttack == MASTER_ANGUISH_ATTACKS.DIVIDE)
			{
				list.Remove(MASTER_ANGUISH_ATTACKS.MERGE);
			}
			if (Random.Range(0f, 1f) > 0.6f)
			{
				list.Remove(MASTER_ANGUISH_ATTACKS.MERGE);
			}
			return list[Random.Range(0, list.Count)];
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		private MasterAnguishAttackConfig GetAttackConfig(MASTER_ANGUISH_ATTACKS atk)
		{
			return this.attacksConfig.Find((MasterAnguishAttackConfig x) => x.atk == atk);
		}

		public void LaunchActionFromBehaviourTree(MASTER_ANGUISH_ATTACKS atk)
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.LaunchAction(atk)));
		}

		private IEnumerator LaunchAction(MASTER_ANGUISH_ATTACKS atk)
		{
			this.StartAttackAction();
			this._lastAttack = atk;
			MasterAnguishAttackConfig currentConfig = this.GetAttackConfig(atk);
			yield return new WaitForSeconds(currentConfig.preparationSeconds);
			switch (atk)
			{
			case MASTER_ANGUISH_ATTACKS.SPEAR:
			{
				Debug.Log("LAUNCHING SPEAR ACTION");
				int nSpears = this.GetSpearNumber();
				this.singleAnguishLance.Behaviour.StopDancing();
				this.singleAnguishLance.Behaviour.OnActionFinished += this.OnSingleAttackFinished;
				this.singleAnguishLance.Behaviour.IssueSpearAttack(nSpears, 1f);
				break;
			}
			case MASTER_ANGUISH_ATTACKS.MACE:
				this.singleAnguishMace.Behaviour.StopDancing();
				this.singleAnguishMace.Behaviour.OnActionFinished += this.OnSingleAttackFinished;
				this.singleAnguishMace.Behaviour.IssueMaceAttack();
				break;
			case MASTER_ANGUISH_ATTACKS.SHIELD:
			{
				int nSpears = this.GetSpearNumber();
				this.singleAnguishShield.Behaviour.StopDancing();
				this.singleAnguishShield.Behaviour.OnActionFinished += this.OnSingleAttackFinished;
				this.singleAnguishShield.Behaviour.IssueSpearAttack(nSpears, 1f);
				break;
			}
			case MASTER_ANGUISH_ATTACKS.MERGE:
			{
				this.followPath = false;
				this._currentBeamTransform = this.TresAngustias.bossfightPoints.GetRandomBeamPoint();
				base.transform.position = this._currentBeamTransform.position;
				Vector2 p = this.GetMergePoint(0);
				Vector2 p2 = this.GetMergePoint(1);
				Vector2 p3 = this.GetMergePoint(2);
				this._mergeCounter = 3;
				this.singleAnguishLance.Behaviour.OnActionFinished += this.OnMergeRepositionFinished;
				this.singleAnguishLance.Behaviour.IssueMerge(p);
				this.singleAnguishMace.Behaviour.OnActionFinished += this.OnMergeRepositionFinished;
				this.singleAnguishMace.Behaviour.IssueMerge(p2);
				this.singleAnguishShield.Behaviour.OnActionFinished += this.OnMergeRepositionFinished;
				this.singleAnguishShield.Behaviour.IssueMerge(p3);
				break;
			}
			case MASTER_ANGUISH_ATTACKS.DIVIDE:
				this.TresAngustias.AnimatorInyector.Divide();
				this.TresAngustias.Audio.PlayDivide();
				yield return new WaitForSeconds(1f);
				this.DivideIntoThree(delegate
				{
					this.StartWaitingPeriod(currentConfig.preparationSeconds);
				});
				this.singleAnguishLance.Behaviour.BackToDance();
				this.singleAnguishMace.Behaviour.BackToDance();
				this.singleAnguishShield.Behaviour.BackToDance();
				this.followPath = true;
				break;
			case MASTER_ANGUISH_ATTACKS.AREA:
			{
				GameObject area = this.bossAreaAttack.SummonAreaOnPoint(this._currentBeamTransform.position - Vector3.up * this.beamOffsetY, 0f, 1f, null);
				area.transform.parent = this.TresAngustias.bossfightPoints.transform;
				this.StartWaitingPeriod(currentConfig.recoverySeconds);
				break;
			}
			case MASTER_ANGUISH_ATTACKS.MULTIAREA:
			{
				Transform newPoint = this.TresAngustias.bossfightPoints.GetDifferentBeamTransform(this._currentBeamTransform);
				Vector2 pToTeleport = newPoint.position;
				GameObject firstArea = this.bossAreaAttack.SummonAreaOnPoint(this._currentBeamTransform.position - Vector3.up * this.beamOffsetY, 0f, 1f, null);
				GameObject secondArea = this.bossAreaAttack.SummonAreaOnPoint(pToTeleport, 0f, 1f, null);
				secondArea.transform.position = new Vector3(secondArea.transform.position.x, firstArea.transform.position.y, 0f);
				firstArea.transform.parent = this.TresAngustias.bossfightPoints.transform;
				secondArea.transform.parent = this.TresAngustias.bossfightPoints.transform;
				yield return new WaitForSeconds(2f);
				base.transform.position = pToTeleport;
				this._currentBeamTransform = newPoint;
				this.StartWaitingPeriod(currentConfig.recoverySeconds);
				break;
			}
			case MASTER_ANGUISH_ATTACKS.COMBO1:
				this._repositionCounter = 3;
				this.singleAnguishLance.Behaviour.OnActionFinished += this.OnComboReposition;
				this.singleAnguishLance.Behaviour.IssueCombo(this.TresAngustias.bossfightPoints.beamPoints[1].position + Vector3.up * 4f);
				yield return new WaitForSeconds(0.4f);
				this.singleAnguishMace.Behaviour.OnActionFinished += this.OnComboReposition;
				this.singleAnguishMace.Behaviour.IssueCombo(this.TresAngustias.bossfightPoints.beamPoints[0].position + Vector3.up * 4f);
				yield return new WaitForSeconds(0.4f);
				this.singleAnguishShield.Behaviour.OnActionFinished += this.OnComboReposition;
				this.singleAnguishShield.Behaviour.IssueCombo(this.TresAngustias.bossfightPoints.beamPoints[2].position + Vector3.up * 4f);
				break;
			case MASTER_ANGUISH_ATTACKS.HORIZONTALAREA:
			{
				Vector2 p = this.TresAngustias.bossfightPoints.beamPoints[0].position;
				Vector2 p2 = this.TresAngustias.bossfightPoints.beamPoints[1].position;
				Vector2 p3 = this.TresAngustias.bossfightPoints.beamPoints[2].position;
				this.bossAreaAttack.SummonAreaOnPoint(p2 - Vector2.up, 0f, 1f, null);
				yield return new WaitForSeconds(2.5f);
				this.bossAreaAttack.SummonAreaOnPoint(p - Vector2.up - Vector2.right * 2f, -90f, 1f, null);
				yield return new WaitForSeconds(2.5f);
				this.bossAreaAttack.SummonAreaOnPoint(p3 - Vector2.up + Vector2.right * 2f, 90f, 1f, null);
				break;
			}
			case MASTER_ANGUISH_ATTACKS.COMBO2:
			{
				this._repositionCounter = 3;
				List<Transform> comboPoints = this.TresAngustias.bossfightPoints.GetSpearPoints();
				this.singleAnguishLance.Behaviour.OnActionFinished += this.OnSpearComboReposition;
				this.singleAnguishLance.Behaviour.IssueCombo(comboPoints[0].position);
				yield return new WaitForSeconds(0.5f);
				this.singleAnguishMace.Behaviour.OnActionFinished += this.OnSpearComboReposition;
				this.singleAnguishMace.Behaviour.IssueCombo(comboPoints[1].position);
				yield return new WaitForSeconds(0.5f);
				this.singleAnguishShield.Behaviour.OnActionFinished += this.OnSpearComboReposition;
				this.singleAnguishShield.Behaviour.IssueCombo(comboPoints[2].position);
				yield return new WaitForSeconds(0.5f);
				break;
			}
			case MASTER_ANGUISH_ATTACKS.MACE_AROUND:
				this.singleAnguishMace.Behaviour.StopDancing();
				this.singleAnguishMace.Behaviour.OnActionFinished += this.OnSingleAttackFinished;
				this.singleAnguishMace.Behaviour.IssueMaceSurroundAttack();
				break;
			}
			yield break;
		}

		private void OnSingleAttackFinished(SingleAnguishBehaviour obj)
		{
			Debug.Log("SINGLE ATTACK FINISHED: " + this._lastAttack.ToString());
			obj.OnActionFinished -= this.OnSingleAttackFinished;
			obj.BackToDance();
			this.StartWaitingPeriod(this.GetWaitingPeriodFromHP());
		}

		private void OnComboReposition(SingleAnguishBehaviour obj)
		{
			obj.OnActionFinished -= this.OnComboReposition;
			this._repositionCounter--;
			if (this._repositionCounter == 0)
			{
				this.OnAllSingleAnguishInComboPosition();
			}
		}

		private void OnSpearComboReposition(SingleAnguishBehaviour obj)
		{
			obj.OnActionFinished -= this.OnSpearComboReposition;
			this._repositionCounter--;
			if (this._repositionCounter == 0)
			{
				this.OnAllSingleAnguishInCombo2Position();
			}
		}

		private void OnAllSingleAnguishInCombo2Position()
		{
			this.singleAnguishShield.Behaviour.ChangeToAction();
			this.singleAnguishLance.Behaviour.ChangeToAction();
			this.singleAnguishMace.Behaviour.ChangeToAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.Combo2Coroutine()));
		}

		private void OnAllSingleAnguishInComboPosition()
		{
			this.singleAnguishShield.Behaviour.ChangeToAction();
			this.singleAnguishLance.Behaviour.ChangeToAction();
			this.singleAnguishMace.Behaviour.ChangeToAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.Combo1Coroutine()));
		}

		private IEnumerator Combo1Coroutine()
		{
			this.singleAnguishShield.Behaviour.IssueSpearAttack(2, 1f);
			yield return new WaitForSeconds(1.5f);
			this.singleAnguishLance.Behaviour.IssueSpearAttack(2, 1f);
			yield return new WaitForSeconds(1.5f);
			this.singleAnguishMace.Behaviour.IssueMaceAttack();
			base.StartCoroutine(this.WaitAllFreeAndCallback(new Action(this.OnAllComboAttacks)));
			yield break;
		}

		private IEnumerator Combo2Coroutine()
		{
			this.singleAnguishMace.Behaviour.IssueMaceSurroundAttack();
			yield return new WaitForSeconds(0.5f);
			this.singleAnguishShield.Behaviour.IssueSpearAttack(2, 1.25f);
			yield return new WaitForSeconds(1.5f);
			this.singleAnguishLance.Behaviour.IssueSpearAttack(2, 1.25f);
			base.StartCoroutine(this.WaitAllFreeAndCallback(new Action(this.OnAllComboAttacks)));
			yield break;
		}

		private IEnumerator WaitAllFreeAndCallback(Action callback)
		{
			while (this.IsEverySingleAnguishBusy())
			{
				yield return null;
			}
			callback();
			yield break;
		}

		private bool IsEverySingleAnguishBusy()
		{
			return this.singleAnguishLance.Behaviour.currentState != BOSS_STATES.AVAILABLE_FOR_ACTION || this.singleAnguishMace.Behaviour.currentState != BOSS_STATES.AVAILABLE_FOR_ACTION || this.singleAnguishShield.Behaviour.currentState != BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		private void OnAllComboAttacks()
		{
			this.singleAnguishShield.Behaviour.BackToDance();
			this.singleAnguishLance.Behaviour.BackToDance();
			this.singleAnguishMace.Behaviour.BackToDance();
			this.StartWaitingPeriod(3f);
		}

		private void DivideIntoThree(Action callback = null)
		{
			this.RevealBody(false);
			Vector2 mergePoint = this.GetMergePoint(0);
			Vector2 mergePoint2 = this.GetMergePoint(1);
			Vector2 mergePoint3 = this.GetMergePoint(2);
			this.singleAnguishLance.transform.position = mergePoint;
			this.singleAnguishMace.transform.position = mergePoint2;
			this.singleAnguishShield.transform.position = mergePoint3;
			this.currentMasterAnguishState = MASTER_ANGUISH_STATES.DIVIDED;
			if (callback != null)
			{
				callback();
			}
		}

		public float GetHealthPercentage()
		{
			return this.TresAngustias.CurrentLife / this.TresAngustias.Stats.Life.Base;
		}

		private int GetSpearNumber()
		{
			float healthPercentage = this.GetHealthPercentage();
			if (healthPercentage > 0.75f)
			{
				return 1;
			}
			if (healthPercentage > 0.5f)
			{
				return 2;
			}
			if (healthPercentage > 0.3f)
			{
				return 3;
			}
			return 4;
		}

		private float GetWaitingPeriodFromHP()
		{
			float healthPercentage = this.GetHealthPercentage();
			float num = 0.5f;
			float num2 = 3f;
			return Mathf.Lerp(num, num2, healthPercentage);
		}

		private void OnMergeRepositionFinished(SingleAnguishBehaviour obj)
		{
			obj.OnActionFinished -= this.OnMergeRepositionFinished;
			this._mergeCounter--;
			if (this._mergeCounter == 0)
			{
				this.OnAllSingleAnguishInMergePosition();
			}
		}

		private void OnAllSingleAnguishInMergePosition()
		{
			if (this.TresAngustias.Status.Dead)
			{
				return;
			}
			this.singleAnguishShield.Behaviour.ChangeToMerged();
			this.singleAnguishLance.Behaviour.ChangeToMerged();
			this.singleAnguishMace.Behaviour.ChangeToMerged();
			this.TresAngustias.Audio.PlayMerge();
			this.TresAngustias.AnimatorInyector.Merge();
			this.RevealBody(true);
			this.TriggerTraps();
			base.StartCoroutine(this.AfterMerge());
		}

		private void RevealBody(bool reveal)
		{
			this.TresAngustias.SpriteRenderer.enabled = reveal;
			this.TresAngustias.DamageArea.DamageAreaCollider.enabled = reveal;
		}

		private void TriggerTraps()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 2.2f, 0.3f, 1.8f);
			TriggerBasedTrap[] array = Object.FindObjectsOfType<TriggerBasedTrap>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Use();
			}
		}

		private IEnumerator AfterMerge()
		{
			MasterAnguishAttackConfig currentConfig = this.GetAttackConfig(MASTER_ANGUISH_ATTACKS.MERGE);
			yield return new WaitForSeconds(currentConfig.recoverySeconds);
			if (this.IsHPUnderFinalThreshold())
			{
				this.currentMasterAnguishState = MASTER_ANGUISH_STATES.FINAL;
			}
			else
			{
				this.currentMasterAnguishState = MASTER_ANGUISH_STATES.MERGED;
			}
			this.ActionFinished();
			yield break;
		}

		private Vector2 GetMergePoint(int index)
		{
			return this.mergePointMarkers[index].transform.position;
		}

		private void FollowPathUpdate()
		{
			float num = this.currentCurve.Evaluate(this._updateCounter / this.secondsToFullLoop);
			Vector3 point = this.currentPath.GetPoint(num);
			base.transform.position = point;
			this._updateCounter += Time.deltaTime;
			this._updateCounter %= this.secondsToFullLoop;
		}

		public bool IsHPUnderFinalThreshold()
		{
			return this.TresAngustias.CurrentLife < this.TresAngustias.Stats.Life.MaxValue / 4f;
		}

		private void ClearAll()
		{
			this.bossAreaAttack.ClearAll();
		}

		public void Death()
		{
			this.ClearAll();
			this.TresAngustias.Audio.PlayPreDeath();
			this.scrollManager.scrollActive = false;
			base.StopAllCoroutines();
			this.StartAttackAction();
			this.followPath = false;
			if (this.currentMasterAnguishState == MASTER_ANGUISH_STATES.DIVIDED || this.currentMasterAnguishState == MASTER_ANGUISH_STATES.MERGED)
			{
				this.StartDividedDeathSequence();
			}
			else
			{
				Vector3 position = this.TresAngustias.bossfightPoints.beamPoints[1].position;
				TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, position, 3f, false), 10);
				Debug.Log("DEAD; STARTING SEQUENCE");
				this.StartDeathSequence();
			}
		}

		private void StartDividedDeathSequence()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 2.2f, 0.3f, 1.8f);
			this.TresAngustias.AnimatorInyector.Disappear();
			this.SetCurrentCoroutine(base.StartCoroutine(this.DeathSequenceDivided()));
			this.TresAngustias.bossfightPoints.HideFlameWall();
			Core.Logic.Penitent.Status.Invulnerable = true;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(5f, Vector2.up * 2f, 140, 0.25f, 0f, default(Vector3), 0.06f, true);
			Core.Input.SetBlocker("CINEMATIC", true);
		}

		private IEnumerator DeathSequenceDivided()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 2.2f, 0.3f, 1.8f);
			yield return new WaitForSeconds(0.1f);
			this.singleAnguishLance.Behaviour.ForceWait(0.2f);
			this.singleAnguishMace.Behaviour.ForceWait(0.2f);
			this.singleAnguishShield.Behaviour.ForceWait(0.2f);
			base.StartCoroutine(this.WaitAllFreeAndCallback(new Action(this.OnAllOnDeathPositionDivided)));
			yield break;
		}

		private void OnAllOnDeathPositionDivided()
		{
			base.StartCoroutine(this.DelayedDeaths(0.5f));
		}

		private void StartDeathSequence()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 2.2f, 0.3f, 1.8f);
			base.StartCoroutine(this.RandomExplosions(3f, 24, base.transform, 1.5f, this.explosionPrefab, new Action(this.TresAngustias.DamageFlash), new Action(this.AfterExplosions)));
			this.TresAngustias.bossfightPoints.HideFlameWall();
			Core.Logic.Penitent.Status.Invulnerable = true;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(5f, Vector2.up * 2f, 140, 0.25f, 0f, default(Vector3), 0.06f, true);
			Core.Input.SetBlocker("CINEMATIC", true);
		}

		private void AfterExplosions()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.DeathSequence()));
		}

		private IEnumerator DeathSequence()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 2.2f, 0.3f, 1.8f);
			yield return new WaitForSeconds(1f);
			this.TresAngustias.AnimatorInyector.Divide();
			this.TresAngustias.Audio.PlayDivide();
			yield return new WaitForSeconds(0.9f);
			this.DivideIntoThree(null);
			this.singleAnguishLance.Behaviour.ForceWait(0.2f);
			this.singleAnguishMace.Behaviour.ForceWait(0.2f);
			this.singleAnguishShield.Behaviour.ForceWait(0.2f);
			base.StartCoroutine(this.WaitAllFreeAndCallback(new Action(this.OnAllOnDeathPosition)));
			yield break;
		}

		private void OnAllOnDeathPosition()
		{
			this.singleAnguishLance.Behaviour.BackToAction();
			this.singleAnguishMace.Behaviour.BackToAction();
			this.singleAnguishShield.Behaviour.BackToAction();
			base.StartCoroutine(this.DelayedDeaths(0f));
		}

		private IEnumerator DelayedDeaths(float initDelay = 0f)
		{
			yield return new WaitForSeconds(initDelay);
			this.TresAngustias.Audio.PlayDeath();
			yield return new WaitForSeconds(0.5f);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(this.singleAnguishLance.transform.position, 0.3f, 0.3f, 1f);
			this.singleAnguishLance.Kill();
			this.singleAnguishLance.Behaviour.ActivateWeapon(false);
			yield return new WaitForSeconds(0.5f);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(this.singleAnguishMace.transform.position, 0.3f, 0.3f, 1f);
			this.singleAnguishMace.Kill();
			this.singleAnguishMace.Behaviour.ActivateWeapon(false);
			yield return new WaitForSeconds(0.5f);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(this.singleAnguishShield.transform.position, 0.3f, 0.3f, 1f);
			this.singleAnguishShield.Kill();
			this.singleAnguishShield.Behaviour.ActivateWeapon(false);
			this.OnDeathSequenceEnds();
			yield break;
		}

		private void OnDeathSequenceEnds()
		{
			Core.Logic.Penitent.Status.Invulnerable = false;
			Core.Input.SetBlocker("CINEMATIC", false);
		}

		private IEnumerator RandomExplosions(float seconds, int totalExplosions, Transform center, float radius, GameObject poolableExplosion, Action OnExplosion = null, Action callback = null)
		{
			float counter = 0f;
			int expCounter = 0;
			while (counter < seconds)
			{
				counter += Time.deltaTime;
				if (counter > ((float)expCounter + 1f) / seconds)
				{
					expCounter++;
					Vector2 vector = center.position + new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius));
					PoolManager.Instance.ReuseObject(poolableExplosion, vector, Quaternion.identity, false, 1);
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

		public BezierSpline currentPath;

		public AnimationCurve currentCurve;

		public TresAngustiasMaster TresAngustias;

		public float secondsToFullLoop;

		private Vector3 _pathOrigin;

		private float _updateCounter;

		private bool followPath = true;

		public SingleAnguish singleAnguishLance;

		public SingleAnguish singleAnguishShield;

		public SingleAnguish singleAnguishMace;

		public ScrollableModulesManager scrollManager;

		public GameObject explosionPrefab;

		public BossAreaSummonAttack bossAreaAttack;

		public Coroutine _currentCoroutine;

		public List<Transform> mergePointMarkers;

		public BOSS_STATES currentState = BOSS_STATES.AVAILABLE_FOR_ACTION;

		public MASTER_ANGUISH_STATES currentMasterAnguishState;

		public List<MasterAnguishAttackConfig> attacksConfig;

		private int _mergeCounter;

		private int _repositionCounter;

		private List<MASTER_ANGUISH_ATTACKS> _currentlyAvailableAttacks;

		private MASTER_ANGUISH_ATTACKS _lastAttack;

		private Transform _currentBeamTransform;

		private float beamOffsetY = 4.5f;
	}
}
