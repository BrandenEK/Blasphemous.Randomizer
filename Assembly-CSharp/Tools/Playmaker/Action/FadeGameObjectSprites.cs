using System;
using System.Collections.Generic;
using DG.Tweening;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Plays an audio.")]
	public class FadeGameObjectSprites : FsmStateAction
	{
		public override void OnEnter()
		{
			List<SpriteRenderer> list = new List<SpriteRenderer>(this.go.GetComponentsInChildren<SpriteRenderer>());
			list.ForEach(delegate(SpriteRenderer x)
			{
				x.DOFade(this.targetAlpha.Value, this.fadeTime.Value);
			});
			base.Finish();
		}

		public GameObject go;

		public FsmFloat targetAlpha;

		public FsmFloat fadeTime;
	}
}
