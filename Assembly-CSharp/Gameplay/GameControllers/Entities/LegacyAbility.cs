using System;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class LegacyAbility : MonoBehaviour
	{
		private void Start()
		{
			this.entity = base.transform.parent.GetComponent<Entity>();
		}

		protected virtual void OnAbilityStart()
		{
		}

		protected virtual void OnAbilityUpdate()
		{
		}

		protected virtual void OnAbilityEnd()
		{
		}

		private void Update()
		{
			this.UpdateCooldown();
			if (this.IsControlPressed())
			{
				this.Use();
			}
		}

		private void UpdateCooldown()
		{
			if (this.cooldownCount > 0f)
			{
				this.cooldownCount -= Time.deltaTime;
				this.OnAbilityUpdate();
			}
			else if (this.cooldownCount < 0f)
			{
				this.cooldownCount = 0f;
				this.OnAbilityEnd();
			}
		}

		public bool IsControlPressed()
		{
			return false;
		}

		public bool HasCooldown()
		{
			return this.cooldownCount > 0f;
		}

		public void Use()
		{
			if (!this.HasCooldown())
			{
				this.StartCooldown();
				this.OnAbilityStart();
			}
		}

		public void ResetCooldown()
		{
			this.cooldownCount = 0f;
		}

		public void StartCooldown()
		{
			this.cooldownCount = this.cooldownBase;
		}

		[Header("Ability Settings")]
		[SerializeField]
		[Tooltip("Identificator of the ability. If the ID matches a wiredaction the ability will be executed on the stabilished key event.")]
		private string abilityID = string.Empty;

		[SerializeField]
		[Tooltip("Percentaje reduction of the cooldown base")]
		private float cooldownBase;

		private float cooldownCount;

		protected Entity entity;
	}
}
