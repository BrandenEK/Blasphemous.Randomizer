using System;
using HutongGames.PlayMaker;

namespace Tools.Playmaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Starts an audio tool.")]
	public class AudioToolStart : FsmStateAction
	{
		public override void OnEnter()
		{
			base.Finish();
		}

		public FsmGameObject gameObject;

		public FsmFloat fadeTime;

		public FsmFloat delay;
	}
}
