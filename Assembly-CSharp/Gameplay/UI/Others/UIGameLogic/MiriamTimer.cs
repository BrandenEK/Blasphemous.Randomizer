using System;
using Framework.Managers;
using Framework.Util;
using Gameplay.GameControllers.Penitent;
using Sirenix.OdinInspector;
using Tools.DataContainer;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Others.UIGameLogic
{
	public class MiriamTimer : SerializedMonoBehaviour
	{
		public static Core.SimpleEvent OnTimerRunOut { get; internal set; }

		private void Start()
		{
			this.Text.color = this.NotStartedColor;
			this.Hide();
		}

		private void Update()
		{
			if (Singleton<Core>.Instance == null || !Core.ready || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (this.isTimerRunning)
			{
				this.UpdateRemainingTime();
			}
			this.Text.text = this.RunDurationInString(this.remainingTime);
		}

		private void UpdateRemainingTime()
		{
			if (this.remainingTime <= 0f)
			{
				return;
			}
			this.remainingTime -= Time.deltaTime;
			if (this.remainingTime < 0f)
			{
				this.remainingTime = 0f;
				this.StopTimer(false);
				Core.Audio.PlayOneShot(this.failEvent, default(Vector3));
			}
			else if (this.remainingTime < 10f)
			{
				if (!this.lastTenSecondsEventPlayed)
				{
					this.lastTenSecondsEventPlayed = true;
					Core.Audio.PlayNamedSound("event:/SFX/UI/TimerLastSeconds", this.lastTenSecondsEvent);
				}
				if (Core.Input.AppliedRumbles().Count == 0)
				{
					Core.Input.ApplyRumble(this.Rumble);
				}
			}
		}

		public string RunDurationInString(float remainingTime)
		{
			int num = Mathf.FloorToInt(remainingTime) / 60;
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)remainingTime);
			int seconds = timeSpan.Seconds;
			int num2 = timeSpan.Milliseconds / 10;
			return string.Format("{0:D2} : {1:D2} : {2:D2}", num, seconds, num2);
		}

		public void StartTimer()
		{
			this.isTimerRunning = true;
			this.Text.color = this.RunningColor;
			this.lastTenSecondsEventPlayed = false;
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Combine(penitent.OnDead, new Core.SimpleEvent(this.OnPlayerDead));
		}

		private void OnPlayerDead()
		{
			this.StopTimer(false);
		}

		public void StopTimer(bool completed)
		{
			this.isTimerRunning = false;
			this.Text.color = ((!completed) ? this.FailedColor : this.CompletedColor);
			Core.Audio.StopNamedSound(this.lastTenSecondsEvent, 0);
			if (this.remainingTime == 0f)
			{
				PlayMakerFSM.BroadcastEvent("ON TIMER RUNS OUT");
				if (MiriamTimer.OnTimerRunOut != null)
				{
					MiriamTimer.OnTimerRunOut();
				}
			}
			Penitent penitent = Core.Logic.Penitent;
			penitent.OnDead = (Core.SimpleEvent)Delegate.Remove(penitent.OnDead, new Core.SimpleEvent(this.OnPlayerDead));
		}

		public void SetTargetTime(float targetTime)
		{
			this.targetTime = targetTime;
			this.remainingTime = targetTime;
		}

		public void Show()
		{
			this.Text.color = this.NotStartedColor;
			UIController.instance.HidePurgePoints();
			base.gameObject.SetActive(true);
		}

		public void Hide()
		{
			this.Text.color = this.NotStartedColor;
			base.gameObject.SetActive(false);
			UIController.instance.ShowPurgePoints();
		}

		public Text Text;

		public Color NotStartedColor;

		public Color CompletedColor;

		public Color FailedColor;

		public Color RunningColor;

		public RumbleData Rumble;

		private float targetTime;

		private float remainingTime;

		private bool isTimerRunning;

		private string lastTenSecondsEvent = "event:/SFX/UI/TimerLastSeconds";

		private string failEvent = "event:/SFX/UI/TimeFail";

		private bool lastTenSecondsEventPlayed;
	}
}
