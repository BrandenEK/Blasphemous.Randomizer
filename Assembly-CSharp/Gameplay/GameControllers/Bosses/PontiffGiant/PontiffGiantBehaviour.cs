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
using Gameplay.GameControllers.Bosses.PontiffGiant.AI;
using Gameplay.GameControllers.Bosses.PontiffOldman;
using Gameplay.GameControllers.Bosses.PontiffSword;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Environment;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PontiffGiant
{
	public class PontiffGiantBehaviour : EnemyBehaviour
	{
		public PontiffGiant PontiffGiant { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PontiffGiantBehaviour> OnActionFinished;

		public override void OnAwake()
		{
			base.OnAwake();
			this.stIntro = new PontiffGiant_StIntro();
			this.stAction = new PontiffGiant_StAction();
			this.stDeath = new PontiffGiant_StDeath();
			this._fsm = new StateMachine<PontiffGiantBehaviour>(this, this.stIntro, null, null);
			this.results = new RaycastHit2D[1];
			this.currentlyAvailableAttacks = new List<PontiffGiantBehaviour.PontiffGiant_ATTACKS>
			{
				PontiffGiantBehaviour.PontiffGiant_ATTACKS.CAST_FIRE,
				PontiffGiantBehaviour.PontiffGiant_ATTACKS.CAST_MAGIC,
				PontiffGiantBehaviour.PontiffGiant_ATTACKS.CAST_TOXIC
			};
		}

		public override void OnStart()
		{
			base.OnStart();
			this.PontiffGiant = (PontiffGiant)this.Entity;
			this.ChangeBossState(BOSS_STATES.WAITING);
			this.sword.OnSwordDestroyed += this.OnSwordDestroyed;
			this.PontiffGiant.IsGuarding = true;
			PoolManager.Instance.CreatePool(this.deathExplosionPrefab, 10);
			PoolManager.Instance.CreatePool(this.fireSignPrefab, 3);
			PoolManager.Instance.CreatePool(this.toxicSignPrefab, 3);
			PoolManager.Instance.CreatePool(this.lightningSignPrefab, 3);
			PoolManager.Instance.CreatePool(this.magicSignPrefab, 3);
			PoolManager.Instance.CreatePool(this.swordMagicPrefab, 3);
			this.StartIntroSequence();
		}

		private void OnSwordDestroyed()
		{
			this.PontiffGiant.AnimatorInyector.Open(true);
			base.StopAllCoroutines();
			this._swordRevivalCounter = this.maxSwordDeathTime;
			this.StartWaitingPeriod(1.5f);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
			if (this._swordRevivalCounter > 0f && this._fsm.IsInState(this.stAction))
			{
				this._swordRevivalCounter -= Time.deltaTime;
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
			this._comboActionsRemaining--;
			if (this._comboActionsRemaining == 0)
			{
				this.QueuedActionsPush(PontiffGiantBehaviour.PontiffGiant_ATTACKS.COMBO_REST);
			}
		}

		private void CancelCombo()
		{
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

		public void LaunchAction(PontiffGiantBehaviour.PontiffGiant_ATTACKS atk)
		{
			switch (atk)
			{
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.COMBO_REST:
				this.StartWaitingPeriod(1.5f);
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.SLASH:
				this.IssueSlash();
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.CAST_FIRE:
				this.IssueCastFire();
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.CAST_TOXIC:
				this.IssueCastToxic();
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.CAST_LIGHTNING:
				this.IssueCastLightning();
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.CAST_MAGIC:
				this.IssueCastMagic();
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.PLUNGE:
				this.IssuePlunge();
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.REVIVE_SWORD:
				this.PontiffGiant.AnimatorInyector.Open(false);
				this.sword.PontiffSword.Revive();
				this.StartWaitingPeriod(2f);
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.DOUBLE_SLASH:
				this.IssueDoubleSlash();
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.BEAM_ATTACK_1:
				this.IssueCastBeam(0);
				break;
			case PontiffGiantBehaviour.PontiffGiant_ATTACKS.BEAM_ATTACK_2:
				this.IssueCastBeam(1);
				break;
			}
			this.lastAttack = atk;
		}

		public PontiffGiantBehaviour.PontiffGiant_ATTACKS GetNewAttack()
		{
			if (this.queuedActions != null && this.queuedActions.Count > 0)
			{
				return this.QueuedActionsPop();
			}
			if (this._swordRevivalCounter < 0f)
			{
				this._swordRevivalCounter = 0f;
				this.LaunchAction(PontiffGiantBehaviour.PontiffGiant_ATTACKS.REVIVE_SWORD);
			}
			PontiffGiantBehaviour.PontiffGiant_ATTACKS[] array = new PontiffGiantBehaviour.PontiffGiant_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<PontiffGiantBehaviour.PontiffGiant_ATTACKS> list = new List<PontiffGiantBehaviour.PontiffGiant_ATTACKS>(array);
			if (this.sword.currentSwordState == SWORD_STATES.DESTROYED)
			{
				list.Remove(PontiffGiantBehaviour.PontiffGiant_ATTACKS.SLASH);
				list.Remove(PontiffGiantBehaviour.PontiffGiant_ATTACKS.DOUBLE_SLASH);
				list.Remove(PontiffGiantBehaviour.PontiffGiant_ATTACKS.PLUNGE);
			}
			if (list.Count > 1)
			{
				list.Remove(this.lastAttack);
			}
			return list[Random.Range(0, list.Count)];
		}

		public IEnumerator WaitForState(State<PontiffGiantBehaviour> st)
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

		private void QueuedActionsPush(PontiffGiantBehaviour.PontiffGiant_ATTACKS atk)
		{
			if (this.queuedActions == null)
			{
				this.queuedActions = new List<PontiffGiantBehaviour.PontiffGiant_ATTACKS>();
			}
			this.queuedActions.Add(atk);
		}

		private PontiffGiantBehaviour.PontiffGiant_ATTACKS QueuedActionsPop()
		{
			PontiffGiantBehaviour.PontiffGiant_ATTACKS pontiffGiant_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(pontiffGiant_ATTACKS);
			return pontiffGiant_ATTACKS;
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public float GetHealthPercentage()
		{
			return this.PontiffGiant.CurrentLife / this.PontiffGiant.Stats.Life.Base;
		}

		private void SetPhase(PontiffGiantBehaviour.PontiffGiantPhases p)
		{
			this.currentlyAvailableAttacks = p.availableAttacks;
			this._currentPhase = p.phaseId;
		}

		private void ChangePhase(PontiffGiantBehaviour.PontiffGiant_PHASES p)
		{
			PontiffGiantBehaviour.PontiffGiantPhases phase = this.phases.Find((PontiffGiantBehaviour.PontiffGiantPhases x) => x.phaseId == p);
			this.SetPhase(phase);
		}

		private void CheckNextPhase()
		{
			float healthPercentage = this.GetHealthPercentage();
			PontiffGiantBehaviour.PontiffGiant_PHASES currentPhase = this._currentPhase;
			if (currentPhase != PontiffGiantBehaviour.PontiffGiant_PHASES.FIRST)
			{
				if (currentPhase != PontiffGiantBehaviour.PontiffGiant_PHASES.SECOND)
				{
					if (currentPhase != PontiffGiantBehaviour.PontiffGiant_PHASES.LAST)
					{
					}
				}
				else if (healthPercentage < 0.3f)
				{
					this.ChangePhase(PontiffGiantBehaviour.PontiffGiant_PHASES.LAST);
				}
			}
			else if (healthPercentage < 0.6f)
			{
				this.ChangePhase(PontiffGiantBehaviour.PontiffGiant_PHASES.SECOND);
			}
		}

		public void IssueCombo(List<PontiffGiantBehaviour.PontiffGiant_ATTACKS> testCombo)
		{
			for (int i = 0; i < testCombo.Count; i++)
			{
				this.QueuedActionsPush(testCombo[i]);
			}
			this._comboActionsRemaining = testCombo.Count;
			this.StartWaitingPeriod(0.1f);
		}

		private IEnumerator GetIntoStateAndCallback(State<PontiffGiantBehaviour> newSt, float waitSeconds, Action callback)
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
			this.ChangePhase(PontiffGiantBehaviour.PontiffGiant_PHASES.FIRST);
			this.PontiffGiant.AnimatorInyector.IntroBlend();
			yield return new WaitForSeconds(6.5f);
			base.BehaviourTree.StartBehaviour();
			this.ActivateCollisions(true);
			this.sword.PontiffSword.Revive();
			this._fsm.ChangeState(this.stAction);
			this.StartWaitingPeriod(1.5f);
			this.ashPlatformManager.Activate();
			yield break;
		}

		private void ActivateCollisions(bool activate)
		{
			this.PontiffGiant.DamageArea.DamageAreaCollider.enabled = activate;
		}

		private void Shake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 1f, 12, 0.2f, 0f, default(Vector3), 0f, false);
		}

		private void IssueSlash()
		{
			this.StartAttackAction();
			this.sword.ChangeSwordState(SWORD_STATES.MID_ACTION);
			this.SetCurrentCoroutine(base.StartCoroutine(this.SlashCoroutine()));
		}

		private IEnumerator SlashCoroutine()
		{
			Vector2 swordOffset = new Vector2(4f, 6f);
			Vector2 dir = this.GetDirToPenitent(this.sword.transform.position);
			Vector3 offset = swordOffset;
			float sign = -Mathf.Sign(dir.x);
			offset.x *= sign;
			this.sword.Move(Core.Logic.Penitent.transform.position + offset, 1.5f, new TweenCallback(this.OnSwordMoveFinished));
			yield return new WaitForSeconds(0.1f);
			yield break;
		}

		private void OnSwordMoveFinished()
		{
			this.sword.OnSlashFinished += this.Sword_OnSlashFinished;
			this.sword.Slash();
		}

		private void Sword_OnSlashFinished()
		{
			this.sword.OnSlashFinished -= this.Sword_OnSlashFinished;
			this.sword.BackToFlyingAround();
			this.StartWaitingPeriod(0.5f);
		}

		private void IssueDoubleSlash()
		{
			this.StartAttackAction();
			this.sword.ChangeSwordState(SWORD_STATES.MID_ACTION);
			this._swordAttacksRemaining = 2;
			this.SetCurrentCoroutine(base.StartCoroutine(this.DoubleSlashCoroutine()));
		}

		private IEnumerator DoubleSlashCoroutine()
		{
			while (this._swordAttacksRemaining > 0)
			{
				Vector2 swordOffset = new Vector2(4f, 6f);
				Vector2 dir = this.GetDirToPenitent(this.sword.transform.position);
				Vector3 offset = swordOffset;
				float sign = -Mathf.Sign(dir.x);
				offset.x *= sign;
				this._swordMoveFinished = false;
				this.sword.Move(Core.Logic.Penitent.transform.position + offset, 1.5f, new TweenCallback(this.OnSwordDoubleSlashMovementFinished));
				yield return this.WaitForSwordMovement();
				this.sword.Slash();
				yield return new WaitForSeconds(2f);
				this._swordAttacksRemaining--;
			}
			this.Sword_OnDoubleSlashFinished();
			yield break;
		}

		private IEnumerator WaitForSwordMovement()
		{
			while (!this._swordMoveFinished)
			{
				yield return null;
			}
			yield break;
		}

		private void OnSwordDoubleSlashMovementFinished()
		{
			this._swordMoveFinished = true;
			this.sword.OnSlashFinished += this.Sword_OnDoubleSlashFinished;
		}

		private void Sword_OnDoubleSlashFinished()
		{
			this.sword.OnSlashFinished -= this.Sword_OnDoubleSlashFinished;
			this.sword.BackToFlyingAround();
			this.StartWaitingPeriod(0.5f);
		}

		private void IssueCastFire()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.CastFireCoroutine()));
		}

		private IEnumerator CastFireCoroutine()
		{
			this.CreateSpellFX(PONTIFF_SPELLS.FIRE, 2f);
			yield return new WaitForSeconds(1f);
			float d = this.GetDirFromOrientation();
			float machineGunDelay = 1f;
			foreach (BossMachinegunShooter item in this.machineGuns)
			{
				item.StartAttack(Core.Logic.Penitent.transform);
				yield return new WaitForSeconds(machineGunDelay);
			}
			yield return new WaitForSeconds(5.5f);
			this.OnCastFireEnds();
			yield break;
		}

		private void CreateFireSign()
		{
			float dirFromOrientation = this.GetDirFromOrientation();
			PoolManager.Instance.ReuseObject(this.fireSignPrefab, base.transform.position + dirFromOrientation * Vector3.right, Quaternion.identity, false, 1);
		}

		private void OnCastFireEnds()
		{
			this.StartWaitingPeriod(0.1f);
		}

		private void IssueCastMagic()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.CastMagicCoroutine()));
		}

		private void IssueCastBeam(int type)
		{
			this.StartAttackAction();
			if (type == 0)
			{
				this.SetCurrentCoroutine(base.StartCoroutine(this.CastBeams()));
			}
			else
			{
				this.SetCurrentCoroutine(base.StartCoroutine(this.CastBeamsAlternate()));
			}
		}

		private IEnumerator CastBeams()
		{
			this.CreateSpellFX(PONTIFF_SPELLS.MAGIC, 3f);
			this.PontiffGiant.Audio.PlayPurpleSpell_AUDIO();
			yield return new WaitForSeconds(1f);
			float timeBetweenBeams = 3f;
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[0].position, 0f, 1f, null);
			yield return new WaitForSeconds(timeBetweenBeams);
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[1].position, 0f, 1f, null);
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[2].position, 0f, 1f, null);
			yield return new WaitForSeconds(timeBetweenBeams);
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[3].position, 0f, 1f, null);
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[4].position, 0f, 1f, null);
			this.OnCastMagicEnds();
			yield break;
		}

		private IEnumerator CastBeamsAlternate()
		{
			this.CreateSpellFX(PONTIFF_SPELLS.MAGIC, 3f);
			this.PontiffGiant.Audio.PlayPurpleSpell_AUDIO();
			yield return new WaitForSeconds(1f);
			float timeBetweenBeams = 2f;
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[3].position, 0f, 1f, null);
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[4].position, 0f, 1f, null);
			yield return new WaitForSeconds(timeBetweenBeams);
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[1].position, 0f, 1f, null);
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[2].position, 0f, 1f, null);
			yield return new WaitForSeconds(timeBetweenBeams);
			this.beamAttack.SummonAreaOnPoint(this.bossfightPoints.beamPoints[0].position, 0f, 1f, null);
			this.OnCastMagicEnds();
			yield break;
		}

		private IEnumerator MagicExplosion(Vector2 attackPoint, float delay)
		{
			GameObject go = PoolManager.Instance.ReuseObject(this.swordMagicPrefab, attackPoint, Quaternion.identity, false, 1).GameObject;
			yield return new WaitForSeconds(delay);
			this.MagicProyectileExplosion(attackPoint);
			yield break;
		}

		private IEnumerator CastMagicCoroutine()
		{
			this.CreateSpellFX(PONTIFF_SPELLS.MAGIC, 3f);
			this.PontiffGiant.Audio.PlayPurpleSpell_AUDIO();
			float explosionDelay = 0.8f;
			yield return new WaitForSeconds(1f);
			Vector2 pos = this.bossfightPoints.magicPoints[1].position;
			pos = this.bossfightPoints.magicPoints[1].position;
			base.StartCoroutine(this.MagicExplosion(pos, explosionDelay));
			pos = this.bossfightPoints.magicPoints[2].position;
			base.StartCoroutine(this.MagicExplosion(pos, explosionDelay));
			this.OnCastMagicEnds();
			yield break;
		}

		private void CreateSpellFX(PONTIFF_SPELLS spellType, float signOffset)
		{
			Vector3 position = this.bossfightPoints.magicPoints[1].position;
			base.StartCoroutine(this.CreateSignDelayed(spellType, position, 0f));
			position = this.bossfightPoints.magicPoints[2].position;
			base.StartCoroutine(this.CreateSignDelayed(spellType, position, 0f));
		}

		private void MagicProyectileExplosion(Vector2 point)
		{
			Vector2 vector;
			vector..ctor(-1f, 0f);
			this.magicProjectileLauncher.projectileSource = this.magicProjectileLauncher.transform;
			this.magicProjectileLauncher.transform.position = point;
			this.magicProjectileLauncher.Shoot(vector.normalized);
			vector..ctor(1f, 0f);
			this.magicProjectileLauncher.Shoot(vector.normalized);
			vector..ctor(1f, 1f);
			this.magicProjectileLauncher.Shoot(vector.normalized);
			vector..ctor(-1f, 1f);
			this.magicProjectileLauncher.Shoot(vector.normalized);
			vector..ctor(1f, -1f);
			this.magicProjectileLauncher.Shoot(vector.normalized);
			vector..ctor(-1f, -1f);
			this.magicProjectileLauncher.Shoot(vector.normalized);
		}

		private IEnumerator CreateSignDelayed(PONTIFF_SPELLS spellType, Vector2 position, float delay)
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
			GameObject go = PoolManager.Instance.ReuseObject(signPrefab, position, Quaternion.identity, false, 1).GameObject;
			yield break;
		}

		private void OnCastMagicEnds()
		{
			this.StartWaitingPeriod(2f);
		}

		private void IssuePlunge()
		{
			this.StartAttackAction();
			this.sword.ChangeSwordState(SWORD_STATES.MID_ACTION);
			this.SetCurrentCoroutine(base.StartCoroutine(this.PlungeCoroutine()));
		}

		private IEnumerator PlungeCoroutine()
		{
			this.sword.OnPlungeFinished += this.OnSwordPlungeFinished;
			this.sword.Plunge();
			yield return new WaitForSeconds(6f);
			this.OnPlungeEnds();
			yield break;
		}

		private void OnSwordPlungeFinished()
		{
			this.sword.OnPlungeFinished -= this.OnSwordPlungeFinished;
			Vector2 vector = this.sword.transform.position;
			this.magicShockwave.transform.position = vector + Vector2.up;
			this.magicShockwave.SummonAreas(Vector2.right);
			this.magicShockwave.SummonAreas(Vector2.left);
			if (this.sword.currentSwordState != SWORD_STATES.DESTROYED)
			{
				this.sword.BackToFlyingAround();
			}
		}

		private void OnPlungeEnds()
		{
			this.StartWaitingPeriod(0.1f);
		}

		private void IssueCastLightning()
		{
			this.StartAttackAction();
			if (this.sword.currentSwordState == SWORD_STATES.DESTROYED)
			{
				this.SetCurrentCoroutine(base.StartCoroutine(this.CastSingleLightningCoroutine()));
			}
			else
			{
				this.SetCurrentCoroutine(base.StartCoroutine(this.CastLightningCoroutine()));
			}
		}

		private IEnumerator CastSingleLightningCoroutine()
		{
			this.CreateSpellFX(PONTIFF_SPELLS.LIGHTNING, 0f);
			this.PontiffGiant.Audio.PlayBlueSpell_AUDIO();
			yield return new WaitForSeconds(1f);
			Vector2 pos = Core.Logic.Penitent.transform.position;
			this.lightningAreas.SummonAreaOnPoint(pos, 0f, 1f, null);
			yield return new WaitForSeconds(2f);
			pos = Core.Logic.Penitent.transform.position;
			this.lightningAreas.SummonAreaOnPoint(pos, 0f, 1f, null);
			yield return new WaitForSeconds(2f);
			pos = Core.Logic.Penitent.transform.position;
			this.lightningAreas.SummonAreaOnPoint(pos, 0f, 1f, null);
			this.OnCastLightningEnds();
			yield break;
		}

		private IEnumerator CastLightningCoroutine()
		{
			this.CreateSpellFX(PONTIFF_SPELLS.LIGHTNING, 0f);
			this.PontiffGiant.Audio.PlayBlueSpell_AUDIO();
			yield return new WaitForSeconds(1f);
			this.lightningAreas.transform.position = this.bossfightPoints.leftLimitTransform.position;
			this.lightningAreas.SummonAreas(Vector2.right);
			yield return new WaitForSeconds(3f);
			this.lightningAreas.transform.position = this.bossfightPoints.rightLimitTransform.position;
			this.lightningAreas.SummonAreas(Vector2.left);
			yield return new WaitForSeconds(1f);
			this.OnCastLightningEnds();
			yield break;
		}

		private void OnCastLightningEnds()
		{
			this.StartWaitingPeriod(1f);
		}

		private void IssueCastToxic()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.CastToxicCoroutine()));
		}

		private Vector2 GetPointAroundPenitent()
		{
			float num = 5f;
			return Core.Logic.Penitent.transform.position + this.GetRandomVector(-0.8f, 0.8f, 0f, 1f).normalized * num;
		}

		private Vector2 GetRandomVector(float minRandomX, float maxRandomX, float minRandomY, float maxRandomY)
		{
			return new Vector2(Random.Range(minRandomX, maxRandomX), Random.Range(minRandomY, maxRandomY));
		}

		private IEnumerator CastToxicCoroutine()
		{
			this.PontiffGiant.Audio.PlayGreenSpell_AUDIO();
			this.CreateSpellFX(PONTIFF_SPELLS.TOXIC, 0f);
			yield return new WaitForSeconds(1f);
			int i = 5;
			for (int j = 0; j < i; j++)
			{
				Vector2 TP = this.bossfightPoints.fightCenterTransform.position + Vector2.up * 8.5f - Vector2.right * 9f;
				TP += Vector2.right * (float)j * 4.5f;
				this.toxicProjectileLauncher.transform.position = TP;
				Vector2 dir = this.GetDirToPenitent(TP);
				StraightProjectile p = this.toxicProjectileLauncher.Shoot(dir);
				AcceleratedProjectile ap = p as AcceleratedProjectile;
				ap.SetAcceleration(dir.normalized * 4f);
				if (this.sword.currentSwordState == SWORD_STATES.FLYING_AROUND)
				{
					ap.bounceBackToTarget = true;
					ap.SetBouncebackData(this.sword.PontiffSword.damageEffectScript.transform, Vector2.zero, 4);
				}
				else
				{
					ap.bounceBackToTarget = false;
				}
				yield return new WaitForSeconds(0.7f);
			}
			this.OnCastToxicEnds();
			yield break;
		}

		private Vector2 GetDirToPenitent(Vector3 from)
		{
			return Core.Logic.Penitent.transform.position - from;
		}

		private void OnCastToxicEnds()
		{
			this.StartWaitingPeriod(2f);
		}

		public void OnMaskOpened()
		{
			this.PontiffGiant.IsGuarding = false;
			this.ashPlatformManager.heightLimitOn = false;
		}

		private void LaunchMaskOpenAttack()
		{
			this.magicProjectileLauncher.projectileSource = this.magicProjectileLauncher.transform;
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.swordMagicPrefab, this.magicProjectileLauncher.projectileSource.transform.position, Quaternion.identity, false, 1).GameObject;
			gameObject.transform.SetParent(this.magicProjectileLauncher.projectileSource, true);
			Vector2 up = Vector2.up;
			Vector2 vector = Vector2.zero;
			int num = 6;
			up = Vector2.up;
			vector = Vector2.zero;
			float num2 = 170f;
			for (int i = 0; i < num; i++)
			{
				float num3 = Mathf.Lerp(0f, num2, (float)i / (float)num);
				Quaternion quaternion = Quaternion.Euler(0f, 0f, num3);
				Quaternion quaternion2 = Quaternion.Euler(0f, 0f, -num3);
				vector = quaternion * up;
				this.magicProjectileLauncher.Shoot(vector.normalized);
				vector = quaternion2 * up;
				this.magicProjectileLauncher.Shoot(vector.normalized);
			}
		}

		public void OnMaskClosed()
		{
			this.PontiffGiant.IsGuarding = true;
			this.ashPlatformManager.heightLimitOn = false;
		}

		private IEnumerator BlockUntilAnimationEnds()
		{
			this._waitingForAnimationFinish = true;
			while (this._waitingForAnimationFinish)
			{
				yield return null;
			}
			yield break;
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

		[Button(0)]
		public void Death()
		{
			this.StopAllAttacks();
			base.StopAllCoroutines();
			Core.Logic.Penitent.Status.Invulnerable = true;
			ShortcutExtensions.DOKill(base.transform, true);
			base.StopBehaviour();
			GameplayUtils.DestroyAllProjectiles();
			this._fsm.ChangeState(this.stDeath);
			this.sword.PontiffSword.Kill();
			this.ashPlatformManager.Deactivate();
			this.StartDeathSequence();
		}

		private void StopAllAttacks()
		{
			foreach (BossMachinegunShooter bossMachinegunShooter in this.machineGuns)
			{
				bossMachinegunShooter.StopMachinegun();
			}
		}

		private void StartDeathSequence()
		{
			base.StartCoroutine(this.DeathSequence());
		}

		private IEnumerator DeathSequence()
		{
			yield return new WaitForSeconds(1f);
			this.PontiffGiant.AnimatorInyector.Death();
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 2.2f, 0.3f, 1.8f);
			yield return new WaitForSeconds(1f);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.3f, new Vector2(1.3f, 0f), 16, 0.25f, 0f, default(Vector3), 0.04f, true);
			yield return new WaitForSeconds(2f);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.3f, new Vector2(1.3f, 0f), 16, 0.25f, 0f, default(Vector3), 0.04f, true);
			yield return new WaitForSeconds(1f);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(3f, new Vector2(1.3f, 1.5f), 120, 0.25f, 0f, default(Vector3), 0.04f, true);
			this.deathEffectAnim.SetTrigger("TRIGGER");
			Core.Logic.Penitent.Status.Invulnerable = false;
			yield break;
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

		public override void Idle()
		{
			this.StopMovement();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
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
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			this.PontiffGiant.SetOrientation((targetPos.x <= this.PontiffGiant.transform.position.x) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
		}

		public override void StopMovement()
		{
		}

		public void SetGhostTrail(bool active)
		{
			this.PontiffGiant.GhostTrail.EnableGhostTrail = active;
		}

		private float GetDirFromOrientation()
		{
			return (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
		}

		private Vector2 ClampToFightBoundaries(Vector2 dir)
		{
			Vector2 vector = dir;
			if (Physics2D.RaycastNonAlloc(base.transform.position, dir, this.results, dir.magnitude, this.fightBoundariesLayerMask) > 0)
			{
				vector = vector.normalized * this.results[0].distance;
				vector *= 0.9f;
			}
			return vector;
		}

		public void OnDrawGizmos()
		{
		}

		[FoldoutGroup("Debug", true, 0)]
		public BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public PontiffGiantBehaviour.PontiffGiant_ATTACKS lastAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private PontiffSwordBehaviour sword;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack lightningAreas;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack magicShockwave;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossStraightProjectileAttack toxicProjectileLauncher;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossStraightProjectileAttack magicProjectileLauncher;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public PontiffGiantBossfightPoints bossfightPoints;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public List<BossMachinegunShooter> machineGuns;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public GameObject deathExplosionPrefab;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public AshPlatformFightManager ashPlatformManager;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack beamAttack;

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
		[FoldoutGroup("Prefabs References", 0)]
		public GameObject swordMagicPrefab;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private List<PontiffGiantBehaviour.PontiffGiantPhases> phases;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private int maxHitsInRecovery = 3;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private LayerMask fightBoundariesLayerMask;

		public Animator deathEffectAnim;

		private Transform currentTarget;

		private StateMachine<PontiffGiantBehaviour> _fsm;

		private State<PontiffGiantBehaviour> stAction;

		private State<PontiffGiantBehaviour> stIntro;

		private State<PontiffGiantBehaviour> stCast;

		private State<PontiffGiantBehaviour> stDeath;

		private Coroutine currentCoroutine;

		private PontiffGiantBehaviour.PontiffGiant_PHASES _currentPhase;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<PontiffGiantBehaviour.PontiffGiant_ATTACKS> comboMagicA;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<PontiffGiantBehaviour.PontiffGiant_ATTACKS> comboMagicB;

		private List<PontiffGiantBehaviour.PontiffGiant_ATTACKS> currentlyAvailableAttacks;

		private List<PontiffGiantBehaviour.PontiffGiant_ATTACKS> queuedActions;

		private RaycastHit2D[] results;

		public float maxSwordDeathTime = 10f;

		private bool _recovering;

		private int _currentRecoveryHits;

		private bool _isBeingParried;

		private int _comboActionsRemaining;

		private bool _waitingForAnimationFinish;

		private float _swordRevivalCounter;

		private int _swordAttacksRemaining;

		private bool _swordMoveFinished;

		[Serializable]
		public struct PontiffGiantPhases
		{
			public PontiffGiantBehaviour.PontiffGiant_PHASES phaseId;

			public List<PontiffGiantBehaviour.PontiffGiant_ATTACKS> availableAttacks;
		}

		public enum PontiffGiant_PHASES
		{
			FIRST,
			SECOND,
			LAST
		}

		[Serializable]
		public struct PontiffGiantAttackConfig
		{
			public PontiffGiantBehaviour.PontiffGiant_ATTACKS attackType;

			public float preparationSeconds;

			public float waitingSecondsAfterAttack;
		}

		public enum PontiffGiant_ATTACKS
		{
			COMBO_REST,
			SLASH,
			CAST_FIRE,
			CAST_TOXIC,
			CAST_LIGHTNING,
			CAST_MAGIC,
			PLUNGE,
			REVIVE_SWORD,
			DOUBLE_SLASH,
			BEAM_ATTACK_1,
			BEAM_ATTACK_2
		}
	}
}
