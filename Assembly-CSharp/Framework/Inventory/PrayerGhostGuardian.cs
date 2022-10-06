using System;
using Framework.Managers;
using Gameplay.GameControllers.Entities.Guardian;
using UnityEngine;

namespace Framework.Inventory
{
	public class PrayerGhostGuardian : ObjectEffect
	{
		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
		}

		protected override bool OnApplyEffect()
		{
			this.InstantiateGuardian();
			return true;
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			this.DisposeGuardian();
		}

		public void InstantiateGuardian()
		{
			if (this.GhostGuardianPrefab != null && this.GhostGuardianInstantiation == null)
			{
				Vector3 position = Core.Logic.Penitent.transform.position;
				this.GhostGuardianInstantiation = Object.Instantiate<GameObject>(this.GhostGuardianPrefab, position, Quaternion.identity);
			}
			else if (this.GhostGuardianInstantiation != null)
			{
				this.GhostGuardianInstantiation.SetActive(true);
				this.GhostGuardianInstantiation.transform.position = Core.Logic.Penitent.transform.position;
			}
		}

		public void DisposeGuardian()
		{
			if (this.GhostGuardianInstantiation == null)
			{
				return;
			}
			GuardianPrayer componentInChildren = this.GhostGuardianInstantiation.GetComponentInChildren<GuardianPrayer>();
			componentInChildren.Behaviour.VanishFlag = true;
		}

		public GameObject GhostGuardianPrefab;

		private GameObject GhostGuardianInstantiation;
	}
}
