using System;
using System.Collections.Generic;
using FMOD.Studio;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Entities.Audio
{
	public class EntityAudio : MonoBehaviour
	{
		protected virtual void OnWake()
		{
		}

		private void Awake()
		{
			this.Owner = base.GetComponentInParent<Entity>();
			this.OnWake();
		}

		protected virtual void OnStart()
		{
		}

		private void Start()
		{
			this.AudioManager = Core.Audio;
			this.OnStart();
		}

		protected virtual void OnUpdate()
		{
		}

		private void Update()
		{
			this.OnUpdate();
		}

		public bool Mute
		{
			get
			{
				return this.mute;
			}
			set
			{
				this.mute = value;
				if (this.Owner)
				{
					this.Owner.Mute = value;
				}
			}
		}

		protected void SetDeathHitParam(EventInstance eventInstance)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Death", out parameterInstance);
				if (parameterInstance.isValid())
				{
					parameterInstance.setValue(this.DeathValue);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + ex.StackTrace);
				throw;
			}
		}

		private void SetWeaponHitMaterial(EventInstance eventInstance)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Dirt", out parameterInstance);
				if (parameterInstance.isValid())
				{
					parameterInstance.setValue(this.DirtValue);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message + ex.StackTrace);
			}
		}

		public void SetWallMaterialParams(EventInstance eventInstance)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Wood", out parameterInstance);
				parameterInstance.setValue(this.WallWoodValue);
				ParameterInstance parameterInstance2;
				eventInstance.getParameter("Stone", out parameterInstance2);
				parameterInstance2.setValue(this.WallStoneValue);
			}
			catch
			{
			}
		}

		private void SetFloorMaterialParams(EventInstance eventInstance)
		{
			try
			{
				ParameterInstance parameterInstance;
				eventInstance.getParameter("Dirt", out parameterInstance);
				parameterInstance.setValue(this.DirtValue);
				ParameterInstance parameterInstance2;
				eventInstance.getParameter("Water", out parameterInstance2);
				parameterInstance2.setValue(this.WaterValue);
				ParameterInstance parameterInstance3;
				eventInstance.getParameter("Snow", out parameterInstance3);
				parameterInstance3.setValue(this.SnowValue);
				ParameterInstance parameterInstance4;
				eventInstance.getParameter("Stone", out parameterInstance4);
				parameterInstance4.setValue(this.StoneValue);
				ParameterInstance parameterInstance5;
				eventInstance.getParameter("Wood", out parameterInstance5);
				parameterInstance5.setValue(this.WoodValue);
				ParameterInstance parameterInstance6;
				eventInstance.getParameter("Marble", out parameterInstance6);
				parameterInstance6.setValue(this.MarbleValue);
				ParameterInstance parameterInstance7;
				eventInstance.getParameter("Flesh", out parameterInstance7);
				parameterInstance7.setValue(this.FleshValue);
				ParameterInstance parameterInstance8;
				eventInstance.getParameter("Metal", out parameterInstance8);
				parameterInstance8.setValue(this.MetalValue);
				ParameterInstance parameterInstance9;
				eventInstance.getParameter("Mud", out parameterInstance9);
				parameterInstance9.setValue(this.MudValue);
				ParameterInstance parameterInstance10;
				eventInstance.getParameter("SecretFloor", out parameterInstance10);
				parameterInstance10.setValue(this.SecretValue);
				ParameterInstance parameterInstance11;
				eventInstance.getParameter("Grass", out parameterInstance11);
				parameterInstance11.setValue(this.GrassValue);
				ParameterInstance parameterInstance12;
				eventInstance.getParameter("Demake", out parameterInstance12);
				parameterInstance12.setValue(this.DemakeValue);
				ParameterInstance parameterInstance13;
				eventInstance.getParameter("Palio", out parameterInstance13);
				parameterInstance13.setValue(this.PalioValue);
				ParameterInstance parameterInstance14;
				eventInstance.getParameter("Snake", out parameterInstance14);
				parameterInstance14.setValue(this.SnakeValue);
			}
			catch
			{
			}
		}

		public void PlayOneShotEvent(string eventKey, EntityAudio.FxSoundCategory fxSoundCategory)
		{
			if (this.AudioManager == null || this.mute)
			{
				return;
			}
			EventInstance eventInstance = this.AudioManager.CreateCatalogEvent(eventKey, default(Vector3));
			if (!eventInstance.isValid())
			{
				Debug.LogError(string.Format("ERROR: Couldn't find catalog sound event called <{0}>", eventKey));
				return;
			}
			eventInstance.setCallback(this.SetPanning(eventInstance), EVENT_CALLBACK_TYPE.CREATED);
			switch (fxSoundCategory)
			{
			case EntityAudio.FxSoundCategory.Motion:
				this.SetFloorMaterialParams(eventInstance);
				break;
			case EntityAudio.FxSoundCategory.Attack:
				this.SetWeaponHitMaterial(eventInstance);
				break;
			case EntityAudio.FxSoundCategory.Damage:
				this.SetDeathHitParam(eventInstance);
				break;
			case EntityAudio.FxSoundCategory.Climb:
				this.SetWallMaterialParams(eventInstance);
				break;
			}
			eventInstance.start();
			eventInstance.release();
		}

		public FMODAudioManager AudioManager { get; protected set; }

		protected void ReleaseAudioEvents()
		{
			if (this.EventInstances == null)
			{
				return;
			}
			for (int i = 0; i < this.EventInstances.Count; i++)
			{
				if (this.EventInstances[i].isValid())
				{
					this.EventInstances[i].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
					this.EventInstances[i].release();
				}
			}
			this.EventInstances.Clear();
		}

		public void PlayEvent(ref EventInstance eventInstance, string eventKey, bool checkSpriteRendererVisible = true)
		{
			if (checkSpriteRendererVisible && !this.Owner.SpriteRenderer.isVisible)
			{
				return;
			}
			if (this.mute)
			{
				return;
			}
			if (this.reuseEventInstances)
			{
				this.PlayEventReuse(ref eventInstance, eventKey);
			}
			else
			{
				if (eventInstance.isValid())
				{
					return;
				}
				eventInstance = this.AudioManager.CreateCatalogEvent(eventKey, default(Vector3));
				if (eventInstance.isValid())
				{
					eventInstance.start();
				}
			}
		}

		private void PlayEventReuse(ref EventInstance eventInstance, string eventKey)
		{
			if (!eventInstance.isValid())
			{
				eventInstance = this.AudioManager.CreateCatalogEvent(eventKey, default(Vector3));
			}
			if (eventInstance.isValid())
			{
				eventInstance.start();
			}
		}

		public void UpdateEvent(ref EventInstance eventInstance)
		{
			if (!this.Owner.SpriteRenderer.isVisible || this.mute)
			{
				return;
			}
			if (eventInstance.isValid())
			{
				this.SetPanning(eventInstance);
			}
		}

		public void StopEvent(ref EventInstance eventInstance)
		{
			if (!eventInstance.isValid() || this.mute)
			{
				return;
			}
			eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			eventInstance.release();
			eventInstance = default(EventInstance);
		}

		protected EVENT_CALLBACK SetPanning(EventInstance e)
		{
			if (!e.isValid())
			{
				return null;
			}
			ParameterInstance parameterInstance;
			e.getParameter("Panning", out parameterInstance);
			if (parameterInstance.isValid() && this.Owner)
			{
				float panningValueByPosition = FMODAudioManager.GetPanningValueByPosition(this.Owner.transform.position);
				parameterInstance.setValue(panningValueByPosition);
			}
			return null;
		}

		public static EVENT_CALLBACK SetPanning(EventInstance e, Vector3 pos)
		{
			ParameterInstance parameterInstance;
			e.getParameter("Panning", out parameterInstance);
			if (parameterInstance.isValid())
			{
				float panningValueByPosition = FMODAudioManager.GetPanningValueByPosition(pos);
				parameterInstance.setValue(panningValueByPosition);
			}
			return null;
		}

		[Tooltip("This was added on DLC3 to fix issue #83018. Messing with the audio logic at this point is dangerous so we prefer to use the new behaviour just for the specific case we know instead of risking on breaking the audio system elsewhere - DEG")]
		public bool reuseEventInstances;

		protected bool InitilizationError;

		protected Entity Owner;

		private bool mute;

		protected float DeathValue;

		protected float WallWoodValue;

		protected float WallStoneValue;

		protected float DirtValue;

		protected float SnowValue;

		protected float WaterValue;

		protected float StoneValue;

		protected float FleshValue;

		protected float WoodValue;

		protected float MarbleValue;

		protected float MetalValue;

		protected float MudValue;

		protected float SecretValue;

		protected float GrassValue;

		protected float DemakeValue;

		protected float PalioValue;

		protected float SnakeValue;

		public const string LabelParamDirt = "Dirt";

		public const string LabelParamSnow = "Snow";

		public const string LabelParamStone = "Stone";

		public const string LabelParamWood = "Wood";

		public const string LabelParamMarble = "Marble";

		public const string LabelParamMetal = "Metal";

		public const string LabelParamMud = "Mud";

		public const string LabelParamWater = "Water";

		public const string LabelParamSecret = "SecretFloor";

		public const string LabelParamGrass = "Grass";

		public const string LabelParamDemake = "Demake";

		public const string LabelParamPalio = "Palio";

		public const string LabelParamSnake = "Snake";

		public const string LabelParamFlesh = "Flesh";

		public const string LabelDeath = "Death";

		public const string LabelPanning = "Panning";

		[SerializeField]
		protected GameObject FloorCollider;

		protected ICollisionEmitter FloorSensorEmitter;

		[SerializeField]
		protected GameObject WeaponCollider;

		protected ICollisionEmitter WeaponSensorEmitter;

		public LayerMask FloorLayerMask;

		public LayerMask WeaponLayerMask;

		protected List<EventInstance> EventInstances;

		public enum FxSoundCategory
		{
			Motion,
			Attack,
			Damage,
			Climb
		}
	}
}
