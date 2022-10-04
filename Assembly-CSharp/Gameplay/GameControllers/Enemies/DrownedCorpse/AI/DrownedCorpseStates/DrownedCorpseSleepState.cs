using System;
using Gameplay.GameControllers.Entities.StateMachine;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.DrownedCorpse.AI.DrownedCorpseStates
{
	public class DrownedCorpseSleepState : State
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
			this.DrownedCorpse.DamageByContact = false;
			this.Behaviour.StopMovement(0.75f);
		}

		public override void Update()
		{
			base.Update();
			if (!this.DrownedCorpse.MotionChecker.HitsFloor || this.DrownedCorpse.MotionChecker.HitsBlock)
			{
				this.Behaviour.StopMovement();
			}
		}

		private static readonly int Run = Animator.StringToHash("RUN");
	}
}
