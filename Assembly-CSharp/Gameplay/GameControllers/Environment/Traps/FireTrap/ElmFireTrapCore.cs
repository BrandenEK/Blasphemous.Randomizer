using System;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.FireTrap
{
	public class ElmFireTrapCore
	{
		public ElmFireTrapCore(ElmFireTrap fireTrap)
		{
			this._fireTrap = fireTrap;
		}

		public bool IsActive { get; set; }

		public void SetCurrentCycleCooldownToMax()
		{
			this._currentCycleCooldown = this._fireTrap.lightningCycleCooldown;
		}

		public void Update()
		{
			if (this._isLightningCycleRunning)
			{
				return;
			}
			this._currentCycleCooldown += Time.deltaTime;
			if (this._currentCycleCooldown >= this._fireTrap.lightningCycleCooldown)
			{
				this._currentCycleCooldown = 0f;
				this.StartLightningCycle();
			}
		}

		public void StartLightningCycle()
		{
			this._isLightningCycleRunning = true;
			this._fireTrap.ChargeLightnings();
		}

		public void ResetCycle()
		{
			this._isLightningCycleRunning = false;
		}

		private ElmFireTrap _fireTrap;

		private float _currentCycleCooldown;

		private bool _isLightningCycleRunning;
	}
}
