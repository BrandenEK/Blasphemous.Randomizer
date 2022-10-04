using System;
using DG.Tweening;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player
{
	public class PenitentGuardian : MonoBehaviour
	{
		public bool IsTriggered { get; set; }

		private void OnEnable()
		{
			if (this.GuardianSpriteRenderer == null)
			{
				this.GuardianSpriteRenderer = base.GetComponent<SpriteRenderer>();
			}
			if (this.Owner == null)
			{
				this.Owner = Core.Logic.Penitent;
			}
			this.FadeIn();
		}

		public void SetOrientation(Hit hit)
		{
			if (this.Owner == null)
			{
				return;
			}
			Vector3 position = hit.AttackingEntity.transform.position;
			this.GuardianSpriteRenderer.flipX = (position.x <= this.Owner.transform.position.x);
		}

		public void FadeIn()
		{
			if (this.GuardianSpriteRenderer == null)
			{
				return;
			}
			this.GuardianSpriteRenderer.DOFade(1f, 0.1f);
		}

		public void FadeOut()
		{
			if (this.GuardianSpriteRenderer == null)
			{
				return;
			}
			this.GuardianSpriteRenderer.DOFade(0f, 0.1f);
		}

		protected SpriteRenderer GuardianSpriteRenderer;

		protected Penitent Owner;
	}
}
