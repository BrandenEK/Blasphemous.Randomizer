using System;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.DamageObject
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(SpriteRenderer))]
	[RequireComponent(typeof(Animator))]
	public class DamageObject : MonoBehaviour, IDamageable
	{
		private void Awake()
		{
			this.ObjectHit = new Hit
			{
				AttackingEntity = base.gameObject,
				DamageType = DamageArea.DamageType.Normal,
				DamageAmount = this.DamageAmount,
				HitSoundId = this.HitSoundFx,
				Unnavoidable = true
			};
			this.SpriteRenderer = base.GetComponent<SpriteRenderer>();
			this.Animator = base.GetComponent<Animator>();
			this.OnAwake();
		}

		protected virtual void OnAwake()
		{
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if ((this.AffectedEntities.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			IDamageable componentInChildren = other.transform.root.GetComponentInChildren<IDamageable>();
			if (componentInChildren != null)
			{
				this.Attack(componentInChildren);
			}
		}

		public virtual void Attack(IDamageable damageable)
		{
			damageable.Damage(this.ObjectHit);
		}

		public virtual void SetDestroyAnimation()
		{
			this.Animator.SetTrigger("DESTROY");
		}

		protected virtual void TakeDamage(Hit hit)
		{
			if (this.IsDestroyed)
			{
				return;
			}
			this.ObjectLife -= hit.DamageAmount;
			Core.Logic.ScreenFreeze.Freeze(0.1f, 0.1f, 0f, null);
			Core.Audio.PlaySfx(this.DamageSoundFx, 0f);
			if (this.ObjectLife > 0f)
			{
				return;
			}
			this.IsDestroyed = true;
			this.SetDestroyAnimation();
			base.GetComponent<Collider2D>().enabled = false;
			Core.Audio.PlaySfx(this.DestroySoundFx, 0f);
		}

		public virtual void DisableDamageObject()
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		public virtual void Damage(Hit hit)
		{
			this.TakeDamage(hit);
		}

		public virtual Vector3 GetPosition()
		{
			return base.transform.position;
		}

		public bool BleedOnImpact()
		{
			return false;
		}

		public bool SparkOnImpact()
		{
			return false;
		}

		[SerializeField]
		[Tooltip("The life base of the entity.")]
		protected float ObjectLife;

		[SerializeField]
		[Tooltip("The damage amount inflicted to the victim.")]
		protected float DamageAmount;

		[SerializeField]
		[EventRef]
		protected string HitSoundFx;

		[SerializeField]
		[EventRef]
		protected string DamageSoundFx;

		[SerializeField]
		[EventRef]
		protected string DestroySoundFx;

		protected SpriteRenderer SpriteRenderer;

		protected Animator Animator;

		protected bool IsDestroyed;

		protected Hit ObjectHit;

		[Tooltip("The layer of the potential victims.")]
		public LayerMask AffectedEntities;
	}
}
