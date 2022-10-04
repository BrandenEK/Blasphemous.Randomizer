using System;
using HutongGames.PlayMaker;

namespace Tools.Playmaker2.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Plays a one shot on an area emitter.")]
	public class AreaEmitterOneShot : FsmStateAction
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
