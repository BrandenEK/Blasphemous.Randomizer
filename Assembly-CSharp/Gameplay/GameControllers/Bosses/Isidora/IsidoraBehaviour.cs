using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Bosses.Isidora.Audio;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Isidora
{
	public class IsidoraBehaviour : EnemyBehaviour
	{
		public Isidora Isidora { get; set; }

		private void Start()
		{
			this.Isidora = (Isidora)this.Entity;
			this.InitAI();
			this.InitActionDictionary();
			this.homingBonfireBehavior = UnityEngine.Object.FindObjectOfType<HomingBonfireBehaviour>();
			this.currentFightParameters = this.allFightParameters[0];
			PoolManager.Instance.CreatePool(this.singleSparkSimpleVFX, 2);
			PoolManager.Instance.CreatePool(this.slashLineSimpleVFX, 2);
			PoolManager.Instance.CreatePool(this.attackAnticipationWarningSimpleVFX, 2);
		}

		private void OnGUI()
		{
		}

		public void ProjectileAbsortion(Vector2 projectilePosition, Vector2 projectileDirection)
		{
			if (this.Isidora.Status.Dead)
			{
				return;
			}
			base.transform.DOPunchPosition(projectileDirection.normalized * 0.1f, 0.2f, 10, 1f, false);
			this.numberOfCharges++;
			this.PlayFlameParticles();
			if (this.blinkCoroutine != null)
			{
				base.StopCoroutine(this.blinkCoroutine);
			}
			this.blinkCoroutine = base.StartCoroutine(this.BlinkAbsortion(0.1f, 3));
		}

		private IEnumerator BlinkAbsortion(float delay, int blinks)
		{
			this.absorbSpriteRenderer.flipX = this.Isidora.SpriteRenderer.flipX;
			for (int i = 0; i < blinks; i++)
			{
				this.absorbSpriteRenderer.enabled = true;
				yield return new WaitForSeconds(delay);
				this.absorbSpriteRenderer.enabled = false;
			}
			yield break;
		}

		private void DrawMusicBars()
		{
			int lastBar = this.Isidora.Audio.bossAudioSync.LastBar;
			string text = "red";
			int lastAttackMarkerBar = this.Isidora.Audio.lastAttackMarkerBar;
			if (lastAttackMarkerBar == lastBar)
			{
				text = "green";
			}
			bool isidoraVoice = this.Isidora.Audio.GetIsidoraVoice();
			string text2 = (!isidoraVoice) ? "..............................................." : "♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪♫♪";
			string text3 = string.Concat(new string[]
			{
				"<color=",
				text,
				">",
				lastBar.ToString(),
				"</color>",
				text2,
				(lastBar + 1).ToString()
			});
			GUI.Label(new Rect(800f, 30f, 300f, 20f), text3);
			float timeLeftForCurrentBar = this.Isidora.Audio.GetTimeLeftForCurrentBar();
			float singleBarDuration = this.Isidora.Audio.GetSingleBarDuration();
			GUI.HorizontalSlider(new Rect(800f, 50f, 200f, 20f), singleBarDuration - timeLeftForCurrentBar, 0f, singleBarDuration);
		}

		private void InitAI()
		{
			this.stIdle = new Isidora_StIdle();
			this.stAction = new Isidora_StAction();
			this._fsm = new StateMachine<IsidoraBehaviour>(this, this.stIdle, null, null);
		}

		private void InitActionDictionary()
		{
			this.waitBetweenActions_EA = new WaitSeconds_EnemyAction();
			this.intro_EA = new IsidoraBehaviour.Intro_EnemyAction();
			this.death_EA = new IsidoraBehaviour.Death_EnemyAction();
			this.audioSync_EA = new IsidoraBehaviour.AudioSyncTest_EnemyAction();
			this.attackSync_EA = new IsidoraBehaviour.AttackSyncTest_EnemyAction();
			this.firstCombo_EA = new IsidoraBehaviour.FirstCombo_EnemyAction();
			this.secondCombo_EA = new IsidoraBehaviour.SecondCombo_EnemyAction();
			this.thirdCombo_EA = new IsidoraBehaviour.ThirdCombo_EnemyAction();
			this.syncCombo1_EA = new IsidoraBehaviour.SyncCombo1_EnemyAction();
			this.fadeSlashCombo1_EA = new IsidoraBehaviour.FadeSlashCombo1_EnemyAction();
			this.phaseSwitch_EA = new IsidoraBehaviour.PhaseSwitchAttack_EnemyAction();
			this.homingProjectiles_EA = new IsidoraBehaviour.HomingProjectilesAttack_EnemyAction();
			this.bonfireProjectiles_EA = new IsidoraBehaviour.BonfireProjectilesAttack_EnemyAction();
			this.horizontalDash_EA = new IsidoraBehaviour.HorizontalDash_EnemyAction();
			this.blastsAttack_EA = new IsidoraBehaviour.BlastsAttack_EnemyAction();
			this.invisibleHorizontalDash_EA = new IsidoraBehaviour.InvisibleHorizontalDash_EnemyAction();
			this.chargedBlastsAttack_EA = new IsidoraBehaviour.ChargedBlastsAttack_EnemyAction();
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.AUDIO_TEST, new Func<EnemyAction>(this.LaunchAction_AudioTest));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.ATTACK_TEST, new Func<EnemyAction>(this.LaunchAction_AttackTest));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.FIRST_COMBO, new Func<EnemyAction>(this.LaunchAction_FirstCombo));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.SECOND_COMBO, new Func<EnemyAction>(this.LaunchAction_SecondCombo));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.THIRD_COMBO, new Func<EnemyAction>(this.LaunchAction_ThirdCombo));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.SYNC_COMBO_1, new Func<EnemyAction>(this.LaunchAction_FadeSlashCombo1));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.PHASE_SWITCH_ATTACK, new Func<EnemyAction>(this.LaunchAction_PhaseSwitch));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.HOMING_PROJECTILES_ATTACK, new Func<EnemyAction>(this.LaunchAction_HomingProjectiles));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.BONFIRE_PROJECTILES_ATTACK, new Func<EnemyAction>(this.LaunchAction_BonfireProjectiles));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.BONFIRE_INFINITE_PROJECTILES_ATTACK, new Func<EnemyAction>(this.LaunchAction_BonfireInfiniteProjectiles));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.HORIZONTAL_DASH, new Func<EnemyAction>(this.LaunchAction_HorizontalDash));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.BLASTS_ATTACK, new Func<EnemyAction>(this.LaunchAction_BlastsAttack));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.INVISIBLE_HORIZONTAL_DASH, new Func<EnemyAction>(this.LaunchAction_InvisibleHorizontalDash));
			this.actionsDictionary.Add(IsidoraBehaviour.ISIDORA_ATTACKS.CHARGED_BLASTS_ATTACK, new Func<EnemyAction>(this.LaunchAction_ChargedBlastsAttack));
			this.availableAttacks = this.attackConfigData.GetAttackIds(true, true, 1f);
		}

		public void Damage(Hit hit)
		{
			bool flag = this.SwapFightParametersIfNeeded();
			if (flag)
			{
				this.CheckCurrentPhase();
			}
		}

		private void CheckCurrentPhase()
		{
			if (this.queuedAttacks.Contains(IsidoraBehaviour.ISIDORA_ATTACKS.PHASE_SWITCH_ATTACK))
			{
				return;
			}
			if (this.currentFightParameters.advancePhase)
			{
				if (this.currentPhase == IsidoraBehaviour.ISIDORA_PHASES.FIRST)
				{
					this.currentPhase = IsidoraBehaviour.ISIDORA_PHASES.BRIDGE;
					this.Isidora.Audio.SetPhaseBridge();
				}
				else if (this.currentPhase == IsidoraBehaviour.ISIDORA_PHASES.BRIDGE)
				{
					this.currentPhase = IsidoraBehaviour.ISIDORA_PHASES.SECOND;
					this.QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS.PHASE_SWITCH_ATTACK);
					if (this.lastAttack == IsidoraBehaviour.ISIDORA_ATTACKS.INVISIBLE_HORIZONTAL_DASH)
					{
						this.QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS.HORIZONTAL_DASH);
					}
					else
					{
						this.QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS.INVISIBLE_HORIZONTAL_DASH);
					}
					if (this.Isidora.Audio.currentAudioPhase != IsidoraBehaviour.ISIDORA_PHASES.BRIDGE)
					{
						if (this.Isidora.Audio.GetTimeUntilNextAttackAnticipationPeriod() > 0.3f && this.Isidora.Audio.GetTimeUntilNextAttackAnticipationPeriod() < 1.2f)
						{
							this.QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS.HOMING_PROJECTILES_ATTACK);
						}
						else
						{
							this.QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS.SECOND_COMBO);
						}
						this.QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS.FIRST_COMBO);
					}
				}
			}
		}

		private void QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS atk)
		{
			this.queuedAttacks.Add(atk);
		}

		private IsidoraBehaviour.ISIDORA_ATTACKS PopAttackFromQueue()
		{
			IsidoraBehaviour.ISIDORA_ATTACKS result = IsidoraBehaviour.ISIDORA_ATTACKS.ATTACK_TEST;
			int num = this.queuedAttacks.Count - 1;
			if (num >= 0)
			{
				result = this.queuedAttacks[num];
				this.queuedAttacks.RemoveAt(num);
			}
			return result;
		}

		private bool SwapFightParametersIfNeeded()
		{
			bool result = false;
			float hpPercentage = this.Isidora.GetHpPercentage();
			this.availableAttacks = this.attackConfigData.GetAttackIds(true, true, hpPercentage);
			for (int i = 0; i < this.allFightParameters.Count; i++)
			{
				if (this.allFightParameters[i].hpPercentageBeforeApply < this.currentFightParameters.hpPercentageBeforeApply && this.allFightParameters[i].hpPercentageBeforeApply > hpPercentage && !this.currentFightParameters.Equals(this.allFightParameters[i]))
				{
					this.currentFightParameters = this.allFightParameters[i];
					result = true;
					break;
				}
			}
			return result;
		}

		private Vector2 GetDirToPenitent()
		{
			return Core.Logic.Penitent.transform.position - base.transform.position;
		}

		private float GetDirFromOrientation()
		{
			return (this.Isidora.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
		}

		private void LookAtDirUsingOrientation(Vector2 v)
		{
			this.Isidora.SetOrientation((v.x <= 0f) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
		}

		public void LookAtTarget()
		{
			this.LookAtDirUsingOrientation(this.GetDirToPenitent());
		}

		public bool IsIsidoraOnTheRightSide()
		{
			return this.Isidora.transform.position.x > this.battleBounds.center.x;
		}

		public Vector2 ArenaGetBotRightCorner()
		{
			return new Vector2(this.battleBounds.xMax, this.battleBounds.yMin);
		}

		public Vector2 ArenaGetBotLeftCorner()
		{
			return new Vector2(this.battleBounds.xMin, this.battleBounds.yMin);
		}

		public Vector2 ArenaGetBotFarRandomPoint()
		{
			Vector2 zero = Vector2.zero;
			zero.y = this.battleBounds.yMin;
			if (this.IsIsidoraOnTheRightSide())
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.1f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.4f));
			}
			else
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.6f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.9f));
			}
			return zero;
		}

		public Vector2 ArenaGetBotNearRandomPoint()
		{
			Vector2 zero = Vector2.zero;
			zero.y = this.battleBounds.yMin;
			if (this.IsIsidoraOnTheRightSide())
			{
				zero.x = UnityEngine.Random.Range(this.battleBounds.center.x, this.battleBounds.xMax);
			}
			else
			{
				zero.x = UnityEngine.Random.Range(this.battleBounds.xMin, this.battleBounds.center.x);
			}
			return zero;
		}

		public IEnumerator DelayedVoiceActivation(IsidoraAudio audio, bool active, float delay)
		{
			yield return new WaitForSeconds(delay);
			audio.SetIsidoraVoice(active);
			yield break;
		}

		public void SetWeapon(IsidoraBehaviour.ISIDORA_WEAPONS weapon)
		{
			if (this.currentMeleeAttack)
			{
				this.OnMeleeAttackFinished();
			}
			switch (weapon)
			{
			case IsidoraBehaviour.ISIDORA_WEAPONS.SLASH:
				this.currentMeleeAttack = this.slashAttack;
				break;
			case IsidoraBehaviour.ISIDORA_WEAPONS.PRE_RISING_SLASH:
				this.currentMeleeAttack = this.preRisingAttack;
				break;
			case IsidoraBehaviour.ISIDORA_WEAPONS.HOLD_RISING_SLASH:
				this.currentMeleeAttack = this.holdRisingAttack;
				break;
			case IsidoraBehaviour.ISIDORA_WEAPONS.RISING_SLASH:
				this.currentMeleeAttack = this.risingAttack;
				break;
			case IsidoraBehaviour.ISIDORA_WEAPONS.PRE_SLASH:
				this.currentMeleeAttack = this.preSlashAttack;
				break;
			case IsidoraBehaviour.ISIDORA_WEAPONS.TWIRL:
				this.currentMeleeAttack = this.twirlAttack;
				break;
			case IsidoraBehaviour.ISIDORA_WEAPONS.FADE_SLASH:
				this.currentMeleeAttack = this.fadeSlashAttack;
				break;
			}
			IsidoraMeleeAttack isidoraMeleeAttack = this.currentMeleeAttack;
			isidoraMeleeAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(isidoraMeleeAttack.OnAttackGuarded, new Core.SimpleEvent(this.Isidora.Audio.StopMeleeAudios));
			IsidoraMeleeAttack isidoraMeleeAttack2 = this.currentMeleeAttack;
			isidoraMeleeAttack2.OnAttackGuarded = (Core.SimpleEvent)Delegate.Combine(isidoraMeleeAttack2.OnAttackGuarded, new Core.SimpleEvent(this.Isidora.Audio.StopMeleeAudios));
		}

		public void FlipCurrentWeaponCollider()
		{
			Vector3 localScale = this.currentMeleeAttack.transform.localScale;
			localScale.x *= -1f;
			this.currentMeleeAttack.transform.localScale = localScale;
		}

		public void SpawnOrb()
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.orbCollectible, base.transform.position + Vector3.down * 0.7f, base.transform.rotation);
			SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
			componentInChildren.flipX = this.Isidora.SpriteRenderer.flipX;
			this.orbSpawned = true;
		}

		private void Update()
		{
			this._fsm.DoUpdate();
		}

		private void LaunchAutomaticAction()
		{
			List<IsidoraBehaviour.ISIDORA_ATTACKS> filteredAttacks = this.GetFilteredAttacks(this.availableAttacks);
			IsidoraBehaviour.ISIDORA_ATTACKS action;
			if (this.queuedAttacks.Count > 0)
			{
				action = this.PopAttackFromQueue();
			}
			else if (filteredAttacks.Count > 0)
			{
				int index = this.RandomizeUsingWeights(filteredAttacks);
				action = filteredAttacks[index];
			}
			else
			{
				action = IsidoraBehaviour.ISIDORA_ATTACKS.THIRD_COMBO;
			}
			this.LaunchAction(action);
			this.secondLastAttack = this.lastAttack;
			this.lastAttack = action;
		}

		private List<IsidoraBehaviour.ISIDORA_ATTACKS> GetFilteredAttacks(List<IsidoraBehaviour.ISIDORA_ATTACKS> originalList)
		{
			List<IsidoraBehaviour.ISIDORA_ATTACKS> list = new List<IsidoraBehaviour.ISIDORA_ATTACKS>(originalList);
			IsidoraScriptableConfig.IsidoraAttackConfig atkConfig = this.attackConfigData.GetAttackConfig(this.lastAttack);
			if (atkConfig.cantBeFollowedBy != null && atkConfig.cantBeFollowedBy.Count > 0)
			{
				list.RemoveAll((IsidoraBehaviour.ISIDORA_ATTACKS x) => atkConfig.cantBeFollowedBy.Contains(x));
			}
			if (atkConfig.alwaysFollowedBy != null && atkConfig.alwaysFollowedBy.Count > 0)
			{
				list.RemoveAll((IsidoraBehaviour.ISIDORA_ATTACKS x) => !atkConfig.alwaysFollowedBy.Contains(x));
			}
			if (this.bonfireMaskIsEnlarged)
			{
				list.Remove(IsidoraBehaviour.ISIDORA_ATTACKS.BONFIRE_PROJECTILES_ATTACK);
				list.Remove(IsidoraBehaviour.ISIDORA_ATTACKS.INVISIBLE_HORIZONTAL_DASH);
				list.Remove(IsidoraBehaviour.ISIDORA_ATTACKS.BLASTS_ATTACK);
				list.Remove(IsidoraBehaviour.ISIDORA_ATTACKS.SYNC_COMBO_1);
				list.Remove(IsidoraBehaviour.ISIDORA_ATTACKS.HOMING_PROJECTILES_ATTACK);
			}
			else
			{
				list.Remove(IsidoraBehaviour.ISIDORA_ATTACKS.CHARGED_BLASTS_ATTACK);
			}
			if (this.currentPhase != IsidoraBehaviour.ISIDORA_PHASES.FIRST)
			{
				int lastBar = this.Isidora.Audio.bossAudioSync.LastBar;
				int lastAttackMarkerBar = this.Isidora.Audio.lastAttackMarkerBar;
				if (lastBar != lastAttackMarkerBar && lastAttackMarkerBar != lastBar - 1)
				{
					list.Remove(IsidoraBehaviour.ISIDORA_ATTACKS.HOMING_PROJECTILES_ATTACK);
					list.Remove(IsidoraBehaviour.ISIDORA_ATTACKS.CHARGED_BLASTS_ATTACK);
				}
			}
			if (list.Count > 2)
			{
				list.Remove(this.secondLastAttack);
			}
			if (list.Count > 1)
			{
				list.Remove(this.lastAttack);
			}
			return list;
		}

		private int RandomizeUsingWeights(List<IsidoraBehaviour.ISIDORA_ATTACKS> filteredAtks)
		{
			float hpPercentage = this.Isidora.GetHpPercentage();
			List<float> filteredAttacksWeights = this.attackConfigData.GetFilteredAttacksWeights(filteredAtks, true, hpPercentage);
			float max = filteredAttacksWeights.Sum();
			float num = UnityEngine.Random.Range(0f, max);
			float num2 = 0f;
			for (int i = 0; i < filteredAtks.Count; i++)
			{
				num2 += filteredAttacksWeights[i];
				if (num2 > num)
				{
					return i;
				}
			}
			return 0;
		}

		private void StopCurrentAction()
		{
			if (this.currentAction != null)
			{
				this.currentAction.StopAction();
			}
		}

		protected void LaunchAction(IsidoraBehaviour.ISIDORA_ATTACKS action)
		{
			this.StopCurrentAction();
			this._fsm.ChangeState(this.stAction);
			this.currentAction = this.actionsDictionary[action]();
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped += this.CurrentAction_OnActionStops;
		}

		protected EnemyAction LaunchAction_AudioTest()
		{
			return this.audioSync_EA.StartAction(this);
		}

		public void StartIntro()
		{
			this.StopCurrentAction();
			this._fsm.ChangeState(this.stAction);
			this.currentAction = this.intro_EA.StartAction(this);
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped += this.CurrentAction_OnActionStops;
		}

		protected EnemyAction LaunchAction_Death()
		{
			return this.death_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_AttackTest()
		{
			return this.attackSync_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_FirstCombo()
		{
			return this.firstCombo_EA.StartAction(this, 6f, 6f, 0.6f, 0.75f, 0.75f, 0.4f, 4f, 1.4f, 3f, 1);
		}

		protected EnemyAction LaunchAction_SecondCombo()
		{
			return this.secondCombo_EA.StartAction(this, 6f, 6f, 0.6f, 0.3f, 0.75f, 1.25f, 0.4f, 2.5f);
		}

		protected EnemyAction LaunchAction_ThirdCombo()
		{
			return this.thirdCombo_EA.StartAction(this, 6f, 6f, 0.6f, 0.75f, 0.75f, 0.4f, 3f, 0.8f, 2.5f, 1);
		}

		protected EnemyAction LaunchAction_SyncCombo1()
		{
			return this.syncCombo1_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_FadeSlashCombo1()
		{
			return this.fadeSlashCombo1_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_PhaseSwitch()
		{
			return this.phaseSwitch_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_HorizontalDash()
		{
			float anticipationBeforeDash = this.attackConfigData.horDashConfig.anticipationBeforeDash;
			float dashDuration = this.attackConfigData.horDashConfig.dashDuration;
			float shoryukenDuration = this.attackConfigData.horDashConfig.shoryukenDuration;
			float floatingDownDuration = this.attackConfigData.horDashConfig.floatingDownDuration;
			return this.horizontalDash_EA.StartAction(this, anticipationBeforeDash, dashDuration, shoryukenDuration, floatingDownDuration);
		}

		protected EnemyAction LaunchAction_InvisibleHorizontalDash()
		{
			float anticipationBeforeDash = this.attackConfigData.invisibleHorDashConfig.anticipationBeforeDash;
			float dashDuration = this.attackConfigData.invisibleHorDashConfig.dashDuration;
			float shoryukenDuration = this.attackConfigData.invisibleHorDashConfig.shoryukenDuration;
			float floatingDownDuration = this.attackConfigData.invisibleHorDashConfig.floatingDownDuration;
			return this.invisibleHorizontalDash_EA.StartAction(this, anticipationBeforeDash, dashDuration, shoryukenDuration, floatingDownDuration);
		}

		protected EnemyAction LaunchAction_BlastsAttack()
		{
			Vector2 startingPos = (!this.IsIsidoraOnTheRightSide()) ? this.ArenaGetBotLeftCorner() : this.ArenaGetBotRightCorner();
			Vector2 direction = (!this.IsIsidoraOnTheRightSide()) ? Vector2.right : Vector2.left;
			float waitTime = Mathf.Clamp(this.Isidora.Audio.GetTimeLeftForCurrentBar(), 2.5f, this.Isidora.Audio.GetSingleBarDuration());
			int attackRepetitions = this.attackConfigData.GetAttackRepetitions(IsidoraBehaviour.ISIDORA_ATTACKS.BLASTS_ATTACK, true, this.Isidora.GetHpPercentage());
			float distanceBetweenAreas = Mathf.Lerp(4f, 3f, Mathf.Clamp01((float)(attackRepetitions - 5)));
			return this.blastsAttack_EA.StartAction(this, startingPos, direction, waitTime, true, attackRepetitions, distanceBetweenAreas);
		}

		protected EnemyAction LaunchAction_ChargedBlastsAttack()
		{
			return this.chargedBlastsAttack_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_HomingProjectiles()
		{
			return this.homingProjectiles_EA.StartAction(this, 1f, 3f);
		}

		protected EnemyAction LaunchAction_BonfireProjectiles()
		{
			return this.bonfireProjectiles_EA.StartAction(this, this.Isidora.Audio.GetSingleBarDuration() / 4f, 2, false, Vector2.zero, 1f, 1f, 2f);
		}

		protected EnemyAction LaunchAction_BonfireInfiniteProjectiles()
		{
			Vector2 castingPosition = this.homingBonfireBehavior.gameObject.transform.position;
			return this.bonfireProjectiles_EA.StartAction(this, this.Isidora.Audio.GetSingleBarDuration(), 1, true, castingPosition, 0f, 0f, -1f);
		}

		private void CurrentAction_OnActionStops(EnemyAction e)
		{
		}

		private void CurrentAction_OnActionEnds(EnemyAction e)
		{
			e.OnActionEnds -= this.CurrentAction_OnActionEnds;
			e.OnActionIsStopped -= this.CurrentAction_OnActionStops;
			if (e == this.intro_EA)
			{
				this.QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS.INVISIBLE_HORIZONTAL_DASH);
				this.LaunchAutomaticAction();
				return;
			}
			if (e != this.waitBetweenActions_EA)
			{
				if (this.currentFightParameters.waitsInVanish)
				{
					this.Isidora.AnimatorInyector.SetTwirl(false);
					this.Isidora.AnimatorInyector.SetHidden(true);
					this.extraRecoverySeconds += this.Isidora.AnimatorInyector.GetVanishAnimationDuration() + 0.2f;
					if (this.IsAnimatorInState("SLASH") || this.IsAnimatorInState("CASTING") || this.IsAnimatorInState("TWIRL") || this.IsAnimatorInState("OUT"))
					{
						this.extraRecoverySeconds += 0.5f;
					}
				}
				else
				{
					this.LookAtTarget();
				}
				this.WaitBetweenActions();
			}
			else
			{
				if (this.CheckToAdvancePhase())
				{
					this.currentPhase = IsidoraBehaviour.ISIDORA_PHASES.SECOND;
					this.QueueAttack(IsidoraBehaviour.ISIDORA_ATTACKS.PHASE_SWITCH_ATTACK);
				}
				this.LaunchAutomaticAction();
			}
		}

		private bool CheckToAdvancePhase()
		{
			return this.currentPhase == IsidoraBehaviour.ISIDORA_PHASES.BRIDGE && this.Isidora.Audio.currentAudioPhase == IsidoraBehaviour.ISIDORA_PHASES.BRIDGE && !this.queuedAttacks.Contains(IsidoraBehaviour.ISIDORA_ATTACKS.PHASE_SWITCH_ATTACK);
		}

		private void WaitBetweenActions()
		{
			this._fsm.ChangeState(this.stIdle);
			this.StartWait(this.extraRecoverySeconds + this.currentFightParameters.minMaxWaitingTimeBetweenActions.x, this.extraRecoverySeconds + this.currentFightParameters.minMaxWaitingTimeBetweenActions.y);
			this.extraRecoverySeconds = 0f;
		}

		private void StartWait(float min, float max)
		{
			this.StopCurrentAction();
			this.currentAction = this.waitBetweenActions_EA.StartAction(this, min, max);
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
		}

		public void LinearScreenshake()
		{
			Vector2 a = this.GetDirFromOrientation() * Vector2.right;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.2f, a * 1.5f, 10, 0.2f, 0.01f, default(Vector3), 0.01f, true);
		}

		public void BlastScreenshake()
		{
			Vector2 down = Vector2.down;
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.2f, down * 1.5f, 12, 0.2f, 0.01f, default(Vector3), 0.01f, true);
		}

		public void VoiceShockwave()
		{
			base.StartCoroutine(this.SingShockwave());
		}

		private IEnumerator SingShockwave()
		{
			yield return new WaitForSeconds(0.4f);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position + Vector3.up * 2.15f, 0.3f, 0.1f, 0.4f);
			yield return new WaitForSeconds(0.4f);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position + Vector3.up * 2.15f, 0.3f, 0.1f, 0.7f);
			yield return new WaitForSeconds(0.4f);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position + Vector3.up * 2.15f, 0.7f, 0.1f, 0.9f);
			yield break;
		}

		public void PlayFlameParticles()
		{
			if (this.flameParticles.isPlaying)
			{
				this.flameParticles.emission.rateOverTime = (float)(10 * this.numberOfCharges);
				this.sparksParticles.emission.rateOverTime = (float)(5 * this.numberOfCharges);
			}
			else
			{
				this.flameParticles.Play();
				this.sparksParticles.Play();
			}
		}

		public void StopFlameParticles()
		{
			this.flameParticles.Stop();
			this.sparksParticles.Stop();
		}

		private void CheckDebugActions()
		{
			Dictionary<KeyCode, IsidoraBehaviour.ISIDORA_ATTACKS> debugActions = this.attackConfigData.debugActions;
			if (debugActions != null)
			{
				foreach (KeyCode key in debugActions.Keys)
				{
					if (Input.GetKeyDown(key))
					{
						this.QueueAttack(debugActions[key]);
					}
				}
			}
		}

		public bool IsAnimatorInState(string state)
		{
			return this.Isidora.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName(state);
		}

		private Vector2 ClampInsideBoundaries(Vector2 point, bool clampX = true, bool clampY = false)
		{
			if (clampX)
			{
				point.x = Mathf.Clamp(point.x, this.battleBounds.xMin, this.battleBounds.xMax);
			}
			if (clampY)
			{
				point.y = Mathf.Clamp(point.y, this.battleBounds.yMin, this.battleBounds.yMax);
			}
			return point;
		}

		private void SlashLineVFX()
		{
			Vector2 v = base.transform.position;
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.slashLineSimpleVFX, v, Quaternion.identity, false, 1).GameObject;
			gameObject.transform.localScale = new Vector3(this.GetDirFromOrientation(), 1f, 1f);
		}

		private void SingleSparkVFX(float yOffset = 0f)
		{
			Vector2 v = new Vector2(base.transform.position.x + this.singleSparkOffset.x * this.GetDirFromOrientation(), base.transform.position.y + this.singleSparkOffset.y + yOffset);
			PoolManager.Instance.ReuseObject(this.singleSparkSimpleVFX, v, Quaternion.identity, false, 1);
		}

		private void WarningVFX(Vector2 offset)
		{
			Vector2 v = base.transform.position + offset;
			PoolManager.Instance.ReuseObject(this.attackAnticipationWarningSimpleVFX, v, Quaternion.identity, false, 1);
		}

		public void OnMeleeAttackStarts()
		{
			this.currentMeleeAttack.dealsDamage = true;
			this.currentMeleeAttack.CurrentWeaponAttack();
		}

		public void OnMeleeAttackFinished()
		{
			if (this.currentMeleeAttack)
			{
				this.currentMeleeAttack.dealsDamage = false;
			}
		}

		public void DoActivateCollisions(bool b)
		{
			this.Isidora.DamageArea.DamageAreaCollider.enabled = b;
		}

		public override void Damage()
		{
			throw new NotImplementedException();
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

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public void Death()
		{
			PlayMakerFSM.BroadcastEvent("BOSS DEAD");
			this.StopCurrentAction();
			base.StopAllCoroutines();
			base.transform.DOKill(false);
			this.ClearAll();
			this.LaunchAction_Death();
		}

		private void ClearAll()
		{
			GameplayUtils.DestroyAllProjectiles();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(this.battleBounds.center, new Vector3(this.battleBounds.width, this.battleBounds.height, 0f));
			Gizmos.DrawWireCube(this.ArenaGetBotLeftCorner(), Vector3.one * 0.1f);
			Gizmos.DrawWireCube(this.ArenaGetBotRightCorner(), Vector3.one * 0.1f);
		}

		[FoldoutGroup("Battle area", 0)]
		public Rect battleBounds;

		[FoldoutGroup("Battle config", 0)]
		public List<IsidoraBehaviour.IsidoraFightParameters> allFightParameters;

		[FoldoutGroup("Attacks config", 0)]
		public IsidoraScriptableConfig attackConfigData;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public IsidoraMeleeAttack preSlashAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public IsidoraMeleeAttack slashAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public IsidoraMeleeAttack preRisingAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public IsidoraMeleeAttack holdRisingAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public IsidoraMeleeAttack risingAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public IsidoraMeleeAttack twirlAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public IsidoraMeleeAttack fadeSlashAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public HomingBonfireAttack homingBonfireAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public BossAreaSummonAttack blastAttack;

		[FoldoutGroup("Death references", 0)]
		[SerializeField]
		public GameObject orbCollectible;

		[HideInInspector]
		public HomingBonfireBehaviour homingBonfireBehavior;

		[FoldoutGroup("VFX", 0)]
		public ParticleSystem floorSparksParticlesToRight;

		[FoldoutGroup("VFX", 0)]
		public ParticleSystem floorSparksParticlesToLeft;

		[FoldoutGroup("VFX", 0)]
		public ParticleSystem flameParticles;

		[FoldoutGroup("VFX", 0)]
		public ParticleSystem sparksParticles;

		[FoldoutGroup("VFX", 0)]
		public GameObject floorSparksMaskToRight;

		[FoldoutGroup("VFX", 0)]
		public GameObject floorSparksMaskToLeft;

		[FoldoutGroup("VFX", 0)]
		public GameObject singleSparkSimpleVFX;

		[FoldoutGroup("VFX", 0)]
		public GameObject attackAnticipationWarningSimpleVFX;

		[FoldoutGroup("VFX", 0)]
		public Vector2 singleSparkOffset;

		[FoldoutGroup("VFX", 0)]
		public GameObject slashLineSimpleVFX;

		[FoldoutGroup("VFX", 0)]
		public SpriteRenderer absorbSpriteRenderer;

		[FoldoutGroup("VFX", 0)]
		public GhostTrailGenerator ghostTrail;

		private List<IsidoraBehaviour.ISIDORA_ATTACKS> availableAttacks = new List<IsidoraBehaviour.ISIDORA_ATTACKS>();

		[ShowInInspector]
		private List<IsidoraBehaviour.ISIDORA_ATTACKS> queuedAttacks = new List<IsidoraBehaviour.ISIDORA_ATTACKS>();

		private IsidoraBehaviour.IsidoraFightParameters currentFightParameters;

		private EnemyAction currentAction;

		private IsidoraBehaviour.ISIDORA_ATTACKS lastAttack = IsidoraBehaviour.ISIDORA_ATTACKS.DUMMY;

		private IsidoraBehaviour.ISIDORA_ATTACKS secondLastAttack = IsidoraBehaviour.ISIDORA_ATTACKS.DUMMY;

		private Dictionary<IsidoraBehaviour.ISIDORA_ATTACKS, Func<EnemyAction>> actionsDictionary = new Dictionary<IsidoraBehaviour.ISIDORA_ATTACKS, Func<EnemyAction>>();

		private float extraRecoverySeconds;

		private IsidoraMeleeAttack currentMeleeAttack;

		private bool bonfireMaskIsEnlarged;

		private bool orbSpawned;

		private Coroutine blinkCoroutine;

		private bool wasVoiceOn;

		private string debugMessage;

		[HideInInspector]
		public int numberOfCharges;

		private WaitSeconds_EnemyAction waitBetweenActions_EA;

		private IsidoraBehaviour.AudioSyncTest_EnemyAction audioSync_EA;

		private IsidoraBehaviour.AttackSyncTest_EnemyAction attackSync_EA;

		private IsidoraBehaviour.FirstCombo_EnemyAction firstCombo_EA;

		private IsidoraBehaviour.SecondCombo_EnemyAction secondCombo_EA;

		private IsidoraBehaviour.ThirdCombo_EnemyAction thirdCombo_EA;

		private IsidoraBehaviour.SyncCombo1_EnemyAction syncCombo1_EA;

		private IsidoraBehaviour.FadeSlashCombo1_EnemyAction fadeSlashCombo1_EA;

		private IsidoraBehaviour.PhaseSwitchAttack_EnemyAction phaseSwitch_EA;

		private IsidoraBehaviour.HomingProjectilesAttack_EnemyAction homingProjectiles_EA;

		private IsidoraBehaviour.BonfireProjectilesAttack_EnemyAction bonfireProjectiles_EA;

		private IsidoraBehaviour.HorizontalDash_EnemyAction horizontalDash_EA;

		private IsidoraBehaviour.BlastsAttack_EnemyAction blastsAttack_EA;

		private IsidoraBehaviour.InvisibleHorizontalDash_EnemyAction invisibleHorizontalDash_EA;

		private IsidoraBehaviour.ChargedBlastsAttack_EnemyAction chargedBlastsAttack_EA;

		private IsidoraBehaviour.Intro_EnemyAction intro_EA;

		private IsidoraBehaviour.Death_EnemyAction death_EA;

		private StateMachine<IsidoraBehaviour> _fsm;

		private State<IsidoraBehaviour> stIdle;

		private State<IsidoraBehaviour> stAction;

		public IsidoraBehaviour.ISIDORA_PHASES currentPhase;

		[Serializable]
		public struct IsidoraFightParameters
		{
			[ProgressBar(0.0, 1.0, 0.8f, 0f, 0.1f)]
			[SuffixLabel("%", false)]
			public float hpPercentageBeforeApply;

			[MinMaxSlider(0f, 5f, true)]
			public Vector2 minMaxWaitingTimeBetweenActions;

			[SuffixLabel("hits", true)]
			public int maxHitsInHurt;

			[InfoBox("If the boss phase should change after reaching this", InfoMessageType.Info, null)]
			public bool advancePhase;

			[InfoBox("If the boss should wait between actions in a vanished state", InfoMessageType.Info, null)]
			public bool waitsInVanish;
		}

		public enum ISIDORA_PHASES
		{
			FIRST,
			BRIDGE,
			SECOND
		}

		public enum ISIDORA_ATTACKS
		{
			AUDIO_TEST,
			ATTACK_TEST,
			FIRST_COMBO,
			SECOND_COMBO,
			PHASE_SWITCH_ATTACK,
			HOMING_PROJECTILES_ATTACK,
			BONFIRE_PROJECTILES_ATTACK,
			BONFIRE_INFINITE_PROJECTILES_ATTACK,
			HORIZONTAL_DASH,
			BLASTS_ATTACK,
			THIRD_COMBO,
			SYNC_COMBO_1,
			INVISIBLE_HORIZONTAL_DASH,
			CHARGED_BLASTS_ATTACK,
			DUMMY = 999
		}

		public enum ISIDORA_SLASHES
		{
			SLASH,
			RISING_SLASH
		}

		public enum ISIDORA_WEAPONS
		{
			PRE_SLASH = 4,
			SLASH = 0,
			PRE_RISING_SLASH,
			HOLD_RISING_SLASH,
			RISING_SLASH,
			TWIRL = 5,
			FADE_SLASH
		}

		public class Intro_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				(this.owner as IsidoraBehaviour).Isidora.AnimatorInyector.ResetAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				o.Isidora.DamageEffect.StartColorizeLerp(0f, 0.2f, 1f, null);
				this.ACT_WAIT.StartAction(o, 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.homingBonfireBehavior.IsSpawningIsidora = false;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class Death_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				(this.owner as IsidoraBehaviour).Isidora.AnimatorInyector.ResetAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				p.Status.Invulnerable = true;
				o.homingBonfireBehavior.BonfireAttack.ClearAll();
				o.homingBonfireBehavior.DeactivateBonfire(true, true);
				o.blastAttack.ClearAll();
				if (o.blinkCoroutine != null)
				{
					o.StopCoroutine(o.blinkCoroutine);
				}
				o.absorbSpriteRenderer.enabled = false;
				o.Isidora.AnimatorInyector.PlayDeath();
				Vector3 targetPos = o.transform.position;
				targetPos.y = o.battleBounds.yMin + 0.2f;
				float moveTime = Vector2.Distance(targetPos, o.transform.position) * 0.2f + 0.2f;
				this.ACT_MOVE.StartAction(o, targetPos, moveTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				o.homingBonfireBehavior.enabled = false;
				MasterShaderEffects effects = Core.Logic.Penitent.GetComponentInChildren<MasterShaderEffects>();
				if (effects != null)
				{
					effects.StartColorizeLerp(0f, 0.5f, 4f, null);
				}
				o.Isidora.DamageEffect.StartColorizeLerp(0f, 0.2f, 4f, null);
				this.ACT_WAIT.StartAction(o, 4f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Isidora.AnimatorInyector.StopDeath();
				yield return new WaitUntil(() => o.orbSpawned);
				p.Status.Invulnerable = false;
				base.FinishAction();
				UnityEngine.Object.Destroy(o.gameObject);
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class AudioSyncTest_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.Isidora.Audio.SetIsidoraVoice(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour isidora = this.owner as IsidoraBehaviour;
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(isidora.Isidora.Audio);
				Debug.Log(string.Format("<color=blue>Bar finished: Anticipation starts</color>", new object[0]));
				isidora.Isidora.Audio.SetIsidoraVoice(true);
				yield return null;
				isidora.Isidora.Audio.SetIsidoraVoice(false);
				isidora.transform.DOMoveX(isidora.transform.position.x + 3f, isidora.Isidora.Audio.GetSingleBarDuration() * 0.5f, false);
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(isidora.Isidora.Audio);
				Debug.Log(string.Format("<color=blue>Bar finished: Attack starts!</color>", new object[0]));
				isidora.transform.DOMoveX(isidora.transform.position.x - 6f, 0.2f, false);
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(isidora.Isidora.Audio);
				base.FinishAction();
				yield break;
			}
		}

		public class AttackSyncTest_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.Isidora.Audio.SetIsidoraVoice(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				float dashSlashTime = 0.99f;
				IsidoraBehaviour isidora = this.owner as IsidoraBehaviour;
				IsidoraAnimatorInyector animatorInyector = isidora.Isidora.AnimatorInyector;
				isidora.LookAtDirUsingOrientation(Vector2.left);
				Vector2 targetPoint = isidora.ArenaGetBotRightCorner();
				Tweener tween = isidora.transform.DOMove(targetPoint, isidora.Isidora.Audio.GetTimeUntilNextAttackAnticipationPeriod() - 0.1f, false);
				yield return tween.WaitForCompletion();
				isidora.Isidora.Audio.SetIsidoraVoice(true);
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(isidora.Isidora.Audio);
				this.ACT_WAIT.StartAction(isidora, 0.01f);
				yield return this.ACT_WAIT.waitForCompletion;
				isidora.Isidora.Audio.SetIsidoraVoice(false);
				Debug.Log(string.Format("<color=blue>Bar finished: Anticipation starts!</color>", new object[0]));
				animatorInyector.PlaySlashAttack();
				animatorInyector.SetAttackAnticipation(true);
				float remainingTime = isidora.Isidora.Audio.GetTimeLeftForCurrentBar();
				this.ACT_WAIT.StartAction(isidora, remainingTime * 0.55f);
				yield return this.ACT_WAIT.waitForCompletion;
				isidora.transform.DOMoveX(isidora.transform.position.x + 1f, remainingTime * 0.3f, false).SetEase(Ease.InOutCubic);
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(isidora.Isidora.Audio);
				Debug.Log(string.Format("<color=blue>Bar finished: Attack 1 starts!</color>", new object[0]));
				animatorInyector.SetTwirl(true);
				animatorInyector.SetAttackAnticipation(false);
				targetPoint = isidora.ArenaGetBotLeftCorner();
				Tweener t = isidora.transform.DOMove(targetPoint, dashSlashTime, false).SetEase(Ease.InOutCubic);
				yield return t.WaitForCompletion();
				this.ACT_WAIT.StartAction(isidora, 0.33f);
				yield return this.ACT_WAIT.waitForCompletion;
				animatorInyector.SetTwirl(false);
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(isidora.Isidora.Audio);
				animatorInyector.PlaySlashAttack();
				animatorInyector.SetAttackAnticipation(true);
				isidora.LookAtDirUsingOrientation(Vector2.right);
				Debug.Log(string.Format("<color=blue>Bar finished: Dance starts!</color>", new object[0]));
				remainingTime = isidora.Isidora.Audio.GetTimeLeftForCurrentBar();
				Sequence s = DOTween.Sequence();
				s.Append(isidora.transform.DOMoveY(isidora.transform.position.y + 1f, remainingTime * 0.5f, false).SetEase(Ease.InOutCubic));
				s.Append(isidora.transform.DOMoveX(isidora.transform.position.x - 1f, remainingTime * 0.33f, false).SetEase(Ease.InOutCubic));
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(isidora.Isidora.Audio);
				animatorInyector.SetAttackAnticipation(false);
				Debug.Log(string.Format("<color=blue>Bar finished: Attack 2 starts!</color>", new object[0]));
				targetPoint = isidora.ArenaGetBotRightCorner() + Vector2.up;
				isidora.transform.DOMove(targetPoint, dashSlashTime, false);
				isidora.Isidora.Audio.SetIsidoraVoice(false);
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(isidora.Isidora.Audio);
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class WaitUntilIdle : IsidoraBehaviour.WaitUntilAnimationState
		{
			public WaitUntilIdle(IsidoraBehaviour isidora) : base(isidora, "IDLE")
			{
			}
		}

		public class WaitUntilOut : IsidoraBehaviour.WaitUntilAnimationState
		{
			public WaitUntilOut(IsidoraBehaviour isidora) : base(isidora, "OUT")
			{
			}
		}

		public class WaitUntilTwirl : IsidoraBehaviour.WaitUntilAnimationState
		{
			public WaitUntilTwirl(IsidoraBehaviour isidora) : base(isidora, "TWIRL")
			{
			}
		}

		public class WaitUntilCasting : IsidoraBehaviour.WaitUntilAnimationState
		{
			public WaitUntilCasting(IsidoraBehaviour isidora) : base(isidora, "CASTING")
			{
			}
		}

		public class WaitUntilAnimationState : CustomYieldInstruction
		{
			public WaitUntilAnimationState(IsidoraBehaviour isidora, string state)
			{
				this.isidora = isidora;
				this.state = state;
			}

			public override bool keepWaiting
			{
				get
				{
					return !this.isidora.IsAnimatorInState(this.state);
				}
			}

			private IsidoraBehaviour isidora;

			private string state;
		}

		public class MeleeAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 attackDir, IsidoraBehaviour.ISIDORA_SLASHES slashType, float slashTime, bool hold, float holdTime, bool twirl, float twirlTime, bool endsWithTwirlActive, Core.SimpleEvent onAttackGuardedCallback = null)
			{
				this.attackDir = attackDir;
				this.slashType = slashType;
				this.slashTime = slashTime;
				this.hold = hold;
				this.holdTime = holdTime;
				this.twirl = twirl;
				this.twirlTime = twirlTime;
				this.endsWithTwirlActive = endsWithTwirlActive;
				this.onAttackGuardedCallback = onAttackGuardedCallback;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				if (this.onAttackGuardedCallback != null)
				{
					IsidoraMeleeAttack preSlashAttack = isidoraBehaviour.preSlashAttack;
					preSlashAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(preSlashAttack.OnAttackGuarded, this.onAttackGuardedCallback);
					IsidoraMeleeAttack slashAttack = isidoraBehaviour.slashAttack;
					slashAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(slashAttack.OnAttackGuarded, this.onAttackGuardedCallback);
					IsidoraMeleeAttack risingAttack = isidoraBehaviour.risingAttack;
					risingAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(risingAttack.OnAttackGuarded, this.onAttackGuardedCallback);
				}
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				o.LookAtDirUsingOrientation(this.attackDir);
				o.Isidora.AnimatorInyector.SetAttackAnticipation(this.hold);
				o.Isidora.AnimatorInyector.SetHidden(false);
				IsidoraBehaviour.ISIDORA_SLASHES isidora_SLASHES = this.slashType;
				if (isidora_SLASHES != IsidoraBehaviour.ISIDORA_SLASHES.SLASH)
				{
					if (isidora_SLASHES == IsidoraBehaviour.ISIDORA_SLASHES.RISING_SLASH)
					{
						o.Isidora.AnimatorInyector.PlayRisingSlash();
						if (this.onAttackGuardedCallback != null)
						{
							IsidoraMeleeAttack risingAttack = o.risingAttack;
							risingAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Combine(risingAttack.OnAttackGuarded, this.onAttackGuardedCallback);
						}
					}
				}
				else
				{
					o.Isidora.AnimatorInyector.PlaySlashAttack();
					if (this.onAttackGuardedCallback != null)
					{
						IsidoraMeleeAttack preSlashAttack = o.preSlashAttack;
						preSlashAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Combine(preSlashAttack.OnAttackGuarded, this.onAttackGuardedCallback);
						IsidoraMeleeAttack slashAttack = o.slashAttack;
						slashAttack.OnAttackGuarded = (Core.SimpleEvent)Delegate.Combine(slashAttack.OnAttackGuarded, this.onAttackGuardedCallback);
					}
				}
				if (this.hold)
				{
					this.ACT_WAIT.StartAction(o, this.holdTime);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				o.Isidora.AnimatorInyector.SetTwirl(this.twirl);
				o.Isidora.AnimatorInyector.SetAttackAnticipation(false);
				this.ACT_WAIT.StartAction(o, this.slashTime);
				yield return this.ACT_WAIT.waitForCompletion;
				if (this.twirl)
				{
					this.ACT_WAIT.StartAction(o, this.twirlTime);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				if (!this.endsWithTwirlActive)
				{
					o.Isidora.AnimatorInyector.SetTwirl(false);
					o.Isidora.AnimatorInyector.SetHidden(false);
					yield return new IsidoraBehaviour.WaitUntilIdle(o);
				}
				if (this.onAttackGuardedCallback != null)
				{
					IsidoraMeleeAttack preSlashAttack2 = o.preSlashAttack;
					preSlashAttack2.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(preSlashAttack2.OnAttackGuarded, this.onAttackGuardedCallback);
					IsidoraMeleeAttack slashAttack2 = o.slashAttack;
					slashAttack2.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(slashAttack2.OnAttackGuarded, this.onAttackGuardedCallback);
					IsidoraMeleeAttack risingAttack2 = o.risingAttack;
					risingAttack2.OnAttackGuarded = (Core.SimpleEvent)Delegate.Remove(risingAttack2.OnAttackGuarded, this.onAttackGuardedCallback);
				}
				base.FinishAction();
				yield break;
			}

			private Vector2 attackDir;

			private IsidoraBehaviour.ISIDORA_SLASHES slashType;

			private float slashTime;

			private bool hold;

			private float holdTime;

			private bool twirl;

			private float twirlTime;

			private bool endsWithTwirlActive;

			private Core.SimpleEvent onAttackGuardedCallback;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class FirstCombo_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float distanceFromPenitent, float slashDistance, float slashTime, float holdTime1, float holdTime2, float twirlTime1, float twirlDistance1, float twirlTime2, float twirlDistance2, int repetitions = 1)
			{
				this.distanceFromPenitent = distanceFromPenitent;
				this.slashDistance = slashDistance;
				this.slashTime = slashTime;
				this.holdTime1 = holdTime1;
				this.holdTime2 = holdTime2;
				this.twirlTime1 = twirlTime1;
				this.twirlDistance1 = twirlDistance1;
				this.twirlTime2 = twirlTime2;
				this.twirlDistance2 = twirlDistance2;
				this.repetitions = repetitions;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MELEE.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.Isidora.Audio.SetSkullsChoir(false);
				isidoraBehaviour.transform.DOKill(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				float vOffset = 0.3f;
				if (o.IsAnimatorInState("OUT"))
				{
					o.transform.position = o.ArenaGetBotFarRandomPoint();
					o.LookAtTarget();
					o.Isidora.AnimatorInyector.SetHidden(false);
					yield return new IsidoraBehaviour.WaitUntilIdle(o);
				}
				o.Isidora.Audio.SetSkullsChoir(true);
				Vector2 dir = o.GetDirToPenitent();
				Vector2 startingPosition = o.transform.position;
				if (Mathf.Abs(dir.x) > this.distanceFromPenitent)
				{
					if (dir.x > 0f)
					{
						startingPosition.x = p.GetPosition().x - this.distanceFromPenitent;
					}
					else
					{
						startingPosition.x = p.GetPosition().x + this.distanceFromPenitent;
					}
				}
				startingPosition.y = o.battleBounds.yMin - 0.5f;
				float approachTime = (o.transform.position - startingPosition).magnitude * 0.1f + 0.2f;
				this.ACT_MOVE.StartAction(o, startingPosition, approachTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				this.ACT_MELEE.StartAction(o, dir, IsidoraBehaviour.ISIDORA_SLASHES.SLASH, this.slashTime, true, this.holdTime1, true, this.twirlTime1, true, delegate()
				{
					this.ACT_MOVE.StopAction();
				});
				dir = o.GetDirToPenitent();
				float anticipationDistance = 1.5f;
				Vector2 anticipationPoint = o.transform.position + Vector2.left * Mathf.Sign(dir.x) * anticipationDistance;
				anticipationPoint = o.ClampInsideBoundaries(anticipationPoint, true, false);
				this.ACT_MOVE.StartAction(o, anticipationPoint, this.holdTime1, Ease.OutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Vector2 afterSlashPosition = startingPosition + Vector2.right * this.slashDistance * Mathf.Sign(dir.x) + Vector2.down * vOffset;
				afterSlashPosition = o.ClampInsideBoundaries(afterSlashPosition, true, false);
				this.ACT_MOVE.StartAction(o, afterSlashPosition, this.slashTime, Ease.OutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Vector2 newDir = o.GetDirToPenitent();
				Vector2 twirlTarget = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * this.twirlDistance2 + Vector2.up * vOffset;
				twirlTarget = o.ClampInsideBoundaries(twirlTarget, true, false);
				this.ACT_MOVE.StartAction(o, twirlTarget, this.twirlTime1, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				yield return this.ACT_MELEE.waitForCompletion;
				int i = this.repetitions;
				for (int j = 0; j < i; j++)
				{
					newDir = o.GetDirToPenitent();
					float remainingTime = o.Isidora.Audio.GetTimeLeftForCurrentBar();
					if (remainingTime < this.holdTime2)
					{
						float singleBarDuration = o.Isidora.Audio.GetSingleBarDuration();
						remainingTime += singleBarDuration;
						Debug.Log("<color=magenta> ENTRANDO EN EL TWIRL DANCE </color>");
						bool isFirstLoop = true;
						Vector2 vector = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * this.twirlDistance1 * 0.8f;
						vector = o.ClampInsideBoundaries(vector, true, false);
						o.transform.DOMove(vector, singleBarDuration * 0.5f, false).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo).OnStepComplete(delegate
						{
							if (isFirstLoop)
							{
								isFirstLoop = false;
								newDir = o.GetDirToPenitent();
								o.LookAtTarget();
							}
						});
					}
					else
					{
						Vector2 vector2 = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * this.twirlDistance1;
						vector2 = o.ClampInsideBoundaries(vector2, true, false);
						o.transform.DOMove(vector2, remainingTime - this.holdTime2, false).SetEase(Ease.OutQuad);
					}
					this.ACT_WAIT.StartAction(o, remainingTime - this.holdTime2);
					yield return this.ACT_WAIT.waitForCompletion;
					bool keepTwirl = j < i - 1;
					o.ghostTrail.EnableGhostTrail = true;
					this.ACT_MELEE.StartAction(o, newDir, IsidoraBehaviour.ISIDORA_SLASHES.SLASH, this.slashTime, true, this.holdTime2, true, this.twirlTime2, keepTwirl, delegate()
					{
						this.ACT_MOVE.StopAction();
					});
					this.ACT_MOVE.StartAction(o, o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * anticipationDistance, this.holdTime2, Ease.OutQuad, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					float nSlashDistance = o.GetDirToPenitent().magnitude + 1f;
					Vector2 afterSlashPosition2 = o.transform.position + Vector2.right * Mathf.Sign(newDir.x) * nSlashDistance + Vector2.down * vOffset;
					afterSlashPosition2 = o.ClampInsideBoundaries(afterSlashPosition2, true, false);
					this.ACT_MOVE.StartAction(o, afterSlashPosition2, this.slashTime, Ease.OutQuad, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					o.ghostTrail.EnableGhostTrail = false;
					if (!keepTwirl)
					{
						this.twirlDistance2 += 2f;
						this.twirlTime2 += 1f;
					}
					Vector2 twirlTarget2 = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * this.twirlDistance2 + Vector2.up * vOffset;
					twirlTarget2 = o.ClampInsideBoundaries(twirlTarget2, true, false);
					o.Isidora.AnimatorInyector.Decelerate(this.twirlTime2 * 0.5f);
					this.ACT_MOVE.StartAction(o, twirlTarget2, this.twirlTime2, Ease.InOutQuad, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					yield return this.ACT_MELEE.waitForCompletion;
				}
				o.Isidora.Audio.SetSkullsChoir(false);
				base.FinishAction();
				yield break;
			}

			private float distanceFromPenitent;

			private float slashDistance;

			private float slashTime;

			private float holdTime1;

			private float holdTime2;

			private float twirlTime1;

			private float twirlDistance1;

			private float twirlTime2;

			private float twirlDistance2;

			private int repetitions;

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.MeleeAttack_EnemyAction ACT_MELEE = new IsidoraBehaviour.MeleeAttack_EnemyAction();
		}

		public class SecondCombo_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float distanceFromPenitent, float slashDistance, float slashTime, float risingSlashTime, float holdTime1, float holdTime2, float twirlTime1, float twirlDistance1)
			{
				this.distanceFromPenitent = distanceFromPenitent;
				this.slashDistance = slashDistance;
				this.slashTime = slashTime;
				this.risingSlashTime = risingSlashTime;
				this.holdTime1 = holdTime1;
				this.holdTime2 = holdTime2;
				this.twirlTime1 = twirlTime1;
				this.twirlDistance1 = twirlDistance1;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE2.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MELEE.StopAction();
				(this.owner as IsidoraBehaviour).Isidora.AnimatorInyector.ResetAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				float vOffset = 0.3f;
				if (o.IsAnimatorInState("OUT"))
				{
					o.transform.position = o.ArenaGetBotFarRandomPoint();
					o.LookAtTarget();
					o.Isidora.AnimatorInyector.SetHidden(false);
					yield return new IsidoraBehaviour.WaitUntilIdle(o);
				}
				o.Isidora.Audio.SetSkullsChoir(true);
				Vector2 dir = o.GetDirToPenitent();
				Vector2 startingPosition = o.transform.position;
				if (Mathf.Abs(dir.x) > this.distanceFromPenitent)
				{
					startingPosition.x = p.GetPosition().x - Mathf.Sign(dir.x) * this.distanceFromPenitent;
				}
				startingPosition.y = o.battleBounds.yMin - 0.5f;
				float approachTime = (o.transform.position - startingPosition).magnitude * 0.1f + 0.2f;
				this.ACT_MOVE.StartAction(o, startingPosition, approachTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				this.ACT_MELEE.StartAction(o, dir, IsidoraBehaviour.ISIDORA_SLASHES.SLASH, this.slashTime, true, this.holdTime1, true, this.twirlTime1, true, delegate()
				{
					this.ACT_MOVE.StopAction();
				});
				dir = o.GetDirToPenitent();
				float anticipationDistance = 1.5f;
				Vector2 anticipationPoint = o.transform.position + Vector2.left * Mathf.Sign(dir.x) * anticipationDistance;
				anticipationPoint = o.ClampInsideBoundaries(anticipationPoint, true, false);
				this.ACT_MOVE.StartAction(o, anticipationPoint, this.holdTime1, Ease.OutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Vector2 afterSlashPosition = startingPosition + Vector2.right * this.slashDistance * Mathf.Sign(dir.x) + Vector2.down * vOffset;
				afterSlashPosition = o.ClampInsideBoundaries(afterSlashPosition, true, false);
				this.ACT_MOVE.StartAction(o, afterSlashPosition, this.slashTime, Ease.OutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Vector2 newDir = o.GetDirToPenitent();
				Vector2 twirlTarget = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * this.twirlDistance1 + Vector2.up * vOffset;
				twirlTarget = o.ClampInsideBoundaries(twirlTarget, true, false);
				this.ACT_MOVE.StartAction(o, twirlTarget, this.twirlTime1, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				yield return this.ACT_MELEE.waitForCompletion;
				o.ghostTrail.EnableGhostTrail = true;
				this.ACT_MELEE.StartAction(o, newDir, IsidoraBehaviour.ISIDORA_SLASHES.RISING_SLASH, this.risingSlashTime, true, this.holdTime2, true, 1.5f, true, null);
				this.ACT_WAIT.StartAction(o, this.holdTime2 * 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector2 afterSlashPosition2 = twirlTarget;
				if (newDir.x > 0f)
				{
					afterSlashPosition2.x += this.slashDistance;
				}
				else
				{
					afterSlashPosition2.x -= this.slashDistance;
				}
				float risingSlashHeight = 4f;
				afterSlashPosition2.y += risingSlashHeight;
				afterSlashPosition2 = o.ClampInsideBoundaries(afterSlashPosition2, true, false);
				this.ACT_MOVE.StartAction(o, afterSlashPosition2, this.risingSlashTime + this.holdTime2 * 0.5f, Ease.InOutQuad, null, true, null, true, false, 1.7f);
				this.ACT_WAIT.StartAction(o, this.holdTime2 * 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_MOVE2.StartAction(o, afterSlashPosition2, this.risingSlashTime, Ease.OutQuad, null, true, null, false, true, 1.7f);
				yield return this.ACT_MOVE2.waitForCompletion;
				o.ghostTrail.EnableGhostTrail = false;
				o.Isidora.AnimatorInyector.Decelerate(this.risingSlashTime * 2.5f);
				afterSlashPosition2.y += -risingSlashHeight + 0.5f;
				this.ACT_MOVE2.StartAction(o, afterSlashPosition2, this.risingSlashTime * 5f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE2.waitForCompletion;
				o.Isidora.AnimatorInyector.SetTwirl(false);
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				yield return this.ACT_MELEE.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private float distanceFromPenitent;

			private float slashDistance;

			private float slashTime;

			private float risingSlashTime;

			private float holdTime1;

			private float holdTime2;

			private float twirlTime1;

			private float twirlDistance1;

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE2 = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.MeleeAttack_EnemyAction ACT_MELEE = new IsidoraBehaviour.MeleeAttack_EnemyAction();
		}

		public class ThirdCombo_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float distanceFromPenitent, float slashDistance, float slashTime, float holdTime1, float holdTime2, float vanishTime, float vanishDistance, float twirlTime, float twirlDistance, int repetitions = 1)
			{
				this.distanceFromPenitent = distanceFromPenitent;
				this.slashDistance = slashDistance;
				this.slashTime = slashTime;
				this.holdTime1 = holdTime1;
				this.holdTime2 = holdTime2;
				this.vanishTime = vanishTime;
				this.vanishDistance = vanishDistance;
				this.twirlTime = twirlTime;
				this.twirlDistance = twirlDistance;
				this.repetitions = repetitions;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MELEE.StopAction();
				this.ACT_TELEPORT.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.Isidora.Audio.SetSkullsChoir(false);
				isidoraBehaviour.transform.DOKill(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				float vOffset = 0.3f;
				if (o.IsAnimatorInState("OUT"))
				{
					o.transform.position = o.ArenaGetBotFarRandomPoint();
					o.LookAtTarget();
					o.Isidora.AnimatorInyector.SetHidden(false);
					yield return new IsidoraBehaviour.WaitUntilIdle(o);
				}
				o.Isidora.Audio.SetSkullsChoir(true);
				Vector2 dir = o.GetDirToPenitent();
				Vector2 startingPosition = o.transform.position;
				if (Mathf.Abs(dir.x) > this.distanceFromPenitent)
				{
					if (dir.x > 0f)
					{
						startingPosition.x = p.GetPosition().x - this.distanceFromPenitent;
					}
					else
					{
						startingPosition.x = p.GetPosition().x + this.distanceFromPenitent;
					}
				}
				startingPosition.y = o.battleBounds.yMin - 0.5f;
				float approachTime = (o.transform.position - startingPosition).magnitude * 0.1f + 0.2f;
				this.ACT_MOVE.StartAction(o, startingPosition, approachTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				this.ACT_MELEE.StartAction(o, dir, IsidoraBehaviour.ISIDORA_SLASHES.SLASH, this.slashTime, true, this.holdTime1, true, this.vanishTime, true, delegate()
				{
					this.ACT_MOVE.StopAction();
				});
				dir = o.GetDirToPenitent();
				float anticipationDistance = 1.5f;
				Vector2 anticipationPoint = o.transform.position + Vector2.left * Mathf.Sign(dir.x) * anticipationDistance;
				anticipationPoint = o.ClampInsideBoundaries(anticipationPoint, true, false);
				this.ACT_MOVE.StartAction(o, anticipationPoint, this.holdTime1, Ease.OutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Vector2 afterSlashPosition = startingPosition + Vector2.right * this.slashDistance * Mathf.Sign(dir.x) + Vector2.down * vOffset;
				afterSlashPosition = o.ClampInsideBoundaries(afterSlashPosition, true, false);
				this.ACT_MOVE.StartAction(o, afterSlashPosition, this.slashTime, Ease.OutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Vector2 vanishTarget = p.GetPosition() + Vector2.left * Mathf.Sign(dir.x) * this.vanishDistance;
				vanishTarget.y = afterSlashPosition.y + vOffset;
				vanishTarget = o.ClampInsideBoundaries(vanishTarget, true, false);
				this.ACT_TELEPORT.StartAction(o, vanishTarget, this.vanishTime, true, false, o.GetTarget());
				yield return this.ACT_TELEPORT.waitForCompletion;
				yield return this.ACT_MELEE.waitForCompletion;
				int i = this.repetitions;
				Vector2 newDir = o.GetDirToPenitent();
				for (int j = 0; j < i; j++)
				{
					newDir = o.GetDirToPenitent();
					float remainingTime = o.Isidora.Audio.GetTimeLeftForCurrentBar();
					if (remainingTime < this.holdTime2)
					{
						float singleBarDuration = o.Isidora.Audio.GetSingleBarDuration();
						remainingTime += singleBarDuration;
						bool isFirstLoop = true;
						Debug.Log("<color=magenta> ENTRANDO EN EL TWIRL DANCE </color>");
						Vector2 vector = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * this.vanishDistance * 0.8f;
						vector = o.ClampInsideBoundaries(vector, true, false);
						o.transform.DOMove(vector, singleBarDuration * 0.5f, false).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo).OnStepComplete(delegate
						{
							if (isFirstLoop)
							{
								isFirstLoop = false;
								newDir = o.GetDirToPenitent();
								o.LookAtTarget();
							}
						});
					}
					else
					{
						Vector2 vector2 = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * this.vanishDistance;
						vector2 = o.ClampInsideBoundaries(vector2, true, false);
						float duration = Mathf.Clamp(remainingTime - this.holdTime2, 0.75f, remainingTime - this.holdTime2);
						o.transform.DOMove(vector2, duration, false).SetEase(Ease.OutQuad);
					}
					this.ACT_WAIT.StartAction(o, remainingTime - this.holdTime2);
					yield return this.ACT_WAIT.waitForCompletion;
					bool keepTwirl = j < i - 1;
					o.ghostTrail.EnableGhostTrail = true;
					this.ACT_MELEE.StartAction(o, newDir, IsidoraBehaviour.ISIDORA_SLASHES.SLASH, this.slashTime, true, this.holdTime2, true, this.twirlTime, keepTwirl, delegate()
					{
						this.ACT_MOVE.StopAction();
					});
					Vector2 targetPoint = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * anticipationDistance;
					targetPoint = o.ClampInsideBoundaries(targetPoint, true, false);
					this.ACT_MOVE.StartAction(o, targetPoint, this.holdTime2, Ease.OutQuad, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					float nSlashDistance = o.GetDirToPenitent().magnitude + 1f;
					Vector2 afterSlashPosition2 = o.transform.position + Vector2.right * Mathf.Sign(newDir.x) * nSlashDistance + Vector2.down * vOffset;
					afterSlashPosition2 = o.ClampInsideBoundaries(afterSlashPosition2, true, false);
					this.ACT_MOVE.StartAction(o, afterSlashPosition2, this.slashTime, Ease.OutQuad, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					o.ghostTrail.EnableGhostTrail = false;
					if (!keepTwirl)
					{
						this.twirlDistance += 2f;
						this.twirlTime += 1f;
					}
					Vector2 twirlTarget2 = o.transform.position + Vector2.left * Mathf.Sign(newDir.x) * this.twirlDistance + Vector2.up * vOffset;
					twirlTarget2 = o.ClampInsideBoundaries(twirlTarget2, true, false);
					o.Isidora.AnimatorInyector.Decelerate(this.twirlTime * 0.5f);
					this.ACT_MOVE.StartAction(o, twirlTarget2, this.twirlTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					yield return this.ACT_MELEE.waitForCompletion;
				}
				o.Isidora.Audio.SetSkullsChoir(false);
				base.FinishAction();
				yield break;
			}

			private float distanceFromPenitent;

			private float slashDistance;

			private float slashTime;

			private float holdTime1;

			private float holdTime2;

			private float vanishTime;

			private float vanishDistance;

			private float twirlTime;

			private float twirlDistance;

			private int repetitions;

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.MeleeAttack_EnemyAction ACT_MELEE = new IsidoraBehaviour.MeleeAttack_EnemyAction();

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();
		}

		public class FadeSlashCombo1_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_FADE_SLASH.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				IsidoraAnimatorInyector anim = o.Isidora.AnimatorInyector;
				int i = 1;
				for (int j = 0; j < i; j++)
				{
					Vector2 targetPoint = o.ArenaGetBotRightCorner() + Vector2.left;
					Vector2 dir = Vector2.left;
					if (o.IsIsidoraOnTheRightSide())
					{
						dir = Vector2.right;
						targetPoint = o.ArenaGetBotLeftCorner() + Vector2.right;
					}
					targetPoint.y -= 0.75f;
					this.ACT_FADE_SLASH.StartAction(o, dir, targetPoint);
					yield return this.ACT_FADE_SLASH.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private IsidoraBehaviour.SingleFadeSlash_EnemyAction ACT_FADE_SLASH = new IsidoraBehaviour.SingleFadeSlash_EnemyAction();
		}

		public class SingleFadeSlash_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 direction, Vector2 startPosition)
			{
				this.direction = direction;
				this.startPosition = startPosition;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE_AUX.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_TELEPORT.StopAction();
				this.ACT_BLASTS.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.Isidora.Audio.SetSkullsChoir(false);
				isidoraBehaviour.Isidora.Audio.SetIsidoraVoice(false);
				isidoraBehaviour.transform.DOKill(false);
				isidoraBehaviour.Isidora.AnimatorInyector.SetFadeSlash(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				IsidoraAnimatorInyector anim = o.Isidora.AnimatorInyector;
				if (!o.IsAnimatorInState("OUT"))
				{
					o.Isidora.AnimatorInyector.SetHidden(true);
				}
				yield return new IsidoraBehaviour.WaitUntilOut(o);
				Vector2 targetPoint = this.startPosition;
				o.transform.position = targetPoint;
				int currentBar = o.Isidora.Audio.bossAudioSync.LastBar;
				int lastAttackMarkerBar = o.Isidora.Audio.lastAttackMarkerBar;
				o.LookAtDirUsingOrientation(this.direction);
				o.Isidora.AnimatorInyector.SetHidden(false);
				yield return new IsidoraBehaviour.WaitUntilIdle(o);
				anim.PlaySlashAttack();
				anim.SetAttackAnticipation(true);
				float untilAnticipation = 0.7f;
				this.ACT_WAIT.StartAction(o, untilAnticipation);
				yield return this.ACT_WAIT.waitForCompletion;
				Debug.Log(string.Format("<color=blue>Anticipation starts!</color>", new object[0]));
				float remainingTime = o.Isidora.Audio.GetTimeLeftForCurrentBar();
				Debug.Log(string.Format("<color=blue>LETS SEE -> Remaining time for current bar </color>" + remainingTime, new object[0]));
				if (remainingTime < 0.75f)
				{
					Debug.Log(string.Format("<color=orange>NOT ENOUGH TIME. Adding a full bar. </color>", new object[0]));
					remainingTime += o.Isidora.Audio.GetSingleBarDuration();
					Debug.Log(string.Format("<color=orange>[0]Remaining time for current bar </color>" + remainingTime, new object[0]));
				}
				this.ACT_WAIT.StartAction(o, remainingTime * 0.33f);
				yield return this.ACT_WAIT.waitForCompletion;
				Debug.Log(string.Format("<color=blue>[1]Remaining time for current bar </color>" + o.Isidora.Audio.GetTimeLeftForCurrentBar(), new object[0]));
				Debug.Log(string.Format("<color=blue>[1]Accumulated remainingTime </color>" + (remainingTime - remainingTime * 0.33f), new object[0]));
				Vector2 anticipationDir = (!o.IsIsidoraOnTheRightSide()) ? Vector2.left : Vector2.right;
				targetPoint = o.transform.position + anticipationDir;
				this.ACT_MOVE.StartAction(o, targetPoint, remainingTime * 0.33f, Ease.OutCubic, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Debug.Log(string.Format("<color=blue>[2]Remaining time for current bar </color>" + o.Isidora.Audio.GetTimeLeftForCurrentBar(), new object[0]));
				Debug.Log(string.Format("<color=blue>[2]Accumulated remainingTime </color>" + (remainingTime - remainingTime * 0.66f), new object[0]));
				float fadeSlashAnticipationSeconds = 0.25f;
				float justWait = remainingTime * 0.33f - fadeSlashAnticipationSeconds;
				this.ACT_MOVE.StartAction(o, targetPoint - anticipationDir, justWait, Ease.InCubic, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Debug.Log(string.Format("<color=blue>[3]Remaining time for current bar </color>" + o.Isidora.Audio.GetTimeLeftForCurrentBar(), new object[0]));
				Debug.Log(string.Format("<color=blue>[3]Accumulated remainingTime </color>" + (remainingTime - remainingTime * 0.66f - justWait), new object[0]));
				int i = 3;
				for (int j = 0; j < i; j++)
				{
					Debug.Log(string.Format("<color=blue>Bar finished: Fade Slash starts!</color>", new object[0]));
					targetPoint.x = ((!o.IsIsidoraOnTheRightSide()) ? (o.battleBounds.xMax - 2f) : (o.battleBounds.xMin + 2f));
					anim.SetFadeSlash(true);
					anim.SetHidden(true);
					anim.SetAttackAnticipation(false);
					o.ghostTrail.EnableGhostTrail = true;
					o.WarningVFX(Vector2.up);
					float waitBeforeMove = (j != 0) ? fadeSlashAnticipationSeconds : (fadeSlashAnticipationSeconds - 0.1f);
					this.ACT_WAIT.StartAction(o, waitBeforeMove);
					yield return this.ACT_WAIT.waitForCompletion;
					Debug.Log(string.Format("<color=blue>[4]Remaining time for current bar </color>" + o.Isidora.Audio.GetTimeLeftForCurrentBar(), new object[0]));
					o.LinearScreenshake();
					o.SlashLineVFX();
					o.Isidora.Audio.PlayFadeDash();
					this.ACT_MOVE.StartAction(o, targetPoint, 0.1f, Ease.InOutCubic, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					o.ghostTrail.EnableGhostTrail = false;
					if (j < i - 1)
					{
						float restSeconds = 0.75f;
						this.ACT_WAIT.StartAction(o, restSeconds);
						yield return this.ACT_WAIT.waitForCompletion;
						o.LookAtTarget(o.battleBounds.center);
						anim.SetHidden(false);
						this.ACT_WAIT.StartAction(o, 0.05f);
						yield return this.ACT_WAIT.waitForCompletion;
					}
					else
					{
						anim.SetFadeSlash(false);
						anim.SetHidden(false);
						this.ACT_WAIT.StartAction(o, 0.1f);
						yield return this.ACT_WAIT.waitForCompletion;
					}
				}
				Vector2 startingPos = o.transform.position;
				this.ACT_BLASTS.StartAction(o, startingPos - Vector2.right * 0.75f, Vector2.right, 2f, false, 1, 0.4f);
				this.ACT_BLASTS.StartAction(o, startingPos + Vector2.right * 0.75f, Vector2.left, 2f, false, 1, 0.4f);
				anim.SetFadeSlash(false);
				anim.SetHidden(true);
				float fadeSlashRecoverSeconds = 1.85f;
				this.ACT_WAIT.StartAction(o, fadeSlashRecoverSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AUX = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();

			private IsidoraBehaviour.BlastsAttack_EnemyAction ACT_BLASTS = new IsidoraBehaviour.BlastsAttack_EnemyAction();

			private Vector2 startPosition;

			private Vector2 direction;
		}

		public class SyncCombo1_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE_AUX.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_TELEPORT.StopAction();
				this.ACT_BLASTS.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.Isidora.Audio.SetSkullsChoir(false);
				isidoraBehaviour.Isidora.Audio.SetIsidoraVoice(false);
				isidoraBehaviour.transform.DOKill(false);
				isidoraBehaviour.Isidora.AnimatorInyector.SetFadeSlash(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				IsidoraAnimatorInyector anim = o.Isidora.AnimatorInyector;
				if (!o.IsAnimatorInState("OUT"))
				{
					o.Isidora.AnimatorInyector.SetHidden(true);
				}
				yield return new IsidoraBehaviour.WaitUntilOut(o);
				Vector2 targetPoint = (!o.IsIsidoraOnTheRightSide()) ? (o.ArenaGetBotRightCorner() + Vector2.left) : (o.ArenaGetBotLeftCorner() + Vector2.right);
				targetPoint.y -= 0.4f;
				o.transform.position = targetPoint;
				int currentBar = o.Isidora.Audio.bossAudioSync.LastBar;
				int lastAttackMarkerBar = o.Isidora.Audio.lastAttackMarkerBar;
				if (currentBar != lastAttackMarkerBar)
				{
				}
				o.Isidora.AnimatorInyector.SetHidden(false);
				o.LookAtTarget(o.ArenaGetBotFarRandomPoint());
				anim.PlaySlashAttack();
				anim.SetAttackAnticipation(true);
				float untilAnticipation = 0.5f;
				this.ACT_WAIT.StartAction(o, untilAnticipation);
				yield return this.ACT_WAIT.waitForCompletion;
				Debug.Log(string.Format("<color=blue>Bar finished: Anticipation starts!</color>", new object[0]));
				float remainingTime = o.Isidora.Audio.GetTimeLeftForCurrentBar();
				this.ACT_WAIT.StartAction(o, remainingTime * 0.3f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector2 anticipationDir = (!o.IsIsidoraOnTheRightSide()) ? Vector2.left : Vector2.right;
				targetPoint = o.transform.position + anticipationDir;
				this.ACT_MOVE.StartAction(o, targetPoint, remainingTime * 0.3f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(o.Isidora.Audio);
				Debug.Log(string.Format("<color=blue>Bar finished: Attack 1 starts!</color>", new object[0]));
				anim.SetTwirl(true);
				targetPoint.x = ((!o.IsIsidoraOnTheRightSide()) ? (o.battleBounds.xMax - 2f) : (o.battleBounds.xMin + 2f));
				anim.SetFadeSlash(true);
				anim.SetAttackAnticipation(false);
				float fadeSlashAnticipationSeconds = 0.25f;
				float fadeSlashRecoverSeconds = 0.7f;
				this.ACT_WAIT.StartAction(o, fadeSlashAnticipationSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				o.SlashLineVFX();
				this.ACT_MOVE.StartAction(o, targetPoint, 0.1f, Ease.InOutCubic, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				float attackTime = o.Isidora.Audio.GetSingleBarDuration() * 0.4f;
				Vector2 startingPos = o.transform.position;
				this.ACT_BLASTS.StartAction(o, startingPos - Vector2.right * 0.5f, Vector2.right, 1.5f, false, 3, 0.5f);
				this.ACT_BLASTS.StartAction(o, startingPos + Vector2.right * 0.5f, Vector2.left, 1.5f, false, 3, 0.5f);
				anim.SetFadeSlash(false);
				this.ACT_WAIT.StartAction(o, fadeSlashRecoverSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				float waitTime = o.Isidora.Audio.GetSingleBarDuration() * 0.5f;
				anim.Decelerate(waitTime * 0.8f);
				this.ACT_WAIT.StartAction(o, waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(o.Isidora.Audio);
				Debug.Log(string.Format("<color=blue>Bar finished: Dance starts!</color>", new object[0]));
				remainingTime = o.Isidora.Audio.GetSingleBarDuration();
				targetPoint = o.transform.position + Vector3.up * 0.75f;
				this.ACT_MOVE.StartAction(o, targetPoint, remainingTime * 0.5f, Ease.InOutCubic, null, true, null, false, true, 1.7f);
				anticipationDir = ((!o.IsIsidoraOnTheRightSide()) ? Vector2.left : Vector2.right);
				targetPoint.x = o.transform.position.x + anticipationDir.x;
				this.ACT_MOVE_AUX.StartAction(o, targetPoint, remainingTime * 0.3f, Ease.InOutCubic, null, true, null, true, false, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				yield return this.ACT_MOVE_AUX.waitForCompletion;
				anim.resetAnimationSpeedFlag = true;
				anim.PlaySlashAttack();
				anim.SetAttackAnticipation(true);
				o.LookAtTarget(o.ArenaGetBotFarRandomPoint());
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(o.Isidora.Audio);
				Debug.Log(string.Format("<color=blue>Bar finished: Attack 2 starts!</color>", new object[0]));
				targetPoint.x = ((!o.IsIsidoraOnTheRightSide()) ? (o.ArenaGetBotRightCorner().x - 2f) : (o.ArenaGetBotLeftCorner().x + 2f));
				this.ACT_MOVE.StartAction(o, targetPoint, attackTime, Ease.InOutCubic, null, true, null, true, true, 1.7f);
				this.ACT_WAIT.StartAction(o, attackTime * 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Isidora.Audio.SetIsidoraVoice(false);
				anim.SetAttackAnticipation(false);
				yield return this.ACT_MOVE.waitForCompletion;
				float twirlTime = o.Isidora.Audio.GetSingleBarDuration() * 0.5f;
				anim.Decelerate(twirlTime * 0.3f);
				targetPoint.y -= 0.75f;
				this.ACT_MOVE.StartAction(o, targetPoint, twirlTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				anim.SetTwirl(false);
				base.FinishAction();
				yield break;
			}

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AUX = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();

			private IsidoraBehaviour.BlastsAttack_EnemyAction ACT_BLASTS = new IsidoraBehaviour.BlastsAttack_EnemyAction();
		}

		public class HorizontalDash_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float anticipationTime, float dashDuration, float shoryukenDuration, float floatDownDuration)
			{
				this.anticipationTime = anticipationTime;
				this.dashDuration = dashDuration;
				this.shoryukenDuration = shoryukenDuration;
				this.floatDownDuration = floatDownDuration;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE2.StopAction();
				this.ACT_TELEPORT.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MELEE.StopAction();
				this.ACT_SINGLEBLAST.StopAction();
				this.ACT_SINGLEBLAST2.StopAction();
				this.ACT_SINGLEBLAST3.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				IsidoraAnimatorInyector animatorInyector = isidoraBehaviour.Isidora.AnimatorInyector;
				animatorInyector.SetFireScythe(false);
				animatorInyector.ResetAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				IsidoraAnimatorInyector anim = o.Isidora.AnimatorInyector;
				float distanceFromWall = 2f;
				Vector2 leftCorner = o.ArenaGetBotLeftCorner() + Vector2.right * distanceFromWall;
				Vector2 rightCorner = o.ArenaGetBotRightCorner() + Vector2.left * distanceFromWall;
				Vector2 dir = (p.GetPosition().x <= o.battleBounds.center.x) ? Vector2.left : Vector2.right;
				Vector2 startPosition = (dir.x <= 0f) ? rightCorner : leftCorner;
				Vector2 endPosition = (dir.x <= 0f) ? leftCorner : rightCorner;
				startPosition.y += 0.5f;
				endPosition.y += 0.5f;
				endPosition -= dir * 2f;
				if (o.IsAnimatorInState("OUT"))
				{
					o.transform.position = startPosition;
					anim.SetHidden(false);
					anim.SetTwirl(true);
					this.ACT_WAIT.StartAction(o, 0.1f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				else
				{
					this.ACT_TELEPORT.StartAction(o, startPosition, 0.1f, true, false, p.transform);
					yield return this.ACT_TELEPORT.waitForCompletion;
				}
				o.LookAtDirUsingOrientation(dir);
				anim.SetFireScythe(true);
				anim.PlayRisingSlash();
				anim.SetAttackAnticipation(true);
				this.ACT_SINGLEBLAST.StartAction(o, o.transform.position - dir * 1.5f, 0f, 0.25f, 0.5f, 0.1f, false);
				this.ACT_WAIT.StartAction(o, this.anticipationTime);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_MOVE.StartAction(o, endPosition, this.dashDuration, Ease.InCubic, null, true, null, true, false, 1.7f);
				this.ACT_WAIT.StartAction(o, this.dashDuration * 0.9f);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.SetAttackAnticipation(false);
				float risingHeight = 4f;
				float extraDelay = this.shoryukenDuration * 0.5f;
				Vector2 blastOffset = dir * 1.25f;
				this.ACT_SINGLEBLAST.StartAction(o, endPosition + blastOffset, 0f, 0.2f, 0.3f, extraDelay, false);
				this.ACT_SINGLEBLAST2.StartAction(o, endPosition + blastOffset * 2f, 0f, 0.2f, 0.3f, extraDelay + 0.2f, false);
				this.ACT_SINGLEBLAST3.StartAction(o, endPosition + blastOffset * 3f, 0f, 0.2f, 0.3f, extraDelay + 0.4f, false);
				this.ACT_MOVE2.StartAction(o, endPosition + Vector2.up * risingHeight, this.shoryukenDuration, Ease.OutCubic, null, true, null, false, true, 1.7f);
				yield return this.ACT_MOVE2.waitForCompletion;
				anim.Decelerate(this.floatDownDuration * 0.7f);
				this.ACT_MOVE2.StartAction(o, endPosition, this.floatDownDuration, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE2.waitForCompletion;
				anim.SetTwirl(false);
				anim.SetFireScythe(false);
				this.ACT_WAIT.StartAction(o, 0.3f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private float anticipationTime;

			private float dashDuration;

			private float shoryukenDuration;

			private float floatDownDuration;

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE2 = new MoveEasing_EnemyAction();

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.MeleeAttack_EnemyAction ACT_MELEE = new IsidoraBehaviour.MeleeAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST2 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST3 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();
		}

		public class InvisibleHorizontalDash_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float anticipationTime, float dashDuration, float shoryukenDuration, float floatDownDuration)
			{
				this.anticipationTime = anticipationTime;
				this.dashDuration = dashDuration;
				this.shoryukenDuration = shoryukenDuration;
				this.floatDownDuration = floatDownDuration;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE2.StopAction();
				this.ACT_TELEPORT.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MELEE.StopAction();
				this.ACT_SINGLEBLAST.StopAction();
				this.ACT_SINGLEBLAST2.StopAction();
				this.ACT_SINGLEBLAST3.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				IsidoraAnimatorInyector animatorInyector = isidoraBehaviour.Isidora.AnimatorInyector;
				animatorInyector.SetFireScythe(false);
				animatorInyector.ResetAll();
				isidoraBehaviour.floorSparksParticlesToRight.Stop();
				isidoraBehaviour.floorSparksParticlesToLeft.Stop();
				isidoraBehaviour.floorSparksMaskToRight.transform.DOKill(true);
				isidoraBehaviour.floorSparksMaskToLeft.transform.DOKill(true);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				IsidoraAnimatorInyector anim = o.Isidora.AnimatorInyector;
				float distanceFromWall = 2f;
				Vector2 leftCorner = o.ArenaGetBotLeftCorner() + Vector2.right * distanceFromWall;
				Vector2 rightCorner = o.ArenaGetBotRightCorner() + Vector2.left * distanceFromWall;
				Vector2 dir = (p.GetPosition().x <= o.battleBounds.center.x) ? Vector2.left : Vector2.right;
				Vector2 startPosition = (dir.x <= 0f) ? rightCorner : leftCorner;
				this.endPosition = ((dir.x <= 0f) ? leftCorner : rightCorner);
				startPosition.y += 0.5f;
				this.endPosition.y = this.endPosition.y + 0.5f;
				this.endPosition -= dir * 2f;
				if (!o.IsAnimatorInState("OUT"))
				{
					o.Isidora.AnimatorInyector.SetHidden(true);
				}
				yield return new IsidoraBehaviour.WaitUntilOut(o);
				o.transform.position = startPosition;
				o.LookAtDirUsingOrientation(dir);
				anim.SetFireScythe(true);
				anim.PlayRisingSlash();
				anim.SetAttackAnticipation(true);
				o.SingleSparkVFX(0f);
				this.ACT_WAIT.StartAction(o, this.anticipationTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Isidora.Audio.PlayInvisibleDash();
				if (dir.x > 0f)
				{
					o.floorSparksParticlesToLeft.Play();
					o.floorSparksMaskToLeft.gameObject.SetActive(true);
					Vector3 prevScale = o.floorSparksMaskToLeft.transform.localScale;
					o.floorSparksMaskToLeft.transform.localScale = Vector3.zero;
					o.floorSparksMaskToLeft.transform.DOScale(prevScale, this.dashDuration).OnComplete(delegate
					{
						o.floorSparksMaskToLeft.gameObject.SetActive(false);
						o.floorSparksMaskToLeft.transform.localScale = prevScale;
					});
				}
				else
				{
					o.floorSparksParticlesToRight.Play();
					o.floorSparksMaskToRight.gameObject.SetActive(true);
					Vector3 prevScale = o.floorSparksMaskToRight.transform.localScale;
					o.floorSparksMaskToRight.transform.localScale = Vector3.zero;
					o.floorSparksMaskToRight.transform.DOScale(prevScale, this.dashDuration).OnComplete(delegate
					{
						o.floorSparksMaskToRight.gameObject.SetActive(false);
						o.floorSparksMaskToRight.transform.localScale = prevScale;
					});
				}
				this.ACT_MOVE.StartAction(o, this.endPosition, this.dashDuration, Ease.InQuart, null, true, new Action(this.CheckIfNearPenitentToAppear), true, false, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				anim.SetTwirl(true);
				anim.SetAttackAnticipation(false);
				o.floorSparksParticlesToRight.Stop();
				o.floorSparksParticlesToLeft.Stop();
				o.floorSparksMaskToRight.transform.DOKill(true);
				o.floorSparksMaskToLeft.transform.DOKill(true);
				this.endPosition = o.transform.position + dir;
				this.ACT_MOVE.StartAction(o, this.endPosition, 0.3f, Ease.InQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				float extraDelay = this.shoryukenDuration * 0.5f;
				Vector2 blastOffset = dir * 1.25f;
				this.ACT_SINGLEBLAST.StartAction(o, this.endPosition + blastOffset, 0f, 0.2f, 0.3f, extraDelay, false);
				this.ACT_SINGLEBLAST2.StartAction(o, this.endPosition + blastOffset * 2f, 0f, 0.2f, 0.3f, extraDelay + 0.2f, false);
				this.ACT_SINGLEBLAST3.StartAction(o, this.endPosition + blastOffset * 3f, 0f, 0.2f, 0.3f, extraDelay + 0.4f, false);
				o.ghostTrail.EnableGhostTrail = true;
				float risingHeight = 4f;
				this.ACT_MOVE2.StartAction(o, this.endPosition + Vector2.up * risingHeight, this.shoryukenDuration, Ease.OutCubic, null, true, null, false, true, 1.7f);
				yield return this.ACT_MOVE2.waitForCompletion;
				o.ghostTrail.EnableGhostTrail = false;
				anim.Decelerate(this.floatDownDuration * 0.7f);
				this.ACT_MOVE2.StartAction(o, this.endPosition, this.floatDownDuration, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE2.waitForCompletion;
				anim.SetTwirl(false);
				anim.SetFireScythe(false);
				this.ACT_WAIT.StartAction(o, 0.3f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private void CheckIfNearPenitentToAppear()
			{
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				Penitent penitent = Core.Logic.Penitent;
				IsidoraAnimatorInyector animatorInyector = isidoraBehaviour.Isidora.AnimatorInyector;
				if (Vector2.Distance(isidoraBehaviour.transform.position, penitent.GetPosition()) < 2f || Vector2.Distance(isidoraBehaviour.transform.position, this.endPosition) < 2f)
				{
					animatorInyector.SetHidden(false);
				}
				if (!isidoraBehaviour.IsAnimatorInState("OUT"))
				{
					this.ACT_MOVE.StopAction();
				}
			}

			private float anticipationTime;

			private float dashDuration;

			private float shoryukenDuration;

			private float floatDownDuration;

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE2 = new MoveEasing_EnemyAction();

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.MeleeAttack_EnemyAction ACT_MELEE = new IsidoraBehaviour.MeleeAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST2 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST3 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private Vector2 endPosition;
		}

		public class Teleport_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 targetPosition, float timeInvisible, bool twirlOnExit, bool castingOnExit, Transform lookTarget)
			{
				this.targetPosition = targetPosition;
				this.timeInvisible = timeInvisible;
				this.twirlOnExit = twirlOnExit;
				this.castingOnExit = castingOnExit;
				this.lookTarget = lookTarget;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				(this.owner as IsidoraBehaviour).Isidora.AnimatorInyector.ResetAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				IsidoraAnimatorInyector anim = o.Isidora.AnimatorInyector;
				anim.SetHidden(true);
				this.timeInvisible = Mathf.Clamp(this.timeInvisible, anim.GetVanishAnimationDuration(), this.timeInvisible);
				this.ACT_WAIT.StartAction(o, this.timeInvisible);
				yield return this.ACT_WAIT.waitForCompletion;
				o.transform.position = this.targetPosition;
				if (this.lookTarget != null)
				{
					o.LookAtTarget(this.lookTarget.position);
				}
				anim.SetTwirl(this.twirlOnExit);
				anim.SetCasting(this.castingOnExit);
				anim.SetHidden(false);
				if (this.twirlOnExit)
				{
					yield return new IsidoraBehaviour.WaitUntilTwirl(o);
				}
				else if (this.castingOnExit)
				{
					yield return new IsidoraBehaviour.WaitUntilCasting(o);
				}
				else
				{
					yield return new IsidoraBehaviour.WaitUntilIdle(o);
				}
				base.FinishAction();
				yield break;
			}

			private Vector2 targetPosition;

			private float timeInvisible;

			private bool twirlOnExit;

			private bool castingOnExit;

			private Transform lookTarget;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class PhaseSwitchAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_BONFIRE.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_BLASTS.StopAction();
				this.ACT_TELEPORT.StopAction();
				this.ACT_SINGLEBLAST.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.Isidora.Audio.SetIsidoraVoice(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour isidora = this.owner as IsidoraBehaviour;
				IsidoraAnimatorInyector animatorInyector = isidora.Isidora.AnimatorInyector;
				Penitent p = Core.Logic.Penitent;
				IsidoraAudio mAudio = isidora.Isidora.Audio;
				isidora.Isidora.Audio.SetSecondPhase();
				Debug.Log("Audio.SetSecondPhase TIME:  " + mAudio.GetTimeSinceLevelLoad());
				animatorInyector.SetHidden(true);
				this.ACT_SINGLEBLAST.StartAction(isidora, isidora.transform.position, 0.5f, 1f, 3f, 0f, false);
				this.ACT_WAIT.StartAction(isidora, 0.8f);
				yield return this.ACT_WAIT.waitForCompletion;
				Debug.Log("Audio.WaitUntilNextValidBar - TIME:  " + mAudio.GetTimeSinceLevelLoad());
				yield return new IsidoraBehaviour.WaitUntilNextValidBar(mAudio);
				Debug.Log("AUDIO: ToPhase3 (2 bars) TIME:  " + mAudio.GetTimeSinceLevelLoad());
				float barTime = mAudio.GetSingleBarDuration();
				Debug.Log("BARTIME: " + barTime);
				Vector2 targetPoint = isidora.ArenaGetBotRightCorner() + Vector2.left * 2f;
				Vector2 blastOrientation = Vector2.left;
				if (p.GetPosition().x > isidora.battleBounds.center.x)
				{
					targetPoint = isidora.ArenaGetBotLeftCorner() + Vector2.right * 2f;
					blastOrientation = Vector2.right;
				}
				Vector2 startingPos = targetPoint + blastOrientation * -2.5f;
				this.ACT_BLASTS.StartAction(isidora, startingPos, blastOrientation, barTime, false, 5, 4f);
				yield return this.ACT_BLASTS.waitForCompletion;
				startingPos += blastOrientation * -0.5f;
				this.ACT_BLASTS.StartAction(isidora, startingPos, blastOrientation, barTime, false, 7, 3f);
				yield return this.ACT_BLASTS.waitForCompletion;
				Debug.Log("AUDIO: Phase3 starts (2 bars) TIME:  " + mAudio.GetTimeSinceLevelLoad());
				isidora.Isidora.Audio.SetIsidoraVoice(true);
				targetPoint = isidora.battleBounds.center + Vector2.right * 0.3f;
				isidora.transform.position = targetPoint;
				startingPos += blastOrientation * 1.5f;
				this.ACT_BLASTS.StartAction(isidora, startingPos, blastOrientation, barTime, false, 5, 4f);
				yield return this.ACT_BLASTS.waitForCompletion;
				startingPos += blastOrientation * -1.5f;
				this.ACT_BLASTS.StartAction(isidora, startingPos, blastOrientation, barTime, false, 7, 3f);
				yield return this.ACT_BLASTS.waitForCompletion;
				Debug.Log("AUDIO: Epic voice starts(2 bars) TIME:  " + mAudio.GetTimeSinceLevelLoad());
				animatorInyector.SetHidden(false);
				animatorInyector.SetCasting(true);
				targetPoint = isidora.transform.position + Vector3.up * 3f;
				this.ACT_MOVE.StartAction(isidora, targetPoint, 2f * barTime, Ease.InOutCubic, null, true, null, true, true, 1.7f);
				Vector2 castingPos = isidora.homingBonfireBehavior.gameObject.transform.position;
				this.ACT_BONFIRE.StartAction(isidora, isidora.Isidora.Audio.GetSingleBarDuration(), 1, true, castingPos, 0f, 0f, -1f);
				yield return this.ACT_BONFIRE.waitForCompletion;
				MasterShaderEffects effects = Core.Logic.Penitent.GetComponentInChildren<MasterShaderEffects>();
				if (effects != null)
				{
					effects.StartColorizeLerp(0.5f, 0f, 5f, null);
				}
				isidora.Isidora.DamageEffect.StartColorizeLerp(0.2f, 0f, 5f, null);
				startingPos = isidora.ArenaGetBotLeftCorner();
				Vector2 startingPosRight = isidora.ArenaGetBotRightCorner();
				int i = 6;
				for (int j = 0; j < i; j++)
				{
					float baseOffset = 1.5f;
					Vector2 offset = Vector2.right * (float)j * baseOffset;
					this.ACT_SINGLEBLAST.StartAction(isidora, startingPos + offset, 0.5f, 1f, 0.7f, 0f, false);
					this.ACT_SINGLEBLAST2.StartAction(isidora, startingPosRight - offset, 0.5f, 1f, 0.7f, 0f, false);
					yield return this.ACT_SINGLEBLAST.waitForCompletion;
				}
				yield return this.ACT_MOVE.waitForCompletion;
				animatorInyector.SetCasting(false);
				targetPoint = isidora.transform.position + Vector3.down * 4f;
				isidora.Isidora.Audio.SetIsidoraVoice(false);
				this.ACT_MOVE.StartAction(isidora, targetPoint, 4f, Ease.InOutCubic, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private IsidoraBehaviour.BonfireProjectilesAttack_EnemyAction ACT_BONFIRE = new IsidoraBehaviour.BonfireProjectilesAttack_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.BlastsAttack_EnemyAction ACT_BLASTS = new IsidoraBehaviour.BlastsAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST2 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();
		}

		public class HomingProjectilesAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float horizontalSpacingFactor, float verticalSpacingFactor)
			{
				this.horizontalSpacingFactor = horizontalSpacingFactor;
				this.verticalSpacingFactor = verticalSpacingFactor;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_WAIT_AUX.StopAction();
				this.ACT_TELEPORT.StopAction();
				this.ACT_SINGLEBLAST.StopAction();
				this.ACT_SINGLEBLAST2.StopAction();
				this.ACT_SINGLEBLAST3.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.Isidora.Audio.SetIsidoraVoice(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				if (!o.IsAnimatorInState("OUT"))
				{
					o.Isidora.AnimatorInyector.SetHidden(true);
				}
				yield return new IsidoraBehaviour.WaitUntilOut(o);
				Vector2 center = o.battleBounds.center + Vector2.right * 0.3f;
				o.transform.position = center - Vector2.up * 1.5f;
				int currentBar = o.Isidora.Audio.bossAudioSync.LastBar;
				int lastAttackMarkerBar = o.Isidora.Audio.lastAttackMarkerBar;
				if (currentBar != lastAttackMarkerBar)
				{
					o.Isidora.Audio.SetIsidoraVoice(true);
					float timeLeft = o.Isidora.Audio.GetTimeLeftForCurrentBar();
					if (timeLeft > 1f)
					{
						this.ACT_SINGLEBLAST.StartAction(o, center, 0f, timeLeft * 0.4f, timeLeft * 0.4f, 0f, false);
						this.ACT_SINGLEBLAST2.StartAction(o, center + Vector2.right * 3f, 0f, timeLeft * 0.4f, timeLeft * 0.4f, 0f, false);
						this.ACT_SINGLEBLAST3.StartAction(o, center + Vector2.left * 3f, 0f, timeLeft * 0.4f, timeLeft * 0.4f, 0f, false);
					}
					this.ACT_WAIT.StartAction(o, timeLeft * 0.9f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				else
				{
					yield return new WaitUntil(() => currentBar != o.Isidora.Audio.bossAudioSync.LastBar);
					center.y = o.battleBounds.yMin;
					this.ACT_SINGLEBLAST.StartAction(o, center, 0f, 1f, 1f, 0f, false);
					this.ACT_SINGLEBLAST2.StartAction(o, center + Vector2.right * 3f, 0f, 1f, 1f, 0f, false);
					this.ACT_SINGLEBLAST3.StartAction(o, center + Vector2.left * 3f, 0f, 1f, 1f, 0f, false);
					o.Isidora.Audio.SetIsidoraVoice(true);
					this.ACT_WAIT.StartAction(o, o.Isidora.Audio.GetSingleBarDuration() * 0.9f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				o.Isidora.AnimatorInyector.SetHidden(false);
				o.Isidora.AnimatorInyector.SetCasting(true);
				o.homingBonfireAttack.NumProjectiles = 1;
				o.homingBonfireAttack.HorizontalSpacingFactor = this.horizontalSpacingFactor;
				o.homingBonfireAttack.VerticalSpacingFactor = this.verticalSpacingFactor;
				int i = 4;
				float delay = o.Isidora.Audio.GetSingleBarDuration() * 0.4f;
				float intro = o.Isidora.Audio.GetSingleBarDuration() * 0.4f;
				float outro = o.Isidora.Audio.GetSingleBarDuration() * 0.3f;
				Tweener t = o.transform.DOMoveY(o.transform.position.y + 2f, intro + delay * (float)i + outro, false).SetEase(Ease.InOutCubic);
				this.ACT_WAIT.StartAction(o, intro);
				yield return this.ACT_WAIT.waitForCompletion;
				currentBar = o.Isidora.Audio.bossAudioSync.LastBar;
				for (int j = 0; j < i; j++)
				{
					o.homingBonfireAttack.FireProjectile();
					this.ACT_WAIT.StartAction(o, delay);
					yield return this.ACT_WAIT.waitForCompletion;
					if (currentBar != o.Isidora.Audio.bossAudioSync.LastBar)
					{
						o.Isidora.Audio.SetIsidoraVoice(false);
					}
				}
				o.Isidora.AnimatorInyector.SetCasting(false);
				t = o.transform.DOMoveY(o.transform.position.y - 3f, 1f, false).SetEase(Ease.InOutCubic);
				yield return t.WaitForCompletion();
				base.FinishAction();
				yield break;
			}

			private float horizontalSpacingFactor;

			private float verticalSpacingFactor;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT_AUX = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST2 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST3 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();
		}

		public class BonfireProjectilesAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float attackCooldown, int numProjectiles, bool useCastingPosition, Vector2 castingPosition, float horizontalSpacingFactor, float verticalSpacingFactor, float activeTime)
			{
				this.attackCooldown = attackCooldown;
				this.numProjectiles = numProjectiles;
				this.useCastingPosition = useCastingPosition;
				this.castingPosition = castingPosition;
				this.horizontalSpacingFactor = horizontalSpacingFactor;
				this.verticalSpacingFactor = verticalSpacingFactor;
				this.activeTime = activeTime;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				(this.owner as IsidoraBehaviour).Isidora.AnimatorInyector.ResetAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				this.ACT_WAIT.StartAction(o, o.Isidora.Audio.GetTimeUntilNextValidBar());
				yield return this.ACT_WAIT.waitForCompletion;
				o.homingBonfireBehavior.SetupAttack(this.attackCooldown, this.numProjectiles, this.useCastingPosition, this.castingPosition, this.horizontalSpacingFactor, this.verticalSpacingFactor);
				o.homingBonfireBehavior.ActivateBonfire(true, 1f, 0f);
				if (this.activeTime < 0f)
				{
					o.homingBonfireBehavior.EnlargeMask();
					o.bonfireMaskIsEnlarged = true;
				}
				else
				{
					this.ACT_WAIT.StartAction(o, this.activeTime);
					yield return this.ACT_WAIT.waitForCompletion;
					o.homingBonfireBehavior.DeactivateBonfire(true, false);
				}
				base.FinishAction();
				yield break;
			}

			private float attackCooldown;

			private int numProjectiles;

			private bool useCastingPosition;

			private Vector2 castingPosition;

			private float horizontalSpacingFactor;

			private float verticalSpacingFactor;

			private float activeTime;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class BonfireChargeIsidoraAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 castingPosition, float timeToMaxRate, float activeTime)
			{
				this.castingPosition = castingPosition;
				this.timeToMaxRate = timeToMaxRate;
				this.activeTime = activeTime;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.homingBonfireBehavior.DeactivateBonfire(false, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				o.homingBonfireBehavior.SetupAttack(2, this.castingPosition, 0.5f, 0.5f);
				o.homingBonfireBehavior.StartChargingIsidora(this.timeToMaxRate);
				this.ACT_WAIT.StartAction(o, this.activeTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.homingBonfireBehavior.DeactivateBonfire(false, false);
				for (int i = 0; i < 30; i++)
				{
					if (!o.homingBonfireBehavior.IsAnyProjectileVisible())
					{
						break;
					}
					this.ACT_WAIT.StartAction(o, 0.1f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				o.homingBonfireBehavior.IsChargingIsidora = false;
				o.homingBonfireBehavior.BonfireAttack.ChargesIsidora = false;
				o.homingBonfireBehavior.ActivateBonfire(false, 1f, 0f);
				o.homingBonfireBehavior.SetupAttack(1, this.castingPosition, 0f, 0f);
				base.FinishAction();
				yield break;
			}

			private Vector2 castingPosition;

			private float timeToMaxRate;

			private float activeTime;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.BonfireProjectilesAttack_EnemyAction ACT_BONFIRE = new IsidoraBehaviour.BonfireProjectilesAttack_EnemyAction();
		}

		public class BlastsAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 startingPos, Vector2 direction, float waitTime, bool shouldVanish, int totalAreas = -1, float distanceBetweenAreas = -1f)
			{
				this.startingPos = startingPos;
				this.direction = direction;
				this.waitTime = waitTime;
				this.shouldVanish = shouldVanish;
				this.totalAreas = totalAreas;
				this.distanceBetweenAreas = distanceBetweenAreas;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_TELEPORT.StopAction();
				this.ACT_WAIT.StopAction();
				(this.owner as IsidoraBehaviour).Isidora.AnimatorInyector.ResetAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				if (this.shouldVanish)
				{
					o.Isidora.AnimatorInyector.SetHidden(true);
				}
				this.ACT_WAIT.StartAction(o, this.waitTime);
				int prevTotalAreas = o.blastAttack.totalAreas;
				float prevDistanceBetweenAreas = o.blastAttack.distanceBetweenAreas;
				o.blastAttack.totalAreas = ((this.totalAreas != -1) ? this.totalAreas : o.blastAttack.totalAreas);
				o.blastAttack.distanceBetweenAreas = ((this.distanceBetweenAreas != -1f) ? this.distanceBetweenAreas : o.blastAttack.distanceBetweenAreas);
				this.direction.y = 0f;
				o.blastAttack.SummonAreas(this.startingPos, this.direction, (this.direction.x <= 0f) ? EntityOrientation.Left : EntityOrientation.Right);
				o.blastAttack.totalAreas = prevTotalAreas;
				o.blastAttack.distanceBetweenAreas = prevDistanceBetweenAreas;
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private Vector2 startingPos;

			private Vector2 direction;

			private float waitTime;

			private bool shouldVanish;

			private int totalAreas;

			private float distanceBetweenAreas;

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class ChargedBlastsAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_TELEPORT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_BLASTS.StopAction();
				this.ACT_BONFIRE.StopAction();
				this.ACT_SINGLEBLAST1.StopAction();
				this.ACT_SINGLEBLAST2.StopAction();
				this.ACT_SINGLEBLAST3.StopAction();
				this.ACT_SINGLEBLAST4.StopAction();
				this.ACT_SINGLEBLAST5.StopAction();
				IsidoraBehaviour isidoraBehaviour = this.owner as IsidoraBehaviour;
				isidoraBehaviour.Isidora.AnimatorInyector.ResetAll();
				isidoraBehaviour.StopFlameParticles();
				if (this.xTween != null)
				{
					this.xTween.Kill(false);
					this.xTween = null;
				}
				if (this.yTween != null)
				{
					this.yTween.Kill(false);
					this.yTween = null;
				}
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				Penitent p = Core.Logic.Penitent;
				IsidoraAnimatorInyector anim = o.Isidora.AnimatorInyector;
				o.numberOfCharges = 0;
				if (!o.IsAnimatorInState("OUT"))
				{
					anim.SetHidden(true);
				}
				yield return new IsidoraBehaviour.WaitUntilOut(o);
				Vector2 castingPosBottom = (!o.IsIsidoraOnTheRightSide()) ? (o.ArenaGetBotRightCorner() + Vector2.left) : (o.ArenaGetBotLeftCorner() + Vector2.right);
				o.transform.position = castingPosBottom;
				Vector2 blastsDir = o.battleBounds.center - castingPosBottom;
				blastsDir.y = 0f;
				blastsDir = blastsDir.normalized;
				o.LookAtDirUsingOrientation(blastsDir);
				int currentBar = o.Isidora.Audio.bossAudioSync.LastBar;
				int lastAttackMarkerBar = o.Isidora.Audio.lastAttackMarkerBar;
				if (currentBar != lastAttackMarkerBar)
				{
					o.Isidora.Audio.SetIsidoraVoice(true);
					float timeLeft = o.Isidora.Audio.GetTimeLeftForCurrentBar();
					if (timeLeft > 2f)
					{
						this.ACT_SINGLEBLAST1.StartAction(o, castingPosBottom, 0f, 1f, 1f, 0f, false);
						this.ACT_SINGLEBLAST2.StartAction(o, castingPosBottom + blastsDir * 3f, 0f, 1f, 1f, 0f, false);
						this.ACT_SINGLEBLAST3.StartAction(o, castingPosBottom + blastsDir * 6f, 0f, 1f, 1f, 0f, false);
					}
					this.ACT_WAIT.StartAction(o, timeLeft * 0.9f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				else
				{
					yield return new WaitUntil(() => currentBar != o.Isidora.Audio.bossAudioSync.LastBar);
					o.Isidora.Audio.SetIsidoraVoice(true);
					this.ACT_SINGLEBLAST1.StartAction(o, castingPosBottom, 0f, 1f, 1f, 0f, false);
					this.ACT_SINGLEBLAST2.StartAction(o, castingPosBottom + blastsDir * 3f, 0f, 1f, 1f, 0f, false);
					this.ACT_SINGLEBLAST3.StartAction(o, castingPosBottom + blastsDir * 6f, 0f, 1f, 1f, 0f, false);
					this.ACT_WAIT.StartAction(o, o.Isidora.Audio.GetSingleBarDuration() * 0.9f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				anim.SetHidden(false);
				anim.SetCasting(true);
				float singleBar = o.Isidora.Audio.GetSingleBarDuration();
				Vector2 castingPosTop = o.transform.position + Vector2.up * 4f;
				this.ACT_MOVE.StartAction(o, castingPosTop, singleBar * 0.7f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				this.ACT_BONFIRE.StartAction(o, o.homingBonfireBehavior.transform.position, singleBar * 0.5f, singleBar * 0.7f);
				yield return this.ACT_BONFIRE.waitForCompletion;
				yield return this.ACT_MOVE.waitForCompletion;
				yield return new IsidoraBehaviour.WaitUntilBarFinishes(o.Isidora.Audio);
				float bar = o.Isidora.Audio.GetSingleBarDuration();
				float totalTime = bar * 4f;
				Vector2 oppositePos = (castingPosTop.x <= o.battleBounds.center.x) ? (castingPosTop + o.battleBounds.width * Vector2.right * 0.8f) : (castingPosTop + o.battleBounds.width * Vector2.left * 0.8f);
				Vector2 startingDir = (oppositePos - castingPosTop).normalized;
				this.xTween = o.transform.DOMoveX(oppositePos.x, totalTime * 0.4f, false);
				this.xTween.SetEase(Ease.InQuad);
				this.xTween.OnComplete(delegate
				{
					this.xTween = o.transform.DOMoveX(oppositePos.x + startingDir.x, totalTime * 0.05f, false);
					this.xTween.SetEase(Ease.Linear);
					this.xTween.OnComplete(delegate
					{
						this.xTween = o.transform.DOMoveX(oppositePos.x, totalTime * 0.05f, false);
						this.xTween.SetEase(Ease.Linear);
						this.xTween.OnComplete(delegate
						{
							this.xTween = o.transform.DOMoveX(castingPosTop.x, totalTime * 0.4f, false);
							this.xTween.SetEase(Ease.InQuad);
							this.xTween.OnComplete(delegate
							{
								this.xTween = o.transform.DOMoveX(castingPosTop.x - startingDir.x, totalTime * 0.05f, false);
								this.xTween.SetEase(Ease.Linear);
								this.xTween.OnComplete(delegate
								{
									this.xTween = o.transform.DOMoveX(castingPosTop.x, totalTime * 0.05f, false);
									this.xTween.SetEase(Ease.Linear);
								});
							});
						});
					});
				});
				this.yTween = o.transform.DOMoveY(castingPosTop.y - 2f, totalTime * 0.4f, false);
				this.yTween.SetEase(Ease.InOutQuad);
				this.yTween.OnComplete(delegate
				{
					this.yTween = o.transform.DOMoveY(castingPosTop.y, totalTime * 0.1f, false);
					this.yTween.SetEase(Ease.InOutQuad);
					this.yTween.OnComplete(delegate
					{
						this.yTween = o.transform.DOMoveY(castingPosTop.y - 2f, totalTime * 0.4f, false);
						this.yTween.SetEase(Ease.InOutQuad);
						this.yTween.OnComplete(delegate
						{
							this.yTween = o.transform.DOMoveY(castingPosTop.y, totalTime * 0.1f, false);
							this.yTween.SetEase(Ease.InOutQuad);
						});
					});
				});
				o.StopFlameParticles();
				for (int i = 0; i < 4; i++)
				{
					Vector2 startingPos = p.GetPosition();
					startingPos.y = o.battleBounds.yMin;
					this.ACT_SINGLEBLAST1.StartAction(o, startingPos, bar, bar * 0.75f, bar * 0.25f, 0f, false);
					if (o.numberOfCharges > 4)
					{
						this.ACT_SINGLEBLAST2.StartAction(o, startingPos + Vector2.right * 2f, bar, bar * 0.75f, bar * 0.25f, 0f, true);
						this.ACT_SINGLEBLAST3.StartAction(o, startingPos + Vector2.left * 2f, bar, bar * 0.75f, bar * 0.25f, 0f, false);
					}
					if (o.numberOfCharges > 8)
					{
						this.ACT_SINGLEBLAST4.StartAction(o, startingPos + Vector2.right * 4f, bar, bar * 0.75f, bar * 0.25f, 0f, true);
						this.ACT_SINGLEBLAST5.StartAction(o, startingPos + Vector2.left * 4f, bar, bar * 0.75f, bar * 0.25f, 0f, false);
					}
					yield return this.ACT_SINGLEBLAST1.waitForCompletion;
					if (i == 2)
					{
						o.Isidora.Audio.SetIsidoraVoice(false);
					}
				}
				o.StopFlameParticles();
				anim.SetCasting(false);
				this.ACT_MOVE.StartAction(o, castingPosBottom, 3f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private IsidoraBehaviour.BlastsAttack_EnemyAction ACT_BLASTS = new IsidoraBehaviour.BlastsAttack_EnemyAction();

			private IsidoraBehaviour.BonfireChargeIsidoraAttack_EnemyAction ACT_BONFIRE = new IsidoraBehaviour.BonfireChargeIsidoraAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST1 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST2 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST3 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST4 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private IsidoraBehaviour.SingleBlastAttack_EnemyAction ACT_SINGLEBLAST5 = new IsidoraBehaviour.SingleBlastAttack_EnemyAction();

			private Tween xTween;

			private Tween yTween;
		}

		public class SingleBlastAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 startingPos, float waitAfterBlast, float anticipationTime = 1f, float activeTime = 1f, float delay = 0f, bool screenshake = false)
			{
				this.startingPos = startingPos;
				this.waitAfterBlast = waitAfterBlast;
				this.activeTime = activeTime;
				this.anticipationTime = anticipationTime;
				this.delay = delay;
				this.screenshake = screenshake;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_TELEPORT.StopAction();
				this.ACT_WAIT.StopAction();
				(this.owner as IsidoraBehaviour).Isidora.AnimatorInyector.ResetAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				IsidoraBehaviour o = this.owner as IsidoraBehaviour;
				if (this.delay > 0f)
				{
					this.ACT_WAIT.StartAction(o, this.delay);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				GameObject b = o.blastAttack.SummonAreaOnPoint(this.startingPos, 0f, 1f, null);
				BossSpawnedAreaAttack area = b.GetComponent<BossSpawnedAreaAttack>();
				area.SetCustomTimes(this.anticipationTime, this.activeTime);
				if (this.screenshake)
				{
					o.BlastScreenshake();
				}
				this.ACT_WAIT.StartAction(o, this.waitAfterBlast);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private Vector2 startingPos;

			private float waitAfterBlast;

			private float activeTime;

			private float anticipationTime;

			private float delay;

			private bool screenshake;

			private IsidoraBehaviour.Teleport_EnemyAction ACT_TELEPORT = new IsidoraBehaviour.Teleport_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		private class WaitUntilNextMarker : CustomYieldInstruction
		{
			public WaitUntilNextMarker(IsidoraAudio isidoraAudio)
			{
				this.finished = false;
				isidoraAudio.OnNextMarker += this.IsidoraAudio_OnNextMarker;
			}

			public override bool keepWaiting
			{
				get
				{
					return !this.finished;
				}
			}

			private void IsidoraAudio_OnNextMarker(IsidoraAudio obj)
			{
				obj.OnNextMarker -= this.IsidoraAudio_OnNextMarker;
				this.finished = true;
			}

			private bool finished;
		}

		private class WaitUntilNextAttackMarker : CustomYieldInstruction
		{
			public WaitUntilNextAttackMarker(IsidoraAudio isidoraAudio)
			{
				this.finished = false;
				isidoraAudio.OnAttackMarker += this.IsidoraAudio_OnNextAttackMarker;
			}

			public override bool keepWaiting
			{
				get
				{
					return !this.finished;
				}
			}

			private void IsidoraAudio_OnNextAttackMarker(IsidoraAudio obj)
			{
				obj.OnNextMarker -= this.IsidoraAudio_OnNextAttackMarker;
				this.finished = true;
			}

			private bool finished;
		}

		private class WaitUntilBarFinishes : CustomYieldInstruction
		{
			public WaitUntilBarFinishes(IsidoraAudio isidoraAudio)
			{
				this.finished = false;
				isidoraAudio.OnBarBegins += this.IsidoraAudio_OnBarBegins;
			}

			public override bool keepWaiting
			{
				get
				{
					return !this.finished;
				}
			}

			private void IsidoraAudio_OnBarBegins(IsidoraAudio obj)
			{
				obj.OnBarBegins -= this.IsidoraAudio_OnBarBegins;
				this.finished = true;
			}

			private bool finished;
		}

		private class WaitUntilNextValidBar : CustomYieldInstruction
		{
			public WaitUntilNextValidBar(IsidoraAudio isidoraAudio)
			{
				this.finished = false;
				isidoraAudio.OnBarBegins += this.IsidoraAudio_OnBarBegins;
			}

			public override bool keepWaiting
			{
				get
				{
					return !this.finished;
				}
			}

			private void IsidoraAudio_OnBarBegins(IsidoraAudio obj)
			{
				if (obj.IsLastBarValid())
				{
					obj.OnBarBegins -= this.IsidoraAudio_OnBarBegins;
					this.finished = true;
				}
			}

			private bool finished;
		}

		private class WaitUntilAnticipationPeriod : CustomYieldInstruction
		{
			public WaitUntilAnticipationPeriod(IsidoraAudio isidoraAudio)
			{
				this.finished = false;
				this.targetTime = isidoraAudio.GetTimeSinceLevelLoad() + isidoraAudio.GetTimeUntilNextAttackAnticipationPeriod();
				this.targetTime -= 0.1f;
				isidoraAudio.OnBarBegins += this.IsidoraAudio_OnBarBegins;
			}

			public override bool keepWaiting
			{
				get
				{
					return !this.finished;
				}
			}

			private void IsidoraAudio_OnBarBegins(IsidoraAudio obj)
			{
				if (obj.GetTimeSinceLevelLoad() >= this.targetTime)
				{
					obj.OnBarBegins -= this.IsidoraAudio_OnBarBegins;
					this.finished = true;
				}
			}

			private bool finished;

			public float targetTime;
		}
	}
}
