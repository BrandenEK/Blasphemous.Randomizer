using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.Generic.Attacks;
using Gameplay.GameControllers.Bosses.HighWills.Attack;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Maikel.StatelessFSM;
using Plugins.Maikel.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.HighWills
{
	[RequireComponent(typeof(HighWills))]
	public class HighWillsBehaviour : EnemyBehaviour
	{
		public override void OnAwake()
		{
			base.OnAwake();
			this.HighWills = base.GetComponent<HighWills>();
			this.fireMachineGun = base.GetComponentInChildren<BossMachinegunShooter>();
			this.mineAttackAction = new HighWillsBehaviour.HighWillsMineAttack_EnemyAction();
			this.blastAttackAction = new HighWillsBehaviour.HighWillsBlastAttack_EnemyAction();
			this.deathAction = new HighWillsBehaviour.HighWillsDeath_EnemyAction();
			this.introAction = new HighWillsBehaviour.HighWillsIntro_EnemyAction();
			this.stInactive = new HighWillsBehaviour.HighWillsSt_Inactive();
			this.stWaiting = new HighWillsBehaviour.HighWillsSt_Wait();
			this.stAction = new HighWillsBehaviour.HighWillsSt_Action();
			this._fsm = new StateMachine<HighWillsBehaviour>(this, this.stInactive, null, null);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this._fsm.DoUpdate();
		}

		protected void UpdateActionState()
		{
		}

		protected void UpdateWaitState()
		{
			this.UpdateTimers();
			if (Core.Logic.Penitent && this.HighWills.VisionCone.CanSeeTarget(Core.Logic.Penitent.transform, "Penitent", false) && this.CanAttack())
			{
				this.ResetAttackTimer();
				this.ChooseAttacksAndLaunch();
			}
		}

		protected void ChooseAttacksAndLaunch()
		{
			if (this.chosenAttacks.Count == 0)
			{
				this.GetNewChosenAttacks();
			}
			HighWillsBehaviour.HW_ATTACKS hw_ATTACKS = this.chosenAttacks[0];
			if (hw_ATTACKS != HighWillsBehaviour.HW_ATTACKS.MINES_1)
			{
				if (hw_ATTACKS == HighWillsBehaviour.HW_ATTACKS.BLAST)
				{
					this.LaunchBlastAttackAction();
				}
			}
			else
			{
				this.LaunchMineAttackAction();
			}
			this.chosenAttacks.RemoveAt(0);
		}

		private void GetNewChosenAttacks()
		{
			float weightsSum = 0f;
			this.AttacksAndWeights.ForEach(delegate(HighWillsBehaviour.HWAttacksWithWeight x)
			{
				weightsSum += x.Weight;
			});
			float num = UnityEngine.Random.Range(0f, weightsSum);
			weightsSum = 0f;
			for (int i = 0; i < this.AttacksAndWeights.Count; i++)
			{
				this.chosenAttacks = new List<HighWillsBehaviour.HW_ATTACKS>(this.AttacksAndWeights[i].Attacks);
				weightsSum += this.AttacksAndWeights[i].Weight;
				if (weightsSum > num)
				{
					break;
				}
			}
		}

		protected void UpdateInactiveState()
		{
		}

		private bool CanAttack()
		{
			return this.attackTimer <= 0f;
		}

		private void ResetAttackTimer()
		{
			this.attackTimer = this.attackCD;
		}

		private void UpdateTimers()
		{
			if (this.attackTimer > 0f)
			{
				this.attackTimer -= Time.deltaTime;
			}
		}

		private void DoProjectileAttack()
		{
			this.fireMachineGun.StartAttack(Core.Logic.Penitent.transform);
		}

		private void DoMineAttack()
		{
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.MineShooter1, this.MineShootingPoint1.position, Quaternion.identity, true, 3);
			RangedMineShooter component = objectInstance.GameObject.GetComponent<RangedMineShooter>();
			component.StartShootingMines(this.MineShootingPoint1.position);
		}

		private void DoBlastAttack()
		{
			Vector3 position = Core.Logic.Penitent.GetPosition();
			position.y = this.BlastShootingPointForHeight.position.y;
			this.BlastAttack.SummonAreaOnPoint(position, 0f, 1f, null);
		}

		protected void LaunchMineAttackAction()
		{
			this.LaunchAttackAction(this.mineAttackAction);
		}

		protected void LaunchBlastAttackAction()
		{
			this.LaunchAttackAction(this.blastAttackAction);
		}

		protected void LaunchAttackAction(EnemyAction attack)
		{
			this.StopCurrentAction();
			this._fsm.ChangeState(this.stAction);
			this.currentAction = attack.StartAction(this);
			this.SuscribeToActionEvents();
		}

		public void LaunchIntroAction()
		{
			this.StopCurrentAction();
			this._fsm.ChangeState(this.stAction);
			this.currentAction = this.introAction.StartAction(this);
			this.SuscribeToActionEvents();
			this.ClearMines();
		}

		private void ClearMines()
		{
			RangedMine[] array = UnityEngine.Object.FindObjectsOfType<RangedMine>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(false);
			}
		}

		public void LaunchDeathAction()
		{
			this.StopCurrentAction();
			this._fsm.ChangeState(this.stInactive);
			this.currentAction = this.deathAction.StartAction(this);
			this.currentAction.OnActionEnds -= this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped -= this.CurrentAction_OnActionStops;
		}

		protected void SuscribeToActionEvents()
		{
			this.currentAction.OnActionEnds -= this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped -= this.CurrentAction_OnActionStops;
			this.currentAction.OnActionEnds += this.CurrentAction_OnActionEnds;
			this.currentAction.OnActionIsStopped += this.CurrentAction_OnActionStops;
		}

		private void CurrentAction_OnActionStops(EnemyAction e)
		{
			this._fsm.ChangeState(this.stWaiting);
		}

		private void CurrentAction_OnActionEnds(EnemyAction e)
		{
			this._fsm.ChangeState(this.stWaiting);
		}

		private void StopCurrentAction()
		{
			if (this.currentAction != null)
			{
				this.currentAction.StopAction();
			}
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

		public HighWills HighWills;

		public HighWillsBossScrollManager ScrollManager;

		public BossMachinegunShooter fireMachineGun;

		public GameObject MineShooter1;

		public Transform MineShootingPoint1;

		public BossAreaSummonAttack BlastAttack;

		public Transform BlastShootingPointForHeight;

		public List<HighWillsBehaviour.HWAttacksWithWeight> AttacksAndWeights = new List<HighWillsBehaviour.HWAttacksWithWeight>();

		private StateMachine<HighWillsBehaviour> _fsm;

		private State<HighWillsBehaviour> stWaiting;

		private State<HighWillsBehaviour> stAction;

		private State<HighWillsBehaviour> stInactive;

		private EnemyAction currentAction;

		private EnemyAction mineAttackAction;

		private EnemyAction blastAttackAction;

		private EnemyAction introAction;

		private EnemyAction deathAction;

		private List<HighWillsBehaviour.HW_ATTACKS> chosenAttacks = new List<HighWillsBehaviour.HW_ATTACKS>();

		private float attackCD = 4f;

		private float attackTimer;

		public enum HW_ATTACKS
		{
			MINES_1,
			BLAST
		}

		[Serializable]
		public struct HWAttacksWithWeight
		{
			public List<HighWillsBehaviour.HW_ATTACKS> Attacks;

			public float Weight;
		}

		public class HighWillsSt_Inactive : State<HighWillsBehaviour>
		{
			public override void Enter(HighWillsBehaviour owner)
			{
			}

			public override void Execute(HighWillsBehaviour owner)
			{
				owner.UpdateInactiveState();
			}

			public override void Exit(HighWillsBehaviour owner)
			{
			}
		}

		public class HighWillsSt_Wait : State<HighWillsBehaviour>
		{
			public override void Enter(HighWillsBehaviour owner)
			{
			}

			public override void Execute(HighWillsBehaviour owner)
			{
				owner.UpdateWaitState();
			}

			public override void Exit(HighWillsBehaviour owner)
			{
			}
		}

		public class HighWillsSt_Action : State<HighWillsBehaviour>
		{
			public override void Enter(HighWillsBehaviour owner)
			{
			}

			public override void Execute(HighWillsBehaviour owner)
			{
				owner.UpdateActionState();
			}

			public override void Exit(HighWillsBehaviour owner)
			{
			}
		}

		public class HighWillsIntro_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				this.ACT_MOVE.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				HighWillsBehaviour o = this.owner as HighWillsBehaviour;
				o.HighWills.Stats.Life.SetToCurrentMax();
				this.ACT_MOVE.StartAction(o, o.transform.position + new Vector3(-10f, 0f, 0f), 2f, Ease.OutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				this.ACT_MOVE.StartAction(o, o.transform.position + new Vector3(5f, 0f, 0f), 4f, Ease.InOutQuad, null, true, null, true, true, 1.7f);
				yield return this.ACT_MOVE.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();

			private MoveEasing_EnemyAction ACT_MOVE = new MoveEasing_EnemyAction();
		}

		public class HighWillsMineAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				HighWillsBehaviour o = this.owner as HighWillsBehaviour;
				o.HighWills.ActivateMiddleHWEyes(0.2f, 2f, 0.2f);
				this.ACT_WAIT.StartAction(this.owner, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.DoMineAttack();
				this.ACT_WAIT.StartAction(this.owner, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class HighWillsBlastAttack_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				HighWillsBehaviour o = this.owner as HighWillsBehaviour;
				o.HighWills.ActivateLeftHWEyes(0.2f, 1f, 0.2f);
				this.ACT_WAIT.StartAction(this.owner, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				o.DoBlastAttack();
				this.ACT_WAIT.StartAction(this.owner, 0.2f);
				yield return this.ACT_WAIT.waitForCompletion;
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}

		public class HighWillsDeath_EnemyAction : EnemyAction
		{
			protected override void DoOnStop()
			{
				this.ACT_WAIT.StopAction();
				base.DoOnStop();
			}

			protected override IEnumerator BaseCoroutine()
			{
				HighWillsBehaviour o = this.owner as HighWillsBehaviour;
				o.ScrollManager.Stop();
				Core.Logic.Penitent.Physics.EnablePhysics(false);
				this.ACT_WAIT.StartAction(this.owner, 0.1f);
				yield return this.ACT_WAIT.waitForCompletion;
				PlayMakerFSM.BroadcastEvent("BOSS DEAD");
				base.FinishAction();
				yield break;
			}

			private WaitSeconds_EnemyAction ACT_WAIT = new WaitSeconds_EnemyAction();
		}
	}
}
