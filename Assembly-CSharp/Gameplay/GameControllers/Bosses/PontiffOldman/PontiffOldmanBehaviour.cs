using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Generic.Attacks;
using Gameplay.GameControllers.Bosses.PontiffOldman.AI;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Environment.AreaEffects;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffOldman
{
	public class PontiffOldmanBehaviour : EnemyBehaviour
	{
		public PontiffOldman PontiffOldman { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PontiffOldmanBehaviour> OnActionFinished;

		public override void OnAwake()
		{
			base.OnAwake();
			this.stIntro = new PontiffOldman_StIntro();
			this.stAction = new PontiffOldman_StAction();
			this.stDeath = new PontiffOldman_StDeath();
			this.stCast = new PontiffOldman_StCasting();
			this._fsm = new StateMachine<PontiffOldmanBehaviour>(this, this.stIntro, null, null);
			this.results = new RaycastHit2D[1];
			this.currentlyAvailableAttacks = new List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS>
			{
				PontiffOldmanBehaviour.PontiffOldman_ATTACKS.CAST_FIRE,
				PontiffOldmanBehaviour.PontiffOldman_ATTACKS.CAST_MAGIC,
				PontiffOldmanBehaviour.PontiffOldman_ATTACKS.CAST_TOXIC
			};
		}

		public override void OnStart()
		{
			base.OnStart();
			this.PontiffOldman = (PontiffOldman)this.Entity;
			this.ChangeBossState(BOSS_STATES.WAITING);
			PoolManager.Instance.CreatePool(this.fireSignPrefab, 1);
			PoolManager.Instance.CreatePool(this.toxicSignPrefab, 1);
			PoolManager.Instance.CreatePool(this.lightningSignPrefab, 1);
			PoolManager.Instance.CreatePool(this.magicSignPrefab, 1);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
		}

		private void CheckAshChargerSpawn()
		{
			this._ashChargerSpawnCounter += Time.deltaTime;
			if (this._ashChargerSpawnCounter >= this.ashChargerSpawnLapse)
			{
				this.SpawnNewCharger();
				this._ashChargerSpawnCounter = 0f;
			}
		}

		private void SpawnNewCharger()
		{
			Vector2 vector = this.bossfightPoints.GetPointAwayOfPenitent(Core.Logic.Penitent.transform.position).position;
			this.ashChargerSpawn.Spawn(vector, this.GetDirToPenitent(vector), 0f, null);
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
			this._comboActionsRemaining--;
			if (this._comboActionsRemaining == 0)
			{
				this.QueuedActionsPush(PontiffOldmanBehaviour.PontiffOldman_ATTACKS.COMBO_REST);
			}
		}

		private void CancelCombo()
		{
			this.PontiffOldman.AnimatorInyector.CancelAll();
			this.queuedActions.Clear();
			this._comboActionsRemaining = -1;
		}

		private void ActionFinished()
		{
			this.ChangeBossState(BOSS_STATES.AVAILABLE_FOR_ACTION);
			if (this.OnActionFinished != null)
			{
				this.OnActionFinished(this);
			}
		}

		public void LaunchAction(PontiffOldmanBehaviour.PontiffOldman_ATTACKS atk)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"TIME: ",
				Time.time,
				" Launching action: ",
				atk.ToString()
			}));
			switch (atk)
			{
			case PontiffOldmanBehaviour.PontiffOldman_ATTACKS.COMBO_REST:
				this.PontiffOldman.AnimatorInyector.ComboMode(false);
				this.StartWaitingPeriod(1.5f);
				break;
			case PontiffOldmanBehaviour.PontiffOldman_ATTACKS.REPOSITION:
				this.IssueReposition();
				break;
			case PontiffOldmanBehaviour.PontiffOldman_ATTACKS.CAST_FIRE:
				this.IssueCastFire();
				break;
			case PontiffOldmanBehaviour.PontiffOldman_ATTACKS.CAST_TOXIC:
				this.IssueCastToxic();
				break;
			case PontiffOldmanBehaviour.PontiffOldman_ATTACKS.CAST_LIGHTNING:
				this.IssueCastLightning();
				break;
			case PontiffOldmanBehaviour.PontiffOldman_ATTACKS.CAST_MAGIC:
				this.IssueCastMagic();
				break;
			case PontiffOldmanBehaviour.PontiffOldman_ATTACKS.REPOSITION_MID:
				this.IssueRepositionMid();
				break;
			}
			this.lastAttack = atk;
		}

		private bool LastAttackWasReposition()
		{
			return this.lastAttack == PontiffOldmanBehaviour.PontiffOldman_ATTACKS.REPOSITION || this.lastAttack == PontiffOldmanBehaviour.PontiffOldman_ATTACKS.REPOSITION_MID;
		}

		public PontiffOldmanBehaviour.PontiffOldman_ATTACKS GetNewAttack()
		{
			if (this.queuedActions != null && this.queuedActions.Count > 0)
			{
				return this.QueuedActionsPop();
			}
			PontiffOldmanBehaviour.PontiffOldman_ATTACKS[] array = new PontiffOldmanBehaviour.PontiffOldman_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS> list = new List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS>(array);
			if (this.LastAttackWasReposition())
			{
				UnityEngine.Debug.Log("<color=green> LAST ATTACK WAS REPOSITION, REMOVING THEM");
				list.Remove(PontiffOldmanBehaviour.PontiffOldman_ATTACKS.REPOSITION);
				list.Remove(PontiffOldmanBehaviour.PontiffOldman_ATTACKS.REPOSITION_MID);
			}
			else
			{
				list.Remove(this.lastAttack);
			}
			if (!this.IsInLightningPoint())
			{
				list.Remove(PontiffOldmanBehaviour.PontiffOldman_ATTACKS.CAST_LIGHTNING);
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		private bool IsInLightningPoint()
		{
			return this._lastRepositionPoint == this.bossfightPoints.repositionPoints[0];
		}

		public IEnumerator WaitForState(State<PontiffOldmanBehaviour> st)
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

		private void QueuedActionsPush(PontiffOldmanBehaviour.PontiffOldman_ATTACKS atk)
		{
			if (this.queuedActions == null)
			{
				this.queuedActions = new List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS>();
			}
			this.queuedActions.Add(atk);
		}

		private PontiffOldmanBehaviour.PontiffOldman_ATTACKS QueuedActionsPop()
		{
			PontiffOldmanBehaviour.PontiffOldman_ATTACKS pontiffOldman_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(pontiffOldman_ATTACKS);
			return pontiffOldman_ATTACKS;
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public float GetHealthPercentage()
		{
			return this.PontiffOldman.CurrentLife / this.PontiffOldman.Stats.Life.Base;
		}

		private void SetPhase(PontiffOldmanBehaviour.PontiffOldmanPhases p)
		{
			this.currentlyAvailableAttacks = p.availableAttacks;
			this._currentPhase = p.phaseId;
		}

		private void ChangePhase(PontiffOldmanBehaviour.PontiffOldman_PHASES p)
		{
			PontiffOldmanBehaviour.PontiffOldmanPhases phase = this.phases.Find((PontiffOldmanBehaviour.PontiffOldmanPhases x) => x.phaseId == p);
			this.SetPhase(phase);
		}

		private void CheckNextPhase()
		{
		}

		public void IssueCombo(List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS> testCombo)
		{
			for (int i = 0; i < testCombo.Count; i++)
			{
				this.QueuedActionsPush(testCombo[i]);
			}
			this._comboActionsRemaining = testCombo.Count;
			this.StartWaitingPeriod(0.1f);
			this.PontiffOldman.AnimatorInyector.ComboMode(true);
		}

		private IEnumerator GetIntoStateAndCallback(State<PontiffOldmanBehaviour> newSt, float waitSeconds, Action callback)
		{
			this._fsm.ChangeState(newSt);
			yield return new WaitForSeconds(2f);
			callback();
			yield break;
		}

		private void StartWaitingPeriod(float seconds)
		{
			UnityEngine.Debug.Log(">> WAITING PERIOD: " + seconds);
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
			UnityEngine.Debug.Log(">> READY FOR ACTION: " + Time.time);
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
			this.ChangePhase(PontiffOldmanBehaviour.PontiffOldman_PHASES.FIRST);
			this.LookAtPenitent();
			yield return new WaitForSeconds(1.5f);
			base.BehaviourTree.StartBehaviour();
			this.ActivateCollisions(true);
			this.StartWaitingPeriod(0.1f);
			this._ashChargerActive = false;
			yield break;
		}

		private void ActivateCollisions(bool activate)
		{
			this.PontiffOldman.DamageArea.DamageAreaCollider.enabled = activate;
		}

		private void Shake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 1f, 12, 0.2f, 0f, default(Vector3), 0f, false);
		}

		private void IssueCastFire()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.CastFireCoroutine()));
		}

		private IEnumerator CastFireCoroutine()
		{
			this.LookAtPenitent();
			this.PontiffOldman.AnimatorInyector.Cast(true);
			this.CreateSpellFX(PONTIFF_SPELLS.FIRE, this.castSignOffsetDistance);
			yield return new WaitForSeconds(1f);
			float d = this.GetDirFromOrientation();
			this.fireMachineGun.transform.SetParent(null);
			this.fireMachineGun.transform.position = base.transform.position + Vector3.up * 8f;
			this.fireMachineGun.StartAttack(Core.Logic.Penitent.transform);
			yield return new WaitForSeconds(3f);
			this.PontiffOldman.AnimatorInyector.Cast(false);
			this.OnCastFireEnds();
			yield break;
		}

		private void OnCastFireEnds()
		{
			this.StartWaitingPeriod(this.attackLapses.Find((PontiffOldmanBehaviour.PontiffOldmanAttackConfig x) => x.attackType == this.lastAttack).waitingSecondsAfterAttack);
		}

		private void IssueCastMagic()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.CastMagicCoroutine()));
		}

		private IEnumerator CastMagicCoroutine()
		{
			this.LookAtPenitent();
			this.PontiffOldman.AnimatorInyector.Cast(true);
			this.CreateSpellFX(PONTIFF_SPELLS.MAGIC, 1f);
			yield return new WaitForSeconds(1f);
			int i = 4;
			Vector2 dir = this.GetDirFromOrientation() * Vector2.right;
			float offset = 0.7f;
			int castCounter = 0;
			for (int j = 0; j < i; j++)
			{
				castCounter++;
				this.magicProjectileLauncher.Shoot(dir, Vector2.up * offset, 1f);
				if (castCounter > 3)
				{
					this.PontiffOldman.AnimatorInyector.Cast(false);
				}
				yield return new WaitForSeconds(0.6f);
				castCounter++;
				this.magicProjectileLauncher.Shoot(dir, Vector2.down * offset, 1f);
				yield return new WaitForSeconds(0.6f);
			}
			this.OnCastMagicEnds();
			yield break;
		}

		private void OnCastMagicEnds()
		{
			this.StartWaitingPeriod(this.attackLapses.Find((PontiffOldmanBehaviour.PontiffOldmanAttackConfig x) => x.attackType == this.lastAttack).waitingSecondsAfterAttack);
		}

		private void IssueCastLightning()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.CastLightningCoroutine()));
		}

		private IEnumerator CastLightningCoroutine()
		{
			this.LookAtPenitent();
			this.PontiffOldman.AnimatorInyector.Cast(true);
			this.CreateSpellFX(PONTIFF_SPELLS.LIGHTNING, this.castSignOffsetDistance);
			yield return new WaitForSeconds(1f);
			this.windArea.SetMaxForce();
			this.PontiffOldman.Audio.PlayWind_AUDIO();
			int i = 5;
			for (int j = 0; j < i; j++)
			{
				if (j % 2 == 0)
				{
					this.lightningAttack.SummonAreaOnPoint(Core.Logic.Penitent.transform.position, 0f, 1f, null);
				}
				else
				{
					this.lightningAttack.SummonAreaOnPoint(Core.Logic.Penitent.transform.position + Vector3.right * 2f, 0f, 1f, null);
					this.lightningAttack.SummonAreaOnPoint(Core.Logic.Penitent.transform.position + Vector3.left * 2f, 0f, 1f, null);
				}
				yield return new WaitForSeconds(UnityEngine.Random.Range(1.25f, 2f));
				if ((float)j > (float)i / 2f)
				{
					this.PontiffOldman.AnimatorInyector.Cast(false);
				}
			}
			this.PontiffOldman.AnimatorInyector.Cast(false);
			this.windArea.SetMinForce();
			this.PontiffOldman.Audio.StopWind_AUDIO();
			this.OnCastLightningEnds();
			yield break;
		}

		private void CreateSpellFX(PONTIFF_SPELLS spellType, float signOffset)
		{
			float dirFromOrientation = this.GetDirFromOrientation();
			this.castLoopFx.material = this.matsBySpell.Find((MaterialsBySpellType x) => x.spellType == spellType).mat;
			this.castLoopFx.GetComponentInChildren<SpriteRenderer>().flipX = (dirFromOrientation == -1f);
			base.StartCoroutine(this.CreateSignDelayed(spellType, signOffset, 1.2f));
		}

		private IEnumerator CreateSignDelayed(PONTIFF_SPELLS spellType, float signOffset, float delay)
		{
			yield return new WaitForSeconds(delay);
			GameObject signPrefab = null;
			switch (spellType)
			{
			case PONTIFF_SPELLS.FIRE:
				signPrefab = this.fireSignPrefab;
				break;
			case PONTIFF_SPELLS.LIGHTNING:
				signPrefab = this.lightningSignPrefab;
				break;
			case PONTIFF_SPELLS.TOXIC:
				signPrefab = this.toxicSignPrefab;
				break;
			case PONTIFF_SPELLS.MAGIC:
				signPrefab = this.magicSignPrefab;
				break;
			}
			float d = this.GetDirFromOrientation();
			GameObject go = PoolManager.Instance.ReuseObject(signPrefab, base.transform.position + d * signOffset * Vector3.right + Vector3.up * 2f, Quaternion.identity, false, 1).GameObject;
			go.GetComponentInChildren<SpriteRenderer>().flipX = (d == -1f);
			yield break;
		}

		private void OnCastLightningEnds()
		{
			this.StartWaitingPeriod(this.attackLapses.Find((PontiffOldmanBehaviour.PontiffOldmanAttackConfig x) => x.attackType == this.lastAttack).waitingSecondsAfterAttack);
		}

		private void IssueCastToxic()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.CastToxicCoroutine()));
		}

		private IEnumerator CastToxicCoroutine()
		{
			this.LookAtPenitent();
			this.PontiffOldman.AnimatorInyector.Cast(true);
			this.CreateSpellFX(PONTIFF_SPELLS.TOXIC, 1f);
			yield return new WaitForSeconds(1f);
			int i = 10;
			for (int j = 0; j < i; j++)
			{
				Transform TP = this.bossfightPoints.GetRandomToxicPoint();
				this.toxicProjectileLauncher.projectileSource = TP;
				Vector2 dir = this.GetDirToPenitent(TP.position);
				StraightProjectile p = this.toxicProjectileLauncher.Shoot(dir);
				AcceleratedProjectile ap = p.GetComponent<AcceleratedProjectile>();
				ap.SetAcceleration(dir.normalized * 6f);
				ap.SetBouncebackData(base.transform, this.toxicOrbBounceBackOffset, 4);
				yield return new WaitForSeconds(0.8f);
			}
			this.PontiffOldman.AnimatorInyector.Cast(false);
			this.OnCastToxicEnds();
			yield break;
		}

		private Vector2 GetDirToPenitent(Vector3 from)
		{
			return Core.Logic.Penitent.transform.position - from;
		}

		private void OnCastToxicEnds()
		{
			this.StartWaitingPeriod(this.attackLapses.Find((PontiffOldmanBehaviour.PontiffOldmanAttackConfig x) => x.attackType == this.lastAttack).waitingSecondsAfterAttack);
		}

		private void IssueReposition()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.RepositionFarCoroutine()));
		}

		private void IssueRepositionMid()
		{
			this.StartAttackAction();
			Transform pointInCenter = this.bossfightPoints.GetPointInCenter();
			this._lastRepositionPoint = pointInCenter;
			this.SetCurrentCoroutine(base.StartCoroutine(this.RepositionCoroutine(pointInCenter.position)));
		}

		private IEnumerator RepositionFarCoroutine()
		{
			this.PontiffOldman.AnimatorInyector.Vanish(true);
			this.ActivateCollisions(false);
			yield return base.StartCoroutine(this.BlockUntilAnimationEnds());
			this.LookAtPenitent();
			yield return new WaitForSeconds(this.vanishSeconds);
			Transform p = this.bossfightPoints.GetPointAwayOfPenitent(Core.Logic.Penitent.transform.position);
			base.transform.position = p.position;
			this._lastRepositionPoint = p;
			this.LookAtPenitent();
			this.PontiffOldman.AnimatorInyector.Vanish(false);
			this.EndReposition();
			yield break;
		}

		private IEnumerator RepositionCoroutine(Vector2 newPosition)
		{
			this.PontiffOldman.AnimatorInyector.Vanish(true);
			yield return base.StartCoroutine(this.BlockUntilAnimationEnds());
			base.transform.position = newPosition;
			this.LookAtPenitent();
			yield return new WaitForSeconds(this.vanishSeconds);
			this.PontiffOldman.AnimatorInyector.Vanish(false);
			this.EndReposition();
			yield break;
		}

		private void EndReposition()
		{
			this.ActivateCollisions(true);
			this.StartWaitingPeriod(this.attackLapses.Find((PontiffOldmanBehaviour.PontiffOldmanAttackConfig x) => x.attackType == this.lastAttack).waitingSecondsAfterAttack);
		}

		public void OnEnterCast()
		{
			this._fsm.ChangeState(this.stCast);
		}

		public void OnExitCast()
		{
			this._fsm.ChangeState(this.stAction);
		}

		public void OnVanishEnds()
		{
			this._waitingForAnimationFinish = false;
		}

		private IEnumerator BlockUntilAnimationEnds()
		{
			this._waitingForAnimationFinish = true;
			while (this._waitingForAnimationFinish)
			{
				yield return null;
			}
			UnityEngine.Debug.Log("<color=yellow>Melee animation ended</color>");
			yield break;
		}

		public void OnHitReactionAnimationCompleted()
		{
			UnityEngine.Debug.Log("HIT REACTION COMPLETED. RECOVERING FALSE");
			this.SetRecovering(false);
			this._currentRecoveryHits = 0;
		}

		public void AttackDisplacement(float duration = 0.4f, float displacement = 2f, bool trail = true)
		{
			this.SetGhostTrail(trail);
			this.PontiffOldman.DamageByContact = false;
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
			this.PontiffOldman.DamageByContact = true;
			this.SetGhostTrail(false);
		}

		public void BackDisplacement(float duration = 0.4f, float displacement = 2f)
		{
			this.SetGhostTrail(true);
			this.PontiffOldman.DamageByContact = false;
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

		public bool CloseToPointX(Vector2 p, float closeDistance = 0.1f)
		{
			return Mathf.Abs(p.x - base.transform.position.x) < closeDistance;
		}

		public bool CloseToTarget(float closeDistance = 0.5f)
		{
			Transform target = base.GetTarget();
			return Mathf.Abs(target.position.x - base.transform.position.x) < closeDistance;
		}

		public void ChangeToAction()
		{
			this._fsm.ChangeState(this.stAction);
		}

		private void StopAllMachineGuns()
		{
			this.fireMachineGun.StopMachinegun();
		}

		public void Death()
		{
			this.SetGhostTrail(false);
			base.StopAllCoroutines();
			GameplayUtils.DestroyAllProjectiles();
			Core.Logic.Penitent.Status.Invulnerable = true;
			this.StopAllMachineGuns();
			GameplayUtils.DestroyAllProjectiles();
			base.transform.DOKill(true);
			base.StopBehaviour();
			this.PontiffOldman.AnimatorInyector.Death();
			this._fsm.ChangeState(this.stDeath);
			base.StartCoroutine(this.TransitionCoroutine());
		}

		private IEnumerator TransitionCoroutine()
		{
			this.endingParticles.Play();
			this.endingPanel.DOFade(0.3f, 1f).SetEase(Ease.InOutQuad).OnComplete(delegate
			{
				this.endingPanel.DOFade(0.9f, 2f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
			});
			yield return new WaitForSeconds(1f);
			Core.Logic.Penitent.Status.Invulnerable = false;
			yield break;
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
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
			if (this._currentRecoveryHits < this.maxHitsInRecovery)
			{
				base.StopAllCoroutines();
				this.PontiffOldman.AnimatorInyector.Hurt();
				base.transform.DOKill(true);
				this.LookAtPenitent();
				float displacement = (!this.PontiffOldman.IsGuarding) ? 0.4f : 1.2f;
				this.PontiffOldman.AnimatorInyector.Cast(false);
				this.BackDisplacement(0.3f, displacement);
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					this._currentRecoveryHits,
					(!this._recovering) ? "FALSE" : "TRUE"
				}));
				this._currentRecoveryHits++;
				if (this._currentRecoveryHits >= this.maxHitsInRecovery)
				{
					this.IssueReposition();
				}
				else
				{
					this.StartWaitingPeriod(1f);
				}
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			this.PontiffOldman.SetOrientation((targetPos.x <= this.PontiffOldman.transform.position.x) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
		}

		public override void StopMovement()
		{
		}

		public void SetGhostTrail(bool active)
		{
			this.PontiffOldman.GhostTrail.EnableGhostTrail = active;
		}

		private float GetDirFromOrientation()
		{
			return (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
		}

		private Vector2 ClampToFightBoundaries(Vector2 dir)
		{
			Vector2 vector = dir;
			UnityEngine.Debug.Log("<color=cyan>DRAWING DIR LINE IN GREEN</color>");
			UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + vector, Color.green, 5f);
			if (Physics2D.RaycastNonAlloc(base.transform.position, dir, this.results, dir.magnitude, this.fightBoundariesLayerMask) > 0)
			{
				UnityEngine.Debug.DrawLine(base.transform.position, this.results[0].point, Color.red, 5f);
				vector = vector.normalized * this.results[0].distance;
				vector *= 0.9f;
				UnityEngine.Debug.Log("<color=cyan>CLAMPING DISPLACEMENT</color>");
			}
			return vector;
		}

		public void OnDrawGizmos()
		{
		}

		[FoldoutGroup("Debug", true, 0)]
		public BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public PontiffOldmanBehaviour.PontiffOldman_ATTACKS lastAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack lightningAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack instantLightningAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossStraightProjectileAttack magicProjectileLauncher;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossMachinegunShooter fireMachineGun;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossStraightProjectileAttack toxicProjectileLauncher;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public PontiffOldmanBossfightPoints bossfightPoints;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public WindAreaEffect windArea;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public BossEnemySpawn ashChargerSpawn;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public SpriteRenderer castLoopFx;

		[FoldoutGroup("References", 0)]
		public List<MaterialsBySpellType> matsBySpell;

		public SpriteRenderer endingPanel;

		[SerializeField]
		[FoldoutGroup("Prefabs References", 0)]
		public GameObject fireSignPrefab;

		[SerializeField]
		[FoldoutGroup("Prefabs References", 0)]
		public GameObject lightningSignPrefab;

		[SerializeField]
		[FoldoutGroup("Prefabs References", 0)]
		public GameObject toxicSignPrefab;

		[SerializeField]
		[FoldoutGroup("Prefabs References", 0)]
		public GameObject magicSignPrefab;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private List<PontiffOldmanBehaviour.PontiffOldmanAttackConfig> attackLapses;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private List<PontiffOldmanBehaviour.PontiffOldmanPhases> phases;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private int maxHitsInRecovery = 3;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private LayerMask fightBoundariesLayerMask;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private float ashChargerSpawnLapse = 10f;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private float vanishSeconds = 0.5f;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private float minDistanceToVanish = 5f;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private Vector2 toxicOrbBounceBackOffset;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private float castSignOffsetDistance = 3f;

		public ParticleSystem endingParticles;

		private Transform currentTarget;

		private StateMachine<PontiffOldmanBehaviour> _fsm;

		private State<PontiffOldmanBehaviour> stAction;

		private State<PontiffOldmanBehaviour> stIntro;

		private State<PontiffOldmanBehaviour> stCast;

		private State<PontiffOldmanBehaviour> stDeath;

		private Coroutine currentCoroutine;

		private PontiffOldmanBehaviour.PontiffOldman_PHASES _currentPhase;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS> comboMagicA;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS> comboMagicB;

		private List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS> currentlyAvailableAttacks;

		private List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS> queuedActions;

		private RaycastHit2D[] results;

		private bool _recovering;

		private int _currentRecoveryHits;

		private bool _isBeingParried;

		private int _comboActionsRemaining;

		private float _ashChargerSpawnCounter;

		private bool _waitingForAnimationFinish;

		private bool _ashChargerActive;

		private Transform _lastRepositionPoint;

		[Serializable]
		public struct PontiffOldmanPhases
		{
			public PontiffOldmanBehaviour.PontiffOldman_PHASES phaseId;

			public List<PontiffOldmanBehaviour.PontiffOldman_ATTACKS> availableAttacks;
		}

		public enum PontiffOldman_PHASES
		{
			FIRST,
			SECOND,
			LAST
		}

		[Serializable]
		public struct PontiffOldmanAttackConfig
		{
			public PontiffOldmanBehaviour.PontiffOldman_ATTACKS attackType;

			public float preparationSeconds;

			public float waitingSecondsAfterAttack;
		}

		public enum PontiffOldman_ATTACKS
		{
			COMBO_REST,
			REPOSITION,
			CAST_FIRE,
			CAST_TOXIC,
			CAST_LIGHTNING,
			CAST_MAGIC,
			REPOSITION_MID,
			COMBO_MAGIC_A,
			COMBO_MAGIC_B
		}
	}
}
