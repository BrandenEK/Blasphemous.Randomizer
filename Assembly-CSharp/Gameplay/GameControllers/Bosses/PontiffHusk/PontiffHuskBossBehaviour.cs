using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core.Surrogates;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Generic.Attacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using Tools.Audio;
using Tools.Level.Interactables;
using UnityEngine;
using UnityEngine.Playables;

namespace Gameplay.GameControllers.Bosses.PontiffHusk
{
	public class PontiffHuskBossBehaviour : EnemyBehaviour
	{
		public PontiffHuskBoss PontiffHuskBoss { get; set; }

		private void Start()
		{
			this.PontiffHuskBoss = (PontiffHuskBoss)this.Entity;
			this.InitAI();
			this.InitActionDictionary();
			this.InitCombatArea();
			this.currentFightParameters = this.allFightParameters[0];
			this.bossStartPos = base.transform.position;
			this.deathTrapLeftStartPos = this.DeathTrapLeft.transform.position;
			this.deathTrapRightStartPos = this.DeathTrapRight.transform.position;
			PoolManager.Instance.CreatePool(this.ExecutionPrefab, 1);
		}

		private void InitCombatArea()
		{
			this.combatAreaParent.SetParent(null);
			this.combatAreaParent.transform.position = this.battleBounds.center;
			this.CameraAnchor.transform.SetParent(null);
			this.CameraAnchor.ReferenceTransform = Camera.main.transform;
			Sequence sequence = DOTween.Sequence();
			sequence.AppendInterval(0.2f);
			sequence.OnStepComplete(delegate
			{
				this.battleBounds.center = this.combatAreaParent.transform.position;
			});
			sequence.SetLoops(-1);
			sequence.Play<Sequence>();
		}

		private void OnGUI()
		{
		}

		private void InitAI()
		{
			this.stIdle = new PontiffHuskBoss_StIdle();
			this.stAction = new PontiffHuskBoss_StAction();
			this._fsm = new StateMachine<PontiffHuskBossBehaviour>(this, this.stIdle, null, null);
		}

		private void InitActionDictionary()
		{
			this.waitBetweenActions_EA = new WaitSeconds_EnemyAction();
			this.intro_EA = new PontiffHuskBossBehaviour.Intro_EnemyAction();
			this.death_EA = new PontiffHuskBossBehaviour.Death_EnemyAction();
			this.blastsAttack_EA = new PontiffHuskBossBehaviour.BlastsAttack_EnemyAction();
			this.lasersAttack_EA = new PontiffHuskBossBehaviour.LasersAttack_EnemyAction();
			this.machinegunAttack_EA = new PontiffHuskBossBehaviour.MachinegunAttack_EnemyAction();
			this.bulletHellAttack_EA = new PontiffHuskBossBehaviour.BulletHellAttack_EnemyAction();
			this.simpleProjectileAttack_EA = new PontiffHuskBossBehaviour.SimpleProjectilesAttack_EnemyAction();
			this.windAttack_EA = new PontiffHuskBossBehaviour.WindAttack_EnemyAction();
			this.chargedBlastAttack_EA = new PontiffHuskBossBehaviour.ChargedBlastAttack_EnemyAction();
			this.spiralProjectilesAttack_EA = new PontiffHuskBossBehaviour.SpiralProjectilesAttack_EnemyAction();
			this.altSpiralProjectilesAttack_EA = new PontiffHuskBossBehaviour.AltSpiralProjectilesAttack_EnemyAction();
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.BLASTS_ATTACK, new Func<EnemyAction>(this.LaunchAction_BlastsAttack));
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.SIMPLE_PROJECTILES, new Func<EnemyAction>(this.LaunchAction_SimpleProjectileAttack));
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.WIND_ATTACK, new Func<EnemyAction>(this.LaunchAction_WindAttack));
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.CHARGED_BLAST, new Func<EnemyAction>(this.LaunchAction_ChargedBlastAttack));
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.SPIRAL_PROJECTILES, new Func<EnemyAction>(this.LaunchAction_SpiralProjectilesAttack));
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.LASERS_ATTACK, new Func<EnemyAction>(this.LaunchAction_LasersAttack));
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.MACHINEGUN_ATTACK, new Func<EnemyAction>(this.LaunchAction_MachinegunAttack));
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.BULLET_HELL_ATTACK, new Func<EnemyAction>(this.LaunchAction_BulletHellAttack));
			this.actionsDictionary.Add(PontiffHuskBossBehaviour.PH_ATTACKS.ALT_SPIRAL_PROJECTILES, new Func<EnemyAction>(this.LaunchAction_AltSpiralAttack));
			this.availableAttacks = this.attackConfigData.GetAttackIds(true, true, 1f);
		}

		public void Damage(Hit hit)
		{
			bool flag = this.SwapFightParametersIfNeeded();
			if (flag)
			{
				this.extraRecoverySeconds = 2f;
				float hpPercentage = this.PontiffHuskBoss.GetHpPercentage();
				if (hpPercentage < 0.33f)
				{
					Core.Dialog.StartConversation("DLG_BS203_04", false, false, true, 0, false);
				}
				else if (hpPercentage < 0.66f)
				{
					Core.Dialog.StartConversation("DLG_BS203_03", false, false, true, 0, false);
				}
				this.QueueAttack(PontiffHuskBossBehaviour.PH_ATTACKS.CHARGED_BLAST);
				if (UIController.instance.GetDialog().IsShowingDialog() && this.secondLastAttack != PontiffHuskBossBehaviour.PH_ATTACKS.DUMMY)
				{
					this.QueueAttack(this.secondLastAttack);
				}
			}
		}

		private void QueueAttack(PontiffHuskBossBehaviour.PH_ATTACKS atk)
		{
			this.queuedAttacks.Add(atk);
		}

		private PontiffHuskBossBehaviour.PH_ATTACKS PopAttackFromQueue()
		{
			PontiffHuskBossBehaviour.PH_ATTACKS ph_ATTACKS = PontiffHuskBossBehaviour.PH_ATTACKS.DUMMY;
			int num = this.queuedAttacks.Count - 1;
			if (num >= 0)
			{
				ph_ATTACKS = this.queuedAttacks[num];
				this.queuedAttacks.RemoveAt(num);
			}
			if (ph_ATTACKS == PontiffHuskBossBehaviour.PH_ATTACKS.DUMMY)
			{
				ph_ATTACKS = this.lastAttack;
			}
			return ph_ATTACKS;
		}

		private bool SwapFightParametersIfNeeded()
		{
			bool result = false;
			float hpPercentage = this.PontiffHuskBoss.GetHpPercentage();
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

		public void FlipOrientation()
		{
			EntityOrientation orientation = this.PontiffHuskBoss.Status.Orientation;
			EntityOrientation orientation2 = (orientation != EntityOrientation.Left) ? EntityOrientation.Left : EntityOrientation.Right;
			this.PontiffHuskBoss.SetOrientation(orientation2, true, false);
		}

		private Vector2 GetDirToPenitent()
		{
			return Core.Logic.Penitent.transform.position - base.transform.position;
		}

		private void InstantLookAtDir(Vector2 v)
		{
			this.PontiffHuskBoss.SetOrientation((v.x <= 0f) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
		}

		public void LookAtTarget()
		{
			this.LookAtDir(this.GetDirToPenitent());
		}

		private void LookAtDir(Vector2 v)
		{
			if (this.ShouldTurn(v))
			{
				this.PontiffHuskBoss.AnimatorInyector.PlayTurn();
			}
		}

		public bool ShouldTurnToLookAtTarget()
		{
			return this.ShouldTurn(this.GetDirToPenitent());
		}

		public bool ShouldTurn(Vector2 lookAtDir)
		{
			bool result = false;
			if (!this.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("TURN"))
			{
				bool flag = this.PontiffHuskBoss.Status.Orientation == EntityOrientation.Right;
				bool flag2 = lookAtDir.x > 0f;
				result = (flag != flag2);
			}
			return result;
		}

		public bool IsBossOnTheRightSide()
		{
			return this.PontiffHuskBoss.transform.position.x > this.battleBounds.center.x;
		}

		public Vector2 ArenaGetBotRightCorner()
		{
			return new Vector2(this.battleBounds.xMax, this.battleBounds.yMin);
		}

		public Vector2 ArenaGetBotLeftCorner()
		{
			return new Vector2(this.battleBounds.xMin, this.battleBounds.yMin);
		}

		public Vector2 ArenaGetTopRightCorner()
		{
			return new Vector2(this.battleBounds.xMax, this.battleBounds.yMax);
		}

		public Vector2 ArenaGetTopLeftCorner()
		{
			return new Vector2(this.battleBounds.xMin, this.battleBounds.yMax);
		}

		public Vector2 ArenaGetBotFarRandomPoint()
		{
			Vector2 zero = Vector2.zero;
			zero.y = this.battleBounds.yMin;
			if (this.IsBossOnTheRightSide())
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.3f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.5f));
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
			if (this.IsBossOnTheRightSide())
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.5f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 1f));
			}
			else
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.2f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.5f));
			}
			return zero;
		}

		public Vector2 ArenaGetTopFarRandomPoint()
		{
			Vector2 zero = Vector2.zero;
			zero.y = this.battleBounds.yMax;
			if (this.IsBossOnTheRightSide())
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.3f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.5f));
			}
			else
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.6f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.9f));
			}
			return zero;
		}

		public Vector2 ArenaGetTopNearRandomPoint()
		{
			Vector2 zero = Vector2.zero;
			zero.y = this.battleBounds.yMax;
			if (this.IsBossOnTheRightSide())
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.5f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 1f));
			}
			else
			{
				zero.x = UnityEngine.Random.Range(Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.2f), Mathf.Lerp(this.battleBounds.xMin, this.battleBounds.xMax, 0.5f));
			}
			return zero;
		}

		private WaypointsMovingPlatform GetPlatformInDirection(Vector2 direction, Vector2 position)
		{
			WaypointsMovingPlatform waypointsMovingPlatform = null;
			float num = 20f;
			float num2 = 0.5f;
			float num3 = 3f;
			if (Physics2D.RaycastNonAlloc(position, direction, this.results, num, this.FloorMask) > 0)
			{
				GizmoExtensions.DrawDebugCross(this.results[0].transform.position, Color.yellow, 10f);
				if (this.results[0].transform.position.x + num3 < this.battleBounds.xMax && this.results[0].transform.position.x - num2 > this.battleBounds.xMin && this.results[0].transform.position.y > this.battleBounds.yMin)
				{
					waypointsMovingPlatform = this.results[0].transform.gameObject.GetComponent<WaypointsMovingPlatform>();
					GizmoExtensions.DrawDebugCross(waypointsMovingPlatform.transform.position, Color.green, 10f);
				}
			}
			else
			{
				GizmoExtensions.DrawDebugCross(position + direction * num, Color.red, 10f);
			}
			return waypointsMovingPlatform;
		}

		private void Update()
		{
			this._fsm.DoUpdate();
		}

		private void LaunchAutomaticAction()
		{
			List<PontiffHuskBossBehaviour.PH_ATTACKS> filteredAttacks = this.GetFilteredAttacks(this.availableAttacks);
			PontiffHuskBossBehaviour.PH_ATTACKS action;
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
				action = PontiffHuskBossBehaviour.PH_ATTACKS.LASERS_ATTACK;
			}
			this.LaunchAction(action);
			this.secondLastAttack = this.lastAttack;
			this.lastAttack = action;
		}

		private List<PontiffHuskBossBehaviour.PH_ATTACKS> GetFilteredAttacks(List<PontiffHuskBossBehaviour.PH_ATTACKS> originalList)
		{
			List<PontiffHuskBossBehaviour.PH_ATTACKS> list = new List<PontiffHuskBossBehaviour.PH_ATTACKS>(originalList);
			PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig atkConfig = this.attackConfigData.GetAttackConfig(this.lastAttack);
			if (atkConfig.cantBeFollowedBy != null && atkConfig.cantBeFollowedBy.Count > 0)
			{
				list.RemoveAll((PontiffHuskBossBehaviour.PH_ATTACKS x) => atkConfig.cantBeFollowedBy.Contains(x));
			}
			if (atkConfig.alwaysFollowedBy != null && atkConfig.alwaysFollowedBy.Count > 0)
			{
				list.RemoveAll((PontiffHuskBossBehaviour.PH_ATTACKS x) => !atkConfig.alwaysFollowedBy.Contains(x));
			}
			if (base.transform.position.x < this.battleBounds.center.x)
			{
				list.Remove(PontiffHuskBossBehaviour.PH_ATTACKS.MACHINEGUN_ATTACK);
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

		private int RandomizeUsingWeights(List<PontiffHuskBossBehaviour.PH_ATTACKS> filteredAtks)
		{
			float hpPercentage = this.PontiffHuskBoss.GetHpPercentage();
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

		protected void LaunchAction(PontiffHuskBossBehaviour.PH_ATTACKS action)
		{
			this.StopCurrentAction();
			this._fsm.ChangeState(this.stAction);
			this.currentAction = this.actionsDictionary[action]();
			this.currentAction.OnActionEnds -= this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped -= this.CurrentAction_OnActionStops;
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped += this.CurrentAction_OnActionStops;
		}

		public void StartIntro()
		{
			this.StopCurrentAction();
			this._fsm.ChangeState(this.stAction);
			this.currentAction = this.intro_EA.StartAction(this);
			this.QueueAttack(PontiffHuskBossBehaviour.PH_ATTACKS.SPIRAL_PROJECTILES);
			this.currentAction.OnActionEnds -= this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped -= this.CurrentAction_OnActionStops;
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped += this.CurrentAction_OnActionStops;
		}

		public void ResetCombat()
		{
			this.StopCurrentAction();
			if (this.sceneAudio == null)
			{
				this.sceneAudio = UnityEngine.Object.FindObjectOfType<SceneAudio>();
			}
			if (this.PontiffHuskBoss.Status.Dead)
			{
				this.sceneAudio.RestartSceneAudio();
			}
			this.PontiffHuskBoss.Stats.Life.SetToCurrentMax();
			this.PontiffHuskBoss.Status.Dead = false;
			this.availableAttacks = this.attackConfigData.GetAttackIds(true, true, 1f);
			this.currentFightParameters = this.allFightParameters[0];
			this.queuedAttacks.Clear();
			this.QueueAttack(PontiffHuskBossBehaviour.PH_ATTACKS.SPIRAL_PROJECTILES);
			this.PontiffHuskBoss.AnimatorInyector.PlayHide();
			Core.Audio.Ambient.SetSceneParam("Ending", 0f);
			if (this.firstTimeDialogDone)
			{
				Core.Audio.Ambient.SetSceneParam("Combat", 1f);
			}
			this.PontiffHuskBoss.Audio.StopAmbientPostCombat();
			this.ChargedBlastAttack.ClearAll();
			if (this.CrisantaProtectorInstance != null)
			{
				this.CrisantaProtectorInstance.SetActive(false);
				this.CrisantaProtectorInstance = null;
			}
			if (this.executionInstance != null)
			{
				this.executionInstance.SetActive(false);
				this.executionInstance = null;
			}
			this.InstantLookAtDir(Vector2.left);
		}

		private int GetAttackNumberOfRepetitions(PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig config)
		{
			float hpPercentage = this.PontiffHuskBoss.GetHpPercentage();
			if (hpPercentage > 0.66f)
			{
				return config.repetitions1;
			}
			if (hpPercentage > 0.33f)
			{
				return config.repetitions2;
			}
			return config.repetitions3;
		}

		protected void LaunchAction_Death()
		{
			this.currentAction = this.death_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_BlastsAttack()
		{
			Vector3 position = Core.Logic.Penitent.GetPosition();
			position.y = this.battleBounds.yMax;
			return this.blastsAttack_EA.StartAction(this, position, 2f, 2, 2f);
		}

		protected EnemyAction LaunchAction_LasersAttack()
		{
			return this.lasersAttack_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_MachinegunAttack()
		{
			return this.machinegunAttack_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_BulletHellAttack()
		{
			return this.bulletHellAttack_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_SimpleProjectileAttack()
		{
			PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig attackConfig = this.attackConfigData.GetAttackConfig(PontiffHuskBossBehaviour.PH_ATTACKS.SIMPLE_PROJECTILES);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.simpleProjectileAttack_EA.StartAction(this, 1f, attackNumberOfRepetitions);
		}

		protected EnemyAction LaunchAction_WindAttack()
		{
			return this.windAttack_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_AltSpiralAttack()
		{
			return this.altSpiralProjectilesAttack_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_ChargedBlastAttack()
		{
			return this.chargedBlastAttack_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_SpiralProjectilesAttack()
		{
			PontiffHuskBossScriptableConfig.PontiffHuskBossAttackConfig attackConfig = this.attackConfigData.GetAttackConfig(PontiffHuskBossBehaviour.PH_ATTACKS.SPIRAL_PROJECTILES);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.spiralProjectilesAttack_EA.StartAction(this, attackNumberOfRepetitions);
		}

		private void CurrentAction_OnActionStops(EnemyAction e)
		{
		}

		private void CurrentAction_OnActionEnds(EnemyAction e)
		{
			e.OnActionEnds -= this.CurrentAction_OnActionEnds;
			e.OnActionIsStopped -= this.CurrentAction_OnActionStops;
			if (e != this.waitBetweenActions_EA)
			{
				if (!this.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE") && !this.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE TO HIDE") && !this.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE TO IDLE") && this.ShouldTurnToLookAtTarget())
				{
					this.PontiffHuskBoss.AnimatorInyector.PlayHide();
				}
				this.WaitBetweenActions();
			}
			else if (Core.Logic.Penitent.GetPosition().y < this.battleBounds.yMin || Core.Logic.Penitent.GetPosition().x < this.battleBounds.xMin || Core.Logic.Penitent.GetPosition().x > this.battleBounds.xMax)
			{
				this.PontiffHuskBoss.AnimatorInyector.PlayHide();
				this.InstantLookAtDir(Vector2.left);
				this.WaitBetweenActions();
			}
			else
			{
				this.LaunchAutomaticAction();
			}
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
			this.currentAction.OnActionEnds -= this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			base.transform.DOMoveX(base.transform.position.x + min, min, false).SetEase(Ease.InOutQuad);
			this.PontiffHuskBoss.AnimatorInyector.ResetAll();
		}

		private void CheckDebugActions()
		{
			Dictionary<KeyCode, PontiffHuskBossBehaviour.PH_ATTACKS> debugActions = this.attackConfigData.debugActions;
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
			this.PontiffHuskBoss.DamageArea.DamageAreaCollider.enabled = b;
		}

		public void DoActivateGuard(bool b)
		{
			this.PontiffHuskBoss.IsGuarding = b;
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

		[FoldoutGroup("Character settings", 0)]
		public AnimationCurve timeSlowCurve;

		[FoldoutGroup("Battle area", 0)]
		public Rect battleBounds;

		[FoldoutGroup("Battle area", 0)]
		public Transform combatAreaParent;

		[FoldoutGroup("Battle area", 0)]
		public PontiffHuskBossAnchor CameraAnchor;

		[FoldoutGroup("Battle config", 0)]
		public List<PontiffHuskBossBehaviour.PontiffHuskBossFightParameters> allFightParameters;

		[FoldoutGroup("Attacks config", 0)]
		public PontiffHuskBossScriptableConfig attackConfigData;

		[FoldoutGroup("Debug", 0)]
		public bool debugDrawCurrentAction;

		[FoldoutGroup("Attack references", 0)]
		public BossStraightProjectileAttack AccProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		public BossAreaSummonAttack ChargedBlastAttack;

		[FoldoutGroup("Attack references", 0)]
		public BossSpiralProjectiles SpiralProjectilesAttack;

		[FoldoutGroup("Attack references", 0)]
		public BossSpiralProjectiles WindSpiralProjectilesAttack;

		[FoldoutGroup("Attack references", 0)]
		public BossMachinegunShooter MachinegunShooter;

		[FoldoutGroup("Attack references", 0)]
		public GameObject CrisantaProtectorPrefabFT;

		[FoldoutGroup("Attack references", 0)]
		public GameObject CrisantaProtectorPrefab;

		[FoldoutGroup("Attack references", 0)]
		public List<BossHomingLaserAttack> LaserAttacks;

		[FoldoutGroup("Attack references", 0)]
		public List<Transform> TransformsForLasers;

		[FoldoutGroup("Attack references", 0)]
		public List<Transform> BulletHellConfigs;

		[FoldoutGroup("Attack references", 0)]
		public GameObject ChargedBlastExplosion;

		[FoldoutGroup("Ending references", 0)]
		public GameObject ExecutionPrefab;

		[FoldoutGroup("Ending references", 0)]
		public GameObject HwNpc;

		[FoldoutGroup("Ending references", 0)]
		public PontiffHuskBossScrollManager PontiffHuskBossScrollManager;

		[FoldoutGroup("Ending references", 0)]
		public LayerMask FloorMask;

		[FoldoutGroup("Ending references", 0)]
		public GameObject DeathTrapLeft;

		[FoldoutGroup("Ending references", 0)]
		public GameObject DeathTrapRight;

		[FoldoutGroup("Ending references", 0)]
		public GameObject HWDialoguePlatform;

		[FoldoutGroup("Ending references", 0)]
		[EventRef]
		public string SummaBlasphemiaSfx = "event:/Key Event/BossBattleEnd";

		private List<PontiffHuskBossBehaviour.PH_ATTACKS> availableAttacks = new List<PontiffHuskBossBehaviour.PH_ATTACKS>();

		[ShowInInspector]
		private List<PontiffHuskBossBehaviour.PH_ATTACKS> queuedAttacks = new List<PontiffHuskBossBehaviour.PH_ATTACKS>();

		private PontiffHuskBossBehaviour.PontiffHuskBossFightParameters currentFightParameters;

		private EnemyAction currentAction;

		private PontiffHuskBossBehaviour.PH_ATTACKS lastAttack = PontiffHuskBossBehaviour.PH_ATTACKS.DUMMY;

		private PontiffHuskBossBehaviour.PH_ATTACKS secondLastAttack = PontiffHuskBossBehaviour.PH_ATTACKS.DUMMY;

		private Dictionary<PontiffHuskBossBehaviour.PH_ATTACKS, Func<EnemyAction>> actionsDictionary = new Dictionary<PontiffHuskBossBehaviour.PH_ATTACKS, Func<EnemyAction>>();

		private float extraRecoverySeconds;

		private PontiffHuskBossMeleeAttack currentMeleeAttack;

		private RaycastHit2D[] results = new RaycastHit2D[1];

		private Vector2 bossStartPos;

		private Vector2 deathTrapLeftStartPos;

		private Vector2 deathTrapRightStartPos;

		private GameObject executionInstance;

		[HideInInspector]
		public GameObject CrisantaProtectorInstance;

		private bool firstTimeDialogDone;

		private SceneAudio sceneAudio;

		private WaitSeconds_EnemyAction waitBetweenActions_EA;

		private PontiffHuskBossBehaviour.BlastsAttack_EnemyAction blastsAttack_EA;

		private PontiffHuskBossBehaviour.LasersAttack_EnemyAction lasersAttack_EA;

		private PontiffHuskBossBehaviour.MachinegunAttack_EnemyAction machinegunAttack_EA;

		private PontiffHuskBossBehaviour.BulletHellAttack_EnemyAction bulletHellAttack_EA;

		private PontiffHuskBossBehaviour.SimpleProjectilesAttack_EnemyAction simpleProjectileAttack_EA;

		private PontiffHuskBossBehaviour.WindAttack_EnemyAction windAttack_EA;

		private PontiffHuskBossBehaviour.ChargedBlastAttack_EnemyAction chargedBlastAttack_EA;

		private PontiffHuskBossBehaviour.SpiralProjectilesAttack_EnemyAction spiralProjectilesAttack_EA;

		private PontiffHuskBossBehaviour.AltSpiralProjectilesAttack_EnemyAction altSpiralProjectilesAttack_EA;

		private PontiffHuskBossBehaviour.Intro_EnemyAction intro_EA;

		private PontiffHuskBossBehaviour.Death_EnemyAction death_EA;

		private StateMachine<PontiffHuskBossBehaviour> _fsm;

		private State<PontiffHuskBossBehaviour> stIdle;

		private State<PontiffHuskBossBehaviour> stAction;

		[Serializable]
		public struct PontiffHuskBossFightParameters
		{
			[SuffixLabel("%", false)]
			public float hpPercentageBeforeApply;

			[MinMaxSlider(0f, 5f, true)]
			public Vector2 minMaxWaitingTimeBetweenActions;
		}

		public enum PH_ATTACKS
		{
			BLASTS_ATTACK,
			SIMPLE_PROJECTILES,
			WIND_ATTACK,
			CHARGED_BLAST,
			SPIRAL_PROJECTILES,
			LASERS_ATTACK,
			MACHINEGUN_ATTACK,
			BULLET_HELL_ATTACK,
			ALT_SPIRAL_PROJECTILES,
			DUMMY = 999
		}

		public class Intro_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				o.PontiffHuskBoss.AnimatorInyector.StopHide();
				o.transform.DOKill(false);
				o.InstantLookAtDir(Vector3.left);
				o.transform.position = o.bossStartPos;
				o.DeathTrapLeft.transform.position = o.deathTrapLeftStartPos;
				o.DeathTrapRight.transform.position = o.deathTrapRightStartPos;
				o.DeathTrapLeft.SetActive(true);
				o.DeathTrapRight.SetActive(true);
				o.currentFightParameters = o.allFightParameters[0];
				this.ACT_MOVE.StartAction(o, o.deathTrapLeftStartPos + Vector2.right, 1f, Ease.InOutQuad, o.DeathTrapLeft.transform, true, null, true, true, 1.7f);
				this.ACT_MOVE.StartAction(o, o.deathTrapRightStartPos + Vector2.left, 1f, Ease.InOutQuad, o.DeathTrapRight.transform, true, null, true, true, 1.7f);
				this.ACT_MOVE.StartAction(o, o.bossStartPos, 2f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				if (!o.firstTimeDialogDone)
				{
					o.firstTimeDialogDone = true;
					Core.Input.SetBlocker("HW_DIALOG_TIME", true);
					Core.Dialog.StartConversation("DLG_BS203_01", false, false, false, 0, false);
					yield return new WaitUntil(() => !UIController.instance.GetDialog().IsShowingDialog());
					o.PontiffHuskBoss.Audio.StartCombatMusic();
					Core.Dialog.StartConversation("DLG_BS203_06", false, false, true, 0, false);
					yield return new WaitUntil(() => !UIController.instance.GetDialog().IsShowingDialog());
					Core.Input.SetBlocker("HW_DIALOG_TIME", false);
				}
				else
				{
					o.PontiffHuskBoss.Audio.StartCombatMusic();
				}
				this.ACT_MOVE.StartAction(o, o.bossStartPos, 1f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Core.Dialog.StartConversation("DLG_BS203_02", false, false, true, 0, false);
				o.PontiffHuskBossScrollManager.ActivateModules();
				this.ACT_MOVE.StartAction(o, o.bossStartPos, 1f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class Death_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				if (pontiffHuskBossBehaviour.executionInstance != null)
				{
					pontiffHuskBossBehaviour.executionInstance.SetActive(false);
					pontiffHuskBossBehaviour.executionInstance = null;
					pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.EntityAnimator.Play("HIDE");
					pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.PlayHide();
				}
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				Core.Audio.Ambient.SetSceneParam("Ending", 1f);
				o.PontiffHuskBoss.Audio.PlayAmbientPostCombat();
				o.PontiffHuskBoss.AnimatorInyector.StopHide();
				o.PontiffHuskBoss.AnimatorInyector.PlayDeath();
				o.ChargedBlastAttack.ClearAll();
				o.PontiffHuskBossScrollManager.Stop();
				WaypointsMovingPlatform platform = o.GetPlatformInDirection(Vector2.down, o.transform.position);
				if (platform == null)
				{
					for (int i = 1; i < 21; i++)
					{
						platform = o.GetPlatformInDirection(Vector2.down + Vector2.right * 0.1f * (float)i, o.transform.position);
						if (platform != null)
						{
							break;
						}
					}
				}
				if (platform == null)
				{
					for (int j = 1; j < 21; j++)
					{
						platform = o.GetPlatformInDirection(Vector2.down + Vector2.left * 0.1f * (float)j, o.transform.position);
						if (platform != null)
						{
							break;
						}
					}
				}
				Vector2 baseExecutionPos;
				if (platform != null)
				{
					baseExecutionPos = ((platform.GetDestination().y <= platform.GetOrigin().y) ? platform.GetOrigin() : platform.GetDestination());
					Vector2 executionPos = baseExecutionPos + Vector2.up * 6.5f + Vector2.right * 4f;
					Vector2 moveDir = executionPos - o.transform.position;
					this.ACT_MOVE.StartAction(o, executionPos, 3f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				else
				{
					baseExecutionPos = o.transform.position;
					this.ACT_WAIT.StartAction(o, 3f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				yield return new WaitUntil(() => !UIController.instance.GetDialog().IsShowingDialog());
				yield return new WaitUntil(() => p.Status.IsGrounded);
				Core.Dialog.StartConversation("DLG_BS203_05", false, false, true, 0, false);
				o.PontiffHuskBoss.AnimatorInyector.StopDeath();
				o.executionInstance = PoolManager.Instance.ReuseObject(o.ExecutionPrefab, o.transform.position, o.transform.rotation, true, 1).GameObject;
				o.PontiffHuskBoss.DamageArea.enabled = false;
				FakeExecution execution = o.executionInstance.GetComponent<FakeExecution>();
				yield return new WaitUntil(() => execution.BeingUsed);
				o.PontiffHuskBoss.Audio.PlayExecution();
				Core.UI.ShowGamePlayUI = false;
				o.PontiffHuskBossScrollManager.SetExecutionCamBounds();
				this.ACT_MOVE.StartAction(o, o.DeathTrapLeft.transform.position + Vector3.left * 100f, 3f, Ease.InOutQuad, o.DeathTrapLeft.transform, true, null, true, true, 1.7f);
				this.ACT_MOVE.StartAction(o, o.DeathTrapRight.transform.position + Vector3.right * 100f, 3f, Ease.InOutQuad, o.DeathTrapRight.transform, true, null, true, true, 1.7f);
				p.Shadow.ManuallyControllingAlpha = true;
				Tween t = DOTween.To(() => p.Shadow.GetShadowAlpha(), delegate(float x)
				{
					p.Shadow.SetShadowAlpha(x);
				}, 0f, 0.2f);
				yield return new WaitUntil(() => !execution.BeingUsed);
				Core.Input.SetBlocker("END_BOSS_DEFEATED", true);
				p.SetOrientation(EntityOrientation.Right, true, false);
				Tween t2 = DOTween.To(() => p.Shadow.GetShadowAlpha(), delegate(float x)
				{
					p.Shadow.SetShadowAlpha(x);
				}, 1f, 0.2f);
				t2.OnComplete(delegate
				{
					p.Shadow.ManuallyControllingAlpha = false;
				});
				this.ACT_WAIT.StartAction(o, 1.75f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Audio.PlaySfx(o.SummaBlasphemiaSfx, 0f);
				yield return UIController.instance.ShowFullMessageCourrutine(UIController.FullMensages.EndBossDefeated, 3f, 3f, 2f);
				Core.Input.SetBlocker("END_BOSS_DEFEATED", false);
				PlayMakerFSM.BroadcastEvent("BOSS DEAD");
				o.HWDialoguePlatform.transform.position = baseExecutionPos + Vector2.down * 7f + Vector2.right * 11.5f;
				Vector3 hwDialoguePlatformTargetPos = o.HWDialoguePlatform.transform.position + Vector3.up * 9f;
				float platformWait = 3f;
				Tween t3 = DOTween.To(() => o.HWDialoguePlatform.transform.position, delegate(Vector3Wrapper x)
				{
					o.HWDialoguePlatform.transform.position = x;
				}, hwDialoguePlatformTargetPos, platformWait);
				t3.SetEase(Ease.InOutQuad);
				Core.Input.SetBlocker("HW_FINAL_PLATFORM_RISING", true);
				Sequence s = DOTween.Sequence();
				s.AppendInterval(platformWait);
				s.OnComplete(delegate
				{
					Core.Input.SetBlocker("HW_FINAL_PLATFORM_RISING", false);
				});
				s.Play<Sequence>();
				Vector2 hwnpcPos = new Vector2(p.GetPosition().x + o.battleBounds.width * 2f, o.HwNpc.transform.position.y);
				o.HwNpc.transform.position = hwnpcPos + Vector2.right * o.battleBounds.width * 1.1f;
				o.DeathTrapLeft.SetActive(false);
				o.DeathTrapRight.SetActive(false);
				base.FinishAction();
				o.gameObject.SetActive(false);
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class BlastsAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 startingPos, float waitTime, int totalAreas = -1, float distanceBetweenAreas = -1f)
			{
				this.startingPos = startingPos;
				this.waitTime = waitTime;
				this.totalAreas = totalAreas;
				this.distanceBetweenAreas = distanceBetweenAreas;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				float moveTime = 2f;
				Vector2 target = (Vector2.Distance(p.GetPosition(), o.transform.position) >= o.battleBounds.width / 2f) ? o.ArenaGetTopNearRandomPoint() : o.ArenaGetTopFarRandomPoint();
				this.ACT_MOVE.StartAction(o, target, moveTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				this.ACT_WAIT.StartAction(o, moveTime * 0.9f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.LookAtTarget();
				yield return this.ACT_MOVE.waitForCompletion;
				Core.Dialog.StartConversation("DLG_HW_PLACEHOLDER", false, false, true, 0, false);
				int numSteps = 2;
				for (int i = 0; i < numSteps; i++)
				{
					Vector2 dir = (i % 2 != 0) ? Vector2.down : Vector2.up;
					o.LookAtTarget();
					this.ACT_MOVE.StartAction(o, target + dir, this.waitTime / (float)numSteps, Ease.InOutQuad, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private Vector2 startingPos;

			private float waitTime;

			private int totalAreas;

			private float distanceBetweenAreas;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class LasersAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.LaserAttacks.ForEach(delegate(BossHomingLaserAttack x)
				{
					x.StopBeam();
				});
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				o.PontiffHuskBoss.AnimatorInyector.PlayHide();
				for (int i = 0; i < o.LaserAttacks.Count; i++)
				{
					Transform target2 = o.TransformsForLasers[i];
					o.LaserAttacks[i].DelayedTargetedBeam(target2, 1f, 3f, EntityOrientation.Right, false);
				}
				this.ACT_WAIT.StartAction(o, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector2 target = o.ArenaGetTopFarRandomPoint();
				o.transform.position = target;
				this.ACT_WAIT.StartAction(o, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class MachinegunAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.MachinegunShooter.StopMachinegun();
				pontiffHuskBossBehaviour.ClearAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				o.PontiffHuskBoss.AnimatorInyector.PlayHide();
				this.ACT_WAIT.StartAction(o, 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector2 target = o.ArenaGetTopLeftCorner() + Vector2.right;
				o.transform.position = target;
				o.InstantLookAtDir(Vector2.right);
				o.PontiffHuskBoss.AnimatorInyector.StopHide();
				o.PontiffHuskBoss.AnimatorInyector.PlayAltShoot();
				float waitTime = 1.75f;
				o.transform.parent = o.CameraAnchor.transform;
				this.ACT_WAIT.StartAction(o, waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.MachinegunShooter.StartAttack(Core.Logic.Penitent.transform);
				o.MachinegunShooter.transform.SetParent(o.transform);
				float attackTime = 6.5f;
				this.ACT_MOVE.StartAction(o, o.transform.position + Vector3.up, attackTime * 0.5f, Ease.InOutQuad, null, true, null, false, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				this.ACT_MOVE.StartAction(o, o.transform.position + Vector3.down, attackTime * 0.5f, Ease.InOutQuad, null, true, null, false, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				o.PontiffHuskBoss.AnimatorInyector.StopAltShoot();
				waitTime = 1f;
				this.ACT_WAIT.StartAction(o, waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.transform.parent = null;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class BulletHellAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_LONGWAIT.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.ClearAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour.BulletHellAttack_EnemyAction.<BaseCoroutine>c__Iterator0.<BaseCoroutine>c__AnonStorey1 <BaseCoroutine>c__AnonStorey = new PontiffHuskBossBehaviour.BulletHellAttack_EnemyAction.<BaseCoroutine>c__Iterator0.<BaseCoroutine>c__AnonStorey1();
				<BaseCoroutine>c__AnonStorey.<>f__ref$0 = this;
				<BaseCoroutine>c__AnonStorey.o = (this.owner as PontiffHuskBossBehaviour);
				Penitent p = Core.Logic.Penitent;
				bool goingToHide = <BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE TO HIDE") || <BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.GetHide();
				bool isHiding = <BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE");
				bool wasHiding = <BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE TO IDLE");
				if (goingToHide || isHiding || wasHiding)
				{
					if (goingToHide)
					{
						yield return new WaitUntil(() => <BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE"));
					}
					if (goingToHide || isHiding)
					{
						<BaseCoroutine>c__AnonStorey.o.transform.position = <BaseCoroutine>c__AnonStorey.o.ArenaGetTopRightCorner();
						<BaseCoroutine>c__AnonStorey.o.InstantLookAtDir(Vector2.left);
					}
					<BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.StopHide();
					yield return new WaitUntil(() => <BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE"));
				}
				<BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.PlayCast();
				Vector2 target = new Vector2(<BaseCoroutine>c__AnonStorey.o.battleBounds.center.x, <BaseCoroutine>c__AnonStorey.o.battleBounds.yMax);
				float moveTime = Vector2.Distance(target, <BaseCoroutine>c__AnonStorey.o.transform.position) * 0.1f + 0.2f;
				target.x += moveTime;
				this.ACT_MOVE.StartAction(<BaseCoroutine>c__AnonStorey.o, target, moveTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				int numLoops = 3;
				int numWavesPerLoop = 5;
				float waitTime = 0.6f;
				float shootTime = waitTime * (float)numLoops * (float)numWavesPerLoop + waitTime * 2f * (float)(numLoops + 1);
				this.ACT_LONGWAIT.StartAction(<BaseCoroutine>c__AnonStorey.o, shootTime);
				<BaseCoroutine>c__AnonStorey.o.transform.parent = <BaseCoroutine>c__AnonStorey.o.CameraAnchor.transform;
				this.ACT_WAIT.StartAction(<BaseCoroutine>c__AnonStorey.o, waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector3 orientationOffset = (<BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.Status.Orientation != EntityOrientation.Left) ? (Vector3.right * 1.4f) : Vector3.zero;
				<BaseCoroutine>c__AnonStorey.acc = 4f;
				List<AcceleratedProjectile> lastProjectiles = new List<AcceleratedProjectile>();
				Vector3 rotation = Vector3.zero;
				Transform config = <BaseCoroutine>c__AnonStorey.o.BulletHellConfigs[0];
				float angleIncrement = 5f;
				config.rotation = Quaternion.Euler(Vector3.zero);
				for (int i = 0; i < 3; i++)
				{
					float rotationDir = (i % 2 != 0) ? -1f : 1f;
					for (int j = 1; j < 6; j++)
					{
						PontiffHuskBossBehaviour.BulletHellAttack_EnemyAction.<BaseCoroutine>c__Iterator0.<BaseCoroutine>c__AnonStorey2 <BaseCoroutine>c__AnonStorey2 = new PontiffHuskBossBehaviour.BulletHellAttack_EnemyAction.<BaseCoroutine>c__Iterator0.<BaseCoroutine>c__AnonStorey2();
						<BaseCoroutine>c__AnonStorey2.<>f__ref$0 = this;
						<BaseCoroutine>c__AnonStorey2.<>f__ref$1 = <BaseCoroutine>c__AnonStorey;
						rotation.z += angleIncrement * (float)j * rotationDir;
						config.DORotate(rotation, waitTime, RotateMode.Fast);
						<BaseCoroutine>c__AnonStorey2.origin = config.position + orientationOffset;
						for (int k = 0; k < config.childCount; k++)
						{
							Vector3 normalized = (config.GetChild(k).position - config.position).normalized;
							StraightProjectile straightProjectile = <BaseCoroutine>c__AnonStorey.o.AccProjectileAttack.Shoot(normalized, <BaseCoroutine>c__AnonStorey2.origin, normalized, 1f);
							AcceleratedProjectile component = straightProjectile.GetComponent<AcceleratedProjectile>();
							lastProjectiles.Add(component);
							straightProjectile.transform.parent = <BaseCoroutine>c__AnonStorey.o.combatAreaParent;
						}
						this.ACT_WAIT.StartAction(<BaseCoroutine>c__AnonStorey.o, waitTime);
						yield return this.ACT_WAIT.waitForCompletion;
						PontiffHuskBossBehaviour.BulletHellAttack_EnemyAction.<BaseCoroutine>c__Iterator0.<BaseCoroutine>c__AnonStorey2 <BaseCoroutine>c__AnonStorey3 = <BaseCoroutine>c__AnonStorey2;
						<BaseCoroutine>c__AnonStorey3.origin.x = <BaseCoroutine>c__AnonStorey3.origin.x + waitTime;
						lastProjectiles.ForEach(delegate(AcceleratedProjectile x)
						{
							x.SetAcceleration((x.transform.position - <BaseCoroutine>c__AnonStorey2.origin).normalized * <BaseCoroutine>c__AnonStorey2.<>f__ref$1.acc);
						});
						lastProjectiles.Clear();
					}
					this.ACT_WAIT.StartAction(<BaseCoroutine>c__AnonStorey.o, waitTime * 2f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				yield return this.ACT_LONGWAIT.waitForCompletion;
				waitTime = 1f;
				this.ACT_WAIT.StartAction(<BaseCoroutine>c__AnonStorey.o, waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				<BaseCoroutine>c__AnonStorey.o.PontiffHuskBoss.AnimatorInyector.StopCast();
				this.ACT_WAIT.StartAction(<BaseCoroutine>c__AnonStorey.o, waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				<BaseCoroutine>c__AnonStorey.o.transform.parent = null;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private WaitSeconds_EnemyAction ACT_LONGWAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class SimpleProjectilesAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float chargeTime, int numProjectiles)
			{
				this.chargeTime = chargeTime;
				this.numProjectiles = numProjectiles;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				float moveTime = 2f;
				Vector2 target = (Vector2.Distance(p.GetPosition(), o.transform.position) >= o.battleBounds.width / 2f) ? o.ArenaGetTopNearRandomPoint() : o.ArenaGetTopFarRandomPoint();
				target.y += 1.5f;
				this.ACT_MOVE.StartAction(o, target, moveTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				this.ACT_WAIT.StartAction(o, moveTime * 0.9f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.LookAtTarget();
				yield return this.ACT_MOVE.waitForCompletion;
				o.PontiffHuskBoss.AnimatorInyector.PlayCharge();
				this.ACT_WAIT.StartAction(o, this.chargeTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.PontiffHuskBoss.AnimatorInyector.PlayShoot();
				float timeBetweenProjectiles = 0.5f;
				o.transform.parent = o.CameraAnchor.transform;
				this.ACT_WAIT.StartAction(o, timeBetweenProjectiles);
				yield return this.ACT_WAIT.waitForCompletion;
				for (int i = 0; i < this.numProjectiles; i++)
				{
					Vector3 shootPointOffset = Vector3.down * 4f;
					shootPointOffset.x = ((o.PontiffHuskBoss.Status.Orientation != EntityOrientation.Right) ? -3f : 3f);
					Vector2 dir = (Core.Logic.Penitent.GetPosition() - (o.transform.position + shootPointOffset)).normalized;
					dir.x = ((o.PontiffHuskBoss.Status.Orientation != EntityOrientation.Right) ? Mathf.Min(-0.3f, dir.x) : Mathf.Max(0.3f, dir.x));
					dir.y += (float)i * 0.1f;
					StraightProjectile projectile = o.AccProjectileAttack.Shoot(dir, shootPointOffset, 1f);
					AcceleratedProjectile accelProjectile = projectile.GetComponent<AcceleratedProjectile>();
					accelProjectile.SetAcceleration(dir * 10f);
					this.ACT_WAIT.StartAction(o, timeBetweenProjectiles);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				o.PontiffHuskBoss.AnimatorInyector.StopShoot();
				o.PontiffHuskBoss.AnimatorInyector.StopCharge();
				this.ACT_WAIT.StartAction(o, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.transform.parent = null;
				base.FinishAction();
				yield break;
			}

			private int numProjectiles;

			private float chargeTime;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class WindAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.ClearAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				o.PontiffHuskBoss.AnimatorInyector.StopHide();
				Vector2 target = (o.PontiffHuskBoss.Status.Orientation != EntityOrientation.Left) ? (o.ArenaGetTopLeftCorner() + Vector2.right * Vector2.Distance(o.transform.position, o.ArenaGetTopLeftCorner()) * 0.2f) : (o.ArenaGetTopRightCorner() + Vector2.right * Vector2.Distance(o.transform.position, o.ArenaGetTopRightCorner()) * 0.2f);
				float moveTime = Vector2.Distance(target, o.transform.position) * 0.2f + 0.5f;
				this.ACT_MOVE.StartAction(o, target, moveTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				o.PontiffHuskBoss.AnimatorInyector.PlayCharge();
				float chargeTime = 0.3f;
				float windTime = 3f;
				float extensionTime = 1f;
				o.WindSpiralProjectilesAttack.ActivateAttack(8, windTime, extensionTime);
				o.transform.parent = o.CameraAnchor.transform;
				this.ACT_WAIT.StartAction(o, chargeTime + windTime + extensionTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.transform.parent = null;
				o.PontiffHuskBoss.AnimatorInyector.StopCharge();
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class AltSpiralProjectilesAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.ClearAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				bool goingToHide = o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE TO HIDE") || o.PontiffHuskBoss.AnimatorInyector.GetHide();
				bool isHiding = o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE");
				bool wasHiding = o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE TO IDLE");
				if (goingToHide || isHiding || wasHiding)
				{
					if (goingToHide)
					{
						yield return new WaitUntil(() => o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE"));
					}
					if (goingToHide || isHiding)
					{
						o.transform.position = o.ArenaGetTopLeftCorner();
						o.InstantLookAtDir(Vector2.right);
					}
					o.PontiffHuskBoss.AnimatorInyector.StopHide();
					yield return new WaitUntil(() => o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE"));
				}
				Vector2 target = (o.PontiffHuskBoss.Status.Orientation != EntityOrientation.Left) ? (o.ArenaGetTopLeftCorner() + Vector2.right) : (o.ArenaGetTopRightCorner() + Vector2.left);
				float moveTime = Vector2.Distance(target, o.transform.position) * 0.1f + 0.2f;
				this.ACT_MOVE.StartAction(o, target, moveTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				o.transform.parent = o.CameraAnchor.transform;
				o.PontiffHuskBoss.AnimatorInyector.PlayCharge();
				float chargeTime = 0.3f;
				this.ACT_WAIT.StartAction(o, chargeTime);
				yield return this.ACT_WAIT.waitForCompletion;
				float attackTime = 3f;
				float extensionTime = 1f;
				o.WindSpiralProjectilesAttack.transform.localScale = ((o.PontiffHuskBoss.Status.Orientation != EntityOrientation.Right) ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f));
				o.WindSpiralProjectilesAttack.ActivateAttack(8, attackTime, extensionTime);
				Transform refTransform = o.WindSpiralProjectilesAttack.spinningTransform;
				refTransform.position = o.WindSpiralProjectilesAttack.transform.position;
				Vector3 refTransformPos = refTransform.position;
				refTransformPos.x = ((o.PontiffHuskBoss.Status.Orientation != EntityOrientation.Left) ? o.ArenaGetTopRightCorner().x : o.ArenaGetTopLeftCorner().x);
				this.ACT_MOVE.StartAction(o, refTransformPos, attackTime + extensionTime, Ease.InOutQuad, refTransform, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				o.PontiffHuskBoss.AnimatorInyector.StopCharge();
				o.transform.parent = null;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class ChargedBlastAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				if (pontiffHuskBossBehaviour.CrisantaProtectorInstance != null)
				{
					pontiffHuskBossBehaviour.CrisantaProtectorInstance.SetActive(false);
					pontiffHuskBossBehaviour.CrisantaProtectorInstance = null;
				}
				Penitent penitent = Core.Logic.Penitent;
				penitent.Status.Invulnerable = false;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				o.PontiffHuskBoss.AnimatorInyector.PlayHide();
				this.ACT_WAIT.StartAction(o, 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector2 target = o.ArenaGetBotRightCorner() + Vector2.up * 7.5f;
				o.transform.position = target;
				o.transform.parent = o.CameraAnchor.transform;
				o.InstantLookAtDir(Vector2.left);
				o.PontiffHuskBoss.AnimatorInyector.StopHide();
				o.PontiffHuskBoss.AnimatorInyector.PlayBeam();
				o.PontiffHuskBoss.AnimatorInyector.PlayCharge();
				WaypointsMovingPlatform crisantaPlatform = o.GetPlatformInDirection(Vector2.down, o.battleBounds.center + Vector2.right * 3f);
				if (crisantaPlatform == null)
				{
					for (int k = 1; k < 21; k++)
					{
						crisantaPlatform = o.GetPlatformInDirection(Vector2.down + Vector2.right * 0.1f * (float)k, o.battleBounds.center);
						if (crisantaPlatform != null)
						{
							break;
						}
					}
				}
				if (crisantaPlatform == null)
				{
					for (int l = 1; l < 21; l++)
					{
						crisantaPlatform = o.GetPlatformInDirection(Vector2.down + Vector2.left * 0.1f * (float)l, o.battleBounds.center);
						if (crisantaPlatform != null)
						{
							break;
						}
					}
				}
				if (crisantaPlatform == null)
				{
					Debug.LogError("No platform for Crisanta!");
				}
				Vector2 crisantaPos = (crisantaPlatform.GetDestination().y <= crisantaPlatform.GetOrigin().y) ? crisantaPlatform.GetOrigin() : crisantaPlatform.GetDestination();
				crisantaPos += Vector2.right * 4f;
				for (int i = 0; i < 10; i++)
				{
					if (crisantaPos.y <= crisantaPlatform.transform.position.y || Mathf.Approximately(crisantaPos.y, crisantaPlatform.transform.position.y))
					{
						break;
					}
					this.ACT_WAIT.StartAction(o, 0.1f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				GameObject prefab = (!this.doneBefore) ? o.CrisantaProtectorPrefabFT : o.CrisantaProtectorPrefab;
				o.CrisantaProtectorInstance = PoolManager.Instance.ReuseObject(prefab, crisantaPos, Quaternion.identity, true, 1).GameObject;
				PlayableDirector playableDirector = o.CrisantaProtectorInstance.GetComponent<PlayableDirector>();
				playableDirector.Play();
				if (!this.doneBefore)
				{
					o.PontiffHuskBoss.Audio.PlayChargedBlastNoVoice();
				}
				else
				{
					o.PontiffHuskBoss.Audio.PlayChargedBlast();
				}
				float chargeTime = 2f;
				this.ACT_WAIT.StartAction(o, chargeTime * 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				if (!this.doneBefore)
				{
					this.doneBefore = true;
					Core.Dialog.StartConversation("DLG_12210", false, false, true, 0, false);
					this.ACT_WAIT.StartAction(o, 2f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				this.ACT_WAIT.StartAction(o, chargeTime * 0.8f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.PontiffHuskBoss.AnimatorInyector.PlayShoot();
				Vector2 shootingPoint = o.transform.position + Vector3.down * 4.2f + Vector3.left * 2f;
				GameObject areago = o.ChargedBlastAttack.SummonAreaOnPoint(shootingPoint, 0f, 1f, null);
				areago.transform.parent = o.combatAreaParent;
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				PoolManager.Instance.ReuseObject(o.ChargedBlastExplosion, shootingPoint + Vector2.right * 2f, Quaternion.identity, true, 1);
				float beamTime = 1.5f;
				int numSteps = 10;
				for (int j = 0; j < numSteps; j++)
				{
					this.ACT_WAIT.StartAction(o, beamTime / (float)numSteps);
					yield return this.ACT_WAIT.waitForCompletion;
					if (Vector2.Distance(o.CrisantaProtectorInstance.transform.position, p.GetPosition()) < 4f && p.GetPosition().x < o.CrisantaProtectorInstance.transform.position.x && !p.IsJumping)
					{
						p.Status.Invulnerable = true;
					}
					else
					{
						p.Status.Invulnerable = false;
					}
				}
				o.transform.parent = null;
				o.PontiffHuskBoss.AnimatorInyector.StopShoot();
				o.PontiffHuskBoss.AnimatorInyector.StopCharge();
				this.ACT_WAIT.StartAction(o, 2.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				p.Status.Invulnerable = false;
				o.PontiffHuskBoss.AnimatorInyector.PlayHide();
				this.ACT_WAIT.StartAction(o, 1.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.CrisantaProtectorInstance = null;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private bool doneBefore;
		}

		public class SpiralProjectilesAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				PontiffHuskBossBehaviour pontiffHuskBossBehaviour = this.owner as PontiffHuskBossBehaviour;
				pontiffHuskBossBehaviour.PontiffHuskBoss.AnimatorInyector.ResetAll();
				pontiffHuskBossBehaviour.ClearAll();
				pontiffHuskBossBehaviour.transform.parent = null;
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, int numBalls)
			{
				this.numBalls = numBalls;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				PontiffHuskBossBehaviour o = this.owner as PontiffHuskBossBehaviour;
				Penitent p = Core.Logic.Penitent;
				bool goingToHide = o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE TO HIDE") || o.PontiffHuskBoss.AnimatorInyector.GetHide();
				bool isHiding = o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE");
				bool wasHiding = o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE TO IDLE");
				if (goingToHide || isHiding || wasHiding)
				{
					if (goingToHide)
					{
						yield return new WaitUntil(() => o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("HIDE"));
					}
					if (goingToHide || isHiding)
					{
						o.transform.position = o.ArenaGetTopRightCorner();
						o.InstantLookAtDir(Vector2.left);
					}
					o.PontiffHuskBoss.AnimatorInyector.StopHide();
					yield return new WaitUntil(() => o.PontiffHuskBoss.AnimatorInyector.EntityAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE"));
				}
				Vector2 target = (!o.IsBossOnTheRightSide()) ? (o.ArenaGetTopLeftCorner() + Vector2.right * 2f) : (o.ArenaGetTopRightCorner() + Vector2.left * 1f);
				target.y -= 1f;
				float moveTime = Vector2.Distance(target, o.transform.position) * 0.2f;
				o.LookAtDir(o.battleBounds.center - target);
				bool turnQeued = o.PontiffHuskBoss.AnimatorInyector.IsTurnQeued();
				moveTime += ((!turnQeued) ? 0.5f : 0f);
				this.ACT_MOVE.StartAction(o, target, moveTime, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				Vector2 attackTarget = (!o.IsBossOnTheRightSide()) ? (o.ArenaGetTopRightCorner() + Vector2.right * 3f) : (o.ArenaGetTopLeftCorner() + Vector2.right * 6f);
				attackTarget.y -= 1f;
				moveTime = Vector2.Distance(attackTarget, o.transform.position) * 0.2f + 1f;
				o.PontiffHuskBoss.AnimatorInyector.PlayCharge();
				o.SpiralProjectilesAttack.transform.localScale = ((o.PontiffHuskBoss.Status.Orientation != EntityOrientation.Right) ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f));
				float extensionTime = 1f;
				o.SpiralProjectilesAttack.ActivateAttack(this.numBalls, moveTime, extensionTime);
				float waitTime = (!turnQeued) ? 0.5f : 1.5f;
				o.transform.parent = o.CameraAnchor.transform;
				this.ACT_WAIT.StartAction(o, waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.transform.parent = null;
				target = attackTarget;
				this.ACT_MOVE.StartAction(o, target, moveTime, Ease.InQuad, null, true, null, true, true, 1.7f);
				this.ACT_WAIT.StartAction(o, moveTime - 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.LookAtDir(o.battleBounds.center - target);
				yield return this.ACT_MOVE.waitForCompletion;
				o.PontiffHuskBoss.AnimatorInyector.StopCharge();
				o.transform.parent = o.CameraAnchor.transform;
				this.ACT_WAIT.StartAction(o, extensionTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.transform.parent = null;
				base.FinishAction();
				yield break;
			}

			private int numBalls;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}
	}
}
