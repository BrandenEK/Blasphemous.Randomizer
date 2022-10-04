using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FMOD.Studio;
using Framework.Managers;
using UnityEngine;

namespace Tools.Level.Interactables
{
	[RequireComponent(typeof(Animator))]
	public class ExecutionAnimationEvents : MonoBehaviour
	{
		private void Start()
		{
			this._execution = base.GetComponentInParent<Execution>();
			this._animator = base.GetComponent<Animator>();
			Execution execution = this._execution;
			execution.OnNormalTime = (Core.SimpleEvent)Delegate.Combine(execution.OnNormalTime, new Core.SimpleEvent(this.OnNormalTime));
			Execution execution2 = this._execution;
			execution2.OnSlowMotion = (Core.SimpleEvent)Delegate.Combine(execution2.OnSlowMotion, new Core.SimpleEvent(this.OnSlowMotion));
			this.CreateExecutionSoundEvent();
		}

		private void SetAnimatorSpeed(float speed, float lapse)
		{
			DOTween.To(() => this._animator.speed, delegate(float x)
			{
				this._animator.speed = x;
			}, speed, lapse).SetUpdate(true).SetEase(Ease.Linear);
		}

		private void OnSlowMotion()
		{
		}

		private void OnNormalTime()
		{
		}

		public void DoSlowMotion()
		{
			if (this._execution == null)
			{
				return;
			}
			this.SetAnimatorSpeed(3f, 0.5f);
			this._execution.DoSlowmotion();
		}

		public void StopSlowMotion()
		{
			if (this._execution == null)
			{
				return;
			}
			this._execution.StopSlowMotion();
			this.SetAnimatorSpeed(1f, 0f);
		}

		public void ZoomIn()
		{
			if (this._execution == null)
			{
				return;
			}
			this._execution.CameraZoomIn();
		}

		public void ZoomOut()
		{
			if (this._execution == null)
			{
				return;
			}
			this._execution.CameraZoomOut();
		}

		public void CamShake()
		{
			Core.Logic.CameraManager.ProCamera2DShake.ShakeUsingPreset("SimpleHit");
		}

		private void CreateExecutionSoundEvent()
		{
			try
			{
				this._executionSound = Core.Audio.CreateEvent(this._execution.ActivationSound, default(Vector3));
				this._executionSound.getParameter("Hits", out this._hitParam);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		public void PlayExecutionSound()
		{
			if (!this._executionSound.isValid())
			{
				return;
			}
			this._executionSound.start();
		}

		public void StopExecutionSound()
		{
			if (!this._executionSound.isValid())
			{
				return;
			}
			this._executionSound.release();
		}

		public void UpdateExecutionSoundEventParam(float param)
		{
			if (!this._hitParam.isValid())
			{
				return;
			}
			this._hitParam.setValue(param);
		}

		public void Rumble()
		{
			Core.Logic.Penitent.Rumble.UsePreset("SimpleHit");
		}

		private Execution _execution;

		private Animator _animator;

		private EventInstance _executionSound;

		private ParameterInstance _hitParam;
	}
}
