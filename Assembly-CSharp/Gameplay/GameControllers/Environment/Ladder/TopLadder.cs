using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Ladder
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class TopLadder : MonoBehaviour
	{
		public Penitent Penitent { get; private set; }

		public bool TargetInside { get; private set; }

		private Collider2D Collider2D { get; set; }

		private void Start()
		{
			this.Penitent = Core.Logic.Penitent;
			this.Collider2D = base.GetComponent<BoxCollider2D>();
		}

		private void OnTriggerEnter2D(Collider2D target)
		{
			if ((this.TargetLayerMask.value & 1 << target.gameObject.layer) > 0)
			{
				this.TargetInside = true;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if ((this.TargetLayerMask.value & 1 << other.gameObject.layer) > 0)
			{
				this.TargetInside = false;
			}
		}

		private void Update()
		{
			if (!this.TargetInside)
			{
				return;
			}
			if (this.Penitent.Status.IsGrounded && this.Penitent.PlatformCharacterInput.isJoystickDown)
			{
				Vector3 position = new Vector3(this.Collider2D.bounds.center.x, this.Penitent.transform.position.y, this.Penitent.transform.position.z);
				this.Penitent.transform.position = position;
			}
		}

		public LayerMask TargetLayerMask;
	}
}
