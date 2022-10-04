using System;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class BreakableDamageArea : DamageArea
	{
		protected override void OnAwake()
		{
		}

		public bool GrantsFervour
		{
			get
			{
				return this.grantsFervour;
			}
		}

		[SerializeField]
		protected bool grantsFervour = true;
	}
}
