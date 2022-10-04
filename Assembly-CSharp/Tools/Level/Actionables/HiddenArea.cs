using System;
using DG.Tweening;
using DG.Tweening.Core.Surrogates;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Sirenix.OdinInspector;
using Tools.Gameplay;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[SelectionBase]
	[RequireComponent(typeof(UniqueId))]
	public class HiddenArea : PersistentObject, IActionable
	{
		public void Use()
		{
			Core.Metrics.CustomEvent("SECRET_DISCOVERED", base.name, -1f);
			Core.Audio.PlaySfx(this.disappearSound, 0f);
			this.FadeRenderers(this.disappearTime);
			if (HiddenArea.OnUse != null)
			{
				HiddenArea.OnUse();
			}
		}

		public void PreviouslyUsed()
		{
			this.FadeRenderers(this.disappearTime);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Penitent"))
			{
				this.Use();
			}
		}

		private void FadeRenderers(float time)
		{
			this.triggered = true;
			SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>();
			Collider2D[] componentsInChildren2 = base.GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				SpriteRenderer rend = componentsInChildren[i];
				DOTween.To(() => rend.color, delegate(ColorWrapper x)
				{
					rend.color = x;
				}, new Color(rend.color.r, rend.color.g, rend.color.b, 0f), time);
			}
			foreach (Collider2D collider2D in componentsInChildren2)
			{
				collider2D.enabled = false;
			}
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			BasicPersistence basicPersistence = base.CreatePersistentData<BasicPersistence>();
			basicPersistence.triggered = this.triggered;
			return basicPersistence;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			BasicPersistence basicPersistence = (BasicPersistence)data;
			if (basicPersistence.triggered)
			{
				this.PreviouslyUsed();
			}
		}

		public bool Locked { get; set; }

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float disappearTime = 1f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool triggerOnPlayerEnter;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		private string disappearSound;

		private bool triggered;

		public static Core.SimpleEvent OnUse;
	}
}
