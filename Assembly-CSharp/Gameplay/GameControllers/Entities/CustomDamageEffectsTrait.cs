using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class CustomDamageEffectsTrait : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			PoolManager.Instance.CreatePool(this.bloodFx, 3);
			PoolManager.Instance.CreatePool(this.fx, 3);
		}

		public void SpawnDamageEffect(Vector3 position, bool right)
		{
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.fx, position, Quaternion.identity, false, 1);
			objectInstance.GameObject.GetComponent<SpriteRenderer>().flipX = right;
		}

		public void SpawnBloodEffect(Vector3 position, bool right)
		{
			PoolManager.ObjectInstance objectInstance = PoolManager.Instance.ReuseObject(this.bloodFx, position, Quaternion.identity, false, 1);
			objectInstance.GameObject.GetComponent<SpriteRenderer>().flipX = right;
		}

		public GameObject fx;

		public GameObject bloodFx;
	}
}
