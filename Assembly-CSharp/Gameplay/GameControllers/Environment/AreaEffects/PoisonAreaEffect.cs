using System;
using System.Linq;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Effects.Entity;
using Gameplay.GameControllers.Entities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.AreaEffects
{
	public class PoisonAreaEffect : AreaEffect
	{
		protected override void OnAwake()
		{
			base.OnAwake();
			this._hit = new Hit
			{
				DamageAmount = this.DamageAmount,
				DamageElement = DamageArea.DamageElement.Toxic,
				AttackingEntity = base.gameObject,
				DamageType = DamageArea.DamageType.Simple,
				Unnavoidable = true
			};
		}

		protected override void OnStayAreaEffect()
		{
			base.OnStayAreaEffect();
			this.PoisonEntities();
		}

		public void PoisonEntities()
		{
			for (int i = 0; i < this.Population.Count; i++)
			{
				if (!(this.Population.ElementAtOrDefault(i) == null))
				{
					this.PoisonGameObject(this.Population[i]);
				}
			}
		}

		private void PoisonGameObject(GameObject entityGameObject)
		{
			Entity componentInParent = entityGameObject.GetComponentInParent<Entity>();
			if (componentInParent != null)
			{
				if (componentInParent.Status.Dead)
				{
					return;
				}
				this.PoisonEntity(componentInParent);
			}
			else
			{
				IDamageable componentInParent2 = entityGameObject.GetComponentInParent<IDamageable>();
				this.PoisonDamageable(componentInParent2);
			}
			Core.Audio.PlaySfx(this.DamageAudioEffect, 0f);
		}

		private void PoisonEntity(Entity entity)
		{
			if (entity == null)
			{
				return;
			}
			IDamageable damageable = entity as IDamageable;
			if (damageable == null)
			{
				return;
			}
			this._hit.DamageAmount = this.DamageAmount;
			this._hit.DamageAmount = entity.GetReducedDamage(this._hit);
			damageable.Damage(this._hit);
			if (entity.Status.Dead)
			{
				base.RemoveEntityToAreaPopulation(entity.gameObject);
			}
			this.TriggerVisualEffect(entity);
		}

		private void PoisonDamageable(IDamageable damageable)
		{
			if (damageable == null)
			{
				return;
			}
			this._hit.DamageAmount = this.DamageAmount;
			damageable.Damage(this._hit);
		}

		private void TriggerVisualEffect(Entity entity)
		{
			MasterShaderEffects componentInChildren = entity.GetComponentInChildren<MasterShaderEffects>();
			if (componentInChildren != null && this.DamageEffectMaterial != null)
			{
				componentInChildren.DamageEffectBlink(0f, 0.2f, this.DamageEffectMaterial);
			}
		}

		[FoldoutGroup("EffectSettings", true, 0)]
		[Tooltip("Damage amount per lapse.")]
		public float DamageAmount;

		[FoldoutGroup("EffectSettings", true, 0)]
		[EventRef]
		public string DamageAudioEffect;

		[FoldoutGroup("EffectSettings", true, 0)]
		public Material DamageEffectMaterial;

		private Hit _hit;
	}
}
