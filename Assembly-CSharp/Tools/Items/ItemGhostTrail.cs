using System;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Player.GhostTrail;
using UnityEngine;

namespace Tools.Items
{
	public class ItemGhostTrail : ObjectEffect
	{
		protected override bool OnApplyEffect()
		{
			if (Core.Logic.Penitent == null)
			{
				return false;
			}
			this.trail = Core.Logic.Penitent.GetComponentInChildren<GhostTrailGenerator>();
			this.prevColor = this.trail.TrailColor;
			this.stopped = false;
			return true;
		}

		protected override void OnUpdate()
		{
			if (this.trail == null || this.stopped || Core.Logic.Penitent.IsOnExecution)
			{
				return;
			}
			if (!this.trail.EnableGhostTrail)
			{
				this.trail.EnableGhostTrail = true;
				this.trail.TrailColor = this.color;
			}
		}

		protected override void OnRemoveEffect()
		{
			if (this.trail == null)
			{
				return;
			}
			this.trail.TrailColor = this.prevColor;
			this.trail.EnableGhostTrail = false;
			this.stopped = true;
		}

		[SerializeField]
		private Color color;

		private Color prevColor;

		private GhostTrailGenerator trail;

		private bool stopped;
	}
}
