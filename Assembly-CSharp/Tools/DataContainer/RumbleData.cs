using System;
using Framework.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.DataContainer
{
	[CreateAssetMenu(fileName = "rumble", menuName = "Blasphemous/Rumble")]
	public class RumbleData : ScriptableObject
	{
		private bool ShowRight()
		{
			return (this.type == RumbleData.RumbleType.All && !this.sameCurve) || this.type == RumbleData.RumbleType.Rigth;
		}

		private bool ShowLeft()
		{
			return this.type == RumbleData.RumbleType.All || this.type == RumbleData.RumbleType.Left;
		}

		private bool ShowSameCurve()
		{
			return this.type == RumbleData.RumbleType.All;
		}

		[HideInEditorMode]
		[Button(ButtonSizes.Small)]
		private void TestRummble()
		{
			SingletonSerialized<RumbleSystem>.Instance.ApplyRumble(this);
		}

		[HideInEditorMode]
		[Button(ButtonSizes.Small)]
		private void StopRummble()
		{
			SingletonSerialized<RumbleSystem>.Instance.StopAllRumbles();
		}

		public float duration = 1f;

		[ShowIf("ShowLeft", true)]
		public AnimationCurve left;

		public bool loop;

		[ShowIf("loop", true)]
		[Range(0f, 99f)]
		public int loopCount;

		[ShowIf("ShowRight", true)]
		public AnimationCurve right;

		[ShowIf("ShowSameCurve", true)]
		public bool sameCurve;

		public RumbleData.RumbleType type;

		public enum RumbleType
		{
			All,
			Left,
			Rigth
		}
	}
}
