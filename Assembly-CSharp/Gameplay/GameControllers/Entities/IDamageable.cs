using System;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public interface IDamageable
	{
		bool BleedOnImpact();

		bool SparkOnImpact();

		void Damage(Hit hit);

		Vector3 GetPosition();
	}
}
