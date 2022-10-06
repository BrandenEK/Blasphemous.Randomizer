using System;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.Elevator
{
	public class Elevator : MonoBehaviour
	{
		public bool IsRunning { get; private set; }

		private void Awake()
		{
			SpawnManager.OnPlayerSpawn += this.OnPlayerSpawn;
		}

		private void OnPlayerSpawn(Penitent penitent)
		{
			this.SetPlatformsPositions();
			this.SetFlaggedPosition();
			this.IsRunning = false;
		}

		public void MoveToFloor(int order)
		{
			if (this._currentFloor.Order == order || this.IsRunning)
			{
				return;
			}
			Elevator.Floor wishFloor = this.GetWishFloor(order);
			this._wishFloor = wishFloor;
			TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.OnStart<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, this._floorPositions[this._wishFloor.Order], this.GetDisplacementLapse(this._wishFloor), false), this.MoveEase), new TweenCallback(this.OnMoveStart)), new TweenCallback(this.OnMoveStop));
		}

		private void OnMoveStart()
		{
			this.IsRunning = true;
			this.PlayMotionSound();
		}

		private void OnMoveStop()
		{
			this.IsRunning = false;
			this._currentFloor = this._wishFloor;
			this.SetWishFloorFlag(this._wishFloor);
			this.StopMotionSound();
		}

		private Elevator.Floor GetWishFloor(int order)
		{
			Elevator.Floor result = this._currentFloor;
			foreach (Elevator.Floor floor in this.Floors)
			{
				if (floor.Order == order)
				{
					result = floor;
					break;
				}
			}
			return result;
		}

		private float GetDisplacementLapse(Elevator.Floor wishFloor)
		{
			float num = Vector2.Distance(this._currentFloor.Platform.position, wishFloor.Platform.position);
			return num / this.MovingSpeed;
		}

		private void SetPlatformsPositions()
		{
			foreach (Elevator.Floor floor in this.Floors)
			{
				Vector2 value;
				value..ctor(floor.Platform.position.x, floor.Platform.position.y);
				this._floorPositions.Add(floor.Order, value);
			}
		}

		private void SetWishFloorFlag(Elevator.Floor floor)
		{
			foreach (Elevator.Floor floor2 in this.Floors)
			{
				Core.Events.SetFlag(floor2.OnDestination, false, false);
			}
			Core.Events.SetFlag(floor.OnDestination, true, false);
		}

		private Elevator.Floor GetFlaggedFloor()
		{
			Elevator.Floor result = this.GetWishFloor(0);
			foreach (Elevator.Floor floor in this.Floors)
			{
				if (Core.Events.GetFlag(floor.OnDestination))
				{
					result = floor;
				}
			}
			return result;
		}

		private void SetFlaggedPosition()
		{
			Elevator.Floor flaggedFloor = this.GetFlaggedFloor();
			this._currentFloor = flaggedFloor;
			base.transform.position = this._floorPositions[this._currentFloor.Order];
		}

		private void PlayMotionSound()
		{
			this.PlayElevatorActivation(this.StartMotionAudioFx);
			if (this._elevatorMotionAudio.isValid())
			{
				return;
			}
			Core.Audio.PlayEventNoCatalog(ref this._elevatorMotionAudio, this.MotionAudioFx, default(Vector3));
		}

		private void StopMotionSound()
		{
			this.PlayElevatorActivation(this.StopMotionAudioFx);
			if (!this._elevatorMotionAudio.isValid())
			{
				return;
			}
			this._elevatorMotionAudio.stop(0);
			this._elevatorMotionAudio.release();
			this._elevatorMotionAudio = default(EventInstance);
		}

		private void PlayElevatorActivation(string activationEvent)
		{
			if (string.IsNullOrEmpty(activationEvent))
			{
				return;
			}
			if (Core.Logic.Penitent && Core.Logic.Penitent.IsVisible())
			{
				Core.Audio.PlaySfx(activationEvent, 0f);
			}
		}

		private void OnDestroy()
		{
			this.StopMotionSound();
			SpawnManager.OnPlayerSpawn -= this.OnPlayerSpawn;
		}

		[SerializeField]
		public Elevator.Floor[] Floors;

		[FoldoutGroup("Elevator Settings", 0)]
		[Range(1f, 100f)]
		public float MovingSpeed = 5f;

		[FoldoutGroup("Elevator Settings", 0)]
		public AnimationCurve MoveEase;

		[FoldoutGroup("Audio", 0)]
		[EventRef]
		public string MotionAudioFx;

		[FoldoutGroup("Audio", 0)]
		[EventRef]
		public string StartMotionAudioFx;

		[FoldoutGroup("Audio", 0)]
		[EventRef]
		public string StopMotionAudioFx;

		private EventInstance _elevatorMotionAudio;

		private Elevator.Floor _currentFloor;

		private Elevator.Floor _wishFloor;

		private Dictionary<int, Vector2> _floorPositions = new Dictionary<int, Vector2>();

		[Serializable]
		public struct Floor
		{
			public int Order;

			public Transform Platform;

			public string OnDestination;
		}
	}
}
