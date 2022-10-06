using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Entities.MiriamPortal;
using Gameplay.GameControllers.Entities.MiriamPortal.AI;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Framework.Inventory
{
	public class PrayerMiriam : ObjectEffect
	{
		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!Core.InventoryManager.IsPrayerEquipped("PR201"))
			{
				this.DisposePortal();
			}
		}

		protected override bool OnApplyEffect()
		{
			this.InstantiatePortal();
			return true;
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			this.DisposePortal();
		}

		public void InstantiatePortal()
		{
			if (this.PortalPrefab != null && this.PortalInstantiation == null)
			{
				this.PortalInstantiation = Object.Instantiate<GameObject>(this.PortalPrefab, this.CalculateTargetPosition(), Quaternion.identity);
			}
			else if (this.PortalInstantiation != null)
			{
				if (this.PortalInstantiation.activeInHierarchy)
				{
					this.MiriamPortal.Behaviour.ReappearFlag = true;
					this.MiriamPortal.Behaviour.VanishFlag = false;
				}
				else
				{
					this.PortalInstantiation.SetActive(true);
					this.PortalInstantiation.transform.position = this.CalculateTargetPosition();
				}
			}
			if (this.MiriamPortal == null)
			{
				this.MiriamPortal = this.PortalInstantiation.GetComponentInChildren<MiriamPortalPrayer>();
			}
			this.MiriamPortal.Behaviour.SetState(MiriamPortalPrayerBehaviour.MiriamPortalState.Follow);
		}

		private Vector3 CalculateTargetPosition()
		{
			Penitent penitent = Core.Logic.Penitent;
			Vector3 result = penitent.transform.position + Vector3.up * this.vOffset;
			result.x += ((penitent.GetOrientation() != EntityOrientation.Right) ? this.hOffest : (-this.hOffest));
			return result;
		}

		public void DisposePortal()
		{
			if (this.PortalInstantiation == null)
			{
				return;
			}
			if (this.MiriamPortal == null)
			{
				this.MiriamPortal = this.PortalInstantiation.GetComponentInChildren<MiriamPortalPrayer>();
			}
			if (this.MiriamPortal && this.MiriamPortal.Behaviour)
			{
				this.MiriamPortal.Behaviour.VanishFlag = true;
			}
		}

		public float vOffset = 2f;

		public float hOffest = 2f;

		public GameObject PortalPrefab;

		private GameObject PortalInstantiation;

		private MiriamPortalPrayer MiriamPortal;
	}
}
