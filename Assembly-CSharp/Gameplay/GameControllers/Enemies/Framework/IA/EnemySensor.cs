using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Framework.IA
{
	public class EnemySensor : MonoBehaviour
	{
		private void Start()
		{
			this.entity = base.GetComponentInParent<Entity>();
			this.reversePos = (this.entity.Status.Orientation == EntityOrientation.Left);
			if (this.reversePos)
			{
				this.ReversePosition();
			}
			this.InheritedStart();
		}

		protected virtual void InheritedStart()
		{
		}

		private void Update()
		{
			if (this.entity.Status.Orientation == EntityOrientation.Left)
			{
				if (!this.reversePos)
				{
					this.reversePos = true;
					this.ReversePosition();
				}
			}
			else if (this.entity.Status.Orientation == EntityOrientation.Right && this.reversePos)
			{
				this.reversePos = !this.reversePos;
				this.ReversePosition();
			}
			this.InheritedUpdate();
		}

		protected virtual void InheritedUpdate()
		{
		}

		private void ReversePosition()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.y += 180f;
			base.transform.eulerAngles = eulerAngles;
		}

		private Entity entity;

		private bool reversePos;
	}
}
