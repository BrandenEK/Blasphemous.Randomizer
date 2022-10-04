using System;
using System.Collections;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Damage;
using UnityEngine;

namespace Framework.Penitences
{
	public class PenitencePE01 : IPenitence
	{
		public PenitencePE01()
		{
			this.fervourRegenCoroutine = this.FervourRegenCoroutine();
			this.balance = Resources.Load<Pe01Balance>("PE01/PE01Balance");
			if (!this.balance)
			{
				Debug.LogError("Can't find PE01 balance at PE01/PE01Balance");
			}
		}

		public string Id
		{
			get
			{
				return "PE01";
			}
		}

		public bool Completed { get; set; }

		public bool Abandoned { get; set; }

		public void Activate()
		{
			this.RemoveEffects();
			this.AddEffects();
		}

		public void Deactivate()
		{
			this.RemoveEffects();
		}

		private void RemoveEffects()
		{
			Core.Logic.Penitent.Stats.Strength.FinalStrengthMultiplier = this.balance.normalStrengthMultiplier;
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Remove(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
			Singleton<Core>.Instance.StopCoroutine(this.fervourRegenCoroutine);
		}

		private void AddEffects()
		{
			Core.Logic.Penitent.Stats.Strength.FinalStrengthMultiplier = this.balance.pe01StrengthMultiplier;
			PenitentDamageArea.OnDamagedGlobal = (PenitentDamageArea.PlayerDamagedEvent)Delegate.Combine(PenitentDamageArea.OnDamagedGlobal, new PenitentDamageArea.PlayerDamagedEvent(this.OnDamagedGlobal));
			Singleton<Core>.Instance.StartCoroutine(this.fervourRegenCoroutine);
		}

		private void OnDamagedGlobal(Penitent damaged, Hit hit)
		{
			if (hit.DamageAmount > 0f)
			{
				float a = Core.Logic.Penitent.Stats.Fervour.Current - this.balance.fervourLostAmount;
				Core.Logic.Penitent.Stats.Fervour.Current = Mathf.Max(a, 0f);
			}
		}

		private IEnumerator FervourRegenCoroutine()
		{
			for (;;)
			{
				yield return new WaitForSeconds(this.balance.timePerRegenerationTick);
				Core.Logic.Penitent.Stats.Fervour.Current += this.balance.fervourRecoveryAmount;
			}
			yield break;
		}

		private const string id = "PE01";

		private readonly IEnumerator fervourRegenCoroutine;

		private readonly Pe01Balance balance;

		private const string BALANCE_PATH = "PE01/PE01Balance";
	}
}
