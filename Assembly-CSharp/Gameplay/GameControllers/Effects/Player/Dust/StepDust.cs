using System;
using Framework.FrameworkCore;
using Framework.Pooling;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Dust
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class StepDust : PoolObject
	{
		public Entity Owner { get; set; }

		public void SetSpriteOrientation(EntityOrientation orientation)
		{
			if (this._stepDustSpriteRenderer == null)
			{
				this._stepDustSpriteRenderer = base.GetComponent<SpriteRenderer>();
			}
			this._stepDustSpriteRenderer.flipX = (orientation == EntityOrientation.Left);
		}

		private SpriteRenderer _stepDustSpriteRenderer;

		public StepDust.StepDustType stepDustType;

		public enum StepDustType
		{
			Running,
			StartRun,
			Landing,
			StopRunning,
			Jump,
			Crouch,
			Attack1,
			Attack2,
			FinishingAttack
		}
	}
}
