using System;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Gameplay.UI;
using Gameplay.UI.Others.UIGameLogic;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class FervourPenance : Ability
	{
		protected override void OnStart()
		{
			base.OnStart();
			this._releaseTriggerButton = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			this._rewired = Core.Logic.Penitent.PlatformCharacterInput.Rewired;
			if (this._rewired == null)
			{
				return;
			}
			if (this._rewired.GetButtonUp("Range Attack"))
			{
				this._releaseTriggerButton = true;
			}
			if (!base.Casting && this._releaseTriggerButton && this._disabledAbilities)
			{
				this._disabledAbilities = false;
				Core.Logic.Penitent.RangeAttack.enabled = true;
			}
			if (base.Casting && !base.EntityOwner.Animator.GetCurrentAnimatorStateInfo(0).IsName("FervourPenance"))
			{
				base.StopCast();
			}
			if (!this._rewired.GetButtonShortPress("Range Attack") || base.Casting || !this._releaseTriggerButton)
			{
				return;
			}
			this._releaseTriggerButton = false;
			Core.Logic.Penitent.RangeAttack.enabled = false;
			this._disabledAbilities = true;
			if (Core.Input.InputBlocked || UIController.instance.IsShowingMenu || !base.EntityOwner.Status.IsGrounded)
			{
				return;
			}
			if (this.CanDoPenanceForFervour)
			{
				base.Cast();
			}
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			Core.Events.LaunchEvent(this.EventFired, string.Empty);
			Core.Input.SetBlocker("PLAYER_LOGIC", true);
			this.DrainResources();
			this.RestoreFervour(this.FervourRestored);
			base.EntityOwner.Animator.Play(this._fervourPenanceAnim);
			Core.Audio.PlaySfx(this.FervourPenanceFx, 0f);
			if (FervourPenance.OnPenanceStart != null)
			{
				FervourPenance.OnPenanceStart();
			}
		}

		protected override void OnCastEnd(float castingTime)
		{
			base.OnCastEnd(castingTime);
			Core.Input.SetBlocker("PLAYER_LOGIC", false);
		}

		private bool CanDoPenanceForFervour
		{
			get
			{
				float num = (!Core.PenitenceManager.UseStocksOfHealth) ? this.MinLifeConsumption : 30f;
				bool flag = base.EntityOwner.Stats.Purge.Current > this.GetPurgeCost();
				bool flag2 = base.EntityOwner.CurrentLife > num;
				return flag && flag2;
			}
		}

		private float GetPurgeCost()
		{
			float final = base.EntityOwner.Stats.MeaCulpa.Final;
			return this.BasePurgeConsumption + this.BasePurgeConsumption * final * this.PurgeCostScaleByLevel;
		}

		private void DrainResources()
		{
			base.EntityOwner.Stats.Purge.Current -= this.GetPurgeCost();
			float num = (!Core.PenitenceManager.UseStocksOfHealth) ? this.MinLifeConsumption : PlayerHealthPE02.StocksDamage;
			base.EntityOwner.CurrentLife -= num;
			if (Core.PenitenceManager.UseStocksOfHealth)
			{
				Core.PenitenceManager.ResetRegeneration();
			}
		}

		private void RestoreFervour(float fervour)
		{
			float value = Mathf.Clamp(base.EntityOwner.Stats.Fervour.Current + fervour, base.EntityOwner.Stats.Fervour.Current, base.EntityOwner.Stats.Fervour.MaxValue);
			base.EntityOwner.Stats.Fervour.Current = value;
			PlayerFervour.Instance.ShowSpark();
		}

		public static Core.SimpleEvent OnPenanceStart;

		private Player _rewired;

		[FoldoutGroup("Ability Settings", true, 0)]
		public float BasePurgeConsumption;

		[FoldoutGroup("Ability Settings", true, 0)]
		public float PurgeCostScaleByLevel;

		[FoldoutGroup("Ability Settings", true, 0)]
		public float MinLifeConsumption;

		[FoldoutGroup("Ability Settings", true, 0)]
		public float FervourRestored;

		[FoldoutGroup("Event Fired", true, 0)]
		public string EventFired;

		[FoldoutGroup("Audio", true, 0)]
		[EventRef]
		public string FervourPenanceFx;

		private readonly int _fervourPenanceAnim = Animator.StringToHash("FervourPenance");

		private bool _releaseTriggerButton;

		private bool _disabledAbilities;
	}
}
