using System;
using System.Threading;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Framework.Audio
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Event SnapShot")]
	public class StudioEventSnapshot : MonoBehaviour
	{
		private void Start()
		{
			RuntimeUtils.EnforceLibraryOrder();
			if (this.Preload)
			{
				this.Lookup();
				this.eventDescription.loadSampleData();
				RuntimeManager.StudioSystem.update();
				LOADING_STATE loading_STATE;
				this.eventDescription.getSampleLoadingState(out loading_STATE);
				while (loading_STATE == LOADING_STATE.LOADING)
				{
					Thread.Sleep(1);
					this.eventDescription.getSampleLoadingState(out loading_STATE);
				}
			}
			this.HandleGameEvent(EmitterGameEvent.ObjectStart);
		}

		private void OnApplicationQuit()
		{
			this.isQuitting = true;
		}

		private void OnDestroy()
		{
			if (!this.isQuitting)
			{
				this.HandleGameEvent(EmitterGameEvent.ObjectDestroy);
				if (this.instance.isValid())
				{
					RuntimeManager.DetachInstanceFromGameObject(this.instance);
				}
				if (this.Preload)
				{
					this.eventDescription.unloadSampleData();
				}
			}
			this.Stop();
		}

		private void OnEnable()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectEnable);
		}

		private void OnDisable()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectDisable);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerEnter);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerExit);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerEnter2D);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerExit2D);
			}
		}

		private void OnCollisionEnter()
		{
			this.HandleGameEvent(EmitterGameEvent.CollisionEnter);
		}

		private void OnCollisionExit()
		{
			this.HandleGameEvent(EmitterGameEvent.CollisionExit);
		}

		private void OnCollisionEnter2D()
		{
			this.HandleGameEvent(EmitterGameEvent.CollisionEnter2D);
		}

		private void OnCollisionExit2D()
		{
			this.HandleGameEvent(EmitterGameEvent.CollisionExit2D);
		}

		private void HandleGameEvent(EmitterGameEvent gameEvent)
		{
			if (this.PlayEvent == gameEvent)
			{
				this.Play();
			}
			if (this.StopEvent == gameEvent)
			{
				this.Stop();
			}
		}

		private void Lookup()
		{
			this.eventDescription = RuntimeManager.GetEventDescription(this.Event);
		}

		public void Play()
		{
			if (this.TriggerOnce && this.hasTriggered)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.Event))
			{
				return;
			}
			if (!this.eventDescription.isValid())
			{
				this.Lookup();
			}
			bool flag = false;
			if (!this.Event.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
			{
				this.eventDescription.isOneshot(out flag);
			}
			bool flag2;
			this.eventDescription.is3D(out flag2);
			if (!this.instance.isValid())
			{
				this.instance = default(EventInstance);
			}
			if (flag && this.instance.isValid())
			{
				this.instance.release();
				this.instance = default(EventInstance);
			}
			if (this.instance.isValid())
			{
				this.eventDescription.createInstance(out this.instance);
				if (flag2)
				{
					Rigidbody component = base.GetComponent<Rigidbody>();
					Rigidbody2D component2 = base.GetComponent<Rigidbody2D>();
					Transform component3 = base.GetComponent<Transform>();
					if (component)
					{
						this.instance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, component));
						RuntimeManager.AttachInstanceToGameObject(this.instance, component3, component);
					}
					else
					{
						this.instance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, component2));
						RuntimeManager.AttachInstanceToGameObject(this.instance, component3, component2);
					}
				}
			}
			foreach (ParamRef paramRef in this.Params)
			{
				this.instance.setParameterValue(paramRef.Name, paramRef.Value);
			}
			if (flag2 && this.OverrideAttenuation)
			{
				this.instance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, this.OverrideMinDistance);
				this.instance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, this.OverrideMaxDistance);
			}
			this.instance.start();
			this.hasTriggered = true;
		}

		public void Stop()
		{
			if (this.instance.isValid())
			{
				this.instance.stop((!this.AllowFadeout) ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				this.instance.release();
				this.instance = default(EventInstance);
			}
		}

		public void SetParameter(string name, float value)
		{
			if (this.instance.isValid())
			{
				this.instance.setParameterValue(name, value);
			}
		}

		public bool IsPlaying()
		{
			if (this.instance.isValid())
			{
				PLAYBACK_STATE playback_STATE;
				this.instance.getPlaybackState(out playback_STATE);
				return playback_STATE != PLAYBACK_STATE.STOPPED;
			}
			return false;
		}

		public bool AllowFadeout = true;

		public string CollisionTag = string.Empty;

		[EventRef]
		public string Event = string.Empty;

		private EventDescription eventDescription;

		private bool hasTriggered;

		private EventInstance instance;

		private bool isQuitting;

		public bool OverrideAttenuation;

		public float OverrideMaxDistance = -1f;

		public float OverrideMinDistance = -1f;

		public ParamRef[] Params = new ParamRef[0];

		public EmitterGameEvent PlayEvent;

		public bool Preload;

		public EmitterGameEvent StopEvent;

		public bool TriggerOnce;
	}
}
