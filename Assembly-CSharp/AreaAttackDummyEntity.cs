using System;
using Gameplay.GameControllers.Entities;
using Tools.Level.Actionables;

public class AreaAttackDummyEntity : Entity
{
	protected override void OnAwake()
	{
		base.OnAwake();
		this.mArea = base.GetComponent<ShockwaveArea>();
		if (this.mArea != null)
		{
			this.mArea.SetOwner(this);
		}
	}

	private ShockwaveArea mArea;
}
