using System;
using Framework.Managers;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[HutongGames.PlayMaker.Tooltip("Respawn from current Miriam course.")]
	public class RespawnMiriam : FsmStateAction
	{
		public override void Reset()
		{
			this.UseFade = new FsmBool();
			this.UseFade.UseVariable = false;
			this.UseFade.Value = true;
			this.FadeColor = new FsmColor();
			this.FadeColor.UseVariable = false;
			this.FadeColor.Value = Color.white;
		}

		public override void OnEnter()
		{
			bool useFade = this.UseFade == null || this.UseFade.Value;
			Color value = (this.FadeColor == null) ? Color.black : this.FadeColor.Value;
			Core.SpawnManager.RespawnMiriamSameLevel(useFade, new Color?(value));
			base.Finish();
		}

		public FsmBool UseFade;

		public FsmColor FadeColor;
	}
}
