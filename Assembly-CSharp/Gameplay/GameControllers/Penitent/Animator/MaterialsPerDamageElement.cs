using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Animator
{
	[Serializable]
	public struct MaterialsPerDamageElement
	{
		public DamageArea.DamageElement element;

		public Material mat;
	}
}
