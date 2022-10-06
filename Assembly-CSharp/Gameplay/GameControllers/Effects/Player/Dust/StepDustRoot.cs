using System;
using Framework.FrameworkCore;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dust
{
	public class StepDustRoot : Trait
	{
		private void OnDrawGizmosSelected()
		{
			Color color;
			color..ctor(0.78f, 0.14f, 0.69f, 1f);
			Gizmos.color = color;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
