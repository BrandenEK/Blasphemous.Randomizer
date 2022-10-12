using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Crisanta.AI;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI.Widgets;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using Tools.Level.Interactables;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.Crisanta
{
	public class CrisantaBehaviour : EnemyBehaviour
	{
		public Crisanta Crisanta { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<CrisantaBehaviour> OnActionFinished;

		public bool KeepRunningAnimation()
		{
			return this._keepRunningAnimation;
		}

		public bool AttackIfEnemyClose()
		{
			return this._attackIfEnemyClose;
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this.stIntro = new Crisanta_StIntro();
			this.stAction = new Crisanta_StAction();
			this.stDeath = new Crisanta_StDeath();
			this.stGuard = new Crisanta_StGuard();
			this._fsm = new StateMachine<CrisantaBehaviour>(this, this.stIntro, null, null);
			this.results = new RaycastHit2D[1];
			this.currentlyAvailableAttacks = new List<CrisantaBehaviour.Crisanta_ATTACKS>
			{
				CrisantaBehaviour.Crisanta_ATTACKS.COMBO_SLASHES_A,
				CrisantaBehaviour.Crisanta_ATTACKS.COMBO_BLINK_A
			};
			this.sparkParticles = this.particleParent.GetComponentInChildren<ParticleSystem>();
			this.sparkParticlesMid = this.particleParentMid.GetComponentInChildren<ParticleSystem>();
		}

		public override void OnStart()
		{
			base.OnStart();
			this.Crisanta = (Crisanta)this.Entity;
			this.heavyAttack.OnMeleeAttackGuarded += this.OnHeavyAttackGuarded;
			this.diagonalSlashDashAttack.OnDashBlockedEvent += this.OnDashBlocked;
			this.ChangeBossState(BOSS_STATES.WAITING);
			this.LookAtTarget(base.transform.position + Vector3.left);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
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
			this.comboActionsRemaining--;
			if (this.comboActionsRemaining == 0)
			{
				this.QueuedActionsPush(CrisantaBehaviour.Crisanta_ATTACKS.COMBO_REST);
			}
		}

		private void CancelAttacks()
		{
			this.lightAttack.damageOnEnterArea = false;
			this.heavyAttack.damageOnEnterArea = false;
		}

		private void CancelCombo()
		{
			this.Crisanta.AnimatorInyector.CancelAll();
			if (this.queuedActions != null)
			{
				this.queuedActions.Clear();
			}
			this.comboActionsRemaining = -1;
		}

		private void ActionFinished()
		{
			this.ChangeBossState(BOSS_STATES.AVAILABLE_FOR_ACTION);
			if (this.OnActionFinished != null)
			{
				this.OnActionFinished(this);
			}
		}

		public void LaunchAction(CrisantaBehaviour.Crisanta_ATTACKS atk)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"TIME: ",
				Time.time,
				" Launching action: ",
				atk.ToString()
			}));
			this.SetRecovering(false);
			switch (atk)
			{
			case CrisantaBehaviour.Crisanta_ATTACKS.UPWARDS_SLASH:
				this.IssueUpwardsSlash();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.DOWNWARDS_SLASH:
				this.IssueDownwardsSlash();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_LOW:
				this.IssueBackflip();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.LEFT_HORIZONTAL_BLINK:
				this.IssueLeftBlinkSlash();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.LEFT_DIAGONAL_BLINK:
				this.IssueLeftBlinkDiagonalSlash();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.COUNTER_BLINK:
				this.IssueDownwardsSlash();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.GUARD:
				this.IssueGuard();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.COMBO_SLASHES_A:
				this.IssueCombo(this.comboSlashesA);
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.COMBO_REST:
				this.Crisanta.AnimatorInyector.ComboMode(false);
				this.StartWaitingPeriod(this.comboRestTime);
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_HIGH:
				this.IssueHighBackflip();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.RIGHT_HORIZONTAL_BLINK:
				this.IssueRightBlinkSlash();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.RIGHT_DIAGONAL_BLINK:
				this.IssueRightBlinkDiagonalSlash();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.COMBO_SLASHES_B:
				this.IssueCombo(this.comboSlashesB);
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.COMBO_BLINK_A:
				this.IssueCombo(this.comboBlinkA);
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.COMBO_BLINK_B:
				this.IssueCombo(this.comboBlinkB);
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.UNSEAL:
				this.IssueUnseal();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.DEATH:
				this.DeathAction();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.FORWARD_FLIP:
				this.IssueEscapeAttack();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.INSTANT_GUARD:
				this.IssueInstantGuard();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.INSTANT_DOWNWARDS:
				this.IssueInstantDownwardsSlash();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.CORNER_SHOCKWAVES:
				this.IssueCornerShockwaves();
				break;
			case CrisantaBehaviour.Crisanta_ATTACKS.COMBO_BACKFLIP_SLASH_A:
				this.IssueCombo(this.comboBackflipSlashesA);
				break;
			}
			this.lastAttack = atk;
		}

		public CrisantaBehaviour.Crisanta_ATTACKS GetNewAttack()
		{
			if (this.queuedActions != null && this.queuedActions.Count > 0)
			{
				return this.QueuedActionsPop();
			}
			if (this._currentPhase == CrisantaBehaviour.Crisanta_PHASES.SECOND && !this.unveiled && !this.Crisanta.IsCrisantaRedux)
			{
				return CrisantaBehaviour.Crisanta_ATTACKS.UNSEAL;
			}
			CrisantaBehaviour.Crisanta_ATTACKS[] array = new CrisantaBehaviour.Crisanta_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<CrisantaBehaviour.Crisanta_ATTACKS> list = new List<CrisantaBehaviour.Crisanta_ATTACKS>(array);
			list.Remove(this.lastAttack);
			if (this.justParried)
			{
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.COMBO_SLASHES_A);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.COMBO_SLASHES_B);
				this.justParried = false;
			}
			if (this.GetDirToPenitent().magnitude > 5f)
			{
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_LOW);
			}
			if (this.comboBlinkA.Contains(this.lastAttack) || this.comboBlinkB.Contains(this.lastAttack))
			{
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.COMBO_BLINK_A);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.COMBO_BLINK_B);
			}
			if (this.lastAttack == CrisantaBehaviour.Crisanta_ATTACKS.DOWNWARDS_SLASH || this.lastAttack == CrisantaBehaviour.Crisanta_ATTACKS.INSTANT_DOWNWARDS)
			{
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.INSTANT_DOWNWARDS);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.INSTANT_GUARD);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.COMBO_SLASHES_A);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.COMBO_BACKFLIP_SLASH_A);
			}
			if (this.lastAttack == CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_HIGH || this.lastAttack == CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_LOW || this.lastAttack == CrisantaBehaviour.Crisanta_ATTACKS.FORWARD_FLIP)
			{
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_HIGH);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_LOW);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.FORWARD_FLIP);
			}
			if (this.Crisanta.IsCrisantaRedux && (this.lastAttack == CrisantaBehaviour.Crisanta_ATTACKS.LEFT_HORIZONTAL_BLINK || this.lastAttack == CrisantaBehaviour.Crisanta_ATTACKS.RIGHT_HORIZONTAL_BLINK || this.lastAttack == CrisantaBehaviour.Crisanta_ATTACKS.CORNER_SHOCKWAVES))
			{
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.LEFT_HORIZONTAL_BLINK);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.RIGHT_HORIZONTAL_BLINK);
				list.Remove(CrisantaBehaviour.Crisanta_ATTACKS.CORNER_SHOCKWAVES);
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public IEnumerator WaitForState(State<CrisantaBehaviour> st)
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

		private void QueuedActionsPush(CrisantaBehaviour.Crisanta_ATTACKS atk)
		{
			if (this.queuedActions == null)
			{
				this.queuedActions = new List<CrisantaBehaviour.Crisanta_ATTACKS>();
			}
			this.queuedActions.Add(atk);
		}

		private CrisantaBehaviour.Crisanta_ATTACKS QueuedActionsPop()
		{
			CrisantaBehaviour.Crisanta_ATTACKS crisanta_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(crisanta_ATTACKS);
			return crisanta_ATTACKS;
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public float GetHealthPercentage()
		{
			return this.Crisanta.CurrentLife / this.Crisanta.Stats.Life.Base;
		}

		private void SetPhase(CrisantaBehaviour.CrisantaPhases p)
		{
			this.currentlyAvailableAttacks = p.availableAttacks;
			this._currentPhase = p.phaseId;
		}

		private void ChangePhase(CrisantaBehaviour.Crisanta_PHASES p)
		{
			CrisantaBehaviour.CrisantaPhases phase = this.phases.Find((CrisantaBehaviour.CrisantaPhases x) => x.phaseId == p);
			this.SetPhase(phase);
		}

		private void CheckNextPhase()
		{
			float healthPercentage = this.GetHealthPercentage();
			CrisantaBehaviour.Crisanta_PHASES currentPhase = this._currentPhase;
			if (currentPhase != CrisantaBehaviour.Crisanta_PHASES.FIRST)
			{
				if (currentPhase != CrisantaBehaviour.Crisanta_PHASES.SECOND)
				{
					if (currentPhase != CrisantaBehaviour.Crisanta_PHASES.LAST)
					{
					}
				}
				else if (healthPercentage < 0.3f)
				{
					this.ChangePhase(CrisantaBehaviour.Crisanta_PHASES.LAST);
				}
			}
			else if (healthPercentage < 0.6f)
			{
				this.ChangePhase(CrisantaBehaviour.Crisanta_PHASES.SECOND);
			}
		}

		public void IssueCombo(List<CrisantaBehaviour.Crisanta_ATTACKS> testCombo)
		{
			for (int i = 0; i < testCombo.Count; i++)
			{
				this.QueuedActionsPush(testCombo[i]);
			}
			this.comboActionsRemaining = testCombo.Count;
			this.StartWaitingPeriod(0.1f);
			this.Crisanta.AnimatorInyector.ComboMode(true);
		}

		private IEnumerator GetIntoStateAndCallback(State<CrisantaBehaviour> newSt, float waitSeconds, Action callback)
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

		public void StartReduxIntroSequence()
		{
			this.Crisanta.Controller.SmartPlatformCollider.enabled = false;
			this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = false;
			base.transform.position = this.crisantaNPC.transform.position;
			this.Crisanta.Controller.SmartPlatformCollider.enabled = true;
			this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = true;
			this.Crisanta.Animator.runtimeAnimatorController = this.secondPhaseController;
			this.Crisanta.Animator.speed = 1.5f;
			this.Crisanta.Animator.Play("GUARD");
			this.Crisanta.Behaviour.SetGhostTrail(true);
			this._fsm.ChangeState(this.stIntro);
			this.ActivateCollisions(false);
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.ReduxIntroSequenceCoroutine()));
		}

		private IEnumerator IntroSequenceCoroutine()
		{
			this.ChangePhase(CrisantaBehaviour.Crisanta_PHASES.FIRST);
			this.LookAtPenitent();
			yield return new WaitForSeconds(this.introWaitTime);
			base.BehaviourTree.StartBehaviour();
			this.ActivateCollisions(true);
			this.StartWaitingPeriod(0.1f);
			yield break;
		}

		private IEnumerator ReduxIntroSequenceCoroutine()
		{
			this.ChangePhase(CrisantaBehaviour.Crisanta_PHASES.FIRST);
			this.LookAtPenitent();
			yield return new WaitForSeconds(this.introWaitTime);
			base.BehaviourTree.StartBehaviour();
			this.ActivateCollisions(true);
			yield return new WaitForSeconds(1f);
			this.LaunchAction(CrisantaBehaviour.Crisanta_ATTACKS.DOWNWARDS_SLASH);
			yield break;
		}

		private void ActivateCollisions(bool activate)
		{
			this.Crisanta.DamageArea.DamageAreaCollider.enabled = activate;
		}

		private void Shake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.Shake(0.5f, Vector3.down * 1f, 12, 0.2f, 0f, default(Vector3), 0f, false);
		}

		private void IssueInstantDownwardsSlash()
		{
			this.StartAttackAction();
			this.Crisanta.AnimatorInyector.ChangeStance();
			this.SetCurrentCoroutine(base.StartCoroutine(this.DownwardAttackCoroutine()));
		}

		private void IssueDownwardsSlash()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.DownwardAttackCoroutine()));
		}

		private IEnumerator DownwardAttackCoroutine()
		{
			this.LookAtPenitent();
			this.Crisanta.AnimatorInyector.DownwardsSlash();
			yield return base.StartCoroutine(this.BlockUntilAnimationEnds());
			this.OnDownwardAttackEnds();
			yield break;
		}

		private void OnHeavyAttackGuarded()
		{
			this.Parry();
		}

		private void OnDownwardAttackEnds()
		{
			this.StartWaitingPeriod(0.1f);
		}

		private void IssueUpwardsSlash()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.UpwardsSlashCoroutine()));
		}

		private IEnumerator UpwardsSlashCoroutine()
		{
			this.LookAtPenitent();
			this.Crisanta.AnimatorInyector.UpwardsSlash();
			yield return base.StartCoroutine(this.BlockUntilAnimationEnds());
			this.OnLightAttackEnds();
			yield break;
		}

		private void OnLightAttackEnds()
		{
			this.StartWaitingPeriod(0.1f);
		}

		private void IssueBackflip()
		{
			this.SetRecovering(false);
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.BackflipCoroutine(false, false)));
		}

		private void IssueHighBackflip()
		{
			this.SetRecovering(false);
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.BackflipCoroutine(true, false)));
		}

		private void IssueEscapeAttack()
		{
			this.SetRecovering(false);
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.BackflipCoroutine(true, true)));
		}

		private Vector2 GetDirToPenitent()
		{
			if (Core.Logic.Penitent == null)
			{
				return Vector2.zero;
			}
			return Core.Logic.Penitent.transform.position + Vector3.up - base.transform.position;
		}

		private float DistanceToPenitent()
		{
			if (Core.Logic.Penitent == null)
			{
				return 0f;
			}
			return Vector2.Distance(Core.Logic.Penitent.transform.position + Vector3.up, base.transform.position);
		}

		private IEnumerator BackflipCoroutine(bool isGreatJump, bool forward = false)
		{
			this.LookAtPenitent();
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			float d = this.GetDirFromOrientation();
			float maxWalkSpeed = this.Crisanta.Controller.MaxWalkingSpeed;
			bool diagonalAttack = false;
			if (isGreatJump)
			{
				this.Crisanta.Controller.MaxWalkingSpeed *= 0.6f * this.backflipSpeedFactor;
				diagonalAttack = true;
			}
			else
			{
				this.Crisanta.Controller.MaxWalkingSpeed *= 1.4f * this.backflipSpeedFactor;
			}
			if (forward)
			{
				this.Crisanta.Input.HorizontalInput = d;
			}
			else
			{
				this.Crisanta.Input.HorizontalInput = -d;
			}
			this.SetGhostTrail(true);
			this.Crisanta.AnimatorInyector.Backflip();
			yield return base.StartCoroutine(this.JumpPress(isGreatJump));
			if (diagonalAttack)
			{
				yield return base.StartCoroutine(this.WaitApexOfJump());
				Vector2 dir = new Vector2(this.GetDirFromOrientation(), -0.75f);
				Vector2 dirToPenitent = this.GetDirToPenitent();
				float ang = Vector2.Angle(dir, dirToPenitent);
				if (ang < 3f * this.backflipAngleFactor)
				{
					float num = (float)((Vector2.Dot(dir, dirToPenitent) <= 0f) ? 1 : -1);
					num *= this.GetDirFromOrientation();
					dir = dirToPenitent;
					base.transform.rotation = Quaternion.Euler(0f, 0f, ang * num);
				}
				this.Crisanta.Input.HorizontalInput = 0f;
				this.Crisanta.Controller.MaxWalkingSpeed = maxWalkSpeed;
				this.SetCurrentCoroutine(base.StartCoroutine(this.DiagonalAttack(base.transform.position, dir, 35f, false)));
			}
			else
			{
				yield return new WaitForSeconds(0.2f);
				yield return base.StartCoroutine(this.WaitCloseToGround());
				this.Crisanta.AnimatorInyector.BackflipLand();
				this.SetGhostTrail(false);
				this.Crisanta.Input.HorizontalInput = 0f;
				this.Crisanta.Controller.MaxWalkingSpeed = maxWalkSpeed;
				this.OnBackflipEnds();
			}
			yield break;
		}

		private void IssueBounceAttack()
		{
			base.StopAllCoroutines();
			base.StartCoroutine(this.AttackAfterBounce());
		}

		private IEnumerator AttackAfterBounce()
		{
			yield return base.StartCoroutine(this.WaitApexOfJump());
			this.LookAtPenitent();
			Vector2 dir = new Vector2(this.GetDirFromOrientation(), -0.75f);
			Vector2 dirToPenitent = this.GetDirToPenitent();
			float ang = Vector2.Angle(dir, dirToPenitent);
			if (ang < 15f * this.backflipSpeedFactor)
			{
				float num = (float)((Vector2.Dot(dir, dirToPenitent) <= 0f) ? -1 : 1);
				num *= this.GetDirFromOrientation();
				dir = dirToPenitent;
				base.transform.rotation = Quaternion.Euler(0f, 0f, ang * num);
			}
			this.SetCurrentCoroutine(base.StartCoroutine(this.DiagonalAttack(base.transform.position, dir, 35f, true)));
			yield break;
		}

		private IEnumerator WaitApexOfJump()
		{
			while (this.Crisanta.Controller.PlatformCharacterPhysics.VSpeed >= 0f)
			{
				yield return null;
			}
			yield break;
		}

		private IEnumerator DiagonalAttack(Vector2 pos, Vector2 dir, float distance, bool alreadyBounced = false)
		{
			this.Crisanta.Controller.PlatformCharacterPhysics.Velocity = Vector2.zero;
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 0f;
			this.LookAtTarget(pos + dir);
			this.Crisanta.AnimatorInyector.AirAttack(true);
			this.Crisanta.Audio.PlayAirAttack_AUDIO();
			yield return new WaitForSeconds(0.4f);
			this.diagonalSlashDashAttack.Dash(base.transform, dir.normalized, distance, 0.1f, false);
			yield return new WaitForSeconds(0.2f);
			this.Crisanta.AnimatorInyector.AirAttack(false);
			base.transform.rotation = Quaternion.identity;
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			this.Crisanta.Audio.PlayLanding_AUDIO();
			Vector2 point;
			if (!this.Crisanta.MotionChecker.HitsFloorInPosition(this.Crisanta.transform.position, this.Crisanta.MotionChecker.RangeGroundDetection, out point, false))
			{
				yield return base.StartCoroutine(this.RecoveryBackflip(!alreadyBounced));
			}
			else
			{
				this.LandingShockwave();
				yield return base.StartCoroutine(this.RecoveryBackflip(false));
			}
			this.LookAtPenitent();
			this.SetGhostTrail(false);
			this.StartWaitingPeriod(0.1f);
			yield break;
		}

		private void LandingShockwave()
		{
			this.lightningAttack.SummonAreas(Vector2.right);
			this.lightningAttack.SummonAreas(Vector2.left);
		}

		private IEnumerator RecoveryBackflip(bool attack = false)
		{
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			float d = this.GetDirFromOrientation();
			float maxWalkSpeed = this.Crisanta.Controller.MaxWalkingSpeed;
			this.Crisanta.Controller.MaxWalkingSpeed *= 1f;
			this.Crisanta.Input.HorizontalInput = -d;
			this.SetGhostTrail(true);
			this.SetRecovering(false);
			this.Crisanta.AnimatorInyector.Backflip();
			this.ForceJump(8f);
			yield return new WaitForSeconds(0.2f);
			if (attack)
			{
				this.SetGhostTrail(false);
				this.Crisanta.Input.HorizontalInput = 0f;
				this.Crisanta.Controller.MaxWalkingSpeed = maxWalkSpeed;
				this.IssueBounceAttack();
			}
			else
			{
				yield return base.StartCoroutine(this.WaitCloseToGround());
				this.Crisanta.AnimatorInyector.BackflipLand();
				this.SetGhostTrail(false);
				this.Crisanta.Input.HorizontalInput = 0f;
				this.Crisanta.Controller.MaxWalkingSpeed = maxWalkSpeed;
			}
			yield break;
		}

		private void OnDashBlocked(BossDashAttack obj)
		{
			this.Parry();
		}

		private IEnumerator WaitCloseToGround()
		{
			while (!this.Crisanta.Controller.IsGrounded)
			{
				yield return null;
			}
			yield break;
		}

		private void ForceJump(float vSpeed)
		{
			this.Crisanta.Controller.PlatformCharacterPhysics.VSpeed = vSpeed;
		}

		private IEnumerator JumpPress(bool isLongPress)
		{
			this.Crisanta.Input.Jump = true;
			WaitForSeconds awaitType = (!isLongPress) ? new WaitForSeconds(0.1f) : new WaitForSeconds(0.5f);
			yield return awaitType;
			this.Crisanta.Input.Jump = false;
			yield break;
		}

		private void OnBackflipEnds()
		{
			this.Crisanta.Audio.PlayLanding_AUDIO();
			this.StartWaitingPeriod(0.1f);
		}

		private void IssueGuard()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.GuardCoroutine()));
		}

		private IEnumerator GuardCoroutine()
		{
			this.Crisanta.AnimatorInyector.Guard(true);
			yield return new WaitForSeconds(this.guardWaitTime);
			this.Crisanta.AnimatorInyector.Guard(false);
			this.EndGuard();
			yield break;
		}

		private void EndGuard()
		{
			float seconds = (!this.Crisanta.IsCrisantaRedux) ? 1f : 0.5f;
			this.StartWaitingPeriod(seconds);
		}

		private void IssueInstantGuard()
		{
			this.StartAttackAction();
			this.Crisanta.AnimatorInyector.ChangeStance();
			this.QueuedActionsPush(CrisantaBehaviour.Crisanta_ATTACKS.DOWNWARDS_SLASH);
			this.SetCurrentCoroutine(base.StartCoroutine(this.GuardCoroutine()));
		}

		private void IssueUnseal()
		{
			this.StartAttackAction();
			this.Crisanta.AnimatorInyector.AnimationEvent_SetShieldedOn();
			this.SetCurrentCoroutine(base.StartCoroutine(this.UnsealCoroutine()));
		}

		private IEnumerator UnsealCoroutine()
		{
			this.Crisanta.Animator.runtimeAnimatorController = this.secondPhaseController;
			this.instantLightningAttack.SummonAreaOnPoint(base.transform.position, 0f, 1f, null);
			this.lightningAttack.totalAreas = 12;
			this.lightningAttack.seconds = 2f;
			this.Crisanta.AnimatorInyector.Unseal();
			yield return new WaitForSeconds(1f);
			Core.Logic.ScreenFreeze.Freeze(0.05f, 1f, 0f, this.Crisanta.slowTimeCurve);
			this.LandingShockwave();
			yield return new WaitForSeconds(1f);
			this.EndUnseal();
			yield break;
		}

		private void EndUnseal()
		{
			this.unveiled = true;
			this.QueuedActionsPush(CrisantaBehaviour.Crisanta_ATTACKS.COMBO_BLINK_A);
			this.Crisanta.AnimatorInyector.AnimationEvent_SetShieldedOff();
			this.StartWaitingPeriod(0.01f);
		}

		public void OnBlinkIn()
		{
			this.Crisanta.DamageArea.DamageAreaCollider.enabled = true;
		}

		public void OnBlinkOut()
		{
			this.Crisanta.DamageArea.DamageAreaCollider.enabled = false;
		}

		private bool IsPenitentOnRightSide()
		{
			return this.IsOnRightSide(Core.Logic.Penitent.transform);
		}

		private bool IsPenitentCloseToCenter()
		{
			return this.IsCloseToCenter(Core.Logic.Penitent.transform);
		}

		private bool IsCrisantaOnRightSide()
		{
			return this.IsOnRightSide(base.transform);
		}

		private bool IsCrisantaCloseToCenter()
		{
			return this.IsCloseToCenter(base.transform);
		}

		private bool IsOnRightSide(Transform t)
		{
			return t.position.x > this.fightCenterTransform.position.x;
		}

		private bool IsCloseToCenter(Transform t)
		{
			float num = 4f;
			return Mathf.Abs(t.position.x - this.fightCenterTransform.transform.position.x) < num;
		}

		private void IssueLeftBlinkSlash()
		{
			this.StartAttackAction();
			if (this.IsPenitentCloseToCenter() || this.IsPenitentOnRightSide())
			{
				this.LeftBlinkSlash();
			}
			else
			{
				this.RightBlinkSlash();
			}
		}

		private void IssueRightBlinkSlash()
		{
			this.StartAttackAction();
			if (this.IsPenitentCloseToCenter() || !this.IsPenitentOnRightSide())
			{
				this.RightBlinkSlash();
			}
			else
			{
				this.LeftBlinkSlash();
			}
		}

		private void LeftBlinkSlash()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.BlinkSlashCoroutine(this.fightCenterTransform.position - Vector3.right * 8f - Vector3.up * 0.75f, Vector2.right, 18f, false)));
		}

		private void RightBlinkSlash()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.BlinkSlashCoroutine(this.fightCenterTransform.position + Vector3.right * 8f - Vector3.up * 0.75f, -Vector2.right, 18f, false)));
		}

		private void IssueRightBlinkDiagonalSlash()
		{
			this.StartAttackAction();
			if (this.IsPenitentCloseToCenter() || !this.IsPenitentOnRightSide())
			{
				this.RightBlinkDiagonalSlash();
			}
			else
			{
				this.LeftBlinkDiagonalSlash();
			}
		}

		private void IssueLeftBlinkDiagonalSlash()
		{
			this.StartAttackAction();
			if (this.IsPenitentCloseToCenter() || this.IsPenitentOnRightSide())
			{
				this.LeftBlinkDiagonalSlash();
			}
			else
			{
				this.RightBlinkDiagonalSlash();
			}
		}

		private void RightBlinkDiagonalSlash()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.BlinkSlashCoroutine(Core.Logic.Penitent.transform.position + Vector3.right * 6f + Vector3.up * 4f, new Vector2(-1f, -0.75f), 13f, true)));
		}

		private void LeftBlinkDiagonalSlash()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.BlinkSlashCoroutine(Core.Logic.Penitent.transform.position - Vector3.right * 6f + Vector3.up * 4f, new Vector2(1f, -0.75f), 13f, true)));
		}

		private IEnumerator BlinkOut()
		{
			this.Crisanta.AnimatorInyector.BlinkOut();
			this.Crisanta.Audio.PlayTeleportOut_AUDIO();
			yield return new WaitForSeconds(0.8f);
			yield break;
		}

		private IEnumerator BlinkSlashCoroutine(Vector2 pos, Vector2 dir, float distance, bool diagonal = false)
		{
			this.Crisanta.DamageByContact = false;
			if (!this.Crisanta.Animator.GetCurrentAnimatorStateInfo(0).IsName("HIDDEN"))
			{
				yield return base.StartCoroutine(this.BlinkOut());
			}
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 0f;
			this.Crisanta.Controller.PlatformCharacterPhysics.Velocity = Vector2.zero;
			this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = false;
			base.transform.position = pos;
			this.LookAtTarget(pos + dir);
			yield return new WaitForSeconds(0.1f);
			if (diagonal)
			{
				UnityEngine.Debug.Log("DIAGONAL BLINK");
				this.Crisanta.Audio.PlayAirAttack_AUDIO();
				this.Crisanta.AnimatorInyector.AirAttack(true);
			}
			else
			{
				UnityEngine.Debug.Log("HORIZONTAL_BLINK");
				this.Crisanta.Audio.PlayAirAttack_AUDIO();
				this.Crisanta.AnimatorInyector.Blinkslash(true);
			}
			yield return new WaitForSeconds(0.4f);
			if (diagonal)
			{
				this.diagonalSlashDashAttack.Dash(base.transform, dir, distance, 0f, false);
			}
			else
			{
				this.slashDashAttack.Dash(base.transform, dir, distance, 0f, false);
			}
			this.sparkParticlesMid.Play();
			yield return new WaitForSeconds(0.3f);
			this.sparkParticlesMid.Stop();
			if (diagonal)
			{
				this.Crisanta.AnimatorInyector.AirAttack(false);
			}
			else
			{
				this.Crisanta.AnimatorInyector.Blinkslash(false);
			}
			if (this.NoBlinkActionsRemaining())
			{
				yield return new WaitForSeconds(this.afterBlinkComboWaitTime);
				this.BlinkSlashEnds();
				yield return new WaitForSeconds(0.5f);
				this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
				this.Crisanta.DamageByContact = true;
				this.StartWaitingPeriod(0.1f);
			}
			else
			{
				yield return new WaitForSeconds(1.4f);
				this.StartWaitingPeriod(0.1f);
			}
			yield break;
		}

		private bool NoBlinkActionsRemaining()
		{
			return this.queuedActions == null || (!this.queuedActions.Contains(CrisantaBehaviour.Crisanta_ATTACKS.LEFT_DIAGONAL_BLINK) && !this.queuedActions.Contains(CrisantaBehaviour.Crisanta_ATTACKS.RIGHT_DIAGONAL_BLINK) && !this.queuedActions.Contains(CrisantaBehaviour.Crisanta_ATTACKS.LEFT_HORIZONTAL_BLINK) && !this.queuedActions.Contains(CrisantaBehaviour.Crisanta_ATTACKS.RIGHT_HORIZONTAL_BLINK));
		}

		private void BlinkSlashEnds()
		{
			UnityEngine.Debug.Log("<color=red> BLINK SLASH ENDS</color>");
			Vector3 position = this.fightCenterTransform.transform.position;
			bool flag = Core.Logic.Penitent.GetPosition().x > this.fightCenterTransform.transform.position.x;
			if (flag)
			{
				position.x -= UnityEngine.Random.Range(3f, 5f);
			}
			else
			{
				position.x += UnityEngine.Random.Range(3f, 5f);
			}
			position.y += 0.1f;
			this.Crisanta.Controller.SmartPlatformCollider.enabled = false;
			this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = false;
			base.transform.position = position;
			this.Crisanta.Controller.SmartPlatformCollider.enabled = true;
			this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = true;
			this.LookAtPenitent();
			this.Crisanta.Audio.PlayTeleportIn_AUDIO();
			this.Crisanta.AnimatorInyector.BlinkIn();
		}

		private void IssueCornerShockwaves()
		{
			this.StartAttackAction();
			if (this.IsPenitentCloseToCenter() || !this.IsPenitentOnRightSide())
			{
				this.RightCornerShockwaves();
			}
			else
			{
				this.LeftCornerShockwaves();
			}
		}

		private void RightCornerShockwaves()
		{
			Vector3 v = this.fightCenterTransform.position + Vector3.right * 8f;
			this.SetCurrentCoroutine(base.StartCoroutine(this.CornerShockwavesCoroutine(v, Vector2.left, 4)));
		}

		private void LeftCornerShockwaves()
		{
			Vector3 v = this.fightCenterTransform.position - Vector3.right * 8f;
			this.SetCurrentCoroutine(base.StartCoroutine(this.CornerShockwavesCoroutine(v, Vector2.right, 4)));
		}

		private IEnumerator CornerShockwavesCoroutine(Vector2 pos, Vector2 dir, int numShockWaves)
		{
			this.Crisanta.DamageByContact = false;
			if (!this.Crisanta.Animator.GetCurrentAnimatorStateInfo(0).IsName("HIDDEN"))
			{
				yield return base.StartCoroutine(this.BlinkOut());
			}
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 0f;
			this.Crisanta.Controller.PlatformCharacterPhysics.Velocity = Vector2.zero;
			this.Crisanta.Controller.SmartPlatformCollider.enabled = false;
			this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = false;
			base.transform.position = pos;
			this.Crisanta.Controller.SmartPlatformCollider.enabled = true;
			this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = true;
			this.LookAtPenitent();
			this.Crisanta.Audio.PlayTeleportIn_AUDIO();
			this.Crisanta.AnimatorInyector.BlinkIn();
			yield return new WaitForSeconds(0.5f);
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			this.Crisanta.DamageByContact = true;
			this.ignoreAnimDispl = true;
			float prevSpeed = this.bladeAttack.projectileSpeed;
			bool up = true;
			this.Crisanta.GhostTrail.EnableGhostTrail = false;
			this.Crisanta.Animator.speed = 1f;
			for (int i = 0; i < numShockWaves; i++)
			{
				if (up)
				{
					this.Crisanta.AnimatorInyector.UpwardsSlash();
				}
				else
				{
					this.Crisanta.AnimatorInyector.DownwardsSlash();
				}
				if (i == numShockWaves - 1)
				{
					this.bladeAttack.projectileSpeed *= 1.2f;
					this.Crisanta.AnimatorInyector.Guard(true);
					yield return new WaitForSeconds(0.2f);
					this.Crisanta.AnimatorInyector.Guard(false);
				}
				yield return new WaitUntil(() => this.lightAttack.damageOnEnterArea || this.heavyAttack.damageOnEnterArea);
				this.bladeAttack.Shoot(dir);
				yield return base.StartCoroutine(this.BlockUntilAnimationEnds());
				up = !up;
			}
			this.Crisanta.Animator.speed = 1.5f;
			this.Crisanta.GhostTrail.EnableGhostTrail = true;
			this.bladeAttack.projectileSpeed = prevSpeed;
			this.ignoreAnimDispl = false;
			float prevAnimSpeed = this.Crisanta.Animator.speed;
			this.Crisanta.Animator.speed = 0.6f;
			this.Crisanta.AnimatorInyector.SetStayKneeling(true);
			this.Crisanta.Animator.Play("HURT_2_KNEELED");
			yield return new WaitForSeconds(3f);
			this.Crisanta.AnimatorInyector.SetStayKneeling(false);
			yield return new WaitForSeconds(0.5f);
			this.Crisanta.Animator.speed = prevAnimSpeed;
			this.StartWaitingPeriod(0.1f);
			yield break;
		}

		public void OnEnterGuard()
		{
			this._fsm.ChangeState(this.stGuard);
		}

		public void OnExitGuard()
		{
			this._fsm.ChangeState(this.stAction);
		}

		public void OnExitsDownslash()
		{
			this._waitingForAnimationFinish = false;
		}

		public void OnExitMeleeAttack()
		{
			UnityEngine.Debug.Log("TIME: " + Time.time + " OnExitMeleeAttack called.");
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

		public void CounterImpactShockwave()
		{
			float d = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.instantLightningAttack.SummonAreaOnPoint(base.transform.position - d * Vector3.right, 0f, 1f, null);
		}

		public void OnHitReactionAnimationCompleted()
		{
			UnityEngine.Debug.Log("HIT REACTION COMPLETED. RECOVERING FALSE");
			this.SetRecovering(false);
			this._currentRecoveryHits = 0;
			this.QueueRecoveryAction();
			this.StartWaitingPeriod(0.5f);
		}

		private void QueueRecoveryAction()
		{
			if (this.DistanceToPenitent() < 3f)
			{
				float dirFromOrientation = this.GetDirFromOrientation();
				Vector2 a = new Vector2(-dirFromOrientation, 0f);
				if (this.HasSpaceInDirection(a * 3f))
				{
					this.QueuedActionsPush(CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_LOW);
				}
				else if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
				{
					this.QueuedActionsPush(CrisantaBehaviour.Crisanta_ATTACKS.FORWARD_FLIP);
				}
				else
				{
					this.QueuedActionsPush(CrisantaBehaviour.Crisanta_ATTACKS.INSTANT_GUARD);
				}
			}
			else
			{
				this.QueuedActionsPush(CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_HIGH);
			}
		}

		public void AttackDisplacement(float duration, float displacement, bool trail)
		{
			duration *= this.shortDisplDurationFactor;
			displacement *= this.shortDisplLengthFactor;
			this.SetGhostTrail(trail);
			this.Crisanta.DamageByContact = false;
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

		public void AttackDisplacementToPoint(Vector2 point, float offset, float baseSpeed, bool trail)
		{
			offset *= this.mediumDisplOffsetFactor;
			baseSpeed *= this.mediumDisplSpeedFactor;
			this.SetGhostTrail(trail);
			this.sparkParticles.Play();
			this.Crisanta.DamageByContact = false;
			Ease ease = Ease.OutQuad;
			this.LookAtTarget(point);
			float d = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			point -= Vector2.right * d * offset;
			Vector2 vector = point - base.transform.position;
			float num = Vector2.Distance(point, base.transform.position);
			if (num < 2f)
			{
				num = 0f;
			}
			Vector2 vector2 = Vector2.right * d * num;
			float duration = num / baseSpeed;
			vector2 = this.ClampToFightBoundaries(vector2);
			base.transform.DOMove(base.transform.position + vector2, duration, false).SetEase(ease).OnComplete(delegate
			{
				this.AfterDisplacement();
			});
		}

		private void AfterDisplacement()
		{
			this.sparkParticles.Stop();
			this.Crisanta.DamageByContact = true;
			this.SetGhostTrail(false);
		}

		public void BackDisplacement(float duration, float displacement)
		{
			this.SetGhostTrail(true);
			this.Crisanta.DamageByContact = false;
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

		public void RunToPoint(Vector2 position)
		{
			this.LookAtTarget(position);
			float horizontalInput = (this.Entity.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.Crisanta.Input.HorizontalInput = horizontalInput;
		}

		public void StopRunning()
		{
			this.Crisanta.Input.HorizontalInput = 0f;
			this.Crisanta.Controller.PlatformCharacterPhysics.HSpeed = 0f;
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

		public void Death()
		{
			this.LaunchAction(CrisantaBehaviour.Crisanta_ATTACKS.DEATH);
		}

		private IEnumerator DeathBackflip()
		{
			yield return base.StartCoroutine(this.WaitCloseToGround());
			this.LookAtPenitent();
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			float d = this.GetDirFromOrientation();
			float maxWalkSpeed = this.Crisanta.Controller.MaxWalkingSpeed;
			this.Crisanta.Controller.MaxWalkingSpeed *= 1f;
			this.Crisanta.Input.HorizontalInput = -d;
			this.SetGhostTrail(true);
			this.CancelAttacks();
			this.Crisanta.AnimatorInyector.AirAttack(false);
			this.Crisanta.AnimatorInyector.Blinkslash(false);
			this.Crisanta.AnimatorInyector.BlinkIn();
			this.Crisanta.AnimatorInyector.DeathBackflip();
			yield return base.StartCoroutine(this.JumpPress(false));
			yield return new WaitForSeconds(0.2f);
			yield return base.StartCoroutine(this.WaitCloseToGround());
			this.RepositionBackflipEnd();
			this.Crisanta.Controller.MaxWalkingSpeed = maxWalkSpeed;
			yield break;
		}

		private IEnumerator DeathBackflipAfterRedux()
		{
			this.CancelAttacks();
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			this.Crisanta.AnimatorInyector.AirAttack(false);
			this.Crisanta.AnimatorInyector.Blinkslash(false);
			this.Crisanta.AnimatorInyector.BlinkIn();
			if (base.transform.position.x > this.fightCenterTransform.position.x)
			{
				this.RepositionBackflipStart(-1f, Vector3.right);
			}
			else
			{
				this.RepositionBackflipStart(1f, Vector3.left);
			}
			yield return base.StartCoroutine(this.JumpPress(false));
			yield return new WaitForSeconds(0.2f);
			yield return base.StartCoroutine(this.WaitCloseToGround());
			this.RepositionBackflipEnd();
			yield break;
		}

		private IEnumerator DeathBackflipForRedux()
		{
			this.CancelAttacks();
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			this.Crisanta.AnimatorInyector.AirAttack(false);
			this.Crisanta.AnimatorInyector.Blinkslash(false);
			this.Crisanta.AnimatorInyector.BlinkIn();
			yield return base.StartCoroutine(this.WaitCloseToGround());
			if (base.transform.position.x > this.fightCenterTransform.position.x)
			{
				this.RepositionBackflipStart(-1f, Vector3.right);
				Core.Logic.Penitent.DrivePlayer.MoveToPosition(this.tpoWaypointRight.transform.position, EntityOrientation.Left);
			}
			else
			{
				this.RepositionBackflipStart(1f, Vector3.left);
				Core.Logic.Penitent.DrivePlayer.MoveToPosition(this.tpoWaypointLeft.transform.position, EntityOrientation.Right);
			}
			yield return base.StartCoroutine(this.JumpPress(!this.IsCrisantaCloseToCenter()));
			yield return new WaitForSeconds(0.2f);
			yield return base.StartCoroutine(this.WaitCloseToGround());
			this.RepositionBackflipEnd();
			yield break;
		}

		private void RepositionBackflipStart(float dir, Vector3 dirToLook)
		{
			this.LookAtTarget(base.transform.position + dirToLook);
			this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
			this.Crisanta.Input.HorizontalInput = dir;
			this.SetGhostTrail(true);
			this.CancelAttacks();
			this.Crisanta.AnimatorInyector.AirAttack(false);
			this.Crisanta.AnimatorInyector.DeathBackflip();
		}

		private void RepositionBackflipEnd()
		{
			this.Crisanta.AnimatorInyector.BackflipLand();
			this.Crisanta.GhostTrail.EnableGhostTrail = false;
			this.Crisanta.Input.HorizontalInput = 0f;
		}

		private void DeathAction()
		{
			base.StopBehaviour();
			base.StopAllCoroutines();
			base.transform.DOKill(true);
			FadeWidget.instance.ClearFade();
			if (!this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D)
			{
				base.transform.position = new Vector3(base.transform.position.x, this.fightCenterTransform.position.y + 0.5f, base.transform.position.z);
				this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = true;
			}
			this.StartAttackAction();
			if (this.Crisanta.IsCrisantaRedux)
			{
				base.StartCoroutine(this.DeathBackflipAfterRedux());
			}
			else if (Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.BOSS_RUSH))
			{
				base.StartCoroutine(this.DeathBackflip());
			}
			else
			{
				base.StartCoroutine(this.DeathBackflipForRedux());
			}
		}

		public void SubstituteForExecution()
		{
			this.CancelAttacks();
			if (Core.InventoryManager.IsTrueSwordHeartEquiped() || Core.LevelManager.currentLevel.LevelName.Equals("D22Z01S19"))
			{
				if (this.Crisanta.IsCrisantaRedux)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.trueEndingExecutionPrefab, base.transform.position, base.transform.rotation);
					SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
					componentInChildren.flipX = (this.Crisanta.Status.Orientation == EntityOrientation.Left);
					FakeExecution component = gameObject.GetComponent<FakeExecution>();
					Singleton<Core>.Instance.StartCoroutine(this.WaitAndSetDialogMode(component));
				}
				else
				{
					this.crisantaNPC.transform.position = base.transform.position;
					this.crisantaNPC.SetActive(true);
					if (this.IsPenitentOnRightSide())
					{
						this.crisantaNPC.transform.localScale = new Vector3(-1f, 1f, 1f);
					}
					EntityOrientation orientation = (this.GetDirToPenitent().x <= 0f) ? EntityOrientation.Right : EntityOrientation.Left;
					Core.Logic.Penitent.SetOrientation(orientation, true, false);
					PlayMakerFSM.BroadcastEvent("REPOSITION REDUX");
				}
			}
			else
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.executionPrefab, base.transform.position, base.transform.rotation);
				gameObject2.transform.localScale = ((this.Crisanta.Status.Orientation != EntityOrientation.Left) ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f));
			}
			base.gameObject.SetActive(false);
		}

		private IEnumerator WaitAndSetDialogMode(FakeExecution execution)
		{
			Penitent p = Core.Logic.Penitent;
			yield return new WaitUntil(() => execution.BeingUsed);
			p.Shadow.ManuallyControllingAlpha = true;
			Tween t = DOTween.To(() => p.Shadow.GetShadowAlpha(), delegate(float x)
			{
				p.Shadow.SetShadowAlpha(x);
			}, 0f, 0.2f);
			EntityOrientation targetOrientation = (execution.transform.position.x <= p.transform.position.x) ? EntityOrientation.Left : EntityOrientation.Right;
			execution.InstanceOrientation = ((execution.transform.position.x <= p.transform.position.x) ? EntityOrientation.Right : EntityOrientation.Left);
			p.Animator.SetBool("IS_DIALOGUE_MODE", true);
			yield return new WaitUntil(() => !execution.BeingUsed);
			p.SetOrientation(targetOrientation, true, false);
			t = DOTween.To(() => p.Shadow.GetShadowAlpha(), delegate(float x)
			{
				p.Shadow.SetShadowAlpha(x);
			}, 1f, 0.2f);
			t.OnComplete(delegate
			{
				p.Shadow.ManuallyControllingAlpha = false;
			});
			yield break;
		}

		private bool IsPenitentClose()
		{
			return Vector2.Distance(Core.Logic.Penitent.GetPosition(), base.transform.position) < 3f;
		}

		public override void Idle()
		{
			this.StopMovement();
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		public override void Parry()
		{
			base.Parry();
			base.transform.rotation = Quaternion.identity;
			this.sparkParticles.Stop();
			this.sparkParticlesMid.Stop();
			if (this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale == 0f)
			{
				this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = true;
				this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
				this.Crisanta.DamageByContact = true;
				this.CancelAttacks();
				this.CancelCombo();
				base.StopAllCoroutines();
				this.SetGhostTrail(false);
				base.transform.DOKill(false);
				FadeWidget.instance.ClearFade();
				this.IssueBackflip();
			}
			else
			{
				this.Crisanta.Controller.SmartPlatformCollider.EnableCollision2D = true;
				this.Crisanta.Controller.PlatformCharacterPhysics.GravityScale = 1f;
				this.Crisanta.DamageByContact = true;
				this.CancelAttacks();
				this.CancelCombo();
				base.StopAllCoroutines();
				this.SetGhostTrail(false);
				base.transform.DOKill(false);
				FadeWidget.instance.ClearFade();
				this.Crisanta.AnimatorInyector.Parry();
				if (this.Crisanta.IsCrisantaRedux)
				{
					Core.Logic.ScreenFreeze.Freeze(0.05f, 0.2f, 0f, null);
					base.StartCoroutine(this.FastFadeCoroutine());
					Vector3 position = base.transform.position;
					position.x = (position.x + Core.Logic.Penitent.GetPosition().x) / 2f;
					this.dramaLightningAttack.SummonAreaOnPoint(position, 0f, 1f, null);
					this.lightningAttack.SummonAreaOnPoint(position, 0f, 1f, null);
					Core.Logic.CameraManager.ShockwaveManager.Shockwave(position, 0.5f, 0.3f, 1.5f);
				}
				this.BackDisplacement(this.backDisplDurationFactor, this.backDisplLengthFactor);
				this.SetRecovering(true);
				this.justParried = true;
				this.StartWaitingPeriod(this.afterParryWaitTime);
			}
		}

		private IEnumerator FastFadeCoroutine()
		{
			yield return FadeWidget.instance.FadeCoroutine(new Color(0f, 0f, 0f, 0.2f), new Color(0f, 0f, 0f, 0.4f), 0.05f, true, null);
			yield return FadeWidget.instance.FadeCoroutine(new Color(1f, 1f, 1f, 0.4f), new Color(1f, 1f, 1f, 0.8f), 0.05f, true, null);
			yield return FadeWidget.instance.FadeCoroutine(new Color(1f, 1f, 1f, 1f), Color.clear, 0.05f, true, null);
			yield break;
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
			if (this._recovering && this._currentRecoveryHits < this.maxHitsInRecovery)
			{
				this.CancelCombo();
				this.CancelAttacks();
				base.StopAllCoroutines();
				this.Crisanta.AnimatorInyector.Hurt();
				base.transform.DOKill(true);
				FadeWidget.instance.ClearFade();
				this.LookAtPenitent();
				this.BackDisplacement(0.3f, 0.4f);
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					this._currentRecoveryHits,
					(!this._recovering) ? "FALSE" : "TRUE"
				}));
				this._currentRecoveryHits++;
				if (this._currentRecoveryHits >= this.maxHitsInRecovery)
				{
					UnityEngine.Debug.Log("<color=magenta>COUNTER</color>");
					this._currentRecoveryHits = 0;
					this.LaunchAction(CrisantaBehaviour.Crisanta_ATTACKS.BACKFLIP_LOW);
				}
				else
				{
					this.StartWaitingPeriod(1f);
				}
			}
		}

		public void OnGuardHit()
		{
			if (this._fsm.IsInState(this.stGuard))
			{
				Core.Logic.ScreenFreeze.Freeze(0.05f, 0.2f, 0f, null);
				this.Crisanta.Audio.PlayParry_AUDIO();
				base.StopAllCoroutines();
				this.CancelCombo();
				base.transform.DOKill(true);
				FadeWidget.instance.ClearFade();
				this.LookAtPenitent();
				if (this.queuedActions != null)
				{
					this.queuedActions.Clear();
				}
				this.comboActionsRemaining = 0;
				this.CancelCombo();
				this.Crisanta.AnimatorInyector.Guard(false);
				this.LaunchAction(CrisantaBehaviour.Crisanta_ATTACKS.COUNTER_BLINK);
			}
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			this.Crisanta.SetOrientation((targetPos.x <= this.Crisanta.transform.position.x) ? EntityOrientation.Left : EntityOrientation.Right, true, false);
			this.particleParent.transform.localScale = new Vector3(this.GetDirFromOrientation(), 1f, 1f);
		}

		public override void StopMovement()
		{
		}

		public void SetGhostTrail(bool active)
		{
			this.Crisanta.GhostTrail.EnableGhostTrail = (active || this.Crisanta.IsCrisantaRedux);
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

		public bool HasSpaceInDirection(Vector2 dir)
		{
			Vector2 v = dir;
			UnityEngine.Debug.DrawLine(base.transform.position, base.transform.position + v, Color.green, 5f);
			return Physics2D.RaycastNonAlloc(base.transform.position, dir, this.results, dir.magnitude, this.fightBoundariesLayerMask) <= 0;
		}

		public void OnDrawGizmos()
		{
		}

		[FoldoutGroup("Debug", true, 0)]
		public BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public CrisantaBehaviour.Crisanta_ATTACKS lastAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack lightningAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossStraightProjectileAttack bladeAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack instantLightningAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossAreaSummonAttack dramaLightningAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossStraightProjectileAttack arcProjectiles;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossDashAttack slashDashAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		private BossDashAttack diagonalSlashDashAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public CrisantaMeleeAttack lightAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public CrisantaMeleeAttack heavyAttack;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform leftLimitTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform rightLimitTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public Transform fightCenterTransform;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public RuntimeAnimatorController secondPhaseController;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public GameObject tpoWaypointLeft;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public GameObject tpoWaypointRight;

		[SerializeField]
		[FoldoutGroup("References", 0)]
		public GameObject crisantaNPC;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private List<CrisantaBehaviour.CrisantaPhases> phases;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private float maxChaseTime = 5f;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private int maxHitsInRecovery = 3;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private int maxSpinsToProjectile = 2;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private float spinDistance = 10f;

		[SerializeField]
		[FoldoutGroup("Design settings", 0)]
		private LayerMask fightBoundariesLayerMask;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float introWaitTime = 1.5f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float comboRestTime = 1.5f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float afterBlinkComboWaitTime = 1.4f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float guardWaitTime = 1.5f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float afterParryWaitTime = 1.2f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float shortDisplDurationFactor = 1f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float shortDisplLengthFactor = 1f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float mediumDisplOffsetFactor = 1f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float mediumDisplSpeedFactor = 1f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float backDisplDurationFactor = 0.5f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float backDisplLengthFactor = 1f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float backflipAngleFactor = 1f;

		[SerializeField]
		[FoldoutGroup("Timing settings", 0)]
		private float backflipSpeedFactor = 1f;

		public GameObject particleParent;

		public GameObject particleParentMid;

		private ParticleSystem sparkParticles;

		private ParticleSystem sparkParticlesMid;

		private bool justParried;

		private bool _keepRunningAnimation;

		private bool _attackIfEnemyClose;

		public GameObject executionPrefab;

		public GameObject trueEndingExecutionPrefab;

		private Transform currentTarget;

		private StateMachine<CrisantaBehaviour> _fsm;

		private State<CrisantaBehaviour> stAction;

		private State<CrisantaBehaviour> stIntro;

		private State<CrisantaBehaviour> stGuard;

		private State<CrisantaBehaviour> stDeath;

		private Coroutine currentCoroutine;

		private CrisantaBehaviour.Crisanta_PHASES _currentPhase;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<CrisantaBehaviour.Crisanta_ATTACKS> comboSlashesA;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<CrisantaBehaviour.Crisanta_ATTACKS> comboSlashesB;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<CrisantaBehaviour.Crisanta_ATTACKS> comboBlinkA;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<CrisantaBehaviour.Crisanta_ATTACKS> comboBlinkB;

		[SerializeField]
		[FoldoutGroup("Combo settings", 0)]
		public List<CrisantaBehaviour.Crisanta_ATTACKS> comboBackflipSlashesA;

		private List<CrisantaBehaviour.Crisanta_ATTACKS> currentlyAvailableAttacks;

		private List<CrisantaBehaviour.Crisanta_ATTACKS> queuedActions;

		private RaycastHit2D[] results;

		private Vector2 _runPoint;

		private float _chaseCounter;

		private bool _tiredOfChasing;

		private bool _recovering;

		private int _currentRecoveryHits;

		private int _spinsToProjectile;

		private int comboActionsRemaining;

		private bool _waitingForAnimationFinish;

		private bool unveiled;

		[HideInInspector]
		public bool ignoreAnimDispl;

		[Serializable]
		public struct CrisantaPhases
		{
			public CrisantaBehaviour.Crisanta_PHASES phaseId;

			public List<CrisantaBehaviour.Crisanta_ATTACKS> availableAttacks;
		}

		public enum Crisanta_PHASES
		{
			FIRST,
			SECOND,
			LAST
		}

		[Serializable]
		public struct CrisantaAttackConfig
		{
			public CrisantaBehaviour.Crisanta_ATTACKS attackType;

			public float preparationSeconds;

			public float waitingSecondsAfterAttack;
		}

		public enum Crisanta_ATTACKS
		{
			UPWARDS_SLASH,
			DOWNWARDS_SLASH,
			BACKFLIP_LOW,
			LEFT_HORIZONTAL_BLINK,
			LEFT_DIAGONAL_BLINK,
			COUNTER_BLINK,
			GUARD,
			COMBO_SLASHES_A,
			COMBO_REST,
			BACKFLIP_HIGH,
			RIGHT_HORIZONTAL_BLINK,
			RIGHT_DIAGONAL_BLINK,
			COMBO_SLASHES_B,
			COMBO_BLINK_A,
			COMBO_BLINK_B,
			UNSEAL,
			DEATH,
			FORWARD_FLIP,
			INSTANT_GUARD,
			INSTANT_DOWNWARDS,
			CORNER_SHOCKWAVES,
			COMBO_BACKFLIP_SLASH_A
		}
	}
}
