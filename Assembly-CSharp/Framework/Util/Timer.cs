using System;
using UnityEngine;

namespace Framework.Util
{
	public class Timer
	{
		public Timer(float timeToCountInSec)
		{
			this._totalTime = timeToCountInSec;
		}

		public float TotalTime
		{
			get
			{
				return this._totalTime;
			}
			set
			{
				this._totalTime = value;
			}
		}

		public bool UpdateAndTest()
		{
			this._timeElapsed += Time.unscaledDeltaTime;
			return this._timeElapsed >= this._totalTime;
		}

		public float Elapsed
		{
			get
			{
				return Mathf.Clamp(this._timeElapsed / this._totalTime, 0f, 1f);
			}
		}

		private float _timeElapsed;

		private float _totalTime;
	}
}
