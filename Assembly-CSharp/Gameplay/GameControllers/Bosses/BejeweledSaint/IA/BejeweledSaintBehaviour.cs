using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.BejeweledSaint.Attack;
using Gameplay.GameControllers.Bosses.BossFight;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Gameplay.GameControllers.Penitent.Gizmos;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint.IA
{
	public class BejeweledSaintBehaviour : EnemyBehaviour
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<BejeweledSaintBehaviour> OnActionFinished;

		public BossFightManager BossFight { get; set; }

		public BejeweledSaintBehaviour.Phases CurrentPhase { get; set; }

		public RootMotionDriver StaffRoot { get; private set; }

		public BejeweledSaintArmAttack ArmAttack { get; set; }

		public int SweepAttacksAmount { get; set; }

		public bool FightStarted { get; set; }

		public bool IsPerformingAttack { get; set; }

		public bool IsBossCollapsed { get; set; }

		public float HandsAttackInterval { get; set; }

		public bool HandsUp
		{
			get
			{
				return this._bejeweledSaintHead.WholeBoss.HandsManager.HandsUp;
			}
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this.SetCurrentAttacks();
			this.stIntro = new BejeweledSaint_StIntro();
			this.stAction = new BejeweledSaint_StAction();
			this.stCollapsed = new BejeweledSaint_StCollapsed();
			this.stChasePlayer = new BejeweledSaint_StChasePlayer();
			this.stDeath = new BejeweledSaint_StDeath();
			this.stMoveToPoint = new BejeweledSaint_StMoveToPoint();
			this._fsm = new StateMachine<BejeweledSaintBehaviour>(this, this.stAction, null, null);
		}

		public override void OnStart()
		{
			base.OnStart();
			this._bejeweledSaintHead = (BejeweledSaintHead)this.Entity;
			this._currentChaseCoolDown = this.ChaseCoolDown;
			this.StaffRoot = this._bejeweledSaintHead.WholeBoss.AttackArm.StaffRoot;
			this.BossFight = UnityEngine.Object.FindObjectOfType<BossFightManager>();
			this.ArmAttack = this._bejeweledSaintHead.WholeBoss.GetComponentInChildren<BejeweledSaintArmAttack>();
			this.VisualSensor.OnPenitentEnter += this.OnVisualSensorPenitentEnter;
			this.VisualSensor.OnPenitentExit += this.OnVisualSensorPenitentExit;
			this.HearingSensor.OnPenitentEnter += this.OnHearingSensorPenitentEnter;
			BsHolderManager holdersManager = this._bejeweledSaintHead.WholeBoss.HoldersManager;
			holdersManager.OnBossCollapse = (Core.SimpleEvent)Delegate.Combine(holdersManager.OnBossCollapse, new Core.SimpleEvent(this.OnBossCollapse));
			BejeweledSaintBoss wholeBoss = this._bejeweledSaintHead.WholeBoss;
			wholeBoss.OnRaised = (Core.SimpleEvent)Delegate.Combine(wholeBoss.OnRaised, new Core.SimpleEvent(this.OnRaised));
			this._bejeweledSaintHead.OnDeath += this.BossOnDeath;
			this.HandsAttackInterval = UnityEngine.Random.Range(this.MinHandsAttackInterval, this.MaxHandsAttackInterval);
			this._bejeweledSaintHead.WholeBoss.SetIntroPosition();
		}

		private void OnDestroy()
		{
			this.VisualSensor.OnPenitentEnter -= this.OnVisualSensorPenitentEnter;
			this.VisualSensor.OnPenitentExit -= this.OnVisualSensorPenitentExit;
			this.HearingSensor.OnPenitentEnter -= this.OnHearingSensorPenitentEnter;
			BsHolderManager holdersManager = this._bejeweledSaintHead.WholeBoss.HoldersManager;
			holdersManager.OnBossCollapse = (Core.SimpleEvent)Delegate.Remove(holdersManager.OnBossCollapse, new Core.SimpleEvent(this.OnBossCollapse));
			BejeweledSaintBoss wholeBoss = this._bejeweledSaintHead.WholeBoss;
			wholeBoss.OnRaised = (Core.SimpleEvent)Delegate.Remove(wholeBoss.OnRaised, new Core.SimpleEvent(this.OnRaised));
			this._bejeweledSaintHead.OnDeath -= this.BossOnDeath;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateAttackTimers();
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

		private void ChangeBossState(BejeweledSaintBehaviour.BOSS_STATES newState)
		{
			this.currentState = newState;
		}

		private void StartAttackAction()
		{
			this.ChangeBossState(BejeweledSaintBehaviour.BOSS_STATES.MID_ACTION);
		}

		private void ActionFinished()
		{
			this.ChangeBossState(BejeweledSaintBehaviour.BOSS_STATES.AVAILABLE_FOR_ACTION);
			if (this.OnActionFinished != null)
			{
				this.OnActionFinished(this);
			}
		}

		public void LaunchAction(BejeweledSaintBehaviour.BEJEWELLED_ATTACKS atk)
		{
			this._currentConfig = this.attacksConfig.Find((BejeweledSaintBehaviour.BejewelledAttackConfig x) => x.atk == atk);
			this.lastAttack = atk;
			switch (atk)
			{
			case BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.STAFF:
				UnityEngine.Debug.Log("STAFF");
				this.IssueStaff();
				break;
			case BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.BEAMS:
				UnityEngine.Debug.Log("BEAMS");
				this.IssueBeams();
				break;
			case BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.HANDS:
				UnityEngine.Debug.Log("HANDS");
				this.IssueHands();
				break;
			case BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.TRIPLE_STAFF:
				UnityEngine.Debug.Log("TRIPLE STAFF");
				this.IssueMultiStaff();
				break;
			case BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.HANDS_LINE:
				UnityEngine.Debug.Log("HANDS LINE");
				this.IssueHandsLine();
				break;
			case BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.ONSLAUGHT:
				UnityEngine.Debug.Log("ONSLAUGHT");
				this.IssueOnslaught();
				break;
			}
			this._currentConfig.ResetCooldown();
		}

		private bool IfAttackOnCooldown(BejeweledSaintBehaviour.BEJEWELLED_ATTACKS atk)
		{
			BejeweledSaintBehaviour.BejewelledAttackConfig bejewelledAttackConfig = this.attacksConfig.Find((BejeweledSaintBehaviour.BejewelledAttackConfig x) => x.atk == atk);
			return !bejewelledAttackConfig.CanBeUsed();
		}

		public BejeweledSaintBehaviour.BEJEWELLED_ATTACKS GetNewAttack()
		{
			BejeweledSaintBehaviour.BEJEWELLED_ATTACKS[] array = new BejeweledSaintBehaviour.BEJEWELLED_ATTACKS[this.currentlyAvailableAttacks.Count];
			this.currentlyAvailableAttacks.CopyTo(array);
			List<BejeweledSaintBehaviour.BEJEWELLED_ATTACKS> list = new List<BejeweledSaintBehaviour.BEJEWELLED_ATTACKS>(array);
			list.Remove(this.lastAttack);
			list.RemoveAll(new Predicate<BejeweledSaintBehaviour.BEJEWELLED_ATTACKS>(this.IfAttackOnCooldown));
			if (this._bejeweledSaintHead.WholeBoss.HandsManager.IsBusy)
			{
				list.Remove(BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.HANDS);
				list.Remove(BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.HANDS_LINE);
				list.Remove(BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.TRIPLE_STAFF);
			}
			if (list.Count == 0)
			{
				list.Add(BejeweledSaintBehaviour.BEJEWELLED_ATTACKS.STAFF);
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public void LaunchRandomAction()
		{
			this.LaunchAction(this.GetNewAttack());
		}

		public bool CanExecuteNewAction()
		{
			return this.currentState == BejeweledSaintBehaviour.BOSS_STATES.AVAILABLE_FOR_ACTION;
		}

		public IEnumerator WaitForState(State<BejeweledSaintBehaviour> st)
		{
			while (!this._fsm.IsInState(st))
			{
				yield return null;
			}
			yield break;
		}

		public void UpdateAttackTimers()
		{
			foreach (BejeweledSaintBehaviour.BejewelledAttackConfig bejewelledAttackConfig in this.attacksConfig)
			{
				if (bejewelledAttackConfig.currentTimer > 0f)
				{
					bejewelledAttackConfig.currentTimer -= Time.deltaTime;
				}
			}
		}

		private void StartWaitingPeriod(float seconds)
		{
			this.ChangeBossState(BejeweledSaintBehaviour.BOSS_STATES.WAITING);
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
			UnityEngine.Debug.Log("WAIT PERIOD FINISH");
			this.ActionFinished();
		}

		public BejeweledSaintBehaviour.Phases GetCurrentPhase()
		{
			if (this.BossFight == null)
			{
				this.CurrentPhase = BejeweledSaintBehaviour.Phases.Phase1;
				return this.CurrentPhase;
			}
			string currentBossPhaseId = this.BossFight.CurrentBossPhaseId;
			if (currentBossPhaseId != null)
			{
				if (!(currentBossPhaseId == "state1"))
				{
					if (!(currentBossPhaseId == "state2"))
					{
						if (currentBossPhaseId == "state3")
						{
							this.CurrentPhase = BejeweledSaintBehaviour.Phases.Phase3;
							this._bejeweledSaintHead.WholeBoss.HoldersManager.holdersToFall = 5;
						}
					}
					else
					{
						this.CurrentPhase = BejeweledSaintBehaviour.Phases.Phase2;
						this._bejeweledSaintHead.WholeBoss.HoldersManager.holdersToFall = 4;
					}
				}
				else
				{
					this.CurrentPhase = BejeweledSaintBehaviour.Phases.Phase1;
				}
			}
			return this.CurrentPhase;
		}

		public float DistanceToTarget(Vector3 target)
		{
			if (this.StaffRoot == null)
			{
				return 0f;
			}
			return Vector2.Distance(this.StaffRoot.transform.position, target);
		}

		public float XDistanceToTarget(Vector3 target)
		{
			if (this.StaffRoot == null)
			{
				return 0f;
			}
			return Mathf.Abs(this.StaffRoot.transform.position.x - target.x);
		}

		private void OnHearingSensorPenitentEnter()
		{
		}

		private void OnVisualSensorPenitentEnter()
		{
		}

		private void OnVisualSensorPenitentExit()
		{
		}

		private void SetCurrentAttacks()
		{
			this.CurrentPhase = this.GetCurrentPhase();
			BejeweledSaintBehaviour.Phases currentPhase = this.CurrentPhase;
			if (currentPhase != BejeweledSaintBehaviour.Phases.Phase1)
			{
				if (currentPhase != BejeweledSaintBehaviour.Phases.Phase2)
				{
					if (currentPhase == BejeweledSaintBehaviour.Phases.Phase3)
					{
						this.currentlyAvailableAttacks = this.Phase3AvailableAttacks;
					}
				}
				else
				{
					this.currentlyAvailableAttacks = this.Phase2AvailableAttacks;
				}
			}
			else
			{
				this.currentlyAvailableAttacks = this.Phase1AvailableAttacks;
			}
		}

		private void OnRaised()
		{
			if (this.IsBossCollapsed)
			{
				this.IsBossCollapsed = false;
			}
			this._fsm.ChangeState(this.stAction);
			this.SetCurrentAttacks();
			this.StartWaitingPeriod(0.5f);
		}

		private void OnBossCollapse()
		{
			if (!this.IsBossCollapsed)
			{
				this.IsBossCollapsed = true;
			}
			this._bejeweledSaintHead.AnimatorInyector.SetCackle(false);
			base.StopCoroutine(this.currentCoroutine);
			this._bejeweledSaintHead.AnimatorInyector.ResetAttack();
			this._fsm.ChangeState(this.stCollapsed);
		}

		private void BossOnDeath()
		{
			this._bejeweledSaintHead.WholeBoss.Audio.PlayDeath();
			base.StopBehaviour();
		}

		public override void Idle()
		{
			base.IsChasing = false;
			this._currentChaseCoolDown = this.ChaseCoolDown;
		}

		public override void Wander()
		{
			throw new NotImplementedException();
		}

		private float DistanceToStaff
		{
			get
			{
				return Mathf.Abs(this._bejeweledSaintHead.WholeBoss.transform.position.x - this.StaffRoot.transform.position.x);
			}
		}

		public override void Chase(Transform targetPosition)
		{
			this._currentChaseCoolDown -= Time.deltaTime;
			if (this._currentChaseCoolDown > 0f)
			{
				return;
			}
			Transform transform = this._bejeweledSaintHead.WholeBoss.transform;
			Vector3 position = this._bejeweledSaintHead.Target.transform.position;
			position.x -= this.DistanceToStaff;
			position.y = transform.position.y;
			Vector3 position2 = Vector3.SmoothDamp(transform.position, position, ref this.velocity, this.smoothTime);
			transform.position = position2;
		}

		public override void Attack()
		{
		}

		public override void Damage()
		{
			this.GetCurrentPhase();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public IEnumerator IntroCoroutine()
		{
			this.FightStarted = true;
			this._bejeweledSaintHead.WholeBoss.IntroRaise();
			yield return new WaitForSeconds(2f);
			this._bejeweledSaintHead.AnimatorInyector.SetCackle(true);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(2f, Vector3.up * 4f, 60, 0.02f, 0f, default(Vector3), 0.045f, true);
			yield return new WaitForSeconds(3f);
			this.EndIntro();
			yield break;
		}

		private void EndIntro()
		{
			this._fsm.ChangeState(this.stAction);
			this.ArmAttack.SetCurrentFailedAttackLimit();
			this.StartWaitingPeriod(1.5f);
		}

		public void StartIntro()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.IntroCoroutine()));
		}

		public void OnIntroEnds()
		{
			this._bejeweledSaintHead.AnimatorInyector.SetCackle(false);
		}

		public bool IsCloseToPoint(Vector2 p)
		{
			return this.XDistanceToTarget(p) < 1f;
		}

		public void MoveTowards(Vector2 p)
		{
			Transform transform = this._bejeweledSaintHead.WholeBoss.transform;
			p.x -= this.DistanceToStaff;
			p.y = transform.position.y;
			Vector3 position = Vector3.SmoothDamp(transform.position, p, ref this.velocity, 0.32f);
			transform.position = position;
		}

		public void StartChasingPlayer()
		{
			this._fsm.ChangeState(this.stChasePlayer);
		}

		public bool IsPlayerInStaffRange()
		{
			return this.DistanceToTarget(base.GetTarget().position) < this.MinChasingDistance;
		}

		public void ChangeToAction()
		{
			this._fsm.ChangeState(this.stAction);
		}

		public void UpdateOffset()
		{
			this.holdersOffset = Mathf.Sin(Time.time * this.offsetFrequency) * this.maxOffset;
			Transform transform = this._bejeweledSaintHead.WholeBoss.transform;
			transform.localPosition = new Vector2(transform.localPosition.x, this._bejeweledSaintHead.WholeBoss.BossHeightPosition.y + this.holdersOffset);
		}

		public void IssueStaff()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.GetIntoStaffRange(new Action(this.StaffAttack))));
		}

		private IEnumerator GetIntoStaffRange(Action OnRangeCallback)
		{
			this.StartChasingPlayer();
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			OnRangeCallback();
			yield break;
		}

		private void StaffAttack()
		{
			this.SetCurrentCoroutine(base.StartCoroutine(this.StaffAttackCoroutine(0.4f, 1.6f, new Action(this.OnStaffAttackEnds))));
		}

		private IEnumerator StaffAttackCoroutine(float aimingSeconds, float recoverySeconds, Action OnAttackEnds)
		{
			this._bejeweledSaintHead.AnimatorInyector.BasicStaffAttack();
			yield return new WaitForSeconds(aimingSeconds);
			this._bejeweledSaintHead.WholeBoss.AttackArm.SetArmAngle();
			if (this.CurrentPhase == BejeweledSaintBehaviour.Phases.Phase3)
			{
				this._bejeweledSaintHead.WholeBoss.CastArm.CastSingleBeamDelayed(this._bejeweledSaintHead.WholeBoss.AttackArm.impactTransform.position, 0.2f);
			}
			yield return new WaitForSeconds(recoverySeconds);
			OnAttackEnds();
			yield break;
		}

		private void OnStaffAttackEnds()
		{
			this._bejeweledSaintHead.WholeBoss.AttackArm.DefaultArmAngle(0.9f);
			this.StartWaitingPeriod(0.5f);
		}

		private void IssueMultiStaff()
		{
			this.StartAttackAction();
			this._bejeweledSaintHead.AnimatorInyector.SetCackle(true);
			UnityEngine.Debug.Log("ISSUE MULTI: WAITING FOR RANGE");
			this.SetCurrentCoroutine(base.StartCoroutine(this.GetIntoStaffRange(new Action(this.MultiStaffAttack))));
		}

		private void MultiStaffAttack()
		{
			UnityEngine.Debug.Log("MULTI STAFF ATTACK");
			this._bejeweledSaintHead.WholeBoss.AttackArm.QuickAttackMode(true);
			this.SetCurrentCoroutine(base.StartCoroutine(this.MultipleStaffAttackCoroutine(3)));
		}

		private IEnumerator MultipleStaffAttackCoroutine(int n)
		{
			while (n > 0)
			{
				yield return base.StartCoroutine(this.StaffAttackCoroutine(0.1f, 1f, new Action(this.OnOneMultiStaffAttackFinished)));
				n--;
			}
			yield return new WaitForSeconds(0.4f);
			this.OnMultiStaffAttackEnds();
			yield break;
		}

		private void OnOneMultiStaffAttackFinished()
		{
		}

		private void OnMultiStaffAttackEnds()
		{
			this._bejeweledSaintHead.AnimatorInyector.SetCackle(false);
			this._bejeweledSaintHead.WholeBoss.AttackArm.DefaultArmAngle(0.9f);
			this._bejeweledSaintHead.WholeBoss.AttackArm.QuickAttackMode(false);
			this.StartWaitingPeriod(0.5f);
		}

		private void IssueOnslaught()
		{
			UnityEngine.Debug.Log("ISSUE ONSLAUGHT");
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.OnslaughtMasterCoroutine()));
		}

		private void StartMoveToPoint(Vector2 p)
		{
			this.movePoint = p;
			this._fsm.ChangeState(this.stMoveToPoint);
		}

		private IEnumerator OnslaughtMasterCoroutine()
		{
			Vector2 originPoint = this._bejeweledSaintHead.WholeBoss.LeftSweepAttackLimitPosition;
			Vector2 endPoint = this._bejeweledSaintHead.WholeBoss.RightSweepAttackLimitPosition;
			int i = 6;
			int counter = 1;
			this.StartMoveToPoint(originPoint);
			yield return base.StartCoroutine(this.WaitForState(this.stAction));
			this._bejeweledSaintHead.AnimatorInyector.SetCackle(true);
			Core.Logic.CameraManager.ProCamera2DShake.Shake(1f, Vector3.up * 7f, 30, 0.02f, 0f, default(Vector3), 0.045f, true);
			yield return new WaitForSeconds(1f);
			this._bejeweledSaintHead.WholeBoss.AttackArm.QuickAttackMode(true);
			while (counter <= i)
			{
				Vector2 nextPoint = Vector2.Lerp(originPoint, endPoint, (float)counter / (float)i);
				this.StartMoveToPoint(nextPoint);
				yield return base.StartCoroutine(this.WaitForState(this.stAction));
				yield return base.StartCoroutine(this.OnslaughtAttackCoroutine());
				counter++;
			}
			this._bejeweledSaintHead.WholeBoss.AttackArm.QuickAttackMode(false);
			this.StartWaitingPeriod(2f);
			this._bejeweledSaintHead.AnimatorInyector.SetCackle(false);
			yield break;
		}

		private IEnumerator OnslaughtAttackCoroutine()
		{
			this._bejeweledSaintHead.AnimatorInyector.BasicStaffAttack();
			yield return new WaitForSeconds(0.3f);
			this._bejeweledSaintHead.WholeBoss.CastArm.CastSingleBeam(this._bejeweledSaintHead.WholeBoss.AttackArm.angleCastCenter.position);
			yield return new WaitForSeconds(0.4f);
			yield break;
		}

		public void IssueHands()
		{
			this.StartAttackAction();
			this.SmashHandsAttack();
			this.StartWaitingPeriod(this._currentConfig.recovery);
		}

		public void IssueHandsLine()
		{
			this.StartAttackAction();
			this.SmashLineHandsAttack();
			this.StartWaitingPeriod(this._currentConfig.recovery);
		}

		public void SmashHandsAttack()
		{
			this._bejeweledSaintHead.WholeBoss.HandsManager.SmashAttack();
		}

		public void SmashLineHandsAttack()
		{
			Vector3 vector = base.GetTarget().position - this._bejeweledSaintHead.transform.position;
			Vector2 dir = Vector2.right * Mathf.Sign(vector.x);
			this._bejeweledSaintHead.WholeBoss.HandsManager.LineAttack(base.transform.position, dir);
		}

		private void IssueBeams()
		{
			this.StartAttackAction();
			this.SetCurrentCoroutine(base.StartCoroutine(this.BeamAttackCoroutine()));
		}

		private IEnumerator BeamAttackCoroutine()
		{
			yield return new WaitForSeconds(0.2f);
			this.DivineBeamAttack();
			this.StartWaitingPeriod(this._currentConfig.recovery);
			yield break;
		}

		private void DivineBeamAttack()
		{
			this._bejeweledSaintHead.WholeBoss.CastArm.DoCastSign();
		}

		[FoldoutGroup("Attack config", true, 0)]
		public List<BejeweledSaintBehaviour.BejewelledAttackConfig> attacksConfig;

		[FoldoutGroup("Debug", true, 0)]
		public BejeweledSaintBehaviour.BOSS_STATES currentState;

		[FoldoutGroup("Debug", true, 0)]
		public BejeweledSaintBehaviour.BEJEWELLED_ATTACKS lastAttack;

		private Coroutine currentCoroutine;

		private List<BejeweledSaintBehaviour.BEJEWELLED_ATTACKS> currentlyAvailableAttacks;

		public List<BejeweledSaintBehaviour.BEJEWELLED_ATTACKS> Phase1AvailableAttacks;

		public List<BejeweledSaintBehaviour.BEJEWELLED_ATTACKS> Phase2AvailableAttacks;

		public List<BejeweledSaintBehaviour.BEJEWELLED_ATTACKS> Phase3AvailableAttacks;

		public StateMachine<BejeweledSaintBehaviour> _fsm;

		public State<BejeweledSaintBehaviour> stIntro;

		public State<BejeweledSaintBehaviour> stAction;

		public State<BejeweledSaintBehaviour> stChasePlayer;

		public State<BejeweledSaintBehaviour> stCollapsed;

		public State<BejeweledSaintBehaviour> stMoveToPoint;

		public State<BejeweledSaintBehaviour> stDeath;

		private BejeweledSaintHead _bejeweledSaintHead;

		public float ChaseCoolDown;

		private float _currentChaseCoolDown;

		public float holdersOffset;

		public float maxOffset = 1f;

		public float offsetFrequency = 1f;

		public float MinChasingDistance;

		public float smoothTime = 0.3f;

		public Vector2 movePoint;

		private Vector3 velocity = Vector3.zero;

		private float onslaughtCounter;

		public float MinHandsAttackInterval;

		public float MaxHandsAttackInterval;

		private BejeweledSaintBehaviour.BejewelledAttackConfig _currentConfig;

		public enum BOSS_STATES
		{
			WAITING,
			MID_ACTION,
			AVAILABLE_FOR_ACTION
		}

		public enum BEJEWELLED_ATTACKS
		{
			STAFF,
			BEAMS,
			HANDS,
			TRIPLE_STAFF,
			HANDS_LINE,
			ONSLAUGHT
		}

		[Serializable]
		public class BejewelledAttackConfig
		{
			public bool CanBeUsed()
			{
				return this.currentTimer <= 0f;
			}

			public void ResetCooldown()
			{
				this.currentTimer = this.cooldown;
			}

			public BejeweledSaintBehaviour.BEJEWELLED_ATTACKS atk;

			public float cooldown;

			public float currentTimer;

			public float recovery;
		}

		public enum Phases
		{
			Phase1,
			Phase2,
			Phase3
		}
	}
}
