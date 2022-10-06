using System;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class ActionableForce : MonoBehaviour, IActionable
	{
		public bool Locked { get; set; }

		[Button(0)]
		public void Use()
		{
			this.Impact(1f);
		}

		private void Impact(float mult = 1f)
		{
			Vector2 vector = Vector2.one;
			if (this.fromPenitent && Core.Logic.Penitent != null)
			{
				vector = (base.transform.position - (Core.Logic.Penitent.transform.position + this.forceOriginOffset)).normalized * this.force * mult;
			}
			base.GetComponent<Rigidbody2D>().AddForce(vector, 1);
		}

		public void HeavyUse()
		{
			this.Impact(2f);
		}

		public float force = 10f;

		public bool fromPenitent = true;

		public Vector2 forceDirection;

		public Vector2 forceOriginOffset;
	}
}
