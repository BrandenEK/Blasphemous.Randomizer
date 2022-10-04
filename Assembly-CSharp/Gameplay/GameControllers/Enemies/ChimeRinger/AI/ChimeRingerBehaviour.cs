using System;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Sirenix.OdinInspector;
using Tools.Level.Actionables;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.ChimeRinger.AI
{
	public class ChimeRingerBehaviour : EnemyBehaviour
	{
		public ChimeRinger ChimeRinger { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.ChimeRinger = (ChimeRinger)this.Entity;
			this.ringLapse = this.trapTriggerer.trapManager.GetSceneTrapLapse();
			this._currentRingLapse = this.ringLapse - this.trapTriggerer.trapManager.GetFirstTrapLapse();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.ChimeRinger.Status.Dead)
			{
				return;
			}
			this._currentRingLapse += Time.deltaTime;
			if (this._currentRingLapse >= this.ringLapse)
			{
				this._currentRingLapse = 0f;
				this.RingTheBell();
			}
		}

		public override void Idle()
		{
		}

		public override void Wander()
		{
		}

		public override void Chase(Transform targetPosition)
		{
		}

		public override void Attack()
		{
		}

		private void RingTheBell()
		{
			this.ChimeRinger.AnimatorInyector.Ring();
		}

		public void TriggerAllTraps()
		{
			this.trapTriggerer.TriggerAllTrapsInTheScene();
		}

		public override void Damage()
		{
			if (this.ChimeRinger == null)
			{
				return;
			}
			this.ChimeRinger.AnimatorInyector.Hurt();
		}

		public void Death()
		{
			if (this.ChimeRinger == null)
			{
				return;
			}
			this.ChimeRinger.AnimatorInyector.Death();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public GlobalTrapTriggerer trapTriggerer;

		[FoldoutGroup("Overwritten by scene trap manager", false, 0)]
		public float ringLapse = 5f;

		private float _currentRingLapse;
	}
}
