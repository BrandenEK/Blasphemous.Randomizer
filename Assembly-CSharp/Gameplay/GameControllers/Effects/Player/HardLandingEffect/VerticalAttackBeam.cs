using System;
using Framework.Pooling;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.HardLandingEffect
{
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Animator))]
	public class VerticalAttackBeam : PoolObject
	{
		public void DestroyPrefab()
		{
			base.Destroy();
		}

		public void Pause()
		{
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
		}

		public Animator VerticalBeamAnimator;

		public SpriteRenderer VerticalBeamRenderer;
	}
}
