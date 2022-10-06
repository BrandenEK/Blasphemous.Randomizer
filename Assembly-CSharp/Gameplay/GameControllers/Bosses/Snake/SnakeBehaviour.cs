using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.HomingTurret.Attack;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Environment.AreaEffects;
using Gameplay.GameControllers.Environment.Traps.FireTrap;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Attack;
using Gameplay.UI.Widgets;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Rewired;
using Sirenix.OdinInspector;
using Tools.Level.Interactables;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Snake
{
	public class SnakeBehaviour : EnemyBehaviour
	{
		public Snake Snake { get; set; }

		private void InitAI()
		{
			this.stIdle = new SnakeBehaviour_StIdle();
			this.stAction = new Snake_StAction();
			this.stDeath = new Snake_StDeath();
			this._fsm = new StateMachine<SnakeBehaviour>(this, this.stIdle, null, null);
		}

		private void InitActionDictionary()
		{
			this.waitBetweenActions_EA = new WaitSeconds_EnemyAction();
			this.intro_EA = new SnakeBehaviour.Intro_EnemyAction();
			this.goUp_EA = new SnakeBehaviour.GoUp_EnemyAction();
			this.death_EA = new SnakeBehaviour.Death_EnemyAction();
			this.chargedBiteLeftHead_EA = new SnakeBehaviour.ChargedBiteLeftHead_EnemyAction();
			this.chargedBiteRightHead_EA = new SnakeBehaviour.ChargedBiteRightHead_EnemyAction();
			this.scalesSpikesLeftHead_EA = new SnakeBehaviour.ScalesSpikesLeftHead_EnemyAction();
			this.scalesSpikesRightHead_EA = new SnakeBehaviour.ScalesSpikesRightHead_EnemyAction();
			this.chainedLightningLeftHead_EA = new SnakeBehaviour.ChainedLightningLeftHead_EnemyAction();
			this.chainedLightningRightHead_EA = new SnakeBehaviour.ChainedLightningRightHead_EnemyAction();
			this.bigChainedLightningLeftHead_EA = new SnakeBehaviour.BigChainedLightningLeftHead_EnemyAction();
			this.bigChainedLightningRightHead_EA = new SnakeBehaviour.BigChainedLightningRightHead_EnemyAction();
			this.tailOrbs_EA = new SnakeBehaviour.TailOrbs_EnemyAction();
			this.tailBeam_EA = new SnakeBehaviour.TailBeam_EnemyAction();
			this.scalesSpikesRightHeadObstacles_EA = new SnakeBehaviour.ScalesSpikesRightHeadObstacles_EnemyAction();
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.INTRO, new Func<EnemyAction>(this.LaunchAction_Intro));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.CHARGED_BITE, new Func<EnemyAction>(this.LaunchAction_ChargedBite));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.SCALES_SPIKES, new Func<EnemyAction>(this.LaunchAction_ScalesSpikes));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.CHAINED_LIGHTNING, new Func<EnemyAction>(this.LaunchAction_ChainedLightning));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.TAIL_ORBS, new Func<EnemyAction>(this.LaunchAction_TailOrbs));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.TAIL_BEAM, new Func<EnemyAction>(this.LaunchAction_TailBeam));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.GO_UP, new Func<EnemyAction>(this.LaunchAction_GoUp));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.DEATH, new Func<EnemyAction>(this.LaunchAction_Death));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.SEEKING_CHAINED_LIGHTNING, new Func<EnemyAction>(this.LaunchAction_SeekingChainedLightning));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.BIG_CHAINED_LIGHTNING, new Func<EnemyAction>(this.LaunchAction_BigChainedLightning));
			this.actionsDictionary.Add(SnakeBehaviour.SNAKE_ATTACKS.SCALES_SPIKES_OBSTACLES, new Func<EnemyAction>(this.LaunchAction_ScalesSpikesObstacles));
			this.availableAttacks = this.AttackConfigData.GetAttackIds(true, true, 1f);
		}

		private IEnumerator Start()
		{
			this.Snake = (Snake)this.Entity;
			this.InitAI();
			this.InitActionDictionary();
			this.currentFightParameters = this.AllFightParameters[0];
			this.HideElmFireTraps(this.ElmFireLoopLeftHead1);
			this.HideElmFireTraps(this.ElmFireLoopLeftHead2);
			this.HideElmFireTraps(this.ElmFireLoopLeftHead3);
			this.HideElmFireTraps(this.ElmFireLoopRightHead1);
			this.HideElmFireTraps(this.ElmFireLoopRightHead2);
			this.HideElmFireTraps(this.ElmFireLoopRightHead3);
			this.HideElmFireTraps(this.ElmFireLoopLeftHeadSeeking);
			this.HideElmFireTraps(this.ElmFireLoopRightHeadSeeking);
			this.HideElmFireTraps(this.ElmFireLoopLeftHeadBig1);
			this.HideElmFireTraps(this.ElmFireLoopLeftHeadBig2);
			this.HideElmFireTraps(this.ElmFireLoopRightHeadBig1);
			this.HideElmFireTraps(this.ElmFireLoopRightHeadBig2);
			this.Snake.SnakeAnimatorInyector.PlayCloseMouth();
			this.Snake.TongueLeftIdleMouth.DamageAreaCollider.enabled = false;
			this.Snake.TongueLeftOpenMouth.DamageAreaCollider.enabled = false;
			this.Snake.TongueRightOpenMouth.DamageAreaCollider.enabled = false;
			this.Snake.TongueRightIdleMouth.DamageAreaCollider.enabled = false;
			this.Snake.Audio.PlaySnakeRain();
			this.WindToTheRight.SetMaxForce();
			this.WindToTheLeft.SetMaxForce();
			LevelManager.OnBeforeLevelLoad += this.StopRainOnLevelLoad;
			yield return new WaitUntil(() => Core.Logic.Penitent);
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnPenitentDead));
			yield break;
		}

		private void OnPenitentDead()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.OnPenitentDead));
			this.Snake.Audio.StopAll();
			this.Snake.Audio.StopSnakeRain();
		}

		private void StopRainOnLevelLoad(Level a, Level b)
		{
			LevelManager.OnBeforeLevelLoad -= this.StopRainOnLevelLoad;
			this.Snake.Audio.StopAll();
			this.Snake.Audio.StopSnakeRain();
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.ResetRain));
		}

		private void HideElmFireTraps(ElmFireTrapManager elmFireLoopManager)
		{
			elmFireLoopManager.transform.parent.gameObject.SetActive(true);
			elmFireLoopManager.InstantHideElmFireTraps();
		}

		private void Update()
		{
			this._fsm.DoUpdate();
		}

		private void OnGUI()
		{
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(this.BattleBounds.center, new Vector3(this.BattleBounds.width, this.BattleBounds.height, 0f));
			Gizmos.DrawWireCube(this.ArenaGetBotLeftCorner(), Vector3.one * 0.1f);
			Gizmos.DrawWireCube(this.ArenaGetBotRightCorner(), Vector3.one * 0.1f);
		}

		public void StartIntro()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.ResetRain));
			this.LaunchAction(SnakeBehaviour.SNAKE_ATTACKS.INTRO);
			this.QueueAttack(SnakeBehaviour.SNAKE_ATTACKS.CHARGED_BITE);
		}

		private List<SnakeBehaviour.SNAKE_ATTACKS> GetFilteredAttacks(List<SnakeBehaviour.SNAKE_ATTACKS> originalList)
		{
			List<SnakeBehaviour.SNAKE_ATTACKS> list = new List<SnakeBehaviour.SNAKE_ATTACKS>(originalList);
			SnakeScriptableConfig.SnakeAttackConfig atkConfig = this.AttackConfigData.GetAttackConfig(this.lastAttack);
			if (atkConfig.cantBeFollowedBy != null && atkConfig.cantBeFollowedBy.Count > 0)
			{
				list.RemoveAll((SnakeBehaviour.SNAKE_ATTACKS x) => atkConfig.cantBeFollowedBy.Contains(x));
			}
			if (atkConfig.alwaysFollowedBy != null && atkConfig.alwaysFollowedBy.Count > 0)
			{
				list.RemoveAll((SnakeBehaviour.SNAKE_ATTACKS x) => !atkConfig.alwaysFollowedBy.Contains(x));
			}
			if (this.lastAttack == SnakeBehaviour.SNAKE_ATTACKS.BIG_CHAINED_LIGHTNING || this.lastAttack == SnakeBehaviour.SNAKE_ATTACKS.CHAINED_LIGHTNING || this.lastAttack == SnakeBehaviour.SNAKE_ATTACKS.SEEKING_CHAINED_LIGHTNING)
			{
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.BIG_CHAINED_LIGHTNING);
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.CHAINED_LIGHTNING);
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.SEEKING_CHAINED_LIGHTNING);
			}
			if (this.lastAttack == SnakeBehaviour.SNAKE_ATTACKS.GO_UP)
			{
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.SEEKING_CHAINED_LIGHTNING);
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.SCALES_SPIKES_OBSTACLES);
			}
			if (this.Snake.IsRightHeadVisible)
			{
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.TAIL_BEAM);
			}
			else
			{
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.SCALES_SPIKES_OBSTACLES);
			}
			if (this.ScalesSpikesObstacles.instantiations.Exists((GameObject x) => x.activeInHierarchy))
			{
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.SCALES_SPIKES_OBSTACLES);
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.CHAINED_LIGHTNING);
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.BIG_CHAINED_LIGHTNING);
				list.Remove(SnakeBehaviour.SNAKE_ATTACKS.TAIL_BEAM);
				if (this.Snake.IsRightHeadVisible)
				{
					list.Remove(SnakeBehaviour.SNAKE_ATTACKS.SCALES_SPIKES);
				}
			}
			if (list.Count > 1)
			{
				list.Remove(this.lastAttack);
			}
			if (list.Count > 2)
			{
				list.Remove(this.prevToLastAttack);
			}
			return list;
		}

		private int RandomizeUsingWeights(List<SnakeBehaviour.SNAKE_ATTACKS> filteredAtks)
		{
			float hpPercentage = this.Snake.GetHpPercentage();
			List<float> filteredAttacksWeights = this.AttackConfigData.GetFilteredAttacksWeights(filteredAtks, true, hpPercentage);
			float num = filteredAttacksWeights.Sum();
			float num2 = Random.Range(0f, num);
			float num3 = 0f;
			for (int i = 0; i < filteredAtks.Count; i++)
			{
				num3 += filteredAttacksWeights[i];
				if (num3 > num2)
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

		private void QueueAttack(SnakeBehaviour.SNAKE_ATTACKS atk)
		{
			this.queuedAttacks.Add(atk);
		}

		private SnakeBehaviour.SNAKE_ATTACKS PopAttackFromQueue()
		{
			SnakeBehaviour.SNAKE_ATTACKS result = SnakeBehaviour.SNAKE_ATTACKS.CHARGED_BITE;
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
			float hpPercentage = this.Snake.GetHpPercentage();
			this.availableAttacks = this.AttackConfigData.GetAttackIds(true, true, hpPercentage);
			for (int i = 0; i < this.AllFightParameters.Count; i++)
			{
				if (this.AllFightParameters[i].HpPercentageBeforeApply < this.currentFightParameters.HpPercentageBeforeApply && this.AllFightParameters[i].HpPercentageBeforeApply > hpPercentage && !this.currentFightParameters.Equals(this.AllFightParameters[i]))
				{
					this.currentFightParameters = this.AllFightParameters[i];
					result = true;
					break;
				}
			}
			return result;
		}

		private void LaunchAutomaticAction()
		{
			List<SnakeBehaviour.SNAKE_ATTACKS> filteredAttacks = this.GetFilteredAttacks(this.availableAttacks);
			SnakeBehaviour.SNAKE_ATTACKS action;
			if (this.queuedAttacks.Count > 0)
			{
				action = this.PopAttackFromQueue();
			}
			else
			{
				int index = this.RandomizeUsingWeights(filteredAttacks);
				action = filteredAttacks[index];
			}
			this.LaunchAction(action);
			this.prevToLastAttack = this.lastAttack;
			this.lastAttack = action;
		}

		protected void LaunchAction(SnakeBehaviour.SNAKE_ATTACKS action)
		{
			this.StopCurrentAction();
			this._fsm.ChangeState(this.stAction);
			this.currentAction = this.actionsDictionary[action]();
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped += this.CurrentAction_OnActionStops;
		}

		protected EnemyAction LaunchAction_Death()
		{
			return this.death_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_Intro()
		{
			return this.intro_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_ChargedBite()
		{
			float hpPercentage = this.Snake.GetHpPercentage();
			bool changesHead = hpPercentage < 0.66f;
			float num = 1f + hpPercentage * 0.2f;
			if (this.Snake.IsRightHeadVisible)
			{
				return this.chargedBiteRightHead_EA.StartAction(this, 1f * num, 0.7f * num, changesHead);
			}
			return this.chargedBiteLeftHead_EA.StartAction(this, 1f * num, 1.2f * num, changesHead);
		}

		protected EnemyAction LaunchAction_ScalesSpikes()
		{
			if (this.Snake.IsLeftHeadVisible)
			{
				return this.scalesSpikesLeftHead_EA.StartAction(this);
			}
			return this.scalesSpikesRightHead_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_ChainedLightning()
		{
			ElmFireTrapManager elmFireLoopManager;
			int numSteps;
			if (this.Snake.IsLeftHeadVisible)
			{
				if (this.currentFightParameters.Phase == SnakeBehaviour.SNAKE_PHASE.FIRST)
				{
					elmFireLoopManager = this.ElmFireLoopLeftHead1;
					numSteps = 6;
				}
				else if (this.currentFightParameters.Phase == SnakeBehaviour.SNAKE_PHASE.SECOND)
				{
					elmFireLoopManager = this.ElmFireLoopLeftHead2;
					numSteps = 4;
				}
				else
				{
					elmFireLoopManager = this.ElmFireLoopLeftHead3;
					numSteps = 5;
				}
				return this.chainedLightningLeftHead_EA.StartAction(this, elmFireLoopManager, 0.2f, 0.2f, numSteps, false);
			}
			if (this.currentFightParameters.Phase == SnakeBehaviour.SNAKE_PHASE.FIRST)
			{
				elmFireLoopManager = this.ElmFireLoopRightHead1;
				numSteps = 6;
			}
			else if (this.currentFightParameters.Phase == SnakeBehaviour.SNAKE_PHASE.SECOND)
			{
				elmFireLoopManager = this.ElmFireLoopRightHead2;
				numSteps = 4;
			}
			else
			{
				elmFireLoopManager = this.ElmFireLoopRightHead3;
				numSteps = 5;
			}
			return this.chainedLightningRightHead_EA.StartAction(this, elmFireLoopManager, 0.2f, 0.2f, numSteps, false);
		}

		protected EnemyAction LaunchAction_SeekingChainedLightning()
		{
			int numSteps = 4;
			if (this.Snake.IsLeftHeadVisible)
			{
				ElmFireTrapManager elmFireLoopLeftHeadSeeking = this.ElmFireLoopLeftHeadSeeking;
				return this.chainedLightningLeftHead_EA.StartAction(this, elmFireLoopLeftHeadSeeking, 0.1f, 0.2f, numSteps, true);
			}
			ElmFireTrapManager elmFireLoopRightHeadSeeking = this.ElmFireLoopRightHeadSeeking;
			return this.chainedLightningRightHead_EA.StartAction(this, elmFireLoopRightHeadSeeking, 0.1f, 0.2f, numSteps, true);
		}

		protected EnemyAction LaunchAction_BigChainedLightning()
		{
			int num = 4;
			if (this.Snake.IsLeftHeadVisible)
			{
				ElmFireTrapManager elmFireLoopLeftHeadBig = this.ElmFireLoopLeftHeadBig1;
				ElmFireTrapManager elmFireLoopLeftHeadBig2 = this.ElmFireLoopLeftHeadBig2;
				return this.bigChainedLightningLeftHead_EA.StartAction(this, elmFireLoopLeftHeadBig, 0.2f, 0.2f, num, elmFireLoopLeftHeadBig2, 0.2f, 0.2f, num, 0.2f);
			}
			ElmFireTrapManager elmFireLoopRightHeadBig = this.ElmFireLoopRightHeadBig1;
			ElmFireTrapManager elmFireLoopRightHeadBig2 = this.ElmFireLoopRightHeadBig2;
			return this.bigChainedLightningRightHead_EA.StartAction(this, elmFireLoopRightHeadBig, 0.2f, 0.2f, num, elmFireLoopRightHeadBig2, 0.2f, 0.2f, num, 0.2f);
		}

		protected EnemyAction LaunchAction_ScalesSpikesObstacles()
		{
			return this.scalesSpikesRightHeadObstacles_EA.StartAction(this);
		}

		protected EnemyAction LaunchAction_TailSpikes()
		{
			throw new NotImplementedException("The Tail Spikes attack has been removed.");
		}

		protected EnemyAction LaunchAction_TailOrbs()
		{
			float hpPercentage = this.Snake.GetHpPercentage();
			int attackRepetitions = this.AttackConfigData.GetAttackRepetitions(SnakeBehaviour.SNAKE_ATTACKS.TAIL_ORBS, true, hpPercentage);
			bool isRightHeadVisible = this.Snake.IsRightHeadVisible;
			return this.tailOrbs_EA.StartAction(this, attackRepetitions, isRightHeadVisible, true);
		}

		protected EnemyAction LaunchAction_TailSweep()
		{
			throw new NotImplementedException("The Tail Sweep attack has been removed.");
		}

		protected EnemyAction LaunchAction_TailBeam()
		{
			bool isRightHeadVisible = this.Snake.IsRightHeadVisible;
			return this.tailBeam_EA.StartAction(this, isRightHeadVisible, true);
		}

		protected EnemyAction LaunchAction_GoUp()
		{
			return this.goUp_EA.StartAction(this);
		}

		private void CurrentAction_OnActionStops(EnemyAction e)
		{
		}

		private void CurrentAction_OnActionEnds(EnemyAction e)
		{
			e.OnActionEnds -= this.CurrentAction_OnActionEnds;
			e.OnActionIsStopped -= this.CurrentAction_OnActionStops;
			if (this._fsm.IsInState(this.stDeath))
			{
				return;
			}
			if (e != this.waitBetweenActions_EA)
			{
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
			this.StartWait(this.extraRecoverySeconds + this.currentFightParameters.MinMaxWaitingTimeBetweenActions.x, this.extraRecoverySeconds + this.currentFightParameters.MinMaxWaitingTimeBetweenActions.y);
			this.extraRecoverySeconds = 0f;
		}

		private void StartWait(float min, float max)
		{
			this.StopCurrentAction();
			this.currentAction = this.waitBetweenActions_EA.StartAction(this, min, max);
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
		}

		private void CheckDebugActions()
		{
			Dictionary<KeyCode, SnakeBehaviour.SNAKE_ATTACKS> debugActions = this.AttackConfigData.debugActions;
			if (debugActions != null)
			{
				foreach (KeyCode keyCode in debugActions.Keys)
				{
					if (Input.GetKeyDown(keyCode))
					{
						this.QueueAttack(debugActions[keyCode]);
					}
				}
			}
		}

		public void SetWeapon(SnakeBehaviour.SNAKE_WEAPONS weapon)
		{
			if (this.CurrentMeleeAttackLeftHead || this.CurrentMeleeAttackRightHead)
			{
				this.OnMeleeAttackFinished();
			}
			switch (weapon)
			{
			case SnakeBehaviour.SNAKE_WEAPONS.CHARGING_OPEN_MOUTH:
				this.CurrentMeleeAttackLeftHead = this.OpenedMouthAttackLeft;
				this.CurrentMeleeAttackRightHead = this.OpenedMouthAttackRight;
				break;
			case SnakeBehaviour.SNAKE_WEAPONS.OPEN_TO_CLOSED:
				this.CurrentMeleeAttackLeftHead = this.OpenedToClosedAttackLeft;
				this.CurrentMeleeAttackRightHead = this.OpenedToClosedAttackRight;
				break;
			case SnakeBehaviour.SNAKE_WEAPONS.CASTING_OPEN_MOUTH:
				this.CurrentMeleeAttackLeftHead = this.CastingOpenedMouthAttackLeft;
				this.CurrentMeleeAttackRightHead = this.CastingOpenedMouthAttackRight;
				break;
			case SnakeBehaviour.SNAKE_WEAPONS.CHARGED_BITE:
				this.CurrentMeleeAttackLeftHead = this.ChargedBiteAttackLeft;
				this.CurrentMeleeAttackRightHead = this.ChargedBiteAttackRight;
				break;
			}
		}

		public void ResetRain()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.ResetRain));
			this.Snake.SnakeSegmentsMovementController.ResetRain();
		}

		private Vector2 GetDirToPenitent()
		{
			return Core.Logic.Penitent.transform.position - base.transform.position;
		}

		public Vector2 ArenaGetBotRightCorner()
		{
			return new Vector2(this.BattleBounds.xMax, this.BattleBounds.yMin);
		}

		public Vector2 ArenaGetBotLeftCorner()
		{
			return new Vector2(this.BattleBounds.xMin, this.BattleBounds.yMin);
		}

		public Vector2 ArenaGetBotFarRandomPoint()
		{
			Vector2 zero = Vector2.zero;
			zero.y = this.BattleBounds.yMin;
			if (base.transform.position.x > this.BattleBounds.center.x)
			{
				zero.x = Random.Range(this.BattleBounds.xMin, this.BattleBounds.center.x);
			}
			else
			{
				zero.x = Random.Range(this.BattleBounds.center.x, this.BattleBounds.xMax);
			}
			return zero;
		}

		public Vector2 ArenaGetBotNearRandomPoint()
		{
			Vector2 zero = Vector2.zero;
			zero.y = this.BattleBounds.yMin;
			if (base.transform.position.x < this.BattleBounds.center.x)
			{
				zero.x = Random.Range(this.BattleBounds.xMin, this.BattleBounds.center.x);
			}
			else
			{
				zero.x = Random.Range(this.BattleBounds.center.x, this.BattleBounds.xMax);
			}
			return zero;
		}

		public void OnMeleeAttackStarts()
		{
			if (this.CurrentMeleeAttackLeftHead)
			{
				this.CurrentMeleeAttackLeftHead.DealsDamage = true;
				this.CurrentMeleeAttackLeftHead.CurrentWeaponAttack();
			}
			if (this.CurrentMeleeAttackRightHead)
			{
				this.CurrentMeleeAttackRightHead.DealsDamage = true;
				this.CurrentMeleeAttackRightHead.CurrentWeaponAttack();
			}
		}

		public void OnMeleeAttackFinished()
		{
			if (this.CurrentMeleeAttackLeftHead)
			{
				this.CurrentMeleeAttackLeftHead.DealsDamage = false;
			}
			if (this.CurrentMeleeAttackRightHead)
			{
				this.CurrentMeleeAttackRightHead.DealsDamage = false;
			}
		}

		public void DoActivateCollisionsOpenMouth(bool b)
		{
			if (b)
			{
				if (this.Snake.IsRightHeadVisible)
				{
					this.Snake.TongueRightOpenMouth.DamageAreaCollider.enabled = b;
				}
				else
				{
					this.Snake.TongueLeftOpenMouth.DamageAreaCollider.enabled = b;
				}
			}
			else
			{
				this.Snake.TongueLeftOpenMouth.DamageAreaCollider.enabled = b;
				this.Snake.TongueRightOpenMouth.DamageAreaCollider.enabled = b;
			}
		}

		public void DoActivateCollisionsIdle(bool b)
		{
			if (b)
			{
				if (this.Snake.IsRightHeadVisible)
				{
					this.Snake.TongueRightIdleMouth.DamageAreaCollider.enabled = b;
				}
				else
				{
					this.Snake.TongueLeftIdleMouth.DamageAreaCollider.enabled = b;
				}
			}
			else
			{
				this.Snake.TongueLeftIdleMouth.DamageAreaCollider.enabled = b;
				this.Snake.TongueRightIdleMouth.DamageAreaCollider.enabled = b;
			}
		}

		public void Death()
		{
			this.Snake.Audio.StopAll();
			this.Snake.Audio.PlaySnakeRain();
			GameplayUtils.DestroyAllProjectiles();
			this.StopCurrentAction();
			base.StopAllCoroutines();
			ShortcutExtensions.DOKill(base.transform, false);
			this._fsm.ChangeState(this.stDeath);
			Core.Logic.Penitent.Status.Invulnerable = true;
			this.LaunchAction(SnakeBehaviour.SNAKE_ATTACKS.DEATH);
		}

		private void ClearAllAttacks()
		{
			this.HomingLaserAttack.Clear();
			this.HomingLaserAttack.Clear();
			this.ScalesSpikesSlow.ClearAll();
			this.ScalesSpikesFast.ClearAll();
			this.ScalesSpikesObstacles.ClearAll();
		}

		public void ShoutShockwave(Vector3 startPosition, Vector3 endPosition)
		{
			base.StartCoroutine(this.ShoutShockwaveCoroutine(startPosition, endPosition));
		}

		private IEnumerator ShoutShockwaveCoroutine(Vector3 startPosition, Vector3 endPosition)
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.45f, Vector3.up * 2.5f, 25, 0.2f, 0.01f, default(Vector3), 0.01f, true);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(startPosition, 0.3f, 0.1f, 0.3f);
			yield return new WaitForSeconds(0.2f);
			Core.Logic.ScreenFreeze.Freeze(0.05f, 0.1f, 0f, null);
			Vector3 position = Vector3.Lerp(startPosition, endPosition, 0.5f);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(position, 0.3f, 0.1f, 1f);
			yield return new WaitForSeconds(0.1f);
			Core.Logic.ScreenFreeze.Freeze(0.05f, 0.1f, 0f, null);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(endPosition, 0.4f, 0.2f, 2f);
			yield return new WaitForSeconds(0.3f);
			yield break;
		}

		public void Damage(Hit hit)
		{
			if (hit.AttackingEntity.CompareTag("Penitent"))
			{
				Vector3 centerPos = (!this.Snake.IsRightHeadVisible) ? this.Snake.GetLeftDamagedPosition() : this.Snake.GetRightDamagedPosition();
				PenitentSword penitentSword = (PenitentSword)Core.Logic.Penitent.PenitentAttack.CurrentPenitentWeapon;
				penitentSword.SpawnSparks(centerPos);
				if (!hit.DontSpawnBlood)
				{
					penitentSword.SpawnBlood(centerPos, hit);
				}
			}
			bool flag = this.SwapFightParametersIfNeeded();
			if (flag)
			{
				this.QeuePhaseSwitchAttacks();
			}
		}

		private void QeuePhaseSwitchAttacks()
		{
			this.QueueAttack(SnakeBehaviour.SNAKE_ATTACKS.GO_UP);
		}

		public override void Attack()
		{
			throw new NotImplementedException();
		}

		public override void Chase(Transform targetPosition)
		{
			throw new NotImplementedException();
		}

		public override void Damage()
		{
			throw new NotImplementedException();
		}

		public override void Idle()
		{
			throw new NotImplementedException();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		[FoldoutGroup("Character settings", 0)]
		public AnimationCurve TimeSlowCurve;

		[FoldoutGroup("Battle area", 0)]
		public Rect BattleBounds;

		[FoldoutGroup("Battle area", 0)]
		public Transform SnakeLeftCorner;

		[FoldoutGroup("Battle area", 0)]
		public Transform SnakeRightCorner;

		[FoldoutGroup("Battle config", 0)]
		public List<SnakeBehaviour.SnakeFightParameters> AllFightParameters;

		[FoldoutGroup("Attacks config", 0)]
		public SnakeScriptableConfig AttackConfigData;

		[FoldoutGroup("Debug", 0)]
		public bool DebugDrawCurrentAction;

		[FoldoutGroup("Attack references", 0)]
		public SnakeMeleeAttack OpenedMouthAttackLeft;

		[FoldoutGroup("Attack references", 0)]
		public SnakeMeleeAttack OpenedMouthAttackRight;

		[FoldoutGroup("Attack references", 0)]
		public SnakeMeleeAttack CastingOpenedMouthAttackLeft;

		[FoldoutGroup("Attack references", 0)]
		public SnakeMeleeAttack CastingOpenedMouthAttackRight;

		[FoldoutGroup("Attack references", 0)]
		public SnakeMeleeAttack OpenedToClosedAttackLeft;

		[FoldoutGroup("Attack references", 0)]
		public SnakeMeleeAttack OpenedToClosedAttackRight;

		[FoldoutGroup("Attack references", 0)]
		public SnakeMeleeAttack ChargedBiteAttackLeft;

		[FoldoutGroup("Attack references", 0)]
		public SnakeMeleeAttack ChargedBiteAttackRight;

		[FoldoutGroup("Attack references", 0)]
		public BossAreaSummonAttack ScalesSpikesFast;

		[FoldoutGroup("Attack references", 0)]
		public BossAreaSummonAttack ScalesSpikesSlow;

		[FoldoutGroup("Attack references", 0)]
		public BossAreaSummonAttack ScalesSpikesObstacles;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopLeftHead1;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopLeftHead2;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopLeftHead3;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopRightHead1;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopRightHead2;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopRightHead3;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopLeftHeadSeeking;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopRightHeadSeeking;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopLeftHeadBig1;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopLeftHeadBig2;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopRightHeadBig1;

		[FoldoutGroup("Attack references", 0)]
		public ElmFireTrapManager ElmFireLoopRightHeadBig2;

		[FoldoutGroup("Attack references", 0)]
		public BossStraightProjectileAttack TailSpikesAttack;

		[FoldoutGroup("Attack references", 0)]
		public Transform TailBeamShootingPoint;

		[FoldoutGroup("Attack references", 0)]
		public Transform TailOrbsCenterPoint;

		[FoldoutGroup("Attack references", 0)]
		public HomingTurretAttack HomingProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		public Transform OrbLeftPoint;

		[FoldoutGroup("Attack references", 0)]
		public Transform OrbRightPoint;

		[FoldoutGroup("Attack references", 0)]
		public BossHomingLaserAttack HomingLaserAttack;

		[FoldoutGroup("Attack references", 0)]
		public WindAreaEffect WindToTheRight;

		[FoldoutGroup("Attack references", 0)]
		public WindAreaEffect WindToTheLeft;

		[FoldoutGroup("Attack references", 0)]
		public WindAreaEffect WindAtTheTop;

		[FoldoutGroup("Death references", 0)]
		public Transform TpoWaypointLeftHead;

		[FoldoutGroup("Death references", 0)]
		public Transform TpoWaypointRightHead;

		[FoldoutGroup("Death references", 0)]
		public Transform TpoPositionForExecution;

		[FoldoutGroup("Death references", 0)]
		public Transform SnakePositionForExecution;

		[FoldoutGroup("Death references", 0)]
		public Transform SnakeDeathWaypointLeftHead;

		[FoldoutGroup("Death references", 0)]
		public Transform SnakeDeathWaypointRightHead;

		[FoldoutGroup("Death references", 0)]
		public GameObject ExecutionPrefab;

		[FoldoutGroup("Death references", 0)]
		public GameObject DeadSnakePrefab;

		[FoldoutGroup("Orbs attack additional settings", 0)]
		public List<SnakeBehaviour.OrbsSettings> AllOrbsAttacksSettings;

		[HideInInspector]
		public SnakeMeleeAttack CurrentMeleeAttackLeftHead;

		[HideInInspector]
		public SnakeMeleeAttack CurrentMeleeAttackRightHead;

		[HideInInspector]
		public bool WindAtTheTopActivated;

		private List<SnakeBehaviour.SNAKE_ATTACKS> availableAttacks = new List<SnakeBehaviour.SNAKE_ATTACKS>();

		[ShowInInspector]
		private List<SnakeBehaviour.SNAKE_ATTACKS> queuedAttacks = new List<SnakeBehaviour.SNAKE_ATTACKS>();

		private SnakeBehaviour.SnakeFightParameters currentFightParameters;

		private EnemyAction currentAction;

		private SnakeBehaviour.SNAKE_ATTACKS lastAttack = SnakeBehaviour.SNAKE_ATTACKS.DUMMY;

		private SnakeBehaviour.SNAKE_ATTACKS prevToLastAttack = SnakeBehaviour.SNAKE_ATTACKS.DUMMY;

		private Dictionary<SnakeBehaviour.SNAKE_ATTACKS, Func<EnemyAction>> actionsDictionary = new Dictionary<SnakeBehaviour.SNAKE_ATTACKS, Func<EnemyAction>>();

		private float extraRecoverySeconds;

		private WaitSeconds_EnemyAction waitBetweenActions_EA;

		private SnakeBehaviour.Intro_EnemyAction intro_EA;

		private SnakeBehaviour.GoUp_EnemyAction goUp_EA;

		private SnakeBehaviour.Death_EnemyAction death_EA;

		private SnakeBehaviour.ChargedBiteLeftHead_EnemyAction chargedBiteLeftHead_EA;

		private SnakeBehaviour.ChargedBiteRightHead_EnemyAction chargedBiteRightHead_EA;

		private SnakeBehaviour.ScalesSpikesLeftHead_EnemyAction scalesSpikesLeftHead_EA;

		private SnakeBehaviour.ScalesSpikesRightHead_EnemyAction scalesSpikesRightHead_EA;

		private SnakeBehaviour.ChainedLightningLeftHead_EnemyAction chainedLightningLeftHead_EA;

		private SnakeBehaviour.ChainedLightningRightHead_EnemyAction chainedLightningRightHead_EA;

		private SnakeBehaviour.BigChainedLightningLeftHead_EnemyAction bigChainedLightningLeftHead_EA;

		private SnakeBehaviour.BigChainedLightningRightHead_EnemyAction bigChainedLightningRightHead_EA;

		private SnakeBehaviour.ScalesSpikesRightHeadObstacles_EnemyAction scalesSpikesRightHeadObstacles_EA;

		private SnakeBehaviour.TailOrbs_EnemyAction tailOrbs_EA;

		private SnakeBehaviour.TailBeam_EnemyAction tailBeam_EA;

		private StateMachine<SnakeBehaviour> _fsm;

		private State<SnakeBehaviour> stIdle;

		private State<SnakeBehaviour> stAction;

		private State<SnakeBehaviour> stDeath;

		public enum SNAKE_ATTACKS
		{
			INTRO,
			CHARGED_BITE,
			SCALES_SPIKES,
			CHAINED_LIGHTNING = 4,
			GO_UP = 6,
			DEATH,
			TAIL_ORBS,
			TAIL_BEAM,
			SEEKING_CHAINED_LIGHTNING,
			BIG_CHAINED_LIGHTNING,
			SCALES_SPIKES_OBSTACLES,
			DUMMY = 999
		}

		public enum SNAKE_WEAPONS
		{
			CHARGING_OPEN_MOUTH,
			OPEN_TO_CLOSED,
			CASTING_OPEN_MOUTH,
			CHARGED_BITE
		}

		public enum SNAKE_PHASE
		{
			FIRST,
			SECOND,
			THIRD
		}

		[Serializable]
		public struct SnakeFightParameters
		{
			[EnumToggleButtons]
			public SnakeBehaviour.SNAKE_PHASE Phase;

			[ProgressBar(0.0, 1.0, 0.8f, 0f, 0.1f)]
			[SuffixLabel("%", false)]
			public float HpPercentageBeforeApply;

			[MinMaxSlider(0f, 5f, true)]
			public Vector2 MinMaxWaitingTimeBetweenActions;

			[SuffixLabel("health", true)]
			public int MaxHealthLostBeforeCounter;
		}

		[Serializable]
		public struct OrbsSettings
		{
			public int NumOrbs;

			[EnumToggleButtons]
			public EntityOrientation TailLocation;

			public float WaitTimeBetweenSpawns;

			public float WaitTimeBeforeFirstLaunch;

			public float WaitTimeBetweenLaunchs;
		}

		public class Intro_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				this.ACT_WAIT.StartAction(o, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.SnakeAnimatorInyector.BackgroundAnimationSetSpeed(1f, 1f);
				o.Snake.ShadowMaskSprites.ForEach(delegate(SpriteRenderer x)
				{
					x.gameObject.SetActive(false);
				});
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.3f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.3f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_WAIT.StartAction(o, 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.SnakeSegmentsMovementController.MoveToNextStage();
				o.Snake.Audio.PlaySnakePhaseMovement();
				this.ACT_WAIT.StartAction(o, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.5f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_WAIT.StartAction(o, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.5f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_WAIT.StartAction(o, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				GameObject leftHead = o.Snake.HeadLeft;
				Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
				Vector2 startPos = new Vector2(o.BattleBounds.xMin + 2f, o.BattleBounds.yMax - 1f);
				leftHead.transform.position = outOfCameraPos;
				o.Snake.Audio.PlaySnakeVanishIn();
				this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
				this.ACT_WAIT.StartAction(o, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.5f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_WAIT.StartAction(o, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.5f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.ShadowMaskSprites.ForEach(delegate(SpriteRenderer x)
				{
					x.gameObject.SetActive(true);
				});
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
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
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				p.Status.Invulnerable = true;
				Core.Audio.Ambient.SetSceneParam("BossDeath", 1f);
				o.DoActivateCollisionsIdle(false);
				o.DoActivateCollisionsOpenMouth(false);
				o.WindAtTheTopActivated = false;
				o.WindAtTheTop.IsDisabled = true;
				Core.Logic.ScreenFreeze.Freeze(0.05f, 0.2f, 0f, null);
				this.ACT_WAIT.StartAction(o, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				string blockerName = "TPOReposition";
				Core.Input.SetBlocker(blockerName, true);
				bool rightHeadShowing = o.Snake.IsRightHeadVisible;
				GameObject headToUse = (!rightHeadShowing) ? leftHead : rightHead;
				Vector2 snakeTargetPos = (!rightHeadShowing) ? o.SnakeDeathWaypointLeftHead.position : o.SnakeDeathWaypointRightHead.position;
				Vector2 tpoTargetPos = (!rightHeadShowing) ? o.TpoWaypointLeftHead.position : o.TpoWaypointRightHead.position;
				EntityOrientation targetOrientation = (!rightHeadShowing) ? EntityOrientation.Left : EntityOrientation.Right;
				p.DrivePlayer.MoveToPosition(tpoTargetPos, targetOrientation);
				float timeForSnakeMove = Vector2.Distance(snakeTargetPos, headToUse.transform.position) * 0.1f;
				this.ACT_MOVE.StartAction(o, snakeTargetPos, timeForSnakeMove, 7, headToUse.transform, true, null, true, true, 1.7f);
				o.Snake.ShadowMaskSprites.ForEach(delegate(SpriteRenderer x)
				{
					x.gameObject.SetActive(false);
				});
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.3f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.ClearAllAttacks();
				this.ACT_WAIT.StartAction(o, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_WAIT.StartAction(o, 0.05f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.15f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.ShadowMaskSprites.ForEach(delegate(SpriteRenderer x)
				{
					x.gameObject.SetActive(true);
				});
				yield return this.ACT_MOVE.waitForCompletion;
				o.Snake.SnakeAnimatorInyector.PlayDeath();
				this.ACT_WAIT.StartAction(o, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.SnakeAnimatorInyector.PlayDeathBite();
				this.ACT_WAIT.StartAction(o, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				float moveTime = 1.2f;
				this.ACT_MOVE.StartAction(o, tpoTargetPos, 1.2f, 26, headToUse.transform, true, null, true, true, 1.7f);
				this.ACT_WAIT.StartAction(o, moveTime * 0.85f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.SnakeAnimatorInyector.StopDeathBite();
				this.ACT_WAIT.StartAction(o, moveTime * 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				FadeWidget.instance.StartEasyFade(Color.clear, Color.black, 0f, true);
				o.Snake.SnakeAnimatorInyector.BackgroundAnimationSetActive(false);
				o.Snake.Audio.PlaySnakeDeath();
				this.ACT_WAIT.StartAction(o, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.Audio.PlaySnakeGrunt1();
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.Audio.PlaySnakeGrunt2();
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_WAIT.StartAction(o, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.HeadLeft.SetActive(false);
				o.Snake.HeadRight.SetActive(false);
				o.Snake.Tail.SetActive(false);
				o.Snake.ChainLeft.SetActive(false);
				o.Snake.ChainRight.SetActive(false);
				o.Snake.SnakeSegments.ForEach(delegate(SnakeSegmentVisualController x)
				{
					x.gameObject.SetActive(false);
				});
				p.Physics.EnablePhysics(false);
				p.Teleport(o.TpoPositionForExecution.position);
				p.SetOrientation(EntityOrientation.Right, true, false);
				p.Shadow.ManuallyControllingAlpha = true;
				p.Shadow.SetShadowAlpha(0f);
				o.Snake.SnakeSegmentsMovementController.InstantSetCamAsStart();
				Object.Instantiate<GameObject>(o.DeadSnakePrefab, o.SnakePositionForExecution.position, Quaternion.identity);
				GameObject executionGO = Object.Instantiate<GameObject>(o.ExecutionPrefab, o.SnakePositionForExecution.position, Quaternion.identity);
				FadeWidget.instance.StartEasyFade(Color.black, Color.clear, 2f, false);
				this.ACT_WAIT.StartAction(o, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				Player player = p.PlatformCharacterInput.Rewired;
				yield return new WaitUntil(() => player.GetButtonDown(8) || Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH));
				p.Physics.EnablePhysics(true);
				executionGO.GetComponent<FakeExecution>().UseEvenIfInputBlocked();
				this.ACT_WAIT.StartAction(this.owner, 2.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				Tween t = DOTween.To(() => Core.Logic.Penitent.Shadow.GetShadowAlpha(), delegate(float x)
				{
					Core.Logic.Penitent.Shadow.SetShadowAlpha(x);
				}, 1f, 0.2f);
				TweenSettingsExtensions.OnComplete<Tween>(t, delegate()
				{
					Core.Logic.Penitent.Shadow.ManuallyControllingAlpha = false;
				});
				this.ACT_WAIT.StartAction(this.owner, 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Input.SetBlocker(blockerName, false);
				PlayMakerFSM.BroadcastEvent("BOSS DEAD");
				p.Status.Invulnerable = false;
				base.FinishAction();
				Object.Destroy(o.gameObject);
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class ChargedBiteLeftHead_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, float waitTime, float moveTime, bool changesHead)
			{
				this.waitTime = waitTime;
				this.moveTime = moveTime;
				this.changesHead = changesHead;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				Vector2 startPos = new Vector2(o.BattleBounds.xMin + 2f, o.BattleBounds.yMax - 1f);
				Vector2 endPos = new Vector2(o.BattleBounds.xMax - 6f, o.BattleBounds.yMin + 0.6f);
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				if (!o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					leftHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Right, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_BITE);
				this.ACT_WAIT.StartAction(o, this.waitTime * 0.6f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector3 shockwaveStartPoint = leftHead.transform.position + new Vector3(1f, -2.25f, 0f);
				Vector3 shockwaveEndPoint = leftHead.transform.position + new Vector3(2f, -2.25f, 0f);
				GizmoExtensions.DrawDebugCross(shockwaveStartPoint, Color.yellow, 0.9f);
				GizmoExtensions.DrawDebugCross(shockwaveEndPoint, Color.red, 1f);
				o.ShoutShockwave(shockwaveStartPoint, shockwaveEndPoint);
				if (!o.WindAtTheTopActivated)
				{
					o.Snake.Audio.PlaySnakeWind();
					Sequence sequence = DOTween.Sequence();
					TweenSettingsExtensions.OnStart<Sequence>(sequence, delegate()
					{
						o.WindToTheRight.IsDisabled = false;
					});
					TweenSettingsExtensions.AppendInterval(sequence, 1f);
					TweenSettingsExtensions.OnComplete<Sequence>(sequence, delegate()
					{
						o.WindToTheRight.IsDisabled = true;
					});
					TweenExtensions.Play<Sequence>(sequence);
				}
				this.ACT_WAIT.StartAction(o, this.waitTime * 0.4f);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.BackgroundAnimationSetSpeed(3f, 0.5f);
				this.ACT_MOVE.StartAction(o, endPos, this.moveTime, 26, leftHead.transform, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				anim.BackgroundAnimationSetSpeed(1f, 0.5f);
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				this.ACT_WAIT.StartAction(o, this.waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.StopCloseMouth();
				anim.BackgroundAnimationSetSpeed(-1f, 0.5f);
				this.ACT_MOVE.StartAction(o, startPos, this.moveTime * 4f, 7, leftHead.transform, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				anim.BackgroundAnimationSetSpeed(1f, 0.5f);
				anim.PlayCloseMouth();
				if (this.changesHead)
				{
					Vector2 leftOutPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, leftOutPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					if (!o.queuedAttacks.Contains(SnakeBehaviour.SNAKE_ATTACKS.GO_UP))
					{
						Vector2 rightOutPos = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 1f);
						rightHead.transform.position = rightOutPos;
						Vector2 rightInPos = new Vector2(o.BattleBounds.xMax - 2f, o.BattleBounds.yMin + 2f);
						o.Snake.Audio.PlaySnakeVanishIn();
						this.ACT_MOVE.StartAction(o, rightInPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
						yield return this.ACT_MOVE.waitForCompletion;
					}
				}
				base.FinishAction();
				yield break;
			}

			private float waitTime;

			private float moveTime;

			private bool changesHead;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class ChargedBiteRightHead_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, float waitTime, float moveTime, bool changesHead)
			{
				this.waitTime = waitTime;
				this.moveTime = moveTime;
				this.changesHead = changesHead;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				Vector2 startPos = new Vector2(o.BattleBounds.xMax - 2f, o.BattleBounds.yMin + 2f);
				Vector2 pointA = new Vector2(o.SnakeRightCorner.transform.position.x, o.SnakeLeftCorner.transform.position.y);
				Vector2 pointB = new Vector2(o.SnakeLeftCorner.transform.position.x, o.SnakeRightCorner.transform.position.y);
				Vector2 dir = pointB - pointA;
				Vector2 endPos = startPos + dir * 0.3f;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				if (!o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 1f);
					rightHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Left, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_BITE);
				this.ACT_WAIT.StartAction(o, this.waitTime * 0.6f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector3 shockwaveStartPoint = rightHead.transform.position + new Vector3(-1f, -2.25f, 0f);
				Vector3 shockwaveEndPoint = rightHead.transform.position + new Vector3(-2f, -2.25f, 0f);
				GizmoExtensions.DrawDebugCross(shockwaveStartPoint, Color.yellow, 0.9f);
				GizmoExtensions.DrawDebugCross(shockwaveEndPoint, Color.red, 1f);
				o.ShoutShockwave(shockwaveStartPoint, shockwaveEndPoint);
				if (!o.WindAtTheTopActivated)
				{
					o.Snake.Audio.PlaySnakeWind();
					Sequence sequence = DOTween.Sequence();
					TweenSettingsExtensions.OnStart<Sequence>(sequence, delegate()
					{
						o.WindToTheLeft.IsDisabled = false;
					});
					TweenSettingsExtensions.AppendInterval(sequence, 1f);
					TweenSettingsExtensions.OnComplete<Sequence>(sequence, delegate()
					{
						o.WindToTheLeft.IsDisabled = true;
					});
					TweenExtensions.Play<Sequence>(sequence);
				}
				this.ACT_WAIT.StartAction(o, this.waitTime * 0.4f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_MOVE.StartAction(o, endPos, this.moveTime, 26, rightHead.transform, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				this.ACT_WAIT.StartAction(o, this.waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.StopCloseMouth();
				this.ACT_MOVE.StartAction(o, startPos, this.moveTime * 4f, 7, rightHead.transform, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				anim.PlayCloseMouth();
				if (this.changesHead)
				{
					Vector2 rightOutPos = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, rightOutPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					if (!o.queuedAttacks.Contains(SnakeBehaviour.SNAKE_ATTACKS.GO_UP))
					{
						Vector2 leftOutPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
						leftHead.transform.position = leftOutPos;
						Vector2 leftInPos = new Vector2(o.BattleBounds.xMin + 2f, o.BattleBounds.yMax - 1f);
						o.Snake.Audio.PlaySnakeVanishIn();
						this.ACT_MOVE.StartAction(o, leftInPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
						yield return this.ACT_MOVE.waitForCompletion;
					}
				}
				base.FinishAction();
				yield break;
			}

			private float waitTime;

			private float moveTime;

			private bool changesHead;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class ScalesSpikesLeftHead_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_TAILSPIKES.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				Vector2 startPos = new Vector2(o.BattleBounds.xMin + 2f, o.BattleBounds.yMax - 1f);
				if (!o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					leftHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Right, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_SUMMON_SPIKES);
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector3 scalesSpikesStartPos = o.SnakeRightCorner.position + Vector3.down * 0.5f;
				Vector3 scalesSpikesDir = (o.SnakeLeftCorner.position - o.SnakeRightCorner.position).normalized;
				o.ScalesSpikesFast.SummonAreas(scalesSpikesStartPos, scalesSpikesDir);
				this.ACT_WAIT.StartAction(o, 3f);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private SnakeBehaviour.TailSpikes_EnemyAction ACT_TAILSPIKES = new SnakeBehaviour.TailSpikes_EnemyAction();
		}

		public class ScalesSpikesRightHead_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_TAILSPIKES.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				Vector2 startPos = new Vector2(o.BattleBounds.xMax - 2f, o.BattleBounds.yMin + 1f);
				if (!o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 3f);
					rightHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Left, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_SUMMON_SPIKES);
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector3 scalesSpikesStartPos = o.SnakeLeftCorner.position + Vector3.down * 0.2f;
				Vector3 scalesSpikesDir = (o.SnakeRightCorner.position - o.SnakeLeftCorner.position).normalized;
				o.ScalesSpikesSlow.SummonAreas(scalesSpikesStartPos, scalesSpikesDir);
				this.ACT_WAIT.StartAction(o, 3f);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private SnakeBehaviour.TailSpikes_EnemyAction ACT_TAILSPIKES = new SnakeBehaviour.TailSpikes_EnemyAction();
		}

		public class ScalesSpikesRightHeadObstacles_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_TAILSPIKES.StopAction();
				SnakeBehaviour snakeBehaviour = this.owner as SnakeBehaviour;
				snakeBehaviour.ScalesSpikesObstacles.StopAllCoroutines();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				Vector2 startPos = new Vector2(o.BattleBounds.xMax - 2f, o.BattleBounds.yMin + 1f);
				if (!o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 3f);
					rightHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Left, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_SUMMON_SPIKES);
				this.ACT_WAIT.StartAction(o, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector3 scalesSpikesStartPos = o.SnakeLeftCorner.position + Vector3.down * 0.2f;
				Vector3 scalesSpikesDir = (o.SnakeRightCorner.position - o.SnakeLeftCorner.position).normalized;
				scalesSpikesStartPos += scalesSpikesDir * 2f;
				o.ScalesSpikesObstacles.SummonAreas(scalesSpikesStartPos, scalesSpikesDir);
				this.ACT_WAIT.StartAction(o, 3f);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private SnakeBehaviour.TailSpikes_EnemyAction ACT_TAILSPIKES = new SnakeBehaviour.TailSpikes_EnemyAction();
		}

		public class ChainedLightningLeftHead_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.elmFireLoopManager.InstantHideElmFireTraps();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, ElmFireTrapManager elmFireLoopManager, float waitTimeToShowEachTrap, float lightningChargeLapse, int numSteps, bool seeking = false)
			{
				this.elmFireLoopManager = elmFireLoopManager;
				this.waitTimeToShowEachTrap = waitTimeToShowEachTrap;
				this.lightningChargeLapse = lightningChargeLapse;
				this.numSteps = numSteps;
				this.seeking = seeking;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				Vector2 startPos = new Vector2(o.BattleBounds.xMin + 2f, o.BattleBounds.yMax - 1f);
				if (!o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					leftHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Right, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_CAST);
				if (this.seeking)
				{
					Vector3 position = p.GetPosition();
					float num = (position.x - o.SnakeLeftCorner.position.x) / (o.SnakeRightCorner.position.x - o.SnakeLeftCorner.position.x);
					float num2 = Mathf.Lerp(o.SnakeLeftCorner.position.y, o.SnakeRightCorner.position.y, num);
					this.elmFireLoopManager.transform.parent.position = new Vector2(position.x, num2);
				}
				this.elmFireLoopManager.ShowElmFireTrapRecursively(this.elmFireLoopManager.elmFireTrapNodes[0], this.waitTimeToShowEachTrap, this.lightningChargeLapse, false);
				yield return new WaitUntil(() => this.elmFireLoopManager.ElmFireLoopEndReached);
				this.ACT_WAIT.StartAction(o, this.waitTimeToShowEachTrap * (float)this.numSteps);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager.elmFireTrapNodes[0].SetCurrentCycleCooldownToMax();
				this.elmFireLoopManager.EnableTraps();
				this.ACT_WAIT.StartAction(o, this.lightningChargeLapse * (float)this.numSteps);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				this.elmFireLoopManager.DisableTraps();
				this.elmFireLoopManager.HideElmFireTrapRecursively(this.elmFireLoopManager.elmFireTrapNodes[0], 0.2f);
				yield return new WaitUntil(() => this.elmFireLoopManager.ElmFireLoopEndReached);
				base.FinishAction();
				yield break;
			}

			private ElmFireTrapManager elmFireLoopManager;

			private float waitTimeToShowEachTrap;

			private float lightningChargeLapse;

			private int numSteps;

			private bool seeking;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class ChainedLightningRightHead_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.elmFireLoopManager.InstantHideElmFireTraps();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, ElmFireTrapManager elmFireLoopManager, float waitTimeToShowEachTrap, float lightningChargeLapse, int numSteps, bool seeking = false)
			{
				this.elmFireLoopManager = elmFireLoopManager;
				this.waitTimeToShowEachTrap = waitTimeToShowEachTrap;
				this.lightningChargeLapse = lightningChargeLapse;
				this.numSteps = numSteps;
				this.seeking = seeking;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				Vector2 startPos = new Vector2(o.BattleBounds.xMax - 2f, o.BattleBounds.yMin + 1f);
				if (!o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 3f);
					rightHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Left, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_CAST);
				if (this.seeking)
				{
					Vector3 position = p.GetPosition();
					float num = (position.x - o.SnakeLeftCorner.position.x) / (o.SnakeRightCorner.position.x - o.SnakeLeftCorner.position.x);
					float num2 = Mathf.Lerp(o.SnakeLeftCorner.position.y, o.SnakeRightCorner.position.y, num);
					this.elmFireLoopManager.transform.parent.position = new Vector2(position.x, num2);
				}
				this.elmFireLoopManager.ShowElmFireTrapRecursively(this.elmFireLoopManager.elmFireTrapNodes[0], this.waitTimeToShowEachTrap, this.lightningChargeLapse, false);
				yield return new WaitUntil(() => this.elmFireLoopManager.ElmFireLoopEndReached);
				this.ACT_WAIT.StartAction(o, this.waitTimeToShowEachTrap * (float)this.numSteps);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager.elmFireTrapNodes[0].SetCurrentCycleCooldownToMax();
				this.elmFireLoopManager.EnableTraps();
				this.ACT_WAIT.StartAction(o, this.lightningChargeLapse * (float)this.numSteps);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				this.elmFireLoopManager.DisableTraps();
				this.elmFireLoopManager.HideElmFireTrapRecursively(this.elmFireLoopManager.elmFireTrapNodes[0], 0.2f);
				yield return new WaitUntil(() => this.elmFireLoopManager.ElmFireLoopEndReached);
				base.FinishAction();
				yield break;
			}

			private ElmFireTrapManager elmFireLoopManager;

			private float waitTimeToShowEachTrap;

			private float lightningChargeLapse;

			private int numSteps;

			private bool seeking;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class BigChainedLightningLeftHead_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.elmFireLoopManager1.InstantHideElmFireTraps();
				this.elmFireLoopManager2.InstantHideElmFireTraps();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, ElmFireTrapManager elmFireLoopManager1, float waitTimeToShowEachTrap1, float lightningChargeLapse1, int numSteps1, ElmFireTrapManager elmFireLoopManager2, float waitTimeToShowEachTrap2, float lightningChargeLapse2, int numSteps2, float timeBetweenBothLoop)
			{
				this.elmFireLoopManager1 = elmFireLoopManager1;
				this.elmFireLoopManager2 = elmFireLoopManager2;
				this.waitTimeToShowEachTrap1 = waitTimeToShowEachTrap1;
				this.waitTimeToShowEachTrap2 = waitTimeToShowEachTrap2;
				this.lightningChargeLapse1 = lightningChargeLapse1;
				this.lightningChargeLapse2 = lightningChargeLapse2;
				this.numSteps1 = numSteps1;
				this.numSteps2 = numSteps2;
				this.timeBetweenBothLoop = timeBetweenBothLoop;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				Vector2 startPos = new Vector2(o.BattleBounds.xMin + 2f, o.BattleBounds.yMax - 1f);
				if (!o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					leftHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Right, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_CAST);
				this.elmFireLoopManager1.ShowElmFireTrapRecursively(this.elmFireLoopManager1.elmFireTrapNodes[0], this.waitTimeToShowEachTrap1, this.lightningChargeLapse1, false);
				yield return new WaitUntil(() => this.elmFireLoopManager1.ElmFireLoopEndReached);
				this.ACT_WAIT.StartAction(o, this.waitTimeToShowEachTrap1 * (float)this.numSteps1);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager1.elmFireTrapNodes[0].SetCurrentCycleCooldownToMax();
				this.elmFireLoopManager1.EnableTraps();
				this.ACT_WAIT.StartAction(o, this.lightningChargeLapse1 * (float)this.numSteps1);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager1.DisableTraps();
				this.elmFireLoopManager1.HideElmFireTrapRecursively(this.elmFireLoopManager1.elmFireTrapNodes[0], 0.2f);
				yield return new WaitUntil(() => this.elmFireLoopManager1.ElmFireLoopEndReached);
				this.ACT_WAIT.StartAction(o, this.timeBetweenBothLoop);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager2.ShowElmFireTrapRecursively(this.elmFireLoopManager2.elmFireTrapNodes[0], this.waitTimeToShowEachTrap2, this.lightningChargeLapse2, false);
				yield return new WaitUntil(() => this.elmFireLoopManager2.ElmFireLoopEndReached);
				this.ACT_WAIT.StartAction(o, this.waitTimeToShowEachTrap2 * (float)this.numSteps2);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager2.elmFireTrapNodes[0].SetCurrentCycleCooldownToMax();
				this.elmFireLoopManager2.EnableTraps();
				this.ACT_WAIT.StartAction(o, this.lightningChargeLapse2 * (float)this.numSteps2);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager2.DisableTraps();
				this.elmFireLoopManager2.HideElmFireTrapRecursively(this.elmFireLoopManager2.elmFireTrapNodes[0], 0.2f);
				yield return new WaitUntil(() => this.elmFireLoopManager2.ElmFireLoopEndReached);
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				base.FinishAction();
				yield break;
			}

			private ElmFireTrapManager elmFireLoopManager1;

			private ElmFireTrapManager elmFireLoopManager2;

			private float waitTimeToShowEachTrap1;

			private float waitTimeToShowEachTrap2;

			private float lightningChargeLapse1;

			private float lightningChargeLapse2;

			private int numSteps1;

			private int numSteps2;

			private float timeBetweenBothLoop;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class BigChainedLightningRightHead_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.elmFireLoopManager1.InstantHideElmFireTraps();
				this.elmFireLoopManager2.InstantHideElmFireTraps();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, ElmFireTrapManager elmFireLoopManager1, float waitTimeToShowEachTrap1, float lightningChargeLapse1, int numSteps1, ElmFireTrapManager elmFireLoopManager2, float waitTimeToShowEachTrap2, float lightningChargeLapse2, int numSteps2, float timeBetweenBothLoop)
			{
				this.elmFireLoopManager1 = elmFireLoopManager1;
				this.elmFireLoopManager2 = elmFireLoopManager2;
				this.waitTimeToShowEachTrap1 = waitTimeToShowEachTrap1;
				this.waitTimeToShowEachTrap2 = waitTimeToShowEachTrap2;
				this.lightningChargeLapse1 = lightningChargeLapse1;
				this.lightningChargeLapse2 = lightningChargeLapse2;
				this.numSteps1 = numSteps1;
				this.numSteps2 = numSteps2;
				this.timeBetweenBothLoop = timeBetweenBothLoop;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				if (o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				Vector2 startPos = new Vector2(o.BattleBounds.xMax - 2f, o.BattleBounds.yMin + 1f);
				if (!o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 3f);
					rightHead.transform.position = outOfCameraPos2;
					o.Snake.Audio.PlaySnakeVanishIn();
					this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.SetOrientation(EntityOrientation.Left, false, false);
				anim.StopCloseMouth();
				anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_CAST);
				this.elmFireLoopManager1.ShowElmFireTrapRecursively(this.elmFireLoopManager1.elmFireTrapNodes[0], this.waitTimeToShowEachTrap1, this.lightningChargeLapse1, false);
				yield return new WaitUntil(() => this.elmFireLoopManager1.ElmFireLoopEndReached);
				this.ACT_WAIT.StartAction(o, this.waitTimeToShowEachTrap1 * (float)this.numSteps1);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager1.elmFireTrapNodes[0].SetCurrentCycleCooldownToMax();
				this.elmFireLoopManager1.EnableTraps();
				this.ACT_WAIT.StartAction(o, this.lightningChargeLapse1 * (float)this.numSteps1);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager1.DisableTraps();
				this.elmFireLoopManager1.HideElmFireTrapRecursively(this.elmFireLoopManager1.elmFireTrapNodes[0], 0.2f);
				yield return new WaitUntil(() => this.elmFireLoopManager1.ElmFireLoopEndReached);
				this.ACT_WAIT.StartAction(o, this.timeBetweenBothLoop);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager2.ShowElmFireTrapRecursively(this.elmFireLoopManager2.elmFireTrapNodes[0], this.waitTimeToShowEachTrap2, this.lightningChargeLapse2, false);
				yield return new WaitUntil(() => this.elmFireLoopManager2.ElmFireLoopEndReached);
				this.ACT_WAIT.StartAction(o, this.waitTimeToShowEachTrap2 * (float)this.numSteps2);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager2.elmFireTrapNodes[0].SetCurrentCycleCooldownToMax();
				this.elmFireLoopManager2.EnableTraps();
				this.ACT_WAIT.StartAction(o, this.lightningChargeLapse2 * (float)this.numSteps2);
				yield return this.ACT_WAIT.waitForCompletion;
				this.elmFireLoopManager2.DisableTraps();
				this.elmFireLoopManager2.HideElmFireTrapRecursively(this.elmFireLoopManager2.elmFireTrapNodes[0], 0.2f);
				yield return new WaitUntil(() => this.elmFireLoopManager2.ElmFireLoopEndReached);
				anim.StopOpenMouth();
				anim.PlayCloseMouth();
				base.FinishAction();
				yield break;
			}

			private ElmFireTrapManager elmFireLoopManager1;

			private ElmFireTrapManager elmFireLoopManager2;

			private float waitTimeToShowEachTrap1;

			private float waitTimeToShowEachTrap2;

			private float lightningChargeLapse1;

			private float lightningChargeLapse2;

			private int numSteps1;

			private int numSteps2;

			private float timeBetweenBothLoop;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class TailSpikes_EnemyAction : EnemyAction
		{
		}

		public class TailOrbs_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.projectiles.ForEach(delegate(HomingProjectile x)
				{
					x.SetTTL(0f);
				});
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, int numOrbs, bool shootFromRightSide, bool careForHeadAnims)
			{
				this.numOrbs = numOrbs;
				this.shootFromRightSide = shootFromRightSide;
				this.careForHeadAnims = careForHeadAnims;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				GameObject tail = o.Snake.Tail;
				Animator tailAnimator = o.Snake.TailAnimator;
				if (this.careForHeadAnims)
				{
					if (!o.Snake.IsRightHeadVisible && !o.Snake.IsLeftHeadVisible)
					{
						Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
						leftHead.transform.position = outOfCameraPos;
						o.Snake.Audio.PlaySnakeVanishIn();
						Vector2 startPos = new Vector2(o.BattleBounds.xMin + 2f, o.BattleBounds.yMax - 1f);
						this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
						yield return this.ACT_MOVE.waitForCompletion;
						this.shootFromRightSide = false;
					}
					anim.StopCloseMouth();
					anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_CAST);
				}
				string triggerName = (!this.shootFromRightSide) ? "RIGHT_TAIL" : "LEFT_TAIL";
				EntityOrientation tailLocation = (!this.shootFromRightSide) ? EntityOrientation.Right : EntityOrientation.Left;
				tailAnimator.SetBool(triggerName, true);
				o.Snake.Audio.PlaySnakeTail();
				this.ACT_WAIT.StartAction(o, 0.35f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.Audio.PlaySnakeElectricTail();
				this.ACT_WAIT.StartAction(o, 0.35f);
				yield return this.ACT_WAIT.waitForCompletion;
				SnakeBehaviour.OrbsSettings settings = o.AllOrbsAttacksSettings.Find((SnakeBehaviour.OrbsSettings x) => x.NumOrbs == this.numOrbs && x.TailLocation == tailLocation);
				this.projectiles.Clear();
				for (int i = 0; i < this.numOrbs; i++)
				{
					o.HomingProjectileAttack.UseEntityPosition = false;
					o.HomingProjectileAttack.UseEntityOrientation = false;
					o.HomingProjectileAttack.ShootingPoint = o.TailOrbsCenterPoint;
					o.HomingProjectileAttack.OffsetPosition = o.OrbLeftPoint.localPosition;
					HomingProjectile p = o.HomingProjectileAttack.FireProjectileToTarget(o.OrbRightPoint.position);
					p.timeToLive = 30f;
					p.ResetTTL();
					p.ResetSpeed();
					p.ResetRotateSpeed();
					p.ChangesRotatesSpeedInFlight = true;
					p.currentDirection = Vector2.left;
					p.TargetOffset = Vector2.zero;
					p.DestroyedOnReachingTarget = false;
					p.ChangeTargetToAlternative(o.TailOrbsCenterPoint, 0.75f, 1f, 1f);
					p.GetComponentInChildren<GhostTrailGenerator>().EnableGhostTrail = false;
					this.projectiles.Add(p);
					this.ACT_WAIT.StartAction(o, settings.WaitTimeBetweenSpawns);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				this.ACT_WAIT.StartAction(o, settings.WaitTimeBeforeFirstLaunch);
				yield return this.ACT_WAIT.waitForCompletion;
				foreach (HomingProjectile p2 in this.projectiles)
				{
					p2.timeToLive = 3f;
					p2.ResetTTL();
					p2.ChangeTargetToPenitent(false);
					p2.TargetOffset = Vector3.up + Random.insideUnitSphere * 0.5f;
					p2.GetComponentInChildren<GhostTrailGenerator>().EnableGhostTrail = true;
					p2.OnDisableEvent += this.DeactivateGhostTrail;
					p2.ChangesRotatesSpeedInFlight = false;
					p2.ResetRotateSpeed();
					this.ACT_WAIT.StartAction(o, settings.WaitTimeBetweenLaunchs);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				tailAnimator.SetBool(triggerName, false);
				o.Snake.Audio.StopSnakeElectricTail();
				if (this.careForHeadAnims)
				{
					anim.StopOpenMouth();
					anim.PlayCloseMouth();
				}
				yield return new WaitUntil(() => tailAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE"));
				base.FinishAction();
				yield break;
			}

			private void DeactivateGhostTrail(Projectile obj)
			{
				GhostTrailGenerator componentInChildren = obj.GetComponentInChildren<GhostTrailGenerator>();
				componentInChildren.EnableGhostTrail = false;
			}

			private int numOrbs;

			private bool shootFromRightSide;

			private bool careForHeadAnims;

			private List<HomingProjectile> projectiles = new List<HomingProjectile>();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class TailBeam_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				SnakeBehaviour snakeBehaviour = this.owner as SnakeBehaviour;
				snakeBehaviour.HomingLaserAttack.StopBeam();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, bool shootFromRightSide, bool careForHeadAnims)
			{
				this.shootFromRightSide = shootFromRightSide;
				this.careForHeadAnims = careForHeadAnims;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				GameObject tail = o.Snake.Tail;
				Animator tailAnimator = o.Snake.TailAnimator;
				if (this.careForHeadAnims)
				{
					if (!o.Snake.IsRightHeadVisible && !o.Snake.IsLeftHeadVisible)
					{
						Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
						leftHead.transform.position = outOfCameraPos;
						o.Snake.Audio.PlaySnakeVanishIn();
						Vector2 startPos = new Vector2(o.BattleBounds.xMin + 2f, o.BattleBounds.yMax - 1f);
						this.ACT_MOVE.StartAction(o, startPos, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
						yield return this.ACT_MOVE.waitForCompletion;
						this.shootFromRightSide = false;
					}
					anim.StopCloseMouth();
					anim.PlayOpenMouth(SnakeAnimatorInyector.OPEN_MOUTH_INTENTIONS.TO_CAST);
				}
				string triggerName = (!this.shootFromRightSide) ? "RIGHT_TAIL" : "LEFT_TAIL";
				tailAnimator.SetBool(triggerName, true);
				o.Snake.Audio.PlaySnakeTail();
				this.ACT_WAIT.StartAction(o, 0.35f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.Audio.PlaySnakeElectricTail();
				this.ACT_WAIT.StartAction(o, 0.35f);
				yield return this.ACT_WAIT.waitForCompletion;
				float warningTime = 1f;
				float beamTime = 3f;
				o.HomingLaserAttack.transform.position = o.TailBeamShootingPoint.position;
				o.HomingLaserAttack.DelayedTargetedBeam(Core.Logic.Penitent.transform, warningTime, beamTime, EntityOrientation.Right, false);
				float beamSfxSyncTime = 0.2f;
				this.ACT_WAIT.StartAction(o, warningTime - beamSfxSyncTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.Audio.PlaySnakeElectricShot();
				this.ACT_WAIT.StartAction(o, beamSfxSyncTime);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.CameraManager.ProCamera2DShake.Shake(beamTime - warningTime, Vector2.down, 50, 0.2f, 0.01f, default(Vector3), 0.1f, false);
				this.ACT_WAIT.StartAction(o, beamTime - warningTime);
				yield return this.ACT_WAIT.waitForCompletion;
				tailAnimator.SetBool(triggerName, false);
				o.Snake.Audio.StopSnakeElectricTail();
				o.Snake.Audio.StopSnakeElectricShot();
				if (this.careForHeadAnims)
				{
					anim.StopOpenMouth();
					anim.PlayCloseMouth();
				}
				yield return new WaitUntil(() => tailAnimator.GetCurrentAnimatorStateInfo(0).IsName("IDLE"));
				base.FinishAction();
				yield break;
			}

			private bool shootFromRightSide;

			private bool careForHeadAnims;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class TailSweep_EnemyAction : EnemyAction
		{
		}

		public class GoUp_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				SnakeBehaviour o = this.owner as SnakeBehaviour;
				Penitent p = Core.Logic.Penitent;
				SnakeAnimatorInyector anim = o.Snake.SnakeAnimatorInyector;
				GameObject leftHead = o.Snake.HeadLeft;
				GameObject rightHead = o.Snake.HeadRight;
				SpriteRenderer leftHeadSprite = o.Snake.HeadLeftSprite;
				SpriteRenderer rightHeadSprite = o.Snake.HeadRightSprite;
				GameObject tail = o.Snake.Tail;
				o.Snake.ShadowMaskSprites.ForEach(delegate(SpriteRenderer x)
				{
					x.gameObject.SetActive(false);
				});
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.3f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.ScalesSpikesObstacles.ClearAll();
				this.ACT_WAIT.StartAction(o, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_WAIT.StartAction(o, 0.05f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.15f);
				o.Snake.Audio.PlaySnakeThunder();
				this.ACT_WAIT.StartAction(o, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Snake.ShadowMaskSprites.ForEach(delegate(SpriteRenderer x)
				{
					x.gameObject.SetActive(true);
				});
				if (o.Snake.IsRightHeadVisible)
				{
					Vector2 outOfCameraPos = new Vector2(o.BattleBounds.xMax + 8f, o.BattleBounds.yMin + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos, 1.5f, 7, rightHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				else if (o.Snake.IsLeftHeadVisible)
				{
					Vector2 outOfCameraPos2 = new Vector2(o.BattleBounds.xMin - 8f, o.BattleBounds.yMax + 1f);
					o.Snake.Audio.PlaySnakeVanishOut();
					this.ACT_MOVE.StartAction(o, outOfCameraPos2, 1.5f, 7, leftHead.transform, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				o.Snake.Audio.IncreaseSnakeRainState();
				o.Snake.Audio.PlaySnakePhaseMovement();
				tail.transform.position += Vector3.up * 15f;
				SnakeSegmentsMovementController.STAGES curStage = o.Snake.SnakeSegmentsMovementController.CurrentStage;
				o.Snake.SnakeSegmentsMovementController.MoveToNextStage();
				float thunderSeconds = 1f;
				this.ACT_WAIT.StartAction(o, thunderSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.CameraManager.ScreenEffectsManager.ScreenModeThunder(0.3f);
				o.Snake.Audio.PlaySnakeThunder();
				yield return new WaitUntil(() => curStage != o.Snake.SnakeSegmentsMovementController.CurrentStage);
				if (o.Snake.SnakeSegmentsMovementController.CurrentStage == SnakeSegmentsMovementController.STAGES.STAGE_TWO)
				{
					o.WindAtTheTopActivated = true;
					o.WindAtTheTop.IsDisabled = false;
					o.Snake.Audio.PlaySnakeWind();
				}
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}
	}
}
