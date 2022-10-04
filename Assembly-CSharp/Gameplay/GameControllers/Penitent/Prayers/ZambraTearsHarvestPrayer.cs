using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Prayers
{
	public class ZambraTearsHarvestPrayer : MonoBehaviour
	{
		private void Start()
		{
			this.CreatePoolEffect();
		}

		public void EnableEffect()
		{
			this.InstantiateEffect();
		}

		public void DisableEffect()
		{
			this.DisposeEffect();
		}

		private void CreatePoolEffect()
		{
			if (this.ZambraEffectPrefab)
			{
				PoolManager.Instance.CreatePool(this.ZambraEffectPrefab, 1);
			}
		}

		private void InstantiateEffect()
		{
			if (!this.ZambraEffectPrefab)
			{
				return;
			}
			Vector3 position = Core.Logic.Penitent.GetPosition();
			this.zambraEffect = PoolManager.Instance.ReuseObject(this.ZambraEffectPrefab, position, Quaternion.identity, false, 1).GameObject;
		}

		private void DisposeEffect()
		{
			if (!this.zambraEffect)
			{
				return;
			}
			TearHarvestEffect componentInChildren = this.zambraEffect.GetComponentInChildren<TearHarvestEffect>();
			if (componentInChildren)
			{
				componentInChildren.Dispose();
			}
		}

		public GameObject ZambraEffectPrefab;

		private GameObject zambraEffect;
	}
}
