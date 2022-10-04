using System;
using Framework.Managers;
using Framework.Pooling;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Prayers
{
	public class TearHarvestEffect : PoolObject
	{
		private void LateUpdate()
		{
			Vector3 position = Core.Logic.Penitent.GetPosition() + ((!Core.Logic.Penitent.IsCrouched) ? this.Offset : Vector3.zero);
			base.transform.position = position;
		}

		public void Dispose()
		{
			base.Destroy();
		}

		public Vector2 Offset;
	}
}
