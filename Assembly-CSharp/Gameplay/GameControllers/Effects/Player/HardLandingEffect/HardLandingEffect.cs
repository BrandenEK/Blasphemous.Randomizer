using System;
using Framework.Pooling;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.HardLandingEffect
{
	[RequireComponent(typeof(Animator))]
	public class HardLandingEffect : PoolObject
	{
		public void PlayLandingEffect()
		{
			this.HardLandingEffectAnimator.Play("VerticalAttackLandingEffect");
		}

		public void DestroyPrefab()
		{
			base.Destroy();
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
		}

		public Animator HardLandingEffectAnimator;
	}
}
