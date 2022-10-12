using System;
using System.Diagnostics;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.PietyMonster.Attack;
using Gameplay.GameControllers.Enemies.Framework.IA;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.PietyMonster.IA
{
	public class PietyMonsterBehaviour : EnemyBehaviour
	{
		public bool Awake { get; set; }

		public bool TargetOnRange { get; set; }

		public bool ReadyToAttack { get; set; }

		public bool Attacking { get; set; }

		public bool Spiting { get; set; }

		public PietyStompAttack StompAttack { get; private set; }

		public PietyClawAttack ClawAttack { get; private set; }

		public PietySmashAttack SmashAttack { get; private set; }

		public PietySpitAttack SpitAttack { get; set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Core.SimpleEvent OnBehaviourChange;

		public bool StatusChange { get; set; }

		public bool CanSpit
		{
			get
			{
				return this.StompAttackCounter > 1;
			}
		}

		public override void OnAwake()
		{
			base.OnAwake();
			this._pietyMonster = (PietyMonster)this.Entity;
			this.StompAttack = this._pietyMonster.GetComponentInChildren<PietyStompAttack>();
			this.ClawAttack = this._pietyMonster.GetComponentInChildren<PietyClawAttack>();
			this.SmashAttack = this._pietyMonster.GetComponentInChildren<PietySmashAttack>();
			this.SpitAttack = this._pietyMonster.GetComponentInChildren<PietySpitAttack>();
		}

		public override void OnStart()
		{
			base.OnStart();
			this._pietyMonster.OnDeath += this.PietyMonsterOnEntityDie;
			this._gameLogic = Core.Logic;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (this._gameLogic.CurrentState)
			{
			case LogicStates.Unresponsive:
				break;
			case LogicStates.Playing:
				break;
			case LogicStates.PlayerDead:
				this.Idle();
				if (base.BehaviourTree.isRunning)
				{
					base.BehaviourTree.StopBehaviour();
				}
				this._pietyMonster.AnimatorInyector.ResetAttacks();
				break;
			case LogicStates.Console:
				break;
			case LogicStates.Pause:
				break;
			case LogicStates.PopUp:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (this._pietyMonster.Stats.Life.MissingRatio >= 0.9f)
			{
				if (this._pietyMonster.CurrentBossStatus == PietyMonster.BossStatus.First)
				{
					return;
				}
				this._pietyMonster.CurrentBossStatus = PietyMonster.BossStatus.First;
			}
			if (this.Attacking || base.IsAttacking || this.lastAttackWasArea)
			{
				return;
			}
			if (this._pietyMonster.Stats.Life.MissingRatio >= 0.8f)
			{
				this.SetNewBossStatus(PietyMonster.BossStatus.Second);
			}
			else if (this._pietyMonster.Stats.Life.MissingRatio >= 0.6f)
			{
				this.SetNewBossStatus(PietyMonster.BossStatus.Third);
			}
			else if (this._pietyMonster.Stats.Life.MissingRatio >= 0.4f)
			{
				this.SetNewBossStatus(PietyMonster.BossStatus.Forth);
			}
			else if (this._pietyMonster.Stats.Life.MissingRatio >= 0.2f)
			{
				this.SetNewBossStatus(PietyMonster.BossStatus.Fifth);
			}
			else if (this._pietyMonster.Stats.Life.MissingRatio >= 0.05f)
			{
				this.SetNewBossStatus(PietyMonster.BossStatus.Sixth);
			}
		}

		private void SetNewBossStatus(PietyMonster.BossStatus currentStatus)
		{
			if (this._pietyMonster.CurrentBossStatus == currentStatus)
			{
				return;
			}
			this._pietyMonster.CurrentBossStatus = currentStatus;
			this._pietyMonster.AnimatorInyector.AreaAttack();
			this.OnBossBehaviourChange();
			this.lastAttackWasArea = true;
		}

		public override void Idle()
		{
			this.StopMovement();
			this._pietyMonster.AnimatorInyector.Idle();
		}

		public override void Wander()
		{
		}

		public override void Chase(Transform target)
		{
			if (this._pietyMonster == null)
			{
				return;
			}
			float num = (this._pietyMonster.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this._pietyMonster.Inputs.HorizontalInput = ((!this._pietyMonster.CanMove) ? 0f : num);
			this._pietyMonster.AnimatorInyector.Walk();
		}

		public override void Attack()
		{
			this.StopMovement();
			if (this.Attacking || !this.TargetOnRange)
			{
				return;
			}
			this.Attacking = true;
			float value = UnityEngine.Random.value;
			if (value <= 0.45f)
			{
				this._pietyMonster.AnimatorInyector.StompAttack();
				this.StompAttackCounter++;
			}
			else
			{
				this._pietyMonster.AnimatorInyector.ClawAttack();
			}
			this.lastAttackWasArea = false;
		}

		public void Spit()
		{
			this.StopMovement();
			this._pietyMonster.AnimatorInyector.SpitAttack();
		}

		public override void Damage()
		{
		}

		public override void StopMovement()
		{
			this._pietyMonster.Inputs.HorizontalInput = 0f;
			this._pietyMonster.AnimatorInyector.Stop();
		}

		public void StopTurning()
		{
			if (base.TurningAround)
			{
				base.TurningAround = !base.TurningAround;
			}
			this._pietyMonster.AnimatorInyector.StopTurning();
		}

		public override void LookAtTarget(Vector3 targetPos)
		{
			if (this._pietyMonster.Animator.GetNextAnimatorStateInfo(0).IsName("Walk"))
			{
				float num = (this._pietyMonster.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
				this._pietyMonster.Inputs.HorizontalInput = ((!this._pietyMonster.CanMove) ? 0f : num);
			}
			else
			{
				this.StopMovement();
			}
			this._pietyMonster.AnimatorInyector.TurnAround();
		}

		private void PietyMonsterOnEntityDie()
		{
			this.StopMovement();
			base.BehaviourTree.StopBehaviour();
			this._pietyMonster.AnimatorInyector.Death();
		}

		private void OnDestroy()
		{
			this._pietyMonster.OnDeath -= this.PietyMonsterOnEntityDie;
		}

		protected virtual void OnBossBehaviourChange()
		{
			Core.SimpleEvent onBehaviourChange = this.OnBehaviourChange;
			if (onBehaviourChange != null)
			{
				onBehaviourChange();
			}
		}

		private PietyMonster _pietyMonster;

		private LogicManager _gameLogic;

		public int StompAttackCounter;

		private bool lastAttackWasArea;
	}
}
