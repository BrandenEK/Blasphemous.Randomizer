using System;
using System.Collections.Generic;
using DG.Tweening;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Plays an audio.")]
	public class FadeGameObjectSprites : FsmStateAction
	{
		public override void OnEnter()
		{
			List<SpriteRenderer> list = new List<SpriteRenderer>(this.go.GetComponentsInChildren<SpriteRenderer>());
			list.ForEach(delegate(SpriteRenderer x)
			{
				ShortcutExtensions43.DOFade(x, this.targetAlpha.Value, this.fadeTime.Value);
			});
			base.Finish();
		}

		public GameObject go;

		public FsmFloat targetAlpha;

		public FsmFloat fadeTime;
	}
}
