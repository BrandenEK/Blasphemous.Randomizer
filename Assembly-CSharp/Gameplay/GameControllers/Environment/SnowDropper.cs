using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Environment
{
	[RequireComponent(typeof(ParticleSystem))]
	public class SnowDropper : MonoBehaviour
	{
		private void Awake()
		{
			if (Core.Logic != null)
			{
			}
		}

		private void Start()
		{
			this._penitent = Core.Logic.Penitent;
			if (this._penitent != null && !this.isAtPlayerPosition)
			{
				this.keepPosition(this._penitent.transform.position);
			}
		}

		private void LateUpdate()
		{
			if (this._penitent != null && !this._penitent.Status.Dead)
			{
				this.keepPosition(this._penitent.transform.position);
			}
		}

		protected void keepPosition(Vector3 playerPosition)
		{
			base.transform.position = playerPosition;
		}

		private Penitent _penitent;

		private bool isAtPlayerPosition;

		public float maxHeight = 50f;
	}
}
