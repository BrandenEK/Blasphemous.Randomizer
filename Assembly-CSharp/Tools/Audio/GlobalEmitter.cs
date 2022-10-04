using System;
using Framework.FrameworkCore;
using Framework.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Audio
{
	[DefaultExecutionOrder(1)]
	public class GlobalEmitter : AudioTool
	{
		protected override void BaseAwake()
		{
			base.BaseAwake();
			base.IsEmitter = true;
			LevelManager.OnLevelLoaded += this.OnLevelLoaded;
		}

		protected override void BaseDestroy()
		{
			base.BaseDestroy();
			LevelManager.OnLevelLoaded -= this.OnLevelLoaded;
		}

		private void OnLevelLoaded(Level oldLevel, Level newLevel)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
		}

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[Range(0f, 10f)]
		private float startFadeTime;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		[Range(0f, 10f)]
		private float startDelay;

		[SerializeField]
		[BoxGroup("Audio Settings", true, false, 0)]
		private AudioParam[] parameters = new AudioParam[0];
	}
}
