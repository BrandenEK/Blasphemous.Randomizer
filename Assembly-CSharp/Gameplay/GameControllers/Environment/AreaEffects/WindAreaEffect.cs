using System;
using System.Collections.Generic;
using CreativeSpore.SmartColliders;
using DG.Tweening;
using FMOD.Studio;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using Tools.Audio;
using UnityEngine;

namespace Gameplay.GameControllers.Environment.AreaEffects
{
	public class WindAreaEffect : AreaEffect
	{
		public float CurrentWindForce { get; private set; }

		private AmbientMusicSettings AmbientMusic { get; set; }

		protected override void OnAwake()
		{
			base.OnAwake();
			this.Controllers = new List<PlatformCharacterController>();
			this.RigidBodies = new List<Rigidbody2D>();
		}

		protected override void OnStart()
		{
			base.OnStart();
			this.CurrentRandomWindDirection = (((double)Random.value <= 0.5) ? Vector3.left : Vector3.right);
			this.CurrentAlternateWindDirection = Vector3.right;
			this.AmbientMusic = Core.Audio.Ambient;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (this.useTimers)
			{
				if (this.CurrentWindPauseTime < this.WindPauseTime)
				{
					this.CurrentWindPauseTime += Time.deltaTime;
					return;
				}
				if (this.CurrentWindForce < this.MaxWindForce && !this.IsWindAcceleration)
				{
					this.SetWindForce(this.MaxWindForce);
				}
				if (this.CurrentWindGustTime < this.WindGustTime && this.CurrentWindForce >= this.MaxWindForce)
				{
					this.CurrentWindGustTime += Time.deltaTime;
					return;
				}
				if (this.CurrentWindForce > 0f && !this.IsWindAcceleration)
				{
					this.SetWindForce(0f);
				}
				this.SetAmbientWindParam(this.NormalizedWindForce);
			}
		}

		public void SetMaxForce()
		{
			this.SetWindForce(this.MaxWindForce);
		}

		public void SetMinForce()
		{
			this.SetWindForce(0f);
		}

		protected override void OnEnterAreaEffect(Collider2D other)
		{
			base.OnEnterAreaEffect(other);
			PlatformCharacterController componentInChildren = other.transform.root.GetComponentInChildren<PlatformCharacterController>();
			Rigidbody2D componentInChildren2 = other.transform.root.GetComponentInChildren<Rigidbody2D>();
			if (!this.Controllers.Contains(componentInChildren))
			{
				this.Controllers.Add(componentInChildren);
			}
			if (!this.RigidBodies.Contains(componentInChildren2))
			{
				this.RigidBodies.Add(componentInChildren2);
			}
			this._penitent = other.transform.root.GetComponentInChildren<Penitent>();
		}

		protected override void OnStayAreaEffect()
		{
			base.OnStayAreaEffect();
			if (this._penitent == null || this.Controllers.Count == 0 || this.RigidBodies.Count == 0)
			{
				this._penitent = Core.Logic.Penitent;
				PlatformCharacterController componentInChildren = this._penitent.transform.root.GetComponentInChildren<PlatformCharacterController>();
				Rigidbody2D componentInChildren2 = this._penitent.transform.root.GetComponentInChildren<Rigidbody2D>();
				if (!this.Controllers.Contains(componentInChildren))
				{
					this.Controllers.Add(componentInChildren);
				}
				if (!this.RigidBodies.Contains(componentInChildren2))
				{
					this.RigidBodies.Add(componentInChildren2);
				}
			}
			if (this._penitent != null)
			{
				if (this._penitent.ThrowBack.IsThrown)
				{
					return;
				}
				if (this._penitent.IsGrabbingCliffLede || this._penitent.IsClimbingCliffLede || this._penitent.IsClimbingLadder || this._penitent.IsStickedOnWall)
				{
					this.SetDynamicMode(this._penitent.RigidBody, false);
					return;
				}
			}
			foreach (PlatformCharacterController platformCharacterController in this.Controllers)
			{
				this.ApplyWindForce(platformCharacterController);
				this.ApplyPhysicsForce(platformCharacterController.IsGrounded);
			}
		}

		protected override void OnExitAreaEffect(Collider2D other)
		{
			base.OnExitAreaEffect(other);
			PlatformCharacterController componentInChildren = other.transform.root.GetComponentInChildren<PlatformCharacterController>();
			Rigidbody2D componentInChildren2 = other.transform.root.GetComponentInChildren<Rigidbody2D>();
			Penitent componentInParent = other.GetComponentInParent<Penitent>();
			if (!componentInParent || !componentInParent.ThrowBack.IsThrown)
			{
				this.SetDynamicMode(componentInChildren2, false);
			}
			if (this.Controllers.Contains(componentInChildren))
			{
				this.Controllers.Remove(componentInChildren);
			}
			if (this.RigidBodies.Contains(componentInChildren2))
			{
				this.RigidBodies.Remove(componentInChildren2);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.Controllers.Clear();
			this.SetAmbientWindParam(0f);
		}

		public void ApplyWindForce(PlatformCharacterController controller)
		{
			Vector3 windForce = this.GetWindForce();
			controller.PlatformCharacterPhysics.AddAcceleration(windForce);
		}

		public void ApplyPhysicsForce(bool apply)
		{
			foreach (Rigidbody2D rigidbody2D in this.RigidBodies)
			{
				if (apply)
				{
					this.SetDynamicMode(rigidbody2D, false);
				}
				else
				{
					this.SetDynamicMode(rigidbody2D, true);
					rigidbody2D.AddForce(this.GetWindForce().normalized * this.WindImpulse, 0);
					rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, 3.5f);
				}
			}
		}

		private void SetDynamicMode(Rigidbody2D rigidBody, bool dynamicMode)
		{
			if (dynamicMode)
			{
				rigidBody.isKinematic = false;
				rigidBody.gravityScale = 0f;
				rigidBody.mass = 1f;
			}
			else
			{
				rigidBody.mass = 10f;
				rigidBody.velocity = Vector2.zero;
				rigidBody.gravityScale = 3f;
				rigidBody.isKinematic = true;
			}
		}

		public Vector3 GetWindForce()
		{
			Vector3 result;
			switch (this.WindDirection)
			{
			case WindAreaEffect.Direction.Right:
				result = Vector3.right * this.CurrentWindForce;
				break;
			case WindAreaEffect.Direction.Left:
				result = Vector3.left * this.CurrentWindForce;
				break;
			case WindAreaEffect.Direction.Random:
				result = this.CurrentRandomWindDirection * this.CurrentWindForce;
				break;
			case WindAreaEffect.Direction.Alternate:
				result = this.CurrentAlternateWindDirection * this.CurrentWindForce;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return result;
		}

		private void SetWindForce(float finalWindForce)
		{
			TweenSettingsExtensions.SetEase<Tweener>(TweenSettingsExtensions.OnComplete<Tweener>(TweenSettingsExtensions.OnStart<Tweener>(DOTween.To(delegate(float x)
			{
				this.CurrentWindForce = x;
			}, this.CurrentWindForce, finalWindForce, this.WindAccelerationTime), new TweenCallback(this.OnAccelerationWindForce)), new TweenCallback(this.OnStopAccelerationWindForce)), 3);
		}

		private void OnAccelerationWindForce()
		{
			this.IsWindAcceleration = true;
		}

		private void OnStopAccelerationWindForce()
		{
			this.IsWindAcceleration = false;
			if (this.CurrentWindForce > 0f)
			{
				return;
			}
			this.CurrentWindGustTime = 0f;
			this.CurrentWindPauseTime = 0f;
			this.CurrentRandomWindDirection = (((double)Random.value < 0.5) ? Vector3.left : Vector3.right);
			this.CurrentAlternateWindDirection = ((!(this.CurrentAlternateWindDirection == Vector3.right)) ? Vector3.right : Vector3.left);
		}

		public void SetAmbientWindParam(float value)
		{
			if (this.AmbientMusic == null)
			{
				return;
			}
			EventInstance audioInstance = this.AmbientMusic.AudioInstance;
			if (!audioInstance.isValid())
			{
				return;
			}
			ParameterInstance parameterInstance;
			audioInstance.getParameter("Wind", ref parameterInstance);
			if (parameterInstance.isValid())
			{
				parameterInstance.setValue(value);
			}
		}

		private float NormalizedWindForce
		{
			get
			{
				return this.CurrentWindForce / this.MaxWindForce;
			}
		}

		protected Vector3 CurrentRandomWindDirection;

		protected Vector3 CurrentAlternateWindDirection;

		[FoldoutGroup("Wind Settings", 0)]
		public WindAreaEffect.Direction WindDirection;

		[FoldoutGroup("Wind Settings", 0)]
		public float MaxWindForce;

		[FoldoutGroup("Wind Settings", 0)]
		[Tooltip("Time to reach max force")]
		public float WindAccelerationTime;

		[FoldoutGroup("Wind Settings", 0)]
		[Tooltip("Wind impulse in mid-air")]
		[Range(1f, 50f)]
		public float WindImpulse = 10f;

		[FoldoutGroup("Wind Settings", 0)]
		public bool useTimers = true;

		[FoldoutGroup("Wind Settings", 0)]
		[ShowIf("useTimers", true)]
		public float WindGustTime;

		[FoldoutGroup("Wind Settings", 0)]
		[ShowIf("useTimers", true)]
		public float WindPauseTime;

		protected bool IsWindAcceleration;

		protected float CurrentWindGustTime;

		protected float CurrentWindPauseTime;

		protected List<PlatformCharacterController> Controllers;

		protected List<Rigidbody2D> RigidBodies;

		protected Penitent _penitent;

		public enum Direction
		{
			Right,
			Left,
			Random,
			Alternate
		}
	}
}
