using System;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools.Gameplay;
using Tools.Level;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.MovingPlatforms
{
	[RequireComponent(typeof(Collider2D))]
	public class StraightMovingPlatform : PersistentObject, IActionable
	{
		public bool Enabled { get; private set; }

		public bool IsRunning
		{
			get
			{
				return this._running;
			}
		}

		private bool TimeoutConsumed
		{
			get
			{
				return this.currentRunningTimeoutLapse <= 0f;
			}
		}

		private void Start()
		{
			this.currentRunningTimeoutLapse = this.RunningTimeout;
			MovingPlatformDestination componentInChildren = base.GetComponentInChildren<MovingPlatformDestination>();
			Vector3 position = componentInChildren.transform.position;
			this._spriteRenderer = base.GetComponent<SpriteRenderer>();
			this._destination = (this.DestinationPosition = new Vector2(position.x, position.y));
			this._origin = (this.OriginPosition = new Vector2(base.transform.position.x, base.transform.position.y));
			this._platformCollider = base.GetComponent<BoxCollider2D>();
			if (this.StartRunning)
			{
				this._running = true;
			}
			if (Core.Events.GetFlag(this.OnDestination))
			{
				base.transform.position = this.DestinationPosition;
				this.SwapTravelPoints(ref this._origin, ref this._destination);
			}
			if (!this.IsSafePosition && base.gameObject.GetComponent<NoSafePosition>() == null)
			{
				base.gameObject.AddComponent<NoSafePosition>();
			}
		}

		public Vector3 GetVelocity()
		{
			return this._velocity;
		}

		private void Update()
		{
			if (!this._platformCollider.enabled || !this._running)
			{
				return;
			}
			float num = Vector2.Distance(base.transform.position, this._destination);
			this.currentRunningTimeoutLapse -= Time.deltaTime;
			if (num > 0f && !DOTween.IsTweening(base.transform, false) && this.TimeoutConsumed)
			{
				this.tweener = TweenSettingsExtensions.SetUpdate<Tweener>(TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOMove(base.transform, this._destination, num / this.Speed, false), 4), 0, false);
				this.PlayMovementLoopFx(ref this._movementLoopFx);
			}
			if (num <= 0.01f)
			{
				this._journeyCount++;
				this.currentRunningTimeoutLapse = this.RunningTimeout;
				base.transform.position = this._destination;
				this._destination = this._origin;
				this._origin = base.transform.position;
				this.StopMovementLoopFx(ref this._movementLoopFx);
				this.PlayMovementStopFx();
				TweenExtensions.Kill(this.tweener, false);
				this.tweener = null;
			}
			this.SetMotionConstraints();
			if (this.DeactivatesGameobjectsWhenRunning && !this._gosDeactivated)
			{
				this._gosDeactivated = true;
				this.GameobjectsToDeactivate.ForEach(delegate(GameObject x)
				{
					x.SetActive(false);
				});
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.CompareTag("Penitent"))
			{
				Core.Logic.Penitent.FloorChecker.OnMovingPlatform = true;
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.CompareTag("Penitent"))
			{
				Core.Logic.Penitent.FloorChecker.OnMovingPlatform = false;
			}
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying && this.Destination != null)
			{
				Debug.DrawLine(base.transform.position, this.Destination.position);
			}
		}

		public void DestinyFlagName(string flagName)
		{
			this.OnDestination = flagName.Replace(' ', '_').ToUpper();
		}

		private void SetMotionConstraints()
		{
			if (!this.OneWay || this._journeyCount <= 0)
			{
				return;
			}
			if (base.transform.position == this.DestinationPosition)
			{
				this._running = false;
			}
			if (base.transform.position == this.OriginPosition)
			{
				this._running = false;
			}
			this.SetMotionFlags();
		}

		private void SetMotionFlags()
		{
			bool b = base.transform.position == this.DestinationPosition;
			Core.Events.SetFlag(this.OnDestination, b, false);
		}

		public void ResetJourneyCounter()
		{
			if (this._journeyCount > 0)
			{
				this._journeyCount = 0;
			}
		}

		private void SwapTravelPoints(ref Vector3 origin, ref Vector3 destination)
		{
			Vector3 vector = origin;
			origin = destination;
			destination = vector;
		}

		public void Use()
		{
			this.ResetJourneyCounter();
			this._running = !this._running;
			if (DOTween.IsTweening(base.transform, false))
			{
				this.StopMovementLoopFx(ref this._movementLoopFx);
				DOTween.Kill(base.transform, false);
			}
		}

		public void Reset()
		{
			this.ResetJourneyCounter();
			this._running = false;
			if (DOTween.IsTweening(base.transform, false))
			{
				this.StopMovementLoopFx(ref this._movementLoopFx);
				DOTween.Kill(base.transform, false);
			}
			this.currentRunningTimeoutLapse = this.RunningTimeout;
			MovingPlatformDestination componentInChildren = base.GetComponentInChildren<MovingPlatformDestination>(true);
			Vector3 position = componentInChildren.transform.position;
			this._destination = this.DestinationPosition;
			this._origin = this.OriginPosition;
			if (Core.Events.GetFlag(this.OnDestination))
			{
				base.transform.position = this.DestinationPosition;
				this.SwapTravelPoints(ref this._origin, ref this._destination);
			}
			else
			{
				base.transform.position = this.OriginPosition;
			}
		}

		public bool Locked { get; set; }

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			if (!this.persistState)
			{
				return;
			}
			BasicPersistence basicPersistence = (BasicPersistence)data;
			this._running = basicPersistence.triggered;
			if (Core.Events.GetFlag(this.OnDestination))
			{
				this._running = false;
			}
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			if (!this.persistState)
			{
				return null;
			}
			BasicPersistence basicPersistence = base.CreatePersistentData<BasicPersistence>();
			basicPersistence.triggered = this._running;
			return basicPersistence;
		}

		private void OnDestroy()
		{
			if (this._running)
			{
				Core.Events.SetFlag(this.OnDestination, true, false);
			}
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
				this._movementLoopFx.stop(0);
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
		[BoxGroup("Design Settings", true, false, 0)]
		protected bool IsSafePosition;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		protected string MovingPlatformLoop;

		[SerializeField]
		[FoldoutGroup("Audio", 0)]
		[EventRef]
		protected string MovingPlatformStop;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool persistState;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private Transform Destination;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[PropertyRange(0.0, 60.0)]
		private float RunningTimeout;

		private float currentRunningTimeoutLapse;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		public float Speed = 2f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private float ChasingElongation = 0.5f;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool StartRunning;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		private bool DeactivatesGameobjectsWhenRunning;

		[SerializeField]
		[BoxGroup("Design Settings", true, false, 0)]
		[ShowIf("DeactivatesGameobjectsWhenRunning", true)]
		private List<GameObject> GameobjectsToDeactivate;

		[SerializeField]
		[FoldoutGroup("Motion Settings", 0)]
		private bool OneWay;

		[SerializeField]
		[BoxGroup("Flag Settings", true, false, 0)]
		[OnValueChanged("DestinyFlagName", false)]
		protected string OnDestination;

		private Vector3 _origin;

		private Vector3 _destination;

		protected Vector2 OriginPosition;

		protected Vector2 DestinationPosition;

		private float _accumulatedTime;

		private Vector3 _velocity = Vector3.zero;

		private Collider2D _platformCollider;

		private bool _running;

		private SpriteRenderer _spriteRenderer;

		private const float VerticalOffset = 0.01f;

		private bool _gosDeactivated;

		private int _journeyCount;

		private Tweener tweener;

		private EventInstance _movementLoopFx;
	}
}
