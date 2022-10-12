using System;
using UnityEngine;

namespace Tools.Util
{
	[RequireComponent(typeof(Animator))]
	public class AnimationRandomLoop : MonoBehaviour
	{
		private void Start()
		{
			this.anim = base.GetComponent<Animator>();
			bool flag = this.anim.HasState(0, Animator.StringToHash(this.initialState));
			if (this.anim && flag)
			{
				this.anim.Play(this.initialState, 0, UnityEngine.Random.Range(0f, 1f));
			}
		}

		public string initialState;

		private Animator anim;
	}
}
