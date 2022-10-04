using System;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Enemies.DrownedCorpse.Animator
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class DrownedCorpseHelmet : MonoBehaviour
	{
		public void Initialize(Entity owner)
		{
			this._owner = owner;
			this._spriteRenderer = base.GetComponent<SpriteRenderer>();
		}

		private void OnEnable()
		{
			if (!this._owner)
			{
				return;
			}
			EntityOrientation orientation = this._owner.Status.Orientation;
			this.SetOrientation(orientation);
		}

		private void SetOrientation(EntityOrientation orientation)
		{
			this._spriteRenderer.flipX = (orientation == EntityOrientation.Left);
		}

		private Entity _owner;

		private SpriteRenderer _spriteRenderer;
	}
}
