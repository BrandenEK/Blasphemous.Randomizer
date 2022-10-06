using System;
using DG.Tweening;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[RequireComponent(typeof(BreakableObject))]
	public class BreakableInteractor : Interactable
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this.BreakableTrigger = base.GetComponent<BreakableObject>();
			BreakableObject breakableTrigger = this.BreakableTrigger;
			breakableTrigger.OnBreak = (Core.SimpleEvent)Delegate.Combine(breakableTrigger.OnBreak, new Core.SimpleEvent(this.OnBroken));
		}

		private void OnBroken()
		{
			TweenSettingsExtensions.OnComplete<Sequence>(TweenSettingsExtensions.SetDelay<Sequence>(DOTween.Sequence(), this.InteractionTimeout), delegate()
			{
				this.ActivateActionable(this.InteractionTargets);
			});
		}

		private void ActivateActionable(GameObject[] gameObjects)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				if (gameObject)
				{
					IActionable component = gameObject.GetComponent<IActionable>();
					if (component != null)
					{
						component.Use();
					}
				}
			}
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			BreakableObject breakableTrigger = this.BreakableTrigger;
			breakableTrigger.OnBreak = (Core.SimpleEvent)Delegate.Remove(breakableTrigger.OnBreak, new Core.SimpleEvent(this.OnBroken));
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		protected GameObject[] InteractionTargets;

		[Tooltip("A Lapse before interaction starts")]
		public float InteractionTimeout;

		private BreakableObject BreakableTrigger;
	}
}
