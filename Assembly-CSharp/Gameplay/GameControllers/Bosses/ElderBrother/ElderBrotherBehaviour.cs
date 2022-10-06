using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.ElderBrother
{
	public class ElderBrotherBehaviour : EnemyBehaviour
	{
		public ElderBrother ElderBrother { get; private set; }

		public bool Awaken { get; private set; }

		private List<ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS> GetCurrentStateAttacks()
		{
			return new List<ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS>
			{
				ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS.AREA,
				ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS.JUMP
			};
		}

		public override void OnAwake()
		{
			base.OnAwake();
			PoolManager.Instance.CreatePool(this.corpseFxPrefab, 30);
			this.ElderBrother = (ElderBrother)this.Entity;
			this.currentlyAvailableAttacks = this.GetCurrentStateAttacks();
		}

		public override void OnStart()
		{
			base.OnStart();
			this.ChangeBossState(ElderBrotherBehaviour.BOSS_STATES.WAITING);
		}

		private void Update()
		{
		}

		private void SetCurrentCoroutine(Coroutine c)
		{
			if (this.currentCoroutine != null)
			{
				base.StopCoroutine(this.currentCoroutine);
			}
			this.currentCoroutine = c;
		}

		private void ChangeBossState(ElderBrotherBehaviour.BOSS_STATES newState)
		{
			this.currentState = newState;
		}

		private void StartAttackAction()
		{
			this.ChangeBossState(ElderBrotherBehaviour.BOSS_STATES.MID_ACTION);
		}

		public ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS GetNewAttack()
		{
			ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS[] array = new ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS> list = new List<ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS>(array);
			if (!this.canRepeatAttack)
			{
				list.Remove(this.lastAttack);
			}
			return list[Random.Range(0, list.Count)];
		}

		public void LaunchRandomAction()
		{
			this.LaunchAction(this.GetNewAttack(), true);
		}

		private void QueuedActionsPush(ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS atk)
		{
			if (this.queuedActions == null)
			{
				this.queuedActions = new List<ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS>();
			}
			this.queuedActions.Add(atk);
		}

		private ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS QueuedActionsPop()
		{
			ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS elder_BROTHER_ATTACKS = this.queuedActions[0];
			this.queuedActions.Remove(elder_BROTHER_ATTACKS);
			return elder_BROTHER_ATTACKS;
		}

		public void LaunchAction(ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS atk, bool checkReposition = true)
		{
			this.lastAttack = atk;
			if (atk != ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS.JUMP)
			{
				if (atk != ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS.AREA)
				{
					throw new ArgumentOutOfRangeException("atk", atk, null);
				}
				this.AreaAttack();
			}
			else
			{
				this.JumpAttack();
			}
		}

		private ElderBrotherBehaviour.ElderBrotherAttackConfig GetAttackConfig(ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS atk)
		{
			return this.attacksConfiguration.Find((ElderBrotherBehaviour.ElderBrotherAttackConfig x) => x.attackType == atk);
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == ElderBrotherBehaviour.BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public void IntroJump()
		{
			base.StartCoroutine(this.JumpIntroCoroutine());
		}

		private IEnumerator JumpIntroCoroutine()
		{
			float jumpPreparationDuration = 0.65f;
			float jumpDuration = this.jumpAttack.moveSeconds;
			this.LookAtTarget(base.GetTarget().position);
			this.ElderBrother.AnimatorInyector.SetMidAir(true);
			yield return new WaitForSeconds(jumpPreparationDuration);
			this.jumpAttack.Use(base.transform, this.introJumpPoint.position);
			yield return new WaitForSeconds(jumpDuration * 0.9f);
			this.ElderBrother.AnimatorInyector.SetMidAir(false);
			yield return new WaitForSeconds(1f);
			base.BehaviourTree.StartBehaviour();
			this.StartWaitingPeriod(1f);
			yield break;
		}

		private void StartWaitingPeriod(float seconds)
		{
			this.ChangeBossState(ElderBrotherBehaviour.BOSS_STATES.WAITING);
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
			this.ChangeBossState(ElderBrotherBehaviour.BOSS_STATES.AVAILABLE_FOR_ACTION);
		}

		private Vector3 GetTargetPredictedPos()
		{
			Penitent penitent = Core.Logic.Penitent;
			Vector2 vector = penitent.transform.position;
			vector.x += penitent.GetVelocity().x;
			return vector;
		}

		public void JumpAttack()
		{
			this.StartAttackAction();
			this.currentTarget = base.GetTarget();
			this.jumpAttack.OnJumpLanded += this.OnJumpLanded;
			Debug.Log("ElderBrother: JUMP OUT");
			this.SetCurrentCoroutine(base.StartCoroutine(this.JumpAttackCoroutine()));
		}

		private IEnumerator JumpAttackCoroutine()
		{
			float jumpPreparationDuration = 0.65f;
			float jumpDuration = this.jumpAttack.moveSeconds;
			this.ElderBrother.AnimatorInyector.SetMidAir(true);
			yield return new WaitForSeconds(jumpPreparationDuration);
			Vector3 predictedPosition = this.GetTargetPredictedPos() + this.jumpOffset;
			this.jumpAttack.Use(base.transform, predictedPosition);
			yield return new WaitForSeconds(jumpDuration * 0.9f);
			this.ElderBrother.AnimatorInyector.SetMidAir(false);
			this.LookAtTarget(base.GetTarget().position);
			yield break;
		}

		private void OnJumpLanded()
		{
			this.jumpAttack.OnJumpLanded -= this.OnJumpLanded;
			ElderBrotherBehaviour.ElderBrotherAttackConfig attackConfig = this.GetAttackConfig(ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS.JUMP);
			float extraWaitTime = this.GetExtraWaitTime();
			this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack + extraWaitTime);
		}

		public void AreaAttack()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.PreparingChargeAttack()));
		}

		private IEnumerator PreparingChargeAttack()
		{
			this.currentTarget = base.GetTarget();
			this.LookAtTarget(this.currentTarget.position);
			Vector2 dir = (this.ElderBrother.Status.Orientation != EntityOrientation.Right) ? Vector2.left : Vector2.right;
			this.ElderBrother.AnimatorInyector.BigSmashPreparation();
			yield return new WaitForSeconds(this.attacksConfiguration.Find((ElderBrotherBehaviour.ElderBrotherAttackConfig x) => x.attackType == ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS.AREA).preparationSeconds);
			Debug.Log("ElderBrother: DASH ATTACK");
			this.ElderBrother.AnimatorInyector.Smash();
			this.areaAttack.SummonAreas(Vector3.right * Mathf.Sign(dir.x));
			this.maceImpact.SummonAreas(Vector3.right * Mathf.Sign(dir.x));
			Core.Logic.CameraManager.ShockwaveManager.Shockwave(base.transform.position + Vector3.right * Mathf.Sign(dir.x) * 1.5f, 0.4f, 0.3f, 1.4f);
			this.OnAreaAttackFinished();
			yield break;
		}

		private void OnAreaAttackFinished()
		{
			ElderBrotherBehaviour.ElderBrotherAttackConfig attackConfig = this.GetAttackConfig(ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS.AREA);
			float extraWaitTime = this.GetExtraWaitTime();
			this.StartWaitingPeriod(attackConfig.waitingSecondsAfterAttack + extraWaitTime);
		}

		private float GetExtraWaitTime()
		{
			return Random.Range(this.minExtraWaitTime, this.maxExtraWaitTime);
		}

		private void UnsubscribeFromEverything()
		{
			Debug.Log("UNSUBSCRIBING FROM EVERY EVENT");
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (targetPos.x > this.ElderBrother.transform.position.x)
			{
				if (this.ElderBrother.Status.Orientation == EntityOrientation.Right)
				{
					return;
				}
				this.ElderBrother.SetOrientation(EntityOrientation.Right, true, false);
			}
			else
			{
				if (this.ElderBrother.Status.Orientation == EntityOrientation.Left)
				{
					return;
				}
				this.ElderBrother.SetOrientation(EntityOrientation.Left, true, false);
			}
		}

		public void Death()
		{
			base.StopAllCoroutines();
			this.UnsubscribeFromEverything();
			this.StopMovement();
			this.jumpAttack.StopJump();
			base.BehaviourTree.StopBehaviour();
			this.ElderBrother.AnimatorInyector.Death();
		}

		public override void StopMovement()
		{
		}

		public override void Idle()
		{
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

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
		}

		public Transform introJumpPoint;

		[FoldoutGroup("Attacks", true, 0)]
		public BossJumpAttack jumpAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossAreaSummonAttack areaAttack;

		[FoldoutGroup("Attacks", true, 0)]
		public BossAreaSummonAttack maceImpact;

		[FoldoutGroup("Attacks", true, 0)]
		public List<ElderBrotherBehaviour.ElderBrotherAttackConfig> attacksConfiguration;

		[FoldoutGroup("Traits", true, 0)]
		public EntityMotionChecker motionChecker;

		public GameObject corpseFxPrefab;

		[FoldoutGroup("Debug", true, 0)]
		public ElderBrotherBehaviour.BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS lastAttack;

		public Vector2 jumpOffset;

		public float minExtraWaitTime;

		public float maxExtraWaitTime;

		public bool canRepeatAttack;

		private Transform currentTarget;

		private List<ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS> currentlyAvailableAttacks;

		private List<ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS> queuedActions;

		private Coroutine currentCoroutine;

		[Serializable]
		public struct ElderBrotherAttackConfig
		{
			public ElderBrotherBehaviour.ELDER_BROTHER_ATTACKS attackType;

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

		public enum ELDER_BROTHER_ATTACKS
		{
			JUMP = 1,
			AREA
		}
	}
}
