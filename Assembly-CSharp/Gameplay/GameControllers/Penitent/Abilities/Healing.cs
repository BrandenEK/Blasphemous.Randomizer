using System;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Camera;
using Gameplay.GameControllers.Effects.Player.Healing;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using Gameplay.UI.Others.UIGameLogic;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class Healing : Ability
	{
		public bool IsHealing { get; private set; }

		public CameraPan CameraPan { get; private set; }

		public bool InvincibleEffect { get; set; }

		protected override void OnStart()
		{
			base.OnStart();
			this._stats = base.EntityOwner.Stats;
			if (this.HealingAura == null)
			{
				Debug.LogError("You need a Healing Aura Prefab");
			}
			else
			{
				this._healingAura = Object.Instantiate<GameObject>(this.HealingAura).GetComponent<HealingAura>();
			}
			base.EntityOwner.OnDeath += this.EntityOwnerOnEntityDie;
			base.EntityOwner.OnDamaged += this.OnOwnerDamaged;
			this.CameraPan = Object.FindObjectOfType<CameraPan>();
			this.swordHeart06 = Core.InventoryManager.GetSword("HE06");
		}

		private void LateUpdate()
		{
			if (this._healingAura != null)
			{
				this._healingAura.transform.position = base.EntityOwner.Animator.transform.position;
			}
			if (Core.Input.InputBlocked || UIController.instance.IsShowingMenu)
			{
				return;
			}
			if (this.GetHealingInput())
			{
				bool flag = base.Animator.GetCurrentAnimatorStateInfo(0).IsName("HardLanding");
				if (this.IsHealing || flag)
				{
					return;
				}
				if (!base.EntityOwner.Status.IsGrounded || base.EntityOwner.Status.IsHurt || base.EntityOwner.Status.Dead)
				{
					return;
				}
				if (this.swordHeart06 != null && this.swordHeart06.IsEquiped)
				{
					return;
				}
				if (this._stats.Flask.Current < 1f)
				{
					Core.Logic.Penitent.Audio.EmptyFlask();
					return;
				}
				base.Cast();
			}
		}

		private bool GetHealingInput()
		{
			bool result = false;
			if (this.CameraPan != null)
			{
				result = (base.Rewired.GetButtonDown(23) && !Core.Logic.Penitent.PlatformCharacterController.GetActionState(16));
			}
			return result;
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			this.Heal();
			base.Animator.Play(this._healingAnimHash);
			this.IsHealing = true;
			this._healingAura.StartAura(base.EntityOwner.Status.Orientation);
			Core.Logic.Penitent.Audio.UseFlask();
			RosaryBead rosaryBead = Core.InventoryManager.GetRosaryBead("RB28");
			if (rosaryBead != null && rosaryBead.IsEquiped)
			{
				base.EntityOwner.Status.Invulnerable = true;
				this.InvincibleEffect = true;
			}
			if (Healing.OnHealingStart != null)
			{
				Healing.OnHealingStart();
			}
		}

		private void Heal()
		{
			if (this._stats.Flask.Current < 1f)
			{
				return;
			}
			this._stats.Flask.Current -= 1f;
			if (Core.PenitenceManager.UseFervourFlasks)
			{
				this._stats.Fervour.Current += this._stats.FlaskHealth.Final;
				PlayerFervour.Instance.ShowSpark();
			}
			else if (Core.PenitenceManager.UseStocksOfHealth)
			{
				this._stats.Life.Current += PlayerHealthPE02.StocksHeal;
				Core.PenitenceManager.ResetRegeneration();
			}
			else
			{
				this._stats.Life.Current += this._stats.FlaskHealth.Final;
			}
		}

		public void StopHeal()
		{
			if (!this.IsHealing)
			{
				return;
			}
			this.IsHealing = false;
			this._healingAura.StopAura();
			Core.Logic.Penitent.Audio.StopUseFlask();
			base.StopCast();
			if (!this.InvincibleEffect)
			{
				return;
			}
			base.EntityOwner.Status.Invulnerable = false;
			this.InvincibleEffect = false;
		}

		private void OnOwnerDamaged()
		{
			if (this.IsHealing)
			{
				base.Animator.SetTrigger("HURT");
				this.StopHeal();
			}
		}

		private void EntityOwnerOnEntityDie()
		{
			if (this._healingAura == null)
			{
				return;
			}
			Object.Destroy(this._healingAura.gameObject);
			base.enabled = false;
		}

		private void OnDestroy()
		{
			if (!base.EntityOwner)
			{
				return;
			}
			base.EntityOwner.OnDeath -= this.EntityOwnerOnEntityDie;
			base.EntityOwner.OnDamaged -= this.OnOwnerDamaged;
		}

		public static Core.SimpleEvent OnHealingStart;

		private readonly int _healingAnimHash = Animator.StringToHash("Healing");

		public GameObject HealingAura;

		private HealingAura _healingAura;

		private EntityStats _stats;

		private Sword swordHeart06;
	}
}
