using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using Tools.Level.Actionables;
using UnityEngine;

namespace Tools.Items
{
	public class IncorruptHandBell : ObjectEffect
	{
		private float UpdateLapse { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			HiddenArea.OnUse = (Core.SimpleEvent)Delegate.Combine(HiddenArea.OnUse, new Core.SimpleEvent(this.OnUse));
			LevelManager.OnBeforeLevelLoad += this.OnBeforeLevelLoad;
		}

		private void OnBeforeLevelLoad(Level oldlevel, Level newlevel)
		{
			this.InitialWarningDelayCounter = 0f;
			if (this.secrets != null && this.secrets.Count > 0)
			{
				this.secrets.Clear();
			}
		}

		private void OnUse()
		{
			if (this.secrets == null)
			{
				return;
			}
			if (this.secrets.Count > 0)
			{
				if (this.closestTarget == null)
				{
					return;
				}
				this.secrets.Remove(this.closestTarget);
				this.closestTarget = null;
			}
		}

		protected override bool OnApplyEffect()
		{
			this.CreateAudioInstance();
			this.secrets = Object.FindObjectsOfType<BreakableWall>().ToList<BreakableWall>().FindAll((BreakableWall x) => !x.Destroyed);
			this.UpdateLapse = 3f;
			return true;
		}

		protected override void OnRemoveEffect()
		{
			this.instance.stop(0);
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			float deltaTime = Time.deltaTime;
			if (this.secrets != null && this.secrets.Count > 0)
			{
				this.InitialWarningDelayCounter += deltaTime;
			}
			this.UpdateLapse += deltaTime;
			if (this.UpdateLapse <= 3f)
			{
				return;
			}
			this.UpdateLapse = 0f;
			this.TimedUpdate();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			HiddenArea.OnUse = (Core.SimpleEvent)Delegate.Remove(HiddenArea.OnUse, new Core.SimpleEvent(this.OnUse));
			LevelManager.OnBeforeLevelLoad -= this.OnBeforeLevelLoad;
			if (this.instance.isValid())
			{
				this.instance.release();
			}
		}

		private void TimedUpdate()
		{
			this.UpdateClosestTarget();
			this.UpdateIntensity();
			this.UpdateItemLogic();
		}

		private void UpdateItemLogic()
		{
			if (this.InitialWarningDelayCounter < 3f)
			{
				return;
			}
			if (this.SecretsInRange && !Core.Logic.Penitent.Status.Dead && this.closestTarget != null)
			{
				this.instance.stop(0);
				this.instance.start();
				Core.UI.Glow.color = this.haloColor;
				Core.UI.Glow.Show(this.haloDuration, 1);
			}
		}

		private void UpdateClosestTarget()
		{
			Penitent penitent = Core.Logic.Penitent;
			if (this.secrets == null || this.secrets.Count == 0 || penitent == null)
			{
				return;
			}
			Vector3 position = penitent.transform.position;
			foreach (BreakableWall breakableWall in this.secrets)
			{
				if (this.closestTarget == null)
				{
					this.closestTarget = breakableWall;
				}
				float num = Vector2.Distance(breakableWall.transform.position, position);
				float num2 = Vector2.Distance(this.closestTarget.transform.position, position);
				if (num < num2)
				{
					this.closestTarget = breakableWall;
				}
			}
		}

		private void UpdateIntensity()
		{
			this.audioIntensity = Mathf.CeilToInt(5f - this.NormalizedProgress * 5f);
			this.instance.setParameterValue("Distance", (float)this.audioIntensity);
			int num = this.audioIntensity - 1;
			if (num >= 0 && num < this.intensityConfiguration.Length)
			{
				this.haloColor = new Color(1f, 1f, 1f, this.intensityConfiguration[num].haloTransparency);
				this.haloDuration = this.intensityConfiguration[num].haloDuration;
			}
		}

		private bool SecretsInRange
		{
			get
			{
				return this.closestTarget != null && this.ClosestSecretDistance <= this.interactionDistance;
			}
		}

		private float ClosestSecretDistance
		{
			get
			{
				if (this.closestTarget != null)
				{
					return Vector2.Distance(this.closestTarget.transform.position, Core.Logic.Penitent.transform.position);
				}
				return -1f;
			}
		}

		private float NormalizedProgress
		{
			get
			{
				return this.ClosestSecretDistance / this.interactionDistance;
			}
		}

		public void CreateAudioInstance()
		{
			if (!this.instance.isValid())
			{
				this.instance = Core.Audio.CreateEvent(this.audioId, default(Vector3));
			}
		}

		private PLAYBACK_STATE PlaybackState
		{
			get
			{
				PLAYBACK_STATE result;
				this.instance.getPlaybackState(ref result);
				return result;
			}
		}

		private const float UPDATE_RATE = 3f;

		private const float InitialWarningDelay = 3f;

		private float InitialWarningDelayCounter;

		private const float INTENSITY_LEVELS = 5f;

		private const string AUDIO_INTENSITY_PARAM = "Distance";

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[EventRef]
		private string audioId;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float interactionDistance;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private IncorruptHandConfig[] intensityConfiguration = new IncorruptHandConfig[5];

		[SerializeField]
		[BoxGroup("Debug Information", true, false, 0)]
		[ReadOnly]
		private int audioIntensity;

		[SerializeField]
		[BoxGroup("Debug Information", true, false, 0)]
		[ReadOnly]
		private Color haloColor;

		[SerializeField]
		[BoxGroup("Debug Information", true, false, 0)]
		[ReadOnly]
		private float haloDuration;

		private List<BreakableWall> secrets;

		private BreakableWall closestTarget;

		private EventInstance instance;
	}
}
