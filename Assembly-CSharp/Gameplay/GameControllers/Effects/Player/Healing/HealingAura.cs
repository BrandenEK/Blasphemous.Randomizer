using System;
using Framework.FrameworkCore;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Healing
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class HealingAura : MonoBehaviour
	{
		private void Awake()
		{
			this._auraAnimator = base.GetComponent<Animator>();
			this._auraRenderer = base.GetComponent<SpriteRenderer>();
		}

		public void StartAura(EntityOrientation orientation)
		{
			if (this._auraAnimator == null)
			{
				return;
			}
			this.SetOrientatation(orientation);
			this._auraAnimator.SetBool("HEALING", true);
			this._auraAnimator.Play(0, 0, 0f);
		}

		public void StopAura()
		{
			if (this._auraAnimator == null)
			{
				return;
			}
			this._auraAnimator.SetBool("HEALING", false);
		}

		private void SetOrientatation(EntityOrientation orientation)
		{
			this._auraRenderer.flipX = (orientation == EntityOrientation.Left);
		}

		public void PlayHealingExplosion()
		{
			Core.Logic.Penitent.Audio.HealingExplosion();
		}

		private Animator _auraAnimator;

		private SpriteRenderer _auraRenderer;
	}
}
