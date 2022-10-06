using System;
using Framework.Managers;
using HutongGames.PlayMaker;

namespace Tools.PlayMaker.Action
{
	[ActionCategory("Blasphemous Action")]
	[Tooltip("Gets the name of the previous scene.")]
	public class GetPreviousSceneName : FsmStateAction
	{
		public override void Reset()
		{
			this.storeName = null;
		}

		public override void OnEnter()
		{
			this.storeName.Value = Core.LevelManager.GetLastSceneName();
			base.Finish();
		}

		[UIHint(10)]
		public FsmString storeName;
	}
}
