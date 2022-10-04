using System;
using DamageEffect;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Bosses.BejeweledSaint
{
	public class BejeweledSaintHolder : MonoBehaviour, IDamageable
	{
		public Animator Animator { get; set; }

		public SpriteRenderer SpriteRenderer { get; set; }

		public DamageEffectScript DamageEffect { get; set; }

		public BoxCollider2D DamageArea { get; private set; }

		private void Start()
		{
			this.Animator = base.GetComponent<Animator>();
			this.SpriteRenderer = base.GetComponent<SpriteRenderer>();
			this.DamageEffect = base.GetComponent<DamageEffectScript>();
			this.DamageArea = base.GetComponent<BoxCollider2D>();
			this.Animator.SetInteger("ID", this.Id);
			this._currentLife = this.MaxLife;
			this._defaultLocalPosition = new Vector2(base.transform.localPosition.x, base.transform.localPosition.y);
			this._defaultParent = base.transform.parent;
		}

		public void Damage(Hit hit)
		{
			this.DamageEffect.Blink(0f, 0.1f);
			this._currentLife--;
			Core.Audio.PlaySfx(this.DamageSoundId, 0f);
			if (this._currentLife <= 0 && !this.IsCollapsed)
			{
				this.IsCollapsed = true;
				this.GetDown();
				this.Animator.SetTrigger("DISAPPEAR");
				Core.Audio.PlaySfx(this.DownFallSoundId, 0f);
			}
		}

		public void EnableDamageArea(bool enableDamageArea)
		{
			if (this.DamageArea == null)
			{
				return;
			}
			this.DamageArea.enabled = enableDamageArea;
		}

		public void Heal()
		{
			this._currentLife = this.MaxLife;
		}

		public Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public void GetDown()
		{
			base.transform.parent = base.transform.root;
			if (BejeweledSaintHolder.OnHolderCollapse != null)
			{
				BejeweledSaintHolder.OnHolderCollapse();
			}
		}

		public void SetDefaultLocalPosition()
		{
			base.transform.parent = this._defaultParent;
			this.IsCollapsed = false;
			base.transform.localPosition = this._defaultLocalPosition;
			this.SpriteRenderer.enabled = true;
			this.DamageArea.GetComponent<BoxCollider2D>().enabled = true;
		}

		public void OnDisappear()
		{
			this.SpriteRenderer.enabled = false;
			this.DamageArea.GetComponent<BoxCollider2D>().enabled = false;
		}

		public bool BleedOnImpact()
		{
			return true;
		}

		public bool SparkOnImpact()
		{
			return true;
		}

		public static Core.SimpleEvent OnHolderCollapse;

		public int Id;

		public AnimationCurve AppearingMoveCurve;

		private Transform _defaultParent;

		[EventRef]
		public string DamageSoundId;

		[EventRef]
		public string ShieldedSoundId;

		[EventRef]
		public string DownFallSoundId;

		public int MaxLife;

		private int _currentLife;

		public bool IsCollapsed;

		private Vector2 _defaultLocalPosition;
	}
}
