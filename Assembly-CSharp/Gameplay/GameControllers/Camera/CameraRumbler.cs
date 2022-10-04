using System;
using Com.LuisPedroFonseca.ProCamera2D;
using Framework.Managers;
using Tools.DataContainer;
using UnityEngine;

namespace Gameplay.GameControllers.Camera
{
	public class CameraRumbler : MonoBehaviour
	{
		private float RumbleCoolDown { get; set; }

		private void Start()
		{
			ProCamera2DShake proCamera2DShake = Core.Logic.CameraManager.ProCamera2DShake;
			proCamera2DShake.OnShakeStarted = (Action<float>)Delegate.Combine(proCamera2DShake.OnShakeStarted, new Action<float>(this.OnShakeStarted));
			this.RumbleCoolDown = 0f;
		}

		private void Update()
		{
			this.RumbleCoolDown -= Time.deltaTime;
		}

		private void OnShakeStarted(float duration)
		{
			if (this.RumbleCoolDown > 0f)
			{
				return;
			}
			if (duration <= this.ShortRumble.duration)
			{
				if (this.ShortRumble != null)
				{
					if (Core.Input.AppliedRumbles().Count == 0)
					{
						Core.Input.ApplyRumble(this.ShortRumble);
						this.RumbleCoolDown = this.ShortRumble.duration;
					}
				}
				else
				{
					Debug.LogError("CameraRumbler::OnShakeStarted: ShortRumble is null!");
				}
			}
			else if (duration <= this.NormalRumble.duration)
			{
				if (this.NormalRumble != null)
				{
					if (Core.Input.AppliedRumbles().Count == 0)
					{
						Core.Input.ApplyRumble(this.NormalRumble);
						this.RumbleCoolDown = this.NormalRumble.duration;
					}
				}
				else
				{
					Debug.LogError("CameraRumbler::OnShakeStarted: NormalRumble is null!");
				}
			}
			else if (this.LongRumble != null)
			{
				if (Core.Input.AppliedRumbles().Count == 0)
				{
					Core.Input.ApplyRumble(this.LongRumble);
					this.RumbleCoolDown = this.LongRumble.duration;
				}
			}
			else
			{
				Debug.LogError("CameraRumbler::OnShakeStarted: LongRumble is null!");
			}
		}

		private void OnDestroy()
		{
			if (Core.Logic.CameraManager)
			{
				ProCamera2DShake proCamera2DShake = Core.Logic.CameraManager.ProCamera2DShake;
				proCamera2DShake.OnShakeStarted = (Action<float>)Delegate.Remove(proCamera2DShake.OnShakeStarted, new Action<float>(this.OnShakeStarted));
			}
		}

		public RumbleData ShortRumble;

		public RumbleData NormalRumble;

		public RumbleData LongRumble;
	}
}
