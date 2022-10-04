using System;
using System.Collections;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Level.Actionables
{
	public class DestroyableBridge : PersistentObject, IActionable
	{
		public bool Enabled { get; private set; }

		public bool Locked { get; set; }

		public void Use()
		{
			this.PlaySound();
			base.StartCoroutine(this.MoveToDestinationCoroutine());
		}

		private void Start()
		{
		}

		private void GetFinalPositionAndRotation()
		{
			this._targetDestination = this.Destination.position;
			this._targetRotation = this.Destination.rotation;
		}

		private void SetActivatedTransform()
		{
			this.childBridge.position = this._targetDestination;
			this.childBridge.rotation = this._targetRotation;
		}

		private IEnumerator MoveToDestinationCoroutine()
		{
			Vector2 originPos = this.childBridge.position;
			Vector2 targetPosition = this.Destination.position;
			float angle = this.Destination.localEulerAngles.z;
			this.childBridge.DOMove(targetPosition, this.interpolationSeconds, false).SetEase(this.translationCurve);
			this.childBridge.DOLocalRotate(new Vector3(0f, 0f, angle), this.interpolationSeconds, RotateMode.Fast).SetEase(this.rotationCurve);
			this.childBridge.GetComponentInChildren<Collider2D>().enabled = true;
			this.alreadyUsed = true;
			yield return null;
			yield break;
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
			{
				Debug.DrawLine(this.childBridge.position, this.Destination.position);
			}
		}

		public override PersistentManager.PersistentData GetCurrentPersistentState(string dataPath, bool fullSave)
		{
			DestroyableBridge.DestroyableBridgePersistenceData destroyableBridgePersistenceData = base.CreatePersistentData<DestroyableBridge.DestroyableBridgePersistenceData>();
			destroyableBridgePersistenceData.used = this.alreadyUsed;
			return destroyableBridgePersistenceData;
		}

		public override void SetCurrentPersistentState(PersistentManager.PersistentData data, bool isloading, string dataPath)
		{
			DestroyableBridge.DestroyableBridgePersistenceData destroyableBridgePersistenceData = (DestroyableBridge.DestroyableBridgePersistenceData)data;
			this.alreadyUsed = destroyableBridgePersistenceData.used;
			if (this.alreadyUsed)
			{
				this.GetFinalPositionAndRotation();
				this.SetActivatedTransform();
			}
		}

		public void PlaySound()
		{
			if (this._audioInstance.isValid())
			{
				this._audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			this._audioInstance = Core.Audio.CreateEvent(this.bridgeFallingSound, default(Vector3));
			this._audioInstance.start();
		}

		public Transform Destination;

		public float interpolationSeconds = 0.5f;

		public AnimationCurve translationCurve;

		public AnimationCurve rotationCurve;

		public bool alreadyUsed;

		public Transform childBridge;

		private Vector2 _targetDestination;

		private Quaternion _targetRotation;

		[SerializeField]
		[BoxGroup("Audio", true, false, 0)]
		[EventRef]
		private string bridgeFallingSound;

		private EventInstance _audioInstance;

		private class DestroyableBridgePersistenceData : PersistentManager.PersistentData
		{
			public DestroyableBridgePersistenceData(string id) : base(id)
			{
			}

			public bool used;
		}
	}
}
