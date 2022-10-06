using System;
using System.Collections.Generic;
using Gameplay.GameControllers.Enemies.Framework.IA;
using Tools.Level.Actionables;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.CauldronNun.AI
{
	public class CauldronNunBehaviour : EnemyBehaviour
	{
		public CauldronNun CauldronNun { get; private set; }

		public override void OnStart()
		{
			base.OnStart();
			this.CauldronNun = (CauldronNun)this.Entity;
			this._currentPullLapse = this.pullLapse;
			this._cauldronTraps = new List<CauldronTrap>(Object.FindObjectsOfType<CauldronTrap>());
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.CauldronNun.Status.Dead)
			{
				return;
			}
			this._currentPullLapse += Time.deltaTime;
			if (this._currentPullLapse >= this.pullLapse)
			{
				this._currentPullLapse = 0f;
				this.PullChain();
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

		private void PullChain()
		{
			this.CauldronNun.AnimatorInyector.PullChain();
		}

		public void TriggerAllTraps()
		{
			foreach (CauldronTrap cauldronTrap in this._cauldronTraps)
			{
				cauldronTrap.Use();
			}
		}

		public override void ReadSpawnerConfig(SpawnBehaviourConfig config)
		{
			base.ReadSpawnerConfig(config);
			this.pullLapse = config.TryGetFloat("DELAY");
		}

		public override void Damage()
		{
			if (this.CauldronNun == null)
			{
				return;
			}
			this.CauldronNun.AnimatorInyector.Hurt();
		}

		public void Death()
		{
			if (this.CauldronNun == null)
			{
				return;
			}
			this.CauldronNun.AnimatorInyector.Death();
		}

		public override void StopMovement()
		{
			throw new NotImplementedException();
		}

		public GlobalTrapTriggerer trapTriggerer;

		public float pullLapse = 5f;

		private float _currentPullLapse;

		private List<CauldronTrap> _cauldronTraps;
	}
}
