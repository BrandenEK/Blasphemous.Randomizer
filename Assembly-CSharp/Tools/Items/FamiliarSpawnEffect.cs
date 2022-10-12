using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Familiar;
using Gameplay.GameControllers.Penitent;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.Items
{
	public class FamiliarSpawnEffect : ObjectEffect
	{
		private Familiar FamiliarEntity { get; set; }

		protected override bool OnApplyEffect()
		{
			if (!Core.Logic.IsMenuScene())
			{
				this.InstantiateFamiliar(this.GetPosition);
			}
			return this.FamiliarEntity != null;
		}

		protected override void OnRemoveEffect()
		{
			base.OnRemoveEffect();
			if (this.FamiliarEntity)
			{
				this.FamiliarEntity.Dispose();
			}
		}

		private void InstantiateFamiliar(Vector3 position)
		{
			if (this.FamiliarPrefab == null)
			{
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.FamiliarPrefab, position, Quaternion.identity);
			this.FamiliarEntity = gameObject.GetComponentInChildren<Familiar>();
			if (this.FamiliarEntity)
			{
				this.FamiliarEntity.Owner = Core.Logic.Penitent;
			}
		}

		private Vector3 GetPosition
		{
			get
			{
				Penitent penitent = Core.Logic.Penitent;
				return penitent.transform.position + this.Offset;
			}
		}

		[FormerlySerializedAs("Familiar")]
		public GameObject FamiliarPrefab;

		public Vector2 Offset;
	}
}
