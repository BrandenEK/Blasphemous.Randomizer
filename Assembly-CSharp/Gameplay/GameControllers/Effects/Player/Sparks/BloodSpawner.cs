using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Sparks
{
	public class BloodSpawner : Trait
	{
		public GameObject GetBloodFX(BloodSpawner.BLOOD_FX_TYPES bloodType)
		{
			BloodFXTableElement randomElementOfType = this.bloodVFXTable.GetRandomElementOfType(bloodType);
			return PoolManager.Instance.ReuseObject(randomElementOfType.prefab, base.transform.position, base.transform.rotation, false, 1).GameObject;
		}

		protected override void OnAwake()
		{
			base.OnAwake();
			this.InitPool();
		}

		private void InitPool()
		{
			foreach (BloodFXTableElement bloodFXTableElement in this.bloodVFXTable.bloodVFXList)
			{
				PoolManager.Instance.CreatePool(bloodFXTableElement.prefab, bloodFXTableElement.poolSize);
			}
		}

		public BloodVFXTable bloodVFXTable;

		public enum BLOOD_FX_TYPES
		{
			SMALL,
			MEDIUM,
			BIG
		}
	}
}
