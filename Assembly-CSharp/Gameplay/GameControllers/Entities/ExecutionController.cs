using System;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Penitent.Abilities;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class ExecutionController
	{
		public ExecutionController()
		{
			this._killedEnemyList = new List<Enemy>();
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
			Entity.Death += this.OnEnemyDeath;
			Enemy.OnExecutionFired = (Core.SimpleEvent)Delegate.Combine(Enemy.OnExecutionFired, new Core.SimpleEvent(this.OnExecutionFired));
		}

		public bool CurrentEnemyCanBeExecuted { get; private set; }

		private void OnExecutionFired()
		{
			this.ClearKilledEntityList();
			this.SetCurrentPointer();
			this.CurrentEnemyCanBeExecuted = false;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			this.SetCurrentPointer();
		}

		private void OnEnemyDeath(Entity entity)
		{
			this.AddKilledEnemyToList(entity);
			this.SetExecutionFlag();
		}

		private void SetExecutionFlag()
		{
			int count = this._killedEnemyList.Count;
			if (count == 0)
			{
				return;
			}
			if (count >= this._currentPointer - 1)
			{
				this.CurrentEnemyCanBeExecuted = true;
			}
		}

		private void AddKilledEnemyToList(Entity entity)
		{
			if (!entity.IsExecutable)
			{
				return;
			}
			Enemy item = (Enemy)entity;
			if (!this._killedEnemyList.Contains(item))
			{
				this._killedEnemyList.Add(item);
			}
		}

		public void ClearKilledEntityList()
		{
			if (this._killedEnemyList.Count > 0)
			{
				this._killedEnemyList.Clear();
			}
		}

		public void SetCurrentPointer()
		{
			Combo combo = Core.Logic.Penitent.PenitentAttack.Combo;
			if (combo.IsAvailable)
			{
				UnlockableSkill getMaxSkill = combo.GetMaxSkill;
				if (getMaxSkill.id.Equals("COMBO_1"))
				{
					this._currentPointer = Random.Range(combo.FirstUpgradeExecutionTier.MinExecutionTier, combo.FirstUpgradeExecutionTier.MaxExecutionTier);
				}
				else
				{
					this._currentPointer = Random.Range(combo.SecondUpgradeExecutionTier.MinExecutionTier, combo.SecondUpgradeExecutionTier.MaxExecutionTier);
				}
			}
			else
			{
				Combo.ExecutionTier defaulExecutionTier = combo.DefaulExecutionTier;
				this._currentPointer = Random.Range(defaulExecutionTier.MinExecutionTier, defaulExecutionTier.MaxExecutionTier);
			}
		}

		private List<Enemy> _killedEnemyList;

		private int _currentPointer;
	}
}
