using System;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Framework.Util
{
	public class PenitentOnElevatorFix : MonoBehaviour
	{
		public void OnLeverActivation()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.SpriteRenderer.enabled = false;
			penitent.DamageArea.enabled = false;
			penitent.Physics.EnablePhysics(false);
			this.FakeTPO.SetActive(true);
			SpriteRenderer component = base.GetComponent<SpriteRenderer>();
			component.flipX = penitent.SpriteRenderer.flipX;
		}

		public void OnDestinationReached()
		{
			this.FakeTPO.SetActive(false);
			Core.Logic.Penitent.Teleport(this.FakeTPO.transform.position);
			Penitent penitent = Core.Logic.Penitent;
			penitent.SpriteRenderer.enabled = true;
			penitent.DamageArea.enabled = true;
			penitent.Physics.EnablePhysics(true);
		}

		public GameObject FakeTPO;
	}
}
