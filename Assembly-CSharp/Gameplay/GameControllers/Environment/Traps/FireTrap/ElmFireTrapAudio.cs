using System;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Traps.FireTrap
{
	[RequireComponent(typeof(ElmFireTrap))]
	public class ElmFireTrapAudio : MonoBehaviour
	{
		private void Start()
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
			this.elmFireTrap = base.GetComponent<ElmFireTrap>();
			ElmFireTrap elmFireTrap = this.elmFireTrap;
			elmFireTrap.OnChargeStart = (Core.SimpleEventParam)Delegate.Combine(elmFireTrap.OnChargeStart, new Core.SimpleEventParam(this.StartCharge));
			ElmFireTrap elmFireTrap2 = this.elmFireTrap;
			elmFireTrap2.OnLightningCast = (Core.SimpleEventParam)Delegate.Combine(elmFireTrap2.OnLightningCast, new Core.SimpleEventParam(this.PlayShot));
		}

		private void Update()
		{
			if (!this.IsTrapVisible())
			{
				if (this.idleIsPlaying)
				{
					this.StopIdle();
				}
				if (this.chargeIsPlaying)
				{
					this.StopCharge();
				}
				return;
			}
			if (this.chargeIsPlaying)
			{
				this.currentChargingTime += Time.deltaTime;
				float percentage = this.currentChargingTime / this.totalChargingTime;
				this.UpdateChargeAudioParameter(percentage);
			}
			else if (!this.idleIsPlaying)
			{
				this.PlayIdle();
			}
		}

		private void OnDisable()
		{
			this.StopIdle();
			if (this.elmFireTrap == null)
			{
				this.elmFireTrap = base.GetComponent<ElmFireTrap>();
			}
			ElmFireTrap elmFireTrap = this.elmFireTrap;
			elmFireTrap.OnChargeStart = (Core.SimpleEventParam)Delegate.Remove(elmFireTrap.OnChargeStart, new Core.SimpleEventParam(this.StartCharge));
			ElmFireTrap elmFireTrap2 = this.elmFireTrap;
			elmFireTrap2.OnLightningCast = (Core.SimpleEventParam)Delegate.Remove(elmFireTrap2.OnLightningCast, new Core.SimpleEventParam(this.PlayShot));
		}

		public void PlayShot(object targetPosition)
		{
			this.StopCharge();
			Vector3 position = ((Vector3)targetPosition + base.transform.position) / 2f;
			Core.Audio.PlayOneShot(this.ShotEvent, position);
		}

		public void AnimEvent_PlayIn()
		{
			this.PlayIn();
		}

		public void AnimEvent_PlayOut()
		{
			this.PlayOut();
		}

		public void PlayIn()
		{
			Core.Audio.PlayOneShot(this.InEvent, base.transform.position);
		}

		public void PlayOut()
		{
			this.StopIdle();
			Core.Audio.PlayOneShot(this.OutEvent, base.transform.position);
		}

		public void PlayIdle()
		{
			this.idleIsPlaying = true;
			this.idleEventInstance = default(EventInstance);
			Core.Audio.PlayEventNoCatalog(ref this.idleEventInstance, (this.elmFireTrap.linkType != ElmFireTrap.LinkType.Static) ? this.IdleEvent : this.IdlePulsesEvent, base.transform.position);
		}

		public void StopIdle()
		{
			this.idleIsPlaying = false;
			this.idleEventInstance.stop(0);
			this.idleEventInstance.release();
		}

		public void StartCharge(object chargingTime)
		{
			this.PlayCharge();
			this.totalChargingTime = (float)chargingTime;
			this.currentChargingTime = 0f;
		}

		public void PlayCharge()
		{
			this.StopIdle();
			this.chargeIsPlaying = true;
			this.chargeEventInstance = default(EventInstance);
			Core.Audio.PlayEventNoCatalog(ref this.chargeEventInstance, this.ChargeEvent, base.transform.position);
		}

		public void StopCharge()
		{
			this.chargeIsPlaying = false;
			this.chargeEventInstance.stop(0);
			this.chargeEventInstance.release();
		}

		public void UpdateChargeAudioParameter(float percentage)
		{
			try
			{
				ParameterInstance parameterInstance;
				this.chargeEventInstance.getParameter("Charge", ref parameterInstance);
				parameterInstance.setValue(percentage);
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		private bool IsTrapVisible()
		{
			return this.spriteRenderer.IsVisibleFrom(Camera.main);
		}

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string IdleEvent;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string ShotEvent;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string ChargeEvent;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string InEvent;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string OutEvent;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[EventRef]
		public string IdlePulsesEvent;

		private const string ChargeParam = "Charge";

		private ElmFireTrap elmFireTrap;

		private SpriteRenderer spriteRenderer;

		private EventInstance idleEventInstance;

		private EventInstance chargeEventInstance;

		public bool idleIsPlaying;

		public bool chargeIsPlaying;

		private float totalChargingTime;

		private float currentChargingTime;
	}
}
