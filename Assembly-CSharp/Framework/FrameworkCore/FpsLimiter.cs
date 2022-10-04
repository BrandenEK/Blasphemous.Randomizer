using System;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.FrameworkCore
{
	public class FpsLimiter : MonoBehaviour
	{
		private void Start()
		{
			this.TargetFrameRate = 60;
			this.UpdateTargetDeltaTime();
			this.OldTime = Time.realtimeSinceStartup;
			FpsLimiter.SetTargetFrameRate(60);
		}

		private void UpdateTargetDeltaTime()
		{
			this.TargetDeltaTime = 1f / (float)this.TargetFrameRate;
		}

		private void Update()
		{
			if (QualitySettings.vSyncCount == 0)
			{
				FpsLimiter.SetTargetFrameRate(60);
			}
		}

		private void LateUpdate()
		{
			if (QualitySettings.vSyncCount == 0)
			{
				return;
			}
			this.ForceDeltaTimeDelay();
		}

		private static void SetTargetFrameRate(int targetFrameRate)
		{
			if (Application.targetFrameRate != targetFrameRate)
			{
				Application.targetFrameRate = targetFrameRate;
			}
		}

		private void ForceDeltaTimeDelay()
		{
			this.CurTime = Time.realtimeSinceStartup;
			this.TimeTaken = this.CurTime - this.OldTime;
			if (this.TimeTaken < this.TargetDeltaTime)
			{
				Thread.Sleep((int)(1000f * (this.TargetDeltaTime - this.TimeTaken)));
			}
			this.OldTime = Time.realtimeSinceStartup;
		}

		private float OldTime;

		private float TargetDeltaTime;

		private float CurTime;

		private float TimeTaken;

		[PropertyRange(1.0, 100.0)]
		[OnValueChanged("UpdateTargetDeltaTime", false)]
		public int TargetFrameRate = 60;
	}
}
