using System;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Framework.Managers;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools.Level;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.MovingPlatforms
{
	[RequireComponent(typeof(Collider2D))]
	public class WaypointsMovingPlatform : MonoBehaviour, IActionable
	{
		private bool ShowGameobjectsToDeactivate
		{
			get
			{
				return this.DeactivatesGameobjectsWhenStepped || this.DeactivatesGameobjectsWhenRunning;
			}
		}

		public bool Enabled { get; private set; }

		private bool RunningTimeoutConsumed
		{
			get
			{
				return this.currentRunningTimeoutLapse <= 0f;
			}
		}

		private bool StartingTimeoutConsumed
		{
			get
			{
				return this.currentStartingTimeoutLapse <= 0f;
			}
		}

		public bool Locked { get; set; }

		private void Start()
		{
			this._spriteRenderer = base.GetComponent<SpriteRenderer>();
			this._platformCollider = base.GetComponent<BoxCollider2D>();
			if (this.StartRunning)
			{
				this._running = true;
			}
			this._currentSectionIndex = ((!Core.Events.GetFlag(this.OnChosenWaypoint)) ? 0 : this.IndexOfTheChosenWaypoint);
			this.SetSectionData();
			this.currentRunningTimeoutLapse = this.RunningTimeout;
			this.currentStartingTimeoutLapse = this.StartingTimeout;
			base.transform.position = this._origin;
		}

		private void Update()
		{
			if (!this._platformCollider.enabled || !this._running)
			{
				return;
			}
			if (this.DeactivatesGameobjectsWhenRunning && !this._tpoHasSteppedThis)
			{
				foreach (GameObject gameObject in this.GameobjectsToDeactivate)
				{
					if (gameObject)
					{
						gameObject.SetActive(!DOTween.IsTweening(base.transform, false));
					}
				}
			}
			this.currentStartingTimeoutLapse -= Time.deltaTime;
			if (!this.StartingTimeoutConsumed)
			{
				return;
			}
			if (Core.Logic.IsPaused)
			{
				if (this._horizontalTweener != null && TweenExtensions.IsPlaying(this._horizontalTweener) && this._verticalTweener != null && TweenExtensions.IsPlaying(this._verticalTweener))
				{
					TweenExtensions.Pause<Tweener>(this._horizontalTweener);
					TweenExtensions.Pause<Tweener>(this._verticalTweener);
					return;
				}
			}
			else if (this._horizontalTweener != null && !TweenExtensions.IsPlaying(this._horizontalTweener) && this._verticalTweener != null && !TweenExtensions.IsPlaying(this._verticalTweener))
			{
				TweenExtensions.TogglePause(this._horizontalTweener);
				TweenExtensions.TogglePause(this._verticalTweener);
			}
			float num = Vector2.Distance(base.transform.position, this._destination);
			float num2 = num / this._speed;
			this.currentRunningTimeoutLapse -= Time.deltaTime;
			if (!DOTween.IsTweening(base.transform, false) && this.RunningTimeoutConsumed && num > 0f)
			{
				if (!this._waypointsDetached)
				{
					this.DetachWaypoints();
				}
				this._horizontalTweener = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveX(base.transform, this._destination.x, num2, false), this._horizontalEase);
				this._verticalTweener = TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMoveY(base.transform, this._destination.y, num2, false), this._verticalEase);
				this.PlayMovementLoopFx(ref this._movementLoopFx);
			}
			if (num <= 0.01f)
			{
				if (this._currentSectionIndex == this.waypointsSections.Count - 1)
				{
					if (this.StopAtEnd)
					{
						this.Stop();
					}
					else
					{
						this._currentSectionIndex = 0;
						this.SetSectionData();
					}
				}
				else
				{
					this._currentSectionIndex++;
					this.SetSectionData();
				}
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				if (this._horizontalTweener != null && TweenExtensions.IsPlaying(this._horizontalTweener) && this._verticalTweener != null && TweenExtensions.IsPlaying(this._verticalTweener))
				{
					TweenExtensions.TogglePause(this._horizontalTweener);
					TweenExtensions.TogglePause(this._verticalTweener);
				}
			}
			else if (this._horizontalTweener != null && TweenExtensions.IsPlaying(this._horizontalTweener) && this._verticalTweener != null && TweenExtensions.IsPlaying(this._verticalTweener))
			{
				TweenExtensions.Pause<Tweener>(this._horizontalTweener);
				TweenExtensions.Pause<Tweener>(this._verticalTweener);
			}
		}

		private void DetachWaypoints()
		{
			this._waypointsDetached = true;
			foreach (WaypointsSection waypointsSection in this.waypointsSections)
			{
				waypointsSection.StartingPoint.parent = null;
				waypointsSection.EndingPoint.parent = null;
			}
		}

		private void AttachWaypoints()
		{
			this._waypointsDetached = false;
			foreach (WaypointsSection waypointsSection in this.waypointsSections)
			{
				waypointsSection.StartingPoint.parent = base.gameObject.transform;
				waypointsSection.EndingPoint.parent = base.gameObject.transform;
			}
		}

		private void SetSectionData()
		{
			this._speed = this.waypointsSections[this._currentSectionIndex].speed;
			this._horizontalEase = this.waypointsSections[this._currentSectionIndex].horizontalEase;
			this._verticalEase = this.waypointsSections[this._currentSectionIndex].verticalEase;
			Transform startingPoint = this.waypointsSections[this._currentSectionIndex].StartingPoint;
			Transform endingPoint = this.waypointsSections[this._currentSectionIndex].EndingPoint;
			this._origin = new Vector2(startingPoint.position.x, startingPoint.position.y);
			this._destination = new Vector2(endingPoint.position.x, endingPoint.position.y);
			if (this._currentSectionIndex > 0)
			{
				this.currentStartingTimeoutLapse = this.waypointsSections[this._currentSectionIndex - 1].waitTimeAtDestination;
			}
			else
			{
				this.currentStartingTimeoutLapse = this.waypointsSections[this.waypointsSections.Count - 1].waitTimeAtDestination;
			}
		}

		private void Stop()
		{
			this.currentRunningTimeoutLapse = this.RunningTimeout;
			this.currentStartingTimeoutLapse = this.StartingTimeout;
			base.transform.position = this._destination;
			this._destination = this._origin;
			this._origin = base.transform.position;
			this.StopMovementLoopFx(ref this._movementLoopFx);
			this.PlayMovementStopFx();
			TweenExtensions.Kill(this._horizontalTweener, false);
			TweenExtensions.Kill(this._verticalTweener, false);
			this._horizontalTweener = null;
			this._verticalTweener = null;
			if (this.OneWayOnly)
			{
				this._endArrived = true;
				this._running = false;
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.gameObject.CompareTag("Penitent"))
			{
				return;
			}
			Core.Logic.Penitent.FloorChecker.OnMovingPlatform = true;
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (!other.gameObject.CompareTag("Penitent"))
			{
				return;
			}
			if (!Core.Logic.Penitent.IsGrabbingCliffLede && !Core.Logic.Penitent.IsClimbingCliffLede)
			{
				this._tpoHasSteppedThis = true;
				if (this.DeactivatesGameobjectsWhenStepped)
				{
					foreach (GameObject gameObject in this.GameobjectsToDeactivate)
					{
						if (gameObject)
						{
							gameObject.SetActive(false);
						}
					}
				}
				if (this.StartRunningWhenFirstStepped && !this._running)
				{
					this.Use();
				}
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (!other.gameObject.CompareTag("Penitent"))
			{
				return;
			}
			Core.Logic.Penitent.FloorChecker.OnMovingPlatform = false;
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying && this.waypointsSections.Count > 0)
			{
				foreach (WaypointsSection waypointsSection in this.waypointsSections)
				{
					if (waypointsSection.StartingPoint && waypointsSection.EndingPoint)
					{
						Debug.DrawLine(waypointsSection.StartingPoint.position, waypointsSection.EndingPoint.position);
					}
				}
			}
		}

		private void FixFlagName(string flagName)
		{
			this.OnChosenWaypoint = flagName.Replace(' ', '_').ToUpper();
		}

		private IList<ValueDropdownItem<int>> ListCurrentIndexes()
		{
			ValueDropdownList<int> valueDropdownList = new ValueDropdownList<int>();
			for (int i = 0; i < this.waypointsSections.Count; i++)
			{
				valueDropdownList.Add(i);
			}
			return valueDropdownList;
		}

		private void SetMotionFlags()
		{
			bool b = base.transform.position == this.waypointsSections[this.IndexOfTheChosenWaypoint].StartingPoint.position;
			Core.Events.SetFlag(this.OnChosenWaypoint, b, false);
		}

		public void ResetPlatform()
		{
			this._running = false;
			if (DOTween.IsTweening(base.transform, false))
			{
				this.StopMovementLoopFx(ref this._movementLoopFx);
				DOTween.Kill(base.transform, false);
			}
			this._currentSectionIndex = ((!Core.Events.GetFlag(this.OnChosenWaypoint)) ? 0 : this.IndexOfTheChosenWaypoint);
			this.SetSectionData();
			this.currentRunningTimeoutLapse = this.RunningTimeout;
			this.currentStartingTimeoutLapse = this.StartingTimeout;
			base.transform.position = this._origin;
			if (this.OneWayOnly)
			{
				this._endArrived = false;
			}
			if (this._waypointsDetached)
			{
				this.AttachWaypoints();
			}
			this._tpoHasSteppedThis = false;
			foreach (GameObject gameObject in this.GameobjectsToDeactivate)
			{
				if (gameObject)
				{
					gameObject.SetActive(true);
				}
			}
		}

		public Vector2 GetOrigin()
		{
			return this._origin;
		}

		public Vector2 GetDestination()
		{
			return this._destination;
		}

		public void Use()
		{
			if (this.OneWayOnly && this._endArrived)
			{
				return;
			}
			if (!this._running)
			{
				this.SetSectionData();
				this.currentRunningTimeoutLapse = this.RunningTimeout;
				this.currentStartingTimeoutLapse = this.StartingTimeout;
			}
			this._running = !this._running;
			if (DOTween.IsTweening(base.transform, false))
			{
				this.StopMovementLoopFx(ref this._movementLoopFx);
				DOTween.Kill(base.transform, false);
			}
		}

		private void OnDestroy()
		{
			this.StopMovementLoopFx(ref this._movementLoopFx);
		}

		private void PlayMovementStopFx()
		{
			if (StringExtensions.IsNullOrWhitespace(this.MovingPlatformStop))
			{
				return;
			}
			if (this._spriteRenderer.isVisible)
			{
				Core.Audio.PlaySfx(this.MovingPlatformStop, 0f);
			}
		}

		private void PlayMovementLoopFx(ref EventInstance audioInstance)
		{
			if (StringExtensions.IsNullOrWhitespace(this.MovingPlatformLoop))
			{
				return;
			}
			if (this._movementLoopFx.isValid())
			{
				this._movementLoopFx.stop(1);
				this._movementLoopFx.release();
				this._movementLoopFx = default(EventInstance);
			}
			this._movementLoopFx = Core.Audio.CreateEvent(this.MovingPlatformLoop, base.transform.position);
			this._movementLoopFx.start();
		}

		private void StopMovementLoopFx(ref EventInstance audioInstance)
		{
			if (!this._movementLoopFx.isValid())
			{
				return;
			}
			this.PlayMovementStopFx();
			this._movementLoopFx.stop(1);
			this._movementLoopFx.release();
			this._movementLoopFx = default(EventInstance);
		}

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		private string MovingPlatformLoop;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		private string MovingPlatformStop;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private List<WaypointsSection> waypointsSections;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[PropertyRange(0.0, 60.0)]
		private float RunningTimeout;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[PropertyRange(0.0, 60.0)]
		private float StartingTimeout;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool StartRunning;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool StartRunningWhenFirstStepped;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool StopAtEnd;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool OneWayOnly;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool DeactivatesGameobjectsWhenStepped;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool DeactivatesGameobjectsWhenRunning;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("ShowGameobjectsToDeactivate", true)]
		private List<GameObject> GameobjectsToDeactivate;

		[SerializeField]
		[BoxGroup("Flag Settings", true, false, 0)]
		[OnValueChanged("FixFlagName", false)]
		private string OnChosenWaypoint;

		[SerializeField]
		[BoxGroup("Flag Settings", true, false, 0)]
		[ValueDropdown("ListCurrentIndexes")]
		private readonly int IndexOfTheChosenWaypoint;

		private const float DistanceThreshold = 0.01f;

		private SpriteRenderer _spriteRenderer;

		private Collider2D _platformCollider;

		private EventInstance _movementLoopFx;

		private Tweener _horizontalTweener;

		private Tweener _verticalTweener;

		private Vector3 _origin;

		private Vector3 _destination;

		private int _currentSectionIndex;

		private float _speed;

		private Ease _horizontalEase;

		private Ease _verticalEase;

		private bool _running;

		private bool _endArrived;

		private bool _waypointsDetached;

		private bool _tpoHasSteppedThis;

		private float currentRunningTimeoutLapse;

		private float currentStartingTimeoutLapse;
	}
}
