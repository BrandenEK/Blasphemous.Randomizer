using System;
using Framework.Managers;
using UnityEngine;

namespace Gameplay.GameControllers.Entities
{
	public class MotionLerper : MonoBehaviour
	{
		public bool IsLerping { get; private set; }

		public void StartLerping(Vector3 dir)
		{
			if (this.OnLerpStart != null)
			{
				this.OnLerpStart();
			}
			this.timeStartedLerping = Time.time;
			Vector3 vector;
			vector..ctor(this.motionObject.position.x, this.motionObject.position.y, this.motionObject.position.z);
			this.startPosition = vector;
			this.endPosition = vector + dir * this.distanceToMove;
			if (!this.IsLerping)
			{
				this.IsLerping = true;
			}
		}

		public void StopLerping()
		{
			if (this.IsLerping)
			{
				this.IsLerping = !this.IsLerping;
			}
			if (this.OnLerpStop != null)
			{
				this.OnLerpStop();
			}
		}

		private void Update()
		{
			if (!this.IsLerping)
			{
				return;
			}
			float num = Time.time - this.timeStartedLerping;
			float num2 = num / this.TimeTakenDuringLerp;
			this.motionObject.position = Vector3.Lerp(this.startPosition, this.endPosition, this.speedCurve.Evaluate(num2));
			if (num2 >= 1f)
			{
				this.IsLerping = false;
				if (this.OnLerpStop != null)
				{
					this.OnLerpStop();
				}
			}
		}

		public Core.SimpleEvent OnLerpStart;

		public Core.SimpleEvent OnLerpStop;

		[Header("Motion Params")]
		[Tooltip("The transform used to move the game object")]
		[SerializeField]
		private Transform motionObject;

		[Tooltip("The time taken to move from the start to finish positions during Lerp")]
		[Range(0.01f, 10f)]
		public float TimeTakenDuringLerp = 1f;

		[Tooltip("How far the object should move when along the X axis when the lerper is fired")]
		public float distanceToMove = 10f;

		[Tooltip("The behaviour of the lerp acceleration")]
		public AnimationCurve speedCurve;

		private Vector3 startPosition;

		private Vector3 endPosition;

		private float timeStartedLerping;
	}
}
