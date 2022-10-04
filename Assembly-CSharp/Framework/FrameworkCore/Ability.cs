using System;
using System.Collections.Generic;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.FrameworkCore
{
	public class Ability : MonoBehaviour
	{
		public Entity EntityOwner { get; private set; }

		private protected Animator Animator { protected get; private set; }

		public string LastUnlockedSkillId { get; protected set; }

		public Player Rewired { get; private set; }

		public bool IsUsingAbility { get; protected set; }

		private IList<string> SkillsValues()
		{
			return null;
		}

		protected UnlockableSkill GetLastUnlockedSkill()
		{
			UnlockableSkill result = null;
			foreach (string skill in this.unlocableSkill)
			{
				if (!Core.SkillManager.IsSkillUnlocked(skill))
				{
					break;
				}
				result = Core.SkillManager.GetSkill(skill);
			}
			return result;
		}

		protected bool CanExecuteSkilledAbility()
		{
			return !this.useUnlocableSkill || this.GetLastUnlockedSkill() != null;
		}

		protected void SetCooldown(float newCooldown)
		{
			this.timeCount = newCooldown;
			this.cooldown = newCooldown;
		}

		protected virtual float FervorConsumption
		{
			get
			{
				float num = this.MinFervourConsumption;
				if (this.SwordHeart && Core.InventoryManager.IsSwordEquipped(this.SwordHeart.id))
				{
					float num2 = this.MinFervourConsumption * this.ReducePercentageByHeart / 100f;
					num -= num2;
				}
				return num;
			}
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnStart()
		{
			this.IsUsingAbility = false;
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnFixedUpdate()
		{
		}

		protected virtual void OnDamageReceived()
		{
		}

		protected virtual void OnCastStart()
		{
			if (this.ConsumeFervour)
			{
				this.EntityOwner.Stats.Fervour.Current -= this.FervorConsumption;
			}
			this.IsUsingAbility = true;
		}

		protected virtual void OnCastEnd(float castingTime)
		{
			this.ToggleAbilities(true);
			this.IsUsingAbility = false;
		}

		protected virtual void OnFixedCastStart()
		{
		}

		protected virtual void OnDead()
		{
		}

		protected virtual bool Condition()
		{
			return true;
		}

		protected virtual string Description()
		{
			return this.abilityDescription;
		}

		protected virtual string CastInformation()
		{
			return this.castInformation;
		}

		protected virtual void OnAbilityGizmosSelected()
		{
		}

		private void Reset()
		{
			this.abilityDescription = this.Description();
		}

		private void OnDrawGizmosSelected()
		{
			this.OnAbilityGizmosSelected();
		}

		private void Awake()
		{
			this.castTime = 0f;
			this.ReloadOwner();
			this.OnAwake();
		}

		private void Start()
		{
			this.Rewired = ReInput.players.GetPlayer(0);
			this.castTime = 0f;
			if (this.EntityOwner)
			{
				this.OnStart();
			}
		}

		private void Update()
		{
			if (!this.EntityOwner)
			{
				return;
			}
			this.UpdateInput();
			this.UpdateCooldown();
			this.OnUpdate();
		}

		private void FixedUpdate()
		{
			this.OnFixedUpdate();
			if (!this.fixedCastScheduled)
			{
				return;
			}
			this.fixedCastScheduled = false;
			this.OnFixedCastStart();
		}

		private void UpdateCooldown()
		{
			if (this.timeCount > 0f)
			{
				this.timeCount -= Time.deltaTime;
			}
			else if (this.timeCount < 0f)
			{
				this.timeCount = 0f;
				this.OnCooldownFinished();
			}
		}

		private void UpdateInput()
		{
			if (!this.EntityOwner || this.Rewired == null)
			{
				return;
			}
			if (!this.EntityOwner.CompareTag("Penitent"))
			{
				return;
			}
			if (this.Rewired.GetButtonDown(this.triggerCode))
			{
				this.Cast();
			}
			if (this.Rewired.GetButtonUp(this.triggerCode))
			{
				this.StopCast();
			}
		}

		public void Cast()
		{
			if (!this.EntityOwner)
			{
				this.castInformation = "INVALID OWNER";
			}
			else if (this.Casting)
			{
				this.castInformation = "ALREADY CASTING";
			}
			else if (!this.ReadyToUse)
			{
				this.castInformation = "ABILITY NOT READY";
			}
			else if (this.EntityOwner.Status.Dead)
			{
				this.castInformation = "ENTITY DEAD";
			}
			else if (this.EntityOwner.Status.AbilitiesDisabled)
			{
				this.castInformation = "ABILITIES DISABLED";
			}
			else if (!this.Condition())
			{
				this.castInformation = "CONDITION NOT MET";
			}
			else
			{
				this.castInformation = "SUCCESSFULLY EXECUTED";
				this.castTime += Time.deltaTime;
				this.StartCooldown();
				this.OnCastStart();
				this.fixedCastScheduled = true;
				this.ToggleAbilities(false);
			}
		}

		public void StopCast()
		{
			if (this.Casting)
			{
				this.OnCastEnd(this.castTime);
				this.castTime = 0f;
			}
		}

		private void StartCooldown()
		{
			this.timeCount = this.cooldown;
		}

		private void ReloadOwner()
		{
			this.EntityOwner = base.GetComponentInParent<Entity>();
			if (this.EntityOwner != null)
			{
				this.Animator = this.EntityOwner.GetComponentInChildren<Animator>();
			}
		}

		public bool ReadyToUse
		{
			get
			{
				return this.timeCount <= 0f;
			}
		}

		public bool Casting
		{
			get
			{
				return this.castTime > 0f;
			}
		}

		protected bool HasEnoughFervour
		{
			get
			{
				return !this.ConsumeFervour || this.EntityOwner.Stats.Fervour.Current >= this.FervorConsumption;
			}
		}

		protected float CastingTime
		{
			get
			{
				return this.castTime;
			}
		}

		protected virtual void OnCooldownFinished()
		{
		}

		protected virtual void ToggleAbilities(bool toggle)
		{
			foreach (MonoBehaviour monoBehaviour in this.DisableAbilities)
			{
				monoBehaviour.enabled = toggle;
			}
		}

		private float timeCount;

		private bool fixedCastScheduled;

		[FoldoutGroup("Fervour Consumption", true, 0)]
		public bool ConsumeFervour;

		[FoldoutGroup("Fervour Consumption", true, 0)]
		[ShowIf("ConsumeFervour", true)]
		public Sword SwordHeart;

		[FoldoutGroup("Fervour Consumption", true, 0)]
		[ShowIf("ConsumeFervour", true)]
		public float MinFervourConsumption = 5f;

		[FoldoutGroup("Fervour Consumption", true, 0)]
		[Range(0f, 100f)]
		[ShowIf("ConsumeFervour", true)]
		public float ReducePercentageByHeart = 5f;

		public MonoBehaviour[] DisableAbilities;

		[SerializeField]
		[FoldoutGroup("Information", true, 0)]
		[TextArea]
		[ReadOnly]
		private string abilityDescription;

		[SerializeField]
		[FoldoutGroup("Information", true, 0)]
		[ReadOnly]
		private string castInformation;

		[SerializeField]
		[FoldoutGroup("Generic Settings", true, 0)]
		private float cooldown;

		[SerializeField]
		[FoldoutGroup("Generic Settings", true, 0)]
		private float castTime;

		[SerializeField]
		[FoldoutGroup("Generic Settings", true, 0)]
		private float castEnergyCost;

		[SerializeField]
		[FoldoutGroup("Generic Settings", true, 0)]
		private string triggerCode;

		[SerializeField]
		[FoldoutGroup("Skills Settings", true, 0)]
		private bool useUnlocableSkill;

		[SerializeField]
		[FoldoutGroup("Skills Settings", true, 0)]
		[ShowIf("useUnlocableSkill", true)]
		[ValueDropdown("SkillsValues")]
		private List<string> unlocableSkill = new List<string>();

		[TutorialId]
		public string TutorialId;
	}
}
