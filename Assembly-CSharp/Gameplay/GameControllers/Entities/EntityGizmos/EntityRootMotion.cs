using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.EntityGizmos
{
	public class EntityRootMotion : MonoBehaviour
	{
		public Vector3 GetPosition()
		{
			return (this.Entity.Status.Orientation != EntityOrientation.Right) ? this.GetReversePosition() : this.GetStraightPosition();
		}

		private Vector3 GetStraightPosition()
		{
			return base.transform.position;
		}

		private Vector3 GetReversePosition()
		{
			Vector3 result = new Vector3(base.transform.position.x - base.transform.localPosition.x * 2f, base.transform.position.y, 0f);
			return result;
		}

		public Entity Entity;
	}
}
