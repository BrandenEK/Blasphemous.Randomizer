using System;
using System.Collections;
using System.Collections.Generic;
using Framework.FrameworkCore;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Penitent;
using Tools.Level.Layout;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.GhostTrail
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class GhostTrailGenerator : Trait
	{
		public void DoEnableTrail()
		{
			this.EnableGhostTrail = true;
		}

		public void DoDisableTrail()
		{
			this.EnableGhostTrail = false;
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._currentLevel = Core.Logic.CurrentLevelConfig;
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			GhostTrailGenerator.AreGhostTrailsAllowed = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (!GhostTrailGenerator.AreGhostTrailsAllowed && this.EnableGhostTrail)
			{
				this.EnableGhostTrail = false;
			}
			if (this.EnableGhostTrail)
			{
				if (this._startGenerateTrail)
				{
					return;
				}
				this._startGenerateTrail = true;
				base.InvokeRepeating("GetGhostTrail", 0f, this.TimeStep);
				Penitent penitent = Core.Logic.Penitent;
				penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnPenitentDead));
			}
			else
			{
				if (!this._startGenerateTrail)
				{
					return;
				}
				this._startGenerateTrail = !this._startGenerateTrail;
				base.CancelInvoke();
				Penitent penitent2 = Core.Logic.Penitent;
				penitent2.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent2.OnDead, new Core.SimpleEvent(this.OnPenitentDead));
			}
		}

		private void OnPenitentDead()
		{
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.OnPenitentDead));
			this.EnableGhostTrail = false;
		}

		private void GetGhostTrail()
		{
			if (!this.EnableGhostTrail)
			{
				return;
			}
			SpriteRenderer spriteRenderer;
			if (this._trailParts.Count > 0)
			{
				spriteRenderer = this._trailParts[this._trailParts.Count - 1];
				if (!spriteRenderer)
				{
					return;
				}
				this._trailParts.Remove(spriteRenderer);
				spriteRenderer.gameObject.SetActive(true);
				spriteRenderer.gameObject.transform.position = base.transform.position;
				if (this.followRotation)
				{
					spriteRenderer.gameObject.transform.rotation = base.transform.rotation;
				}
				if (this.followScale)
				{
					spriteRenderer.transform.localScale = base.transform.localScale;
				}
				spriteRenderer.sprite = base.GetComponent<SpriteRenderer>().sprite;
				Color trailColor = this.TrailColor;
				if (trailColor.a < 1f)
				{
					trailColor.a = 1f;
				}
				spriteRenderer.color = trailColor;
			}
			else
			{
				GameObject gameObject = new GameObject
				{
					layer = LayerMask.NameToLayer("Penitent")
				};
				spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
				if (this.customMaterial)
				{
					spriteRenderer.material = this.customMaterial;
				}
				spriteRenderer.sprite = base.GetComponent<SpriteRenderer>().sprite;
				spriteRenderer.color = this.TrailColor;
				gameObject.transform.position = base.transform.position;
				if (this.followRotation)
				{
					gameObject.transform.rotation = base.transform.rotation;
				}
				gameObject.transform.localScale = base.transform.localScale;
				gameObject.transform.parent = this._currentLevel.LevelEffectsStore.transform;
			}
			if (this.followOrientation)
			{
				spriteRenderer.flipX = (base.EntityOwner.Status.Orientation == EntityOrientation.Left);
			}
			Singleton<Core>.Instance.StartCoroutine(this.FadeTrailPart(spriteRenderer));
		}

		private void StoreGhostTrail(SpriteRenderer ghostTrail)
		{
			if (!ghostTrail.gameObject.activeSelf)
			{
				return;
			}
			this._trailParts.Add(ghostTrail);
			ghostTrail.gameObject.SetActive(false);
		}

		public void DrainGhostTrailPool()
		{
			if (this._trailParts.Count > 0)
			{
				this._trailParts.Clear();
			}
		}

		private IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
		{
			Color color = trailPartRenderer.color;
			color.a = this.InitialGhostTrailAlpha;
			WaitForEndOfFrame delay = new WaitForEndOfFrame();
			while (color.a >= 0f)
			{
				if (!trailPartRenderer)
				{
					yield break;
				}
				trailPartRenderer.color = color;
				color.a -= this.AlphaStep;
				yield return delay;
			}
			if (!trailPartRenderer)
			{
				yield break;
			}
			color.a = 0f;
			trailPartRenderer.color = color;
			this.StoreGhostTrail(trailPartRenderer);
			yield break;
		}

		private readonly List<SpriteRenderer> _trailParts = new List<SpriteRenderer>();

		private LevelInitializer _currentLevel;

		public Material customMaterial;

		[Tooltip("The alpha decrease factor in every step")]
		[Range(0f, 1f)]
		public float AlphaStep = 0.1f;

		[Tooltip("The alpha value of the ghost trail SpriteRenderer color when it is spawned")]
		[Range(0f, 1f)]
		public float InitialGhostTrailAlpha = 0.6f;

		[Tooltip("The time rate step for invoking ghost trail")]
		[Range(0f, 1f)]
		public float TimeStep;

		[Tooltip("The color of the ghost trail sprite renderer")]
		public Color TrailColor;

		[Tooltip("Should the rotation of the sprite be followed?")]
		public bool followRotation;

		[Tooltip("Should the scale of the entity be followed?")]
		public bool followScale;

		[Tooltip("Should the orientation of the entity be followed?")]
		public bool followOrientation = true;

		[SerializeField]
		public bool EnableGhostTrail;

		private bool _startGenerateTrail;

		public static bool AreGhostTrailsAllowed = true;
	}
}
