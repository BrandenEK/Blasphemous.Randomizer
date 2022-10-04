using System;
using Framework.Pooling;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.Swimmer.Animator
{
	public class SwimmerTerrainEffect : PoolObject
	{
		private void Awake()
		{
			this.Animator = base.GetComponent<Animator>();
		}

		public void RisingEffect(bool rising)
		{
			if (this.Animator != null)
			{
				this.Animator.SetBool("RISING", rising);
			}
		}

		protected Animator Animator;
	}
}
