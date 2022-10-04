using System;
using DG.Tweening;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class Fader : MonoBehaviour, IActionable
	{
		public void Use()
		{
			SpriteRenderer component = base.GetComponent<SpriteRenderer>();
			component.DOFade(0f, this.time);
		}

		public bool Locked { get; set; }

		public float time;
	}
}
