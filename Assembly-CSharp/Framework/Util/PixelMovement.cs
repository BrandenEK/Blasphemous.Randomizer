using System;
using UnityEngine;

namespace Framework.Util
{
	[ExecuteInEditMode]
	public class PixelMovement : MonoBehaviour
	{
		public Vector3 RealPosition
		{
			get
			{
				return this.realPosition;
			}
		}

		private void Awake()
		{
			this.realPosition = base.transform.position;
		}

		private void Update()
		{
			this.movement = base.transform.position - this.oldPosition;
			this.oldPosition = base.transform.position;
			this.realPosition += this.movement;
		}

		private void LateUpdate()
		{
			this.PixelPerfectPosition();
		}

		private void PixelPerfectPosition()
		{
			float num = Mathf.Floor(base.transform.position.x * 32f) / 32f;
			float num2 = Mathf.Floor(base.transform.position.y * 32f) / 32f;
			base.transform.position = new Vector3(num, num2, base.transform.position.z);
		}

		private Vector3 realPosition;

		private Vector3 oldPosition;

		private Vector3 movement;
	}
}
