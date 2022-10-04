using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.DrownedCorpse.AI.DrownedCorpseStates
{
	public class DrownedCorpseChaseState : State
	{
		private DrownedCorpse DrownedCorpse { get; set; }

		private DrownedCorpseBehaviour Behaviour { get; set; }

		public override void OnStateInitialize(StateMachine machine)
		{
			base.OnStateInitialize(machine);
			this.DrownedCorpse = machine.GetComponent<DrownedCorpse>();
			this.Behaviour = this.DrownedCorpse.GetComponent<DrownedCorpseBehaviour>();
		}

		public override void OnStateEnter()
		{
			base.OnStateEnter();
			this.Behaviour.LookAtTarget(this.DrownedCorpse.Target.transform.position);
			this.DrownedCorpse.AnimatorInyector.Awake();
			this.DrownedCorpse.AnimatorInyector.DontRun();
			this.isSetChaseDirection = false;
			this.timeoutBeforeChase = 0f;
		}

		public override void Update()
		{
			base.Update();
			this.timeoutBeforeChase += Time.deltaTime;
			if (this.timeoutBeforeChase < this.Behaviour.MaxTimeAwaitingBeforeChase)
			{
				return;
			}
			this.DrownedCorpse.AnimatorInyector.Run();
			Vector3 position = this.DrownedCorpse.Target.transform.position;
			if (this.DrownedCorpse.Animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
			{
				this.Chase(position);
			}
		}

		public override void OnStateExit()
		{
			base.OnStateExit();
			this.DrownedCorpse.Behaviour.IsChasing = false;
		}

		private void Chase(Vector3 position)
		{
			if (!this.DrownedCorpse.MotionChecker.HitsFloor || this.DrownedCorpse.MotionChecker.HitsBlock || this.DrownedCorpse.Status.Dead)
			{
				this.Behaviour.StopMovement();
				return;
			}
			this.DrownedCorpse.Behaviour.IsChasing = true;
			float horizontalInput = (this.DrownedCorpse.Status.Orientation != EntityOrientation.Right) ? -1f : 1f;
			this.DrownedCorpse.Input.HorizontalInput = horizontalInput;
		}

		private bool isSetChaseDirection;

		private float timeoutBeforeChase;
	}
}
