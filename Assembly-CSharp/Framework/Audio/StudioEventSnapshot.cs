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
				this.eventDescription.getSampleLoadingState(ref loading_STATE);
				while (loading_STATE == 2)
				{
					Thread.Sleep(1);
					this.eventDescription.getSampleLoadingState(ref loading_STATE);
				}
			}
			this.HandleGameEvent(1);
		}

		private void OnApplicationQuit()
		{
			this.isQuitting = true;
		}

		private void OnDestroy()
		{
			if (!this.isQuitting)
			{
				this.HandleGameEvent(2);
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
			this.HandleGameEvent(11);
		}

		private void OnDisable()
		{
			this.HandleGameEvent(12);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(3);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(4);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(5);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(6);
			}
		}

		private void OnCollisionEnter()
		{
			this.HandleGameEvent(7);
		}

		private void OnCollisionExit()
		{
			this.HandleGameEvent(8);
		}

		private void OnCollisionEnter2D()
		{
			this.HandleGameEvent(9);
		}

		private void OnCollisionExit2D()
		{
			this.HandleGameEvent(10);
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
				this.eventDescription.isOneshot(ref flag);
			}
			bool flag2;
			this.eventDescription.is3D(ref flag2);
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
				this.eventDescription.createInstance(ref this.instance);
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
				this.instance.setProperty(3, this.OverrideMinDistance);
				this.instance.setProperty(4, this.OverrideMaxDistance);
			}
			this.instance.start();
			this.hasTriggered = true;
		}

		public void Stop()
		{
			if (this.instance.isValid())
			{
				this.instance.stop((!this.AllowFadeout) ? 1 : 0);
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
				this.instance.getPlaybackState(ref playback_STATE);
				return playback_STATE != 2;
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
