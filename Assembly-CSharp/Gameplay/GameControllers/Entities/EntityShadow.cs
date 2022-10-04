using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity.BlobShadow;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class EntityShadow : Trait
	{
		public BlobShadow BlobShadow { get; private set; }

		protected override void OnStart()
		{
			if (!Core.GameModeManager.IsCurrentMode(GameModeManager.GAME_MODES.DEMAKE))
			{
				this.AssignBlobShadow();
				base.EntityOwner.FlagChanged += this.OnFlagChanged;
			}
		}

		private void OnFlagChanged(string key, bool active)
		{
			base.enabled = key.Equals("HIDE_SHADOW");
		}

		protected override void OnTraitDestroy()
		{
			this.RemoveBlobShadow();
		}

		private void AssignBlobShadow()
		{
			GameObject blowShadow = Core.Logic.CurrentLevelConfig.BlobShadowManager.GetBlowShadow(base.transform.position);
			if (blowShadow == null)
			{
				return;
			}
			BlobShadow component = blowShadow.GetComponent<BlobShadow>();
			this.BlobShadow = component;
			this.BlobShadow.SetEntity(base.EntityOwner);
		}

		public void RemoveBlobShadow()
		{
			BlobShadowManager blobShadowManager = Core.Logic.CurrentLevelConfig.BlobShadowManager;
			if (blobShadowManager != null && this.BlobShadow != null)
			{
				blobShadowManager.StoreBlobShadow(this.BlobShadow.gameObject);
			}
		}

		[Range(0f, 0.5f)]
		public float ShadowXOffset = 0.15f;

		[Range(0f, 0.5f)]
		public float ShadowYOffset = 0.15f;
	}
}
