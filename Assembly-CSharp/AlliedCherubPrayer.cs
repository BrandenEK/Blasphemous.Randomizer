using System;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.Protection;
using UnityEngine;

public class AlliedCherubPrayer : MonoBehaviour
{
	private void Start()
	{
		if (this.AlliedCherubSystem)
		{
			PoolManager.Instance.CreatePool(this.AlliedCherubSystem, 1);
		}
	}

	public void InstantiateCherubs()
	{
		if (this.AlliedCherubSystem == null)
		{
			return;
		}
		Vector3 position = Core.Logic.Penitent.transform.position;
		this._instantiatedAlliedCherubSystem = PoolManager.Instance.ReuseObject(this.AlliedCherubSystem, position, Quaternion.identity, false, 1).GameObject;
		AlliedCherubSystem componentInChildren = this._instantiatedAlliedCherubSystem.GetComponentInChildren<AlliedCherubSystem>();
		if (componentInChildren && !componentInChildren.IsEnable)
		{
			componentInChildren.DeployCherubs();
			componentInChildren.OnCherubsDepleted += this.OnCherubsDepleted;
		}
	}

	private void OnCherubsDepleted(AlliedCherubSystem obj)
	{
		obj.OnCherubsDepleted -= this.OnCherubsDepleted;
		Core.Logic.Penitent.PrayerCast.ForcePrayerEnd();
	}

	public void DisposeCherubs()
	{
		if (this._instantiatedAlliedCherubSystem == null)
		{
			return;
		}
		AlliedCherubSystem componentInChildren = this._instantiatedAlliedCherubSystem.GetComponentInChildren<AlliedCherubSystem>();
		if (componentInChildren)
		{
			componentInChildren.DisposeSystem();
		}
	}

	public GameObject AlliedCherubSystem;

	private GameObject _instantiatedAlliedCherubSystem;
}
