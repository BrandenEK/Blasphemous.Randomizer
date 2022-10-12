using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.MovingPlatforms
{
	public class MovingPlatformDecorator : MonoBehaviour
	{
		private void Awake()
		{
			this.plat = base.GetComponent<StraightMovingPlatform>();
		}

		private void Update()
		{
			foreach (Material speed in this.materialsLinkedToSpeed)
			{
				this.SetSpeed(speed);
			}
		}

		private float ConvertSpeed(Vector3 v)
		{
			float num;
			if (this.isVertical)
			{
				num = v.y;
			}
			else
			{
				num = v.x;
			}
			return -num * this.shaderToUnitFactor;
		}

		private void SetSpeed(Material m)
		{
			float value = this.ConvertSpeed(this.plat.GetVelocity());
			m.SetFloat("_Speed", value);
		}

		public List<Material> materialsLinkedToSpeed;

		public float maxSpeed;

		public float minSpeed;

		public bool isVertical;

		private StraightMovingPlatform plat;

		public float shaderToUnitFactor = 0.2f;
	}
}
