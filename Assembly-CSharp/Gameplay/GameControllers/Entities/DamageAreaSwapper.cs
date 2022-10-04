using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	[RequireComponent(typeof(Collider2D))]
	public class DamageAreaSwapper : Trait
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._entityDamageArea = base.EntityOwner.EntityDamageArea;
			this._damageAreaIsSet = (this._entityDamageArea != null);
			if (this._damageAreaIsSet)
			{
				this._damageAreaColliderIsSet = (this._entityDamageArea.DamageAreaCollider != null);
			}
			if (!this._damageAreaColliderIsSet)
			{
				Debug.LogError("Damage Area Collider must be set in Damage Area");
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this.SetDamageAreaColliderOrientation();
		}

		private void SetDamageAreaColliderOrientation()
		{
			if (!this._damageAreaColliderIsSet)
			{
				return;
			}
			EntityOrientation orientation = base.EntityOwner.Status.Orientation;
			Vector3 localScale = base.transform.localScale;
			localScale.x = (float)((orientation != EntityOrientation.Left) ? 1 : -1);
			base.transform.localScale = localScale;
		}

		private DamageArea _entityDamageArea;

		private bool _damageAreaIsSet;

		private bool _damageAreaColliderIsSet;
	}
}
