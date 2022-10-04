using System;
using Framework.Managers;
using Framework.Pooling;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Effects
{
	public class GuiltRecoverEffect : PoolObject
	{
		private void LateUpdate()
		{
			Vector3 position = Core.Logic.Penitent.transform.position;
			base.transform.position = position;
		}
	}
}
