using System;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using UnityEngine;

namespace Framework.Audio
{
	[RequireComponent(typeof(BoxCollider2D))]
	public class BackgroundAudio : MonoBehaviour
	{
		public float Volume
		{
			get
			{
				this._volumeParameter.getValue(out this.volume);
				return this.volume;
			}
			set
			{
				if (!this._volumeParameter.isValid())
				{
					return;
				}
				float value2 = Mathf.Clamp01(value);
				this._volumeParameter.setValue(value2);
			}
		}

		private void Start()
		{
			this._eventInstance = Core.Audio.CreateEvent(this.audio, default(Vector3));
			if (!this._eventInstance.isValid())
			{
				return;
			}
			try
			{
				this._eventInstance.getParameter("Volume", out this._volumeParameter);
				this.volume = 1f;
				this._volumeParameter.setValue(this.volume);
				this._eventInstance.start();
				this.Volume = 0f;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message + ex.StackTrace);
			}
		}

		private void Update()
		{
			if (this._fading)
			{
				this._interpolatingTime += Time.deltaTime * this.FadingSpeed;
				this.Volume = Mathf.Lerp(this.Volume, this._desiredVolume, this._interpolatingTime);
			}
			if (Mathf.Approximately(this.Volume, this._desiredVolume) && this._fading)
			{
				this._fading = false;
			}
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if ((this.TargetLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this._desiredVolume = 1f;
			if (Mathf.Approximately(this.Volume, this._desiredVolume))
			{
				return;
			}
			if (!this._fading)
			{
				this._interpolatingTime = 0f;
				this._fading = true;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if ((this.TargetLayer.value & 1 << other.gameObject.layer) <= 0)
			{
				return;
			}
			this._desiredVolume = 0f;
			if (!this._fading)
			{
				this._interpolatingTime = 0f;
				this._fading = true;
			}
		}

		private void OnDestroy()
		{
			if (!this._eventInstance.isValid())
			{
				return;
			}
			this._eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this._eventInstance.release();
		}

		public const string VolumeParamLabel = "Volume";

		private float _desiredVolume;

		private EventInstance _eventInstance;

		private bool _fading;

		private float _interpolatingTime;

		private ParameterInstance _volumeParameter;

		[SerializeField]
		[EventRef]
		private string audio;

		[Range(0f, 1f)]
		public float FadingSpeed = 0.75f;

		public LayerMask TargetLayer;

		protected float volume;
	}
}
