using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Util;
using Rewired;
using Tools.DataContainer;
using UnityEngine;

public class RumbleSystem : SingletonSerialized<RumbleSystem>
{
	public void Update()
	{
		if (!this.RumblesEnabled || (this.activeRumbles.Count == 0 && !this.forceUpdate))
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		this.forceUpdate = false;
		foreach (RumbleSystem.ActiveRumble activeRumble in this.activeRumbles)
		{
			activeRumble.currentTime += Time.unscaledDeltaTime;
			if (activeRumble.currentTime >= activeRumble.rumble.duration)
			{
				if (!activeRumble.rumble.loop)
				{
					continue;
				}
				activeRumble.currentLoop++;
				if (activeRumble.rumble.loopCount <= 0 || activeRumble.currentLoop <= activeRumble.rumble.loopCount)
				{
					activeRumble.currentTime -= activeRumble.rumble.duration;
				}
			}
			float time = activeRumble.currentTime / activeRumble.rumble.duration;
			if (activeRumble.rumble.type == RumbleData.RumbleType.All || activeRumble.rumble.type == RumbleData.RumbleType.Left)
			{
				num += activeRumble.rumble.left.Evaluate(time);
			}
			if (activeRumble.rumble.type == RumbleData.RumbleType.All || activeRumble.rumble.type == RumbleData.RumbleType.Rigth)
			{
				if (activeRumble.rumble.type == RumbleData.RumbleType.All && activeRumble.rumble.sameCurve)
				{
					num2 += activeRumble.rumble.left.Evaluate(time);
				}
				else
				{
					num2 += activeRumble.rumble.right.Evaluate(time);
				}
			}
		}
		ReInput.players.GetPlayer(0).SetVibration(0, Mathf.Clamp(num, 0f, 1f));
		ReInput.players.GetPlayer(0).SetVibration(1, Mathf.Clamp(num2, 0f, 1f));
		this.activeRumbles.RemoveAll((RumbleSystem.ActiveRumble element) => element.currentTime >= element.rumble.duration);
	}

	public void ApplyRumble(RumbleData rumble)
	{
		RumbleSystem.ActiveRumble item = new RumbleSystem.ActiveRumble(rumble);
		this.activeRumbles.Add(item);
	}

	public void StopRumble(string name)
	{
		this.activeRumbles.RemoveAll((RumbleSystem.ActiveRumble element) => element.rumble.name == name);
		this.forceUpdate = true;
	}

	public void StopAllRumbles()
	{
		this.activeRumbles.Clear();
		this.forceUpdate = true;
	}

	public List<string> AppliedRumbles()
	{
		return (from element in this.activeRumbles
		select element.rumble.name).ToList<string>();
	}

	private List<RumbleSystem.ActiveRumble> activeRumbles = new List<RumbleSystem.ActiveRumble>();

	private bool forceUpdate;

	public bool RumblesEnabled = true;

	private class ActiveRumble
	{
		public ActiveRumble(RumbleData rumble)
		{
			this.rumble = rumble;
			this.currentTime = 0f;
			this.currentLoop = 0;
		}

		public RumbleData rumble { get; private set; }

		public float currentTime { get; set; }

		public int currentLoop { get; set; }
	}
}
