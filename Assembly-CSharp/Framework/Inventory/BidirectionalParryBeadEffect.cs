using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Attack;
using UnityEngine;

namespace Framework.Inventory
{
	public class BidirectionalParryBeadEffect : ObjectEffect
	{
		private PenitentSword _penitentSword { get; set; }

		private Parry _parryAbility { get; set; }

		private bool IsEquiped { get; set; }

		private void LoadDependencies()
		{
			this._penitentSword = (PenitentSword)Core.Logic.Penitent.PenitentAttack.CurrentPenitentWeapon;
			this._parryAbility = Core.Logic.Penitent.Parry;
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
			LevelManager.OnBeforeLevelLoad += this.BeforeLevelLoad;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.LoadDependencies();
			this._penitentSword.OnParry += this.OnParry;
		}

		private void BeforeLevelLoad(Level oldlevel, Level newlevel)
		{
			if (this._penitentSword)
			{
				this._penitentSword.OnParry -= this.OnParry;
			}
		}

		private void OnParry(object param)
		{
			if (!this.IsEquiped)
			{
				return;
			}
			Entity entity = (Entity)param;
			Vector3 position = entity.transform.position;
			BidirectionalParryBeadEffect.FaceToEnemy(position);
		}

		protected override bool OnApplyEffect()
		{
			this.IsEquiped = true;
			if (this._penitentSword)
			{
				this._penitentSword.CheckParryEnemyDirection = false;
			}
			return true;
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			if (this._penitentSword)
			{
				this._penitentSword.CheckParryEnemyDirection = true;
			}
			this.IsEquiped = false;
		}

		private static void FaceToEnemy(Vector3 enemyPosition)
		{
			Core.Logic.Penitent.SetOrientationbyHit(enemyPosition);
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			LevelManager.OnBeforeLevelLoad -= this.BeforeLevelLoad;
		}
	}
}
