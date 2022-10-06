using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BezierSplines;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.Damage;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Maikel.StatelessFSM;
using Maikel.SteeringBehaviors;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Amanecidas
{
	public class AmanecidasBehaviour : EnemyBehaviour
	{
		public Amanecidas Amanecidas { get; set; }

		private void Start()
		{
			this.Amanecidas = (Amanecidas)this.Entity;
			this.agent = base.GetComponent<AutonomousAgent>();
			this.waitBetweenActions_EA = new WaitSeconds_EnemyAction();
			this.hurtDisplacement_EA = new AmanecidasBehaviour.HurtDisplacement_EnemyAction();
			this.jumpBackNShoot_EA = new AmanecidasBehaviour.JumpBackAndShoot_EnemyAction();
			this.shootRicochetArrow_EA = new AmanecidasBehaviour.ShootRicochetArrow_EnemyAction();
			this.shootLaserArrow_EA = new AmanecidasBehaviour.ShootLaserArrow_EnemyAction();
			this.shootMineArrows_EA = new AmanecidasBehaviour.ShootMineArrows_EnemyAction();
			this.freezeTimeBlinkShots_EA = new AmanecidasBehaviour.FreezeTimeBlinkShots_EnemyAction();
			this.freezeTimeMultiShots_EA = new AmanecidasBehaviour.FreezeTimeMultiShots_EnemyAction();
			this.fastShot_EA = new AmanecidasBehaviour.FastShot_EnemyAction();
			this.fastShots_EA = new AmanecidasBehaviour.FastShots_EnemyAction();
			this.chargedShot_EA = new AmanecidasBehaviour.ChargedShot_EnemyAction();
			this.spikesBlinkShots_EA = new AmanecidasBehaviour.SpikesBlinkShots_EnemyAction();
			this.freezeTimeNRicochetShots_ECA = new AmanecidasBehaviour.FreezeTimeNRicochetShots_EnemyComboAction();
			this.shootFrozenLances_EA = new AmanecidasBehaviour.ShootFrozenLances_EnemyAction();
			this.doubleShootFrozenLances_EA = new AmanecidasBehaviour.DoubleShootFrozenLances_EnemyAction();
			this.horizontalBlinkDashes_EA = new AmanecidasBehaviour.HorizontalBlinkDashes_EnemyAction();
			this.multiFrontalDash_EA = new AmanecidasBehaviour.MultiFrontalDash_EnemyAction();
			this.diagonalBlinkDashes_EA = new AmanecidasBehaviour.DiagonalBlinkDashes_EnemyAction();
			this.freezeTimeNHorizontalDashes_ECA = new AmanecidasBehaviour.FreezeTimeNHorizontalDashes_EnemyComboAction();
			this.dualAxeThrow_EA = new AmanecidasBehaviour.LaunchTwoAxesHorizontal_EnemyAction();
			this.dualAxeFlyingThrow_EA = new AmanecidasBehaviour.FlyAndLaunchTwoAxes_EnemyAction();
			this.spinAxesAround_EA = new AmanecidasBehaviour.SpinAxesAround_EnemyAction();
			this.followAndSpinAxes_EA = new AmanecidasBehaviour.FollowSplineAndSpinAxesAround_EnemyAction();
			this.jumpSmash_EA = new AmanecidasBehaviour.JumpSmash_EnemyAction();
			this.multiStompNLavaBalls_ECA = new AmanecidasBehaviour.MultiStompNLavaBalls_EnemyComboAction();
			this.blink_EA = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();
			this.blinkDash_EA = new AmanecidasBehaviour.BlinkAndDashToPenitent_EnemyAction();
			this.chainDash_EA = new AmanecidasBehaviour.ChainDash_EnemyAction();
			this.falcataSlashProjectile_EA = new AmanecidasBehaviour.FalcataSlashProjectile_EnemyAction();
			this.falcataSlashBarrage_EA = new AmanecidasBehaviour.FalcataSlashBarrage_EnemyAction();
			this.meleeAttack_EA = new AmanecidasBehaviour.MeleeAttackTowardsPenitent_EnemyAction();
			this.meleeProjectile_EA = new AmanecidasBehaviour.MeleeAttackProjectile_EnemyAction();
			this.ghostProjectile_EA = new AmanecidasBehaviour.GhostProjectile_EnemyAction();
			this.recoverShield_EA = new AmanecidasBehaviour.RechargeShield_EnemyAction();
			this.stompAttack_EA = new AmanecidasBehaviour.StompAttack_EnemyAction();
			this.launchAxesToPenitent_EA = new AmanecidasBehaviour.LaunchAxesToPenitent_EnemyAction();
			this.launchCrawlerAxesToPenitent_EA = new AmanecidasBehaviour.LaunchCrawlerAxesToPenitent_EnemyAction();
			this.launchBallsToPenitent_EA = new AmanecidasBehaviour.LaunchBallsToPenitent_EnemyAction();
			this.multiStompAttack_EA = new AmanecidasBehaviour.MultiStompAttack_EnemyAction();
			this.changeWeapon_EA = new AmanecidasBehaviour.ChangeWeapon_EnemyAction();
			this.quickLunge_EA = new AmanecidasBehaviour.QuickLunge_EnemyAction();
			this.falcataSlashStorm_ECA = new AmanecidasBehaviour.FalcataSlashStorm_EnemyComboAction();
			this.intro_EA = new AmanecidasBehaviour.Intro_EnemyAction();
			this.death_EA = new AmanecidasBehaviour.Death_EnemyAction();
			this.falcataCounter_EA = new AmanecidasBehaviour.DodgeAndCounterAttack_EnemyAction();
			this.shieldShockwave_EA = new AmanecidasBehaviour.ShieldShockwave();
			this.chargeEnergy_EA = new AmanecidasBehaviour.EnergyChargePeriod_EnemyAction();
			this.moveBattleBounds_EA = new AmanecidasBehaviour.MoveBattleBounds_EnemyAction();
			this.spinAttack = new AmanecidasBehaviour.FalcataSlashOnFloor_EnemyAction();
			this.hurt_EA = new AmanecidasBehaviour.Hurt_EnemyAction();
			this.damageParameters = new Dictionary<string, float>();
			this.SetBalanceData();
			this.bullets = new List<BulletTimeProjectile>();
			this.multiBullets = new List<BulletTimeProjectile>();
			this.mines = new List<BossSpawnedAreaAttack>();
			PoolManager.Instance.CreatePool(this.amanecidaAxePrefab, 2);
			PoolManager.Instance.CreatePool(this.poolSplinePrefab, 2);
			PoolManager.Instance.CreatePool(this.spikePrefab.gameObject, 30);
			PoolManager.Instance.CreatePool(this.vortexVFX, 2);
			PoolManager.Instance.CreatePool(this.dustVFX, 2);
			PoolManager.Instance.CreatePool(this.shieldShockwave, 3);
			PoolManager.Instance.CreatePool(this.clonePrefab, 2);
			this.results = new RaycastHit2D[1];
			this.SpawnBothAxes();
			this.InitAI();
			this.InitActionDictionary();
			this.InitCombatArea();
			this.Amanecidas.AnimatorInyector.SetAmanecidaWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.HAND);
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnDestroy()
		{
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.penitentHealing = penitent.GetComponentInChildren<Healing>();
		}

		public void SetWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON wpn)
		{
			this.availableAttacks.Clear();
			this.currentWeapon = wpn;
			if (this.Amanecidas == null)
			{
				this.Amanecidas = (Amanecidas)this.Entity;
			}
			if (this.Amanecidas.IsLaudes)
			{
				if (this.currentFightParameterList == null)
				{
					this.currentFightParameterList = this.laudesFightParameters;
					this.currentFightParameters = this.currentFightParameterList[0];
				}
				this.availableAttacks = this.laudesAttackConfigData.GetAttackIdsByWeapon(wpn, false, 1f);
			}
			else
			{
				switch (this.currentWeapon)
				{
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.HAND:
					this.currentFightParameterList = this.falcataFightParameters;
					break;
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE:
					this.currentFightParameterList = this.axeFightParameters;
					this.lastAttack = AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlameBlade;
					this.attackBeforeLastAttack = AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlameBlade;
					break;
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.LANCE:
					this.currentFightParameterList = this.lanceFightParameters;
					this.lastAttack = AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_HorizontalBlinkDashes;
					this.attackBeforeLastAttack = AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_HorizontalBlinkDashes;
					break;
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW:
					this.currentFightParameterList = this.bowFightParameters;
					this.lastAttack = AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot;
					this.attackBeforeLastAttack = AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot;
					break;
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD:
					this.currentFightParameterList = this.falcataFightParameters;
					this.lastAttack = AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_SpinAttack;
					this.attackBeforeLastAttack = AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_SpinAttack;
					break;
				}
				this.currentFightParameters = this.currentFightParameterList[0];
				this.availableAttacks = this.attackConfigData.GetAttackIdsByWeapon(wpn, true, this.GetHpPercentage());
			}
			this.currentMeleeAttack = ((this.currentWeapon != AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE) ? this.meleeFalcataAttack : this.meleeAxeAttack);
		}

		private float GetHpPercentage()
		{
			return this.Amanecidas.GetHpPercentage();
		}

		private void InitAI()
		{
			this.stFloating = new Amanecidas_StFloating();
			this.stAction = new Amanecidas_StAction();
			this.stDeath = new Amanecidas_StAction();
			this.stIntro = new Amanecidas_StAction();
			this.stHurt = new Amanecidas_StHurt();
			this.stRecharge = new Amanecidas_StRecharging();
			this._fsm = new StateMachine<AmanecidasBehaviour>(this, this.stIntro, null, null);
			this.actionsBeforeShieldRecharge = this.currentFightParameters.maxActionsBeforeShieldRecharge;
		}

		private void InitCombatArea()
		{
			this.combatAreaParent.SetParent(null);
			this.combatAreaParent.transform.position = this.battleBounds.center;
		}

		private void InitActionDictionary()
		{
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlyAndSpin, new Func<EnemyAction>(this.LaunchAction_FlyAndSpin));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_DualThrow, new Func<EnemyAction>(this.LaunchAction_DualThrow));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_JumpSmash, new Func<EnemyAction>(this.LaunchAction_JumpSmash));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_JumpSmashWithPillars, new Func<EnemyAction>(this.LaunchAction_JumpSmashWithPillars));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_DualThrowCross, new Func<EnemyAction>(this.LaunchAction_DualThrowCross));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlyAndToss, new Func<EnemyAction>(this.LaunchAction_FlyAndToss));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_MeleeAttack, new Func<EnemyAction>(this.LaunchAction_AxeMeleeAttack));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlameBlade, new Func<EnemyAction>(this.LaunchAction_FlameBlade));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndAxeToss, new Func<EnemyAction>(this.LaunchAction_FollowAndTossAxes));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndCrawlerAxeToss, new Func<EnemyAction>(this.LaunchAction_FollowAndTossCrawlerAxes));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndLavaBall, new Func<EnemyAction>(this.LaunchAction_FollowAndLavaBall));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_COMBO_StompNLavaBalls, new Func<EnemyAction>(this.LaunchComboAction_StompNLavaBalls));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FreezeTimeBlinkShots, new Func<EnemyAction>(this.LaunchAction_FreezeTimeBlinkShots));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_RicochetShot, new Func<EnemyAction>(this.LaunchAction_RicochetShot));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_MineShots, new Func<EnemyAction>(this.LaunchAction_MineShots));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FreezeTimeMultiShots, new Func<EnemyAction>(this.LaunchAction_FreezeTimeMultiShots));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot, new Func<EnemyAction>(this.LaunchAction_FastShot));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShots, new Func<EnemyAction>(this.LaunchAction_FastShots));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_ChargedShot, new Func<EnemyAction>(this.LaunchAction_ChargedShot));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_SpikesBlinkShot, new Func<EnemyAction>(this.LaunchAction_SpikesBlinkShot));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_COMBO_FreezeTimeNRicochetShots, new Func<EnemyAction>(this.LaunchComboAction_FreezeTimeNRicochetShots));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_LaserShot, new Func<EnemyAction>(this.LaunchAction_LaserShot));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_BlinkDash, new Func<EnemyAction>(this.LaunchAction_LanceBlinkDash));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeLances, new Func<EnemyAction>(this.LaunchAction_FreezeTimeLances));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeLancesOnPenitent, new Func<EnemyAction>(this.LaunchAction_FreezeTimeLancesOnPenitent));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeHail, new Func<EnemyAction>(this.LaunchAction_FreezeTimeHail));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_JumpBackAndDash, new Func<EnemyAction>(this.LaunchAction_JumpBackNDash));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_HorizontalBlinkDashes, new Func<EnemyAction>(this.LaunchAction_HorizontalBlinkDashes));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_MultiFrontalDash, new Func<EnemyAction>(this.LaunchAction_MultiFrontalDash));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_DiagonalBlinkDashes, new Func<EnemyAction>(this.LaunchAction_DiagonalBlinkDashes));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_COMBO_FreezeTimeNHorizontalDashes, new Func<EnemyAction>(this.LaunchComboAction_FreezeTimeNHorizontalDashes));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_BlinkDash, new Func<EnemyAction>(this.LaunchAction_FalcataBlinkBehindAndDash));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_MeleeAttack, new Func<EnemyAction>(this.LaunchAction_FalcataMeleeAttack));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_SpinAttack, new Func<EnemyAction>(this.LaunchAction_SpinAttack));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_SlashProjectile, new Func<EnemyAction>(this.LaunchAction_FalcataSlashProjectile));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_SlashBarrage, new Func<EnemyAction>(this.LaunchAction_FalcataSlashBarrage));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_QuickLunge, new Func<EnemyAction>(this.LaunchAction_MultiLunge));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_COMBO_STORM, new Func<EnemyAction>(this.LaunchAction_FalcataCombo));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_CounterAttack, new Func<EnemyAction>(this.LaunchAction_Counter));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_ChainDash, new Func<EnemyAction>(this.LaunchAction_ChainDash));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_NoxiousBlade, new Func<EnemyAction>(this.LaunchAction_NoxiousBlade));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_BlinkAway, new Func<EnemyAction>(this.LaunchAction_BlinkAway));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_RechargeShield, new Func<EnemyAction>(this.LaunchAction_RechargeShield));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_Intro, new Func<EnemyAction>(this.LaunchAction_Intro));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_StompAttack, new Func<EnemyAction>(this.LaunchAction_StompSmash));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_ChangeWeapon, new Func<EnemyAction>(this.LaunchAction_ChangeWeapon));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_Death, new Func<EnemyAction>(this.LaunchAction_Death));
			this.actionsDictionary.Add(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_MoveBattleBounds, new Func<EnemyAction>(this.LaunchAction_MoveBattleBounds));
		}

		private void SpawnBothAxes()
		{
			this.SpawnAxe(Vector2.right * 2f);
			this.SpawnAxe(Vector2.left * 2f);
			for (int i = 0; i < 2; i++)
			{
				this.axes[i].target = this.axeTargets[i];
			}
		}

		private void SpawnAxe(Vector2 dir)
		{
			if (this.axes == null)
			{
				this.axes = new List<AmanecidaAxeBehaviour>();
			}
			AmanecidaAxeBehaviour component = PoolManager.Instance.ReuseObject(this.amanecidaAxePrefab, base.transform.position + Vector2.up * 1f + dir, Quaternion.identity, false, 1).GameObject.GetComponent<AmanecidaAxeBehaviour>();
			component.GetComponentInChildren<AmanecidasAnimationEventsController>().amanecidasBehaviour = this;
			PathFollowingProjectile component2 = component.GetComponent<PathFollowingProjectile>();
			Hit h = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageAmount = this.GetDamageParameter("AXE_DMG"),
				DamageType = component2.damageType,
				DamageElement = component2.damageElement,
				Force = component2.force,
				HitSoundId = component2.hitSound,
				Unnavoidable = component2.unavoidable
			};
			component.InitDamageData(h);
			this.axes.Add(component);
			component.SetVisible(false);
		}

		private void SetBalanceData()
		{
			this.SetDamageParameter("AXE_DMG", 20f);
		}

		public void SetDamageParameter(string key, float value)
		{
			this.damageParameters[key] = value;
		}

		private float GetDamageParameter(string key)
		{
			return this.damageParameters[key];
		}

		public bool CanBeInterrupted()
		{
			return this._interruptable && this.lastAttack != AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_RechargeShield;
		}

		public void SetInterruptable(bool state)
		{
			this._interruptable = state;
		}

		public void ActivateBeam()
		{
			float delay = 0f;
			this.beamLauncher.gameObject.SetActive(true);
			this.beamLauncher.ActivateDelayedBeam(delay, true);
			Debug.Log("ACTIVATING BEAM");
		}

		public void DeactivateBeam(float delay = 0.3f)
		{
			Debug.Log("DEACTIVATING BEAM");
			this.beamLauncher.ActivateBeamAnimation(false);
			base.StartCoroutine(this.DelayedBeamDeactivation(delay));
		}

		private IEnumerator DelayedBeamDeactivation(float seconds)
		{
			yield return new WaitForSeconds(seconds);
			this.beamLauncher.gameObject.SetActive(false);
			yield break;
		}

		private bool DoCrystalLancesPlatformsExist()
		{
			return Object.FindObjectsOfType<AmanecidaCrystal>().Length > 0;
		}

		private void DestroyCrystalLancesPlatforms(float multiplierToTtl, float maxTtl)
		{
			Object.FindObjectsOfType<AmanecidaCrystal>().ToList<AmanecidaCrystal>().ForEach(delegate(AmanecidaCrystal x)
			{
				x.MultiplyCurrentTimeToLive(multiplierToTtl, maxTtl);
			});
		}

		private void UpdateCooldowns()
		{
			if (this.dodgeCooldown > 0f)
			{
				this.dodgeCooldown = Mathf.Clamp(this.dodgeCooldown - Time.deltaTime, 0f, this.dodgeMaxCooldown);
			}
		}

		private void CheckDebugActions()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			Dictionary<KeyCode, AmanecidasBehaviour.AMANECIDA_ATTACKS> debugActions = amanecidaAttackScriptableConfig.debugActions;
			if (debugActions != null)
			{
				foreach (KeyCode keyCode in debugActions.Keys)
				{
					if (Input.GetKeyDown(keyCode))
					{
						this.LaunchAction(debugActions[keyCode]);
					}
				}
			}
		}

		private void UpdateThrowbackCounter()
		{
			if (this.IsPenitentThrown())
			{
				this.throwbackExtraTime = this.maxThrowbackExtraTime;
			}
		}

		private void Update()
		{
			this._fsm.DoUpdate();
			this.UpdateCooldowns();
			this.UpdateAnimationParameters();
			this.UpdateThrowbackCounter();
		}

		public IEnumerator WaitForState(State<AmanecidasBehaviour> st)
		{
			while (!this._fsm.IsInState(st))
			{
				yield return null;
			}
			yield break;
		}

		public void ActivateFloating(bool active)
		{
			this.agent.enabled = active;
			if (active)
			{
				this.agent.SetConfig(this.floatingConfig);
			}
			else
			{
				this.agent.SetConfig(this.actionConfig);
			}
		}

		public void CheckCounterAttack()
		{
			if (this.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD)
			{
				this.SetDodge(true);
			}
		}

		public bool IsDodging()
		{
			return this.dodge && this.dodgeCooldown == 0f;
		}

		public void OnCrystalExplode(AmanecidaCrystal crystal)
		{
			this.verticalCrystalBeam.SummonAreaOnPoint(crystal.transform.position, 270f, 1f, null);
		}

		public void SetDodge(bool active)
		{
			this.dodge = active;
			this.Amanecidas.sparksOnImpact = !active;
		}

		public void UpdateFloatingCounter()
		{
			this.floatingCounter -= Time.deltaTime;
			if (this.floatingCounter < 0f)
			{
				this.floatingCounter = Random.Range(this.minFloatingPointCD, this.maxFloatingPointCD);
				this.ChangeFloatingPoint(this.GetNewValidFloatPoint());
			}
		}

		private bool IsFarEnoughToTurn()
		{
			float num = 4f;
			Arrive component = base.GetComponent<Arrive>();
			return Vector2.Distance(component.target, base.transform.position) > num;
		}

		public void UpdateAnimationParameters()
		{
		}

		private bool IsWieldingAxe()
		{
			return this.Amanecidas.AnimatorInyector.GetWeapon() == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE;
		}

		public void ApplyStuckOffset()
		{
			Vector3 vector;
			vector..ctor(-0.15f * (float)this.GetDirFromOrientation(), 0.5f, 0f);
			base.transform.position += vector;
		}

		private void ChangeFloatingPoint(Vector2 newPoint)
		{
			this.agent.GetComponent<Arrive>().target = newPoint;
		}

		private Vector2 GetNewValidFloatPoint()
		{
			Vector2 vector = this.RandomPointInsideRect(this.battleBounds);
			Vector2 vector2 = vector - this.agent.GetComponent<Arrive>().target;
			int num = 0;
			int num2 = 10;
			while (num < num2 && Vector2.SqrMagnitude(vector2) < 2f)
			{
				vector = this.RandomPointInsideRect(this.battleBounds);
				vector2 = vector - this.agent.GetComponent<Arrive>().target;
			}
			return vector;
		}

		private Vector2 RandomPointInsideRect(Rect r)
		{
			return new Vector2(Random.Range(r.xMin, r.xMin + r.width), Random.Range(r.yMin, r.yMin + r.height));
		}

		private void ResetShieldActions()
		{
			this.actionsBeforeShieldRecharge = this.currentFightParameters.maxActionsBeforeShieldRecharge;
		}

		private bool ShouldRechargeShield()
		{
			return this.Amanecidas.shieldCurrentHP <= 0f && this.actionsBeforeShieldRecharge <= 0;
		}

		public void InitShieldRechargeDamage()
		{
			this.damageWhileRecharging = (float)this.currentFightParameters.maxDamageBeforeInterruptingRecharge;
		}

		public void CheckShieldRechargeDamage()
		{
			if (this.damageWhileRecharging < 0f)
			{
				Debug.Log("SHIELD RECHARGE INTERRUPTED!");
				this.InterruptShieldRecharge();
			}
		}

		private void InterruptShieldRecharge()
		{
			this.StopCurrentAction();
			this.ResetShieldActions();
			this.Amanecidas.AnimatorInyector.SetRecharging(false);
			this._fsm.ChangeState(this.stHurt);
			this.Amanecidas.shield.InterruptShieldRecharge();
			this.Amanecidas.Audio.StopShieldRecharge_AUDIO();
			this.currentAction = this.LaunchAction_Hurt(false);
			this.currentAction.OnActionEnds -= this.OnHurtActionEnds;
			this.currentAction.OnActionEnds += this.OnHurtActionEnds;
			this.lastShieldRechargeWasInterrupted = true;
		}

		private List<AmanecidasBehaviour.AMANECIDA_ATTACKS> GetFilteredAttacks(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> originalList)
		{
			List<AmanecidasBehaviour.AMANECIDA_ATTACKS> list = new List<AmanecidasBehaviour.AMANECIDA_ATTACKS>(originalList);
			if (this.actionsBeforeShieldRecharge == this.currentFightParameters.maxActionsBeforeShieldRecharge && this.Amanecidas.shieldCurrentHP <= 0f && !this.lastShieldRechargeWasInterrupted && !this.Amanecidas.IsLaudes)
			{
				list.RemoveAll((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => x < (AmanecidasBehaviour.AMANECIDA_ATTACKS)100);
			}
			else
			{
				list.RemoveAll((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => x > (AmanecidasBehaviour.AMANECIDA_ATTACKS)100);
				switch (this.currentWeapon)
				{
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE:
					this.FilterAxeAttacks(list);
					break;
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.LANCE:
					this.FilterLanceAttacks(list);
					break;
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW:
					this.FilterBowAttacks(list);
					break;
				case AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD:
					this.FilterFalcataAttacks(list);
					break;
				}
				if (list.Count > 1)
				{
					list.Remove(this.lastAttack);
				}
				else if (list.Count == 0)
				{
					Debug.Log(string.Concat(new object[]
					{
						"We have filtered the attacks a bit too much! lastAttack: ",
						this.lastAttack,
						", attackBeforeLastAttack: ",
						this.attackBeforeLastAttack
					}));
					list.Add(this.attackBeforeLastAttack);
				}
			}
			return list;
		}

		private void FilterGenericWeapon(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> attacks, AmanecidasAnimatorInyector.AMANECIDA_WEAPON wpn)
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig atkConfig = amanecidaAttackScriptableConfig.GetAttackConfig(wpn, this.lastAttack);
			if (atkConfig.cantBeFollowedBy != null && atkConfig.cantBeFollowedBy.Count > 0)
			{
				attacks.RemoveAll((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => atkConfig.cantBeFollowedBy.Contains(x));
			}
			if (atkConfig.alwaysFollowedBy != null && atkConfig.alwaysFollowedBy.Count > 0)
			{
				attacks.RemoveAll((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => !atkConfig.alwaysFollowedBy.Contains(x));
			}
		}

		private void FilterAxeAttacks(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> attacks)
		{
			this.FilterGenericWeapon(attacks, AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE);
			if (this.IsWieldingAxe())
			{
				Debug.Log("REMOVING ATTACKS WITHOUT WIELDING AXES");
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_DualThrow);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_DualThrowCross);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndLavaBall);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlyAndSpin);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_JumpSmash);
			}
			else
			{
				Debug.Log("REMOVING ATTACKS WIELDING AXE");
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_MeleeAttack);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndAxeToss);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndCrawlerAxeToss);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlameBlade);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_StompAttack);
			}
			if (!this.Amanecidas.IsLaudes)
			{
				int num = (int)(this.Amanecidas.CurrentLife * 3f / this.Amanecidas.Stats.Life.Final);
				if (this.numActiveFlamePillarPairs < 3 - num)
				{
					attacks.RemoveAll((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => x != AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_JumpSmashWithPillars);
				}
				else
				{
					attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_JumpSmashWithPillars);
				}
			}
		}

		private void FilterLanceAttacks(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> attacks)
		{
			this.FilterGenericWeapon(attacks, AmanecidasAnimatorInyector.AMANECIDA_WEAPON.LANCE);
			if (this.IsPenitentInTop())
			{
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_StompAttack);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_MultiFrontalDash);
			}
			else
			{
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeHail);
			}
			if (this.actionsBeforeShieldRecharge <= 1)
			{
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeLances);
				attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeLancesOnPenitent);
			}
			if (attacks.Count > 1)
			{
				attacks.Remove(this.attackBeforeLastAttack);
			}
		}

		private void FilterBowAttacks(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> attacks)
		{
			this.FilterGenericWeapon(attacks, AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW);
			if (this.Amanecidas.IsLaudes)
			{
				if (this.CanUseLaserShotAttack())
				{
					attacks.RemoveAll((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => x != AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_LaserShot);
				}
				else
				{
					attacks.Remove(AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_LaserShot);
				}
			}
			else if (this.lastAttack != AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot && this.lastAttack != AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot)
			{
				if (this.penitentHealing != null && this.penitentHealing.IsHealing)
				{
					attacks.RemoveAll((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => x != AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot);
				}
				else
				{
					if (!this.CanUseFastShotsAttacks() && this.CanRemoveFastShotsAttacks(attacks))
					{
						this.RemoveFastShotsAttacks(attacks);
					}
					attacks.Remove(this.attackBeforeLastAttack);
				}
			}
			AmanecidasBehaviour.AMANECIDA_ATTACKS amanecida_ATTACKS = this.lastAttack;
			if (amanecida_ATTACKS != AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot && amanecida_ATTACKS != AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShots)
			{
			}
		}

		public bool CanUseLaserShotAttack()
		{
			return Core.Logic.Penitent.GetPosition().y < this.battleBounds.center.y - this.battleBounds.height / 1.8f;
		}

		private void FilterFalcataAttacks(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> attacks)
		{
			this.FilterGenericWeapon(attacks, AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD);
			if (attacks.Count > 1)
			{
				attacks.Remove(this.attackBeforeLastAttack);
			}
		}

		private bool CanUseFastShotsAttacks()
		{
			return base.transform.position.y > Core.Logic.Penitent.GetPosition().y + 1f && base.transform.position.y < this.battleBounds.yMax + 3f && (base.transform.position - Core.Logic.Penitent.GetPosition()).magnitude > 2.5f;
		}

		private bool CanRemoveFastShotsAttacks(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> attacks)
		{
			bool result;
			if (attacks.Count <= 2)
			{
				result = attacks.Any((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => x != AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot && x != AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShots);
			}
			else
			{
				result = true;
			}
			return result;
		}

		private void RemoveFastShotsAttacks(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> attacks)
		{
			attacks.RemoveAll((AmanecidasBehaviour.AMANECIDA_ATTACKS x) => x == AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot || x == AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShots);
		}

		private int RandomizeUsingWeights(List<AmanecidasBehaviour.AMANECIDA_ATTACKS> filteredAtks)
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			List<float> weights = amanecidaAttackScriptableConfig.GetWeights(filteredAtks, this.currentWeapon);
			float num = weights.Sum();
			float num2 = Random.Range(0f, num);
			float num3 = 0f;
			for (int i = 0; i < filteredAtks.Count; i++)
			{
				num3 += weights[i];
				if (num3 > num2)
				{
					return i;
				}
			}
			return 0;
		}

		private void LaunchAutomaticAction()
		{
			AmanecidasBehaviour.AMANECIDA_ATTACKS action;
			if (this.needsToSwapWeapon)
			{
				Debug.Log("<<<NEED TO SWAP WEAPON>>>");
				action = AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_ChangeWeapon;
			}
			else if (this.ShouldRechargeShield())
			{
				Debug.Log("<<<NEED TO RECHARGE SHIELD>>>");
				action = AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_RechargeShield;
			}
			else if (this.Amanecidas.IsLaudes && this.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW && this.actionsBeforeMovingBattlebounds <= 0 && this.laudesBowFightPhase != AmanecidaArena.WEAPON_FIGHT_PHASE.THIRD)
			{
				Debug.Log("<<<NEED TO MOVE BATTLEBOUNDS>>>");
				action = AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_MoveBattleBounds;
			}
			else
			{
				List<AmanecidasBehaviour.AMANECIDA_ATTACKS> filteredAttacks = this.GetFilteredAttacks(this.availableAttacks);
				int index = this.RandomizeUsingWeights(filteredAttacks);
				action = filteredAttacks[index];
				if (this.Amanecidas.shieldCurrentHP <= 0f)
				{
					this.actionsBeforeShieldRecharge--;
					if (this.Amanecidas.IsLaudes && this.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.BOW && this.laudesBowFightPhase != AmanecidaArena.WEAPON_FIGHT_PHASE.THIRD)
					{
						this.actionsBeforeMovingBattlebounds--;
					}
				}
			}
			this.LaunchAction(action);
			this.attackBeforeLastAttack = this.lastAttack;
			this.lastAttack = action;
		}

		protected void LaunchAction(AmanecidasBehaviour.AMANECIDA_ATTACKS action)
		{
			this.StopCurrentAction();
			this.DoWeaponSpecificOnLaunch();
			this._fsm.ChangeState(this.stAction);
			this.currentAction = this.actionsDictionary[action]();
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped += this.CurrentAction_OnActionStops;
		}

		private void StopCurrentAction()
		{
			if (this.currentAction != null)
			{
				this.currentAction.StopAction();
			}
		}

		private void DoWeaponSpecificOnLaunch()
		{
			foreach (AmanecidaAxeBehaviour amanecidaAxeBehaviour in this.axes)
			{
			}
		}

		private void DoWeaponSpecificOnWait()
		{
			foreach (AmanecidaAxeBehaviour amanecidaAxeBehaviour in this.axes)
			{
				amanecidaAxeBehaviour.SetSeek(true);
			}
		}

		private bool IsPenitentInTop()
		{
			return Core.Logic.Penitent.transform.position.y > this.battleBounds.center.y - 1f;
		}

		public void StartCombat()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.Amanecidas.Audio.StopAll));
			base.StartCoroutine(this.StartCombatCoroutine());
		}

		private IEnumerator StartCombatCoroutine()
		{
			this._fsm.ChangeState(this.stAction);
			if (this.Amanecidas.IsLaudes)
			{
				this.DoSummonWeaponAnimation();
			}
			yield return new WaitForSeconds(0.5f);
			this.extraRecoverySeconds = 1f;
			this.WaitBetweenActions();
			yield break;
		}

		public void SetExtraRecoverySeconds(float newRecovery)
		{
			this.extraRecoverySeconds = newRecovery;
		}

		private void WaitBetweenActions()
		{
			this._fsm.ChangeState(this.stFloating);
			this.DoWeaponSpecificOnWait();
			this.StartWait(this.extraRecoverySeconds + this.currentFightParameters.minMaxWaitingTimeBetweenActions.x, this.extraRecoverySeconds + this.currentFightParameters.minMaxWaitingTimeBetweenActions.y);
			this.extraRecoverySeconds = 0f;
		}

		private void StartWait(float min, float max)
		{
			this.StopCurrentAction();
			this.LookAtPenitent(false);
			this.currentAction = this.waitBetweenActions_EA.StartAction(this, min, max);
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
		}

		private void CurrentAction_OnActionEnds(EnemyAction e)
		{
			e.OnActionEnds -= this.CurrentAction_OnActionEnds;
			e.OnActionIsStopped -= this.CurrentAction_OnActionStops;
			if (e == this.intro_EA)
			{
				return;
			}
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
				this.SetGhostTrail(true);
				this.LaunchAutomaticAction();
			}
		}

		private void CurrentAction_OnActionStops(EnemyAction e)
		{
			this.Amanecidas.AnimatorInyector.ClearAll(true);
			this.Amanecidas.AnimatorInyector.ClearRotationAndFlip();
			e.OnActionEnds -= this.CurrentAction_OnActionEnds;
			e.OnActionIsStopped -= this.CurrentAction_OnActionStops;
		}

		public void DoActivateCollisions(bool b)
		{
			this.Amanecidas.DamageArea.DamageAreaCollider.enabled = b;
		}

		public void SummonWeapon()
		{
			this.Amanecidas.AnimatorInyector.SetAmanecidaWeapon(this.currentWeapon);
		}

		public void OnMeleeAttackStarts()
		{
			this.currentMeleeAttack.damageOnEnterArea = true;
			this.currentMeleeAttack.CurrentWeaponAttack();
		}

		public void OnMeleeAttackFinished()
		{
			this.currentMeleeAttack.damageOnEnterArea = false;
		}

		[FoldoutGroup("Battle area", 0)]
		[Button("CenterBattleAreaHere", 22)]
		private void CenterBattleAreaHere()
		{
			this.battleBounds.center = base.transform.position;
		}

		private void Foo_PlayAnimation()
		{
			Debug.Log("PLAYING ANIMATION THROUGH THE ANIMATOR INYECTOR");
		}

		public void DoDummyAttack()
		{
			Debug.Log("<DUMMY ATTACK>");
			this.instantProjectileAttack.Shoot(base.transform.position, Core.Logic.Penitent.transform.position - base.transform.position);
		}

		public void DoDummyBackJump()
		{
			Debug.Log("<DUMMY ATTACK>");
			Vector2 vector = -base.transform.right * 4f;
			Vector2 vector2 = base.transform.position + vector;
			this.jumpAttack.Use(base.transform, vector2);
		}

		public void DoSummonWeaponAnimation()
		{
			this.Amanecidas.AnimatorInyector.PlaySummonWeapon();
		}

		public void ShowSprites(bool show)
		{
			this.Amanecidas.AnimatorInyector.ShowSprites(show);
		}

		public void DoSpinAnimation(bool spin, bool doDamage = false)
		{
			if (spin)
			{
				this.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				this.Amanecidas.AnimatorInyector.PlayStompAttack(doDamage);
			}
			else
			{
				this.Amanecidas.AnimatorInyector.SetMeleeHold(false);
			}
		}

		public void OnLanceDashAdvance(float value)
		{
			float num = 2f;
			if (Mathf.Abs(this.lastX - base.transform.position.x) >= num)
			{
				this.SummonSpike(base.transform.position);
				this.lastX = base.transform.position.x;
			}
		}

		public void DoSmallJumpSmash(bool usePillars)
		{
			Vector2 pointBelowPenitent = this.GetPointBelowPenitent(false);
			this.DoSmallJumpSmashToPoint(pointBelowPenitent, usePillars);
		}

		public void DoSmallJumpSmashToPoint(Vector2 p, bool usePillars)
		{
			this.ClearJumpEvents();
			this.jumpAttack.Use(base.transform, p);
			if (usePillars)
			{
				this.jumpAttack.OnJumpLanded += this.OnSmallJumpSmashLanded;
			}
			else
			{
				this.jumpAttack.OnJumpLanded += this.Amanecidas.Audio.PlayGroundAttack_AUDIO;
			}
		}

		public void DoLavaBallJumpSmash()
		{
			this.ClearJumpEvents();
			bool flag;
			if (Mathf.Approximately(base.transform.position.x, this.battleBounds.center.x))
			{
				flag = ((double)Random.value < 0.5);
			}
			else
			{
				flag = (base.transform.position.x < this.battleBounds.center.x);
			}
			Vector2 zero = Vector2.zero;
			if (flag)
			{
				zero..ctor(this.battleBounds.xMax, this.battleBounds.yMin);
			}
			else
			{
				zero..ctor(this.battleBounds.xMin, this.battleBounds.yMin);
			}
			this.jumpAttack.Use(base.transform, this.GetPointBelow(zero, false, 100f));
			this.jumpAttack.OnJumpLanded += this.OnLavaBallJumpSmashLanded;
		}

		public void DoJumpSmash()
		{
			this.ClearJumpEvents();
			Vector2 pointBelowPenitent = this.GetPointBelowPenitent(false);
			this.jumpAttack.Use(base.transform, pointBelowPenitent);
			this.jumpAttack.OnJumpLanded += this.OnJumpSmashLanded;
		}

		public void DoJumpSmashWithPillars()
		{
			this.ClearJumpEvents();
			Vector2 pointBelowPenitent = this.GetPointBelowPenitent(false);
			pointBelowPenitent.x = Mathf.Clamp(pointBelowPenitent.x, this.battleBounds.xMin + 1f, this.battleBounds.xMax - 1f);
			this.jumpAttack.Use(base.transform, pointBelowPenitent);
			this.jumpAttack.OnJumpLanded += this.OnJumpSmashWithPillarsLanded;
		}

		private void ClearJumpEvents()
		{
			this.jumpAttack.OnJumpLanded -= this.OnJumpSmashLanded;
			this.jumpAttack.OnJumpLanded -= this.OnSmallJumpSmashLanded;
			this.jumpAttack.OnJumpLanded -= this.OnJumpSmashWithPillarsLanded;
			this.jumpAttack.OnJumpLanded -= this.OnLavaBallJumpSmashLanded;
			this.jumpAttack.OnJumpLanded -= this.Amanecidas.Audio.PlayGroundAttack_AUDIO;
		}

		private void OnJumpSmashLanded()
		{
			this.ShakeWave(true);
			this.SpikeWave(base.transform.position, 0.6f, 15, true, 1.1f);
			this.shockwave.SummonAreaOnPoint(base.transform.position, 0f, 1f, null);
		}

		private void OnJumpSmashWithPillarsLanded()
		{
			this.ShakeWave(true);
			this.SpikeWave(base.transform.position, 1f, 15, true, 1.5f);
			this.numActiveFlamePillarPairs++;
			GameObject gameObject = this.verticalBlastAxeAttack.SummonAreaOnPoint(new Vector2(this.battleBounds.xMin - 1f, this.battleBounds.yMin), 0f, 1f, new Action(this.IncreaseBattleBoundsWidth));
			gameObject.GetComponent<Entity>().SetOrientation(EntityOrientation.Right, true, true);
			gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 4 - this.numActiveFlamePillarPairs;
			gameObject = this.verticalBlastAxeAttack.SummonAreaOnPoint(new Vector2(this.battleBounds.xMax + 1f, this.battleBounds.yMin), 0f, 1f, null);
			gameObject.GetComponent<Entity>().SetOrientation(EntityOrientation.Left, true, true);
			gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = 4 - this.numActiveFlamePillarPairs;
			this.DecreaseBattleBoundsWidth();
			this.shockwave.SummonAreaOnPoint(base.transform.position, 0f, 1f, null);
		}

		private void IncreaseBattleBoundsWidth()
		{
			float num = 2f;
			this.battleBounds.center = new Vector2(this.battleBounds.center.x - num / 2f, this.battleBounds.center.y);
			this.battleBounds.width = this.battleBounds.width + num;
		}

		private void DecreaseBattleBoundsWidth()
		{
			float num = 2f;
			this.battleBounds.center = new Vector2(this.battleBounds.center.x + num / 2f, this.battleBounds.center.y);
			this.battleBounds.width = this.battleBounds.width - num;
		}

		public void SpikeWave(Vector2 center, float heightPercentage = 1f, int n = 14, bool doShockWaves = true, float totalSeconds = 1.5f)
		{
			int num = 1;
			float num2 = 1f;
			for (int i = 0; i < 2; i++)
			{
				if (doShockWaves)
				{
					this.shockwave.SummonAreas(center, Vector2.right * (float)num);
				}
				for (int j = 0; j < n; j++)
				{
					Vector2 vector = center + Vector2.right * ((float)(j + 1) * num2) * (float)num;
					if (vector.x > this.battleBounds.xMin && vector.x < this.battleBounds.xMax)
					{
						BossSpawnedGeoAttack bossSpawnedGeoAttack = this.SummonSpike(vector);
						float num3 = (float)j / (float)n;
						bossSpawnedGeoAttack.SpawnGeo(num3 * totalSeconds, heightPercentage);
					}
				}
				num = -1;
			}
		}

		private BossSpawnedGeoAttack SummonSpike(Vector3 position)
		{
			return PoolManager.Instance.ReuseObject(this.spikePrefab.gameObject, position + Vector3.up, Quaternion.identity, false, 1).GameObject.GetComponent<BossSpawnedGeoAttack>();
		}

		public void ShowRicochetArrowTrail(Vector2 startPoint, Vector2 endPoint)
		{
			Vector2 dir = endPoint - startPoint;
			this.arrowTrailInstantProjectileAttack.Shoot(startPoint, dir);
			this.Amanecidas.Audio.PlayLaserPreattack_AUDIO();
		}

		public void ShowRicochetArrowTrailFast(Vector2 startPoint, Vector2 endPoint)
		{
			Vector2 dir = endPoint - startPoint;
			this.arrowTrailFastInstantProjectileAttack.Shoot(startPoint, dir);
			this.Amanecidas.Audio.PlayHorizontalPreattack_AUDIO();
		}

		public void ShowMineArrowTrail(Vector2 startPoint, Vector2 endPoint)
		{
			Vector2 dir = endPoint - startPoint;
			this.arrowTrailInstantProjectileAttack.Shoot(startPoint, dir);
			this.Amanecidas.Audio.PlayHorizontalPreattack_AUDIO();
		}

		public void ShootRicochetArrow(Vector2 startPoint, Vector2 endPoint)
		{
			this.ricochetArrowInstantProjectileAttack.Shoot(startPoint, endPoint - startPoint);
		}

		private void OnSmallJumpSmashLanded()
		{
			this.ShakeWave(false);
			this.SpikeWave(base.transform.position, 0.8f, 10, false, 1f);
			this.Amanecidas.Audio.PlayGroundAttack_AUDIO();
			this.shockwave.SummonAreaOnPoint(base.transform.position, 0f, 1f, null);
		}

		private void OnLavaBallJumpSmashLanded()
		{
			this.ShakeWave(false);
			this.SpikeWave(base.transform.position, 0.6f, 20, false, 1.5f);
			this.shockwave.SummonAreas((base.transform.position.x >= this.battleBounds.center.x) ? Vector2.left : Vector2.right);
			this.flameBladeAttack.Shoot(Vector2.left, Vector2.down * 0.4f, 1f);
			this.flameBladeAttack.Shoot(Vector2.right, Vector2.down * 0.4f, 1f);
			this.Amanecidas.Audio.PlayGroundAttack_AUDIO();
			this.shockwave.SummonAreaOnPoint(base.transform.position, 0f, 1f, null);
		}

		public void ShootMineArrow()
		{
			Vector2 vector = (base.transform.position.x <= this.battleBounds.center.x) ? Vector2.right : Vector2.left;
			this.AimToPointWithBow(base.transform.position + vector);
			List<GameObject> list = this.mineArrowProjectileAttack.Shoot(base.transform.position + new Vector2(0f, 1.75f), vector);
			foreach (GameObject gameObject in list)
			{
				this.mines.Add(gameObject.GetComponent<BossSpawnedAreaAttack>());
			}
		}

		public void StartChargingArrow()
		{
			this.Amanecidas.Audio.PlayAttackCharge_AUDIO();
		}

		public void ReleaseChargedArrow()
		{
			float angle = (this.Amanecidas.transform.position.x <= this.battleBounds.center.x) ? 180f : 0f;
			this.chargedArrowExplosionAttack.SummonAreaOnPoint(this.Amanecidas.transform.position + Vector2.up, angle, 1f, null);
			this.shockwave.SummonAreaOnPoint(this.Amanecidas.transform.position, 0f, 1f, null);
			this.ShakeWave(true);
			this.Amanecidas.AnimatorInyector.SetBow(false);
		}

		public void ShootSpikeSummoningArrow()
		{
			Vector2 vector = this.Amanecidas.transform.position + Vector3.right;
			this.arrowInstantProjectileAttack.Shoot(vector, Vector2.down);
			this.Amanecidas.Audio.PlayArrowFireFast_AUDIO();
			vector.y = this.battleBounds.yMin;
			this.verticalBlastArrowAttack.SummonAreaOnPoint(vector, 270f, 1f, null);
			Core.Logic.ScreenFreeze.Freeze(0.05f, 0.2f, 0f, null);
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.5f, 0.3f, 2f);
			this.SpikeWave(vector, 1f, 20, true, 1.8f);
			this.Amanecidas.AnimatorInyector.SetBow(false);
		}

		public void ShakeWave(bool doShockwave = true)
		{
			Core.Logic.ScreenFreeze.Freeze(0.05f, 0.2f, 0f, null);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.35f, Vector3.down * 0.5f, 12, 0.2f, 0.01f, default(Vector3), 0.01f, true);
			if (doShockwave)
			{
				Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.5f, 0.3f, 2f);
			}
		}

		public void SmallDistortion()
		{
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position, 0.2f, 0.1f, 0.5f);
		}

		public void ActivateMines()
		{
			if (this.mines == null || this.mines.Count == 0)
			{
				return;
			}
			for (int i = 0; i < this.mines.Count; i++)
			{
				this.mines[i].SetRemainingPreparationTime((float)i * 0.8f);
			}
			this.mines.Clear();
			this.ClearRotationAndFlip();
			this.Amanecidas.AnimatorInyector.SetBow(false);
			this.Amanecidas.SetOrientation(this.Amanecidas.Status.Orientation, true, false);
		}

		public void SetFrozenArrow()
		{
			this.Amanecidas.AnimatorInyector.PlayBlinkshot();
			Vector2 dirToPenitent = this.GetDirToPenitent(base.transform.position);
			StraightProjectile straightProjectile = this.bulletTimeProjectileAttack.Shoot(dirToPenitent);
			this.AimToPointWithBow(base.transform.position + dirToPenitent);
			this.bullets.Add(straightProjectile as BulletTimeProjectile);
			this.Amanecidas.Audio.PlayArrowCharge_AUDIO();
		}

		public void PlayChargeEnergy(float seconds = 1f, bool useLongRangeParticles = false, bool playSfx = true)
		{
			this.chargeParticles.Play();
			if (useLongRangeParticles)
			{
				this.chargeParticlesLongRange.Play();
			}
			this.chargeEnergy_EA.StartAction(this, seconds, playSfx);
		}

		public void SetFrozenArrowVariation()
		{
			this.Amanecidas.AnimatorInyector.PlayBlinkshot();
			Vector2 vector = base.transform.position - new Vector2(this.battleBounds.center.x, this.battleBounds.yMax + 4f);
			StraightProjectile straightProjectile = this.bulletTimeProjectileAttack.Shoot(vector);
			this.AimToPointWithBow(base.transform.position + vector);
			this.bullets.Add(straightProjectile as BulletTimeProjectile);
			this.Amanecidas.Audio.PlayArrowCharge_AUDIO();
		}

		public void SetThreeMultiFrozenArrows()
		{
			this.SetMultiFrozenArrows(3, 1.2f);
		}

		public void SetFourMultiFrozenArrows()
		{
			this.SetMultiFrozenArrows(4, 1.4f);
		}

		private void SetMultiFrozenArrows(int numArrows, float width)
		{
			Vector2 dirToPenitent = this.GetDirToPenitent(base.transform.position);
			Vector2 vector;
			vector..ctor(dirToPenitent.y, -dirToPenitent.x);
			Vector2 normalized = vector.normalized;
			for (int i = 0; i < numArrows; i++)
			{
				Vector2 offset = Vector2.Lerp(normalized * -width, normalized * width, (float)i / ((float)numArrows - 1f));
				offset.y += 0.9f;
				Vector2 dir = dirToPenitent + normalized * 1f * (float)i;
				StraightProjectile straightProjectile = this.bulletTimeProjectileAttack.Shoot(dir, offset, 1f);
				this.multiBullets.Add(straightProjectile as BulletTimeProjectile);
			}
			this.Amanecidas.Audio.PlayArrowCharge_AUDIO();
		}

		public void SetFrozenHail(Vector2 lancePosition)
		{
			Vector2 vector = lancePosition - base.transform.position + Random.Range(-1.5f, 1.5f) * Vector2.down;
			vector += Random.Range(-0.5f, 0.5f) * Vector2.right;
			Vector2 vector2 = Vector2.down + Random.Range(-0.25f, 0.25f) * Vector2.right;
			StraightProjectile straightProjectile = this.bulletTimeHailAttack.Shoot(vector2.normalized, vector, 1f);
			this.bullets.Add(straightProjectile as BulletTimeProjectile);
		}

		public void SetFrozenLance(Vector2 lancePosition)
		{
			if (lancePosition.x > this.battleBounds.xMax || lancePosition.x < this.battleBounds.xMin)
			{
				return;
			}
			Vector2 offset = lancePosition - base.transform.position;
			StraightProjectile straightProjectile = this.bulletTimeLanceAttack.Shoot(Vector2.down, offset, 1f);
			this.bullets.Add(straightProjectile as BulletTimeProjectile);
		}

		public void ShootGhostProjectile(Vector2 startPoint, Vector2 dir)
		{
			Vector3 rotation;
			rotation..ctor(0f, 0f, Mathf.Atan2(dir.y, dir.x) * 57.29578f);
			if (dir.x < 0f)
			{
				rotation.y = 180f;
			}
			this.ghostProjectileAttack.Shoot(dir, Vector2.zero, rotation, 1f);
		}

		public void UpdateBowAimRotation()
		{
			this.Amanecidas.AnimatorInyector.SetBow(true);
			this.AimToPointWithBow(this.bowAimTarget);
		}

		private void AimToPointWithBow(Vector2 point)
		{
			Vector2 vector = point - base.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			num = (num + 360f) % 360f;
			this.Amanecidas.AnimatorInyector.FlipSpriteWithAngle(num);
			this.Amanecidas.AnimatorInyector.SetSpriteRotation(num, this.bowAngleDifference);
		}

		private void AimMeleeDirection(Vector2 dir, float difference = 15f)
		{
			float num = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
			num = (num + 360f) % 360f;
			this.Amanecidas.AnimatorInyector.FlipSpriteWithAngle(num);
			this.Amanecidas.AnimatorInyector.SetSpriteRotation(num, difference);
		}

		public void DoLanceProjectile()
		{
			Debug.Log("<PROJECTILE ATTACK>");
			Vector2 dirToPenitent = this.GetDirToPenitent(base.transform.position);
			dirToPenitent.y = 0f;
			StraightProjectile straightProjectile = this.bulletTimeLanceAttack.Shoot(dirToPenitent);
			this.bullets.Add(straightProjectile as BulletTimeProjectile);
		}

		public void ClearRotationAndFlip()
		{
			this.Amanecidas.AnimatorInyector.ClearRotationAndFlip();
			this.ReapplyOrientation();
		}

		private void ReapplyOrientation()
		{
			this.Amanecidas.SetOrientation(this.Amanecidas.Status.Orientation, true, false);
		}

		public void ActivateFrozenArrows()
		{
			Debug.Log("<ACTIVATE FROZEN ARROWS>");
			if (this.bullets == null || this.bullets.Count == 0)
			{
				return;
			}
			foreach (BulletTimeProjectile bulletTimeProjectile in this.bullets)
			{
				bulletTimeProjectile.Accelerate(1.1f);
			}
			this.bullets.Clear();
			this.ClearRotationAndFlip();
			this.Amanecidas.AnimatorInyector.SetBow(false);
			this.Amanecidas.SetOrientation(this.Amanecidas.Status.Orientation, true, false);
			this.Amanecidas.Audio.PlayArrowFire_AUDIO();
		}

		public void ActivateMultiFrozenArrows()
		{
			Debug.Log("<Activate Multi Frozen Arrows>");
			if (this.multiBullets == null || this.multiBullets.Count == 0)
			{
				return;
			}
			foreach (BulletTimeProjectile bulletTimeProjectile in this.multiBullets)
			{
				bulletTimeProjectile.Accelerate(1f);
			}
			this.multiBullets.Clear();
			this.Amanecidas.Audio.PlayArrowFire_AUDIO();
		}

		public void ShowBothAxes(bool v)
		{
			this.ShowFirstAxe(v);
			this.ShowSecondAxe(v);
		}

		public void ShowAxe(bool show, bool isFirstAxe)
		{
			this.ShowAxe(show, isFirstAxe, false, Vector2.zero);
		}

		public void ShowAxe(bool show, bool isFirstAxe, bool setAxePosition, Vector2 axePosition)
		{
			int index = (!isFirstAxe) ? 1 : 0;
			if (show)
			{
				int dirFromOrientation = this.GetDirFromOrientation();
				if (setAxePosition)
				{
					this.axes[index].transform.position = axePosition;
				}
				else
				{
					this.axes[index].transform.position = base.transform.position + new Vector2(this.axeOffset.x * (float)dirFromOrientation, this.axeOffset.y);
				}
				this.axes[index].SetSeek(false);
			}
			this.axes[index].SetVisible(show);
		}

		public void ShowFirstAxe(bool v)
		{
			this.ShowAxe(v, true);
		}

		public void ShowSecondAxe(bool v)
		{
			this.ShowAxe(v, false);
		}

		public void ActivateFrozenHail()
		{
			base.StartCoroutine(this.ActivateFrozenHailRandomDelayed(0.01f));
		}

		public void ActivateFrozenLances()
		{
			Debug.Log("<Activate Frozen Lances>");
			base.StartCoroutine(this.ActivateFrozenLancesDelayed(0.1f));
		}

		private IEnumerator ActivateFrozenLancesDelayed(float delay = 0.1f)
		{
			Debug.Log("<Activate Frozen Lances>");
			if (this.bullets != null && this.bullets.Count != 0)
			{
				foreach (BulletTimeProjectile item in this.bullets)
				{
					item.Accelerate(1f);
					yield return new WaitForSeconds(delay);
				}
				this.bullets.Clear();
			}
			yield break;
		}

		private IEnumerator ActivateFrozenHailRandomDelayed(float delay = 0.1f)
		{
			Debug.Log("<Activate Frozen HAIL>");
			if (this.bullets != null && this.bullets.Count != 0)
			{
				while (this.bullets.Count > 0)
				{
					BulletTimeProjectile item = this.bullets[Random.Range(0, this.bullets.Count)];
					item.Accelerate(1f);
					yield return new WaitForSeconds(delay);
					this.bullets.Remove(item);
				}
				this.bullets.Clear();
			}
			yield break;
		}

		public void DoAnticipateShockwave()
		{
			Debug.Log("SHIELD RECHARGE FINISHED");
			Debug.Log("ANTICIPATE SHOCKWAVE");
			this.PlayChargeEnergy(1f, false, false);
			this.Amanecidas.AnimatorInyector.SetShockwaveAnticipation(true);
			this.Amanecidas.AnimatorInyector.SetRecharging(false);
			this.Amanecidas.IsGuarding = true;
			this.Amanecidas.Audio.PlayShieldRecharge_AUDIO();
		}

		public void DoShieldShockwave()
		{
			Debug.Log("ACTIVATE SHIELD");
			this.Amanecidas.AnimatorInyector.SetShockwaveAnticipation(false);
			this._fsm.ChangeState(this.stAction);
			this.ShockwaveOnSelf();
			this.ResetShieldActions();
			this.Amanecidas.ActivateShield();
		}

		public void ShockwaveOnSelf()
		{
			this.shieldShockwave_EA.StartAction(this);
		}

		public void SetGhostTrail(bool active)
		{
			this.Amanecidas.GhostTrail.EnableGhostTrail = active;
		}

		private void DoRechargeShield()
		{
			Debug.Log("START RECHARGE SHIELD LOOP");
			this.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
			this.ShowBothAxes(false);
			this.Amanecidas.AnimatorInyector.SetRecharging(true);
			this.Amanecidas.shield.StartToRecoverShield((float)this.currentFightParameters.shieldRechargeTime * 0.8f, (float)this.currentFightParameters.shieldRechargeTime * 0.2f, (float)this.currentFightParameters.shieldShockwaveAnticipationTime * 0.2f, (float)this.currentFightParameters.shieldShockwaveAnticipationTime * 0.8f, null);
			this._fsm.ChangeState(this.stRecharge);
		}

		private void LookAtPenitent(bool instant = false)
		{
			this.LookAtTarget(Core.Logic.Penitent.transform.position, instant);
		}

		private void LookAtPenitentUsingOrientation()
		{
			Vector2 dirToPenitent = this.GetDirToPenitent(base.transform.position);
			this.LookAtDirUsingOrientation(dirToPenitent);
		}

		private void LookAtPointUsingOrientation(Vector2 p)
		{
			Vector2 v = p - base.transform.position;
			this.LookAtDirUsingOrientation(v);
		}

		public void PlayAnticipationGrunt(AmanecidasBehaviour.AMANECIDA_GRUNTS grunt)
		{
		}

		private void LookAtDirUsingOrientation(Vector2 v)
		{
			this.Amanecidas.SetOrientation((v.x <= 0f) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			base.LookAtTarget(targetPos);
			throw new Exception("You really really didn't mean to use this method. Look at the one below: that is the one you are looking for.");
		}

		public void LookAtTarget(Vector3 targetPos, bool instant)
		{
			bool flag = targetPos.x > this.Amanecidas.transform.position.x;
			if ((flag && this.Amanecidas.Status.Orientation == EntityOrientation.Left) || (!flag && this.Amanecidas.Status.Orientation == EntityOrientation.Right))
			{
				this.ForceTurnAround(instant);
			}
		}

		private void ForceTurnAround(bool instant = false)
		{
			Debug.Log(string.Format("<color=red> !!! <<TURNING>> Orientation before: {2}. myX:{0} penitentX:{1} </color>", base.transform.position.x, Core.Logic.Penitent.GetPosition().x, this.Amanecidas.Status.Orientation));
			this.Amanecidas.AnimatorInyector.PlayTurnAround(instant);
		}

		private Vector2 GetPointBelowPenitent(bool stopOnOneWayDowns)
		{
			return this.GetPointBelow(Core.Logic.Penitent.transform.position + Vector3.up * 0.25f, stopOnOneWayDowns, 100f);
		}

		private Vector2 GetDirToPenitent()
		{
			return Core.Logic.Penitent.transform.position - base.transform.position;
		}

		private Vector2 GetDirToPenitent(Vector2 point)
		{
			return Core.Logic.Penitent.transform.position - point;
		}

		public float GetAngleDifference()
		{
			return (this.currentWeapon != AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD) ? this.lanceRotationDiference : this.falcataRotationDiference;
		}

		public int GetDirFromOrientation()
		{
			return (this.Amanecidas.Status.Orientation != EntityOrientation.Right) ? -1 : 1;
		}

		private bool IsInsideGround(Vector2 p, LayerMask m)
		{
			if (Physics2D.RaycastNonAlloc(p, Vector2.down, this.results, 0.1f, m) > 0)
			{
				Debug.DrawLine(p, this.results[0].point, Color.cyan, 5f);
				Debug.Log("IS INSIDE GROUND");
				return true;
			}
			return false;
		}

		private Vector2 GetPointBelow(Vector2 p, bool stopOnOneWayDowns = false, float maxDistance = 100f)
		{
			float num = 0.4f;
			LayerMask layerMask = (!stopOnOneWayDowns) ? this.floorMask : this.floorNOneWayDownMask;
			if (Physics2D.RaycastNonAlloc(p, Vector2.down, this.results, maxDistance, layerMask) > 0)
			{
				return this.results[0].point + Vector2.up * num;
			}
			return p;
		}

		private bool HasSolidFloorBelow(bool checkOneWayDowns = false)
		{
			Vector2 pointBelowMe = this.GetPointBelowMe(checkOneWayDowns, 0.9f);
			return Vector2.Distance(pointBelowMe, base.transform.position) >= Mathf.Epsilon;
		}

		private Vector2 GetPointBelowMe(bool stopOnOneWayDowns = false, float maxDistance = 100f)
		{
			float num = 0f;
			LayerMask m = (!stopOnOneWayDowns) ? this.floorMask : this.floorNOneWayDownMask;
			if (this.IsInsideGround(base.transform.position, m))
			{
				num = 2f;
			}
			return this.GetPointBelow(base.transform.position + Vector2.up * num, stopOnOneWayDowns, maxDistance);
		}

		private Vector2 GetValidPointInDirection(Vector2 dir, float amount)
		{
			Vector2 normalized = dir.normalized;
			Vector2 vector = base.transform.position;
			if (Physics2D.RaycastNonAlloc(vector, normalized, this.results, amount, this.BlockLayerMask) > 0)
			{
				Debug.DrawLine(vector, this.results[0].point, Color.red, 5f);
				amount = this.results[0].distance;
			}
			else
			{
				Debug.DrawLine(vector, vector + dir * amount, Color.green, 5f);
			}
			return vector + normalized * amount;
		}

		public override void Parry()
		{
			base.Parry();
			this.StopCurrentAction();
			this.LookAtPenitent(true);
			this.SetGhostTrail(false);
			ShortcutExtensions.DOKill(base.transform, true);
			this.Amanecidas.AnimatorInyector.Parry();
			this.SetInterruptable(true);
			this.WaitAfterParry();
		}

		private void WaitAfterParry()
		{
			float num = 1.5f;
			this.StartWait(num, num);
		}

		public override void Attack()
		{
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Idle()
		{
		}

		public override void StopMovement()
		{
		}

		public override void Wander()
		{
		}

		public override void Damage()
		{
		}

		public void SpawnClone()
		{
			GameObject gameObject = PoolManager.Instance.ReuseObject(this.clonePrefab, base.transform.position, Quaternion.identity, false, 1).GameObject;
			SimpleDamageableObject component = gameObject.GetComponent<SimpleDamageableObject>();
			component.SetFlip(this.Amanecidas.Status.Orientation == EntityOrientation.Left);
		}

		private bool CheckAndSwapFightParameters()
		{
			bool result = false;
			float hpPercentage = this.Amanecidas.GetHpPercentage();
			if (this.Amanecidas.IsLaudes)
			{
				AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = this.laudesAttackConfigData;
				this.availableAttacks = amanecidaAttackScriptableConfig.GetAttackIdsByWeapon(this.currentWeapon, false, 1f);
			}
			else
			{
				AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig2 = this.attackConfigData;
				this.availableAttacks = amanecidaAttackScriptableConfig2.GetAttackIdsByWeapon(this.currentWeapon, true, this.GetHpPercentage());
			}
			for (int i = 0; i < this.currentFightParameterList.Count; i++)
			{
				if (this.currentFightParameterList[i].hpPercentageBeforeApply < this.currentFightParameters.hpPercentageBeforeApply && this.currentFightParameterList[i].hpPercentageBeforeApply > hpPercentage && !this.currentFightParameters.Equals(this.currentFightParameterList[i]))
				{
					this.currentFightParameters = this.currentFightParameterList[i];
					result = true;
					break;
				}
			}
			return result;
		}

		public void ShieldDamage(Hit hit)
		{
		}

		public void Damage(Hit hit)
		{
			if (this.CheckAndSwapFightParameters() && this.Amanecidas.IsLaudes)
			{
				this.needsToSwapWeapon = true;
			}
			if (this._fsm.IsInState(this.stRecharge))
			{
				this.damageWhileRecharging -= hit.DamageAmount;
				Debug.Log("DAMAGE BEFORE RECHARGE " + this.damageWhileRecharging);
			}
			if (this.CanBeInterrupted() && this._currentHurtHits < this.currentFightParameters.maxHitsInHurt)
			{
				bool isLastHurt = false;
				this._fsm.ChangeState(this.stHurt);
				this.StopCurrentAction();
				ShortcutExtensions.DOKill(base.transform, true);
				this.LookAtPenitent(true);
				this._currentHurtHits++;
				if (this._currentHurtHits >= this.currentFightParameters.maxHitsInHurt)
				{
					this._currentHurtHits = 0;
					isLastHurt = true;
				}
				this.currentAction = this.LaunchAction_Hurt(isLastHurt);
				this.currentAction.OnActionEnds -= this.OnHurtActionEnds;
				this.currentAction.OnActionEnds += this.OnHurtActionEnds;
			}
		}

		private void OnHurtActionEnds(EnemyAction e)
		{
			e.OnActionEnds -= this.OnHurtActionEnds;
			this.LaunchAutomaticAction();
		}

		public bool DodgeHit(Hit h)
		{
			if (this.IsDodging() && this.lastAttack != AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_ChangeWeapon)
			{
				this.LaunchAction(AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_CounterAttack);
				this.dodgeCooldown = this.dodgeMaxCooldown;
				return true;
			}
			return false;
		}

		public void Death()
		{
			PlayMakerFSM.BroadcastEvent("BOSS DEAD");
			this.Amanecidas.Audio.StopAll();
			this.CleanAll();
			this.StopCurrentAction();
			base.StopAllCoroutines();
			ShortcutExtensions.DOKill(base.transform, false);
			this._fsm.ChangeState(this.stDeath);
			this.LookAtPenitentUsingOrientation();
			Core.Logic.Penitent.Status.Invulnerable = true;
			this.LaunchAction(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_Death);
		}

		private float GetAttackRecoverySeconds(AmanecidaAttackScriptableConfig.AmanecidaAttackConfig config)
		{
			float hpPercentage = this.GetHpPercentage();
			if (hpPercentage > 0.66f)
			{
				return config.recoverySeconds;
			}
			if (hpPercentage > 0.33f)
			{
				return config.recoverySeconds2;
			}
			return config.recoverySeconds3;
		}

		private int GetAttackNumberOfRepetitions(AmanecidaAttackScriptableConfig.AmanecidaAttackConfig config)
		{
			float hpPercentage = this.GetHpPercentage();
			if (hpPercentage > 0.66f)
			{
				return config.repetitions;
			}
			if (hpPercentage > 0.33f)
			{
				return config.repetitions2nd;
			}
			return config.repetitions3rd;
		}

		private float GetAttackWeight(AmanecidaAttackScriptableConfig.AmanecidaAttackConfig config)
		{
			float hpPercentage = this.GetHpPercentage();
			if (hpPercentage > 0.66f)
			{
				return config.weight;
			}
			if (hpPercentage > 0.33f)
			{
				return config.weight2;
			}
			return config.weight3;
		}

		private void CleanAll()
		{
			this.ShowBothAxes(false);
			if (this.bullets != null & this.bullets.Count > 0)
			{
				foreach (BulletTimeProjectile bulletTimeProjectile in this.bullets)
				{
					bulletTimeProjectile.gameObject.SetActive(false);
				}
			}
			GameplayUtils.DestroyAllProjectiles();
		}

		public bool IsShieldBroken()
		{
			return this.Amanecidas.shieldCurrentHP <= 0f;
		}

		public void InstantBreakShield()
		{
			if (!this.IsShieldBroken())
			{
				this.Amanecidas.ForceBreakShield();
			}
		}

		public bool IsPenitentThrown()
		{
			return Core.ready && Core.Logic.Penitent != null && Core.Logic.Penitent.ThrowBack.IsThrown;
		}

		public void ShowCurrentWeapon(bool show)
		{
			if (show)
			{
				this.Amanecidas.AnimatorInyector.SetAmanecidaWeapon(this.currentWeapon);
			}
			else
			{
				this.Amanecidas.AnimatorInyector.SetAmanecidaWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.HAND);
			}
		}

		private Vector2 GetTargetPosition(float error = 0f)
		{
			Vector2 vector = Core.Logic.Penitent.transform.position + Vector2.up;
			return vector + new Vector2(Random.Range(-error, error), Random.Range(-error, error));
		}

		private BezierSpline CopySplineFrom(BezierSpline spline)
		{
			BezierSpline component = PoolManager.Instance.ReuseObject(this.poolSplinePrefab, spline.transform.position, spline.transform.rotation, false, 1).GameObject.GetComponent<BezierSpline>();
			component.transform.localScale = Vector3.one;
			component.Copy(spline);
			return component;
		}

		public void StartIntro()
		{
			this.LaunchAction(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_Intro);
		}

		private EnemyAction LaunchAction_StompSmash()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_StompAttack);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.multiStompAttack_EA.StartAction(this, attackNumberOfRepetitions, 0f, true, true, true, false, 1f);
		}

		private EnemyAction LaunchAction_FlyAndSpin()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlyAndSpin);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.FollowAndSpinAxes(this.aroundThrow, this.aroundThrow2);
		}

		private EnemyAction LaunchAction_FollowAndLavaBall()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndLavaBall);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.launchBallsToPenitent_EA.StartAction(this, 1f, 2, true) : this.launchBallsToPenitent_EA.StartAction(this, 0.8f, 5, true);
		}

		private EnemyAction LaunchAction_FollowAndTossAxes()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndAxeToss);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.launchAxesToPenitent_EA.StartAction(this, this.axes[0], this.axes[1], 1f, attackNumberOfRepetitions);
		}

		private EnemyAction LaunchAction_FollowAndTossCrawlerAxes()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FollowAndCrawlerAxeToss);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.launchCrawlerAxesToPenitent_EA.StartAction(this, this.axes[0], this.axes[1], 1f, attackNumberOfRepetitions);
		}

		private EnemyAction LaunchAction_FlameBlade()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlameBlade);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			Vector2 zero = Vector2.zero;
			if (base.transform.position.x > this.battleBounds.center.x)
			{
				zero..ctor(this.battleBounds.xMax, this.battleBounds.yMin - 0.2f);
			}
			else
			{
				zero..ctor(this.battleBounds.xMin, this.battleBounds.yMin - 0.2f);
			}
			return this.meleeProjectile_EA.StartAction(this, zero, 1.5f, new Action(this.Amanecidas.Audio.PlayAxeAttack_AUDIO), this.flameBladeAttack, attackNumberOfRepetitions);
		}

		private EnemyAction LaunchAction_DualThrow()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_DualThrow);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.LaunchDualAxesSameThrow(this.horizontalThrow, Vector2.zero, false, false);
		}

		private EnemyAction LaunchAction_DualThrowCross()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_DualThrowCross);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.LaunchDualAxesSameThrow(this.crossThrow, Vector2.up, false, true);
		}

		private EnemyAction LaunchAction_FlyAndToss()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlyAndToss);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.LaunchDualAxesVertical(this.straightThrow, true, false);
		}

		private EnemyAction LaunchAction_JumpSmash()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_JumpSmash);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.JumpSmashAttack(false);
		}

		private EnemyAction LaunchAction_JumpSmashWithPillars()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_JumpSmashWithPillars);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.JumpSmashAttack(true);
		}

		private EnemyAction LaunchAction_AxeMeleeAttack()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_MeleeAttack);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.meleeAttack_EA.StartAction(this, 0.7f, new Action(this.Amanecidas.Audio.PlayAxeAttack_AUDIO), false);
		}

		private EnemyAction LaunchComboAction_StompNLavaBalls()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_COMBO_StompNLavaBalls);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.CurrentLife > this.Amanecidas.Stats.Life.Final / 2f) ? this.multiStompNLavaBalls_ECA.StartAction(this, new Action(this.DoLavaBallJumpSmash), attackNumberOfRepetitions) : this.multiStompNLavaBalls_ECA.StartAction(this, new Action(this.DoLavaBallJumpSmash), attackNumberOfRepetitions);
		}

		private EnemyAction LaunchAction_RicochetShot()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_RicochetShot);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.shootRicochetArrow_EA.StartAction(this, Mathf.RoundToInt((float)attackConfig.repetitions * 1.5f), new Action<Vector2, Vector2>(this.ShowRicochetArrowTrail), new Action<Vector2, Vector2>(this.ShootRicochetArrow), 2f, this.ricochetMask, false, false) : this.shootRicochetArrow_EA.StartAction(this, attackConfig.repetitions, new Action<Vector2, Vector2>(this.ShowRicochetArrowTrail), new Action<Vector2, Vector2>(this.ShootRicochetArrow), 1.5f, this.ricochetMask, false, false);
		}

		private EnemyAction LaunchAction_FreezeTimeBlinkShots()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FreezeTimeBlinkShots);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.freezeTimeBlinkShots_EA.StartAction(this, attackNumberOfRepetitions, 0.1f, true, new Action(this.SetFrozenArrow), new Action(this.ActivateFrozenArrows)) : this.freezeTimeBlinkShots_EA.StartAction(this, attackNumberOfRepetitions + 4, 0f, false, new Action(this.SetFrozenArrowVariation), new Action(this.ActivateFrozenArrows));
		}

		private EnemyAction LaunchAction_FreezeTimeMultiShots()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FreezeTimeMultiShots);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.freezeTimeMultiShots_EA.StartAction(this, attackNumberOfRepetitions, 0.9f, new Action(this.SetThreeMultiFrozenArrows), new Action(this.ActivateMultiFrozenArrows)) : this.freezeTimeMultiShots_EA.StartAction(this, attackNumberOfRepetitions + 4, 0.7f, new Action(this.SetFourMultiFrozenArrows), new Action(this.ActivateMultiFrozenArrows));
		}

		private EnemyAction LaunchAction_MineShots()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_MineShots);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			Vector2 originPoint;
			Vector2 endPoint;
			if (Core.Logic.Penitent.GetPosition().x > this.battleBounds.center.x)
			{
				originPoint..ctor(this.battleBounds.xMin - 0.45f, this.battleBounds.yMin - 0.55f);
				endPoint..ctor(this.battleBounds.xMin + 2f, this.battleBounds.yMax + 1.7f);
			}
			else
			{
				originPoint..ctor(this.battleBounds.xMax + 0.45f, this.battleBounds.yMin - 0.55f);
				endPoint..ctor(this.battleBounds.xMax - 2f, this.battleBounds.yMax + 1.7f);
			}
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.shootMineArrows_EA.StartAction(this, attackNumberOfRepetitions, originPoint, endPoint, new Action(this.ShootMineArrow), new Action(this.ActivateMines), new Action<Vector2, Vector2>(this.ShowMineArrowTrail), 2.4f, 0.55f, 1f) : this.shootMineArrows_EA.StartAction(this, attackNumberOfRepetitions + 2, originPoint, endPoint, new Action(this.ShootMineArrow), new Action(this.ActivateMines), new Action<Vector2, Vector2>(this.ShowMineArrowTrail), 2.4f, 0.45f, 0.5f);
		}

		private EnemyAction LaunchAction_FastShot()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShot);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.fastShot_EA.StartAction(this, 0.3f, this.bulletTimeProjectileAttack) : this.fastShot_EA.StartAction(this, 0.2f, this.bulletTimeProjectileAttack);
		}

		private EnemyAction LaunchAction_FastShots()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_FastShots);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.fastShots_EA.StartAction(this, 0.4f, attackNumberOfRepetitions, this.bulletTimeProjectileAttack) : this.fastShots_EA.StartAction(this, 0.4f, attackNumberOfRepetitions + 1, this.bulletTimeProjectileAttack);
		}

		private EnemyAction LaunchAction_ChargedShot()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_ChargedShot);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.chargedShot_EA.StartAction(this, new Action(this.StartChargingArrow), new Action(this.ReleaseChargedArrow), 4f, 4f) : this.chargedShot_EA.StartAction(this, new Action(this.StartChargingArrow), new Action(this.ReleaseChargedArrow), 4f, 5f);
		}

		private EnemyAction LaunchAction_SpikesBlinkShot()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_SpikesBlinkShot);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.spikesBlinkShots_EA.StartAction(this, attackNumberOfRepetitions, 1.2f, 0.3f, new Action(this.ShootSpikeSummoningArrow)) : this.spikesBlinkShots_EA.StartAction(this, attackNumberOfRepetitions + 2, 1f, 0.2f, new Action(this.ShootSpikeSummoningArrow));
		}

		private EnemyAction LaunchComboAction_FreezeTimeNRicochetShots()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_COMBO_FreezeTimeNRicochetShots);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.CurrentLife > this.Amanecidas.Stats.Life.Final / 2f) ? this.freezeTimeNRicochetShots_ECA.StartAction(this, attackNumberOfRepetitions + 3, 0f, false, new Action(this.SetFrozenArrowVariation), new Action(this.ActivateFrozenArrows), attackNumberOfRepetitions, new Action<Vector2, Vector2>(this.ShowRicochetArrowTrail), new Action<Vector2, Vector2>(this.ShootRicochetArrow), 1.8f, this.ricochetMask) : this.freezeTimeNRicochetShots_ECA.StartAction(this, attackNumberOfRepetitions + 5, 0f, false, new Action(this.SetFrozenArrowVariation), new Action(this.ActivateFrozenArrows), attackNumberOfRepetitions + 2, new Action<Vector2, Vector2>(this.ShowRicochetArrowTrail), new Action<Vector2, Vector2>(this.ShootRicochetArrow), 1.6f, this.ricochetMask);
		}

		private EnemyAction LaunchAction_MoveBattleBounds()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			this.extraRecoverySeconds = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_MoveBattleBounds).recoverySeconds;
			Vector2 zero = Vector2.zero;
			if (this.laudesBowFightPhase == AmanecidaArena.WEAPON_FIGHT_PHASE.FIRST)
			{
				zero..ctor(-2f, 8.5f);
			}
			else
			{
				zero..ctor(2f, 8.5f);
			}
			this.laudesBowFightPhase++;
			return this.moveBattleBounds_EA.StartAction(this, zero, 0.2f, this.laudesBowFightPhase);
		}

		private EnemyAction LaunchAction_LaserShot()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			this.extraRecoverySeconds = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.BOW_LaserShot).recoverySeconds;
			return this.shootLaserArrow_EA.StartAction(this, 0.3f, this.floorMask, new Action<Vector2, Vector2>(this.ShowRicochetArrowTrailFast), new Action<Vector2, Vector2>(this.ShootRicochetArrow));
		}

		private EnemyAction LaunchAction_FreezeTimeHail()
		{
			Vector2 firstOriginPoint;
			firstOriginPoint..ctor(this.battleBounds.center.x + 0.1f, this.battleBounds.yMax);
			Vector2 firstEndPoint;
			firstEndPoint..ctor(this.battleBounds.xMax + 0.5f, this.battleBounds.yMax);
			Vector2 secondOriginPoint;
			secondOriginPoint..ctor(this.battleBounds.center.x - 0.1f, this.battleBounds.yMax);
			Vector2 secondEndPoint;
			secondEndPoint..ctor(this.battleBounds.xMin - 0.5f, this.battleBounds.yMax);
			if (this.IsPenitentInTop())
			{
				firstOriginPoint.y += 2f;
				firstEndPoint.y += 2f;
				secondOriginPoint.y += 2f;
				secondEndPoint.y += 2f;
			}
			Vector2 targetPosition = this.battleBounds.center + Vector2.up;
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeHail);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.doubleShootFrozenLances_EA.StartAction(this, attackNumberOfRepetitions, targetPosition, firstOriginPoint, firstEndPoint, secondOriginPoint, secondEndPoint, new Action<Vector2>(this.SetFrozenHail), new Action(this.ActivateFrozenHail), 0.2f, 1f, 0f, true);
		}

		private EnemyAction LaunchAction_FreezeTimeLances()
		{
			Penitent penitent = Core.Logic.Penitent;
			Vector2 originPoint;
			Vector2 endPoint;
			if (penitent.GetPosition().x > this.battleBounds.center.x)
			{
				originPoint..ctor(this.battleBounds.xMin + 0.5f, this.battleBounds.yMax + 2f);
				endPoint..ctor(this.battleBounds.xMax - 0.5f, this.battleBounds.yMax + 2f);
			}
			else
			{
				originPoint..ctor(this.battleBounds.xMax - 0.5f, this.battleBounds.yMax + 2f);
				endPoint..ctor(this.battleBounds.xMin + 0.5f, this.battleBounds.yMax + 2f);
			}
			if (this.IsPenitentInTop())
			{
				originPoint.y += 2f;
				endPoint.y += 2f;
			}
			Vector2 targetPosition = this.battleBounds.center + Vector2.up;
			float num = Random.Range(-0.1f, 0.1f);
			originPoint.x += num;
			endPoint.x += num;
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeLances);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.shootFrozenLances_EA.StartAction(this, attackNumberOfRepetitions, originPoint, endPoint, targetPosition, new Action<Vector2>(this.SetFrozenLance), new Action(this.ActivateFrozenLances), 0.2f, 0.3f, 0.1f, false) : this.shootFrozenLances_EA.StartAction(this, Mathf.FloorToInt((float)attackNumberOfRepetitions * 1.5f), originPoint, endPoint, targetPosition, new Action<Vector2>(this.SetFrozenLance), new Action(this.ActivateFrozenLances), 0.1f, 0.2f, 0.1f, false);
		}

		private EnemyAction LaunchAction_FreezeTimeLancesOnPenitent()
		{
			Vector3 position = Core.Logic.Penitent.transform.position;
			Vector2 originPoint;
			originPoint..ctor(position.x, this.battleBounds.yMax + 2f);
			Vector2 endPoint;
			endPoint..ctor(position.x, this.battleBounds.yMax + 2f);
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_FreezeTimeLancesOnPenitent);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			int num = (this.Amanecidas.shieldCurrentHP > 0f) ? attackNumberOfRepetitions : Mathf.FloorToInt((float)attackNumberOfRepetitions * 1.5f);
			originPoint.x += (float)num * 0.55f * (float)this.GetDirFromOrientation();
			endPoint.x -= (float)num * 0.55f * (float)this.GetDirFromOrientation();
			if (this.IsPenitentInTop())
			{
				originPoint.y += 2f;
				endPoint.y += 2f;
			}
			Vector2 targetPosition;
			targetPosition..ctor(endPoint.x - (float)this.GetDirFromOrientation(), this.battleBounds.yMax + 1f);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.shootFrozenLances_EA.StartAction(this, attackNumberOfRepetitions, originPoint, endPoint, targetPosition, new Action<Vector2>(this.SetFrozenLance), new Action(this.ActivateFrozenLances), 0.05f, 0.2f, 0f, false) : this.shootFrozenLances_EA.StartAction(this, num, originPoint, endPoint, targetPosition, new Action<Vector2>(this.SetFrozenLance), new Action(this.ActivateFrozenLances), 0.05f, 0.1f, 0f, false);
		}

		private EnemyAction LaunchAction_LanceBlinkDash()
		{
			Penitent penitent = Core.Logic.Penitent;
			float num = Random.Range(0f, 1f);
			Vector2 vector = (num <= 0.5f) ? (penitent.GetPosition() + Vector3.right * 6f) : (penitent.GetPosition() + Vector3.left * 6f);
			vector += Vector2.up * 2f;
			BossDashAttack dashAttack = this.lanceDashAttack;
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_BlinkDash);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			return this.blinkDash_EA.StartAction(this, vector, dashAttack, 0.5f, false, false, true, 15f);
		}

		private EnemyAction LaunchAction_ChainDash()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_ChainDash);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.chainDash_EA.StartAction(this, attackNumberOfRepetitions);
		}

		private EnemyAction LaunchAction_FalcataBlinkBehindAndDash()
		{
			Penitent penitent = Core.Logic.Penitent;
			float num = 3f;
			Vector2 pointBelowPenitent = this.GetPointBelowPenitent(true);
			Vector2 vector = (penitent.GetOrientation() != EntityOrientation.Right) ? (pointBelowPenitent + Vector2.right * num) : (pointBelowPenitent + Vector2.left * num);
			vector += Vector2.up * 1f;
			BossDashAttack dashAttack = this.slashDashAttack;
			return this.blinkDash_EA.StartAction(this, vector, dashAttack, 0.5f, false, false, true, 15f);
		}

		private EnemyAction LaunchAction_MultiLunge()
		{
			return this.quickLunge_EA.StartAction(this, 3);
		}

		private EnemyAction LaunchAction_Counter()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD, AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_CounterAttack);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			return this.falcataCounter_EA.StartAction(this, this.dodgeDistance);
		}

		private EnemyAction LaunchAction_JumpBackNDash()
		{
			Penitent penitent = Core.Logic.Penitent;
			Vector2 vector = (penitent.GetOrientation() != EntityOrientation.Right) ? (penitent.GetPosition() + Vector3.right * 6f) : (penitent.GetPosition() + Vector3.left * 6f);
			vector += Vector2.up * 2f;
			BossDashAttack dashAttack = this.lanceDashAttack;
			return this.blinkDash_EA.StartAction(this, vector, dashAttack, 0.5f, false, false, false, 15f);
		}

		private EnemyAction LaunchAction_HorizontalBlinkDashes()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_HorizontalBlinkDashes);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.horizontalBlinkDashes_EA.StartAction(this, attackNumberOfRepetitions, 0.75f, true) : this.horizontalBlinkDashes_EA.StartAction(this, attackNumberOfRepetitions, 0.5f, false);
		}

		private EnemyAction LaunchAction_MultiFrontalDash()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_MultiFrontalDash);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.multiFrontalDash_EA.StartAction(this, attackNumberOfRepetitions, 0.5f);
		}

		private EnemyAction LaunchAction_DiagonalBlinkDashes()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.LANCE_DiagonalBlinkDashes);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return (this.Amanecidas.shieldCurrentHP > 0f) ? this.diagonalBlinkDashes_EA.StartAction(this, attackNumberOfRepetitions, 1.2f, 0.8f, 0.8f) : this.diagonalBlinkDashes_EA.StartAction(this, attackNumberOfRepetitions, 1.2f, 0.8f, 1.2f);
		}

		private EnemyAction LaunchComboAction_FreezeTimeNHorizontalDashes()
		{
			Penitent penitent = Core.Logic.Penitent;
			Vector2 originPoint;
			Vector2 endPoint;
			Vector2 targetPosition;
			if (penitent.GetPosition().x > this.battleBounds.center.x)
			{
				originPoint..ctor(this.battleBounds.xMin - 0.5f, this.battleBounds.yMax + 3f);
				endPoint..ctor(this.battleBounds.xMax + 0.5f, this.battleBounds.yMax + 3f);
				targetPosition.x = Mathf.Clamp(penitent.GetPosition().x - 8f, this.battleBounds.xMin, this.battleBounds.xMax);
			}
			else
			{
				originPoint..ctor(this.battleBounds.xMax + 0.5f, this.battleBounds.yMax + 3f);
				endPoint..ctor(this.battleBounds.xMin - 0.5f, this.battleBounds.yMax + 3f);
				targetPosition.x = Mathf.Clamp(penitent.GetPosition().x + 8f, this.battleBounds.xMin, this.battleBounds.xMax);
			}
			targetPosition.y = this.battleBounds.yMax + 1f;
			float num = Random.Range(-0.1f, 0.1f);
			originPoint.x += num;
			endPoint.x += num;
			return (this.Amanecidas.CurrentLife > this.Amanecidas.Stats.Life.Final / 2f) ? this.freezeTimeNHorizontalDashes_ECA.StartAction(this, 16, originPoint, endPoint, targetPosition, new Action<Vector2>(this.SetFrozenLance), new Action(this.ActivateFrozenLances), 0.5f, 0.3f, false, 3, 0.5f, true) : this.freezeTimeNHorizontalDashes_ECA.StartAction(this, 32, originPoint, endPoint, targetPosition, new Action<Vector2>(this.SetFrozenLance), new Action(this.ActivateFrozenLances), 0.5f, 1f, true, 5, 0.3f, false);
		}

		private EnemyAction LaunchAction_FalcataSlashProjectile()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_SlashProjectile);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			List<Vector2> dirs = new List<Vector2>
			{
				this.GetDirToPenitent(base.transform.position)
			};
			return this.falcataSlashProjectile_EA.StartAction(this, dirs, 0.15f);
		}

		private EnemyAction LaunchAction_FalcataSlashBarrage()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_SlashBarrage);
			int num = this.GetAttackNumberOfRepetitions(attackConfig);
			bool startsFromRight = true;
			if (this.IsShieldBroken())
			{
				num += 2;
				startsFromRight = false;
			}
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			return this.falcataSlashBarrage_EA.StartAction(this, num, startsFromRight, 5f, 0.4f);
		}

		private EnemyAction LaunchAction_NoxiousBlade()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_NoxiousBlade);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			Vector2 zero = Vector2.zero;
			if (base.transform.position.x > this.battleBounds.center.x)
			{
				zero..ctor(this.battleBounds.xMax, this.battleBounds.yMin - 0.2f);
			}
			else
			{
				zero..ctor(this.battleBounds.xMin, this.battleBounds.yMin - 0.2f);
			}
			return this.meleeProjectile_EA.StartAction(this, zero, 1f, new Action(this.Amanecidas.Audio.PlaySwordAttack_AUDIO), this.noxiousBladeAttack, attackNumberOfRepetitions);
		}

		private EnemyAction LaunchAction_SpinAttack()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_SpinAttack);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			int attackNumberOfRepetitions = this.GetAttackNumberOfRepetitions(attackConfig);
			return this.spinAttack.StartAction(this, attackNumberOfRepetitions);
		}

		private EnemyAction LaunchAction_FalcataCombo()
		{
			return this.falcataSlashStorm_ECA.StartAction(this, 1);
		}

		private EnemyAction LaunchAction_Intro()
		{
			return this.intro_EA.StartAction(this);
		}

		private EnemyAction LaunchAction_Death()
		{
			return this.death_EA.StartAction(this);
		}

		private EnemyAction LaunchAction_HurtDisplacement()
		{
			return this.hurtDisplacement_EA.StartAction(this, 0.5f);
		}

		private EnemyAction LaunchAction_Hurt(bool isLastHurt)
		{
			return this.hurt_EA.StartAction(this, isLastHurt);
		}

		private EnemyAction LaunchAction_BlinkAway()
		{
			bool flag = false;
			Vector2 vector = this.battleBounds.center;
			while (!flag)
			{
				vector = this.RandomPointInsideRect(this.battleBounds);
				if (Vector2.Distance(vector, Core.Logic.Penitent.transform.position) > 2f)
				{
					flag = true;
				}
			}
			return this.blink_EA.StartAction(this, vector, 2f, true, false);
		}

		private EnemyAction LaunchAction_ChangeWeapon()
		{
			return this.changeWeapon_EA.StartAction(this, 1f);
		}

		private EnemyAction LaunchAction_RechargeShield()
		{
			return this.recoverShield_EA.StartAction(this, (float)this.currentFightParameters.shieldRechargeTime, (float)this.currentFightParameters.shieldShockwaveAnticipationTime, new Action(this.DoRechargeShield), new Action(this.DoAnticipateShockwave), new Action(this.DoShieldShockwave));
		}

		private EnemyAction LaunchAction_FalcataMeleeAttack()
		{
			AmanecidaAttackScriptableConfig amanecidaAttackScriptableConfig = (!this.Amanecidas.IsLaudes) ? this.attackConfigData : this.laudesAttackConfigData;
			AmanecidaAttackScriptableConfig.AmanecidaAttackConfig attackConfig = amanecidaAttackScriptableConfig.GetAttackConfig(this.currentWeapon, AmanecidasBehaviour.AMANECIDA_ATTACKS.FALCATA_MeleeAttack);
			this.extraRecoverySeconds = this.GetAttackRecoverySeconds(attackConfig);
			return this.meleeAttack_EA.StartAction(this, 0.5f, new Action(this.Amanecidas.Audio.PlaySwordAttack_AUDIO), false);
		}

		private EnemyAction JumpSmashAttack(bool pillars)
		{
			Vector2 jumpOrigin;
			jumpOrigin..ctor(this.battleBounds.xMin, this.battleBounds.yMin);
			if (Core.Logic.Penitent.GetPosition().x < this.battleBounds.center.x)
			{
				jumpOrigin.x = this.battleBounds.xMax;
			}
			return (!pillars) ? this.jumpSmash_EA.StartAction(this, jumpOrigin, new Action(this.DoJumpSmash), false) : this.jumpSmash_EA.StartAction(this, jumpOrigin, new Action(this.DoJumpSmashWithPillars), true);
		}

		private EnemyAction ThrowAxeAtPlayer()
		{
			Vector2 origin = base.transform.position + Vector3.right * 2f + Vector3.up * 1f;
			Vector2 targetPosition = this.GetTargetPosition(0f);
			BezierSpline spline = this.CopySplineFrom(this.horizontalThrow.spline);
			return this.axes[0].axeSplineFollowAction.StartAction(this.axes[0], this.axes[0].splineFollower, origin, targetPosition, 3, this.horizontalThrow, spline);
		}

		private EnemyAction ThrowAxeAtDir(AmanecidaAxeBehaviour axe, Vector2 dir)
		{
			Vector2 vector = axe.transform.position;
			Vector2 end = vector + dir;
			BezierSpline spline = this.CopySplineFrom(this.horizontalThrow.spline);
			return this.axes[0].axeSplineFollowAction.StartAction(this.axes[0], this.axes[0].splineFollower, vector, end, 3, this.horizontalThrow, spline);
		}

		private EnemyAction LaunchDualAxesSameThrow(SplineThrowData throwData, Vector2 offset, bool mirroredX = false, bool mirroredY = false)
		{
			BezierSpline item = this.CopySplineFrom(throwData.spline);
			BezierSpline bezierSpline = this.CopySplineFrom(throwData.spline);
			if (mirroredX || mirroredY)
			{
				bezierSpline.transform.localScale = new Vector3((float)((!mirroredX) ? 1 : -1), (float)((!mirroredY) ? 1 : -1), 1f);
			}
			float x = Core.Logic.Penitent.transform.position.x;
			int dir = 1;
			Vector2 vector = new Vector2(this.battleBounds.xMin + 1.4f, this.battleBounds.yMin) + offset;
			if (x < this.battleBounds.center.x)
			{
				dir = -1;
				vector += Vector2.right * (this.battleBounds.width - 2.8f);
			}
			return this.dualAxeThrow_EA.StartAction(this, 2, vector, this.battleBounds.width + 3f, dir, this.axes, new List<BezierSpline>
			{
				item,
				bezierSpline
			}, new List<SplineThrowData>
			{
				throwData,
				throwData
			});
		}

		private EnemyAction LaunchDualAxesVertical(SplineThrowData throwData, bool mirroredX = false, bool mirroredY = false)
		{
			BezierSpline item = this.CopySplineFrom(throwData.spline);
			BezierSpline bezierSpline = this.CopySplineFrom(throwData.spline);
			if (mirroredX || mirroredY)
			{
				bezierSpline.transform.localScale = new Vector3((float)((!mirroredX) ? 1 : -1), (float)((!mirroredY) ? 1 : -1), 1f);
			}
			int n = 4;
			if (this.Amanecidas.shieldCurrentHP <= 0f)
			{
				n = 8;
			}
			return this.dualAxeFlyingThrow_EA.StartAction(this, n, Vector2.down * 8f, this.axes, new List<BezierSpline>
			{
				item,
				bezierSpline
			}, new List<SplineThrowData>
			{
				throwData,
				throwData
			});
		}

		private EnemyAction SpinAxes(SplineThrowData throwData1, SplineThrowData throwData2)
		{
			return this.spinAxesAround_EA.StartAction(this, 3, this.axes, new List<BezierSpline>
			{
				throwData1.spline,
				throwData2.spline
			}, new List<SplineThrowData>
			{
				throwData1,
				throwData2
			});
		}

		private EnemyAction FollowAndSpinAxes(SplineThrowData throwData1, SplineThrowData throwData2)
		{
			return this.followAndSpinAxes_EA.StartAction(this, 3, this.flightPattern, this.axes, new List<BezierSpline>
			{
				throwData1.spline,
				throwData2.spline
			}, new List<SplineThrowData>
			{
				throwData1,
				throwData2
			});
		}

		public bool MoveBattleBoundsIfNeeded()
		{
			if (this.laudesBowFightPhase == AmanecidaArena.WEAPON_FIGHT_PHASE.THIRD)
			{
				return false;
			}
			this._fsm.ChangeState(this.stAction);
			this.StopCurrentAction();
			this.LaunchAction(AmanecidasBehaviour.AMANECIDA_ATTACKS.COMMON_MoveBattleBounds);
			return true;
		}

		public void AimToPenitentWithBow()
		{
			Vector2 dirToPenitent = this.GetDirToPenitent(base.transform.position);
			this.AimToPointWithBow(base.transform.position + dirToPenitent);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(this.battleBounds.center, this.battleBounds.size);
			Gizmos.DrawWireSphere(base.transform.position + this.axeOffset, 0.1f);
			if (this.debugDrawCurrentAction)
			{
				string text = "Waiting action";
				if (this.currentAction != null)
				{
					text = this.currentAction.ToString();
				}
				GizmoExtensions.DrawString(text, base.transform.position - Vector3.up * 0.5f, new Color?(Color.green));
			}
			Gizmos.DrawWireSphere(this.bowAimTarget, 0.1f);
		}

		[FoldoutGroup("Character settings", 0)]
		public AnimationCurve timeSlowCurve;

		[FoldoutGroup("Character settings", 0)]
		public Vector2 centerBodyOffset = new Vector2(0f, 1.2f);

		[FoldoutGroup("Battle area", 0)]
		public Rect battleBounds;

		[FoldoutGroup("Battle area", 0)]
		public Transform combatAreaParent;

		[FoldoutGroup("Movement", 0)]
		public AutonomousAgentConfig floatingConfig;

		[FoldoutGroup("Movement", 0)]
		public AutonomousAgentConfig actionConfig;

		[FoldoutGroup("Movement", 0)]
		public AutonomousAgentConfig keepDistanceConfig;

		[FoldoutGroup("Movement", 0)]
		public float minFloatingPointCD = 1f;

		[FoldoutGroup("Movement", 0)]
		public float maxFloatingPointCD = 3f;

		[FoldoutGroup("Movement", 0)]
		public LayerMask floorNOneWayDownMask;

		[FoldoutGroup("Movement", 0)]
		public LayerMask floorMask;

		[FoldoutGroup("Attacks config", 0)]
		public AmanecidaAttackScriptableConfig attackConfigData;

		[FoldoutGroup("Laudes only", 0)]
		public AmanecidaAttackScriptableConfig laudesAttackConfigData;

		[TableList]
		[FoldoutGroup("Laudes only", 0)]
		public List<AmanecidasBehaviour.AmanecidasFightParameters> laudesFightParameters;

		[TableList]
		[FoldoutGroup("Axe", 0)]
		public List<AmanecidasBehaviour.AmanecidasFightParameters> axeFightParameters;

		[FoldoutGroup("Axe", 0)]
		public SplineThrowData horizontalThrow;

		[FoldoutGroup("Axe", 0)]
		public SplineThrowData circleDownThrow;

		[FoldoutGroup("Axe", 0)]
		public SplineThrowData crossThrow;

		[FoldoutGroup("Axe", 0)]
		public SplineThrowData aroundThrow;

		[FoldoutGroup("Axe", 0)]
		public SplineThrowData aroundThrow2;

		[FoldoutGroup("Axe", 0)]
		public SplineThrowData straightThrow;

		[FoldoutGroup("Axe", 0)]
		public GameObject poolSplinePrefab;

		[FoldoutGroup("Axe", 0)]
		public GameObject amanecidaAxePrefab;

		[FoldoutGroup("Axe", 0)]
		public List<AmanecidaAxeBehaviour> axes;

		[FoldoutGroup("Axe", 0)]
		public Transform splineParent;

		[FoldoutGroup("Axe", 0)]
		public SplineFollower splineFollower;

		[FoldoutGroup("Axe", 0)]
		public SplineThrowData flightPattern;

		[FoldoutGroup("Axe", 0)]
		public Vector2 axeOffset;

		[FoldoutGroup("Axe", 0)]
		public List<Transform> axeTargets;

		[TableList]
		[FoldoutGroup("Bow", 0)]
		public List<AmanecidasBehaviour.AmanecidasFightParameters> bowFightParameters;

		[FoldoutGroup("Bow", 0)]
		public LayerMask ricochetMask;

		[FoldoutGroup("Bow", 0)]
		public float bowAngleDifference = 40f;

		[TableList]
		[FoldoutGroup("Lance", 0)]
		public List<AmanecidasBehaviour.AmanecidasFightParameters> lanceFightParameters;

		[FoldoutGroup("Lance", 0)]
		public float lanceRotationDiference = 40f;

		[TableList]
		[FoldoutGroup("Falcata", 0)]
		public List<AmanecidasBehaviour.AmanecidasFightParameters> falcataFightParameters;

		[FoldoutGroup("Falcata", 0)]
		public float falcataRotationDiference = 30f;

		[FoldoutGroup("Falcata", 0)]
		public SplineThrowData dodgeSplineData;

		[FoldoutGroup("Falcata", 0)]
		public float dodgeMaxCooldown;

		[FoldoutGroup("Falcata", 0)]
		public float dodgeDistance;

		[FoldoutGroup("Falcata", 0)]
		public GameObject clonePrefab;

		[FoldoutGroup("Shield", 0)]
		public GameObject shieldShockwave;

		[FoldoutGroup("VFX", 0)]
		public GameObject vortexVFX;

		[FoldoutGroup("VFX", 0)]
		public GameObject dustVFX;

		[FoldoutGroup("VFX", 0)]
		public ParticleSystem chargeParticles;

		[FoldoutGroup("VFX", 0)]
		public ParticleSystem chargeParticlesLongRange;

		[FoldoutGroup("Debug", 0)]
		public bool debugDrawCurrentAction;

		public GameObject trailGameObject;

		private StateMachine<AmanecidasBehaviour> _fsm;

		private State<AmanecidasBehaviour> stFloating;

		private State<AmanecidasBehaviour> stAction;

		private State<AmanecidasBehaviour> stIntro;

		private State<AmanecidasBehaviour> stDeath;

		private State<AmanecidasBehaviour> stHurt;

		private State<AmanecidasBehaviour> stRecharge;

		private WaitSeconds_EnemyAction waitBetweenActions_EA;

		private AmanecidasBehaviour.JumpBackAndShoot_EnemyAction jumpBackNShoot_EA;

		private AmanecidasBehaviour.ShootRicochetArrow_EnemyAction shootRicochetArrow_EA;

		private AmanecidasBehaviour.ShootLaserArrow_EnemyAction shootLaserArrow_EA;

		private AmanecidasBehaviour.ShootMineArrows_EnemyAction shootMineArrows_EA;

		private AmanecidasBehaviour.FreezeTimeBlinkShots_EnemyAction freezeTimeBlinkShots_EA;

		private AmanecidasBehaviour.FreezeTimeMultiShots_EnemyAction freezeTimeMultiShots_EA;

		private AmanecidasBehaviour.FastShot_EnemyAction fastShot_EA;

		private AmanecidasBehaviour.FastShots_EnemyAction fastShots_EA;

		private AmanecidasBehaviour.ChargedShot_EnemyAction chargedShot_EA;

		private AmanecidasBehaviour.SpikesBlinkShots_EnemyAction spikesBlinkShots_EA;

		private AmanecidasBehaviour.FreezeTimeNRicochetShots_EnemyComboAction freezeTimeNRicochetShots_ECA;

		private AmanecidasBehaviour.MoveBattleBounds_EnemyAction moveBattleBounds_EA;

		private AmanecidasBehaviour.ShootFrozenLances_EnemyAction shootFrozenLances_EA;

		private AmanecidasBehaviour.DoubleShootFrozenLances_EnemyAction doubleShootFrozenLances_EA;

		private AmanecidasBehaviour.HorizontalBlinkDashes_EnemyAction horizontalBlinkDashes_EA;

		private AmanecidasBehaviour.MultiFrontalDash_EnemyAction multiFrontalDash_EA;

		private AmanecidasBehaviour.DiagonalBlinkDashes_EnemyAction diagonalBlinkDashes_EA;

		private AmanecidasBehaviour.FreezeTimeNHorizontalDashes_EnemyComboAction freezeTimeNHorizontalDashes_ECA;

		private AmanecidasBehaviour.FalcataSlashStorm_EnemyComboAction falcataSlashStorm_ECA;

		private AmanecidasBehaviour.LaunchTwoAxesHorizontal_EnemyAction dualAxeThrow_EA;

		private AmanecidasBehaviour.HurtDisplacement_EnemyAction hurtDisplacement_EA;

		private AmanecidasBehaviour.Hurt_EnemyAction hurt_EA;

		private AmanecidasBehaviour.FlyAndLaunchTwoAxes_EnemyAction dualAxeFlyingThrow_EA;

		private AmanecidasBehaviour.SpinAxesAround_EnemyAction spinAxesAround_EA;

		private AmanecidasBehaviour.FollowSplineAndSpinAxesAround_EnemyAction followAndSpinAxes_EA;

		private AmanecidasBehaviour.JumpSmash_EnemyAction jumpSmash_EA;

		private AmanecidasBehaviour.LaunchAxesToPenitent_EnemyAction launchAxesToPenitent_EA;

		private AmanecidasBehaviour.LaunchCrawlerAxesToPenitent_EnemyAction launchCrawlerAxesToPenitent_EA;

		private AmanecidasBehaviour.LaunchBallsToPenitent_EnemyAction launchBallsToPenitent_EA;

		private AmanecidasBehaviour.MultiStompNLavaBalls_EnemyComboAction multiStompNLavaBalls_ECA;

		private AmanecidasBehaviour.BlinkToPoint_EnemyAction blink_EA;

		private AmanecidasBehaviour.RechargeShield_EnemyAction recoverShield_EA;

		private AmanecidasBehaviour.BlinkAndDashToPenitent_EnemyAction blinkDash_EA;

		private AmanecidasBehaviour.MeleeAttackTowardsPenitent_EnemyAction meleeAttack_EA;

		private AmanecidasBehaviour.MeleeAttackProjectile_EnemyAction meleeProjectile_EA;

		private AmanecidasBehaviour.GhostProjectile_EnemyAction ghostProjectile_EA;

		private AmanecidasBehaviour.Intro_EnemyAction intro_EA;

		private AmanecidasBehaviour.Death_EnemyAction death_EA;

		private AmanecidasBehaviour.FalcataSlashProjectile_EnemyAction falcataSlashProjectile_EA;

		private AmanecidasBehaviour.FalcataSlashBarrage_EnemyAction falcataSlashBarrage_EA;

		private AmanecidasBehaviour.StompAttack_EnemyAction stompAttack_EA;

		private AmanecidasBehaviour.MultiStompAttack_EnemyAction multiStompAttack_EA;

		private AmanecidasBehaviour.ChangeWeapon_EnemyAction changeWeapon_EA;

		private AmanecidasBehaviour.QuickLunge_EnemyAction quickLunge_EA;

		private AmanecidasBehaviour.DodgeAndCounterAttack_EnemyAction falcataCounter_EA;

		private AmanecidasBehaviour.ChainDash_EnemyAction chainDash_EA;

		private AmanecidasBehaviour.ShieldShockwave shieldShockwave_EA;

		private AmanecidasBehaviour.EnergyChargePeriod_EnemyAction chargeEnergy_EA;

		private AmanecidasBehaviour.FalcataSlashOnFloor_EnemyAction spinAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossInstantProjectileAttack instantProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossInstantProjectileAttack arrowTrailInstantProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossInstantProjectileAttack arrowTrailFastInstantProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossInstantProjectileAttack ricochetArrowInstantProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossInstantProjectileAttack arrowInstantProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossInstantProjectileAttack mineArrowProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossAreaSummonAttack chargedArrowExplosionAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossAreaSummonAttack verticalBlastArrowAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossAreaSummonAttack verticalSlowBlastArrowAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossAreaSummonAttack verticalBlastAxeAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossAreaSummonAttack verticalCrystalBeam;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossAreaSummonAttack verticalFastBlastAxeAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossAreaSummonAttack verticalNormalBlastAxeAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossAreaSummonAttack introBeamAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossJumpAttack jumpAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossDashAttack slashDashAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossDashAttack lanceDashAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossStraightProjectileAttack bulletTimeProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossStraightProjectileAttack bulletTimeLanceAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossStraightProjectileAttack bulletTimeHailAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossStraightProjectileAttack falcataSlashProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossStraightProjectileAttack lavaBallAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossStraightProjectileAttack flameBladeAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		private BossStraightProjectileAttack noxiousBladeAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public AmanecidasMeleeAttack meleeAxeAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public BossStraightProjectileAttack ghostProjectileAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public AmanecidasMeleeAttack meleeFalcataAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public AmanecidasMeleeAttack meleeStompAttack;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public BossSpawnedGeoAttack spikePrefab;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public BossAreaSummonAttack shockwave;

		[FoldoutGroup("Attack references", 0)]
		[SerializeField]
		public TileableBeamLauncher beamLauncher;

		public AmanecidasAnimatorInyector.AMANECIDA_WEAPON currentWeapon;

		private List<AmanecidasBehaviour.AMANECIDA_ATTACKS> availableAttacks = new List<AmanecidasBehaviour.AMANECIDA_ATTACKS>
		{
			AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_DualThrow,
			AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_FlyAndSpin,
			AmanecidasBehaviour.AMANECIDA_ATTACKS.AXE_JumpSmash
		};

		private AmanecidasBehaviour.AMANECIDA_ATTACKS lastAttack;

		private AmanecidasBehaviour.AMANECIDA_ATTACKS attackBeforeLastAttack;

		private Dictionary<AmanecidasBehaviour.AMANECIDA_ATTACKS, Func<EnemyAction>> actionsDictionary = new Dictionary<AmanecidasBehaviour.AMANECIDA_ATTACKS, Func<EnemyAction>>();

		private EnemyAction currentAction;

		private List<AmanecidasBehaviour.AmanecidasFightParameters> currentFightParameterList;

		private AutonomousAgent agent;

		private RaycastHit2D[] results;

		private int _currentHurtHits;

		private AmanecidasBehaviour.AmanecidasFightParameters currentFightParameters;

		private float floatingCounter;

		private int actionsBeforeShieldRecharge;

		private float damageWhileRecharging;

		private bool _interruptable;

		private float extraRecoverySeconds;

		private Healing penitentHealing;

		private bool lastShieldRechargeWasInterrupted;

		private float throwbackExtraTime;

		private float maxThrowbackExtraTime = 0.5f;

		private bool dodge;

		private Vector2 bowAimTarget;

		private Dictionary<string, float> damageParameters;

		private float dodgeCooldown;

		private List<BulletTimeProjectile> bullets;

		private List<BulletTimeProjectile> multiBullets;

		private float lastX;

		private List<BossSpawnedAreaAttack> mines;

		private AmanecidasMeleeAttack currentMeleeAttack;

		private int numActiveFlamePillarPairs;

		public bool needsToSwapWeapon;

		private AmanecidaArena.WEAPON_FIGHT_PHASE laudesBowFightPhase;

		private int actionsBeforeMovingBattlebounds = 5;

		private const string AXE_DAMAGE = "AXE_DMG";

		[Serializable]
		public struct AmanecidasFightParameters
		{
			public AmanecidasAnimatorInyector.AMANECIDA_WEAPON weapon;

			[ProgressBar(0.0, 1.0, 0.8f, 0f, 0.1f)]
			[SuffixLabel("%", false)]
			public float hpPercentageBeforeApply;

			[MinMaxSlider(0f, 5f, true)]
			public Vector2 minMaxWaitingTimeBetweenActions;

			[SuffixLabel("hits", true)]
			public int maxHitsInHurt;

			[SuffixLabel("actions", true)]
			public int maxActionsBeforeShieldRecharge;

			[SuffixLabel("seconds", true)]
			public int shieldRechargeTime;

			[SuffixLabel("seconds", true)]
			public int shieldShockwaveAnticipationTime;

			[SuffixLabel("damage", true)]
			public int maxDamageBeforeInterruptingRecharge;
		}

		public enum AMANECIDA_ATTACKS
		{
			AXE_FlyAndSpin,
			AXE_DualThrow,
			AXE_JumpSmash,
			AXE_JumpSmashWithPillars = 39,
			AXE_DualThrowCross = 20,
			AXE_FlyAndToss,
			AXE_MeleeAttack = 25,
			AXE_FlameBlade = 35,
			AXE_FollowAndAxeToss,
			AXE_FollowAndCrawlerAxeToss = 43,
			AXE_FollowAndLavaBall = 37,
			AXE_COMBO_StompNLavaBalls = 101,
			BOW_RicochetShot = 3,
			BOW_FreezeTimeBlinkShots,
			BOW_MineShots,
			BOW_FreezeTimeMultiShots = 19,
			BOW_FastShot = 26,
			BOW_FastShots,
			BOW_ChargedShot,
			BOW_SpikesBlinkShot,
			BOW_COMBO_FreezeTimeNRicochetShots = 102,
			BOW_LaserShot = 41,
			LANCE_JumpBackAndDash = 6,
			LANCE_BlinkDash,
			LANCE_FreezeTimeLances,
			LANCE_FreezeTimeLancesOnPenitent = 42,
			LANCE_HorizontalBlinkDashes = 13,
			LANCE_DiagonalBlinkDashes,
			LANCE_MultiFrontalDash = 33,
			LANCE_FreezeTimeHail,
			LANCE_COMBO_FreezeTimeNHorizontalDashes = 103,
			FALCATA_BlinkDash = 9,
			FALCATA_MeleeAttack = 15,
			FALCATA_SpinAttack,
			FALCATA_SlashProjectile,
			FALCATA_SlashBarrage,
			FALCATA_QuickLunge = 24,
			FALCATA_ChainDash = 32,
			FALCATA_COMBO_STORM = 104,
			FALCATA_CounterAttack = 31,
			FALCATA_NoxiousBlade = 38,
			COMMON_BlinkAway = 10,
			COMMON_RechargeShield,
			COMMON_Intro,
			COMMON_StompAttack = 22,
			COMMON_ChangeWeapon,
			COMMON_Death = 30,
			COMMON_MoveBattleBounds = 40
		}

		public enum AMANECIDA_GRUNTS
		{
			MULTI_FRONTAL_GRUNT,
			GRUNT2
		}

		public class HurtDisplacement_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float amount)
			{
				this.displacementAmount = amount;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE_CHARACTER.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour amanecida = this.owner as AmanecidasBehaviour;
				Vector2 dir = (this.owner.transform.position - Core.Logic.Penitent.transform.position).normalized;
				Vector2 pos = amanecida.GetValidPointInDirection(dir, this.displacementAmount);
				this.ACT_MOVE_CHARACTER.StartAction(this.owner, amanecida.agent, pos, 2f);
				yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private float displacementAmount;

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();
		}

		public class Hurt_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, bool isLastHurt)
			{
				this.isLastHurt = isLastHurt;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_DISP.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				if (this.isLastHurt)
				{
					amanecidasBehaviour.SetInterruptable(false);
				}
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				ama.Amanecidas.AnimatorInyector.PlayHurt();
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				this.ACT_DISP.StartAction(this.owner, 0.5f);
				yield return this.ACT_DISP.waitForCompletion;
				if (this.isLastHurt)
				{
					ama.SetInterruptable(false);
				}
				float hurtRecoveryTime = 0.5f;
				this.ACT_WAIT.StartAction(this.owner, hurtRecoveryTime);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				base.FinishAction();
				yield break;
			}

			private bool isLastHurt;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.HurtDisplacement_EnemyAction ACT_DISP = new AmanecidasBehaviour.HurtDisplacement_EnemyAction();
		}

		public class Death_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				ama.GetComponentInChildren<EnemyDamageArea>().DamageAreaCollider.enabled = false;
				ama.Amanecidas.AnimatorInyector.PlayDeath();
				this.ACT_WAIT.StartAction(this.owner, 1.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				Core.Logic.Penitent.Status.Invulnerable = false;
				base.FinishAction();
				Object.Destroy(ama.gameObject);
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class ShieldShockwave : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				Vector2 p = ama.transform.position + ama.centerBodyOffset;
				if (ama.HasSolidFloorBelow(false))
				{
					ama.SpikeWave(ama.transform.position, 1f, 14, true, 1.5f);
				}
				ama.ShakeWave(true);
				PoolManager.Instance.ReuseObject(ama.shieldShockwave, p, Quaternion.identity, false, 1);
				ama.Amanecidas.Audio.PlayShieldExplosion_AUDIO();
				this.ACT_WAIT.StartAction(this.owner, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class MoveUsingSpline_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour owner, Vector2 point, SplineThrowData throwData)
			{
				this.throwData = throwData;
				this.endPoint = point;
				return base.StartAction(owner);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.throwData.spline.transform.SetParent(this.oldParent);
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.splineFollower.followActivated = false;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				amanecidasBehaviour.DoSpinAnimation(false, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				int right = ama.GetDirFromOrientation();
				BezierSpline spline = this.throwData.spline;
				this.oldParent = spline.transform.parent;
				spline.transform.localScale = new Vector3(spline.transform.localScale.x * (float)right, Mathf.Sign(Random.Range(-1f, 1f)), 1f);
				spline.transform.SetParent(null, true);
				Vector2 controlPointOrigin = spline.points[1] - spline.points[0];
				Vector2 controlPointEnd = spline.points[2] - spline.points[3];
				Vector2 origin = spline.transform.InverseTransformPoint(ama.transform.position);
				this.endPoint = spline.transform.InverseTransformPoint(this.endPoint);
				spline.SetControlPoint(0, origin);
				spline.SetControlPoint(3, this.endPoint);
				spline.SetControlPoint(1, spline.points[0] + controlPointOrigin);
				spline.SetControlPoint(2, spline.points[3] + controlPointEnd);
				ama.DoSpinAnimation(true, false);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				ama.splineFollower.SetData(this.throwData);
				ama.splineFollower.StartFollowing(false);
				ama.SmallDistortion();
				this.ACT_WAIT.StartAction(this.owner, this.throwData.duration);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.DoSpinAnimation(false, false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				this.ACT_LOOK.StartAction(this.owner);
				yield return this.ACT_LOOK.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				ama.SmallDistortion();
				ama.splineFollower.followActivated = false;
				spline.transform.SetParent(this.oldParent, true);
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();

			private SplineThrowData throwData;

			private Transform oldParent;

			private Vector2 endPoint;
		}

		public class DodgeAndCounterAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour owner, float evadeDistance)
			{
				this.evadeDistance = evadeDistance;
				return base.StartAction(owner);
			}

			protected override void DoOnStop()
			{
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				int lookingRight = ama.GetDirFromOrientation();
				Vector2 dirToDodge = new Vector2(-Mathf.Sign(ama.GetDirToPenitent(ama.transform.position).x), 0f);
				Vector2 dodgePoint = this.owner.transform.position + dirToDodge * this.evadeDistance;
				if (!ama.battleBounds.Contains(dodgePoint))
				{
					dodgePoint = this.owner.transform.position - dirToDodge * this.evadeDistance;
				}
				ama.SpawnClone();
				ama.Amanecidas.Audio.PlayMoveFast_AUDIO();
				this.ACT_DODGE.StartAction(this.owner, dodgePoint, ama.dodgeSplineData);
				yield return this.ACT_DODGE.waitForCompletion;
				this.ACT_WAIT.StartAction(this.owner, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private float evadeDistance;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE = new MoveToPointUsingAgent_EnemyAction();

			private AmanecidasBehaviour.FalcataSlashProjectile_EnemyAction ACT_COUNTERACTION = new AmanecidasBehaviour.FalcataSlashProjectile_EnemyAction();

			private AmanecidasBehaviour.MoveUsingSpline_EnemyAction ACT_DODGE = new AmanecidasBehaviour.MoveUsingSpline_EnemyAction();
		}

		public class ShowAxes_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour amanecida = this.owner as AmanecidasBehaviour;
				amanecida.ShowBothAxes(true);
				amanecida.Amanecidas.AnimatorInyector.SetAmanecidaWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.HAND);
				this.ACT_WAIT.StartAction(this.owner, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class FuseAxes_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour amanecida = this.owner as AmanecidasBehaviour;
				amanecida.ShowBothAxes(false);
				amanecida.Amanecidas.AnimatorInyector.SetAmanecidaWeapon(AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE);
				this.ACT_WAIT.StartAction(this.owner, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class FollowSplineAndSpinAxesAround_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE_AXE1.StopAction();
				this.ACT_MOVE_AXE2.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_SPIN.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				Amanecidas amanecidas = amanecidasBehaviour.Amanecidas;
				amanecidas.Behaviour.SetInterruptable(false);
				this.splineFollower.followActivated = false;
			}

			public EnemyAction StartAction(EnemyBehaviour e, int _n, SplineThrowData mainSplineData, List<AmanecidaAxeBehaviour> axes, List<BezierSpline> axeSplines, List<SplineThrowData> throwData)
			{
				this.n = _n;
				this.mainSplineData = mainSplineData;
				this.axes = axes;
				this.axeSplines = axeSplines;
				this.throwData = throwData;
				this.splineFollower = e.GetComponent<SplineFollower>();
				this.splineFollower.SetData(mainSplineData);
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				Amanecidas amanecida = o.Amanecidas;
				this.ACT_BLINK.StartAction(this.owner, this.mainSplineData.spline.GetPoint(0f), 1f, true, true);
				yield return this.ACT_BLINK.waitForCompletion;
				if (o.IsWieldingAxe())
				{
					o.ShowBothAxes(true);
					o.ShowCurrentWeapon(false);
				}
				float secondsToArrive = 1.5f;
				this.axes[0].SetRepositionMode(true);
				this.axes[1].SetRepositionMode(true);
				this.axes[0].SetSeek(false);
				this.axes[1].SetSeek(false);
				this.ACT_MOVE_AXE1.StartAction(this.axes[0], this.axeSplines[0].GetPoint(0f), secondsToArrive, 7, null, true, null, true, true, 1.7f);
				this.ACT_MOVE_AXE2.StartAction(this.axes[1], this.axeSplines[1].GetPoint(0f), secondsToArrive, 7, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE_AXE1.waitForCompletion;
				yield return this.ACT_MOVE_AXE2.waitForCompletion;
				this.axes[0].SetRepositionMode(false);
				this.axes[1].SetRepositionMode(false);
				this.splineFollower.StartFollowing(true);
				this.ACT_SPIN.StartAction(this.owner, this.n, this.axes, this.axeSplines, this.throwData);
				int maxPossibleTurns = Mathf.FloorToInt(this.mainSplineData.duration / 0.7f);
				for (int i = 0; i < maxPossibleTurns; i++)
				{
					o.LookAtPenitent(false);
					this.ACT_WAIT.StartAction(this.owner, 0.7f);
					yield return this.ACT_WAIT.waitForCompletion;
					if (i == maxPossibleTurns - 1)
					{
						amanecida.Behaviour.SetInterruptable(true);
					}
				}
				yield return this.ACT_SPIN.waitForCompletion;
				amanecida.Behaviour.SetInterruptable(false);
				this.splineFollower.followActivated = false;
				base.FinishAction();
				yield break;
			}

			private int n;

			private List<AmanecidaAxeBehaviour> axes;

			private List<BezierSpline> axeSplines;

			private List<SplineThrowData> throwData;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_CHARACTER = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE1 = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE2 = new MoveEasing_EnemyAction();

			private AmanecidasBehaviour.SpinAxesAround_EnemyAction ACT_SPIN = new AmanecidasBehaviour.SpinAxesAround_EnemyAction();

			private AmanecidasBehaviour.ShowSimpleVFX_EnemyAction ACT_MAGNET_VFX = new AmanecidasBehaviour.ShowSimpleVFX_EnemyAction();

			private SplineFollower splineFollower;

			private SplineThrowData mainSplineData;
		}

		public class InterruptablePeriod_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour owner, float seconds)
			{
				this.seconds = seconds;
				return base.StartAction(owner);
			}

			protected override void DoOnStop()
			{
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.SetInterruptable(false);
				this.ACT_WAIT.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				ama.SetInterruptable(true);
				this.ACT_WAIT.StartAction(this.owner, this.seconds);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.SetInterruptable(false);
				base.FinishAction();
				yield break;
			}

			private float seconds;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class EnergyChargePeriod_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour owner, float seconds, bool playSfx)
			{
				this.seconds = seconds;
				this.playSfx = playSfx;
				return base.StartAction(owner);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetEnergyCharge(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				if (this.playSfx)
				{
					ama.Amanecidas.Audio.PlayEnergyCharge_AUDIO();
				}
				ama.Amanecidas.AnimatorInyector.SetEnergyCharge(true);
				this.ACT_WAIT.StartAction(this.owner, this.seconds);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetEnergyCharge(false);
				base.FinishAction();
				yield break;
			}

			private float seconds;

			private bool playSfx;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class TiredPeriod_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour owner, float seconds, bool interruptable)
			{
				this.seconds = seconds;
				this.interruptable = interruptable;
				return base.StartAction(owner);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_INTERRUPTABLE.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetTired(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				amanecidasBehaviour.LookAtPenitent(true);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				ama.Amanecidas.AnimatorInyector.SetTired(true);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				if (this.interruptable)
				{
					this.ACT_INTERRUPTABLE.StartAction(this.owner, this.seconds);
					yield return this.ACT_INTERRUPTABLE.waitForCompletion;
				}
				else
				{
					this.ACT_WAIT.StartAction(this.owner, this.seconds);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				ama.Amanecidas.AnimatorInyector.SetTired(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				base.FinishAction();
				yield break;
			}

			private float seconds;

			private bool interruptable;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.InterruptablePeriod_EnemyAction ACT_INTERRUPTABLE = new AmanecidasBehaviour.InterruptablePeriod_EnemyAction();
		}

		public class ShowSimpleVFX_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 _point, GameObject _vfxGO, float _seconds)
			{
				this.targetPoint = _point;
				this.seconds = _seconds;
				this.vfxGO = _vfxGO;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				GameObject newFX = PoolManager.Instance.ReuseObject(this.vfxGO, this.targetPoint, Quaternion.identity, false, 1).GameObject;
				SimpleVFX svfx = newFX.GetComponent<SimpleVFX>();
				if (svfx != null)
				{
					svfx.SetMaxTTL(this.seconds);
				}
				this.ACT_WAIT.StartAction(this.owner, this.seconds);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private Vector2 targetPoint;

			private float seconds;

			private GameObject vfxGO;
		}

		public class BlinkToPoint_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 _point, float _seconds, bool reappear = true, bool lookAtPenitent = false)
			{
				this.targetPoint = _point;
				this.seconds = _seconds;
				this.reappear = reappear;
				this.lookAtPenitent = lookAtPenitent;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				Amanecidas amanecidas = (this.owner as AmanecidasBehaviour).Amanecidas;
				if (this.reappear)
				{
					amanecidas.Behaviour.SetGhostTrail(true);
					amanecidas.AnimatorInyector.SetBlink(false);
				}
				if (this.lookAtPenitent)
				{
					amanecidas.Behaviour.LookAtPenitentUsingOrientation();
				}
			}

			protected override IEnumerator BaseCoroutine()
			{
				Amanecidas amanecida = (this.owner as AmanecidasBehaviour).Amanecidas;
				amanecida.AnimatorInyector.SetBlink(true);
				amanecida.Behaviour.SetGhostTrail(false);
				this.ACT_WAIT.StartAction(this.owner, this.baseBlinkoutSeconds + this.seconds);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_MOVE.StartAction(this.owner, this.targetPoint, 0.1f, 8, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				if (this.lookAtPenitent)
				{
					amanecida.Behaviour.LookAtPenitentUsingOrientation();
				}
				this.ACT_WAIT.StartAction(this.owner, 0.01f);
				yield return this.ACT_WAIT.waitForCompletion;
				if (this.reappear)
				{
					amanecida.Behaviour.SetGhostTrail(true);
					amanecida.AnimatorInyector.SetBlink(false);
					this.ACT_WAIT.StartAction(this.owner, this.baseBlinkinSeconds);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private Vector2 targetPoint;

			private float seconds;

			private bool reappear;

			private bool lookAtPenitent;

			private float baseBlinkoutSeconds = 0.55f;

			private float baseBlinkinSeconds = 0.4f;
		}

		public class SpinAxesAround_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE_AXE.StopAction();
				foreach (AmanecidaAxeBehaviour amanecidaAxeBehaviour in this.axes)
				{
					amanecidaAxeBehaviour.axeSplineFollowAction.StopAction();
				}
			}

			public EnemyAction StartAction(EnemyBehaviour e, int _n, List<AmanecidaAxeBehaviour> axes, List<BezierSpline> splines, List<SplineThrowData> throwData)
			{
				this.n = _n;
				this.axes = axes;
				this.splines = splines;
				this.throwData = throwData;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				Transform p = this.splines[0].transform.parent;
				float maxradius = 3f;
				for (int i = 0; i < this.n; i++)
				{
					for (int j = 0; j < this.axes.Count; j++)
					{
						AmanecidaAxeBehaviour amanecidaAxeBehaviour = this.axes[j];
						amanecidaAxeBehaviour.axeSplineFollowAction.StartAction(amanecidaAxeBehaviour, amanecidaAxeBehaviour.splineFollower, this.throwData[j], this.splines[j]);
					}
					Tween tw = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(p, maxradius, this.throwData[0].duration / 2f), 7);
					yield return TweenExtensions.WaitForCompletion(tw);
					tw = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOScale(p, 1f, this.throwData[0].duration / 2f), 7);
					yield return TweenExtensions.WaitForCompletion(tw);
				}
				base.FinishAction();
				yield break;
			}

			private int n;

			private List<AmanecidaAxeBehaviour> axes;

			private List<BezierSpline> splines;

			private List<SplineThrowData> throwData;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_CHARACTER = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE = new MoveEasing_EnemyAction();
		}

		public class FlyAndLaunchTwoAxes_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				foreach (AmanecidaAxeBehaviour amanecidaAxeBehaviour in this.axes)
				{
					amanecidaAxeBehaviour.axeSplineFollowAction.StopAction();
				}
				(this.owner as AmanecidasBehaviour).splineFollower.followActivated = false;
			}

			public EnemyAction StartAction(EnemyBehaviour e, int _n, Vector2 dir, List<AmanecidaAxeBehaviour> axes, List<BezierSpline> splines, List<SplineThrowData> throwData)
			{
				this.n = _n;
				this.axes = axes;
				this.dir = dir;
				this.splines = splines;
				this.throwData = throwData;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				this.ACT_MOVE.StartAction(this.owner, ama.agent, ama.battleBounds.center + Vector2.up * 3f, 2f);
				yield return this.ACT_MOVE.waitForCompletion;
				this.ACT_SHOW_AXES.StartAction(this.owner);
				yield return this.ACT_SHOW_AXES.waitForCompletion;
				this.ACT_WAIT.StartAction(this.owner, 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				SplineThrowData sp = ama.flightPattern;
				this.ACT_BLINK.StartAction(this.owner, sp.spline.points[0], 0.3f, true, true);
				yield return this.ACT_BLINK.waitForCompletion;
				ama.splineFollower.SetData(ama.flightPattern);
				ama.splineFollower.StartFollowing(true);
				for (int i = 0; i < this.n; i++)
				{
					AmanecidaAxeBehaviour axe = this.axes[0];
					Vector2 originPoint = axe.transform.position;
					Vector2 targetPoint = Core.Logic.Penitent.transform.position;
					axe.axeSplineFollowAction.StartAction(axe, axe.splineFollower, originPoint, targetPoint, 3, this.throwData[0], this.splines[0]);
					this.ACT_WAIT.StartAction(this.owner, this.throwData[0].duration / 2f);
					yield return this.ACT_WAIT.waitForCompletion;
					axe = this.axes[1];
					originPoint = axe.transform.position;
					targetPoint = Core.Logic.Penitent.transform.position;
					axe.axeSplineFollowAction.StartAction(axe, axe.splineFollower, originPoint, targetPoint, 3, this.throwData[1], this.splines[1]);
					this.ACT_WAIT.StartAction(this.owner, this.throwData[1].duration / 2f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				ama.splineFollower.followActivated = false;
				this.ACT_WAIT.StartAction(this.owner, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int n;

			private Vector2 dir;

			private List<AmanecidaAxeBehaviour> axes;

			private List<BezierSpline> splines;

			private List<SplineThrowData> throwData;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE = new MoveToPointUsingAgent_EnemyAction();

			private AmanecidasBehaviour.ShowAxes_EnemyAction ACT_SHOW_AXES = new AmanecidasBehaviour.ShowAxes_EnemyAction();

			private AmanecidasBehaviour.ShowSimpleVFX_EnemyAction ACT_MAGNET_VFX = new AmanecidasBehaviour.ShowSimpleVFX_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();
		}

		public class LaunchTwoAxesHorizontal_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_MOVE_AGENT.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE_EASING_1.StopAction();
				this.ACT_MOVE_EASING_2.StopAction();
				foreach (AmanecidaAxeBehaviour amanecidaAxeBehaviour in this.axes)
				{
					amanecidaAxeBehaviour.axeSplineFollowAction.StopAction();
				}
			}

			public EnemyAction StartAction(EnemyBehaviour e, int _n, Vector2 point, float distance, int dir, List<AmanecidaAxeBehaviour> axes, List<BezierSpline> splines, List<SplineThrowData> throwData)
			{
				this.n = _n;
				this.axes = axes;
				this.dir = dir;
				this.point = point;
				this.splines = splines;
				this.distance = distance;
				this.throwData = throwData;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				if (o.IsWieldingAxe())
				{
					o.ShowBothAxes(true);
					o.ShowCurrentWeapon(false);
				}
				this.axes[0].SetRepositionMode(true);
				this.axes[1].SetRepositionMode(true);
				this.ACT_MOVE_AGENT.StartAction(this.owner, this.owner.GetComponent<AutonomousAgent>(), this.point + Vector2.up, 2f);
				yield return this.ACT_MOVE_AGENT.waitForCompletion;
				AmanecidasBehaviour ab = this.owner as AmanecidasBehaviour;
				ab.LookAtPenitent(false);
				Vector2 originAxePoint = this.owner.transform.position + new Vector2(ab.axeOffset.x * (float)this.dir, ab.axeOffset.y);
				float secondsToArrive = 0.75f;
				this.axes[0].SetSeek(false);
				this.axes[1].SetSeek(false);
				this.ACT_MOVE_EASING_1.StartAction(this.axes[0], originAxePoint, secondsToArrive, 7, null, true, null, true, true, 1.7f);
				originAxePoint += Vector2.down * 1.5f;
				this.ACT_MOVE_EASING_2.StartAction(this.axes[1], originAxePoint, secondsToArrive, 7, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE_EASING_1.waitForCompletion;
				yield return this.ACT_MOVE_EASING_2.waitForCompletion;
				this.axes[0].SetRepositionMode(false);
				this.axes[1].SetRepositionMode(false);
				this.ACT_WAIT.StartAction(this.owner, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				for (int i = 0; i < this.n; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						AmanecidaAxeBehaviour axe = this.axes[j];
						Vector2 originPoint = axe.transform.position;
						Vector2 targetPoint = originPoint + Vector2.right * this.distance * (float)this.dir;
						o.Amanecidas.AnimatorInyector.PlayStompAttack(true);
						o.Amanecidas.AnimatorInyector.SetMeleeHold(true);
						o.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
						o.Amanecidas.Audio.PlayAxeThrow_AUDIO();
						axe.axeSplineFollowAction.StartAction(axe, axe.splineFollower, originPoint, targetPoint, 3, this.throwData[j], this.splines[j]);
						this.ACT_WAIT.StartAction(this.owner, 0.5f);
						yield return this.ACT_WAIT.waitForCompletion;
						o.Amanecidas.AnimatorInyector.SetMeleeHold(false);
						yield return new AmanecidasBehaviour.WaitUntilIdle(o, 5f);
						o.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
					}
					this.ACT_WAIT.StartAction(this.owner, this.throwData[0].duration);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				o.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int n;

			private int dir;

			private float distance;

			private Vector2 point;

			private List<AmanecidaAxeBehaviour> axes;

			private List<BezierSpline> splines;

			private List<SplineThrowData> throwData;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_EASING_1 = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_EASING_2 = new MoveEasing_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_AGENT = new MoveToPointUsingAgent_EnemyAction();

			private AmanecidasBehaviour.ShowSimpleVFX_EnemyAction ACT_MAGNET_VFX = new AmanecidasBehaviour.ShowSimpleVFX_EnemyAction();
		}

		public class JumpSmash_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_JUMP.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_CALLAXE1.StopAction();
				this.ACT_CALLAXE2.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_TIRED.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, Vector2 jumpOrigin, Action jumpMethod, bool getTiredAtTheEnd)
			{
				this.jumpOrigin = jumpOrigin;
				this.jumpMethod = jumpMethod;
				this.getTiredAtTheEnd = getTiredAtTheEnd;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				this.ACT_BLINK.StartAction(this.owner, this.jumpOrigin, 0.3f, true, true);
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				Vector2 dir = (this.jumpOrigin.x <= ama.battleBounds.center.x) ? Vector2.right : Vector2.left;
				Vector2 p = this.jumpOrigin + Vector2.up - dir * 0.75f;
				float seconds = 0.3f;
				if (!ama.IsWieldingAxe())
				{
					ama.axes[0].SetSeek(false);
					ama.axes[1].SetSeek(false);
					this.ACT_CALLAXE1.StartAction(this.owner, p, ama.axes[0], seconds, seconds);
					this.ACT_CALLAXE2.StartAction(this.owner, p, ama.axes[1], seconds, seconds);
					yield return this.ACT_CALLAXE1.waitForCompletion;
				}
				yield return this.ACT_BLINK.waitForCompletion;
				ama.ShowBothAxes(false);
				ama.ShowCurrentWeapon(true);
				ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(true);
				ama.Amanecidas.AnimatorInyector.PlayMeleeAttack();
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				ama.Amanecidas.Audio.PlayAxeSmashPreattack_AUDIO();
				this.ACT_MOVE.StartAction(this.owner, this.owner.transform.position - dir, 0.5f, 27, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				this.ACT_JUMP.StartAction(this.owner, this.jumpMethod);
				this.ACT_WAIT.StartAction(this.owner, 0.6f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				ama.Amanecidas.Audio.PlayAxeSmash_AUDIO();
				this.ACT_WAIT.StartAction(this.owner, 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				if (this.getTiredAtTheEnd)
				{
					this.ACT_TIRED.StartAction(this.owner, 3f, true);
					yield return this.ACT_TIRED.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private Action jumpMethod;

			private Vector2 jumpOrigin;

			private bool getTiredAtTheEnd;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private LaunchMethod_EnemyAction ACT_JUMP = new LaunchMethod_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE1 = new AmanecidasBehaviour.CallAxe_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE2 = new AmanecidasBehaviour.CallAxe_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private AmanecidasBehaviour.TiredPeriod_EnemyAction ACT_TIRED = new AmanecidasBehaviour.TiredPeriod_EnemyAction();
		}

		public class JumpBackAndShoot_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_WAIT.StopAction();
				this.ACT_JUMP.StopAction();
				this.ACT_SHOOT.StopAction();
			}

			public EnemyAction StartAction(EnemyBehaviour e, int _n, Action _jumpMethod, Action _shootMethod)
			{
				this.n = _n;
				this.shootMethod = _shootMethod;
				this.jumpMethod = _jumpMethod;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
				LaunchMethod_EnemyAction ACT_JUMP = new LaunchMethod_EnemyAction();
				LaunchMethod_EnemyAction ACT_SHOOT = new LaunchMethod_EnemyAction();
				ACT_JUMP.StartAction(this.owner, this.jumpMethod);
				for (int i = 0; i < this.n; i++)
				{
					ACT_WAIT.StartAction(this.owner, 0.2f, 0.2f);
					yield return ACT_WAIT.waitForCompletion;
					ACT_SHOOT.StartAction(this.owner, this.shootMethod);
				}
				ACT_WAIT.StartAction(this.owner, 0.6f, 0.6f);
				yield return ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int n;

			private float distance;

			private float seconds;

			private Action jumpMethod;

			private Action shootMethod;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private LaunchMethod_EnemyAction ACT_JUMP = new LaunchMethod_EnemyAction();

			private LaunchMethod_EnemyAction ACT_SHOOT = new LaunchMethod_EnemyAction();
		}

		public class ShowDashAnticipationTrail_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_WAIT.StopAction();
			}

			public EnemyAction StartAction(EnemyBehaviour e, Vector2 startPoint, Vector2 endPoint, float duration)
			{
				this.startPoint = startPoint;
				this.endPoint = endPoint;
				this.duration = duration;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner.GetComponent<AmanecidasBehaviour>();
				GameObject trailObject = ama.trailGameObject;
				trailObject.transform.position = this.startPoint;
				ParticleSystem particles = trailObject.GetComponent<ParticleSystem>();
				trailObject.transform.right = this.endPoint - this.startPoint;
				particles.Play();
				this.ACT_WAIT.StartAction(this.owner, this.duration);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private Vector2 startPoint;

			private Vector2 endPoint;

			private float duration;
		}

		public class CallAxe_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_MAGNET_VFX.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE_AXE_X.StopAction();
				this.ACT_MOVE_AXE_Y.StopAction();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, Vector2 pointA, AmanecidaAxeBehaviour axe, float secondsBeforeAttraction, float secondsToArrive)
			{
				this.pointA = pointA;
				this.axe = axe;
				this.secondsBeforeAttraction = secondsBeforeAttraction;
				this.secondsToArrive = secondsToArrive;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ab = this.owner.GetComponent<AmanecidasBehaviour>();
				this.ACT_WAIT.StartAction(this.owner, this.secondsBeforeAttraction);
				yield return this.ACT_WAIT.waitForCompletion;
				this.axe.SetRepositionMode(true);
				if (this.pointA.y > ab.battleBounds.center.y)
				{
					this.ACT_MOVE_AXE_X.StartAction(this.axe, this.pointA, 0.6f, 26, null, true, null, true, false, 1.7f);
					this.ACT_MOVE_AXE_Y.StartAction(this.axe, this.pointA, 0.4f, 27, null, true, null, false, true, 1.7f);
				}
				else
				{
					this.ACT_MOVE_AXE_X.StartAction(this.axe, this.pointA, 0.6f, 27, null, true, null, true, false, 1.7f);
					this.ACT_MOVE_AXE_Y.StartAction(this.axe, this.pointA, 0.4f, 26, null, true, null, false, true, 1.7f);
				}
				yield return this.ACT_MOVE_AXE_Y.waitForCompletion;
				yield return this.ACT_MOVE_AXE_X.waitForCompletion;
				this.axe.SetRepositionMode(false);
				base.FinishAction();
				yield break;
			}

			private Vector2 pointA;

			private AmanecidaAxeBehaviour axe;

			private float secondsBeforeAttraction;

			private float secondsToArrive;

			private AmanecidasBehaviour.ShowSimpleVFX_EnemyAction ACT_MAGNET_VFX = new AmanecidasBehaviour.ShowSimpleVFX_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE_X = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE_Y = new MoveEasing_EnemyAction();
		}

		public class LaunchAxesToPenitent_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE_CHARACTER.StopAction();
				this.ACT_MOVE_AXE_X.StopAction();
				this.ACT_MOVE_AXE_Y.StopAction();
				this.ACT_CALLAXE1.StopAction();
				this.ACT_CALLAXE2.StopAction();
				this.ACT_MOVE_AXE_FULL.StopAction();
				this.ACT_KEEPDISTANCE.StopAction();
				this.ACT_KEEPDISTANCE_AXE.StopAction();
				this.ACT_LOOK.StopAction();
				AmanecidasBehaviour component = this.owner.GetComponent<AmanecidasBehaviour>();
				component.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				component.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, AmanecidaAxeBehaviour firstAxe, AmanecidaAxeBehaviour secondAxe, float minAnticipationSeconds, int numThrows)
			{
				this.firstAxe = firstAxe;
				this.secondAxe = secondAxe;
				this.minAnticipationSeconds = minAnticipationSeconds;
				this.numThrows = numThrows;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner.GetComponent<AmanecidasBehaviour>();
				Vector2 startingPos = new Vector2(ama.transform.position.x, ama.battleBounds.yMax - 1.5f);
				this.ACT_MOVE_CHARACTER.StartAction(this.owner, ama.agent, startingPos, 2f);
				yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
				for (int i = 0; i < this.numThrows; i++)
				{
					bool usingFirstAxe = i % 2 == 0;
					AmanecidaAxeBehaviour currentAxe = (!usingFirstAxe) ? this.secondAxe : this.firstAxe;
					int dir = ama.GetDirFromOrientation();
					Vector2 axeStartingPos = ama.transform.position + new Vector2((float)dir, 1.5f);
					if (i > 1)
					{
						ama.verticalFastBlastAxeAttack.SummonAreaOnPoint(currentAxe.transform.position, 0f, 1f, null);
						this.ACT_WAIT.StartAction(this.owner, 0.3f);
						yield return this.ACT_WAIT.waitForCompletion;
						this.ACT_MOVE_AXE_X.StartAction(currentAxe, axeStartingPos, 0.6f, 26, null, true, null, true, false, 1.7f);
						this.ACT_MOVE_AXE_Y.StartAction(currentAxe, axeStartingPos, 0.4f, 27, null, true, null, false, true, 1.7f);
						yield return this.ACT_MOVE_AXE_Y.waitForCompletion;
						yield return this.ACT_MOVE_AXE_X.waitForCompletion;
					}
					ama.ShowAxe(false, usingFirstAxe);
					ama.ShowCurrentWeapon(true);
					this.ACT_LOOK.StartAction(this.owner);
					yield return this.ACT_LOOK.waitForCompletion;
					ama.LookAtPenitent(true);
					ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(true);
					ama.Amanecidas.AnimatorInyector.PlayMeleeAttack();
					ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
					float anticipationSeconds = this.minAnticipationSeconds + Mathf.Lerp(0.5f, 1f, ama.GetDirToPenitent(ama.transform.position).x / ama.battleBounds.width);
					this.ACT_KEEPDISTANCE.StartAction(this.owner, anticipationSeconds, true, false, (float)(-(float)ama.GetDirFromOrientation()), 0f);
					this.ACT_WAIT.StartAction(this.owner, anticipationSeconds * 0.9f);
					yield return this.ACT_WAIT.waitForCompletion;
					ama.Amanecidas.Audio.PlayAxeThrowPreattack_AUDIO();
					yield return this.ACT_KEEPDISTANCE.waitForCompletion;
					ama.Amanecidas.Audio.PlayAxeThrow_AUDIO();
					ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
					this.ACT_WAIT.StartAction(this.owner, 0.1f);
					yield return this.ACT_WAIT.waitForCompletion;
					dir = ama.GetDirFromOrientation();
					axeStartingPos = ama.transform.position + new Vector2((float)dir, 1.2f);
					ama.ShowCurrentWeapon(false);
					ama.ShowAxe(false, !usingFirstAxe);
					ama.ShowAxe(true, usingFirstAxe, true, axeStartingPos);
					ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
					dir = ama.GetDirFromOrientation();
					Vector2 targetPos = new Vector2(ama.transform.position.x + (float)ama.GetDirFromOrientation() * 1.5f, ama.battleBounds.yMin);
					float travelTime = 0.3f;
					yield return TweenExtensions.WaitForCompletion(ShortcutExtensions.DOPunchPosition(currentAxe.transform, Vector2.up - Vector2.right * (float)dir * 0.2f, 0.1f, 2, 0.1f, false));
					this.ACT_MOVE_AXE_FULL.StartAction(currentAxe, targetPos, travelTime, 6, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE_AXE_FULL.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, travelTime * 0.7f);
					yield return this.ACT_WAIT.waitForCompletion;
					ama.verticalNormalBlastAxeAttack.SummonAreas(currentAxe.transform.position, Vector3.right, EntityOrientation.Right);
					ama.verticalNormalBlastAxeAttack.SummonAreas(currentAxe.transform.position, Vector3.left, EntityOrientation.Left);
					yield return this.ACT_MOVE_AXE_FULL.waitForCompletion;
				}
				Vector2 endingPos = ama.transform.position + Vector2.up * 1.5f;
				this.ACT_CALLAXE1.StartAction(this.owner, endingPos + Vector2.right * (float)ama.GetDirFromOrientation(), this.firstAxe, 0.2f, 0.5f);
				this.ACT_CALLAXE2.StartAction(this.owner, endingPos - Vector2.right * (float)ama.GetDirFromOrientation(), this.secondAxe, 0.2f, 0.5f);
				yield return this.ACT_CALLAXE1.waitForCompletion;
				yield return this.ACT_CALLAXE2.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private AmanecidaAxeBehaviour firstAxe;

			private AmanecidaAxeBehaviour secondAxe;

			private float minAnticipationSeconds;

			private int numThrows;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE_X = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE_Y = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE_FULL = new MoveEasing_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE1 = new AmanecidasBehaviour.CallAxe_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE2 = new AmanecidasBehaviour.CallAxe_EnemyAction();

			private AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction ACT_KEEPDISTANCE = new AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction();

			private AmanecidasBehaviour.KeepDistanceFromAmanecidaUsingAgent_EnemyAction ACT_KEEPDISTANCE_AXE = new AmanecidasBehaviour.KeepDistanceFromAmanecidaUsingAgent_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();
		}

		public class LaunchCrawlerAxesToPenitent_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE_CHARACTER.StopAction();
				this.ACT_MOVE_AXE_X.StopAction();
				this.ACT_MOVE_AXE_Y.StopAction();
				this.ACT_CALLAXE1.StopAction();
				this.ACT_CALLAXE2.StopAction();
				this.ACT_MOVE_AXE_FULL.StopAction();
				this.ACT_KEEPDISTANCE.StopAction();
				this.ACT_KEEPDISTANCE_AXE.StopAction();
				this.ACT_LOOK.StopAction();
				AmanecidasBehaviour component = this.owner.GetComponent<AmanecidasBehaviour>();
				component.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				component.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, AmanecidaAxeBehaviour firstAxe, AmanecidaAxeBehaviour secondAxe, float minAnticipationSeconds, int numThrows)
			{
				this.firstAxe = firstAxe;
				this.secondAxe = secondAxe;
				this.minAnticipationSeconds = minAnticipationSeconds;
				this.numThrows = numThrows;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner.GetComponent<AmanecidasBehaviour>();
				Vector2 startingPos = new Vector2(ama.transform.position.x, ama.battleBounds.yMax - 2f);
				this.ACT_MOVE_CHARACTER.StartAction(this.owner, ama.agent, startingPos, 2f);
				yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
				for (int i = 0; i < this.numThrows; i++)
				{
					bool usingFirstAxe = i % 2 == 0;
					AmanecidaAxeBehaviour currentAxe = (!usingFirstAxe) ? this.secondAxe : this.firstAxe;
					ama.ShowAxe(false, usingFirstAxe);
					if (!ama.IsWieldingAxe())
					{
						ama.DoSummonWeaponAnimation();
						this.ACT_WAIT.StartAction(this.owner, 0.4f);
						yield return this.ACT_WAIT.waitForCompletion;
					}
					this.ACT_LOOK.StartAction(this.owner);
					yield return this.ACT_LOOK.waitForCompletion;
					ama.LookAtPenitent(true);
					ama.Amanecidas.Audio.PlayDashCharge_AUDIO();
					ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(true);
					ama.Amanecidas.AnimatorInyector.PlayMeleeAttack();
					ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
					float anticipationSeconds = this.minAnticipationSeconds + Mathf.Lerp(0.5f, 1f, ama.GetDirToPenitent(ama.transform.position).x / ama.battleBounds.width);
					this.ACT_KEEPDISTANCE.StartAction(this.owner, anticipationSeconds, true, false, (float)(-(float)ama.GetDirFromOrientation()), 0f);
					this.ACT_WAIT.StartAction(this.owner, anticipationSeconds * 0.95f);
					yield return this.ACT_WAIT.waitForCompletion;
					ama.Amanecidas.Audio.PlayAxeThrowPreattack_AUDIO();
					yield return this.ACT_KEEPDISTANCE.waitForCompletion;
					ama.Amanecidas.Audio.StopDashCharge_AUDIO();
					ama.Amanecidas.Audio.PlayAxeThrow_AUDIO();
					ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
					this.ACT_WAIT.StartAction(this.owner, 0.1f);
					yield return this.ACT_WAIT.waitForCompletion;
					int dir = ama.GetDirFromOrientation();
					Vector2 axeStartingPos = ama.transform.position + new Vector2((float)dir, 1.2f);
					ama.ShowCurrentWeapon(false);
					ama.ShowAxe(true, usingFirstAxe, true, axeStartingPos);
					if (i < this.numThrows - 1)
					{
						ama.ShowAxe(false, !usingFirstAxe);
					}
					ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
					dir = ama.GetDirFromOrientation();
					Vector2 targetPos = new Vector2(ama.transform.position.x + (float)ama.GetDirFromOrientation() * 1.5f, ama.battleBounds.yMin);
					float travelTime = 0.2f;
					this.ACT_MOVE_AXE_FULL.StartAction(currentAxe, targetPos, travelTime, 2, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE_AXE_FULL.waitForCompletion;
					Vector2 exitDir = (ama.GetDirToPenitent(targetPos).x <= 0f) ? Vector2.left : Vector2.right;
					int num = (exitDir.x <= 0f) ? 1 : 0;
					ama.shockwave.SummonAreaOnPoint(targetPos, 0f, 1f, null);
					ama.ShakeWave(false);
					this.ACT_WAIT.StartAction(this.owner, 0.1f);
					yield return this.ACT_WAIT.waitForCompletion;
					Vector2 outOfCameraPos = exitDir * ama.battleBounds.width * 2f;
					outOfCameraPos.y = currentAxe.transform.position.y;
					this.ACT_MOVE_AXE_FULL.StartAction(currentAxe, outOfCameraPos, 2f, 26, null, true, null, true, true, 1.4f);
					this.ACT_WAIT.StartAction(this.owner, 1f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				yield return this.ACT_MOVE_AXE_FULL.waitForCompletion;
				Vector2 endingPos = ama.transform.position + Vector2.up * 1.5f;
				this.ACT_CALLAXE1.StartAction(this.owner, endingPos + Vector2.right * (float)ama.GetDirFromOrientation(), this.firstAxe, 0.2f, 0.5f);
				this.ACT_CALLAXE2.StartAction(this.owner, endingPos - Vector2.right * (float)ama.GetDirFromOrientation(), this.secondAxe, 0.2f, 0.5f);
				yield return this.ACT_CALLAXE1.waitForCompletion;
				yield return this.ACT_CALLAXE2.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private AmanecidaAxeBehaviour firstAxe;

			private AmanecidaAxeBehaviour secondAxe;

			private float minAnticipationSeconds;

			private int numThrows;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE_X = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE_Y = new MoveEasing_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_AXE_FULL = new MoveEasing_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE1 = new AmanecidasBehaviour.CallAxe_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE2 = new AmanecidasBehaviour.CallAxe_EnemyAction();

			private AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction ACT_KEEPDISTANCE = new AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction();

			private AmanecidasBehaviour.KeepDistanceFromAmanecidaUsingAgent_EnemyAction ACT_KEEPDISTANCE_AXE = new AmanecidasBehaviour.KeepDistanceFromAmanecidaUsingAgent_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();
		}

		public class LaunchBallsToPenitent_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_KEEPDISTANCE.StopAction();
				this.ACT_LOOK.StopAction();
				AmanecidasBehaviour component = this.owner.GetComponent<AmanecidasBehaviour>();
				component.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				component.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, float anticipationSeconds, int numThrows, bool keepDistance)
			{
				this.anticipationSeconds = anticipationSeconds;
				this.numThrows = numThrows;
				this.keepDistance = keepDistance;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner.GetComponent<AmanecidasBehaviour>();
				Penitent p = Core.Logic.Penitent;
				ama.ShowBothAxes(false);
				ama.ShowCurrentWeapon(false);
				if (this.keepDistance)
				{
					Vector3 throwingPos = ama.transform.position;
					throwingPos.y = ama.battleBounds.yMin + 1f;
					throwingPos.x = ((p.GetPosition().x <= throwingPos.x) ? (ama.battleBounds.xMax - 2f) : (ama.battleBounds.xMin + 2f));
					this.ACT_MOVE.StartAction(this.owner, throwingPos, 0.1f, 7, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				for (int i = 0; i < this.numThrows; i++)
				{
					if (this.keepDistance)
					{
						this.ACT_LOOK.StartAction(this.owner);
						yield return this.ACT_LOOK.waitForCompletion;
					}
					ama.LookAtPenitent(true);
					ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(true);
					ama.Amanecidas.AnimatorInyector.PlayMeleeAttack();
					ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
					ama.PlayChargeEnergy(this.anticipationSeconds, false, true);
					if (this.keepDistance)
					{
						this.ACT_KEEPDISTANCE.StartAction(this.owner, this.anticipationSeconds, true, false, true, ama.battleBounds.xMin, ama.battleBounds.xMax, false, 0f, 0f);
						yield return this.ACT_KEEPDISTANCE.waitForCompletion;
						this.ACT_WAIT.StartAction(this.owner, 0.1f);
						yield return this.ACT_WAIT.waitForCompletion;
					}
					else
					{
						this.ACT_WAIT.StartAction(this.owner, this.anticipationSeconds);
						yield return this.ACT_WAIT.waitForCompletion;
					}
					ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
					p.GetPosition().y = ama.battleBounds.yMin;
					ama.lavaBallAttack.Shoot(ama.GetDirToPenitent(ama.transform.position).normalized, Vector2.zero, Vector2.zero, 1f);
					ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
					this.ACT_WAIT.StartAction(this.owner, 0.5f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private float anticipationSeconds;

			private int numThrows;

			private bool keepDistance;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction ACT_KEEPDISTANCE = new AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();
		}

		public class ChangeWeapon_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_RECHARGE.StopAction();
				this.ACT_MOVE_CHARACTER.StopAction();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, float waitSeconds)
			{
				this.waitSeconds = waitSeconds;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				this.ACT_RECHARGE.StartAction(this.owner, (float)o.currentFightParameters.shieldRechargeTime, (float)o.currentFightParameters.shieldShockwaveAnticipationTime, new Action(o.DoRechargeShield), new Action(o.DoAnticipateShockwave), new Action(o.DoShieldShockwave));
				this.ACT_WAIT.StartAction(this.owner, 2f);
				yield return this.ACT_WAIT.waitForCompletion;
				AmanecidasAnimatorInyector.AMANECIDA_WEAPON nextWeapon = o.currentFightParameters.weapon;
				o.SetWeapon(nextWeapon);
				o.SummonWeapon();
				yield return this.ACT_RECHARGE.waitForCompletion;
				o.currentMeleeAttack = o.meleeStompAttack;
				o.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				o.Amanecidas.AnimatorInyector.PlayStompAttack(false);
				o.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				o.Amanecidas.SetNextLaudesArena(null);
				this.ACT_MOVE_CHARACTER.StartAction(this.owner, o.agent, new Vector2(o.transform.position.x, o.battleBounds.yMax), 2f);
				this.ACT_WAIT.StartAction(this.owner, this.waitSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
				o.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(o, 5f);
				o.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				o.needsToSwapWeapon = false;
				base.FinishAction();
				yield break;
			}

			private float waitSeconds;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.RechargeShield_EnemyAction ACT_RECHARGE = new AmanecidasBehaviour.RechargeShield_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();
		}

		public class FalcataSlashBarrage_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int n, bool startsFromRight, float distance, float anticipationSeconds)
			{
				this.n = n;
				this.startsFromRight = startsFromRight;
				this.distance = distance;
				this.anticipationSeconds = anticipationSeconds;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_SLASH.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_WAIT.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				Vector2 targetPos = Core.Logic.Penitent.GetPosition();
				int dir = (!this.startsFromRight) ? -1 : 1;
				Vector2 p = targetPos + Vector2.right * this.distance * (float)dir;
				p.x = Mathf.Clamp(p.x, ama.battleBounds.xMin, ama.battleBounds.xMax);
				p.y = ama.battleBounds.yMax;
				float seconds = 0.1f;
				float tooCloseDistance = 5f;
				if (ama.GetDirToPenitent().magnitude < tooCloseDistance || ama.GetDirToPenitent().y > 0f)
				{
					this.ACT_BLINK.StartAction(this.owner, p, seconds, true, true);
					yield return this.ACT_BLINK.waitForCompletion;
				}
				else
				{
					this.ACT_LOOK.StartAction(this.owner);
					yield return this.ACT_LOOK.waitForCompletion;
				}
				Vector2 directionVector = ama.GetDirToPenitent(ama.transform.position);
				float maxAngle = 90f;
				float initialAngle = -45f;
				directionVector = Quaternion.Euler(0f, 0f, -initialAngle) * directionVector;
				Quaternion stepRotation = Quaternion.Euler(0f, 0f, -maxAngle / (float)this.n);
				List<Vector2> directions = new List<Vector2>();
				for (int i = 0; i < this.n; i++)
				{
					directions.Add(directionVector);
					directionVector = stepRotation * directionVector;
				}
				this.ACT_SLASH.StartAction(this.owner, directions, this.anticipationSeconds);
				yield return this.ACT_SLASH.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.FalcataSlashProjectile_EnemyAction ACT_SLASH = new AmanecidasBehaviour.FalcataSlashProjectile_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();

			private bool startsFromRight;

			private float distance;

			private int n;

			private float anticipationSeconds;
		}

		public class ChainDash_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int n)
			{
				this.n = n;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_BLINKDASH.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE_CHARACTER.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				int dir = ama.GetDirFromOrientation();
				this.ACT_MOVE_CHARACTER.StartAction(this.owner, ama.agent, this.owner.transform.position + new Vector2((float)(-(float)dir) * 0.3f, 0.3f), 2f);
				this.ACT_WAIT.StartAction(this.owner, 0.4f);
				yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
				yield return this.ACT_WAIT.waitForCompletion;
				ama.throwbackExtraTime = 0f;
				for (int i = 0; i < this.n; i++)
				{
					if (ama.throwbackExtraTime > 0f)
					{
						this.ACT_WAIT.StartAction(this.owner, ama.throwbackExtraTime);
						yield return this.ACT_WAIT.waitForCompletion;
					}
					ama.SmallDistortion();
					ama.throwbackExtraTime = 0f;
					this.ACT_BLINKDASH.StartAction(this.owner, this.owner.transform.position, ama.slashDashAttack, 0.5f, true, true, true, 20f);
					yield return this.ACT_BLINKDASH.waitForCompletion;
					ama.ClearRotationAndFlip();
					float distance = 5f;
					dir = ama.GetDirFromOrientation();
					ama.transform.position = ama.GetPointBelowPenitent(true) + (float)dir * Vector2.right * distance;
				}
				float clampedX = Mathf.Clamp(ama.transform.position.x, ama.battleBounds.xMin, ama.battleBounds.xMax);
				ama.transform.position = new Vector2(clampedX, ama.transform.position.y);
				ama.LookAtPenitentUsingOrientation();
				ama.Amanecidas.AnimatorInyector.SetBlink(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				base.FinishAction();
				yield break;
			}

			private int n;

			private AmanecidasBehaviour.BlinkAndDashToPenitent_EnemyAction ACT_BLINKDASH = new AmanecidasBehaviour.BlinkAndDashToPenitent_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();
		}

		public class FalcataSlashProjectile_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 dir, float meleeAnticipation)
			{
				return this.StartAction(e, new List<Vector2>
				{
					dir
				}, meleeAnticipation);
			}

			public EnemyAction StartAction(EnemyBehaviour e, List<Vector2> dirs, float meleeAnticipation)
			{
				this.dirs = dirs;
				this.meleeAnticipationSeconds = meleeAnticipation;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_LOOK.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				this.ACT_LOOK.StartAction(this.owner);
				yield return this.ACT_LOOK.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.PlayMeleeAttack();
				ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(true);
				this.ACT_WAIT.StartAction(this.owner, this.meleeAnticipationSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				this.ACT_WAIT.StartAction(this.owner, 0.25f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.Audio.PlaySwordAttack_AUDIO();
				ama.Amanecidas.Audio.PlaySwordFirePreattack_AUDIO();
				foreach (Vector2 dir in this.dirs)
				{
					ama.falcataSlashProjectileAttack.Shoot(dir, (float)ama.GetDirFromOrientation() * Vector2.right, 1f);
				}
				this.ACT_WAIT.StartAction(this.owner, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_WAIT.StartAction(this.owner, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				base.FinishAction();
				yield break;
			}

			private List<Vector2> dirs;

			private float meleeAnticipationSeconds;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();
		}

		public class MoveAroundAndAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int _n, float _distance, float _seconds, Action _method)
			{
				this.n = _n;
				this.distance = _distance;
				this.seconds = _seconds;
				this.method = _method;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_METHOD.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				this.ACT_MOVE.StartAction(this.owner, this.owner.transform.position + Vector2.up * 6f, 2f, 21, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				for (int i = 0; i < this.n; i++)
				{
					this.ACT_MOVE.StartAction(this.owner, this.distance, this.seconds, 21, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, 0.5f, 0.5f);
					yield return this.ACT_WAIT.waitForCompletion;
					this.ACT_METHOD.StartAction(this.owner, this.method);
					yield return this.ACT_METHOD.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, 0.2f, 0.2f);
					yield return this.ACT_WAIT.waitForCompletion;
					this.ACT_METHOD.StartAction(this.owner, this.method);
					yield return this.ACT_METHOD.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, 0.2f, 0.2f);
					yield return this.ACT_WAIT.waitForCompletion;
					this.ACT_METHOD.StartAction(this.owner, this.method);
					yield return this.ACT_METHOD.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, 0.5f, 1.5f);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private int n;

			private float distance;

			private float seconds;

			private Action method;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private LaunchMethod_EnemyAction ACT_METHOD = new LaunchMethod_EnemyAction();
		}

		public class StompAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_JUMP.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_LOOK.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				amanecidasBehaviour.currentMeleeAttack.damageOnEnterArea = false;
				amanecidasBehaviour.currentMeleeAttack = this.previousMeleeAttack;
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, bool doBlinkBeforeJump, bool doStompDamage, bool usePillars, bool bounceAfterJump, float bounceHeight = 1f)
			{
				this.doBlinkBeforeJump = doBlinkBeforeJump;
				this.doStompDamage = doStompDamage;
				this.usePillars = usePillars;
				this.bounceAfterJump = bounceAfterJump;
				this.bounceHeight = bounceHeight;
				return base.StartAction(e);
			}

			public EnemyAction StartAction(EnemyBehaviour e, Vector2 targetPoint, bool doBlinkBeforeJump, bool doStompDamage, bool usePillars, bool bounceAfterJump, float bounceHeight = 1f)
			{
				this.targetPoint = targetPoint;
				this.doBlinkBeforeJump = doBlinkBeforeJump;
				this.doStompDamage = doStompDamage;
				this.usePillars = usePillars;
				this.bounceAfterJump = bounceAfterJump;
				this.bounceHeight = bounceHeight;
				return base.StartAction(e);
			}

			public EnemyAction StartAction(EnemyBehaviour e, Action jumpMethod, bool doBlinkBeforeJump, bool doStompDamage, bool bounceAfterJump, float bounceHeight = 1f)
			{
				this.jumpMethod = jumpMethod;
				this.doBlinkBeforeJump = doBlinkBeforeJump;
				this.doStompDamage = doStompDamage;
				this.bounceAfterJump = bounceAfterJump;
				this.bounceHeight = bounceHeight;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				this.previousMeleeAttack = ama.currentMeleeAttack;
				if (this.doBlinkBeforeJump)
				{
					Vector2 target = ama.GetPointBelowPenitent(false);
					Vector2 point = target + Vector2.up * 4f;
					this.ACT_BLINK.StartAction(this.owner, point, 0.2f, true, true);
					yield return this.ACT_BLINK.waitForCompletion;
				}
				else
				{
					this.ACT_LOOK.StartAction(this.owner);
					yield return this.ACT_LOOK.waitForCompletion;
				}
				Vector2 dir = Vector2.right * (float)ama.GetDirFromOrientation();
				ama.currentMeleeAttack = ama.meleeStompAttack;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				ama.Amanecidas.AnimatorInyector.PlayStompAttack(this.doStompDamage);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				if (this.jumpMethod != null)
				{
					this.jumpMethod();
				}
				else if (this.doBlinkBeforeJump)
				{
					ama.DoSmallJumpSmash(this.usePillars);
				}
				else
				{
					ama.DoSmallJumpSmashToPoint(this.targetPoint, this.usePillars);
				}
				this.ACT_WAIT.StartAction(this.owner, 1.25f);
				yield return this.ACT_WAIT.waitForCompletion;
				if (!this.doBlinkBeforeJump && this.bounceAfterJump)
				{
					this.ACT_MOVE.StartAction(this.owner, ama.transform.position + Vector2.up * this.bounceHeight, 0.3f, 12, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				ama.LookAtPenitentUsingOrientation();
				ama.Amanecidas.AnimatorInyector.ClearStompAttackTrigger();
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				ama.currentMeleeAttack = this.previousMeleeAttack;
				base.FinishAction();
				yield break;
			}

			private Action jumpMethod;

			private Vector2 targetPoint;

			private bool doBlinkBeforeJump;

			private AmanecidasMeleeAttack previousMeleeAttack;

			private bool doStompDamage;

			private bool usePillars;

			private bool bounceAfterJump;

			private float bounceHeight;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private LaunchMethod_EnemyAction ACT_JUMP = new LaunchMethod_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();
		}

		public class MultiStompAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int n, float secondsBetweenJumps, bool doBlinkBeforeJump, bool doStompDamage, bool usePillars, bool bounceAfterJump, float bounceHeight = 1f)
			{
				this.n = n;
				this.secondsBetweenJumps = secondsBetweenJumps;
				this.doBlinkBeforeJump = doBlinkBeforeJump;
				this.doStompDamage = doStompDamage;
				this.usePillars = usePillars;
				this.bounceAfterJump = bounceAfterJump;
				this.bounceHeight = bounceHeight;
				return base.StartAction(e);
			}

			public EnemyAction StartAction(EnemyBehaviour e, int n, float secondsBetweenJumps, Action jumpMethod, bool doBlinkBeforeJump, bool doStompDamage, bool usePillars, bool bounceAfterJump, float bounceHeight = 1f)
			{
				this.n = n;
				this.secondsBetweenJumps = secondsBetweenJumps;
				this.jumpMethod = jumpMethod;
				this.doBlinkBeforeJump = doBlinkBeforeJump;
				this.doStompDamage = doStompDamage;
				this.bounceAfterJump = bounceAfterJump;
				this.bounceHeight = bounceHeight;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_JUMP.StopAction();
				this.ACT_STOMP.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				for (int i = 0; i < this.n; i++)
				{
					if (this.jumpMethod != null)
					{
						this.ACT_STOMP.StartAction(this.owner, this.jumpMethod, this.doBlinkBeforeJump, this.doStompDamage, this.bounceAfterJump, this.bounceHeight);
					}
					else
					{
						this.ACT_STOMP.StartAction(this.owner, this.doBlinkBeforeJump, this.doStompDamage, this.usePillars, this.bounceAfterJump, this.bounceHeight);
					}
					yield return this.ACT_STOMP.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, this.secondsBetweenJumps);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private int n;

			private float secondsBetweenJumps;

			private Action jumpMethod;

			private bool doBlinkBeforeJump;

			private bool doStompDamage;

			private bool usePillars;

			private bool bounceAfterJump;

			private float bounceHeight;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private LaunchMethod_EnemyAction ACT_JUMP = new LaunchMethod_EnemyAction();

			private AmanecidasBehaviour.StompAttack_EnemyAction ACT_STOMP = new AmanecidasBehaviour.StompAttack_EnemyAction();
		}

		public class Dash_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, Transform target, BossDashAttack dashAttack)
			{
				this.dir = target.position - e.transform.position;
				this.distance = this.dir.magnitude;
				return this.StartAction(e, this.dir, dashAttack, this.distance, false);
			}

			public EnemyAction StartAction(EnemyBehaviour e, Vector2 dir, BossDashAttack dashAttack, float distance = 10f, bool unblockable = false)
			{
				this.dir = dir;
				this.distance = distance;
				this.finished = false;
				this.dashAttack = dashAttack;
				this.unblockable = unblockable;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				this.parried = false;
				this.dashAttack.OnDashBlockedEvent -= this.DashAttack_OnDashBlocked;
				this.dashAttack.OnDashFinishedEvent -= this.DashAttack_OnDashFinished;
				this.dashAttack.OnDashBlockedEvent += this.DashAttack_OnDashBlocked;
				this.dashAttack.OnDashFinishedEvent += this.DashAttack_OnDashFinished;
				this.dashAttack.unblockable = this.unblockable;
				ama.Amanecidas.Audio.PlaySwordDash_AUDIO();
				this.dashAttack.Dash(ama.transform, this.dir, this.distance, 0f, true);
				while (!this.finished)
				{
					yield return null;
				}
				base.FinishAction();
				yield break;
			}

			private void DashAttack_OnDashFinished()
			{
				this.finished = true;
			}

			private void DashAttack_OnDashBlocked(BossDashAttack obj)
			{
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.ShakeWave(true);
				this.parried = true;
				this.finished = true;
			}

			private Vector2 dir;

			private float distance;

			private bool finished;

			private bool unblockable;

			public bool parried;

			private BossDashAttack dashAttack;
		}

		public class MultiFrontalDash_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_KEEPDISTANCE.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE_EASE.StopAction();
				this.ACT_DASH.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_LUNGE.StopAction();
				this.ACT_LOOK.StopAction();
				this.ACT_TIRED.StopAction();
				this.ACT_STOMP.StopAction();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, int numDashes, float anticipationSeconds)
			{
				this.numDashes = numDashes;
				this.anticipationSeconds = anticipationSeconds;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				Rect battleBounds = ama.battleBounds;
				AmanecidasAnimatorInyector anim = ama.Amanecidas.AnimatorInyector;
				Penitent p = Core.Logic.Penitent;
				int parryCounter = 0;
				float extraAnticipation = 0.3f;
				ama.PlayAnticipationGrunt(AmanecidasBehaviour.AMANECIDA_GRUNTS.MULTI_FRONTAL_GRUNT);
				for (int i = 0; i < this.numDashes; i++)
				{
					this.ACT_LOOK.StartAction(this.owner);
					yield return this.ACT_LOOK.waitForCompletion;
					Vector2 target = ama.GetPointBelowPenitent(true);
					Vector2 dashDir = (target - this.owner.transform.position).normalized;
					dashDir.x += (float)ama.GetDirFromOrientation();
					dashDir.y = 0f;
					dashDir = dashDir.normalized;
					this.ACT_MOVE_EASE.StartAction(this.owner, this.owner.transform.position - dashDir * 1.5f, extraAnticipation + this.anticipationSeconds * 0.4f, 6, null, true, null, true, true, 1.7f);
					this.ACT_LUNGE.StartAction(this.owner, extraAnticipation + this.anticipationSeconds * 0.4f, 0f, 0.15f, true);
					ama.Amanecidas.Audio.PlaySwordDashPreattack_AUDIO();
					this.ACT_WAIT.StartAction(this.owner, extraAnticipation + this.anticipationSeconds);
					yield return this.ACT_WAIT.waitForCompletion;
					extraAnticipation = 0f;
					ama.throwbackExtraTime = 0f;
					this.ACT_DASH.StartAction(this.owner, dashDir, ama.lanceDashAttack, 20f, false);
					yield return this.ACT_DASH.waitForCompletion;
					if (this.ACT_DASH.parried)
					{
						parryCounter++;
						if (parryCounter >= this.numDashes)
						{
							ama.InstantBreakShield();
						}
					}
					float secondstoBlinkOut = 0.6f;
					this.ACT_WAIT.StartAction(this.owner, secondstoBlinkOut);
					yield return this.ACT_WAIT.waitForCompletion;
					Vector2 newPos = ama.transform.position - Vector3.right * 3f * (float)ama.GetDirFromOrientation();
					newPos.x = Mathf.Clamp(newPos.x, ama.battleBounds.xMin, ama.battleBounds.xMax);
					this.ACT_STOMP.StartAction(this.owner, newPos, false, true, false, false, 1f);
					yield return this.ACT_STOMP.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, ama.throwbackExtraTime);
					yield return this.ACT_WAIT.waitForCompletion;
					if (ama.IsPenitentInTop())
					{
						break;
					}
				}
				anim.SetBlink(false);
				this.ACT_WAIT.StartAction(this.owner, 0.4f);
				yield return this.ACT_WAIT.waitForCompletion;
				if (parryCounter >= this.numDashes)
				{
					this.ACT_TIRED.StartAction(this.owner, 3f, true);
					yield return this.ACT_TIRED.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private int numDashes;

			private float anticipationSeconds;

			private AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction ACT_KEEPDISTANCE = new AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE = new MoveToPointUsingAgent_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_EASE = new MoveEasing_EnemyAction();

			private AmanecidasBehaviour.Dash_EnemyAction ACT_DASH = new AmanecidasBehaviour.Dash_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.LungeAnimation_EnemyAction ACT_LUNGE = new AmanecidasBehaviour.LungeAnimation_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();

			private AmanecidasBehaviour.TiredPeriod_EnemyAction ACT_TIRED = new AmanecidasBehaviour.TiredPeriod_EnemyAction();

			private AmanecidasBehaviour.StompAttack_EnemyAction ACT_STOMP = new AmanecidasBehaviour.StompAttack_EnemyAction();
		}

		public class HorizontalBlinkDashes_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_KEEPDISTANCE.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_DASH.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_LANCES.StopAction();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, int numDashes, float delay, bool startDashesAwayFromPenitent)
			{
				this.numDashes = numDashes;
				this.delay = delay;
				this.startDashesAwayFromPenitent = startDashesAwayFromPenitent;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				Rect battleBounds = ama.battleBounds;
				AmanecidasAnimatorInyector anim = ama.Amanecidas.AnimatorInyector;
				Penitent p = Core.Logic.Penitent;
				float xOrigin = 0f;
				float xEnd = 0f;
				bool startFromTheRight = false;
				for (int i = 0; i < this.numDashes; i++)
				{
					if (i == 0)
					{
						startFromTheRight = ((this.startDashesAwayFromPenitent && p.GetPosition().x < battleBounds.center.x) || (!this.startDashesAwayFromPenitent && p.GetPosition().x > battleBounds.center.x));
					}
					else
					{
						startFromTheRight = !startFromTheRight;
					}
					float xOffset = -0.5f;
					if (startFromTheRight)
					{
						xOrigin = battleBounds.xMax;
						xEnd = battleBounds.xMin - xOffset;
					}
					else
					{
						xOrigin = battleBounds.xMin;
						xEnd = battleBounds.xMax + xOffset;
					}
					float y = (i % 2 == 0) ? (battleBounds.yMin - 1f) : (battleBounds.yMin + 3f);
					Vector2 originPoint = new Vector2(xOrigin, y);
					Vector2 endPoint = new Vector2(xEnd, originPoint.y);
					Vector2 dir = endPoint - originPoint;
					if (i == 0)
					{
						Vector3 penitentPos = Core.Logic.Penitent.transform.position;
						Vector2 originLances = new Vector2(penitentPos.x, battleBounds.yMax + 2f);
						Vector2 endLances = new Vector2(penitentPos.x, battleBounds.yMax + 2f);
						int numLances = 3;
						originLances.x += (float)numLances * 0.55f * (float)ama.GetDirFromOrientation();
						endLances.x -= (float)numLances * 0.55f * (float)ama.GetDirFromOrientation();
						if (ama.IsPenitentInTop())
						{
							originLances.y += 2f;
							endLances.y += 2f;
						}
						this.ACT_LANCES.StartAction(this.owner, numLances, originLances, endLances, originPoint, new Action<Vector2>(ama.SetFrozenLance), new Action(ama.ActivateFrozenLances), 0.1f, 0.2f, 0f, false);
						yield return this.ACT_LANCES.waitForCompletion;
						this.ACT_WAIT.StartAction(this.owner, 1.5f);
						yield return this.ACT_WAIT.waitForCompletion;
					}
					else
					{
						this.ACT_BLINK.StartAction(this.owner, originPoint, 0.15f, false, true);
						yield return this.ACT_BLINK.waitForCompletion;
					}
					ama.LookAtDirUsingOrientation((!startFromTheRight) ? Vector2.right : Vector2.left);
					ama.PlayChargeEnergy(1f, true, true);
					this.ACT_WAIT.StartAction(this.owner, this.delay);
					anim.PlayChargeAnticipation(true);
					yield return this.ACT_WAIT.waitForCompletion;
					anim.SetCharge(true);
					ama.lastX = ama.transform.position.x;
					ama.ActivateBeam();
					ama.beamLauncher.transform.right = -dir;
					ama.Amanecidas.Audio.PlaySwordDashPreattack_AUDIO();
					this.ACT_WAIT.StartAction(this.owner, 0.3f);
					yield return this.ACT_WAIT.waitForCompletion;
					float waitAfterDash = 0.4f;
					bool isLastDash = i == this.numDashes - 1;
					if (isLastDash)
					{
						ama.Amanecidas.AnimatorInyector.SetBlink(false);
						ama.Amanecidas.AnimatorInyector.SetStuck(true);
						waitAfterDash = 1f;
						ama.lanceDashAttack.checkCollisions = true;
					}
					else
					{
						ama.lanceDashAttack.checkCollisions = false;
					}
					this.ACT_DASH.StartAction(this.owner, dir, ama.lanceDashAttack, dir.magnitude, true);
					anim.SetCharge(false);
					anim.SetLunge(true);
					yield return this.ACT_DASH.waitForCompletion;
					anim.SetLunge(false);
					float beamDelay = 0.3f;
					if (isLastDash)
					{
						beamDelay = 0f;
						ama.ClearRotationAndFlip();
						ama.ApplyStuckOffset();
					}
					ama.DeactivateBeam(beamDelay);
					this.ACT_WAIT.StartAction(this.owner, waitAfterDash);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				this.ACT_WAIT.StartAction(this.owner, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetStuck(false);
				this.ACT_WAIT.StartAction(this.owner, 0.6f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetHorizontalCharge(false);
				base.FinishAction();
				yield break;
			}

			private int numDashes;

			private float delay;

			private bool startDashesAwayFromPenitent;

			private AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction ACT_KEEPDISTANCE = new AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private AmanecidasBehaviour.Dash_EnemyAction ACT_DASH = new AmanecidasBehaviour.Dash_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.ShootFrozenLances_EnemyAction ACT_LANCES = new AmanecidasBehaviour.ShootFrozenLances_EnemyAction();
		}

		public class DiagonalBlinkDashes_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				base.DoOnStop();
				this.ACT_WAIT.StopAction();
				this.ACT_KEEPDISTANCE.StopAction();
				this.ACT_DASH.StopAction();
				this.ACT_BLINK.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.ClearRotationAndFlip();
			}

			public EnemyAction StartAction(EnemyBehaviour e, int numDashes, float anticipationDelay, float shortAnticipationDelay, float vulnerabilityTime)
			{
				this.numDashes = numDashes;
				this.longAnticipationDelay = anticipationDelay;
				this.shortAnticipationDelay = shortAnticipationDelay;
				this.vulnerabilityTime = vulnerabilityTime;
				return base.StartAction(e);
			}

			private Vector2 GetOriginPoint(bool startFromRight, AmanecidasBehaviour ama, Vector2 distanceToPlayer)
			{
				Penitent penitent = Core.Logic.Penitent;
				Rect battleBounds = ama.battleBounds;
				float num = (!startFromRight) ? (penitent.GetPosition().x - distanceToPlayer.x) : (penitent.GetPosition().x + distanceToPlayer.x);
				float num2 = penitent.GetPosition().y + distanceToPlayer.y;
				num = Mathf.Clamp(num, battleBounds.xMin + 4f, battleBounds.xMax - 4f);
				return new Vector2(num, num2);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				Penitent p = Core.Logic.Penitent;
				Rect battleBounds = ama.battleBounds;
				bool startFromRight = p.GetPosition().x < battleBounds.center.x;
				for (int i = 0; i < this.numDashes; i++)
				{
					ama.Amanecidas.Audio.PlayDashCharge_AUDIO();
					Vector2 distanceToPlayer = new Vector2(5f, 3f);
					Vector2 originPoint = Vector2.zero;
					startFromRight = !startFromRight;
					this.ACT_BLINK.StartAction(this.owner, originPoint, 0.01f, false, true);
					yield return this.ACT_BLINK.waitForCompletion;
					float secondsBeforeReappear = 0.1f;
					this.ACT_WAIT.StartAction(this.owner, secondsBeforeReappear);
					yield return this.ACT_WAIT.waitForCompletion;
					originPoint = this.GetOriginPoint(startFromRight, ama, distanceToPlayer);
					ama.transform.position = originPoint;
					ama.LookAtPenitentUsingOrientation();
					ama.Amanecidas.AnimatorInyector.PlayChargeAnticipation(false);
					bool lastDash = i == this.numDashes - 1;
					float delayToUse = (!lastDash) ? this.shortAnticipationDelay : this.longAnticipationDelay;
					this.ACT_KEEPDISTANCE.StartAction(this.owner, delayToUse, true, true, true, ama.battleBounds.xMin + 0.5f, ama.battleBounds.xMax - 0.5f, false, 0f, 0f);
					yield return this.ACT_KEEPDISTANCE.waitForCompletion;
					float easingSeconds = 0.4f;
					Vector2 dir = (float)ama.GetDirFromOrientation() * distanceToPlayer.x * Vector2.right + Vector2.down * distanceToPlayer.y;
					TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(ama.transform, ama.transform.position - dir.normalized, easingSeconds, false), 6);
					if (lastDash)
					{
						ama.Amanecidas.Audio.StopDashCharge_AUDIO();
						ama.PlayChargeEnergy(1f, false, true);
						easingSeconds = 0.5f;
						ama.Amanecidas.Audio.PlayBeamDashPreattack_AUDIO();
					}
					else
					{
						ama.Amanecidas.Audio.PlaySwordDashPreattack_AUDIO();
					}
					this.ACT_WAIT.StartAction(this.owner, easingSeconds);
					yield return this.ACT_WAIT.waitForCompletion;
					ama.LookAtDirUsingOrientation(dir);
					ama.AimMeleeDirection(dir, ama.lanceRotationDiference);
					ama.Amanecidas.AnimatorInyector.SetCharge(true);
					ama.ActivateBeam();
					ama.beamLauncher.transform.right = -dir;
					if (lastDash)
					{
						ama.lanceDashAttack.checkCollisions = true;
						ama.Amanecidas.AnimatorInyector.SetBlink(false);
						ama.Amanecidas.AnimatorInyector.SetStuck(true);
					}
					else
					{
						ama.Amanecidas.Audio.StopDashCharge_AUDIO();
						ama.lanceDashAttack.checkCollisions = false;
					}
					ama.SmallDistortion();
					this.ACT_DASH.StartAction(this.owner, dir, ama.lanceDashAttack, 15f, lastDash);
					this.ACT_WAIT.StartAction(this.owner, 0.2f);
					yield return this.ACT_WAIT.waitForCompletion;
					ama.Amanecidas.AnimatorInyector.SetCharge(false);
					yield return this.ACT_DASH.waitForCompletion;
					if (lastDash)
					{
						ama.ApplyStuckOffset();
						if (ama.HasSolidFloorBelow(false))
						{
							ama.SpikeWave(ama.transform.position, 1f, 14, true, 1.5f);
						}
					}
					ama.Amanecidas.AnimatorInyector.SetLunge(false);
					ama.DeactivateBeam(0.3f);
					float waitTime = (!lastDash) ? 0.1f : this.vulnerabilityTime;
					if (!ama.HasSolidFloorBelow(false))
					{
						waitTime = 0.1f;
					}
					ama.ClearRotationAndFlip();
					this.ACT_WAIT.StartAction(this.owner, waitTime);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				ama.Amanecidas.AnimatorInyector.SetStuck(false);
				float stuckToIdleTime = 0.4f;
				this.ACT_WAIT.StartAction(this.owner, stuckToIdleTime);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int numDashes;

			private float longAnticipationDelay;

			private float shortAnticipationDelay;

			private float vulnerabilityTime;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction ACT_KEEPDISTANCE = new AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private AmanecidasBehaviour.Dash_EnemyAction ACT_DASH = new AmanecidasBehaviour.Dash_EnemyAction();
		}

		public class BlinkAndDashToPenitent_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_DASH.StopAction();
				this.ACT_BLINK.StopAction();
				base.DoOnStop();
			}

			public EnemyAction StartAction(EnemyBehaviour e, Vector2 point, BossDashAttack dashAttack, float anticipation, bool skipReposition = false, bool endBlinkOut = false, bool showDashAim = false, float distance = 15f)
			{
				this.point = point;
				this.dashAttack = dashAttack;
				this.endBlinkOut = endBlinkOut;
				this.skipReposition = skipReposition;
				this.showDashTrail = showDashAim;
				this.anticipation = anticipation;
				this.distance = distance;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				if (!this.skipReposition)
				{
					this.ACT_BLINK.StartAction(this.owner, this.point, 0.4f, false, true);
					yield return this.ACT_BLINK.waitForCompletion;
				}
				ama.SetGhostTrail(false);
				Vector2 targetPos = Core.Logic.Penitent.transform.position;
				Vector2 curPos = this.owner.transform.position;
				Vector2 dir = (targetPos - curPos).normalized;
				Vector2 dashPoint = targetPos + dir * 5f;
				ama.LookAtPenitentUsingOrientation();
				float angleDifference = ama.GetAngleDifference();
				ama.AimMeleeDirection(dir, angleDifference);
				ama.Amanecidas.Audio.PlaySwordDashPreattack_AUDIO();
				ama.Amanecidas.AnimatorInyector.PlayChargeAnticipation(false);
				float anticipationSeconds = this.anticipation;
				if (this.showDashTrail)
				{
					Vector2 offset = ama.Amanecidas.AnimatorInyector.GetCurrentUp() * 1.25f;
					this.ACT_TRAIL.StartAction(this.owner, ama.transform.position + offset, dashPoint + offset, anticipationSeconds);
					yield return this.ACT_TRAIL.waitForCompletion;
				}
				else
				{
					this.ACT_WAIT.StartAction(this.owner, anticipationSeconds);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				ama.Amanecidas.AnimatorInyector.SetCharge(true);
				this.ACT_WAIT.StartAction(this.owner, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetBlink(this.endBlinkOut);
				this.dashAttack.checkCollisions = false;
				ama.Amanecidas.Audio.PlaySwordDash_AUDIO();
				this.ACT_DASH.StartAction(this.owner, dashPoint - this.owner.transform.position, this.dashAttack, this.distance, false);
				this.ACT_WAIT.StartAction(this.owner, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetCharge(false);
				this.ACT_WAIT.StartAction(this.owner, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.ClearRotationAndFlip();
				ama.Amanecidas.SetOrientation((dir.x <= 0f) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
				ama.Amanecidas.AnimatorInyector.SetLunge(false);
				this.ACT_WAIT.StartAction(this.owner, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.SetGhostTrail(true);
				yield return this.ACT_DASH.waitForCompletion;
				this.ACT_WAIT.StartAction(this.owner, 0.25f);
				yield return this.ACT_WAIT.waitForCompletion;
				if (!this.endBlinkOut)
				{
					yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
					ama.ClearRotationAndFlip();
				}
				base.FinishAction();
				yield break;
			}

			private Vector2 dir;

			private Vector2 point;

			private BossDashAttack dashAttack;

			private float distance;

			private bool skipReposition;

			private bool endBlinkOut;

			private float anticipation;

			private bool showDashTrail;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private AmanecidasBehaviour.Dash_EnemyAction ACT_DASH = new AmanecidasBehaviour.Dash_EnemyAction();

			private AmanecidasBehaviour.ShowDashAnticipationTrail_EnemyAction ACT_TRAIL = new AmanecidasBehaviour.ShowDashAnticipationTrail_EnemyAction();
		}

		private class WaitUntilIdle : AmanecidasBehaviour.WaitUntilAnimationState
		{
			public WaitUntilIdle(AmanecidasBehaviour ama, float timeout = 5f) : base(ama, "IDLE", timeout)
			{
			}
		}

		private class WaitUntilAnimationState : CustomYieldInstruction
		{
			public WaitUntilAnimationState(AmanecidasBehaviour ama, string state, float timeout)
			{
				this.ama = ama;
				this.state = state;
				this.timeout = timeout;
				this.timeElapsed = 0f;
			}

			public override bool keepWaiting
			{
				get
				{
					bool flag = !this.ama.Amanecidas.AnimatorInyector.bodyAnimator.GetCurrentAnimatorStateInfo(0).IsName(this.state);
					bool flag2 = this.timeElapsed >= this.timeout;
					this.timeElapsed += Time.deltaTime;
					return flag || flag2;
				}
			}

			private AmanecidasBehaviour ama;

			private string state;

			private float timeout;

			private float timeElapsed;
		}

		private class WaitUntilNotTurning : CustomYieldInstruction
		{
			public WaitUntilNotTurning(AmanecidasBehaviour ama)
			{
				this.ama = ama;
			}

			public override bool keepWaiting
			{
				get
				{
					return this.ama.Amanecidas.AnimatorInyector.IsTurning();
				}
			}

			private AmanecidasBehaviour ama;
		}

		public class LookAtPenitent_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector anim = ama.Amanecidas.AnimatorInyector;
				int dirFromOrientation = ama.GetDirFromOrientation();
				Vector2 dirToPenitent = ama.GetDirToPenitent(ama.transform.position);
				ama.LookAtPenitent(false);
				if (Mathf.Sign((float)dirFromOrientation) != Mathf.Sign(dirToPenitent.x))
				{
					Debug.Log(string.Format("Turn needed. Orientation: {2} dirFromOrientation:{0} . dirToPenitent.x:{1} ", dirFromOrientation, dirToPenitent.x, ama.Amanecidas.Status.Orientation));
					Debug.DrawRay(this.owner.transform.position, dirToPenitent * 5f, Color.green, 5f);
					this.ACT_WAIT.StartAction(this.owner, 0.2f);
					yield return this.ACT_WAIT.waitForCompletion;
					yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				}
				else
				{
					Debug.Log(string.Format("Turn NOT needed. Orientation: {2} . dirFromOrientation:{0} . dirToPenitent.x:{1} ", dirFromOrientation, dirToPenitent.x, ama.Amanecidas.Status.Orientation));
					Debug.DrawRay(this.owner.transform.position, dirToPenitent * 5f, Color.red, 5f);
				}
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class MeleeAttackProjectile_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 originPoint, float anticipationTime, Action playSfxAction, BossStraightProjectileAttack projectileAttack, int number = 1)
			{
				this.originPoint = originPoint;
				this.anticipationTime = anticipationTime;
				this.projectileAttack = projectileAttack;
				this.n = number;
				this.playSfxAction = playSfxAction;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE_AGENT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_LOOK.StopAction();
				this.ACT_MELEE.StopAction();
				this.ACT_CHARGE.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				this.ACT_MOVE_AGENT.StartAction(this.owner, ama.agent, this.originPoint, 2f);
				yield return this.ACT_MOVE_AGENT.waitForCompletion;
				this.ACT_LOOK.StartAction(this.owner);
				yield return this.ACT_LOOK.waitForCompletion;
				this.ACT_CHARGE.StartAction(this.owner, this.anticipationTime * 0.5f, true);
				ama.Amanecidas.Audio.PlayHorizontalPreattack_AUDIO();
				ama.throwbackExtraTime = 0f;
				for (int i = 0; i < this.n; i++)
				{
					float actualAnticipation = this.anticipationTime;
					if (i > 0)
					{
						if (ama.throwbackExtraTime > 0f)
						{
							this.ACT_WAIT.StartAction(this.owner, ama.throwbackExtraTime);
							yield return this.ACT_WAIT.waitForCompletion;
						}
						actualAnticipation = 0.7f;
					}
					this.ACT_MELEE.StartAction(this.owner, actualAnticipation, 0f, 0.5f);
					yield return this.ACT_MELEE.waitForCallback;
					this.playSfxAction();
					int right = ama.GetDirFromOrientation();
					Vector2 d = Vector2.right * (float)right;
					this.projectileAttack.Shoot(d);
					yield return this.ACT_MELEE.waitForCompletion;
				}
				this.ACT_WAIT.StartAction(this.owner, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private float anticipationTime;

			private BossStraightProjectileAttack projectileAttack;

			private Vector2 originPoint;

			private int n;

			private Action playSfxAction;

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_AGENT = new MoveToPointUsingAgent_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();

			private AmanecidasBehaviour.MeleeAttack_EnemyAction ACT_MELEE = new AmanecidasBehaviour.MeleeAttack_EnemyAction();

			private AmanecidasBehaviour.EnergyChargePeriod_EnemyAction ACT_CHARGE = new AmanecidasBehaviour.EnergyChargePeriod_EnemyAction();
		}

		public class MeleeAttack_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float anticipationTime, float advanceDistance, float afterAttackSeconds)
			{
				this.anticipationTime = anticipationTime;
				this.advanceDistance = advanceDistance;
				this.afterAttackSeconds = afterAttackSeconds;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_LOOK.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				Penitent p = Core.Logic.Penitent;
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				Vector2 target = p.GetPosition();
				Vector2 dir = o.GetDirToPenitent(this.owner.transform.position);
				target -= Vector2.right * Mathf.Sign(dir.x) * this.advanceDistance;
				this.ACT_LOOK.StartAction(this.owner);
				yield return this.ACT_LOOK.waitForCompletion;
				o.LookAtPenitent(true);
				o.Amanecidas.AnimatorInyector.SetMeleeAnticipation(true);
				o.Amanecidas.AnimatorInyector.PlayMeleeAttack();
				this.ACT_WAIT.StartAction(this.owner, this.anticipationTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.LookAtPenitentUsingOrientation();
				dir = o.GetDirToPenitent(o.transform.position);
				float maxDashDistance = 5f;
				Vector2 dashVector = dir.normalized * maxDashDistance;
				target = o.transform.position + dashVector;
				o.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				o.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				float secondsBeforeSlash = 0.15f;
				this.ACT_WAIT.StartAction(this.owner, secondsBeforeSlash);
				yield return this.ACT_WAIT.waitForCompletion;
				base.Callback();
				if (this.advanceDistance > 0f)
				{
					float moveTime = 0.3f;
					this.ACT_MOVE.StartAction(this.owner, target, moveTime, 6, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				this.ACT_WAIT.StartAction(this.owner, this.afterAttackSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				this.ACT_WAIT.StartAction(this.owner, 0.15f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private float anticipationTime;

			private float advanceDistance;

			private float afterAttackSeconds;

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();
		}

		public class MeleeAttackTowardsPenitent_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float anticipationTime, Action launchAudioMethod, bool blinkOut = false)
			{
				this.anticipationTime = anticipationTime;
				this.blinkOut = blinkOut;
				this.launchAudioMethod = launchAudioMethod;
				return base.StartAction(e);
			}

			protected override IEnumerator BaseCoroutine()
			{
				Penitent p = Core.Logic.Penitent;
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				if (o.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE)
				{
					o.currentMeleeAttack = o.meleeAxeAttack;
				}
				else if (o.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.SWORD)
				{
					o.currentMeleeAttack = o.meleeFalcataAttack;
				}
				float distance = 4f;
				float minDashDistance = 1f;
				Vector2 dir = o.GetDirToPenitent(this.owner.transform.position);
				Vector2 target = o.GetPointBelowPenitent(true) - Vector2.right * Mathf.Sign(dir.x) * distance;
				if (dir.magnitude <= minDashDistance)
				{
					target = o.GetPointBelowPenitent(true) - Vector2.right * Mathf.Sign(dir.x) * (distance / 2f);
				}
				this.ACT_MOVE_AGENT.StartAction(this.owner, o.agent, target, 2f);
				yield return this.ACT_MOVE_AGENT.waitForCallback;
				this.ACT_LOOK.StartAction(this.owner);
				yield return this.ACT_LOOK.waitForCompletion;
				o.Amanecidas.AnimatorInyector.SetMeleeAnticipation(true);
				o.Amanecidas.AnimatorInyector.PlayMeleeAttack();
				this.ACT_WAIT.StartAction(this.owner, this.anticipationTime);
				yield return this.ACT_WAIT.waitForCompletion;
				dir = o.GetDirToPenitent(o.transform.position);
				dir.y = 0f;
				float finalDistance = Mathf.Clamp(Mathf.Abs(dir.x) + 1f, minDashDistance, 10f);
				Vector2 attackDir = (float)o.GetDirFromOrientation() * Vector2.right;
				Vector2 dashVector = attackDir * finalDistance;
				target = o.transform.position + dashVector;
				if (o.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE)
				{
					target.y = o.transform.position.y;
				}
				target.x = Mathf.Clamp(target.x, o.battleBounds.xMin, o.battleBounds.xMax);
				o.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				float moveTime = 0.8f;
				this.ACT_MOVE.StartAction(this.owner, target, moveTime, 26, null, true, null, true, true, 2f);
				this.ACT_WAIT.StartAction(this.owner, moveTime * 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Amanecidas.Audio.PlaySwordPreattack_AUDIO();
				this.ACT_WAIT.StartAction(this.owner, moveTime * 0.35f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				if (this.blinkOut)
				{
					o.Amanecidas.AnimatorInyector.SetBlink(this.blinkOut);
				}
				this.ACT_METHOD.StartAction(this.owner, this.launchAudioMethod);
				this.ACT_WAIT.StartAction(this.owner, moveTime * 0.3f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				yield return this.ACT_MOVE.waitForCompletion;
				Vector2 driftTarget = target;
				if (dashVector.x > 0f)
				{
					driftTarget.x += 1f;
				}
				else
				{
					driftTarget.x -= 1f;
				}
				this.ACT_MOVE.StartAction(this.owner, driftTarget, 0.2f, 18, null, true, null, true, true, 1.7f);
				if (o.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE)
				{
					this.ACT_DUST_VFX.StartAction(this.owner, target + Vector2.down * 0.2f, o.dustVFX, 0.3f);
					this.ACT_WAIT.StartAction(this.owner, 0.1f);
					yield return this.ACT_WAIT.waitForCompletion;
					this.ACT_DUST_VFX.StartAction(this.owner, driftTarget + Vector2.down * 0.2f, o.dustVFX, 0.3f);
				}
				yield return this.ACT_MOVE.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE_AGENT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_LOOK.StopAction();
				this.ACT_METHOD.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				base.DoOnStop();
			}

			private float anticipationTime;

			private bool blinkOut;

			private Action launchAudioMethod;

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_AGENT = new MoveToPointUsingAgent_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.LookAtPenitent_EnemyAction ACT_LOOK = new AmanecidasBehaviour.LookAtPenitent_EnemyAction();

			private LaunchMethod_EnemyAction ACT_METHOD = new LaunchMethod_EnemyAction();

			private AmanecidasBehaviour.ShowSimpleVFX_EnemyAction ACT_DUST_VFX = new AmanecidasBehaviour.ShowSimpleVFX_EnemyAction();
		}

		public class GhostProjectile_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Action<Vector2, Vector2> shootProjectileMethod, float anticipationSeconds)
			{
				this.shootProjectileMethod = shootProjectileMethod;
				this.anticipationSeconds = anticipationSeconds;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_INTERRUPTABLE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_METHODLAUNCH_VECTORS.StopAction();
				this.ACT_LUNGE.StopAction();
				this.ACT_PUNCH.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				Penitent p = Core.Logic.Penitent;
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				yield return new WaitUntil(() => !Core.Logic.Penitent.IsJumping);
				Vector2 targetPos = o.battleBounds.center + Vector2.up * 2f;
				this.ACT_MOVE_AGENT.StartAction(o, o.agent, targetPos, 2f);
				yield return this.ACT_MOVE_AGENT.waitForCompletion;
				int i = 5;
				for (int j = 0; j < i; j++)
				{
					Vector2 targetDir = o.GetDirToPenitent(o.transform.position);
					this.ACT_LUNGE.StartAction(this.owner, this.anticipationSeconds * 0.4f, 0f, 0.1f, true);
					this.ACT_PUNCH.StartAction(this.owner, -targetDir.normalized, 1f, this.anticipationSeconds);
					o.Amanecidas.Audio.PlaySwordDashPreattack_AUDIO();
					yield return this.ACT_PUNCH.waitForCompletion;
					o.SetGhostTrail(false);
					o.Amanecidas.Audio.PlaySwordDash_AUDIO();
					this.ACT_METHODLAUNCH_VECTORS.StartAction(this.owner, this.shootProjectileMethod, targetPos, targetDir);
					yield return this.ACT_METHODLAUNCH_VECTORS.waitForCompletion;
					yield return this.ACT_LUNGE.waitForCompletion;
					o.ClearRotationAndFlip();
					yield return new AmanecidasBehaviour.WaitUntilIdle(o, 5f);
				}
				o.SetGhostTrail(true);
				base.FinishAction();
				yield break;
			}

			private float anticipationSeconds;

			private Action<Vector2, Vector2> shootProjectileMethod;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_AGENT = new MoveToPointUsingAgent_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private LaunchMethodWithTwoVectors_EnemyAction ACT_METHODLAUNCH_VECTORS = new LaunchMethodWithTwoVectors_EnemyAction();

			private AmanecidasBehaviour.LungeAnimation_EnemyAction ACT_LUNGE = new AmanecidasBehaviour.LungeAnimation_EnemyAction();

			private AmanecidasBehaviour.EaseAnticipation_EnemyAction ACT_PUNCH = new AmanecidasBehaviour.EaseAnticipation_EnemyAction();

			private AmanecidasBehaviour.InterruptablePeriod_EnemyAction ACT_INTERRUPTABLE = new AmanecidasBehaviour.InterruptablePeriod_EnemyAction();
		}

		public class EaseAnticipation_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour owner, Vector2 dir, float distance, float easeSeconds)
			{
				this.targetDir = dir;
				this.distance = distance;
				this.easeSeconds = easeSeconds;
				return base.StartAction(owner);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				ShortcutExtensions.DOKill(this.owner.transform, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				Sequence s = DOTween.Sequence();
				TweenSettingsExtensions.Append(s, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(this.owner.transform, this.owner.transform.position + this.targetDir * this.distance, 0.75f * this.easeSeconds, false), 21));
				TweenSettingsExtensions.Append(s, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(this.owner.transform, this.owner.transform.position, 0.25f * this.easeSeconds, false), 20));
				TweenExtensions.Play<Sequence>(s);
				this.ACT_WAIT.StartAction(this.owner, this.easeSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private Vector2 targetDir;

			private float distance;

			private float easeSeconds;
		}

		public class QuickLunge_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int number)
			{
				this.number = number;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE_AGENT.StopAction();
				this.ACT_LUNGE.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				Penitent p = Core.Logic.Penitent;
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				Vector2 target = p.GetPosition();
				target += ((this.owner.transform.position.x <= target.x) ? (Vector2.left * 0.5f) : (Vector2.right * 0.5f));
				o.LookAtPenitent(false);
				this.ACT_MOVE_AGENT.StartAction(this.owner, this.owner.GetComponent<AutonomousAgent>(), target, 2f);
				yield return this.ACT_MOVE_AGENT.waitForCompletion;
				for (int i = 0; i < this.number; i++)
				{
					Vector2 dir = (float)o.GetDirFromOrientation() * Vector2.right;
					float baseDistance = 1.5f;
					float finalDistance = 3f;
					if (i == this.number - 1)
					{
						dir *= finalDistance;
					}
					else
					{
						dir *= baseDistance;
					}
					float delay = 0.8f;
					Sequence s = DOTween.Sequence();
					TweenSettingsExtensions.Append(s, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(this.owner.transform, this.owner.transform.position - dir * 0.25f, 0.7f, false), 21));
					TweenSettingsExtensions.Append(s, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(this.owner.transform, this.owner.transform.position + dir * 0.75f, 0.2f, false), 20));
					TweenSettingsExtensions.SetDelay<Sequence>(s, delay);
					TweenExtensions.Play<Sequence>(s);
					o.Amanecidas.Audio.PlaySwordDash_AUDIO();
					this.ACT_LUNGE.StartAction(this.owner, 0.5f, 0f, 0.2f, true);
					yield return this.ACT_LUNGE.waitForCompletion;
				}
				base.FinishAction();
				yield break;
			}

			private AmanecidasBehaviour.LungeAnimation_EnemyAction ACT_LUNGE = new AmanecidasBehaviour.LungeAnimation_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_AGENT = new MoveToPointUsingAgent_EnemyAction();

			private int number = 3;
		}

		public class LungeAnimation_EnemyAction : EnemyAction
		{
			public void StartAction(EnemyBehaviour e, float anticipationSeconds, float chargeSeconds, float lungeSeconds, bool isHorizontalCharge)
			{
				this.anticipationSeconds = anticipationSeconds;
				this.chargeSeconds = chargeSeconds;
				this.lungeSeconds = lungeSeconds;
				this.isHorizontalCharge = isHorizontalCharge;
				base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				float baseAnticipationSeconds = 0.13f;
				float baseChargeSeconds = 0.15f;
				float baseLungeSeconds = 0.12f;
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector anim = ama.Amanecidas.AnimatorInyector;
				anim.PlayChargeAnticipation(this.isHorizontalCharge);
				this.ACT_WAIT.StartAction(this.owner, baseAnticipationSeconds + this.anticipationSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.SetCharge(true);
				this.ACT_WAIT.StartAction(this.owner, baseChargeSeconds + this.chargeSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.SetCharge(false);
				anim.SetLunge(true);
				this.ACT_WAIT.StartAction(this.owner, baseLungeSeconds + this.lungeSeconds);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.SetLunge(false);
				if (this.isHorizontalCharge)
				{
					ama.Amanecidas.AnimatorInyector.SetHorizontalCharge(false);
				}
				base.FinishAction();
				yield break;
			}

			private bool isHorizontalCharge;

			private float anticipationSeconds;

			private float chargeSeconds;

			private float lungeSeconds;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class RechargeShield_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float _rechargeTime, float _anticipationTime, Action _rechargeShieldMethod, Action _anticipationMethod, Action _activateShieldMethod)
			{
				this.activateShieldMethod = _activateShieldMethod;
				this.rechargeShieldMethod = _rechargeShieldMethod;
				this.anticipationMethod = _anticipationMethod;
				this.rechargeTime = _rechargeTime;
				this.anticipationTime = _anticipationTime;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE_EASE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_METHOD.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_CALLAXE1.StopAction();
				this.ACT_CALLAXE2.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetRecharging(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetShockwaveAnticipation(false);
				amanecidasBehaviour.ShowCurrentWeapon(true);
				base.DoOnStop();
			}

			private bool IsInValidRechargePoint(Vector2 p)
			{
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				return amanecidasBehaviour.battleBounds.Contains(p);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				if (!this.IsInValidRechargePoint(ama.transform.position))
				{
					this.ACT_BLINK.StartAction(this.owner, ama.battleBounds.center, 0.1f, true, false);
					yield return this.ACT_BLINK.waitForCompletion;
				}
				if (ama.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.LANCE && ama.DoCrystalLancesPlatformsExist())
				{
					ama.DestroyCrystalLancesPlatforms(0.75f, 2.5f);
					ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
					ama.Amanecidas.AnimatorInyector.PlayStompAttack(true);
					ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
					this.ACT_WAIT.StartAction(ama, 1f);
					yield return this.ACT_WAIT.waitForCompletion;
					this.ACT_WAIT.StartAction(ama, 1.5f);
					ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
					yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
					ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				Vector2 p = ama.GetPointBelowMe(true, 100f);
				float moveTime = Vector2.Distance(this.owner.transform.position, p) * 0.25f + 0.5f;
				this.ACT_MOVE_EASE.StartAction(this.owner, p, moveTime, 7, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE_EASE.waitForCompletion;
				if (ama.currentWeapon == AmanecidasAnimatorInyector.AMANECIDA_WEAPON.AXE && !ama.IsWieldingAxe())
				{
					this.ACT_CALLAXE1.StartAction(this.owner, this.owner.transform.position + Vector3.up * 1.75f, ama.axes[0], 0.1f, 0.3f);
					this.ACT_CALLAXE2.StartAction(this.owner, this.owner.transform.position + Vector3.up * 1.75f, ama.axes[1], 0.1f, 0.3f);
					yield return this.ACT_CALLAXE1.waitForCompletion;
					yield return this.ACT_CALLAXE2.waitForCompletion;
				}
				this.ACT_METHOD.StartAction(this.owner, this.rechargeShieldMethod);
				this.ACT_WAIT.StartAction(this.owner, this.rechargeTime);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_METHOD.StartAction(this.owner, this.anticipationMethod);
				float distance = 1f;
				Vector2 explosionPoint = this.owner.transform.position + Vector3.up * distance;
				this.ACT_MOVE_EASE.StartAction(this.owner, explosionPoint, this.anticipationTime, 8, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE_EASE.waitForCompletion;
				this.ACT_METHOD.StartAction(this.owner, this.activateShieldMethod);
				this.ACT_WAIT.StartAction(this.owner, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_MOVE_EASE.StartAction(this.owner, explosionPoint - Vector2.up * distance, this.anticipationTime * 0.1f, 10, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE_EASE.waitForCompletion;
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				ama.ShowCurrentWeapon(true);
				ama.lastShieldRechargeWasInterrupted = false;
				base.FinishAction();
				yield break;
			}

			private Action activateShieldMethod;

			private Action rechargeShieldMethod;

			private Action anticipationMethod;

			private float rechargeTime;

			private float anticipationTime;

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE = new MoveToPointUsingAgent_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE_EASE = new MoveEasing_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private LaunchMethod_EnemyAction ACT_METHOD = new LaunchMethod_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE1 = new AmanecidasBehaviour.CallAxe_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE2 = new AmanecidasBehaviour.CallAxe_EnemyAction();
		}

		public class ShootRicochetArrow_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int numBounces, Action<Vector2, Vector2> showArrowTrailMethod, Action<Vector2, Vector2> shootArrowMethod, float waitTime, LayerMask mask, bool isPartOfCombo = false, bool launchToTheRight = false)
			{
				this.numBounces = numBounces;
				this.showArrowTrailMethod = showArrowTrailMethod;
				this.shootArrowMethod = shootArrowMethod;
				this.waitTime = waitTime;
				this.mask = mask;
				this.isPartOfCombo = isPartOfCombo;
				this.launchToTheRight = launchToTheRight;
				this.results = new RaycastHit2D[1];
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_METHODLAUNCH_VECTORS.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				Rect battleBounds = o.battleBounds;
				Vector2[] startPoints = new Vector2[this.numBounces];
				Vector2[] targetDirections = new Vector2[this.numBounces];
				Vector2[] targetPoints = new Vector2[this.numBounces];
				if (this.isPartOfCombo)
				{
					targetDirections[0] = Vector2.down + ((!this.launchToTheRight) ? (Vector2.left * 1.3f) : (Vector2.right * 1.3f));
					startPoints[0] = o.transform.position + new Vector2(0.5f, -1.1f);
				}
				else
				{
					Vector2 blinkPoint = new Vector2((Core.Logic.Penitent.GetPosition().x <= battleBounds.center.x) ? battleBounds.xMax : battleBounds.xMin, battleBounds.yMin);
					targetDirections[0] = Vector2.down + ((Core.Logic.Penitent.GetPosition().x <= battleBounds.center.x) ? (Vector2.left * Random.Range(1f, 3f)) : (Vector2.right * Random.Range(1f, 3f)));
					startPoints[0] = blinkPoint + Vector2.up * 0.75f;
					this.ACT_BLINK.StartAction(this.owner, blinkPoint, 0.15f, true, true);
					yield return this.ACT_BLINK.waitForCompletion;
				}
				startPoints[0] += Vector2.up;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				anim.SetBow(true);
				float bowReadyTime = 0.3f;
				this.ACT_WAIT.StartAction(this.owner, bowReadyTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.SetGhostTrail(false);
				if (this.isPartOfCombo)
				{
					o.AimToPointWithBow(o.transform.position + Vector2.down);
				}
				else
				{
					o.AimToPointWithBow(o.transform.position + targetDirections[0]);
				}
				int bounceCounter = 0;
				for (int i = 0; i < this.numBounces; i++)
				{
					if (!this.ThrowRay(startPoints[i], targetDirections[i]))
					{
						break;
					}
					bounceCounter++;
					targetPoints[i] = this.results[0].point;
					GizmoExtensions.DrawDebugCross(targetPoints[i], Color.green, 1f);
					this.ACT_METHODLAUNCH_VECTORS.StartAction(this.owner, this.showArrowTrailMethod, startPoints[i], targetPoints[i]);
					yield return this.ACT_METHODLAUNCH_VECTORS.waitForCompletion;
					if (i + 1 < this.numBounces)
					{
						targetDirections[i + 1] = this.CalculateBounceDirection(targetPoints[i] - startPoints[i], this.results[0]);
						startPoints[i + 1] = targetPoints[i] + targetDirections[i + 1] * 0.01f;
					}
				}
				this.ACT_WAIT.StartAction(this.owner, this.waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.SetBow(false);
				float bowShotTime = 0.15f;
				this.ACT_WAIT.StartAction(this.owner, bowShotTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.SetGhostTrail(true);
				o.LookAtPenitent(true);
				o.ClearRotationAndFlip();
				o.Amanecidas.SetOrientation(o.Amanecidas.Status.Orientation, true, false);
				o.Amanecidas.Audio.PlayArrowFireFast_AUDIO();
				for (int j = 0; j < bounceCounter; j++)
				{
					this.ACT_METHODLAUNCH_VECTORS.StartAction(this.owner, this.shootArrowMethod, startPoints[j], targetPoints[j]);
					yield return this.ACT_METHODLAUNCH_VECTORS.waitForCompletion;
				}
				yield return new AmanecidasBehaviour.WaitUntilIdle(o, 5f);
				base.FinishAction();
				yield break;
			}

			private Vector2 CalculateBounceDirection(Vector2 direction, RaycastHit2D hit)
			{
				return Vector3.Reflect(direction, hit.normal).normalized;
			}

			private bool ThrowRay(Vector2 startPoint, Vector2 direction)
			{
				bool result = false;
				if (Physics2D.RaycastNonAlloc(startPoint, direction, this.results, 100f, this.mask) > 0)
				{
					Debug.DrawRay(startPoint, direction.normalized * this.results[0].distance, Color.red, 1f);
					result = true;
				}
				else
				{
					Debug.DrawRay(startPoint, direction.normalized * 100f, Color.yellow, 1f);
				}
				return result;
			}

			private int numBounces;

			private Action<Vector2, Vector2> showArrowTrailMethod;

			private Action<Vector2, Vector2> shootArrowMethod;

			private float waitTime;

			private LayerMask mask;

			private bool isPartOfCombo;

			private bool launchToTheRight;

			private RaycastHit2D[] results;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private LaunchMethodWithTwoVectors_EnemyAction ACT_METHODLAUNCH_VECTORS = new LaunchMethodWithTwoVectors_EnemyAction();
		}

		public class ShootLaserArrow_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float waitTime, LayerMask mask, Action<Vector2, Vector2> showArrowTrailMethod, Action<Vector2, Vector2> shootArrowMethod)
			{
				this.showArrowTrailMethod = showArrowTrailMethod;
				this.shootArrowMethod = shootArrowMethod;
				this.waitTime = waitTime;
				this.mask = mask;
				this.results = new RaycastHit2D[1];
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_METHODLAUNCH_VECTORS.StopAction();
				this.ACT_MOVE_CHARACTER.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				anim.SetBow(false);
				anim.SetBlink(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(o, 1f);
				this.ACT_MOVE_CHARACTER.StartAction(this.owner, o.agent, o.battleBounds.center + Vector2.up * 1.5f, 2f);
				yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
				Vector2 startPoint = o.transform.position + new Vector2(0f, 1.75f);
				Vector2 targetDirection = o.GetDirToPenitent(startPoint);
				targetDirection.x += Random.Range(-0.2f, 0.2f);
				if (Core.Logic.Penitent.IsJumping || Core.Logic.Penitent.IsDashing)
				{
					targetDirection.x += ((Core.Logic.Penitent.GetOrientation() != EntityOrientation.Right) ? -0.5f : 0.5f);
				}
				anim.SetBow(true);
				float bowReadyTime = 0.3f;
				this.ACT_WAIT.StartAction(this.owner, bowReadyTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.SetGhostTrail(false);
				o.AimToPointWithBow(o.transform.position + targetDirection);
				this.ThrowRay(startPoint, targetDirection);
				Vector2 targetPoint = this.results[0].point;
				GizmoExtensions.DrawDebugCross(targetPoint, Color.green, 1f);
				this.ACT_METHODLAUNCH_VECTORS.StartAction(this.owner, this.showArrowTrailMethod, startPoint, targetPoint);
				yield return this.ACT_METHODLAUNCH_VECTORS.waitForCompletion;
				this.ACT_WAIT.StartAction(this.owner, this.waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.SetBow(false);
				float bowShotTime = 0.15f;
				this.ACT_WAIT.StartAction(this.owner, bowShotTime);
				yield return this.ACT_WAIT.waitForCompletion;
				o.SetGhostTrail(true);
				o.LookAtPenitent(true);
				o.ClearRotationAndFlip();
				o.Amanecidas.SetOrientation(o.Amanecidas.Status.Orientation, true, false);
				o.Amanecidas.Audio.PlayArrowFireFast_AUDIO();
				this.ACT_METHODLAUNCH_VECTORS.StartAction(this.owner, this.shootArrowMethod, startPoint, targetPoint);
				yield return this.ACT_METHODLAUNCH_VECTORS.waitForCompletion;
				yield return new AmanecidasBehaviour.WaitUntilIdle(o, 1f);
				base.FinishAction();
				yield break;
			}

			private bool ThrowRay(Vector2 startPoint, Vector2 direction)
			{
				bool result = false;
				if (Physics2D.RaycastNonAlloc(startPoint, direction, this.results, 100f, this.mask) > 0)
				{
					Debug.DrawRay(startPoint, direction.normalized * this.results[0].distance, Color.red, 1f);
					result = true;
				}
				else
				{
					Debug.DrawRay(startPoint, direction.normalized * 100f, Color.yellow, 1f);
				}
				return result;
			}

			private Action<Vector2, Vector2> showArrowTrailMethod;

			private Action<Vector2, Vector2> shootArrowMethod;

			private float waitTime;

			private LayerMask mask;

			private RaycastHit2D[] results;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private LaunchMethodWithTwoVectors_EnemyAction ACT_METHODLAUNCH_VECTORS = new LaunchMethodWithTwoVectors_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();
		}

		public class ShootMineArrows_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int numBlinks, Vector2 originPoint, Vector2 endPoint, Action shootMineMethod, Action activateMinesMethod, Action<Vector2, Vector2> showArrowTrailMethod, float anticipationtWaitTime, float blinksWaitTime, float afterEndReachedWaitTime)
			{
				this.numBlinks = numBlinks;
				this.originPoint = originPoint;
				this.endPoint = endPoint;
				this.shootMineMethod = shootMineMethod;
				this.activateMinesMethod = activateMinesMethod;
				this.showArrowTrailMethod = showArrowTrailMethod;
				this.anticipationtWaitTime = anticipationtWaitTime;
				this.blinksWaitTime = blinksWaitTime;
				this.afterEndReachedWaitTime = afterEndReachedWaitTime;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_METHODLAUNCH.StopAction();
				this.ACT_METHODLAUNCH_VECTORS.StopAction();
				this.ACT_MOVE_CHARACTER.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				for (int i = 0; i < this.numBlinks; i++)
				{
					Vector2 target = Vector2.Lerp(this.originPoint, this.endPoint, (float)i / (float)this.numBlinks);
					if (i == 0)
					{
						this.ACT_MOVE_CHARACTER.StartAction(this.owner, o.agent, target, 2f);
						yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
						anim.SetBow(true);
						anim.SetBlink(false);
						o.SetGhostTrail(false);
						Vector2 dir = (o.transform.position.x <= o.battleBounds.center.x) ? Vector2.right : Vector2.left;
						o.AimToPointWithBow(o.transform.position + dir);
						Vector2 trailStartPos = o.transform.position + new Vector2(-dir.x, 1.75f);
						Vector2 trailEndPos = trailStartPos + dir * 11f;
						this.ACT_METHODLAUNCH_VECTORS.StartAction(this.owner, this.showArrowTrailMethod, trailStartPos, trailEndPos);
						yield return this.ACT_METHODLAUNCH_VECTORS.waitForCompletion;
						this.ACT_WAIT.StartAction(this.owner, this.anticipationtWaitTime);
						yield return this.ACT_WAIT.waitForCompletion;
						anim.SetBow(false);
						anim.SetBlink(true);
					}
					else
					{
						this.ACT_MOVE.StartAction(this.owner, target, 0.2f, 8, null, true, null, true, true, 1.7f);
						yield return this.ACT_MOVE.waitForCompletion;
						anim.PlayBlinkshot();
					}
					this.ACT_METHODLAUNCH.StartAction(this.owner, this.shootMineMethod);
					yield return this.ACT_METHODLAUNCH.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, this.blinksWaitTime);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				this.ACT_WAIT.StartAction(this.owner, this.afterEndReachedWaitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_METHODLAUNCH.StartAction(this.owner, this.activateMinesMethod);
				yield return this.ACT_METHODLAUNCH.waitForCompletion;
				this.ACT_MOVE.StartAction(this.owner, o.battleBounds.center, 0.2f, 8, null, true, null, true, true, 1.7f);
				anim.SetBlink(false);
				o.SetGhostTrail(true);
				this.ACT_WAIT.StartAction(this.owner, this.afterEndReachedWaitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int numBlinks;

			private Vector2 originPoint;

			private Vector2 endPoint;

			private Action shootMineMethod;

			private Action activateMinesMethod;

			private Action<Vector2, Vector2> showArrowTrailMethod;

			private float anticipationtWaitTime;

			private float blinksWaitTime;

			private float afterEndReachedWaitTime;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private LaunchMethod_EnemyAction ACT_METHODLAUNCH = new LaunchMethod_EnemyAction();

			private LaunchMethodWithTwoVectors_EnemyAction ACT_METHODLAUNCH_VECTORS = new LaunchMethodWithTwoVectors_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();
		}

		public class ShootFrozenLances_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int numLances, Vector2 originPoint, Vector2 endPoint, Vector2 targetPosition, Action<Vector2> setLanceMethod, Action activateLancesMethod, float anticipationtWaitTime, float afterEndReachedWaitTime, float timeBetweenLances = 0.1f, bool shouldSpin = false)
			{
				this.numLances = numLances;
				this.originPoint = originPoint;
				this.endPoint = endPoint;
				this.targetPosition = targetPosition;
				this.setLanceMethod = setLanceMethod;
				this.activateLancesMethod = activateLancesMethod;
				this.anticipationtWaitTime = anticipationtWaitTime;
				this.afterEndReachedWaitTime = afterEndReachedWaitTime;
				this.timeBetweenLances = timeBetweenLances;
				this.shouldSpin = shouldSpin;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_METHODLAUNCH.StopAction();
				this.ACT_METHODLAUNCH_VECTOR.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				if (Vector3.Distance(o.transform.position, this.targetPosition) > 1f)
				{
					this.ACT_BLINK.StartAction(this.owner, this.targetPosition, this.anticipationtWaitTime, true, true);
					yield return this.ACT_BLINK.waitForCompletion;
				}
				if (this.shouldSpin)
				{
					o.currentMeleeAttack = o.meleeStompAttack;
					o.Amanecidas.AnimatorInyector.SetMeleeHold(true);
					o.Amanecidas.AnimatorInyector.PlayStompAttack(true);
					o.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				}
				for (int i = 0; i < this.numLances; i++)
				{
					float portion = (float)i / (float)(this.numLances - 1);
					Vector2 target = Vector2.Lerp(this.originPoint, this.endPoint, portion);
					this.ACT_METHODLAUNCH_VECTOR.StartAction(this.owner, this.setLanceMethod, target);
					yield return this.ACT_METHODLAUNCH_VECTOR.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, this.timeBetweenLances);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				if (this.shouldSpin)
				{
					o.Amanecidas.AnimatorInyector.SetMeleeHold(false);
					yield return new AmanecidasBehaviour.WaitUntilIdle(o, 5f);
					o.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				}
				if (this.afterEndReachedWaitTime > 0f)
				{
					this.ACT_WAIT.StartAction(this.owner, this.afterEndReachedWaitTime);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				this.ACT_METHODLAUNCH.StartAction(this.owner, this.activateLancesMethod);
				yield return this.ACT_METHODLAUNCH.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int numLances;

			private Vector2 originPoint;

			private Vector2 endPoint;

			private Vector2 targetPosition;

			private Action<Vector2> setLanceMethod;

			private Action activateLancesMethod;

			private float anticipationtWaitTime;

			private float afterEndReachedWaitTime;

			private float timeBetweenLances;

			private bool shouldSpin;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private LaunchMethod_EnemyAction ACT_METHODLAUNCH = new LaunchMethod_EnemyAction();

			private LaunchMethodWithVector_EnemyAction ACT_METHODLAUNCH_VECTOR = new LaunchMethodWithVector_EnemyAction();
		}

		public class DoubleShootFrozenLances_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int totalNumLances, Vector2 targetPosition, Vector2 firstOriginPoint, Vector2 firstEndPoint, Vector2 secondOriginPoint, Vector2 secondEndPoint, Action<Vector2> setLanceMethod, Action activateLancesMethod, float anticipationtWaitTime, float afterEndReachedWaitTime, float timeBetweenLances = 0.1f, bool shouldSpin = false)
			{
				this.totalNumLances = totalNumLances;
				this.firstOriginPoint = firstOriginPoint;
				this.firstEndPoint = firstEndPoint;
				this.secondOriginPoint = secondOriginPoint;
				this.secondEndPoint = secondEndPoint;
				this.targetPosition = targetPosition;
				this.setLanceMethod = setLanceMethod;
				this.activateLancesMethod = activateLancesMethod;
				this.anticipationtWaitTime = anticipationtWaitTime;
				this.afterEndReachedWaitTime = afterEndReachedWaitTime;
				this.timeBetweenLances = timeBetweenLances;
				this.shouldSpin = shouldSpin;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_SHOOT_1.StopAction();
				this.ACT_SHOOT_2.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				this.ACT_SHOOT_1.StartAction(this.owner, this.totalNumLances / 2, this.firstOriginPoint, this.firstEndPoint, this.targetPosition, this.setLanceMethod, this.activateLancesMethod, this.anticipationtWaitTime, this.afterEndReachedWaitTime, this.timeBetweenLances, this.shouldSpin);
				this.ACT_SHOOT_2.StartAction(this.owner, this.totalNumLances / 2, this.secondOriginPoint, this.secondEndPoint, this.targetPosition, this.setLanceMethod, this.activateLancesMethod, this.anticipationtWaitTime, this.afterEndReachedWaitTime, this.timeBetweenLances, this.shouldSpin);
				yield return this.ACT_SHOOT_1.waitForCompletion;
				yield return this.ACT_SHOOT_2.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int totalNumLances;

			private Vector2 targetPosition;

			private Vector2 firstOriginPoint;

			private Vector2 firstEndPoint;

			private Vector2 secondOriginPoint;

			private Vector2 secondEndPoint;

			private Action<Vector2> setLanceMethod;

			private Action activateLancesMethod;

			private float anticipationtWaitTime;

			private float afterEndReachedWaitTime;

			private float timeBetweenLances;

			private bool shouldSpin;

			private AmanecidasBehaviour.ShootFrozenLances_EnemyAction ACT_SHOOT_1 = new AmanecidasBehaviour.ShootFrozenLances_EnemyAction();

			private AmanecidasBehaviour.ShootFrozenLances_EnemyAction ACT_SHOOT_2 = new AmanecidasBehaviour.ShootFrozenLances_EnemyAction();
		}

		public class FreezeTimeBlinkShots_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int numBullets, float waitTime, bool doRandomDisplacement, Action setBulletMethod, Action activateMethod)
			{
				this.numBullets = numBullets;
				this.waitTime = waitTime;
				this.doRandomDisplacement = doRandomDisplacement;
				this.setBulletMethod = setBulletMethod;
				this.activateMethod = activateMethod;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_ACTIVATE.StopAction();
				this.ACT_SETBULLET.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				Vector2 pointA = o.battleBounds.center + Vector2.left * 5f + Vector2.up * 3.5f;
				Vector2 pointB = pointA + Vector2.right * 10f;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				anim.SetBlink(true);
				o.SetGhostTrail(false);
				this.ACT_WAIT.StartAction(this.owner, 1f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector2 target;
				for (int i = 0; i < this.numBullets; i++)
				{
					target = Vector2.Lerp(pointA, pointB, (float)i / ((float)this.numBullets - 1f));
					if (this.doRandomDisplacement)
					{
						target += Vector2.up * (float)Random.Range(-1, 1) * 0.8f;
					}
					this.ACT_MOVE.StartAction(this.owner, target, 0.15f, 7, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					this.ACT_SETBULLET.StartAction(this.owner, this.setBulletMethod);
					yield return this.ACT_SETBULLET.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, this.waitTime);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				target = Vector2.Lerp(pointA, pointB, 0.5f) + new Vector2(-0.5f, 1.75f);
				this.ACT_MOVE.StartAction(this.owner, target, 0.5f, 7, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				anim.SetBlink(false);
				o.SetGhostTrail(true);
				o.ClearRotationAndFlip();
				this.ACT_ACTIVATE.StartAction(this.owner, this.activateMethod);
				yield return this.ACT_ACTIVATE.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private int numBullets;

			private float waitTime;

			private bool doRandomDisplacement;

			private Action setBulletMethod;

			private Action activateMethod;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private LaunchMethod_EnemyAction ACT_SETBULLET = new LaunchMethod_EnemyAction();

			private LaunchMethod_EnemyAction ACT_ACTIVATE = new LaunchMethod_EnemyAction();
		}

		public class FreezeTimeMultiShots_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int numJumps, float waitTime, Action setBulletMethod, Action activateMethod)
			{
				this.numJumps = numJumps;
				this.waitTime = waitTime;
				this.setBulletMethod = setBulletMethod;
				this.activateMethod = activateMethod;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_BLINK.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_ACTIVATE.StopAction();
				this.ACT_SETBULLET.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				this.activateMethod();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				Penitent p = Core.Logic.Penitent;
				List<Vector2> points = new List<Vector2>();
				float startX = (p.transform.position.x <= o.battleBounds.center.x) ? o.battleBounds.xMax : o.battleBounds.xMin;
				float endX = (p.transform.position.x <= o.battleBounds.center.x) ? (o.battleBounds.xMin + 2f) : (o.battleBounds.xMax - 2f);
				float startY = o.battleBounds.yMin + 1f;
				float endY = o.battleBounds.yMax + 1f;
				for (int j = 0; j < this.numJumps; j++)
				{
					Vector2 zero = Vector2.zero;
					zero.x = Mathf.Lerp(startX, endX, (float)j / ((float)this.numJumps - 1f));
					zero.y = Mathf.Lerp(startY, endY, (float)j / ((float)this.numJumps - 1f));
					points.Add(zero);
				}
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				anim.SetBlink(false);
				anim.SetBow(true);
				o.SetGhostTrail(false);
				for (int i = 0; i < this.numJumps; i++)
				{
					if (i % 2 == 0)
					{
						this.ACT_MOVE.StartAction(this.owner, points[i], this.waitTime, 6, null, true, new Action(o.AimToPenitentWithBow), true, true, 1.7f);
						yield return this.ACT_MOVE.waitForCompletion;
						anim.SetBow(false);
						o.SetGhostTrail(true);
						this.ACT_SETBULLET.StartAction(this.owner, this.setBulletMethod);
						yield return this.ACT_SETBULLET.waitForCompletion;
						o.LookAtPenitent(false);
						yield return new AmanecidasBehaviour.WaitUntilIdle(o, 1f);
					}
					else
					{
						Vector2 targetPoint = points[i];
						targetPoint.y = (points[i - 1].y + points[i + 1].y) / 2f;
						this.ACT_MOVE.StartAction(this.owner, targetPoint, this.waitTime, 5, null, true, null, true, true, 1.7f);
						yield return this.ACT_MOVE.waitForCompletion;
						if (i < this.numJumps - 1)
						{
							anim.SetBow(true);
							o.SetGhostTrail(false);
						}
						this.ACT_ACTIVATE.StartAction(this.owner, this.activateMethod);
						yield return this.ACT_ACTIVATE.waitForCompletion;
						o.LookAtPenitent(true);
						if (o.Amanecidas.IsLaudes && o.CanUseLaserShotAttack())
						{
							anim.SetBow(false);
							anim.SetBlink(false);
							o.SetGhostTrail(true);
							o.LookAtPenitent(true);
							o.ClearRotationAndFlip();
							o.Amanecidas.SetOrientation(o.Amanecidas.Status.Orientation, true, false);
							o.SetExtraRecoverySeconds(-0.5f);
							break;
						}
					}
					this.ACT_ACTIVATE.StartAction(this.owner, this.activateMethod);
					yield return this.ACT_ACTIVATE.waitForCompletion;
					o.ClearRotationAndFlip();
					o.Amanecidas.SetOrientation(o.Amanecidas.Status.Orientation, true, false);
				}
				base.FinishAction();
				yield break;
			}

			private int numJumps;

			private float waitTime;

			private Action setBulletMethod;

			private Action activateMethod;

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private LaunchMethod_EnemyAction ACT_SETBULLET = new LaunchMethod_EnemyAction();

			private LaunchMethod_EnemyAction ACT_ACTIVATE = new LaunchMethod_EnemyAction();
		}

		public class FastShot_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float waitTime, BossStraightProjectileAttack attack)
			{
				this.waitTime = waitTime;
				this.attack = attack;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_KEEPDISTANCE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE_AGENT.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				Penitent p = Core.Logic.Penitent;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				anim.SetBlink(false);
				anim.SetBow(false);
				o.LookAtPenitent(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(o, 1f);
				anim.SetBow(true);
				o.SetGhostTrail(false);
				Vector2 pPos = p.transform.position;
				Vector2 newPos = o.transform.position + o.GetDirToPenitent(o.transform.position).normalized;
				if (pPos.y > o.transform.position.y - 1f)
				{
					newPos = new Vector2(o.transform.position.x, pPos.y);
				}
				this.ACT_MOVE_AGENT.StartAction(o, o.agent, newPos, 2f);
				yield return this.ACT_MOVE_AGENT.waitForCompletion;
				Vector2 dir = o.GetDirToPenitent(o.transform.position);
				o.AimToPointWithBow(o.transform.position + dir);
				this.ACT_WAIT.StartAction(this.owner, this.waitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				BulletTimeProjectile arrow = (BulletTimeProjectile)this.attack.Shoot(dir, Vector2.up * 1.25f, 1f);
				arrow.Accelerate(5.2f);
				o.Amanecidas.Audio.PlayArrowFire_AUDIO();
				anim.SetBow(false);
				o.SetGhostTrail(true);
				this.ACT_WAIT.StartAction(this.owner, 0.12f);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_MOVE.StartAction(this.owner, o.transform.position - dir.normalized * 1.5f, 0.2f, 27, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				o.ClearRotationAndFlip();
				o.Amanecidas.SetOrientation(o.Amanecidas.Status.Orientation, true, false);
				o.LookAtPenitent(false);
				base.FinishAction();
				yield break;
			}

			private float waitTime;

			private BossStraightProjectileAttack attack;

			private AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction ACT_KEEPDISTANCE = new AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_AGENT = new MoveToPointUsingAgent_EnemyAction();
		}

		public class KeepDistanceFromTPOUsingAgent_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float seconds, bool driftHoriontally, bool driftVertically, bool clampHorizontally = false, float minHorizontalPos = 0f, float maxHorizontalPos = 0f, bool clampVertically = false, float minVerticalPos = 0f, float maxVerticalPos = 0f)
			{
				this.seconds = seconds;
				this.driftHoriontally = driftHoriontally;
				this.driftVertically = driftVertically;
				this.clampHorizontally = clampHorizontally;
				this.minHorizontalPos = minHorizontalPos;
				this.maxHorizontalPos = maxHorizontalPos;
				this.clampVertically = clampVertically;
				this.minVerticalPos = minVerticalPos;
				this.maxVerticalPos = maxVerticalPos;
				this.forceOffset = false;
				return base.StartAction(e);
			}

			public EnemyAction StartAction(EnemyBehaviour e, float seconds, bool driftHoriontally, bool driftVertically, float horizontalOffset = 0f, float verticalOffset = 0f)
			{
				this.seconds = seconds;
				this.driftHoriontally = driftHoriontally;
				this.driftVertically = driftVertically;
				this.clampHorizontally = false;
				this.clampVertically = false;
				this.forceOffset = true;
				this.horizontalOffset = horizontalOffset;
				this.verticalOffset = verticalOffset;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				AmanecidasBehaviour component = this.owner.GetComponent<AmanecidasBehaviour>();
				component.agent.SetConfig(component.actionConfig);
				component.agent.enabled = false;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				Penitent p = Core.Logic.Penitent;
				if (!this.forceOffset)
				{
					this.horizontalOffset = this.owner.transform.position.x - p.GetPosition().x;
					this.verticalOffset = this.owner.transform.position.y - p.GetPosition().y;
				}
				AmanecidasBehaviour ama = this.owner.GetComponent<AmanecidasBehaviour>();
				Arrive arrive = ama.agent.GetComponent<Arrive>();
				ama.agent.enabled = true;
				ama.agent.SetConfig(ama.keepDistanceConfig);
				float counter = 0f;
				float timeStep = 0.033333335f;
				while (counter < this.seconds)
				{
					Vector2 targetPoint = this.owner.transform.position;
					if (this.driftHoriontally)
					{
						targetPoint.x = this.horizontalOffset + p.GetPosition().x;
						if (this.clampHorizontally)
						{
							targetPoint.x = Mathf.Clamp(targetPoint.x, this.minHorizontalPos, this.maxHorizontalPos);
							float num = Mathf.Clamp(ama.transform.position.x, this.minHorizontalPos, this.maxHorizontalPos);
							ama.transform.position = new Vector2(num, ama.transform.position.y);
						}
					}
					if (this.driftVertically)
					{
						targetPoint.y = this.verticalOffset + p.GetPosition().y;
						if (this.clampVertically)
						{
							targetPoint.y = Mathf.Clamp(targetPoint.y, this.minVerticalPos, this.maxVerticalPos);
							float num2 = Mathf.Clamp(ama.transform.position.y, this.minVerticalPos, this.maxVerticalPos);
							ama.transform.position = new Vector2(ama.transform.position.x, num2);
						}
					}
					arrive.target = targetPoint;
					counter += timeStep;
					yield return new WaitForSeconds(timeStep);
				}
				ama.agent.enabled = false;
				ama.agent.SetConfig(ama.actionConfig);
				base.FinishAction();
				yield break;
			}

			private float seconds;

			private bool driftHoriontally;

			private bool driftVertically;

			private bool clampHorizontally;

			private float minHorizontalPos;

			private float maxHorizontalPos;

			private bool clampVertically;

			private float minVerticalPos;

			private float maxVerticalPos;

			private bool forceOffset;

			private float horizontalOffset;

			private float verticalOffset;
		}

		public class KeepDistanceFromAmanecidaUsingAgent_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, AmanecidasBehaviour ama, float seconds, float horizontalOffset, float verticalOffset)
			{
				this.ama = ama;
				this.seconds = seconds;
				this.horizontalOffset = horizontalOffset;
				this.verticalOffset = verticalOffset;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidaAxeBehaviour axe = this.owner.GetComponent<AmanecidaAxeBehaviour>();
				axe.ActivateAgent(true);
				float counter = 0f;
				float timeStep = 0.033333335f;
				while (counter < this.seconds)
				{
					Vector2 targetPoint = this.ama.transform.position;
					targetPoint.x += this.horizontalOffset;
					targetPoint.y += this.verticalOffset;
					axe.SeekTarget(targetPoint);
					counter += timeStep;
					yield return new WaitForSeconds(timeStep);
				}
				axe.ActivateAgent(false);
				base.FinishAction();
				yield break;
			}

			private AmanecidasBehaviour ama;

			private float seconds;

			private float horizontalOffset;

			private float verticalOffset;
		}

		public class FastShots_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, float waitTime, int numShots, BossStraightProjectileAttack attack)
			{
				this.waitTime = waitTime;
				this.numShots = numShots;
				this.attack = attack;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_KEEPDISTANCE.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE_AGENT.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				Penitent p = Core.Logic.Penitent;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				anim.SetBlink(false);
				o.SetGhostTrail(false);
				for (int i = 0; i < this.numShots; i++)
				{
					anim.SetBow(true);
					Vector2 pPos = p.transform.position;
					Vector2 newPos = o.transform.position + o.GetDirToPenitent(o.transform.position).normalized;
					if (pPos.y > o.transform.position.y - 1f)
					{
						newPos = new Vector2(o.transform.position.x, pPos.y);
					}
					this.ACT_MOVE_AGENT.StartAction(o, o.agent, newPos, 2f);
					yield return this.ACT_MOVE_AGENT.waitForCompletion;
					Vector2 dir = o.GetDirToPenitent(o.transform.position);
					o.AimToPointWithBow(o.transform.position + dir);
					this.ACT_WAIT.StartAction(this.owner, this.waitTime);
					yield return this.ACT_WAIT.waitForCompletion;
					BulletTimeProjectile arrow = (BulletTimeProjectile)this.attack.Shoot(dir, Vector2.up, 1f);
					arrow.Accelerate(1.1f);
					o.Amanecidas.Audio.PlayArrowFire_AUDIO();
					this.ACT_MOVE.StartAction(this.owner, o.transform.position - dir.normalized * 0.8f, 0.1f, 18, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
					anim.SetBow(false);
					this.ACT_WAIT.StartAction(this.owner, 0.3f);
					yield return this.ACT_WAIT.waitForCompletion;
					if (o.throwbackExtraTime > 0f)
					{
						break;
					}
				}
				o.SetGhostTrail(true);
				o.ClearRotationAndFlip();
				o.Amanecidas.SetOrientation(o.Amanecidas.Status.Orientation, true, false);
				o.LookAtPenitent(false);
				base.FinishAction();
				yield break;
			}

			private float waitTime;

			private int numShots;

			private BossStraightProjectileAttack attack;

			private AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction ACT_KEEPDISTANCE = new AmanecidasBehaviour.KeepDistanceFromTPOUsingAgent_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_AGENT = new MoveToPointUsingAgent_EnemyAction();
		}

		public class ChargedShot_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Action chargeMethod, Action releaseMethod, float anticipationtWaitTime, float recoveryTime)
			{
				this.chargeMethod = chargeMethod;
				this.releaseMethod = releaseMethod;
				this.anticipationtWaitTime = anticipationtWaitTime;
				this.recoveryTime = recoveryTime;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_TIRED.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_METHODLAUNCH.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				Penitent p = Core.Logic.Penitent;
				Vector2 target = Vector2.zero;
				if (p.GetPosition().x > o.battleBounds.center.x)
				{
					target = new Vector2(o.battleBounds.xMax + 0.5f, o.battleBounds.yMin + 0.2f);
				}
				else
				{
					target = new Vector2(o.battleBounds.xMin - 0.5f, o.battleBounds.yMin + 0.2f);
				}
				this.ACT_MOVE.StartAction(this.owner, target, 0.1f, 8, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				o.ClearRotationAndFlip();
				anim.SetBow(true);
				o.SetGhostTrail(false);
				Vector2 dir = (o.transform.position.x <= o.battleBounds.center.x) ? Vector2.right : Vector2.left;
				o.AimToPointWithBow(o.transform.position + dir);
				o.PlayChargeEnergy(this.anticipationtWaitTime - 1f, false, true);
				this.ACT_METHODLAUNCH.StartAction(this.owner, this.chargeMethod);
				yield return this.ACT_METHODLAUNCH.waitForCompletion;
				this.ACT_WAIT.StartAction(this.owner, this.anticipationtWaitTime);
				yield return this.ACT_WAIT.waitForCompletion;
				anim.SetBow(false);
				this.ACT_METHODLAUNCH.StartAction(this.owner, this.releaseMethod);
				yield return this.ACT_METHODLAUNCH.waitForCompletion;
				o.ClearRotationAndFlip();
				o.LookAtDirUsingOrientation(dir);
				o.SetGhostTrail(true);
				this.ACT_TIRED.StartAction(this.owner, this.recoveryTime, true);
				yield return this.ACT_TIRED.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private Action chargeMethod;

			private Action releaseMethod;

			private float anticipationtWaitTime;

			private float recoveryTime;

			private AmanecidasBehaviour.TiredPeriod_EnemyAction ACT_TIRED = new AmanecidasBehaviour.TiredPeriod_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private LaunchMethod_EnemyAction ACT_METHODLAUNCH = new LaunchMethod_EnemyAction();
		}

		public class SpikesBlinkShots_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int numBlinks, float anticipationtWaitTime, float blinksWaitTime, Action shootMethod)
			{
				this.numBlinks = numBlinks;
				this.anticipationtWaitTime = anticipationtWaitTime;
				this.blinksWaitTime = blinksWaitTime;
				this.shootMethod = shootMethod;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE_CHARACTER.StopAction();
				this.ACT_METHODLAUNCH.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector animatorInyector = amanecidasBehaviour.Amanecidas.AnimatorInyector;
				animatorInyector.SetBow(false);
				animatorInyector.SetBlink(false);
				amanecidasBehaviour.SetGhostTrail(true);
				amanecidasBehaviour.LookAtPenitent(true);
				amanecidasBehaviour.ClearRotationAndFlip();
				amanecidasBehaviour.Amanecidas.SetOrientation(amanecidasBehaviour.Amanecidas.Status.Orientation, true, false);
				amanecidasBehaviour.verticalSlowBlastArrowAttack.ClearAll();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour o = this.owner as AmanecidasBehaviour;
				AmanecidasAnimatorInyector anim = o.Amanecidas.AnimatorInyector;
				float minX = o.battleBounds.center.x - o.battleBounds.width / 3f;
				float maxX = o.battleBounds.center.x + o.battleBounds.width / 3f;
				int indexOffset = Random.Range(0, this.numBlinks);
				anim.SetBow(false);
				anim.SetBlink(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(o, 1f);
				for (int i = 0; i < this.numBlinks; i++)
				{
					float lerpPercentage = (float)((i + indexOffset) * 2 % this.numBlinks) / (float)(this.numBlinks - 1);
					float targetX = Mathf.Lerp(minX, maxX, lerpPercentage);
					targetX += Random.Range(-0.5f, 0.5f);
					Vector2 target = new Vector2(targetX, o.battleBounds.yMax);
					if (i == 0)
					{
						this.ACT_MOVE_CHARACTER.StartAction(this.owner, o.agent, target, 2f);
						yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
					}
					else
					{
						this.ACT_MOVE.StartAction(this.owner, target, 0.2f, 8, null, true, null, true, true, 1.7f);
						yield return this.ACT_MOVE.waitForCompletion;
					}
					anim.SetBow(true);
					anim.SetBlink(false);
					o.SetGhostTrail(false);
					o.ClearRotationAndFlip();
					o.AimToPointWithBow(o.transform.position + Vector2.down);
					if (i == 0)
					{
						Vector2 vector;
						vector..ctor(o.battleBounds.xMin - 0.5f, o.battleBounds.yMin);
						Vector2 vector2;
						vector2..ctor(o.battleBounds.xMax + 0.5f, o.battleBounds.yMin);
						o.verticalSlowBlastArrowAttack.SummonAreaOnPoint(vector, 270f, 1f, null);
						o.verticalSlowBlastArrowAttack.SummonAreaOnPoint(vector2, 270f, 1f, null);
					}
					o.Amanecidas.Audio.PlayVerticalPreattack_AUDIO();
					this.ACT_WAIT.StartAction(this.owner, this.anticipationtWaitTime);
					yield return this.ACT_WAIT.waitForCompletion;
					anim.SetBow(false);
					anim.SetBlink(true);
					this.ACT_METHODLAUNCH.StartAction(this.owner, this.shootMethod);
					yield return this.ACT_METHODLAUNCH.waitForCompletion;
					this.ACT_WAIT.StartAction(this.owner, this.blinksWaitTime);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				o.verticalSlowBlastArrowAttack.ClearAll();
				anim.SetBlink(false);
				o.ClearRotationAndFlip();
				o.LookAtPenitent(true);
				o.Amanecidas.SetOrientation(o.Amanecidas.Status.Orientation, true, false);
				o.SetGhostTrail(true);
				base.FinishAction();
				yield break;
			}

			private int numBlinks;

			private float anticipationtWaitTime;

			private float blinksWaitTime;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();

			private LaunchMethod_EnemyAction ACT_METHODLAUNCH = new LaunchMethod_EnemyAction();

			private Action shootMethod;
		}

		public class FreezeTimeNRicochetShots_EnemyComboAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int numBullets, float freezeTimeWaitTime, bool doRandomDisplacement, Action setBulletMethod, Action activateMethod, int numBounces, Action<Vector2, Vector2> showArrowTrailMethod, Action<Vector2, Vector2> shootArrowMethod, float ricochetWaitTime, LayerMask mask)
			{
				this.numBullets = numBullets;
				this.freezeTimeWaitTime = freezeTimeWaitTime;
				this.doRandomDisplacement = doRandomDisplacement;
				this.setBulletMethod = setBulletMethod;
				this.activateMethod = activateMethod;
				this.numBounces = numBounces;
				this.showArrowTrailMethod = showArrowTrailMethod;
				this.shootArrowMethod = shootArrowMethod;
				this.ricochetWaitTime = ricochetWaitTime;
				this.mask = mask;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_FREEZE.StopAction();
				this.ACT_RICOCHET_1.StopAction();
				this.ACT_RICOCHET_2.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				this.ACT_FREEZE.StartAction(this.owner, this.numBullets, this.freezeTimeWaitTime, this.doRandomDisplacement, this.setBulletMethod, delegate()
				{
				});
				yield return this.ACT_FREEZE.waitForCompletion;
				this.ACT_RICOCHET_1.StartAction(this.owner, this.numBounces, this.showArrowTrailMethod, this.shootArrowMethod, this.ricochetWaitTime, this.mask, true, false);
				this.ACT_RICOCHET_2.StartAction(this.owner, this.numBounces, this.showArrowTrailMethod, this.shootArrowMethod, this.ricochetWaitTime, this.mask, true, true);
				yield return this.ACT_RICOCHET_1.waitForCompletion;
				yield return this.ACT_RICOCHET_2.waitForCompletion;
				this.activateMethod();
				base.FinishAction();
				yield break;
			}

			private int numBullets;

			private bool doRandomDisplacement;

			private float freezeTimeWaitTime;

			private Action setBulletMethod;

			private Action activateMethod;

			private int numBounces;

			private float ricochetWaitTime;

			private LayerMask mask;

			private Action<Vector2, Vector2> showArrowTrailMethod;

			private Action<Vector2, Vector2> shootArrowMethod;

			private AmanecidasBehaviour.FreezeTimeBlinkShots_EnemyAction ACT_FREEZE = new AmanecidasBehaviour.FreezeTimeBlinkShots_EnemyAction();

			private AmanecidasBehaviour.ShootRicochetArrow_EnemyAction ACT_RICOCHET_1 = new AmanecidasBehaviour.ShootRicochetArrow_EnemyAction();

			private AmanecidasBehaviour.ShootRicochetArrow_EnemyAction ACT_RICOCHET_2 = new AmanecidasBehaviour.ShootRicochetArrow_EnemyAction();
		}

		public class MultiStompNLavaBalls_EnemyComboAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Action jumpMethod, int numJumps)
			{
				this.jumpMethod = jumpMethod;
				this.numJumps = numJumps;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_STOMP.StopAction();
				this.ACT_BLINK.StopAction();
				this.ACT_WAIT.StopAction();
				this.ACT_CALLAXE1.StopAction();
				this.ACT_CALLAXE2.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				amanecidasBehaviour.ShowBothAxes(false);
				amanecidasBehaviour.currentMeleeAttack = amanecidasBehaviour.meleeAxeAttack;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				this.ACT_BLINK.StartAction(this.owner, ama.battleBounds.center + Vector2.up * 1.4f, 0.2f, true, false);
				yield return this.ACT_BLINK.waitForCompletion;
				bool recallingAxes = false;
				if (!ama.IsWieldingAxe())
				{
					this.ACT_CALLAXE1.StartAction(this.owner, this.owner.transform.position + Vector3.up * 1.75f, ama.axes[0], 0.1f, 0.3f);
					this.ACT_CALLAXE2.StartAction(this.owner, this.owner.transform.position + Vector3.up * 1.75f, ama.axes[1], 0.1f, 0.3f);
					recallingAxes = true;
				}
				ama.currentMeleeAttack = ama.meleeStompAttack;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				ama.Amanecidas.AnimatorInyector.PlayStompAttack(false);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				ama.Amanecidas.Audio.PlayAttackCharge_AUDIO();
				ama.PlayChargeEnergy(0.4f, false, false);
				this.ACT_WAIT.StartAction(this.owner, 1f);
				if (recallingAxes)
				{
					yield return this.ACT_CALLAXE1.waitForCompletion;
					yield return this.ACT_CALLAXE2.waitForCompletion;
				}
				ama.ShowBothAxes(false);
				ama.SetGhostTrail(false);
				yield return this.ACT_WAIT.waitForCompletion;
				this.ACT_STOMP.StartAction(ama, this.numJumps, 0.3f, this.jumpMethod, false, false, true, true, 3f);
				yield return this.ACT_STOMP.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				ama.currentMeleeAttack = ama.meleeAxeAttack;
				ama.SetGhostTrail(true);
				this.ACT_WAIT.StartAction(this.owner, 0.3f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private Action jumpMethod;

			private int numJumps;

			private AmanecidasBehaviour.MultiStompAttack_EnemyAction ACT_STOMP = new AmanecidasBehaviour.MultiStompAttack_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE1 = new AmanecidasBehaviour.CallAxe_EnemyAction();

			private AmanecidasBehaviour.CallAxe_EnemyAction ACT_CALLAXE2 = new AmanecidasBehaviour.CallAxe_EnemyAction();
		}

		public class MoveBattleBounds_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, Vector2 direction, float transitionTime, AmanecidaArena.WEAPON_FIGHT_PHASE fightPhase)
			{
				this.direction = direction;
				this.transitionTime = transitionTime;
				this.fightPhase = fightPhase;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_MOVE_CHARACTER.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				this.ACT_MOVE_CHARACTER.StartAction(this.owner, ama.agent, ama.battleBounds.center + Vector2.up * 1.5f, 2f);
				yield return this.ACT_MOVE_CHARACTER.waitForCompletion;
				ama.currentMeleeAttack = ama.meleeStompAttack;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				ama.Amanecidas.AnimatorInyector.PlayStompAttack(false);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				this.ACT_WAIT.StartAction(this.owner, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				Vector2 targetPos = ama.battleBounds.center;
				targetPos.y += this.direction.y + 2f;
				ama.Amanecidas.LaudesArena.ActivateGameObjectsByWeaponFightPhase(ama.currentWeapon, this.fightPhase);
				this.ACT_MOVE.StartAction(this.owner, targetPos, 5f, 22, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 1f);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				ama.currentMeleeAttack = ama.meleeFalcataAttack;
				AmanecidasBehaviour amanecidasBehaviour = ama;
				amanecidasBehaviour.battleBounds.center = amanecidasBehaviour.battleBounds.center + this.direction;
				this.ACT_WAIT.StartAction(this.owner, this.transitionTime);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private Vector2 direction;

			private float transitionTime;

			private AmanecidaArena.WEAPON_FIGHT_PHASE fightPhase;

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private MoveToPointUsingAgent_EnemyAction ACT_MOVE_CHARACTER = new MoveToPointUsingAgent_EnemyAction();
		}

		public class FalcataSlashStorm_EnemyComboAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int n)
			{
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				amanecidasBehaviour.currentMeleeAttack = amanecidasBehaviour.meleeFalcataAttack;
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				int i = 20;
				this.ACT_BLINK.StartAction(this.owner, ama.GetPointBelow(ama.battleBounds.center, true, 100f), 0.1f, true, false);
				yield return this.ACT_BLINK.waitForCompletion;
				ama.currentMeleeAttack = ama.meleeStompAttack;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				ama.Amanecidas.AnimatorInyector.PlayStompAttack(false);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				ama.PlayChargeEnergy(1f, false, true);
				this.ACT_WAIT.StartAction(this.owner, 1.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				float timeBetweenProjectiles = 0.4f;
				float distance = 5f;
				float seconds = (float)i * timeBetweenProjectiles;
				TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(ama.transform, ama.transform.position.y + distance, seconds, false), 10);
				Vector2 dir = Vector2.up;
				Quaternion rot = Quaternion.Euler(0f, 0f, -15f);
				Quaternion rotQuarter = Quaternion.Euler(0f, 0f, -90f);
				Quaternion rotHalf = Quaternion.Euler(0f, 0f, -180f);
				Quaternion rotThreeQuarter = Quaternion.Euler(0f, 0f, -270f);
				for (int j = 0; j < i; j++)
				{
					ama.falcataSlashProjectileAttack.Shoot(dir);
					ama.falcataSlashProjectileAttack.Shoot(rotHalf * dir);
					dir = rot * dir;
					this.ACT_WAIT.StartAction(this.owner, timeBetweenProjectiles);
					yield return this.ACT_WAIT.waitForCompletion;
				}
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				ama.currentMeleeAttack = ama.meleeFalcataAttack;
				this.ACT_WAIT.StartAction(this.owner, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();
		}

		public class FalcataSlashOnFloor_EnemyAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int repetitions)
			{
				this.numberOfLoops = repetitions;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeAnticipation(false);
				amanecidasBehaviour.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				amanecidasBehaviour.currentMeleeAttack = amanecidasBehaviour.meleeFalcataAttack;
				base.DoOnStop();
			}

			private void LaunchProjectiles()
			{
				AmanecidasBehaviour amanecidasBehaviour = this.owner as AmanecidasBehaviour;
				Vector2 right = Vector2.right;
				Quaternion quaternion = Quaternion.Euler(0f, 0f, -90f);
				Quaternion quaternion2 = Quaternion.Euler(0f, 0f, -180f);
				Quaternion quaternion3 = Quaternion.Euler(0f, 0f, -270f);
				Vector2 offset = Vector2.up * 0.5f;
				amanecidasBehaviour.falcataSlashProjectileAttack.Shoot(right, offset, 1f);
				amanecidasBehaviour.falcataSlashProjectileAttack.Shoot(quaternion * right, offset, 1f);
				amanecidasBehaviour.falcataSlashProjectileAttack.Shoot(quaternion2 * right, offset, 1f);
				amanecidasBehaviour.falcataSlashProjectileAttack.Shoot(quaternion3 * right, offset, 1f);
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour ama = this.owner as AmanecidasBehaviour;
				float distanceFromCenter = 5f;
				this.ACT_BLINK.StartAction(this.owner, ama.GetPointBelow(ama.battleBounds.center + Vector2.right * distanceFromCenter, true, 100f), 0.1f, true, false);
				yield return this.ACT_BLINK.waitForCompletion;
				ama.currentMeleeAttack = ama.meleeStompAttack;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(true);
				ama.Amanecidas.AnimatorInyector.PlayStompAttack(true);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(false);
				this.ACT_WAIT.StartAction(this.owner, 1.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				float singleLoopTime = 1.5f;
				float seconds = (float)this.numberOfLoops * singleLoopTime;
				TweenSettingsExtensions.OnStepComplete<Tweener>(TweenSettingsExtensions.SetLoops<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(ama.transform, ama.transform.position.x - distanceFromCenter * 2f, singleLoopTime, false), 10), this.numberOfLoops, 1), delegate()
				{
					this.LaunchProjectiles();
				});
				this.ACT_WAIT.StartAction(this.owner, seconds * 0.98f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.Amanecidas.AnimatorInyector.SetMeleeHold(false);
				yield return new AmanecidasBehaviour.WaitUntilIdle(ama, 5f);
				ama.Amanecidas.AnimatorInyector.SetWeaponVisible(true);
				this.ACT_WAIT.StartAction(this.owner, 0.5f);
				yield return this.ACT_WAIT.waitForCompletion;
				ama.currentMeleeAttack = ama.meleeFalcataAttack;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private AmanecidasBehaviour.BlinkToPoint_EnemyAction ACT_BLINK = new AmanecidasBehaviour.BlinkToPoint_EnemyAction();

			private int numberOfLoops = 2;
		}

		public class FreezeTimeNHorizontalDashes_EnemyComboAction : EnemyAction
		{
			public EnemyAction StartAction(EnemyBehaviour e, int numLances, Vector2 originPoint, Vector2 endPoint, Vector2 targetPosition, Action<Vector2> setLanceMethod, Action activateLancesMethod, float anticipationtWaitTime, float afterEndReachedWaitTime, bool skipOne, int numDashes, float delay, bool startDashesAwayFromPenitent)
			{
				this.numLances = numLances;
				this.originPoint = originPoint;
				this.endPoint = endPoint;
				this.targetPosition = targetPosition;
				this.setLanceMethod = setLanceMethod;
				this.activateLancesMethod = activateLancesMethod;
				this.anticipationtWaitTime = anticipationtWaitTime;
				this.afterEndReachedWaitTime = afterEndReachedWaitTime;
				this.skipOne = skipOne;
				this.numDashes = numDashes;
				this.delay = delay;
				this.startDashesAwayFromPenitent = startDashesAwayFromPenitent;
				return base.StartAction(e);
			}

			protected override void DoOnStop()
			{
				this.ACT_LANCES.StopAction();
				this.ACT_DASHES.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				yield return null;
				base.FinishAction();
				yield break;
			}

			private int numLances;

			private Vector2 originPoint;

			private Vector2 endPoint;

			private Vector2 targetPosition;

			private Action<Vector2> setLanceMethod;

			private Action activateLancesMethod;

			private float anticipationtWaitTime;

			private float afterEndReachedWaitTime;

			private bool skipOne;

			private int numDashes;

			private float delay;

			private bool startDashesAwayFromPenitent;

			private AmanecidasBehaviour.ShootFrozenLances_EnemyAction ACT_LANCES = new AmanecidasBehaviour.ShootFrozenLances_EnemyAction();

			private AmanecidasBehaviour.HorizontalBlinkDashes_EnemyAction ACT_DASHES = new AmanecidasBehaviour.HorizontalBlinkDashes_EnemyAction();
		}

		public class Intro_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				this.ACT_AXES.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				AmanecidasBehaviour amanecida = this.owner as AmanecidasBehaviour;
				Vector2 introPointStart = amanecida.battleBounds.center + Vector2.left * 5f;
				introPointStart.y = Core.Logic.Penitent.transform.position.y;
				Vector2 introPointEnd = introPointStart + Vector2.up * 4f;
				Debug.DrawRay(introPointStart, introPointEnd, Color.green, 10f);
				if (amanecida.Amanecidas.IsLaudes)
				{
					float secondsFloating = 7f;
					introPointStart = amanecida.battleBounds.center + Vector2.left * 5f + Vector2.up * 5.5f;
					introPointEnd = introPointStart + Vector2.down * 7f;
					amanecida.transform.position = introPointStart;
					this.ACT_MOVE.StartAction(this.owner, introPointEnd, secondsFloating, 9, null, true, null, true, true, 1.7f);
					yield return this.ACT_MOVE.waitForCompletion;
				}
				else
				{
					amanecida.ShowSprites(false);
					amanecida.Amanecidas.AnimatorInyector.ActivateIntroColor();
					GameObject area = amanecida.introBeamAttack.SummonAreaOnPoint(introPointStart, 0f, 1f, null);
					area.GetComponentInChildren<SpriteRenderer>().material = amanecida.Amanecidas.AnimatorInyector.GetCurrentBeamMaterial();
					amanecida.transform.position = introPointStart;
					amanecida.SetGhostTrail(false);
					this.ACT_WAIT.StartAction(this.owner, 0.7f);
					yield return this.ACT_WAIT.waitForCompletion;
					amanecida.ShowSprites(true);
					float secondsFloating2 = 4f;
					this.ACT_MOVE.StartAction(this.owner, introPointEnd, secondsFloating2, 9, null, true, null, true, true, 1.7f);
					this.ACT_WAIT.StartAction(this.owner, 0.5f);
					yield return this.ACT_WAIT.waitForCompletion;
					amanecida.Amanecidas.AnimatorInyector.DeactivateIntroColor();
					area.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Before Player";
					this.ACT_WAIT.StartAction(this.owner, secondsFloating2 * 0.2f);
					yield return this.ACT_WAIT.waitForCompletion;
					amanecida.DoSummonWeaponAnimation();
					yield return this.ACT_MOVE.waitForCompletion;
				}
				amanecida.SetGhostTrail(true);
				amanecida.ShowBothAxes(false);
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();

			private AmanecidasBehaviour.ShowAxes_EnemyAction ACT_AXES = new AmanecidasBehaviour.ShowAxes_EnemyAction();
		}
	}
}
