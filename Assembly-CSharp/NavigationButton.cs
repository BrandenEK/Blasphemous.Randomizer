using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

public class NavigationButton : MonoBehaviour
{
	public void ButtonPressed()
	{
		NavigationWidget componentInParent = base.GetComponentInParent<NavigationWidget>();
		if (Core.Logic.Penitent != null)
		{
			Core.Logic.Penitent.Teleport(this.destination);
		}
		else
		{
			Log.Error("Navigation", "Penitent is null.", null);
		}
		if (componentInParent != null)
		{
			componentInParent.Show(false);
		}
	}

	public Vector3 destination;
}
