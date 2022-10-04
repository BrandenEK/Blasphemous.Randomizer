using System;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Inventory
{
	public class PrayerUseVfx : ObjectEffect_Stat
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			if (this.Vfx != null)
			{
				PoolManager.Instance.CreatePool(this.Vfx, 1);
			}
		}

		protected override bool OnApplyEffect()
		{
			this.InstantiateEffect();
			return base.OnApplyEffect();
		}

		private void InstantiateEffect()
		{
			if (this.Vfx == null)
			{
				return;
			}
			Vector3 position = Core.Logic.Penitent.transform.position;
			PoolManager.Instance.ReuseObject(this.Vfx, position, Quaternion.identity, false, 1);
		}

		[BoxGroup("VFX", true, false, 0)]
		public GameObject Vfx;
	}
}
