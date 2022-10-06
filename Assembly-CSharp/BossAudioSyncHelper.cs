using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AOT;
using FMOD;
using FMOD.Studio;
using Gameplay.GameControllers.Bosses.BossFight;
using UnityEngine;

public class BossAudioSyncHelper : MonoBehaviour
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action OnBar;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<string> OnMarker;

	private void Awake()
	{
		this.bossfightAudio = base.GetComponent<BossFightAudio>();
		this.timelineInfo = new BossAudioSyncHelper.TimelineInfo();
		if (this.bossfightAudio)
		{
			this.bossfightAudio.OnBossMusicStarts += this.BossfightAudio_OnBossMusicStarts;
		}
	}

	private void BossfightAudio_OnBossMusicStarts()
	{
		this.bossfightAudio.OnBossMusicStarts -= this.BossfightAudio_OnBossMusicStarts;
		this.InitCallback();
	}

	private void InitCallback()
	{
		this.beatCallback = new EVENT_CALLBACK(BossAudioSyncHelper.BeatEventCallback);
		this.musicInstance = this.bossfightAudio.GetCurrentMusicInstance();
		this.timelineHandle = GCHandle.Alloc(this.timelineInfo, GCHandleType.Pinned);
		this.musicInstance.setUserData(GCHandle.ToIntPtr(this.timelineHandle));
		this.musicInstance.setCallback(this.beatCallback, 6144);
	}

	private void OnDestroy()
	{
		this.musicInstance.setUserData(IntPtr.Zero);
		this.musicInstance.stop(1);
		this.musicInstance.release();
		this.timelineHandle.Free();
	}

	private void OnGUI()
	{
	}

	private void Update()
	{
		if (this.LastBar != this.timelineInfo.currentMusicBar)
		{
			this.LastBar = this.timelineInfo.currentMusicBar;
			if (this.OnBar != null)
			{
				this.OnBar();
			}
		}
		if (this.timelineInfo.updatedMarker)
		{
			this.lastMarker = this.timelineInfo.lastMarker;
			this.lastMarketBar = this.LastBar;
			if (this.OnMarker != null)
			{
				this.OnMarker(this.lastMarker);
			}
			this.timelineInfo.updatedMarker = false;
		}
	}

	[MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
	private static RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, EventInstance instance, IntPtr parameterPtr)
	{
		IntPtr intPtr;
		RESULT userData = instance.getUserData(ref intPtr);
		if (userData != null)
		{
			Debug.LogError("Timeline Callback error: " + userData);
		}
		else if (intPtr != IntPtr.Zero)
		{
			BossAudioSyncHelper.TimelineInfo timelineInfo = (BossAudioSyncHelper.TimelineInfo)GCHandle.FromIntPtr(intPtr).Target;
			if (type != 4096)
			{
				if (type == 2048)
				{
					timelineInfo.lastMarker = ((TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES))).name;
					timelineInfo.updatedMarker = true;
				}
			}
			else
			{
				timelineInfo.currentMusicBar = ((TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES))).bar;
			}
		}
		return 0;
	}

	private BossAudioSyncHelper.TimelineInfo timelineInfo;

	private GCHandle timelineHandle;

	private EVENT_CALLBACK beatCallback;

	private EventInstance musicInstance;

	[HideInInspector]
	public BossFightAudio bossfightAudio;

	public int LastBar;

	private string lastMarker = string.Empty;

	private int lastMarketBar;

	[StructLayout(LayoutKind.Sequential)]
	private class TimelineInfo
	{
		public int currentMusicBar;

		public StringWrapper lastMarker = default(StringWrapper);

		public bool updatedMarker;
	}
}
