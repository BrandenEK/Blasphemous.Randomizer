using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class MapShow : Trait
	{
		protected override void OnAwake()
		{
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
			this.forceUpdate = true;
			this.lastDigPos = Vector3.zero;
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		public void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.forceUpdate = true;
			this.lastDigPos = Vector3.zero;
		}

		protected override void OnUpdate()
		{
			bool flag = this.forceUpdate;
			if (!flag)
			{
				this.currentWaitTime += Time.deltaTime;
				if (this.currentWaitTime > this.updateTime)
				{
					Vector3 position = base.EntityOwner.transform.position;
					Vector3 vector = this.lastDigPos - position;
					this.currentWaitTime = 0f;
					flag = (vector.sqrMagnitude >= this.minMovementToDig * this.minMovementToDig);
				}
			}
			if (flag)
			{
				this.currentWaitTime = 0f;
				this.lastDigPos = base.EntityOwner.transform.position;
				Core.NewMapManager.RevealCellInPosition(this.lastDigPos);
				this.forceUpdate = false;
			}
		}

		[SerializeField]
		private float updateTime = 0.2f;

		[SerializeField]
		private float minMovementToDig = 5f;

		[SerializeField]
		private Vector3 sizeBound;

		private bool forceUpdate;

		private float currentWaitTime;

		private Vector3 lastDigPos;
	}
}
