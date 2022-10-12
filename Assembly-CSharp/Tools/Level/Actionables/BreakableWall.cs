using System;
using System.Collections;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools.Level.Layout;
using UnityEngine;

namespace Tools.Level.Actionables
{
	[RequireComponent(typeof(Collider2D))]
	public class BreakableWall : PersistentObject, IActionable, IDamageable
	{
		public void Use()
		{
			Hit hit = new Hit
			{
				DamageAmount = 100f
			};
			this.Damage(hit);
		}

		public bool Locked { get; set; }

		public void Damage(Hit hit = default(Hit))
		{
			if (this.destroyed)
			{
				return;
			}
			if (this.layoutMode && this.layoutElement != null)
			{
				this.colorFlash.TriggerColorFlash();
			}
			this.health -= hit.DamageAmount;
			if (this.health <= 0f)
			{
				this.destroyed = true;
				base.gameObject.layer = LayerMask.NameToLayer("Floor");
				base.StartCoroutine(this.DestroyWall());
			}
		}

		private bool PenitentIsFacingDamageableDirection()
		{
			if (this.breakableFrom == BreakableWall.DAMAGEABLE_DIRECTION_LOCK.BOTH)
			{
				return true;
			}
			if (!Core.ready || Core.Logic == null || Core.Logic.Penitent == null)
			{
				return false;
			}
			bool flag = Core.Logic.Penitent.transform.position.x > base.transform.position.x;
			return (this.breakableFrom == BreakableWall.DAMAGEABLE_DIRECTION_LOCK.RIGHT && flag) || (this.breakableFrom == BreakableWall.DAMAGEABLE_DIRECTION_LOCK.LEFT && !flag);
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		private void Awake()
		{
			this.layoutElement = base.GetComponent<LayoutElement>();
			this.wallCollider = base.GetComponent<Collider2D>();
			this.colorFlash = base.GetComponent<ColorFlash>();
			if (this.layoutElement != null)
			{
				this.layoutMode = (this.layoutElement.category == Category.Layout);
			}
		}

		private void Update()
		{
			if (!this.destroyed)
			{
				this.wallCollider.enabled = this.PenitentIsFacingDamageableDirection();
			}
		}

		private IEnumerator DestroyWall()
		{
			this.Secret.Reveal();
			if (this.layoutMode && this.layoutElement != null)
			{
				this.SetColor(Color.green);
			}
			this.PlayBreakSound();
			for (int i = 0; i < this.OnDestroy.Length; i++)
			{
				if (!(this.OnDestroy[i] == null))
				{
					IActionable[] components = this.OnDestroy[i].GetComponents<IActionable>();
					components.ForEach(delegate(IActionable actionable)
					{
						actionable.Use();
					});
				}
			}
			yield return new WaitForSeconds(this.RemainTimeAfterHit);
			this.wallCollider.enabled = false;
			if (this.layoutElement != null && this.layoutMode)
			{
				Color color = this.layoutElement.SpriteRenderer.material.color;
				color.a = 0f;
				this.layoutElement.SpriteRenderer.material.color = color;
			}
			yield break;
		}

		private void PlayBreakSound()
		{
			if (!this.AllowPlaySoundFx || string.IsNullOrEmpty(this.BreakWallFx))
			{
				return;
			}
			Core.Audio.PlaySfx(this.BreakWallFx, 0f);
		}

		private void SetColor(Color c)
		{
			Color color = new Color(c.r, c.g, c.b);
			this.layoutElement.SpriteRenderer.material.color = color;
		}

		public bool Destroyed
		{
			get
			{
				return this.destroyed;
			}
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			BreakableWall.WallPersistenceData wallPersistenceData = base.CreatePersistentData<BreakableWall.WallPersistenceData>();
			wallPersistenceData.Life = this.health;
			return wallPersistenceData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			BreakableWall.WallPersistenceData wallPersistenceData = (BreakableWall.WallPersistenceData)data;
			this.health = wallPersistenceData.Life;
			this.destroyed = (this.health <= 0f);
			if (this.destroyed)
			{
				base.gameObject.layer = LayerMask.NameToLayer("Floor");
				if (this.layoutMode && this.layoutElement != null)
				{
					Color color = this.layoutElement.SpriteRenderer.material.color;
					color.a = 0f;
					this.layoutElement.SpriteRenderer.material.color = color;
				}
			}
			this.wallCollider.enabled = !this.destroyed;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public GameObject[] OnDestroy;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float health;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float RemainTimeAfterHit = 2f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private BreakableWall.DAMAGEABLE_DIRECTION_LOCK breakableFrom;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private SecretReveal Secret = new SecretReveal();

		[BoxGroup("Audio", true, false, 0)]
		public bool AllowPlaySoundFx = true;

		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		public string BreakWallFx;

		private ColorFlash colorFlash;

		private bool destroyed;

		private LayoutElement layoutElement;

		private bool layoutMode;

		private Collider2D wallCollider;

		public enum DAMAGEABLE_DIRECTION_LOCK
		{
			BOTH,
			RIGHT,
			LEFT
		}

		private class WallPersistenceData : PersistentManager.PersistentData
		{
			public WallPersistenceData(string id) : base(id)
			{
			}

			public float Life;
		}
	}
}
