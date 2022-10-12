using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class FlipEntityComponents : Trait
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._prevOrientation = base.EntityOwner.Status.Orientation;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._currentOrientation = base.EntityOwner.Status.Orientation;
			this.FlipComponents();
		}

		private void FlipComponents()
		{
			if (this._prevOrientation == this._currentOrientation)
			{
				return;
			}
			EntityOrientation currentOrientation = this._currentOrientation;
			if (currentOrientation != EntityOrientation.Right)
			{
				if (currentOrientation != EntityOrientation.Left)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.SetXRotateScale(-1f);
				this._prevOrientation = EntityOrientation.Left;
			}
			else
			{
				this.SetXRotateScale(1f);
				this._prevOrientation = EntityOrientation.Right;
			}
		}

		private void SetXRotateScale(float xRotateScale)
		{
			Vector3 localScale = new Vector3(xRotateScale, 1f, 0f);
			foreach (GameObject gameObject in this.EntityComponents)
			{
				gameObject.transform.localScale = localScale;
			}
		}

		public GameObject[] EntityComponents;

		private EntityOrientation _currentOrientation;

		private EntityOrientation _prevOrientation;
	}
}
