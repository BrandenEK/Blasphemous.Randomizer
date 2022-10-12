using System;
using FMOD.Studio;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Bosses.CommonAttacks;
using Gameplay.GameControllers.Bosses.Quirce.Attack;
using Gameplay.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Penitent.Abilities
{
	public class PrayerUse : Ability
	{
		public Prayer GetEquippedPrayer()
		{
			return Core.InventoryManager.GetPrayerInSlot(this.slot);
		}

		public float GetPercentTimeCasting()
		{
			float result = 0f;
			if (base.IsUsingAbility && this.timeToEnd > 0f)
			{
				result = this.timeCasting / this.timeToEnd;
			}
			return result;
		}

		protected override void OnCastStart()
		{
			base.OnCastStart();
			Prayer equippedPrayer = this.GetEquippedPrayer();
			if (equippedPrayer)
			{
				this.StartUsingPrayer(equippedPrayer);
			}
			if (this.OnPrayerStart != null)
			{
				this.OnPrayerStart();
			}
		}

		protected override void OnDead()
		{
			if (this.audioInstance.isValid())
			{
				this.audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			this._penitent = base.EntityOwner.GetComponent<Penitent>();
			if (!this.audioInstance.isValid())
			{
				this.audioInstance = Core.Audio.CreateEvent(this.soundCast, default(Vector3));
				this.audioInstance.setParameterValue(this.soundEventParameter, 0f);
			}
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (base.Rewired.GetButtonTimedPressDown(25, 0f) && !Core.Input.InputBlocked)
			{
				Prayer prayerInSlot = Core.InventoryManager.GetPrayerInSlot(this.slot);
				if (prayerInSlot)
				{
					bool flag = !base.Casting && base.EntityOwner.Status.IsGrounded && !base.IsUsingAbility;
					if (flag && this._penitent.Stats.Fervour.Current < (float)prayerInSlot.fervourNeeded + this._penitent.Stats.PrayerCostAddition.Final)
					{
						UIController.instance.NotEnoughFervour();
					}
				}
				else
				{
					UIController.instance.NotEnoughFervour();
				}
				if (this.CanUsePrayer)
				{
					base.EntityOwner.Animator.Play(this._animAuraTransform);
				}
			}
			if (base.IsUsingAbility)
			{
				this.timeToLaunchEvent -= Time.deltaTime;
				if (this.audioInstance.isValid() && !this.soundEventLaunched && this.timeToLaunchEvent <= 0f)
				{
					this.soundEventLaunched = true;
					this.audioInstance.setParameterValue(this.soundEventParameter, 1f);
				}
				this.timeCasting += Time.deltaTime;
				if (this.timeCasting >= this.timeToEnd)
				{
					this.timeCasting = this.timeToEnd;
					this.EndUsingPrayer();
				}
			}
		}

		private void OnDestroy()
		{
			if (this.audioInstance.isValid())
			{
				this.audioInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this.audioInstance.release();
				this.audioInstance = default(EventInstance);
			}
		}

		private void StartUsingPrayer(Prayer prayer)
		{
			Fervour fervour = this._penitent.Stats.Fervour;
			float num = prayer.EffectTime;
			if (num > 0f)
			{
				num += this._penitent.Stats.PrayerDurationAddition.Final;
			}
			if (Math.Abs(num) > Mathf.Epsilon)
			{
				this.timeToLaunchEvent = num - this.soundEventTime;
				this.soundEventLaunched = false;
			}
			else
			{
				this.soundEventLaunched = true;
			}
			this.timeToEnd = num;
			this.timeCasting = 0f;
			fervour.Current -= (float)prayer.fervourNeeded + this._penitent.Stats.PrayerCostAddition.Final;
			prayer.Use();
			if (this.audioInstance.isValid())
			{
				this.audioInstance.setParameterValue(this.soundEventParameter, 0f);
				this.audioInstance.start();
			}
		}

		public void ForcePrayerEnd()
		{
			this.EndUsingPrayer();
		}

		private void EndUsingPrayer()
		{
			base.StopCast();
			if (this.audioInstance.isValid() && !this.soundEventLaunched)
			{
				this.audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
		}

		private bool CanUsePrayer
		{
			get
			{
				bool result = false;
				Prayer prayerInSlot = Core.InventoryManager.GetPrayerInSlot(this.slot);
				if (prayerInSlot)
				{
					result = (!base.Casting && base.EntityOwner.Status.IsGrounded && !base.IsUsingAbility && this._penitent.Stats.Fervour.Current >= (float)prayerInSlot.fervourNeeded + this._penitent.Stats.PrayerCostAddition.Final && !Core.Input.InputBlocked && !Core.Input.HasBlocker("CONSOLE"));
				}
				return result;
			}
		}

		private void PlaySound(string sound)
		{
			if (!string.IsNullOrEmpty(sound))
			{
				Core.Audio.PlayOneShot(sound, default(Vector3));
			}
		}

		public Core.SimpleEvent OnPrayerStart;

		[FoldoutGroup("Prayers reference", 0)]
		public BossInstantProjectileAttack multishotPrayer;

		[FoldoutGroup("Prayers reference", 0)]
		public BossAreaSummonAttack lightBeamPrayer;

		[FoldoutGroup("Prayers reference", 0)]
		public BossAreaSummonAttack flamePillarsPrayer;

		[FoldoutGroup("Prayers reference", 0)]
		public BossAreaSummonAttack divineLightPrayer;

		[FoldoutGroup("Prayers reference", 0)]
		public BossAreaSummonAttack stuntPrayer;

		[FoldoutGroup("Prayers reference", 0)]
		public BossStraightProjectileAttack crawlerBallsPrayer;

		[FoldoutGroup("Prayers reference", 0)]
		public ShieldSystemPrayer shieldPrayer;

		[FoldoutGroup("Prayers reference", 0)]
		public AlliedCherubPrayer cherubPrayer;

		public int slot;

		public string soundCast = "event:/SFX/Penitent/Prayers/PenitentFervor";

		public float soundEventTime = 2.4f;

		public string soundEventParameter = "EndPrayer";

		public float timeToUseHability = 0.02f;

		public const float TimePressedToActivate = 0f;

		private readonly int _animAuraTransform = Animator.StringToHash("AuraTransform");

		private Penitent _penitent;

		private float timeToLaunchEvent;

		private bool soundEventLaunched;

		private EventInstance audioInstance = default(EventInstance);

		private float timeCasting;

		private float timeToEnd;
	}
}
