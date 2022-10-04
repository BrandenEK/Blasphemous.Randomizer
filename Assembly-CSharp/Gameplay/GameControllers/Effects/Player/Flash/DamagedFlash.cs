using System;
using System.Collections.Generic;
using Framework.Util;
using UnityEngine;

namespace Gameplay.GameControllers.Effects.Player.Flash
{
	public class DamagedFlash : MonoBehaviour
	{
		private void Start()
		{
			this.pixel = new Texture2D(1, 1);
			this.color.a = this.startAlpha;
			this.pixel.SetPixel(0, 0, this.color);
			this.pixel.Apply();
		}

		private void Update()
		{
			DamagedFlash.FLASHSTATE flashstate = this.state;
			if (flashstate != DamagedFlash.FLASHSTATE.UP)
			{
				if (flashstate != DamagedFlash.FLASHSTATE.HOLD)
				{
					if (flashstate == DamagedFlash.FLASHSTATE.DOWN)
					{
						if (this.timer.UpdateAndTest() && !this.isTimerStore)
						{
							this.storeTimer(this.timer);
							this.isTimerStore = true;
						}
					}
				}
				else
				{
					this.isGetTimer = false;
					if (this.timer.UpdateAndTest())
					{
						this.state = DamagedFlash.FLASHSTATE.DOWN;
						if (!this.isGetTimer)
						{
							this.timer = this.getTimer(this.rampDownTime);
						}
					}
				}
			}
			else
			{
				this.isTimerStore = false;
				if (this.timer.UpdateAndTest())
				{
					this.state = DamagedFlash.FLASHSTATE.HOLD;
					if (!this.isGetTimer)
					{
						this.timer = this.getTimer(this.holdTime);
					}
				}
			}
		}

		private void SetPixelAlpha(float a)
		{
			this.color.a = a;
			this.pixel.SetPixel(0, 0, this.color);
			this.pixel.Apply();
		}

		public void OnGUI()
		{
			DamagedFlash.FLASHSTATE flashstate = this.state;
			if (flashstate != DamagedFlash.FLASHSTATE.UP)
			{
				if (flashstate == DamagedFlash.FLASHSTATE.DOWN)
				{
					this.SetPixelAlpha(Mathf.Lerp(this.maxAlpha, this.startAlpha, this.timer.Elapsed));
				}
			}
			else
			{
				this.SetPixelAlpha(Mathf.Lerp(this.startAlpha, this.maxAlpha, this.timer.Elapsed));
			}
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.pixel);
		}

		public void TookDamage()
		{
			this.timer = this.getTimer(this.rampUpTime);
			this.isGetTimer = false;
			this.state = DamagedFlash.FLASHSTATE.UP;
		}

		private Timer getTimer(float _holdTime)
		{
			Timer timer;
			if (this.timerList.Count > 0)
			{
				timer = this.timerList[this.timerList.Count - 1];
				timer.TotalTime = _holdTime;
				this.timerList.Remove(timer);
			}
			else
			{
				timer = new Timer(_holdTime);
			}
			return timer;
		}

		private void storeTimer(Timer _timer)
		{
			this.timerList.Add(_timer);
		}

		private void drainTimerPool()
		{
			if (this.timerList.Count >= 0)
			{
				this.timerList.Clear();
			}
		}

		public Color color = Color.red;

		public float holdTime = 0.5f;

		private bool isGetTimer;

		private bool isTimerStore;

		public float maxAlpha = 1f;

		private Texture2D pixel;

		public float rampDownTime = 0.5f;

		public float rampUpTime = 0.5f;

		public float startAlpha;

		private DamagedFlash.FLASHSTATE state;

		private Timer timer;

		private readonly List<Timer> timerList = new List<Timer>();

		private enum FLASHSTATE
		{
			OFF,
			UP,
			HOLD,
			DOWN
		}
	}
}
