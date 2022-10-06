using System;
using Framework.Pooling;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.GhostTrap
{
	public class GhostTriggerEffect : PoolObject
	{
		protected Animator Animator { get; set; }

		private void Awake()
		{
			this.Animator = base.GetComponent<Animator>();
		}

		public override void OnObjectReuse()
		{
			base.OnObjectReuse();
			this.PlayRandomAnim();
		}

		public void PlayRandomAnim()
		{
			int num = Random.Range(1, this.NumAnimations + 1);
			this.Animator.SetInteger("FLY_PAGES", num);
			this.Animator.speed = Random.Range(0.5f, 1f);
		}

		public void ResetAnimatorParameter()
		{
			this.Animator.SetInteger("FLY_PAGES", 0);
			base.Destroy();
		}

		public int NumAnimations = 3;
	}
}
