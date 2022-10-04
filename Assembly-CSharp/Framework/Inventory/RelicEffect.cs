using System;
using UnityEngine;

namespace Framework.Inventory
{
	[RequireComponent(typeof(Relic))]
	public class RelicEffect : MonoBehaviour
	{
		private protected Relic Relic { protected get; private set; }

		private void Awake()
		{
			this.Relic = base.GetComponent<Relic>();
			this.OnAwake();
		}

		private void Start()
		{
			this.OnStart();
		}

		private void Update()
		{
			this.OnUpdate();
		}

		public void OnEquipInventoryObject()
		{
			this.OnEquipEffect();
		}

		public void OnUnEquipInventoryObject()
		{
			this.OnUnEquipEffect();
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		public virtual void OnEquipEffect()
		{
		}

		public virtual void OnUnEquipEffect()
		{
		}
	}
}
