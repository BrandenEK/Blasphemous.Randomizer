using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Protection;
using UnityEngine;

public class ShieldSystemPrayer : MonoBehaviour
{
	private void Start()
	{
		if (this.ShieldSystem)
		{
			PoolManager.Instance.CreatePool(this.ShieldSystem, 1);
		}
	}

	public void InstantiateShield()
	{
		if (this.ShieldSystem == null)
		{
			return;
		}
		Vector3 position = Core.Logic.Penitent.transform.position;
		this._instantiatedShield = PoolManager.Instance.ReuseObject(this.ShieldSystem, position, Quaternion.identity, false, 1).GameObject;
		PenitentShieldSystem componentInChildren = this._instantiatedShield.GetComponentInChildren<PenitentShieldSystem>();
		if (componentInChildren)
		{
			componentInChildren.SetShieldsOwner(Core.Logic.Penitent);
		}
	}

	public void DisposeShield()
	{
		if (this._instantiatedShield == null)
		{
			return;
		}
		PenitentShieldSystem componentInChildren = this._instantiatedShield.GetComponentInChildren<PenitentShieldSystem>();
		if (componentInChildren)
		{
			componentInChildren.DisposeShield(1f);
		}
	}

	public GameObject ShieldSystem;

	private GameObject _instantiatedShield;
}
